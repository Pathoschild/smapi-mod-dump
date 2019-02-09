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