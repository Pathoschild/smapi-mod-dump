using MegaStorage.Models;
using MegaStorage.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage
{
    public class MenuChanger
    {
        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        public MenuChanger(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void Start()
        {
            _modHelper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            _monitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is LargeItemGrabMenu)
                return;
            if (!(e.NewMenu is ItemGrabMenu itemGrabMenu) || !(itemGrabMenu.context is CustomChest customChest))
                return;
            Game1.activeClickableMenu = customChest.GetItemGrabMenu();
        }

    }
}
