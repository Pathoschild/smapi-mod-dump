using Harmony;
using NpcAdventure.Events;
using NpcAdventure.Internal;
using StardewValley.Quests;
using System;

namespace NpcAdventure.Patches
{
    internal class QuestPatch : Patch<QuestPatch>
    {
        private SpecialModEvents Events { get; set; }
        public override string Name => nameof(QuestPatch);

        public QuestPatch(SpecialModEvents events)
        {
            this.Events = events ?? throw new ArgumentNullException(nameof(events));
            Instance = this;
        }

        /// <summary>
        /// This patches mailbox read method on gamelocation and allow call custom logic 
        /// for NPC Adventures mail letters only. For other mails call vanilla logic.
        /// </summary>
        /// <param name="__instance">Game location</param>
        /// <returns></returns>
        private static void After_questComplete(ref Quest __instance)
        {
            try
            {
                Instance.Events.FireQuestCompleted(__instance, new QuestCompletedArgs(__instance));
            } catch(Exception ex)
            {
                Instance.LogFailure(ex, nameof(After_questComplete));
            }
        }

        private static void After_get_currentObjective(ref Quest __instance, ref string __result)
        {
            try
            {
                Instance.Events.FireQuestReloadObjective(__instance, new QuestReloadObjectiveArgs(__instance));
                if (__instance._currentObjective == null)
                    __instance._currentObjective = "";
                __result = __instance._currentObjective;
            } catch(Exception ex)
            {
                Instance.LogFailure(ex, nameof(After_get_currentObjective));
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_questComplete))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Quest), nameof(Quest.currentObjective)).GetGetMethod(),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_get_currentObjective))
            );
        }
    }
}
