using BetterTrainLoot.Config;
using BetterTrainLoot.Data;
using BetterTrainLoot.GamePatch;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BetterTrainLoot
{
    public class BetterTrainLootMod : Mod
    {
        public static BetterTrainLootMod Instance { get; private set; }
        public static int numberOfRewardsPerTrain = 0;

        internal static Multiplayer multiplayer;

        internal HarmonyInstance harmony { get; private set; }

        private int maxNumberOfTrains;
        private int numberOfTrains = 0;
        private int startTimeOfFirstTrain = 600;

        internal TRAINS trainType;

        private double pctChanceOfNewTrain = 0.0;

        private bool startupMessage = true;
        private bool forceNewTrain;
        private bool enableCreatedTrain = true;
        private bool railroadMapBlocked;
        private bool isMainPlayer;

        internal ModConfig config;
        internal Dictionary<TRAINS, TrainData> trainCars;

        private Railroad railroad;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            config = helper.Data.ReadJsonFile<ModConfig>("config.json") ?? ModConfigDefaultConfig.CreateDefaultConfig("config.json");
            config = ModConfigDefaultConfig.UpdateConfigToLatest(config, "config.json");

            if (config.enableMod)
            {
                helper.Events.Input.ButtonReleased += Input_ButtonReleased;
                helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
                helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
                helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

                harmony = HarmonyInstance.Create("com.aairthegreat.mod.trainloot");
                harmony.Patch(typeof(TrainCar).GetMethod("draw"), null, new HarmonyMethod(typeof(TrainCarOverrider).GetMethod("postfix_getTrainTreasure")));

                string trainCarFile = Path.Combine("DataFiles", "trains.json");
                trainCars = helper.Data.ReadJsonFile<Dictionary<TRAINS, TrainData>>(trainCarFile) ?? TrainDefaultConfig.CreateTrainCarData(trainCarFile);

                SetupMultiplayerObject();
            }
        }
        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.Y && !railroadMapBlocked 
                && config.enableForceCreateTrain && isMainPlayer)
            {
                this.Monitor.Log("Player press Y... Choo choo");                
                forceNewTrain = true;
                enableCreatedTrain = true;
            }
            else if (e.Button == SButton.Y && railroadMapBlocked)
            {
                this.Monitor.Log("Player press Y, but the railraod map is not available... No choo choo for you.");
            }
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            railroad = (Game1.getLocationFromName("Railroad") as Railroad);
            startupMessage = true;
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (railroad != null && !railroadMapBlocked && isMainPlayer)
            {
                if (Game1.player.currentLocation.IsOutdoors && railroad.train.Value == null)
                {
                    if (forceNewTrain)
                    {
                        CreateNewTrain();
                    }
                    else if (enableCreatedTrain
                        && numberOfTrains < maxNumberOfTrains
                        && e.NewTime >= startTimeOfFirstTrain
                        && Game1.random.NextDouble() <= pctChanceOfNewTrain)
                    {
                        CreateNewTrain();
                    }
                }

                if (railroad.train.Value != null && !enableCreatedTrain)
                {
                    enableCreatedTrain = true;
                    trainType = (TRAINS)railroad.train.Value.type.Value;
                }
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (CheckForMapAccess())
            {
                if (IsMainPlayer())
                {
                    ResetDailyValues();
                    SetMaxNumberOfTrainsAndStartTime();
                }
                UpdateTrainLootChances();
            }            
        }

        private bool IsMainPlayer()
        {
            if (Context.IsMainPlayer)
            {
                if (!isMainPlayer)
                {
                    isMainPlayer = true;
                    startupMessage = true;
                }
            }
            else
            {
                isMainPlayer = false;
            }

            SetStartupMessage();

            return isMainPlayer;
        }

        private void SetStartupMessage()
        {
            if (startupMessage && isMainPlayer)
            {
                this.Monitor.Log("Single player or Host:  Mod Enabled.");
            }
            else if (startupMessage && !isMainPlayer)
            {
                this.Monitor.Log("Farmhand player: (Mostly) Mod Disabled.");
            }
            startupMessage = false;
        }

        private bool CheckForMapAccess()
        {
            railroadMapBlocked = (Game1.stats.DaysPlayed < 31U);
            if (railroadMapBlocked)
            {
                Monitor.Log("Railroad map blocked.  No trains can be created, yet.");
            }

            return !railroadMapBlocked;
        }

        private void CreateNewTrain()
        {
            numberOfRewardsPerTrain = 0;
            railroad.setTrainComing(config.trainCreateDelay);
            numberOfTrains++;
            forceNewTrain = false;
            trainType = TRAINS.UNKNOWN;
            //this.Monitor.Log($"Setting train... Choo choo... {Game1.timeOfDay}");
            enableCreatedTrain = false;
            SendMulitplayerMessage("A train is approaching Stardew Valley...");
        }

        private void ResetDailyValues()
        {
            forceNewTrain = false;
            enableCreatedTrain = true;
            numberOfTrains = 0;
            numberOfRewardsPerTrain = 0;
            pctChanceOfNewTrain = Game1.dailyLuck + config.basePctChanceOfTrain;                                                                                    // SDV 1.4... use Game1.player.DailyLuck
        }

        private void SetMaxNumberOfTrainsAndStartTime()
        {
            maxNumberOfTrains = (int)Math.Round((Game1.random.NextDouble() + Game1.dailyLuck) * (double)config.maxTrainsPerDay, 0, MidpointRounding.AwayFromZero);  // SDV 1.4... use Game1.player.DailyLuck

            double ratio = (double)maxNumberOfTrains / (double)config.maxTrainsPerDay;  

            startTimeOfFirstTrain = 1200 - (int)(ratio * 500);
            
            Monitor.Log($"Setting Max Trains to {maxNumberOfTrains}");
        }

        private void UpdateTrainLootChances()
        {
            //Update the treasure chances for today
            foreach (TrainData train in trainCars.Values)
            {
                train.UpdateTrainLootChances(Game1.dailyLuck);                                                                                                      // SDV 1.4... use Game1.player.DailyLuck
            }
        }

        private void SetupMultiplayerObject()
        {
            Type type = typeof(Game1);
            FieldInfo info = type.GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static);
            multiplayer = info.GetValue(null) as Multiplayer;
        }

        internal static void SendMulitplayerMessage(string message)
        {
            if (multiplayer != null && BetterTrainLootMod.Instance.config.enableMultiplayerChatMessage)
            {
                multiplayer.sendChatMessage(LocalizedContentManager.LanguageCode.en, message);
            }
        }
    }
}
