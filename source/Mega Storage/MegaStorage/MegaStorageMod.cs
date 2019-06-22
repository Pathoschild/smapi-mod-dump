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
            modHelper.ReadConfig<ModConfig>();
            modHelper.Content.AssetEditors.Add(new SpritePatcher(Helper, Monitor));
            modHelper.Events.Display.MenuChanged += OnMenuChanged;
            new SaveManager(Helper, Monitor, new ISaver[]
            {
                new InventorySaver(Helper, Monitor),
                new FarmhandInventorySaver(Helper, Monitor),
                new LocationSaver(Helper, Monitor),
                new LocationInventorySaver(Helper, Monitor)
            }).Start();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            Monitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is LargeItemGrabMenu)
                return;
            if (e.NewMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.context is CustomChest customChest)
                Game1.activeClickableMenu = customChest.CreateItemGrabMenu();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new ItemPatcher(Helper, Monitor).Start();
        }

    }
}
