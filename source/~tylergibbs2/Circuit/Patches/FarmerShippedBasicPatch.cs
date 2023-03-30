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
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.shippedBasic))]
    internal class FarmerShippedBasicPatch
    {
        public static void Postfix(int index)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnItemShipped(index);
        }
    }
}
