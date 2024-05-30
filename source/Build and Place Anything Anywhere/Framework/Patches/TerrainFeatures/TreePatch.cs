/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using StardewValley.TerrainFeatures;

namespace AnythingAnywhere.Framework.Patches.TerrainFeatures
{
    internal sealed class TreePatch : PatchHelper
    {
        internal TreePatch() : base(typeof(Tree)) { }
        internal void Apply()
        {
            Patch(PatchType.Postfix, nameof(Tree.IsGrowthBlockedByNearbyTree), nameof(IsGrowthBlockedByNearbyTreePostfix));
        }

        public static void IsGrowthBlockedByNearbyTreePostfix(Tree __instance, ref bool __result)
        {
            if (ModEntry.Config.EnableWildTreeTweaks)
                __result = false;
        }
    }
}
