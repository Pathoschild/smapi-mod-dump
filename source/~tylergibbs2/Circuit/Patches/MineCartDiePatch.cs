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
using StardewValley.Minigames;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(MineCart), nameof(MineCart.Die))]
    internal class MineCartDiePatch
    {
        public static void Postfix()
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnMineCartDied();
        }
    }
}
