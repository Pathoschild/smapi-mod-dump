using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.IO;
using StardewModdingAPI;
using MTN.FarmInfo;

namespace MTN
{
    /// <summary>
    /// This class is the only class that will cross between patches, methods, and other classes. It acts as the delivery mechanism of
    /// needed information for MTN to perform nessecarily.
    /// </summary>
    public static class Memory
    {
        //Used to pass SMAPI's helper around
        public static Mod instance;
        public static MTNMultiplayer multiplayer;

        //Information pertaining to custom farms. Index 0 to 4 will contain canon farms within customFarms.
        public static List<CustomFarmEntry> customFarms = new List<CustomFarmEntry>();
        public static CustomFarmType loadedFarm;
        public static CustomFarmEntry selectedFarm;

        //Map Listings
        public static List<additionalMap<Farm>> farmMaps = new List<additionalMap<Farm>>();

        //Information pertaining to custom exterior designs.
        public static List<HouseExteriorDesign> designList = new List<HouseExteriorDesign>();

        //Settings
        public static bool allowCustomizableFarmHouse = true;
        public static bool noDebrisRequested = false;
        public static int foundAtIndex = 0;

        //General Directory
        public static string MapFolder = Path.Combine("Mods", "MTN", "Maps");

        //Internal Signals.
        public static int mapLoadSignal = 0;
        public static int signal = 0; //Generic
        public static bool isCustomFarmLoaded = false;
        public static bool isFarmHouseRelocated = false;
        public static bool isGreenHouseRelocated = false;
        public static bool isShippBinRelocated = false;
        public static bool isMailBoxRelocated = false;
        public static bool isShrineRelocated = false;
        public static bool isRabbitRelocated = false;
        public static bool isWaterBowlRelocated = false;
        public static bool skipFurniture = false;
        public static bool spawnIntegrityChecked = false;

        //Experimental
        public static int scienceHouseLocInList;
        public static GameLocation lastPlace;

        //Should use C#'s properties, but we're too used to ISO C++
        public static void updateSelectedFarm(string name)
        {
            selectedFarm = getEntryByName(name);
        }

        public static int getFarmIdByName(string name)
        {
            int i;
            int results = -1;
            for (i = 0; i < customFarms.Count; i++)
            {
                if (customFarms[i].Name == name)
                {
                    results = customFarms[i].ID;
                    foundAtIndex = i;
                    break;
                }
            }
            return results;
        }

        public static string getFarmNameById(int id)
        {
            int i;
            string results = "Farm";

            if (id == -1) return "Farm";

            for (i = 0; i < customFarms.Count; i++)
            {
                if (customFarms[i].ID == id)
                {
                    results = customFarms[i].Name;
                    break;
                }
            }
            return results;
        }

        public static CustomFarmEntry getEntryById(int id)
        {
            int i;

            for (i = 0; i < customFarms.Count; i++)
            {
                if (customFarms[i].ID == id)
                {
                    return customFarms[i];
                }
            }
            return null;
        }

        public static CustomFarmEntry getEntryByName(string name)
        {
            int i;
            for (i = 0; i < customFarms.Count; i++)
            {
                if (customFarms[i].Name == name)
                {
                    return customFarms[i];
                }
            }
            return null;
        }

        public static void loadCustomFarmType(int id)
        {
            if (id < 5)
            {
                isCustomFarmLoaded = false;
                return;
            }
            
            isCustomFarmLoaded = true;
            CustomFarmEntry entry = getEntryById(id);
            JsonSerializer serializer = new JsonSerializer();
            string jsonFile = Path.Combine(entry.contentpack.DirectoryPath, "farmType.json");
            if (File.Exists(jsonFile)) {
                using (StreamReader sr = new StreamReader(@jsonFile)) {
                    using (JsonReader reader = new JsonTextReader(sr)) {
                        loadedFarm = (CustomFarmType)serializer.Deserialize(reader, typeof(CustomFarmType));
                        loadedFarm.contentpack = entry.contentpack;
                        if (loadedFarm.farmHouse != null) isFarmHouseRelocated = true;
                        if (loadedFarm.greenHouse != null) isGreenHouseRelocated = true;
                        if (loadedFarm.shippingBin != null) isShippBinRelocated = true;
                        if (loadedFarm.mailBox != null) isMailBoxRelocated = true;
                        if (loadedFarm.grandpaShrine != null) isShrineRelocated = true;
                        if (loadedFarm.rabbitStatue != null) isRabbitRelocated = true;
                        if (loadedFarm.petWaterBowl != null) isWaterBowlRelocated = true;
                    }
                }
            } else {
                instance.Monitor.Log($"Could not find farmType.json in {entry.contentpack.DirectoryPath}! Cannot load map!", LogLevel.Error);
            }
        }
    }
}
