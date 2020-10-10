/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

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