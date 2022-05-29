/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Immutable;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Content which affects fishing.
    /// </summary>
    /// <param name="ModManifest">The manifest of the mod that created this content.</param>
    public record FishingContent(IManifest ModManifest)
    {
        /// <summary>
        /// Gets the fish traits this content is trying to set.
        /// </summary>
        public ImmutableDictionary<NamespacedKey, FishTraits> SetFishTraits { get; init; } =
            ImmutableDictionary<NamespacedKey, FishTraits>.Empty;

        /// <summary>
        /// Gets the fish traits this content is trying to remove.
        /// </summary>
        public ImmutableArray<NamespacedKey> RemoveFishTraits { get; init; } =
            ImmutableArray<NamespacedKey>.Empty;

        /// <summary>
        /// Gets the new fish entries this content wants to create.
        /// </summary>
        public ImmutableArray<FishEntry> AddFish { get; init; } = ImmutableArray<FishEntry>.Empty;

        /// <summary>
        /// Gets the fish entries this content wants to remove.
        /// </summary>
        public ImmutableArray<FishEntryFilter> RemoveFish { get; init; } =
            ImmutableArray<FishEntryFilter>.Empty;

        /// <summary>
        /// Gets the new trash entries this content wants to create.
        /// </summary>
        public ImmutableArray<TrashEntry> AddTrash { get; init; } =
            ImmutableArray<TrashEntry>.Empty;

        /// <summary>
        /// Gets the trash entries this content wants to remove.
        /// </summary>
        public ImmutableArray<TrashEntryFilter> RemoveTrash { get; init; } =
            ImmutableArray<TrashEntryFilter>.Empty;

        /// <summary>
        /// Gets the new treasure entries this content wants to create.
        /// </summary>
        public ImmutableArray<TreasureEntry> AddTreasure { get; init; } =
            ImmutableArray<TreasureEntry>.Empty;

        /// <summary>
        /// Gets the treasure entries this content wants to remove.
        /// </summary>
        public ImmutableArray<TreasureEntryFilter> RemoveTreasure { get; init; } =
            ImmutableArray<TreasureEntryFilter>.Empty;

        /// <summary>
        /// Fishing effects entries to add.
        /// </summary>
        public ImmutableArray<FishingEffectEntry> AddEffects { get; init; } =
            ImmutableArray<FishingEffectEntry>.Empty;
    }
}
