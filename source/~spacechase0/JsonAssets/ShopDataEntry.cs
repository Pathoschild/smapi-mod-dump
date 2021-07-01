/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;

namespace JsonAssets
{
    public class ShopDataEntry
    {
        public string PurchaseFrom;
        public int Price;
        public string[] PurchaseRequirements;
        public Func<ISalable> Object;
        public bool ShowWithStocklist = false;
    }
}
