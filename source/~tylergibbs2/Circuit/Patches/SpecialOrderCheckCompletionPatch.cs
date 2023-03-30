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
    [HarmonyPatch(typeof(SpecialOrder), nameof(SpecialOrder.CheckCompletion))]
    internal class SpecialOrderCheckCompletionPatch
    {
        public static void Postfix(SpecialOrder __instance)
        {
            if (!ModEntry.ShouldPatch())
                return;

            if (__instance.questState.Value == SpecialOrder.QuestState.Complete)
                ModEntry.Instance.TaskManager?.OnSpecialOrderCompleted();
        }
    }
}
