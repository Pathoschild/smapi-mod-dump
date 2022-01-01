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
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which controls what fish are available to catch.
    /// </summary>
    [JsonDescribe]
    public record FishPack : JsonConfigRoot
    {
        /// <summary>
        /// The fish entries to add.
        /// </summary>
        public ImmutableArray<FishEntry> Add { get; init; } = ImmutableArray<FishEntry>.Empty;

        /// <summary>
        /// Merges all the fish entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with { AddFish = content.AddFish.AddRange(this.Add) };
        }
    }
}