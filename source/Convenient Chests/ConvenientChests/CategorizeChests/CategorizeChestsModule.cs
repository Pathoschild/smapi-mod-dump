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
        internal IItemDataManager  ItemDataManager  { get; } = new ItemDataManager();
        internal IChestDataManager ChestDataManager { get; } = new ChestDataManager();
        internal ChestFinder       ChestFinder      { get; } = new ChestFinder();

        protected string      SaveDirectory => Path.Combine(ModEntry.Helper.DirectoryPath, "savedata");
        protected string      SavePath      => Path.Combine(ModEntry.Helper.DirectoryPath, "savedata", $"{Constants.SaveFolderName}.json");
        private   SaveManager SaveManager   { get; set; }


        private WidgetHost WidgetHost { get; set; }

        internal bool ChestAcceptsItem(Chest chest, Item    item)    => item != null && ChestAcceptsItem(chest, ItemDataManager.GetItemKey(item));
        internal bool ChestAcceptsItem(Chest chest, ItemKey itemKey) => ChestDataManager.GetChestData(chest).Accepts(itemKey);

        public CategorizeChestsModule(ModEntry modEntry) : base(modEntry) {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
        }

        public override void Activate() {
            IsActive = true;

            // Menu Events
            MenuEvents.MenuChanged += OnMenuChanged;
            MenuEvents.MenuClosed  += OnMenuClosed;

            if (Context.IsMultiplayer && !Context.IsMainPlayer) {
                ModEntry.Log("Due to limitations in the network code, CHEST CATEGORIES CAN NOT BE SAVED as farmhand, sorry :(", LogLevel.Warn);
                return;
            }

            // Save Events
            SaveManager           =  new SaveManager(ModEntry.ModManifest.Version, this);
            SaveEvents.BeforeSave += OnGameSaving;
            OnGameLoaded(this, EventArgs.Empty);
        }

        public override void Deactivate() {
            IsActive = false;

            // Menu Events
            MenuEvents.MenuChanged -= OnMenuChanged;
            MenuEvents.MenuClosed  -= OnMenuClosed;

            // Save Events
            SaveEvents.BeforeSave -= OnGameSaving;
            SaveEvents.AfterLoad  -= OnGameLoaded;
        }

        private void OnGameSaving(object sender, EventArgs e) {
            try {
                SaveManager.Save(SavePath);
            }
            catch (Exception ex) {
                Monitor.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        private void OnGameLoaded(object sender, EventArgs e) {
            try {
                if (File.Exists(SavePath))
                    SaveManager.Load(SavePath);
            }
            catch (Exception ex) {
                Monitor.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
                Monitor.Log(ex.ToString());
            }
        }

        private void OnMenuChanged(object sender, EventArgsClickableMenuChanged e) {
            if (e.NewMenu == e.PriorMenu)
                return;

            if (e.PriorMenu is ItemGrabMenu)
                ClearMenu();

            if (e.NewMenu is ItemGrabMenu itemGrabMenu)
                CreateMenu(itemGrabMenu);
        }

        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed e) {
            if (e.PriorMenu is ItemGrabMenu)
                ClearMenu();
        }

        private void CreateMenu(ItemGrabMenu itemGrabMenu) {
            if (!(itemGrabMenu.behaviorOnItemGrab?.Target is Chest chest))
                return;

            WidgetHost = new WidgetHost();
            var overlay = new ChestOverlay(this, chest, itemGrabMenu, WidgetHost.TooltipManager);
            WidgetHost.RootWidget.AddChild(overlay);
        }

        private void ClearMenu() {
            WidgetHost?.Dispose();
            WidgetHost = null;
        }
    }
}