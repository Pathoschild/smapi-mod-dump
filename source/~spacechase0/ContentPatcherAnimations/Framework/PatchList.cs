/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace ContentPatcherAnimations.Framework
{
    /// <summary>A list of patches for a content pack.</summary>
    internal class PatchList
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The Content Patcher format version.</summary>
        public ISemanticVersion Format { get; set; }

        /// <summary>The loaded Content Patcher patches.</summary>
        public List<Patch> Changes { get; set; }
    }
}
