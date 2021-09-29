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
using JsonAssets.Framework;
using StardewValley;

namespace JsonAssets
{
    public class ShopDataEntry
    {
        /*********
        ** Accessors
        *********/
        public string PurchaseFrom { get; set; }
        public int Price { get; set; }
        public IParsedConditions PurchaseRequirements { get; set; }
        public Func<ISalable> Object { get; set; }
        public bool ShowWithStocklist { get; set; } = false;
    }
}
