/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    internal class MineShaftDisableRandomBug : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "spawnFlyingMonsterOffScreen");

        public static bool Prefix()
        {
            return false;
        }
    }
}
