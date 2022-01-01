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
    /// Content which controls what trash are available to catch.
    /// </summary>
    [JsonDescribe]
    public record TrashPack : JsonConfigRoot
    {
        /// <summary>
        /// The trash entries to add.
        /// </summary>
        public ImmutableArray<TrashEntry> Add { get; init; } = ImmutableArray<TrashEntry>.Empty;

        /// <summary>
        /// Merges all the trash entries into a single content object.
        /// </summary>
        /// <param name="content">The content to merge into.</param>
        public FishingContent AddTo(FishingContent content)
        {
            return content with { AddTrash = content.AddTrash.AddRange(this.Add) };
        }
    }
}