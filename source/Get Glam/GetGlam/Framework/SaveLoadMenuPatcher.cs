using GetGlam.Framework.DataModels;
using SFarmer = StardewValley.Farmer;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley;
using System.Threading.Tasks;
using System;

namespace GetGlam.Framework
{
    public class SaveLoadMenuPatcher
    {
        ///************************************
        /// This class is based off of:
        /// https://github.com/KabigonFirst/StardewValleyMods/blob/master/Kisekae/Framework/LoadMenuPatcher.cs
        /// It uses the same logic, with different edits here and there for the changes in Get Glam.
        ///************************************

        //Instance of ModEntry
        private ModEntry Entry;

        //Instance of CharacterLoader
        private CharacterLoader PlayerLoader;

        //List of farmers for each save file
        private List<SFarmer> Farmers = new List<SFarmer>();

        //Dictionary that has each config for each farmer
        public Dictionary<string, ConfigModel> FarmerConfigsDictionary = new Dictionary<string, ConfigModel>();

        //The per save cofnig path
        public string PerSaveConfigPath => Constants.SaveFolderName != null
            ? Path.Combine("Saves", $"{Constants.SaveFolderName}_current.json")
            : null;

        //The prvious load menu to restore if they return to title
        private IClickableMenu PreviousLoadMenu;

        //Wether the task was started for finding saves
        private bool TaskStarted = false;

        /// <summary>SaveLoadMenuPatcher's Constructor</summary>
        /// <param name="entry">The instance of ModEntry</param>
        /// <param name="playerLoader">The instance of CharacterLoader</param>
        public SaveLoadMenuPatcher(ModEntry entry, CharacterLoader playerLoader)
        {
            //Set the fields
            Entry = entry;
            PlayerLoader = playerLoader;
        }

        /// <summary>Initialize the class by adding helpers and reading configs</summary>
        public void Init()
        {
            //Read the local configs when the class is initialized
            ReadLocalConfigs();

            //Add the helpers needed to patch the menu
            Entry.Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            Entry.Helper.Events.GameLoop.UpdateTicked += OnUpdateTickedPatchLoadMenu;
        }

        /// <summary>Event to handle the Load Menu if the player returns to the title</summary>
        /// <param name="sender">The object</param>
        /// <param name="e">The returned to title event argument</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            //Reset the state
            PreviousLoadMenu = null;
            Farmers.Clear();

            //Load the per saves configs
            FarmerConfigsDictionary.Clear();
            ReadLocalConfigs();

            //Restore the load-menu patcher
            Entry.Helper.Events.GameLoop.UpdateTicked += OnUpdateTickedPatchLoadMenu;
        }

        /// <summary>Update Ticked event that checks if the loadmenu is up</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateTickedPatchLoadMenu(object sender, UpdateTickedEventArgs e)
        {
            //Make sure the game is loaded
            if (!Game1.hasLoadedGame)
            {
                if (Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu is LoadGameMenu loadMenu)
                {
                    if (loadMenu == null || loadMenu == PreviousLoadMenu)
                        return;

                    //Path the load menu
                    PatchLoadMenu(loadMenu);
                }
                return;
            }

            //Clear the farmer list and dictionary
            Farmers.Clear();
            FarmerConfigsDictionary.Clear();

            //If the save path is not empty then remove the event
            if (!string.IsNullOrEmpty(PerSaveConfigPath))
                Entry.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTickedPatchLoadMenu;
        }

        /// <summary>Patches the farmer texture in the farmers list</summary>
        private void PatchLoadMenuFarmerTexture()
        {
            //Loop through each farmer in the list
            foreach (SFarmer farmer in Farmers)
            {
                ConfigModel model;
                if (!FarmerConfigsDictionary.TryGetValue(farmer.slotName, out model))
                    continue;

                //Load their layout
                PlayerLoader.LoadFarmersLayoutForLoadMenu(farmer, model);
            }
        }

