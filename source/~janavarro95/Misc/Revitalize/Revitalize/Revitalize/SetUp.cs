using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revitalize;
using System.IO;
using StardewValley;
using Revitalize.Resources;
using Revitalize.Persistence;
using StardewModdingAPI;

namespace Revitalize
{

    /// <summary>
    /// A class used to set everything up at initialization time instead of having everything spaggetti codded about.
    /// </summary>
    class SetUp
    {
        public static void DuringEntry()
        {
            Class1.hasLoadedTerrainList = false;
            
            Class1.path = Class1.modHelper.DirectoryPath;
            Class1.newLoc = new List<GameLoc>();


            PlayerVariables.initializePlayerVariables();
            //Log.AsyncG("Revitalize: Running on API Version: " + StardewModdingAPI.Constants.ApiVersion);

            Lists.loadAllListsAtEntry();

            Settings.SettingsManager.Initialize();
            Settings.SettingsManager.LoadAllSettings();
            Settings.SettingsManager.SaveAllSettings();
        }

        public static void AfterGameHasLoaded()
        {
            Class1.modContent = new List<LocalizedContentManager>();//new LocalizedContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
            Dictionaries.initializeDictionaries();
            Lists.initializeAllLists();

           Class1.mapWipe = false;
        }

        /// <summary>
        /// Create all the directories necessary to run this mod.
        /// </summary>
        public static void createDirectories()
        {

            Serialize.DataDirectoryPath = Path.Combine(Class1.path, "PlayerData");
            Serialize.PlayerDataPath = Path.Combine(Serialize.DataDirectoryPath, Game1.player.name);
            Serialize.InvPath = Path.Combine(Serialize.PlayerDataPath, "Inventory");
            Serialize.objectsInWorldPath = Path.Combine(Serialize.PlayerDataPath, "objects");
            Serialize.SerializerTrashPath = Path.Combine(Class1.path, "SerializerTrash");

            // Log.AsyncC(TrackedTerrainDataPath);

            if (!Directory.Exists(Serialize.DataDirectoryPath))
            {
                Directory.CreateDirectory(Serialize.DataDirectoryPath);
            }
            if (!Directory.Exists(Serialize.PlayerDataPath))
            {
                Directory.CreateDirectory(Serialize.PlayerDataPath);
            }
            if (!Directory.Exists(Serialize.InvPath))
            {
                Directory.CreateDirectory(Serialize.InvPath);
            }
            if (!Directory.Exists(Serialize.objectsInWorldPath))
            {
                Directory.CreateDirectory(Serialize.InvPath);
            }
            if (!Directory.Exists(Serialize.SerializerTrashPath))
            {
                Directory.CreateDirectory(Serialize.SerializerTrashPath);
            }


            foreach (GameLocation loc in Game1.locations)
            {
                if (!Directory.Exists(Path.Combine(Serialize.objectsInWorldPath, loc.name)))
                {
                    Directory.CreateDirectory(Path.Combine(Serialize.objectsInWorldPath, loc.name));
                }

            }
        }
    }
}
