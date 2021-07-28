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
using System.Collections.Generic;
using System.Linq;
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
        public string[] PurchaseRequirements { get; set; }
        public Func<ISalable> Object { get; set; }
        public bool ShowWithStocklist { get; set; } = false;


        /*********
        ** Public methods
        *********/
        /// <summary>Format individual requirements for the <see cref="PurchaseRequirements"/> property.</summary>
        /// <param name="requirementFields">The purchase requirements.</param>
        public static string[] FormatRequirements(IList<string> requirementFields)
        {
            return requirementFields?.Any() == true
                ? new[] { string.Join("/", requirementFields) }
                : new string[0];
        }
    }
}
