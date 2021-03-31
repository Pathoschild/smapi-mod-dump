/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace ImJustMatt.CustomBundles.Models
{
    public class BundleItem
    {
        /// <summary>ParentSheetIndex of Object needed for bundle</summary>
        public int ObjectId { get; set; }

        /// <summary>The number of items needed</summary>
        public int NumberNeeded { get; set; }

        /// <summary>Minimum quality required for items</summary>
        public int MinimumQuality { get; set; }
    }
}