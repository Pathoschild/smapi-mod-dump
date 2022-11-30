/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace BattleRoyale.Patches
{
    class AlwaysPassTime : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Game1), "shouldTimePass");

        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
