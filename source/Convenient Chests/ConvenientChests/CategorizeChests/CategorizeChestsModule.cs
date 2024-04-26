/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using System.IO;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.CategorizeChests.Framework.Persistence;
using ConvenientChests.CategorizeChests.Interface;
using ConvenientChests.CategorizeChests.Interface.Widgets;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests {
    public class CategorizeChestsModule : Module {
        internal IItemDataManager ItemDataManager { get; } = new ItemDataManager();
        internal IChestDataManager ChestDataManager { get; } = new ChestDataManager();
        internal ChestFinder ChestFinder { get; } = new ChestFinder();

        protected string SavePath => Path.Combine("savedata", $"{Constants.SaveFolderName}.json");
        protected string AbsoluteSavePath => Path.Combine(ModEntry.Helper.DirectoryPath, SavePath);

        private SaveManager SaveManager { get; set; }
        private WidgetHost WidgetHost { get; set; }

        internal bool ChestAcceptsItem(Chest chest, Item item) => ChestAcceptsItem(chest, item.ToBase().ToItemKey());
        private bool ChestAcceptsItem(Chest chest, ItemKey itemKey)
            => !ItemBlacklist.Includes(itemKey) && ChestDataManager.GetChestData(chest).Accepts(itemKey);

        public CategorizeChestsModule(ModEntry modEntry) : base(modEntry) {
        }

        public override void Activate() {
            IsActive = true;

            // Menu Events
            Events.Display.MenuChanged += OnMenuChanged;

            if (Context.IsMultiplayer && !Context.IsMainPlayer) {
                ModEntry.Log(
                             "Due to limitations in the network code, CHEST CATEGORIES CAN NOT BE SAVED as farmhand, sorry :(",
                             LogLevel.Warn);
                return;
            }

            // Save Events
            SaveManager = new SaveManager(ModEntry.ModManifest.Version, this);
            Events.GameLoop.Saving += OnSaving;
            OnGameLoaded();
        }

        public override void Deactivate() {
            IsActive = false;

            // Menu Events
            this.Events.Display.MenuChanged -= OnMenuChanged;

            // Save Events
            this.Events.GameLoop.Saving -= OnSaving;
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e) {
            try {
                SaveManager.Save(SavePath);
            }
            catch (Exception ex) {
                Monitor.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        private void OnGameLoaded() {
            try {
                if (File.Exists(AbsoluteSavePath))
                    SaveManager.Load(SavePath);
            }
            catch (Exception ex) {
                Monitor.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (e.NewMenu == e.OldMenu)
                return;

            if (e.OldMenu is ItemGrabMenu)
                ClearMenu();

            if (e.NewMenu is ItemGrabMenu itemGrabMenu)
                CreateMenu(itemGrabMenu);
        }

        private void CreateMenu(ItemGrabMenu itemGrabMenu) {
            if (itemGrabMenu.context is not Chest chest)
                return;

            WidgetHost = new WidgetHost(this.Events, this.ModEntry.Helper.Input, this.ModEntry.Helper.Reflection);
            var overlay = new ChestOverlay(this, chest, itemGrabMenu);
            WidgetHost.RootWidget.AddChild(overlay);
        }

        private void ClearMenu() {
            WidgetHost?.Dispose();
            WidgetHost = null;
        }
    }
}