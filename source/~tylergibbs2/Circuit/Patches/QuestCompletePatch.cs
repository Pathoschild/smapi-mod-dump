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
using StardewValley.Quests;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Quest), nameof(Quest.questComplete))]
    internal class QuestCompletePatch
    {
        public static void Postfix(Quest __instance)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnJournalQuestComplete(__instance.id.Value);
        }
    }
}
