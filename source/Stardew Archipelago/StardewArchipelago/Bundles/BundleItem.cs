/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Bundles
{
    public class BundleItem
    {
        private static readonly Dictionary<string, int> QualityTable = new() { {"Basic", 0}, {"Silver", 1}, {"Gold", 2}, {"Iridium", 3} };
        public StardewObject StardewObject { get; set; }
        public int Amount { get; set; }
        public int Quality { get; set; }

        public BundleItem(StardewItemManager itemManager, string itemName, int amount, string quality)
        {
            StardewObject = itemManager.GetObjectByName(itemName);
            Amount = amount;
            Quality = QualityTable[quality];
        }
    }
}