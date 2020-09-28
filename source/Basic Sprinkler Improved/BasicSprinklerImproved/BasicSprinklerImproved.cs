using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;

namespace BasicSprinklerImproved
{
    class BasicSprinklerImproved : Mod
    {
        int sprinklerID;            //ID# of sprinkler item. Could change for more advanced sprinklers or other objects.
        BasicSprinklerConfig myConfig;         //Basic config

        //Name of backup file
        readonly string backupFile = "oldpattern.json";
        //Name of config file
        //readonly string configFile = "config.json";

        WateringPattern toWater;    //Holds config-loaded data
        WateringPattern lastUsed;   //Pattern used previously

        bool noProb;                //Was there an error loading a pattern?

        IModHelper myHelper;        //For use throughout

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("BasicSprinklerImproved: Entry made.");

            myHelper = helper;

            noProb = true;

            sprinklerID = 599;  //ID# of sprinkler object

            LoadConfigsFromFile(backupFile, lastUsed);  //Keep track of last used pattern.
            LoadConfigsFromFile();                      //Load actual config.

            myHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            myHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            myHelper.Events.GameLoop.DayStarted += WaterEachMorning;
            myHelper.Events.GameLoop.Saving += SavePatterns;

            Monitor.Log("Basic Sprinkler Improved => Initialized", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            noProb = true;

            if (myConfig != null)
            {
                //Generic Mod Config Menu setup.
                TryLoadingGMCM();
            }
            else Monitor.Log("Unable to setup menu due to configuration not being set properly.", LogLevel.Warn);
        }

        private void TryLoadingGMCM()
        {
            //See if we can find GMCM, quit if not.
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenu.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                Monitor.Log("Unable to load GMCM API.", LogLevel.Info);
                return;
            }

            WateringPattern dp = new WateringPattern(); //Just need this to refer to a thing
            string[] pattypes = dp.GetPatternTypes();

            api.RegisterModConfig(ModManifest, () => myConfig = new BasicSprinklerConfig(), () => Helper.WriteConfig(myConfig));

            //Pattern types
            api.RegisterChoiceOption(ModManifest, "Watering Pattern", "Which watering pattern to use. Set to 'custom' (it's in the dropdown, but might not be immediately visible) to set a custom area using the controls below. Otherwise they are ignored.", () => myConfig.patternType, (string val) => myConfig.patternType = val, pattypes);
            api.RegisterLabel(ModManifest, "Sum of custom values must not exceed 4.", "The improved basic sprinkler will have the same watering area as the default. Values entered here when custom pattern is selected will throw an error if they add up to more than 4. The game will then use the default pattern.");
            api.RegisterClampedOption(ModManifest, "Custom North Area", "How far north the sprinkler should water.", () => myConfig.northArea, (int val) => myConfig.northArea = val, 0, 4);
            api.RegisterClampedOption(ModManifest, "Custom South Area", "How far south the sprinkler should water.", () => myConfig.southArea, (int val) => myConfig.southArea = val, 0, 4);
            api.RegisterClampedOption(ModManifest, "Custom East Area", "How far east the sprinkler should water.", () => myConfig.eastArea, (int val) => myConfig.eastArea = val, 0, 4);
            api.RegisterClampedOption(ModManifest, "Custom West Area", "How far west the sprinkler should water.", () => myConfig.westArea, (int val) => myConfig.westArea = val, 0, 4);
        }

