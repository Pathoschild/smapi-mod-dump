/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.IO;
using StardewValleyMods.CategorizeChests.Framework;
using StardewValleyMods.CategorizeChests.Framework.Persistence;
using StardewValleyMods.CategorizeChests.Interface;
using StardewValleyMods.CategorizeChests.Interface.Widgets;
using StardewValleyMods.Common;

namespace StardewValleyMods.CategorizeChests
{
    public class CategorizeChestsMod : Mod
    {
        private WidgetHost WidgetHost;
        private Config Config;
        private IChestDataManager ChestDataManager;
        private IChestFinder ChestFinder;
        private IChestFiller ChestFiller;
        private IItemDataManager ItemDataManager;
        private ISaveManager SaveManager;

        /// <summary>
        /// Where to keep the mod's per-save data.
        /// </summary>
        private string SaveDirectory;
        private string SavePath;

        /// <summary>
        /// The entry point of the mod.
        /// </summary>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<Config>();

            if (Config.CheckForUpdates)
                new UpdateNotifier(Monitor).Check(ModManifest);

            SaveDirectory = Path.Combine(Helper.DirectoryPath, "savedata");

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            MenuEvents.MenuChanged += OnMenuChanged;
            MenuEvents.MenuClosed += OnMenuClosed;
            SaveEvents.BeforeSave += OnGameSaving;
            SaveEvents.AfterLoad += OnGameLoaded;
        }

        private void OnGameSaving(object sender, EventArgs e)
        {
            try
            {
                SaveManager.Save(SavePath);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        private void OnGameLoaded(object sender, EventArgs e)
        {
            ItemDataManager = new ItemDataManager(Monitor);
            ChestDataManager = new ChestDataManager(ItemDataManager, Monitor);
            ChestFiller = new ChestFiller(ChestDataManager, Monitor);
            ChestFinder = new ChestFinder();
            SaveManager = new SaveManager(ModManifest.Version, ChestDataManager, ChestFinder, ItemDataManager);

            SavePath = Path.Combine(SaveDirectory, Constants.SaveFolderName + ".json");

            try
            {
                if (File.Exists(SavePath))
                {
                    SaveManager.Load(SavePath);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        private void OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.PriorMenu is ItemGrabMenu)
                ClearMenu();

            if (e.NewMenu is ItemGrabMenu itemGrabMenu)
                CreateMenu(itemGrabMenu);
        }

        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is ItemGrabMenu)
                ClearMenu();
        }

        private void CreateMenu(ItemGrabMenu itemGrabMenu)
        {
            var chest = itemGrabMenu.behaviorOnItemGrab?.Target as Chest;

            if (chest != null)
            {
                WidgetHost = new WidgetHost();
                var overlay = new ChestOverlay(itemGrabMenu, chest, Config, ChestDataManager, ChestFiller, ItemDataManager,
                    WidgetHost.TooltipManager);
                WidgetHost.RootWidget.AddChild(overlay);
            }
        }

        private void ClearMenu()
        {
            this.WidgetHost?.Dispose();
            this.WidgetHost = null;
        }
    }
}