/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomStartDay
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private static ModConfig config;
        static int dayOfMonth;
        static Season season = Season.Spring;
        static Season[] allowedSeasons = Array.Empty<Season>();
        
        static bool needWheatSeeds = false;
        static bool isWinter28 = false;
        string errorString;
        static IModHelper modHelper;

        public override void Entry(IModHelper helper)
        {
            
            config = this.Helper.ReadConfig<ModConfig>();
            modHelper = helper;

            // run when disableAll is false or verification failed
            if (!config.DisableAll && Verification(out errorString))
            {
                helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
                helper.Events.Content.AssetRequested += this.Content_AssetRequested;
                helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
                helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
                helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;

                var harmony = new Harmony(this.ModManifest.UniqueID);
                string tipMethodName = Helper.Reflection.GetMethod(new TV(), "getTodaysTip").MethodInfo.Name;

                harmony.Patch(
                    original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.createdNewCharacter)),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Harmony_SetDateForIntro))
                    );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Intro), nameof(Intro.createBeginningOfLevel)),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(Harmony_ChangeIntroSeason))
                    );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addQuest)),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(Harmony_Quest6ToWheatQuest))
                    );
                harmony.Patch(
                    original: AccessTools.Method(typeof(TV), tipMethodName),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(Harmony_ChangeTodaysTip))
                    );
            }
            else
            {
                helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            }
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Initialize();
        }

        // run when disabled all only
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (config.DisableAll)
                Monitor.Log("This mod is DISABLED. To enable, fix config and re-launch the game.", LogLevel.Debug);
            else if (errorString != "")
                Monitor.Log(errorString, LogLevel.Error);
        }

        // EVENTS
        private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            // randomize: after character created(Harmony)

            if (e.NewStage == LoadStage.CreatedInitialLocations)
            {
                Apply();
                if (config.UseWheatSeeds)
                {
                    // set seed season and put seasonal seeds
                    needWheatSeeds = NeedToGiveWheatSeeds();
                    PutSeasonalSeeds(needWheatSeeds);
                }
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            // if player moves on winter 28th(=starts on spring 1), return to year 1
            if (isWinter28)
            {
                --Game1.year;
                isWinter28 = false;
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            // problem fix: first day, clear mailbox and add willy's mail to tomorrow's mail
            if (Game1.stats.DaysPlayed == 1)
            {
                Game1.mailbox.Clear();
                Game1.addMailForTomorrow("spring_2_1");
            }

            // allow to receive greenraingus mail even if you are not in year 1
            if (Game1.isGreenRain && !Game1.player.hasOrWillReceiveMail("GreenRainGus"))
                Game1.mailbox.Add("GreenRainGus");

            // When an email that should be received last year exists and not received yet
            Dictionary<string, string> mailData = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
            string lastYearMailName = Game1.currentSeason + "_" + Game1.dayOfMonth + "_" + (Game1.year - 1);
            if (mailData.ContainsKey(lastYearMailName))
            {
                if (!Game1.player.hasOrWillReceiveMail(lastYearMailName)) {
                    // add last year mail and remove this year mail
                    Game1.mailbox.Add(lastYearMailName);
                    Game1.mailbox.Remove(season + "_" + Game1.dayOfMonth + "_" + Game1.year);
                }
            }
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // sign and roadside
            if (e.NameWithoutLocale.IsEquivalentTo("Minigames/Intro"))
            {
                if (season != Season.Spring)
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsImage();
                        Texture2D sourceImage = this.Helper.ModContent.Load<Texture2D>("assets/intro_" + season.ToString().ToLower() + ".png");

                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(0, 176, 48, 80)), sourceArea: (new Rectangle(0, 0, 48, 80)));
                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(48, 224, 64, 16)), sourceArea: (new Rectangle(48, 48, 64, 16)));
                    });
                }
            }

            // edit quest #6
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests"))
            {
                string cropWord = GetWheatWord(out string parsnipWord);
                e.Edit(asset =>
                {
                    // add quest
                    var data = asset.AsDictionary<string, string>().Data;
                    data.TryGetValue("6", out string questString);
                    StringBuilder stringBuilder = new(questString);
                    stringBuilder = stringBuilder.Replace(parsnipWord, cropWord).Replace("24", "262");
                    //data.Add("idermailer.RandomStartDay.harvestWheat", stringBuilder.ToString());
                    data["idermailer.RandomStartDay.harvestWheat"] = stringBuilder.ToString();
                });
            }
        }

        // METHODS
        private static bool Verification(out string errorString)
        {
            // if AllowSpringSummerFallWinter is all false, verification fails.
            for (var i = 0; i < config.AllowSpringSummerFallWinter.Length; i++)
            {
                if (config.AllowSpringSummerFallWinter[i])
                {
                    errorString = "";
                    return true;
                }
            }
            errorString = "All seasons are not allowed. Please allow at least one season.\nThis mod will be DISABLED. To enable, fix config and re-launch the game.";
            return false;
        }

        private static void Initialize()
        {
            dayOfMonth = 0;
            season = Season.Spring;
            allowedSeasons = Array.Empty<Season>();

            isWinter28 = false;
            needWheatSeeds = false;
        }

        public static void Randomize(bool useLegacyRandom)
        {
            // get random seed
            Random random;
            if (useLegacyRandom)
            {
                // if use legacy random, other random options are disabled
                random = new((int)Game1.startingGameSeed);
                config.AlwaysStartAt1st = false;
                config.AvoidFestivalDay = false;
                config.AllowSpringSummerFallWinter = Enumerable.Repeat(true, 4).ToArray();
            }
            else
                random = new();
            
            // get allowed seasons
            for (var i = 0; i < config.AllowSpringSummerFallWinter.Length; i++)
            {
                if (config.AllowSpringSummerFallWinter[i])
                {
                    allowedSeasons = allowedSeasons.AddToArray((Season)i);
                }
            }

            // randomize
            // if use legacy random, other random options are disabled,
            // so just set date.
            if (useLegacyRandom)
            {
                season = (Season)random.Next(4);
                dayOfMonth = random.Next(28) + 1;
            }
            // if not, randomize until it doesn't conflict with config.
            else
            {
                bool conflicts;
                SDate tomorrow;

                do
                {
                    conflicts = false;

                    season = (Season)random.Next(4);
                    if (!config.AlwaysStartAt1st)
                        dayOfMonth = random.Next(28) + 1;
                    else
                        // set dayOfMonth to 28 to make next day to 1st
                        dayOfMonth = 28;
                    tomorrow = new SDate(dayOfMonth, season).AddDays(1);

                    // check if tomorrow's season is not allowed
                    if (!allowedSeasons.Contains(tomorrow.Season))
                    {
                        conflicts = true;
                    }

                    // AlwaysStartAt1st ignores AvoidFestivalDay
                    if (!config.AlwaysStartAt1st && config.AvoidFestivalDay)
                    {
                        if (Utility.isFestivalDay(tomorrow.Day, tomorrow.Season))
                        {
                            conflicts = true;
                            continue;
                        }
                    }
                    
                } while (conflicts);
            }

            // check if the date is winter 28th, if the option is used
            if (season == Season.Winter && dayOfMonth == 28 && config.UseWinter28toYear1)
            {
                isWinter28 = true;
            }
            else
            {
                isWinter28 = false;
            }
        }

        private static void Apply()
        {
            Game1.dayOfMonth = dayOfMonth;
            Game1.season = season;
            
            // refresh all locations
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                // this is initial objects, so call seasonal method
                location.seasonUpdate(false);
            }

            // make sure outside not dark, for Dynamic Night Time or something similar
            Game1.timeOfDay = 1200;
        }

        private string GetWheatWord(out string parsnipWord)
        {
            var stringData = Helper.GameContent.Load<Dictionary<string, string>>("Strings/Objects");

            //get parsnip word
            parsnipWord = stringData.GetValueSafe("Parsnip_Name").ToLower();
            //exception: de-DE
            if (Helper.GameContent.CurrentLocale == "de-DE")
                parsnipWord = "Pastinake";

            //get wheat word
            return stringData.GetValueSafe("Wheat_Name").ToLower();
        }

        public static bool NeedToGiveWheatSeeds()
        {
            // Parsnip * 15 : fall 24 ~ spring 23
            // Wheat * 18 : spring 24 ~ fall 23

            if ((season == Season.Fall && dayOfMonth >= 24) || season == Season.Winter || (season == Season.Spring && dayOfMonth <= 23))
                return false;
            else
                return true;
        }

        private static void PutSeasonalSeeds(bool wheatSeed) {
            if (!wheatSeed || Game1.whichModFarm?.Id == "MeadowlandsFarm")
                return;
            GameLocation farmHouse = Game1.getLocationFromName("FarmHouse");
            Farm farm = Game1.getFarm();

            // set seedbox coordinate
            if (!farm.TryGetMapPropertyAs("FarmHouseStarterSeedsPosition", out Vector2 seedBoxLocation))
            {
                seedBoxLocation = Game1.whichFarm switch
                {
                    1 or 2 or 4 => new Vector2(4f, 7f),
                    3 => new Vector2(2f, 9f),
                    6 => new Vector2(8f, 6f),
                    _ => new Vector2(3f, 7f),
                };
            }

            // change seed chest
            farmHouse.objects.Remove(seedBoxLocation);
            Chest chest = new(null, Vector2.Zero, true, giftboxIsStarterGift: true);

            // put items in new chest
            chest.Items.Add(ItemRegistry.Create("(O)483", 18));

            farmHouse.objects.Add(seedBoxLocation, chest);
        }

        // Harmony
        public static void Harmony_ChangeTodaysTip(ref string __result)
        {
            string resultString;
                Dictionary<string, string> dic = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\TipChannel");
                var date = SDate.Now();
                int year = (date.Year + 1) % 2;
                int season = date.SeasonIndex;
                int day = date.Day;
                int todayNumber = 112 * year + (28 * season) + day;
                if (dic.ContainsKey(todayNumber.ToString()))
                {
                    resultString = dic[todayNumber.ToString()];
                }
                else
                {
                    resultString = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13148");

                }
                __result = resultString;
                return;
        }

        public static void Harmony_ChangeIntroSeason(ref Texture2D ___roadsideTexture, ref Texture2D ___treeStripTexture)
        {
            if (season != Season.Spring)
            {
                ___roadsideTexture = Game1.content.Load<Texture2D>("Maps\\" + season + "_outdoorsTileSheet");
                ___treeStripTexture = modHelper.ModContent.Load<Texture2D>("assets/treestrip_" + season.ToString().ToLower() + ".png");
                Game1.changeMusicTrack(season.ToString().ToLower() + "_day_ambient");
            }
        }

        public static void Harmony_SetDateForIntro()
        {
            // RANDOMIZING
            Game1.startingGameSeed ??= new ulong?();
            Randomize(Game1.UseLegacyRandom);
        }

        public static void Harmony_Quest6ToWheatQuest(string questId)
        {
            if (needWheatSeeds)
            {
                if (questId == "6")
                {
                    // To prevent infinite call of the same method, used code in the method
                    Quest questFromId = Quest.getQuestFromId("idermailer.RandomStartDay.harvestWheat");
                    if (questFromId == null)
                        return;
                    else
                    {
                        Game1.player.questLog.Add(questFromId);
                        if (!questFromId.IsHidden())
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
                        Game1.player.removeQuest("6");
                    }
                }
            }
        }

        private static void Debug________()
        {
            //method for test
            season = Season.Fall;
            dayOfMonth = 23;
        }
    }
}
