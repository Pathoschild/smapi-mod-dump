/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley.Monsters;

namespace StardewRoguelike.Patches
{
    internal class NoArmoredBugPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Bug), "takeDamage");

        public static bool Prefix(Bug __instance)
        {
            __instance.isArmoredBug.Value = false;
            return true;
        }
    }
}
