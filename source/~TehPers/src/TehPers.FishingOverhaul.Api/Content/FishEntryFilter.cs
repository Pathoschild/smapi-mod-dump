/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Fish entry filter.
    /// </summary>
    [JsonDescribe]
    public record FishEntryFilter
    {
        /// <summary>
        /// The namespaced key of the fish.
        /// </summary>
        public NamespacedKey? FishKey { get; init; }

        /// <summary>
        /// Checks if the entry matches this filter.
        /// </summary>
        /// <param name="entry">The entry to check.</param>
        /// <returns>Whether the entry matches this filter.</returns>
        public bool Matches(FishEntry entry)
        {
            // Check if the fish key matches
            if (this.FishKey is { } fishKey && fishKey == entry.FishKey)
            {
                return true;
            }

            return false;
        }
    }
}