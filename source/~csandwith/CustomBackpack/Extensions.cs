/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley.Menus;

namespace CustomBackpack
{
    public static class Extensions
    {
        public static int Columns(this InventoryMenu menu)
        {
            return menu.capacity / menu.rows;
        }
        public static int GetOffset(this InventoryMenu menu)
        {
            return ModEntry.scrolled.Value * menu.capacity / menu.rows;
        }
        public static bool Scrolling(this InventoryMenu menu)
        {
            return menu.actualInventory is not null && menu.capacity < menu.actualInventory.Count;
        }
    }
}