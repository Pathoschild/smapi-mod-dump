/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Tree), nameof(Tree.UpdateTapperProduct))]
    internal class TreeUpdateTapperPatch
    {
        public static void Postfix(Tree __instance)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnTreeUpdateTapperProduct(__instance);
        }
    }
}