        private void OnSaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            noProb = true;  //Kinda stumped as to how this sticks between game loads, hope this fixes it...
        }

        //Log some useful data output on game load using Monitor.Log("");
        void DoDiagnostics()
        {
            Monitor.Log("DAY-START DIAGNOSTICS BEGUN");

            //Was there a pattern load error?
            if (!noProb) Monitor.Log($"Pattern load error occurred. Attempt = {toWater}", LogLevel.Warn);

            //Is there an old pattern known, and if so, what is it?
            if (lastUsed == null) Monitor.Log("No prior pattern known.");
            else Monitor.Log($"Prior pattern = {lastUsed}");

            //What is the current pattern?
            Monitor.Log($"Current pattern = {toWater}");

            //What item are we acting on?
            Monitor.Log($"Using item ID#{sprinklerID}");

            Monitor.Log("DAY-START DIAGNOSTICS COMPLETE");
        }
        
        WateringPattern LoadPatternFromConfig(BasicSprinklerConfig config)
        {
            WateringPattern result;

            if (config == null)
            {
                Monitor.Log($"Tried to load nonexistent config...", LogLevel.Error);
                noProb = false;
                return null;
            }

            string type = config.patternType;
            int[] dims = new int[4] { config.northArea, config.southArea, config.eastArea, config.westArea };
            result = new WateringPattern(type, dims);

            if (result.errorMsg != "")
            {
                Monitor.Log($"Error in current pattern: {result.errorMsg}", LogLevel.Warn);
                noProb = false;
            }

            return result;
        }

        void LoadConfigsFromFile(string fileName = "", WateringPattern toSet = null)
        {
            BasicSprinklerConfig tempConfig;
            string loadedName;

            //default config - what we're actually using.
            if (fileName == "")
            {
                Monitor.Log("Loading pattern from default config file.");
                myConfig = this.Helper.ReadConfig<BasicSprinklerConfig>();
            }
            //custom file
            else
            {
                Monitor.Log($"Loading pattern from config file: '{fileName}'");
                tempConfig = this.Helper.Data.ReadJsonFile<BasicSprinklerConfig>(fileName);
                loadedName = fileName;

                if (tempConfig != null && toSet != null)
                {
                    Monitor.Log("Configuration loaded correctly.");
                    toSet = LoadPatternFromConfig(tempConfig);

                    if (toSet == null) Monitor.Log($"No pattern could be loaded from file: '{loadedName}'; may be saved & loaded later.");
                    else if (toSet.errorMsg != "") Monitor.Log($"Error in setting pattern: {toSet.errorMsg}");
                    else Monitor.Log($"Result = {toSet}");
                }
                else Monitor.Log($"Could not load from file '{loadedName}', did you pass a valid pattern to set?");
            }
        }

        //Save last used pattern. The point here being that if the user changes the desired pattern, we need to undo the old pattern.
        void SavePatterns(object sender, EventArgs e)
        {
            Monitor.Log("Saving current pattern.");

            //Helper.Data.WriteJsonFile<BasicSprinklerConfig>(configFile, new BasicSprinklerConfig(toWater.myType, toWater.myPattern));

            if (lastUsed == null) { 
                Monitor.Log("First time save.");
                Monitor.Log($"Current = {toWater}");
            }
            else
            {
                Monitor.Log($"Previous = {lastUsed}");
                Monitor.Log($"New = {toWater}");
            }

            lastUsed = toWater;
            Helper.Data.WriteJsonFile<BasicSprinklerConfig>(backupFile, new BasicSprinklerConfig(lastUsed.myType, lastUsed.myPattern));

            Monitor.Log($"New pattern saved - {lastUsed}");
        }

        //Every day, activate all sprinklers if not raining.
        void WaterEachMorning(object sender, EventArgs e)
        {
            //Just give up if we're stuck with a default sprinkler due to pattern def error
            if (!noProb)
            {
                Monitor.Log("Had to quit watering due to a pattern definition issue, default pattern will be applied.", LogLevel.Warn);
                return;
            }

            toWater = LoadPatternFromConfig(myConfig);
            if (toWater == null)
            {
                Monitor.Log("Had to quit watering due inablity to load pattern to apply from config, default pattern will be applied.", LogLevel.Error);
                return;
            }

            DoDiagnostics();

            //Check to see if we should update the pattern: If the pattern has changed in the congfig, undo the last pattern.
            if (lastUsed != null && lastUsed.ToString() != toWater.ToString())
            {
                Monitor.Log("Updating pattern.");
                Monitor.Log("Prior = " + lastUsed.ToString());
                Monitor.Log("New = " + toWater.ToString());

                LocateSprinklers(UnSprinkleOldPattern);
            }
            else
            {
                Monitor.Log("No pattern update needed. Current pattern = " + toWater.ToString());
            }

            //Suppress default pattern
            LocateSprinklers(UnwaterDefault);

            //Activate user-defined pattern
            LocateSprinklers(ActivateSprinkler);

            //myHelper.Events.Player.Warped -= Event_DoWatering;
        }

        //Locate and act on all sprinklers.
        void LocateSprinklers(Action<GameLocation, Vector2> toDo)
        {
            //Need to have something to do.
            if (toDo == null)
            {
                Monitor.Log("Attempted to act on sprinkler, no valid action given.", LogLevel.Error);
                return;
            }
            Monitor.Log("Finding and acting on all sprinklers...");

            //Act on all found sprinklers in all locations
            foreach (GameLocation location in GetAllGameLocations())
            {
                bool shouldProceed = !(location.IsOutdoors && (Game1.isRaining || Game1.isLightning));
                //Monitor.Log($"Looking for sprinklers at '{location}' - Outdoors = {location.IsOutdoors}, Raining = {Game1.isRaining}, Storm = {Game1.isLightning}, Proceed = {shouldProceed}");
                if (shouldProceed)
                {
                    //Monitor.Log($"Starting sprinkler search in '{location}'");
                    foreach (StardewValley.Object obj in location.objects.Values)
                    {
                        if (obj.parentSheetIndex == sprinklerID)
                        {
                            //Monitor.Log($"Found sprinkler in '{location}' at '{obj.tileLocation}'");
                            toDo(location, obj.tileLocation);
                        }
                    }
                }
            }
        }

        //Undo a certain sprinkler pattern.
        void UnSprinkleOldPattern(GameLocation location, Vector2 position)
        {
            Monitor.Log("'Unsprinkling' of prior pattern begun.");

            ////Just give up if we're stuck with a default sprinkler due to pattern def error
            //if (!noProb)
            //{
            //    Monitor.Log("Had to quit 'unsprinkling' due to a pattern definition issue.", LogLevel.Warn);
            //    return;
            //}

            int desiredState = HoeDirt.dry;     //Want it dried

            float iX = position.X;
            float iY = position.Y;

            Monitor.Log(String.Format("Undoing sprinkler in " + location.ToString() + " at {0},{1}", iX, iY));

            //Unwater stuff per pattern.
            WalkThroughPattern(location, lastUsed.myPattern, iX, iY, desiredState);
            Monitor.Log("'Unsprinkling' complete.");
        }

        //Need to clear the default sprinkler pattern.
        void UnwaterDefault(GameLocation location, Vector2 position)
        {
            int desiredState = HoeDirt.dry; //Want it dried

            float iX = position.X;
            float iY = position.Y;

            float n1 = iY - 1;              //North 1
            float s1 = iY + 1;              //South 1
            float e1 = iX + 1;              //East 1
            float w1 = iX - 1;              //West 1

            //Go north
            ChangeWaterState(location, iX, n1, desiredState);

            //Go south
            ChangeWaterState(location, iX, s1, desiredState);

            //Go east
            ChangeWaterState(location, e1, iY, desiredState);

            //Go west
            ChangeWaterState(location, w1, iY, desiredState);

        }

        //Do sprinkler logic
        void ActivateSprinkler(GameLocation location, Vector2 position)
        {
            Monitor.Log("Sprinkler activation begun.");

            ////Just give up if we're stuck with a default sprinkler due to pattern def error
            //if (!noProb)
            //{
            //    Monitor.Log("Had to quit sprinkler activation due to a pattern definition issue.", LogLevel.Warn);
            //    return;
            //}

            int desiredState = HoeDirt.watered;     //Want it wet

            float iX = position.X;
            float iY = position.Y;

            //Start by clearing any default behavior.
            //UnWaterDefault(location, iX, iY);

            Monitor.Log(String.Format("Actiating sprinkler in " + location.ToString() + " at {0},{1}", iX, iY));

            //Water stuff per pattern.
            WalkThroughPattern(location, toWater.myPattern, iX, iY, desiredState);

            Monitor.Log("Sprinkler activation complete.");

        }

        //walk through the sprinkler dimensions for a given pattern.
        void WalkThroughPattern(GameLocation location, int[] toUse, float X, float Y, int desiredState)
        {
            //Monitor.Log("Walkthrough begun.");
            //Monitor.Log(String.Format("Walking through pattern in " + location.ToString() + " at {0},{1}", X, Y));

            int i = 0;
            int j;
            foreach (int n in toUse)
            {
                j = toUse[i];
                while (j > 0)
                {
                    //North
                    if (i == 0) { ChangeWaterState(location, X, Y - j, desiredState); }
                    //South
                    if (i == 1) { ChangeWaterState(location, X, Y + j, desiredState); }
                    //East
                    if (i == 2) { ChangeWaterState(location, X + j, Y, desiredState); }
                    //West
                    if (i == 3) { ChangeWaterState(location, X - j, Y, desiredState); }

                    j--;
                }
                i++;
            }
            //Monitor.Log("Walkthrough complete.");
        }

        //Change the watered state to the given value
        void ChangeWaterState(GameLocation location, float X, float Y, int newState)
        {
            Vector2 position = new Vector2(X, Y);
            if (location.terrainFeatures.ContainsKey(position) && location.terrainFeatures[position] is HoeDirt)
            {
                (location.terrainFeatures[position] as HoeDirt).state.Value = newState;
            }
        }

        //Sets to "unwatered" the default area of a basic sprinkler. UNUSED.
        //void UnWaterDefault(GameLocation location, float iX, float iY)
        //{
        //    int desiredState = HoeDirt.dry; //Want it dried
        //
        //    float n1 = iY - 1;              //North 1
        //    float s1 = iY + 1;              //South 1
        //    float e1 = iX + 1;              //East 1
        //    float w1 = iX - 1;              //West 1
        //
        //    //Go north
        //    ChangeWaterState(location, iX, n1, desiredState);
        //
        //    //Go south
        //    ChangeWaterState(location, iX, s1, desiredState);
        //
        //    //Go east
        //    ChangeWaterState(location, e1, iY, desiredState);
        //
        //    //Go west
        //    ChangeWaterState(location, w1, iY, desiredState);
        //}

        /// <summary>Get all game locations.</summary>
        public static IEnumerable<GameLocation> GetAllGameLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
    }
}
