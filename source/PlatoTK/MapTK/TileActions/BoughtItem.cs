/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;

namespace MapTK.TileActions
{
    internal class BoughtItem
    {
        public string ItemName { get; set; }

        public int Stock { get; set; }

        public BoughtItem()
        {

        }

        public BoughtItem(ISalable item, int stock)
        {
            ItemName = item.Name;
            Stock = stock;
        }
    }
}