        /// <summary>Update ticked event that allows the farmer textures to be patched</summary>
        /// <param name="sender">The object</param>
        /// <param name="e">The update ticked event argument</param>
        private void OnUpdateTickedPatchLoadMenuFarmer(object sender, UpdateTickedEventArgs e)
        {
            //If it's not the title menu then remove the event
            if (!(Game1.activeClickableMenu is TitleMenu))
            {
                Entry.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTickedPatchLoadMenuFarmer;
                return;
            }

            //Create a new load menu
            LoadGameMenu loadMenu = TitleMenu.subMenu as LoadGameMenu;
            if (loadMenu == null || loadMenu != PreviousLoadMenu)
            {
                Entry.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTickedPatchLoadMenuFarmer;
                return;
            }

            //Patch the farmer texture and remove the event
            PatchLoadMenuFarmerTexture();
            Entry.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTickedPatchLoadMenuFarmer;
        }

        /// <summary>Patches the load menu</summary>
        /// <param name="menu">The menu to patch</param>
        private void PatchLoadMenu(LoadGameMenu menu)
        {
            //Don't want to patch the coop menu
            if (menu is CoopMenu)
                return;

            //Relfedt into the task that is running
            IReflectedField<Task<List<SFarmer>>> task = Entry.Helper.Reflection.GetField<Task<List<SFarmer>>>(menu, "_initTask");
            if (!TaskStarted)
            {
                if (task == null || task.GetValue() == null)
                    return;

                if (!task.GetValue().IsCompleted)
                {
                    TaskStarted = true;
                    return;
                }
            }
            else
            {
                if (task != null && task.GetValue() != null && !task.GetValue().IsCompleted)
                    return;
            }

            TaskStarted = false;
            PreviousLoadMenu = menu;

            //Find the saved games
            if (!Farmers.Any())
                Farmers = Entry.Helper.Reflection.GetMethod(typeof(LoadGameMenu), "FindSaveGames").Invoke<List<SFarmer>>();

            //Create a new instance of the list
            Type elementType = Type.GetType("StardewValley.Menus.LoadGameMenu+MenuSlot, Stardew Valley");
            Type listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
            object list = Activator.CreateInstance(listType);

            //Set value of the menu slots and invoke addSaveFiles method
            Entry.Helper.Reflection.GetField<object>(menu, "menuSlots").SetValue(list);
            Entry.Helper.Reflection.GetMethod(menu, "addSaveFiles").Invoke(Farmers);

            //Add the Patch Farmer event
            Entry.Helper.Events.GameLoop.UpdateTicked += OnUpdateTickedPatchLoadMenuFarmer;
        }

        /// <summary>Reads the local configs before the game loads</summary>
        /// <returns>Boolean whether it read the config</returns>
        public bool ReadLocalConfigs()
        {
            //See of the saves directory exists
            string savesPath = Constants.SavesPath;
            if (!Directory.Exists(savesPath))
                return false;

            //Create a string array of directories within the save folder
            string[] saveDirectories = Directory.GetDirectories(savesPath);
            if (!saveDirectories.Any())
                return true;

            //Loop through each directory
            foreach (string saveDir in saveDirectories)
            {
                //Get the config path and create a new model for each save
                string localConfigPath = Path.Combine("Saves", $"{new DirectoryInfo(saveDir).Name}_current.json");
                ConfigModel model = Entry.Helper.Data.ReadJsonFile<ConfigModel>(localConfigPath);
                if (model == null)
                {
                    model = new ConfigModel();
                    model.Favorites = new List<FavoriteModel>();
                    for (int i = 0; i < 40; i++)
                        model.Favorites.Add(new FavoriteModel());

                    Entry.Monitor.Log("Creating a new save json.", LogLevel.Debug);
                    Entry.Helper.Data.WriteJsonFile<ConfigModel>(localConfigPath, model);
                }
                //Add the save folder names to the model and add it to the config dictionary
                model.SaveFolderName = new DirectoryInfo(saveDir).Name;
                FarmerConfigsDictionary[model.SaveFolderName] = model;
            }

            //Booyah
            return true;
        }
    }
}
