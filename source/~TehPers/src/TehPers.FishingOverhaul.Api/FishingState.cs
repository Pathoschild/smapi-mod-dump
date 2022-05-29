/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The state of a user that is fishing.
    /// </summary>
    public abstract record FishingState
    {
        private FishingState() { }

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> is not fishing.
        /// </summary>
        public sealed record NotFishing : FishingState;

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> is waiting for a bite.
        /// </summary>
        public sealed record WaitingForBite(FishingInfo FishingInfo) : FishingState;

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> is in the fishing minigame.
        /// </summary>
        public sealed record Fishing(FishingInfo FishingInfo, NamespacedKey FishKey) : FishingState;

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> just caught something.
        /// </summary>
        public sealed record Caught(FishingInfo FishingInfo, CatchInfo Catch) : FishingState;

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> is holding a catch.
        /// </summary>
        public sealed record Holding(FishingInfo FishingInfo, CatchInfo Catch) : FishingState;

        /// <summary>
        /// A fishing state that represents when a <see cref="Farmer"/> is opening treasure.
        /// </summary>
        public sealed record OpeningTreasure : FishingState;

        /// <summary>
        /// Creates the initial state.
        /// </summary>
        public static FishingState Start()
        {
            return new NotFishing();
        }
    }
}
