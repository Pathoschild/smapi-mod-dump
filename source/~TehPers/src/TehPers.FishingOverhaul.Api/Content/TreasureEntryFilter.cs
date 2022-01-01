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
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;
using System.Linq;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Treasure entry filter.
    /// </summary>
    [JsonDescribe]
    public record TreasureEntryFilter
    {
        /// <summary>
        /// The namespaced keys of the treasure. This must match every listed item key in the entry
        /// you want to remove. For example, if an entry lists bait, stone, and wood as its
        /// possible item keys, you must list *all* of those to remove it.
        /// </summary>
        public ImmutableArray<NamespacedKey>? ItemKeys { get; init; }

        /// <summary>
        /// A namespaced key in the treasure entry. Any entry that can produce this item will be
        /// removed. This takes precedence over <see cref="ItemKeys"/> (if both are listed and this
        /// condition is matched, then <see cref="ItemKeys"/> is ignored).
        /// </summary>
        public NamespacedKey? AnyWithItem { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(TreasureEntry entry)
        {
            // Check if any of the item keys match
            if (this.AnyWithItem is { } anyWithItem && entry.ItemKeys.Contains(anyWithItem))
            {
                return true;
            }

            // Check if all the item keys match
            if (this.ItemKeys is { } itemKeys
                && itemKeys.Length == entry.ItemKeys.Length
                && itemKeys.All(entry.ItemKeys.Contains))
            {
                return true;
            }

            return false;
        }
    }
}