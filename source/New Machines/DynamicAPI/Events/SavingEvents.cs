using System;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicAPI.Events
{
    public static class SavingEvents
    {
        public static event Action BeforeSaving;
        public static event Action AfterSaving;

        static SavingEvents()
        {
            MenuEvents.MenuChanged += OnMenuChanged;
            MenuEvents.MenuClosed += OnMenuClosed;
        }

        private static void OnMenuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            if (IsSaveGameMenu(args.NewMenu)) BeforeSaving?.Invoke();
        }

        private static void OnMenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (IsSaveGameMenu(args.PriorMenu)) AfterSaving?.Invoke();
        }

        private static bool IsSaveGameMenu(IClickableMenu menu)
        {
            return (menu is SaveGameMenu) || (menu is ShippingMenu);
        }
    }
}