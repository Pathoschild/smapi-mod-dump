/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections;
using System.Collections.Generic;

namespace ContentPatcherAnimations.Framework
{
    /// <summary>The animation state for a screen.</summary>
    internal class ScreenState
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The raw patches loaded by Content Patcher for all installed content packs.</summary>
        public IEnumerable RawPatches { get; set; }

        /// <summary>The patch and animation data for loaded patches.</summary>
        public Dictionary<Patch, PatchData> AnimatedPatches { get; } = new();

        /// <summary>The assets that were recently drawn to the screen.</summary>
        public AssetDrawTracker AssetDrawTracker { get; } = new();

        /// <summary>The global animation tick counter.</summary>
        public uint FrameCounter { get; set; }
    }
}
