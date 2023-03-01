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
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomStartDay
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig config;
        private int dayOfMonth;
        private string currentSeason = "spring";

        private bool introEnd = true; // for asset replacing
        public bool winter28 = false;
        private IAssetName originalSpringTileName;
        string seedSeason = "spring";
        string parsnipWord = "parsnip";
        string cropWord = "crop";
        int questCropID = 24;

        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;

            // run when disableAll is false
            if (!config.disableAll)
            {
                helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
                helper.Events.Content.AssetRequested += this.Content_AssetRequested;
                helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
                helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

                var harmony = new Harmony(this.ModManifest.UniqueID);
                string tipMethodName = Helper.Reflection.GetMethod(new TV(), "getTodaysTip").MethodInfo.Name;
                HarmonyMethodPatches.Initialize(helper, Monitor);

                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Objects.TV), tipMethodName),
                    postfix: new HarmonyMethod(typeof(HarmonyMethodPatches), nameof(HarmonyMethodPatches.changeTodaysTip))
                    );
            }
        }

        // EVENTS
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // disable all
            if (config.disableAll)
            {
                Monitor.Log("DISABLED", LogLevel.Debug);
                return;
            }

            introEnd = true;
            winter28 = false;
            verification();
            // if unique id is used, other random options are disabled
            if (config.isRandomSeedUsed)
            {
                Monitor.Log("ENABLED, using unique ID(9digit number)", LogLevel.Debug);
                config.allowedSeasons = new String[] { "spring", "summer", "fall", "winter" };
                config.avoidFestivalDay = false;
                config.alwaysStartAt1st = false;
            }
            else
            {
                Monitor.Log("ENABLED, using default random seed", LogLevel.Debug);
            }
        }

        private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.CreatedBasicInfo)
            {
                // make introEnd to false because asset is loaded before createdInitialLocations
                introEnd = false;
                // for prevent tilesheet to be fixed to spring
                if (config.isRandomSeedUsed)
                {
                    Random random = new((int)Game1.uniqueIDForThisGame);
                    randomize(random);
                }
                else
                {
                    Random random = new();
                    randomize(random);
                }

                if (config.alwaysStartAt1st)
                {
                    // set day to 28th, because to make next day to 1st day
                    dayOfMonth = 28;
                }

                // check if the date is winter 28th, if the option is used
                if (currentSeason == "winter" && dayOfMonth == 28 && config.useWinter28toYear1)
                {
                    winter28 = true;
                }
                else
                {
                    winter28 = false;
                }
                setSeedSeason();
            }

            if (e.NewStage == LoadStage.CreatedInitialLocations)
            {
                apply();
                if (config.useSeasonalSeeds)
                {
                    putSeasonalSeeds();
                }
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            // if player moves on winter 28th(=starts on spring 1), return to year 1
            if (winter28)
            {
                --Game1.year;
                winter28 = false;
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            // GameLoop.SaveCreated not worked so I use DayStarted I don't know why
            if (introEnd == false)
            {
                Helper.GameContent.InvalidateCache(originalSpringTileName);
            }
            introEnd = true;

            // problem fix: first day, clear mailbox and add willy's mail to tomorrow's mail
            if (Game1.stats.daysPlayed == 1)
            {
                Game1.mailbox.Clear();
                Game1.addMailForTomorrow("spring_2_1");
            }

            // When an email that should be received last year exists and not received yet
            Dictionary<string, string> mailData = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
            if (mailData.ContainsKey(currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + (Game1.year - 1).ToString()))
            {
                if (!Game1.player.hasOrWillReceiveMail(currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + (Game1.year - 1).ToString())) {
                    // add last year mail and remove this year mail
                    Game1.mailbox.Add(currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + (Game1.year - 1).ToString());
                    Game1.mailbox.Remove(currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + (Game1.year).ToString());
                }
            }
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Minigames/Intro"))
            {
                if (currentSeason != "spring")
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsImage();
                        Texture2D sourceImage = this.Helper.ModContent.Load<Texture2D>("assets/intro_" + currentSeason + ".png");

                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(0, 176, 48, 80)), sourceArea: (new Rectangle(0, 0, 48, 80)));
                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(48, 224, 64, 16)), sourceArea: (new Rectangle(48, 48, 64, 16)));
                    });
                }
            }

            // outdoortiles, fixed on spring to seasonal, when introEnd is false
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/spring_outdoorsTileSheet"))
            {
                if (introEnd == false)
                // load asset from game folder
                {
                    originalSpringTileName = e.Name;
                    e.LoadFromModFile<Texture2D>("../../../Content/Maps/" + currentSeason + "_outdoorsTileSheet.xnb", AssetLoadPriority.Low);
                }
            }

            // edit quest #6
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests"))
            {
                e.Edit(asset =>
                {
                    // edit text
                    var data = asset.AsDictionary<int, string>().Data;
                    StringBuilder stringBuilder = new StringBuilder(data[6]);

                    stringBuilder = stringBuilder.Replace(parsnipWord, cropWord);
                    //edit trigger
                    data[6] = stringBuilder.Replace(24.ToString(), questCropID.ToString()).ToString();

                });
            }

            // get parsnip word and crop word
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;
                    //get parsnip word
                    parsnipWord = data[24].Split('/')[4];

                    //get crop word
                    if (seedSeason == "summer")
                    {
                        cropWord = data[264].Split('/')[4];
                    }
                    else if (seedSeason == "fall")
                    {
                        cropWord = data[278].Split('/')[4];
                    }
                    else
                    {
                        cropWord = parsnipWord;
                    }
                    
                });
            }
        }

        // METHODS
        private void verification()
        {
            // if allowed seasons have invalid value (other than spring, summer, fall, winter)
            for (int i = 0; i < config.allowedSeasons.Length; i++)
            {
                switch (config.allowedSeasons[i])
                {
                    case "spring":
                        break;
                    case "summer":
                        break;
                    case "fall":
                        break;
                    case "winter":
                        break;
                    default:
                        {
                            Monitor.Log("array \"allowedSeasons\" contains invalid value(s). Valid values are: \"spring\", \"summer\", \"fall\", \"winter\". This mod did NOT work.", LogLevel.Error);
                            introEnd = true;
                            return;
                        }

                }
            }
        }

        private void randomize(Random random)
        {
            do
            {
                dayOfMonth = random.Next(28) + 1;
                currentSeason = config.allowedSeasons[random.Next(config.allowedSeasons.Length)];
                // if next day is festival day, randomize one more time
                if (!Utility.isFestivalDay(dayOfMonth + 1, currentSeason))
                    break;
                random = new Random();
            } while (true);
        }

        private void apply()
        {
            Game1.dayOfMonth = dayOfMonth;
            Game1.currentSeason = currentSeason;
            
            // refresh all locations
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                // this is initial objects, so call seasonal method
                location.seasonUpdate(currentSeason);
            }

            // make sure outside not dark, for Dynamic Night Time
            Game1.timeOfDay = 1200;
        }

        private void putSeasonalSeeds() {
            Vector2 seedBoxLocation = new(0f, 0f);
            // set seedbox location
            switch (Game1.whichFarm)
            {
                case 0:
                case 5:
                    seedBoxLocation = new Vector2(3f, 7f);
                    break;
                case 1:
                case 2:
                case 4:
                    seedBoxLocation = new Vector2(4f, 7f);
                    break;
                case 3:
                    seedBoxLocation = new Vector2(2f, 9f);
                    break;
                case 6:
                    seedBoxLocation = new Vector2(8f, 6f);
                    break;
            }

            // change seed chest
            // Parsnip * 15 : ~ spring 24
            // Radish * 6 : ~ summer 22
            // Bok Choy * 8 : ~ fall 24

            GameLocation farmHouse = Game1.getLocationFromName("FarmHouse");
            farmHouse.objects.Remove(seedBoxLocation);

            // spring 24 ~ summer 21 
            if (seedSeason == "summer")
            {
                farmHouse.objects.Add(seedBoxLocation, new Chest(0, new List<Item>()
                {
                        (Item)new StardewValley.Object(484, 6)
                }, seedBoxLocation, true));
            }
            // summer 22 ~ fall 23
            else if (seedSeason == "fall")
            {
                farmHouse.objects.Add(seedBoxLocation, new Chest(0, new List<Item>()
                {
                        (Item)new StardewValley.Object(491, 8)
                }, seedBoxLocation, true));
            }
            // fall 24 ~ spring 23
            else
            {
                farmHouse.objects.Add(seedBoxLocation, new Chest(0, new List<Item>()
                {
                        (Item)new StardewValley.Object(472, 15)
                }, seedBoxLocation, true));
            }
        }

        public void setSeedSeason()
        {
            if ((currentSeason == "spring" && dayOfMonth >= 24) || (currentSeason == "summer" && dayOfMonth < 22))
            {
                seedSeason = "summer";
                questCropID = 264;
            }
            // summer 22 ~ fall 23
            else if (currentSeason == "summer" || (currentSeason == "fall" && dayOfMonth < 24))
            {
                seedSeason = "fall";
                questCropID = 278;
            }
            // fall 24 ~ spring 23
            else
            {
                seedSeason = "spring";
                questCropID = 24;
            }

            // invalidateCache for make to reload objectinformation
            Helper.GameContent.InvalidateCache("Data/ObjectInformation." + Helper.GameContent.CurrentLocale);

            return;
        }

        private void test________()
        {
            Monitor.Log("Test Method called!!!!!!", LogLevel.Warn);
            //method for test
            currentSeason = "winter";
            dayOfMonth = 21;
        }
    }
}
