/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/CustomiseChildBedroom
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomiseChildBedroom
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /*****************************/
        /**      Properties         **/
        /*****************************/
        ///<summary>The config file from the player</summary>
        internal static ModConfig Config;
        private Commands Commands;

        internal new static IMonitor Monitor;
        internal new static IModHelper Helper;


        /*****************************/
        /**      Public methods     **/
        /*****************************/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Helper = base.Helper;
            Monitor = base.Monitor;

            Config = Helper.ReadConfig<ModConfig>();

            HarmonyInstance harmony = HarmonyInstance.Create("speshkitty.childbedconfig.harmony");

            harmony.Patch(typeof(QuestionEvent).GetMethod("setUp", BindingFlags.Public | BindingFlags.Instance),
                prefix: new HarmonyMethod(typeof(Patch).GetMethod("PreventPregnancy")));

            Commands = new Commands(Helper);

            if (Config.Farms.Count == 0)
            {
                Config.Farms.Add(new Farm());
            }
            
            //Run daily for when commands are ran
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            bool MadeChanges = false;
            Farm Farm = GetFarmInfo();
            if (Farm.FarmName.Equals(string.Empty))
            {
                string FarmName = Game1.player.farmName.Value;
                Log(Translation.GetString("error.farmnamenotfound", new { FarmName }));
                Log(Translation.GetString("error.creatingconfigentry"));
                Farm.FarmName = FarmName;
                Config.Farms.Add(Farm);
                MadeChanges = true;
            }

            List<GameLocation> buildings = new List<GameLocation>() { Game1.getLocationFromName("FarmHouse") };
            buildings.AddRange(getCabins());

            foreach (GameLocation building in buildings)
            {
                if (!(building is FarmHouse)) // Cabins are a type of farmhouse
                {
                    continue;
                }

                FarmHouse farmHouse = building as FarmHouse;

                int BuildingUpgradeLevel = farmHouse.GetLevel();
                string HouseOwner = farmHouse.GetOwner();
                
                if (HouseOwner.Equals(string.Empty)) { continue; } //Building doesn't have an owner

                if (!Farm.FarmerInfo.TryGetValue(HouseOwner, out Farmer BuildingOwner)) //No config entry for farmer name - Make a default one and continue working
                {
                    BuildingOwner = new Farmer();

                    Config.Farms[Config.Farms.IndexOf(Farm)].FarmerInfo.Add(HouseOwner, BuildingOwner);
                    MadeChanges = true;

                    Farm.FarmerInfo[HouseOwner] = BuildingOwner;
                }

                if (BuildingUpgradeLevel <= 1) //Building doesn't have bedrooms 
                {
                    //This check is after attempting to get info because this way it generates a config file block for every farmhand, not just those who have already upgraded
                    continue;
                }

                if (!BuildingOwner.ShowCrib)
                {
                    farmHouse.RemoveCrib();
                }

                if (!BuildingOwner.ShowLeftBed)
                {
                    farmHouse.RemoveLeftBed();
                }

                if (!BuildingOwner.ShowRightBed)
                {
                    farmHouse.RemoveRightBed();
                }

            }

            if (MadeChanges)
            {
                Config.Save(); //Write any changes made
            }
        }

        internal static void Log(object text, LogLevel logLevel = LogLevel.Info)
            => Monitor.Log(text.ToString(), logLevel);

        internal static void LogError(object text)
            => Log(text, LogLevel.Error);


        private Farm GetFarmInfo()
            => Config.Farms.Find(x => x.FarmName == Game1.player.farmName.Value) ?? new Farm();
        

        public static List<GameLocation> getCabins()
        {
            StardewValley.Farm farm = Game1.getLocationFromName("Farm") as StardewValley.Farm;
            List<GameLocation> cabins = new List<GameLocation>();

            farm.buildings.Where(x => x.isCabin).All(i => { cabins.Add(i.indoors.Value); return true; });

            return cabins;
        }

    }
}