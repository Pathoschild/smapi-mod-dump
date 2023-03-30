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
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    internal class NPCCheckActionPatch
    {
        public static void Postfix(NPC __instance, bool __result, Farmer who)
        {
            if (!ModEntry.ShouldPatch() || !__result)
                return;

            ModEntry.Instance.TaskManager?.OnNPCCheckAction(__instance, who);
        }
    }
}
