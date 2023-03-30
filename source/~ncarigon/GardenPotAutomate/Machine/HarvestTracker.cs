/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace GardenPotAutomate {
    /// <summary>
    /// Used to track the first drop from a harvest.
    /// </summary>
    internal class HarvestTracker {
        public Item Item;
        public bool FirstDrop;

        public HarvestTracker(Item item, bool firstDrop = false) {
            Item = item;
            FirstDrop = firstDrop;
        }
    }
}