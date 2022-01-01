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
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Config.ContentPacks
{
    /// <summary>
    /// Content which modifies the behavior of fish.
    /// </summary>
    [JsonDescribe]
    public record FishTraitsPack : JsonConfigRoot
    {
        /// <summary>
        /// The fish traits to add.
        /// </summary>
        public ImmutableDictionary<NamespacedKey, FishTraits> Add { get; init; } = ImmutableDictionary<NamespacedKey, FishTraits>.Empty;

        /// <summary>
        /// Merges all the traits into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with { SetFishTraits = content.SetFishTraits.AddRange(this.Add) };
        }
    }
}