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
    class KeepGrassInLobby : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Grass), "seasonUpdate");

        public static bool Prefix(Grass __instance)
        {
            if (__instance.currentLocation.Name != "Mountain")
                return true;

            return __instance.currentTileLocation.X < 95;
        }
    }
}
