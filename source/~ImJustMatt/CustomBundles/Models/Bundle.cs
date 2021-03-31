/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace ImJustMatt.CustomBundles.Models
{
    public class Bundle
    {
        /// <summary>Name to display for the bundle</summary>
        public string BundleName { get; set; }

        /// <summary>Reward given for completion of the bundle</summary>
        public BundleReward BundleReward { get; set; }

        /// <summary>Donations needed for bundle</summary>
        public IList<BundleItem> BundleItems { get; set; }

        /// <summary>Index number of bundle color</summary>
        public int ColorIndex { get; set; }

        /// <summary>Number of items out of possible needed to complete bundle</summary>
        public int NumberOfItems { get; set; }

        /// <summary>Translated name if playing in non-English language</summary>
        public string TranslatedName { get; set; }

        public string GetData => string.Join("/",
            BundleName,
            $"{BundleReward.ObjectType} {BundleReward.ObjectId} {BundleReward.NumberGiven}",
            string.Join(" ",
                BundleItems.Select(i => $"{i.ObjectId} {i.NumberNeeded} {i.MinimumQuality}")
            ),
            ColorIndex,
            NumberOfItems,
            TranslatedName
        );

        public bool HasData => BundleItems?.Any() ?? false;
    }
}