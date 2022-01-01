/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace BattleRoyale.Patches
{
    class NoGoldenWalnuts : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Bush), "shake");

        public static bool Prefix(Bush __instance)
        {
            if (__instance.size.Value == 4)
                return false;

            return true;
        }
    }
}
