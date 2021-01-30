/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using BetterTrainLoot.Config;
using BetterTrainLoot.Data;
using BetterTrainLoot.GamePatch;
using BetterTrainLoot.Interfaces;
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

        private IModHelper modHelper;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            modHelper = helper;

            config = helper.Data.ReadJsonFile<ModConfig>("config.json") ?? ModConfigDefaultConfig.CreateDefaultConfig("config.json");
            config = ModConfigDefaultConfig.UpdateConfigToLatest(config, "config.json");

            if (config.enableMod)
            {
                helper.Events.Input.ButtonReleased += Input_ButtonReleased;
                helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
                helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
                helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
                //helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

                harmony = HarmonyInstance.Create("com.aairthegreat.mod.trainloot");
                harmony.Patch(typeof(TrainCar).GetMethod("draw"), null, new HarmonyMethod(typeof(TrainCarOverrider).GetMethod("postfix_getTrainTreasure")));
                harmony.Patch(typeof(Railroad).GetMethod("PlayTrainApproach"), new HarmonyMethod(typeof(RailroadOverrider).GetMethod("prefix_playTrainApproach")));
                string trainCarFile = Path.Combine("DataFiles", "trains.json");
                trainCars = helper.Data.ReadJsonFile<Dictionary<TRAINS, TrainData>>(trainCarFile) ?? TrainDefaultConfig.CreateTrainCarData(trainCarFile);

                bool updateLoot = false;
                foreach(var train in  trainCars.Values)
                {
                    //updated list to include new base game treasure
                    if (!train.HasItem(806))
                    {
                        train.treasureList.Add(new TrainTreasure(806, "Leprechaun Shoes", 0.01, LOOT_RARITY.RARE, true));
                        updateLoot = true;
                    }
                }

                if (updateLoot)
                {
                    helper.Data.WriteJsonFile(trainCarFile, trainCars);
                }

                SetupMultiplayerObject();
            }
        }

        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.O && !railroadMapBlocked 
                && config.enableForceCreateTrain && isMainPlayer)
            {
                this.Monitor.Log("Player press O... Choo choo");                
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

            //Check for JSON Assests
            //if (this.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            //{
            //    var api = modHelper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");

            //    var trees = api?.GetAllFruitTreeIds();
            //    var crops = api?.GetAllCropIds();
            //    //Game1.Get
            //}


            var fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            var fruitTrees = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");

            var crops = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            ////Content Packs
            //foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            //{
            //    this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
            //}
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
        }

        private void ResetDailyValues()
        {
            forceNewTrain = false;
            enableCreatedTrain = true;
            numberOfTrains = 0;
            numberOfRewardsPerTrain = 0;
            pctChanceOfNewTrain = Game1.player.DailyLuck + config.basePctChanceOfTrain;                                                                                    
        }

        private void SetMaxNumberOfTrainsAndStartTime()
        {
            maxNumberOfTrains = (int)Math.Round((Game1.random.NextDouble() + Game1.player.DailyLuck) * (double)config.maxTrainsPerDay, 0, MidpointRounding.AwayFromZero);  

            double ratio = (double)maxNumberOfTrains / (double)config.maxTrainsPerDay;  

            startTimeOfFirstTrain = 1200 - (int)(ratio * 500);
            
            Monitor.Log($"Setting Max Trains to {maxNumberOfTrains}");
        }

        private void UpdateTrainLootChances()
        {
            //Update the treasure chances for today
            foreach (TrainData train in trainCars.Values)
            {
                train.UpdateTrainLootChances(Game1.player.DailyLuck);                                                                                                     
            }
        }

        private void SetupMultiplayerObject()
        {
            Type type = typeof(Game1);
            FieldInfo info = type.GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static);
            multiplayer = info.GetValue(null) as Multiplayer;
        }
    }
}
