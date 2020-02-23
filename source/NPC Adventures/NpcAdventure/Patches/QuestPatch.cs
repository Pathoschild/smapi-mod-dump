using Harmony;
using NpcAdventure.Events;
using NpcAdventure.Internal;
using StardewValley.Quests;

namespace NpcAdventure.Patches
{
    internal class QuestPatch
    {
        private static readonly SetOnce<SpecialModEvents> events = new SetOnce<SpecialModEvents>();
        private static SpecialModEvents Events { get => events.Value; set => events.Value = value; }

        /// <summary>
        /// This patches mailbox read method on gamelocation and allow call custom logic 
        /// for NPC Adventures mail letters only. For other mails call vanilla logic.
        /// </summary>
        /// <param name="__instance">Game location</param>
        /// <returns></returns>
        internal static void After_questComplete(ref Quest __instance)
        {
            Events.FireQuestCompleted(__instance, new QuestCompletedArgs(__instance));
        }

        internal static void After_reloadObjective(ref Quest __instance)
        {
            Events.FireQuestRealoadObjective(__instance, new QuestReloadObjectiveArgs(__instance));
        }

        internal static void Setup(HarmonyInstance harmony, SpecialModEvents events)
        {
            Events = events;

            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_questComplete))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.reloadObjective)),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_reloadObjective))
            );
        }
    }
}
