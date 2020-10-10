/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Tools;

namespace TehPers.FishingOverhaul.Api {
    public class FishingEventArgs : EventArgs {
        /// <summary>The <see cref="Item.ParentSheetIndex"/> of the item being caught.</summary>
        public int ParentSheetIndex { get; set; }

        /// <summary>The farmer who caught the fish.</summary>
        public Farmer Who { get; }

        /// <summary>The fishing rod used to catch the fish.</summary>
        public FishingRod Rod { get; }

        public FishingEventArgs(int parentSheetIndex, Farmer who, FishingRod rod) {
            this.ParentSheetIndex = parentSheetIndex;
            this.Who = who;
            this.Rod = rod;
        }
    }
}