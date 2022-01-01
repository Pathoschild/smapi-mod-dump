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
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Information about a caught item.
    /// </summary>
    /// <param name="FishingInfo">Information about the <see cref="Farmer"/> that caught this.</param>
    /// <param name="Item">The item that was caught.</param>
    /// <param name="FromFishPond">Whether this was caught from a fish pond.</param>
    public abstract record CatchInfo(FishingInfo FishingInfo, Item Item, bool FromFishPond)
    {
        /// <summary>
        /// Information about a caught fish.
        /// </summary>
        /// <inheritdoc cref="CatchInfo"/>
        /// <param name="FishingInfo">Information about the <see cref="Farmer"/> that caught this fish.</param>
        /// <param name="FishEntry">The availability entry for the fish that was caught.</param>
        /// <param name="FishItem">The fish item that was caught.</param>
        /// <param name="FishSize">The size of the caught fish.</param>
        /// <param name="IsLegendary">Whether the fish is legendary.</param>
        /// <param name="FishQuality">The quality level of the fish.</param>
        /// <param name="FishDifficulty">The difficulty of the fish.</param>
        /// <param name="State">The state of the minigame when the fish was caught.</param>
        /// <param name="FromFishPond">Whether this was caught from a fish pond.</param>
        /// <param name="CaughtDouble">Whether two fish were caught instead of just one.</param>
        public sealed record FishCatch(
            FishingInfo FishingInfo,
            FishEntry FishEntry,
            Item FishItem,
            int FishSize,
            bool IsLegendary,
            int FishQuality,
            int FishDifficulty,
            MinigameState State,
            bool FromFishPond,
            bool CaughtDouble = false
        ) : CatchInfo(FishingInfo, FishItem, FromFishPond);

        /// <summary>
        /// Information about a caught trash item.
        /// </summary>
        /// <param name="FishingInfo">Information about the <see cref="Farmer"/> that caught this fish.</param>
        /// <param name="TrashEntry">The availability entry for the trash that was caught.</param>
        /// <param name="TrashItem">The trash item that was caught.</param>
        /// <param name="FromFishPond">Whether this was caught from a fish pond.</param>
        public sealed record TrashCatch(
            FishingInfo FishingInfo,
            TrashEntry TrashEntry,
            Item TrashItem,
            bool FromFishPond
        ) : CatchInfo(FishingInfo, TrashItem, FromFishPond);
    }
}