/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jeffgillean/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace FrugalFarmMenu
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper iModHelper)
        {
            iModHelper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs menuChangedEventArgs)
        {
            if (menuChangedEventArgs.NewMenu is null)
            {
                Helper.Events.Display.MenuChanged -= OnMenuChanged;
                Helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
            }
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs renderedActiveMenuEventArgs)
        {
            Helper.Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs renderingActiveMenuEventArgs)
        {
            try
            {
                if (Game1.activeClickableMenu is GameMenu gameMenu)
                {
                    ReplaceAll(
                        gameMenu.pages,
                        page => page is StardewValley.Menus.InventoryPage and not InventoryPage,
                        page => new InventoryPage(this, page.xPositionOnScreen, page.yPositionOnScreen, page.width, page.height));
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to replace Inventory Page:\n{ex}", LogLevel.Error);
            }

            Helper.Events.Display.RenderingActiveMenu -= OnRenderingActiveMenu;
            Helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        }

        private static void ReplaceAll<T>(List<T> list, Predicate<T> match, Func<T, T> map)
        {
            list.FindAll(match).ConvertAll(list.IndexOf).ForEach(i => list[i] = map.Invoke(list[i]));
        }
    }
}