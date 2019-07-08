using MegaStorage.Models;
using MegaStorage.Persistence;
using MegaStorage.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage
{
    public class MegaStorageMod : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor Logger;
        public static IReflectionHelper Reflection;

        public override void Entry(IModHelper modHelper)
        {
            Monitor.VerboseLog("Entry of MegaStorageMod");
            ModHelper = modHelper;
            Logger = Monitor;
            Reflection = modHelper.Reflection;

            modHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            modHelper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var itemPatcher = new ItemPatcher(Helper, Monitor);
            var spritePatcher = new SpritePatcher(Helper, Monitor);
            var farmhandMonitor = new FarmhandMonitor(Helper, Monitor);
            var savers = new ISaver[]
            {
                new InventorySaver(Helper, Monitor),
                new FarmhandInventorySaver(Helper, Monitor),
                new LocationSaver(Helper, Monitor),
                new LocationInventorySaver(Helper, Monitor)
            };
            var saveManager = new SaveManager(Helper, Monitor, farmhandMonitor, savers);

            Helper.ReadConfig<ModConfig>();
            Helper.Content.AssetEditors.Add(spritePatcher);
            itemPatcher.Start();
            saveManager.Start();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            Monitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is LargeItemGrabMenu)
                return;
            if (e.NewMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.context is CustomChest customChest)
                Game1.activeClickableMenu = customChest.CreateItemGrabMenu();
        }

    }
}
