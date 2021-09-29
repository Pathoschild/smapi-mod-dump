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
    internal class ScreenState
    {
        public IEnumerable CpPatches;

        public Dictionary<Patch, PatchData> AnimatedPatches = new();

        public uint FrameCounter;
        public int FindTargetsCounter;
        public Queue<Patch> FindTargetsQueue = new();
    }
}
