/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// A possible catch from fishing. This may or may not be a fish.
    /// </summary>
    public abstract record PossibleCatch
    {
        /// <summary>
        /// A fish catch.
        /// </summary>
        public sealed record Fish(FishEntry Entry) : PossibleCatch;

        /// <summary>
        /// A trash catch.
        /// </summary>
        public sealed record Trash(TrashEntry Entry) : PossibleCatch;
    }
}