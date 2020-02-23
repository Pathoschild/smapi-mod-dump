using MegaStorage.Framework.Interface;
using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage
{
    internal class MenuChanger
    {
        public static void Start()
        {
            MegaStorageMod.ModHelper.Events.Display.MenuChanged += OnMenuChanged;
            MegaStorageMod.ModHelper.Events.Display.WindowResized += OnWindowResized;
        }

        private static void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is LargeItemGrabMenu))
            {
                return;
            }

            var oldBounds = new Rectangle
            {
                Width = e.OldSize.X,
                Height = e.OldSize.Y
            };

            var newBounds = new Rectangle
            {
                Width = e.NewSize.X,
                Height = e.NewSize.Y
            };

            ((LargeItemGrabMenu)Game1.activeClickableMenu).gameWindowSizeChanged(oldBounds, newBounds);
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is LargeItemGrabMenu)
            {
                return;
            }

            if (!(e.NewMenu is ItemGrabMenu itemGrabMenu) || !(itemGrabMenu.context is CustomChest customChest))
            {
                return;
            }

            Game1.activeClickableMenu = customChest.GetItemGrabMenu();
        }
    }
}