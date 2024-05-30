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
using System.IO;
using System.Linq;
using System.Reflection;

namespace RandomStartDay
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private static ModConfig config;
        private static IMonitor monitor;
        static int dayOfMonth;
        static Season season = Season.Spring;
        static Season[] allowedSeasons = Array.Empty<Season>();

        static bool needWheatSeeds = false;
        static bool isWinter28 = false;
        static string errorString;
        static IModHelper modHelper;

        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>();
            monitor = this.Monitor;
            modHelper = helper;
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // run when disableAll is false or verification failed
            if (!config.DisableAll && Verification(out errorString))
            {
                helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
                helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
                helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
                helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;

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
            }
            else
            {
                helper.Events.GameLoop.GameLaunched += this.ErrorLogging;
            }

            harmony.Patch(
            original: AccessTools.Method(typeof(TV), "getTodaysTip"),
            postfix: new HarmonyMethod(typeof(ModEntry), nameof(Harmony_ChangeTodaysTip))
            );
            if (config.TVRecipeWithSeasonContext)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(TV), "getWeeklyRecipe"),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(Harmony_ChangeCookingChannel))
                    );
            }
            helper.ConsoleCommands.Add("rsd_set_date", "Change date without changing number of days played. SMAPI console commands do with that.\n" +
                    "Usage: rsd_set_date [-d <day>] [-s <season>] [-y <year>] [-nv]\n-nv: No validation for day of month and year. This may cause errors.", SetDate);
            helper.ConsoleCommands.Add("days_played", "Print number of days played on cosole.", PrintDaysPlayed);
        }

        // EVENTS
        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Initialize();
        }

        // logging
        private void ErrorLogging(object sender, GameLaunchedEventArgs e)
        {
            if (config.DisableAll)
                Monitor.Log("This mod is DISABLED. To enable, fix config and re-launch the game.", LogLevel.Debug);
            else if (errorString != "")
                Monitor.Log(errorString, LogLevel.Error);
        }

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
            string dataPath = Path.Combine(new string[] { "Data", "mail" });
            Dictionary<string, string> mailData = Game1.content.Load<Dictionary<string, string>>(dataPath);
            string lastYearMailName = Game1.currentSeason + "_" + Game1.dayOfMonth + "_" + (Game1.year - 1);
            if (mailData.ContainsKey(lastYearMailName))
            {
                if (!Game1.player.hasOrWillReceiveMail(lastYearMailName))
                {
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
        }

        // CONSOLE COMMANDS
        private static void SetDate(string command, string[] args)
        {
            // method termination if not the intended command
            if (command != "rsd_set_date")
            {
                monitor.Log("Oh no! Something seems wrong.\nConsole command entered: " + command + "\nAction executed: SetDate(string command, string[] args)", LogLevel.Error);
                return;
            }

            bool doValidation = true;
            bool changedDay = false;
            bool changedSeason = false;
            bool changedYear = false;

            int d = 1;
            Season s = Season.Spring;
            int y = 1;
            if (args.Length == 0 || (args.Length == 1 && args[0] == "-nv"))
            {
                monitor.Log("This command requires at least one of the following arguments: day, season, or year.\n" +
                    "If you're not sure what to input, try typing \"help rsd_set_date\" into the console.", LogLevel.Info);
                return;
            }

            if (args.Contains("-nv"))
                doValidation = false;

            for (var i = 0; i < args.Length; i++)
            {
                // dayOfMonth set
                if (args[i] == "-d" && !changedDay)
                {
                    if (args.Length == i + 1) // if "-d"is last arg
                    {
                        monitor.Log("There are no arguments for the day of month to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!int.TryParse(args[i + 1], out d)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        if (doValidation)
                        {
                            if (d < 1 || d > 28)
                            {
                                monitor.Log("Validation failed. Proper range for day of month is 1 ~ 28.\nArgument: " + args[i + 1]
                                    + "\nIf you intended that value, disable validation by adding the \"-nv\" argument to the command.", LogLevel.Info);
                                return;
                            }
                        }
                        changedDay = true;
                    }
                }

                // season set
                if (args[i] == "-s" && !changedSeason)
                {
                    if (args.Length == i + 1) // if "-s"is last arg
                    {
                        monitor.Log("There are no arguments for the season to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!Enum.TryParse(args[i + 1], true, out s)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        changedSeason = true;
                    }
                }

                // year set
                if (args[i] == "-y" && !changedYear)
                {
                    if (args.Length == i + 1) // if "-d"is last arg
                    {
                        monitor.Log("There are no arguments for year to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!int.TryParse(args[i + 1], out y)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        if (doValidation)
                        {
                            if (y < 1)
                            {
                                monitor.Log("Validation failed. Proper value is greater than or equal to 1.\nArgument: " + args[i + 1]
                                    + "\nIf you intended that value, disable validation by adding the \"-nv\" argument to the command.", LogLevel.Info);
                                return;
                            }
                        }

                        changedYear = true;
                    }
                }
            }

            if (!changedDay && !changedSeason && !changedYear)
            {
                monitor.Log("None of the day, season or year has changed.", LogLevel.Info);
                return;
            }
            else
            {
                if (changedDay)
                    Game1.dayOfMonth = d;
                if (changedSeason)
                    Game1.season = s;
                if (changedYear)
                    Game1.year = y;
                monitor.Log("Now is Day " + Game1.dayOfMonth + " of " + Game1.season.ToString() + ", Year " + Game1.year, LogLevel.Info);
            }
        }

        private static void PrintDaysPlayed(string command, string[] args)
        {
            // method termination if not the intended command
            if (command != "days_played")
            {
                monitor.Log("Oh no! Something seems wrong.\nConsole command entered: " + command + "\nAction executed: PrintDaysPlayed(string command, string[] args)", LogLevel.Error);
                return;
            }

            monitor.Log("The number of days you played is " + Game1.stats.DaysPlayed, LogLevel.Info);
        }

        // METHODS
        private static bool Verification(out string errorString)
        {
            bool succeeded = true;
            errorString = "";

            // check if allowedSeasons is not empty
            for (var i = 0; i < config.AllowSpringSummerFallWinter.Length; i++)
            {
                if (config.AllowSpringSummerFallWinter[i])
                {
                    allowedSeasons = allowedSeasons.AddToArray((Season)i);
                }
            }
            if (allowedSeasons.Length == 0)
            {
                errorString += "- All seasons are not allowed. Please allow at least one season. This mod will be DISABLED.\nTo enable, fix config and re-launch the game.\n";
                succeeded = false;
            }

            // check if content pack is not found
            if (!modHelper.ModRegistry.IsLoaded("idermailer.RandomStartDayContentPatch"))
            {
                errorString += "- Content Pack for this mod is not found. This mod will be DISABLED.\n" +
                    "To enable, you will probably need to check directory structure, or reinstall this mod.\n";
                succeeded = false;
            }

            return succeeded;
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
                // there are initial objects, so call season update method
                location.seasonUpdate(false);
            }

            // make sure outside not dark, for Dynamic Night Time or something similar
            Game1.timeOfDay = 1200;
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

        private static void PutSeasonalSeeds(bool wheatSeed)
        {
            // continue only if you need wheat seeds
            if (!wheatSeed)
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

            Chest chest = new(null, Vector2.Zero, true, giftboxIsStarterGift: true);
            // put items in new chest
            if (farm.TryGetMapProperty("FarmHouseStarterGift", out string customStarterGiftString))
            {
                string[] splitedString = customStarterGiftString.Split(' ');
                for (var i = 0; i < splitedString.Length; i += 2)
                {
                    Item item;
                    if (splitedString.Length != i + 1)
                    {
                        // if the item is 15 Parsnip Seeds, replace it with 18 Wheat Seeds.
                        if (splitedString[i] == "(O)472" && splitedString[i + 1] == "15")
                        {
                            item = ItemRegistry.Create("(O)483", 18);
                        }
                        else
                        {
                            item = ItemRegistry.Create(splitedString[i], int.Parse(splitedString[i + 1]));
                        }
                    }
                    // if the quantity of the last item is omitted, it is considered 1
                    else
                    {
                        item = ItemRegistry.Create(splitedString[i], 1);
                    }
                    chest.Items.Add(item);
                }
            }
            else
            {
                chest.Items.Add(ItemRegistry.Create("(O)483", 18));
            }

            // change seed chest
            farmHouse.objects.Remove(seedBoxLocation);
            farmHouse.objects.Add(seedBoxLocation, chest);
        }

        // Harmony
        public static void Harmony_ChangeTodaysTip(ref string __result)
        {
            string resultString;
            Dictionary<string, string> tips = DataLoader.Tv_TipChannel(Game1.content);
            int todayNumber = Game1.Date.TotalDays + 1;
            if (tips.ContainsKey(todayNumber.ToString()))
            {
                resultString = tips[todayNumber.ToString()];
            }
            else
            {
                string stringPath = Path.Combine(new string[] { "Strings", "StringsFromCSFiles" });
                Dictionary<string, string> CSStrings = Game1.temporaryContent.Load<Dictionary<string, string>>(stringPath);
                CSStrings.TryGetValue("TV.cs.13148", out resultString);
            }

            if (resultString != null)
            {
                __result = resultString;
            }
            else
            {
                __result = "Strings\\StringsFromCSFiles:TV.cs.13148";
            }
            return;
        }

        public static void Harmony_ChangeCookingChannel(ref string[] __result, ref StardewValley.Objects.TV __instance)
        {
            // only affected on Sunday
            if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) != "Sun")
                return;

            Dictionary<string, string> cookingData = DataLoader.Tv_CookingChannel(Game1.content);
            int todayNumber = Game1.Date.TotalDays + 1;

            int recipeNum = todayNumber % 224 / 7;
            if (todayNumber % 224 == 0)
                recipeNum = 32;
            MethodInfo m = AccessTools.Method(typeof(StardewValley.Objects.TV), "getWeeklyRecipe", new Type[] { typeof(Dictionary<string, string>), typeof(System.String) });
            try
            {
                __result = (string[])m.Invoke(__instance, new object[] { cookingData, recipeNum.ToString() });
            }
            catch
            {
                __result = (string[])m.Invoke(__instance, new object[] { cookingData, "1" });
            }
        }

        public static void Harmony_ChangeIntroSeason(ref Texture2D ___roadsideTexture, ref Texture2D ___treeStripTexture)
        {
            if (season != Season.Spring)
            {
                ___roadsideTexture = Game1.content.Load<Texture2D>("Maps/" + season + "_outdoorsTileSheet");
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
                    Quest questFromId = Quest.getQuestFromId("idermailer.RandomStartDay.harvestWheat");
                    if (questFromId != null)
                    {
                        Game1.player.questLog.Add(questFromId);
                        Game1.player.removeQuest("6");
                    }
                    else
                    {
                        monitor.Log("Quest \"idermailer.RandomStartDay.harvestWheat\" is not found.");
                    }
                }
            }
        }

        private static void Debug________()
        {
        }
    }
}
