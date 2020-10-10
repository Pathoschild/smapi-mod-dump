/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using StardewValley;

namespace ConvenientChests.CategorizeChests.Framework
{
    class DiscoveredItem
    {
        public readonly ItemKey ItemKey;
        public readonly Item Item;

        public DiscoveredItem(ItemType type, int index, Item item)
        {
            ItemKey = new ItemKey(type, index);
            Item = item;
        }
    }
}