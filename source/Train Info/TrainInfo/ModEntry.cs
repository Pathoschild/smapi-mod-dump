/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/TrainInfo
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainInfo
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        public static ModConfig Config;
        private static Railroad Railroad = null;
        private static int TrainTimeTomorrow = -1;
        private static bool HasSentTrainContentsMessage = false;

        public const string ModMessageTrainContents = "train contents";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // set up static properties
            Config = helper.ReadConfig<ModConfig>();

            // set up event handlers
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        /*********
        ** Private methods
        *********/
        /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // reset static variables
            Railroad = null;
            TrainTimeTomorrow = -1;
            HasSentTrainContentsMessage = false;
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenuApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenuApi != null)
            {
                // register mod
                configMenuApi.Register(
                    ModManifest,
                    () => Config = new ModConfig(),
                    () => Helper.WriteConfig(Config)
                );

                // add some config options
                configMenuApi.AddNumberOption(
                    ModManifest,
                    () => Config.NotificationTime,
                    value => Config.NotificationTime = value,
                    () => this.Helper.Translation.Get("config.option.notification_time.name"),
                    () => this.Helper.Translation.Get("config.option.notification_time.description"),
                    600,
                    2400,
                    100,
                    value => Game1.getTimeOfDayString(value)
                );
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // reset
            int trainTime = TrainTimeTomorrow;
            TrainTimeTomorrow = -1;
            HasSentTrainContentsMessage = false;

            // init Railroad
            Railroad ??= (Railroad)Game1.getLocationFromName("Railroad");

            // send train today message
            if (trainTime != -1)
            {
                CustomHUDMessage trainTodayMessage = new(this.Helper.Translation.Get("train_today", new { time = Game1.getTimeOfDayString(trainTime) }));
                Game1.addHUDMessage(trainTodayMessage);
            }

            // check for train tomorrow
            Random r = Utility.CreateRandom(Game1.stats.DaysPlayed + 1, Game1.uniqueIDForThisGame / 2, 0.0, 0.0, 0.0);
            bool isTomorrowAFestival = Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season);
            if (r.NextDouble() < 0.2 && Game1.isLocationAccessible("Railroad") && !isTomorrowAFestival)
            {
                TrainTimeTomorrow = r.Next(900, 1800);
                TrainTimeTomorrow -= TrainTimeTomorrow % 10;
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Railroad?.train.Value != null && !HasSentTrainContentsMessage)
            {
                HasSentTrainContentsMessage = true;
                HandleTrainContents(Railroad.train.Value);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // train tomorrow
            if (TrainTimeTomorrow != -1 && e.NewTime == Config.NotificationTime)
            {
                CustomHUDMessage trainTomorrowMessage = new(this.Helper.Translation.Get("train_tomorrow", new { time = Game1.getTimeOfDayString(TrainTimeTomorrow) }));
                Game1.addHUDMessage(trainTomorrowMessage);
            }
        }

        /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID)
            {
                Monitor.Log($"Received {e.Type} from host.", LogLevel.Trace);
                switch (e.Type)
                {
                    case ModMessageTrainContents:
                        DisplayTrainContents(e.ReadAs<Dictionary<string, int>>());
                        break;
                }
            }
        }

        public void HandleTrainContents(Train train)
        {
            // count each resource from all train cars
            Dictionary<string, int> trainItems = new();
            foreach (var car in train.cars)
            {
                if (car.carType.Value == TrainCar.coalCar)
                {
                    string itemId = null;
                    switch (car.resourceType.Value)
                    {
                        case TrainCar.coal:
                            itemId = "(O)382";
                            break;
                        case TrainCar.metal:
                            itemId = (car.color.Value.R > car.color.Value.G) ? "(O)378" : ((car.color.Value.G > car.color.Value.B) ? "(O)380" : ((car.color.Value.B > car.color.Value.R) ? "(O)384" : "(O)378"));
                            break;
                        case TrainCar.rocks:
                            itemId = Railroad.IsWinterHere() ? "(O)536" : ((Game1.stats.DaysPlayed > 120 && car.color.Value.R > car.color.Value.G) ? "(O)537" : "(O)535");
                            break;
                        case TrainCar.wood:
                            itemId = "(O)388";
                            break;
                        case TrainCar.bricks:
                            itemId = "(O)390";
                            break;
                        case TrainCar.presents:
                            itemId = "(O)MysteryBox";
                            break;
                    }
                    if (itemId != null)
                    {
                        if (trainItems.ContainsKey(itemId))
                        {
                            trainItems[itemId] += car.loaded.Value;
                        }
                        else
                        {
                            trainItems.Add(itemId, car.loaded.Value);
                        }
                    }
                }
            }

            // display train contents
            DisplayTrainContents(trainItems);

            // send to farmhands
            Helper.Multiplayer.SendMessage(trainItems, ModMessageTrainContents);
        }

        public void DisplayTrainContents(Dictionary<string, int> trainItems)
        {
            string message = string.Join(Environment.NewLine, trainItems.Select(item => $"{item.Value} {ItemRegistry.Create(item.Key).DisplayName}"));
            message = string.IsNullOrEmpty(message) ? this.Helper.Translation.Get("empty_train") : message;
            CustomHUDMessage trainContentsMessage = new(message)
            {
                timeLeft = 7000f,
                titleText = this.Helper.Translation.Get("list_title")
            };
            Game1.addHUDMessage(trainContentsMessage);
        }
    }
}