/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;

namespace RandomStartDay
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig config;

        private int dayOfMonth;
        private String currentSeason = "spring";

        private bool introEnd = true; // for asset replacing
        public bool winter28 = false;
        private IAssetName originalSpringTileName;

        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
            helper.Events.Content.AssetRequested += this.Content_AssetRequested;
            helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
        }

        // EVENTS
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            introEnd = true;
            winter28 = false;
            verification();
            // if unique id is used, other random options are disabled
            if (config.isRandomSeedUsed)
            {
                Monitor.Log("Your Game's unique ID will be used. The other randomize options are disabled.", LogLevel.Debug);
                config.allowedSeasons = new String[] { "spring", "summer", "fall", "winter" };
                config.avoidFestivalDay = false;
            }
            else
            {
                Monitor.Log("Default random seed will be used.", LogLevel.Debug);
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

                // check if the date is winter 28th, if the option is used
                if (currentSeason == "winter" && dayOfMonth == 28 && config.useWinter28toYear1)
                {
                    winter28 = true;
                }
                else
                {
                    winter28 = false;
                }
            }

            // Main method
            if (e.NewStage == LoadStage.CreatedInitialLocations) // new game
            {
                apply();
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
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {

            if (!config.useSeasonalTilesetInBusScene)
                return;

            /*  ★ minigames/currentseason_intro 이미지 만들 것
            if (e.NameWithoutLocale.IsEquivalentTo("Minigames/Intro"))
            {
                e.LoadFromModFile<Texture2D>("assets/" + currentSeason + "Intro.png", AssetLoadPriority.Low);
            }
            */
            // outdoortiles, fixed on spring to seasonal, when introEnd is false
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/spring_outdoorsTileSheet"))
            {
                if (introEnd == false)
                // load asset from game folder
                {
                    originalSpringTileName = e.Name;
                    e.LoadFromModFile<Texture2D>("../../Content/Maps/" + currentSeason + "_outdoorsTileSheet.xnb", AssetLoadPriority.Low);
                }
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
                            this.Monitor.Log("array \"allowedSeasons\" contains invalid value(s). Valid values are: \"spring\", \"summer\", \"fall\", \"winter\". This mod did NOT work.", LogLevel.Error);
                            introEnd = true;
                            return;
                        }
                        
                    }
                }
        }

        private void randomize (Random random)
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

        private void test________()
        {
            Monitor.Log("Test Method called!!!!!!", LogLevel.Warn);
            //method for test
            dayOfMonth = 28;
        }
    }
}
