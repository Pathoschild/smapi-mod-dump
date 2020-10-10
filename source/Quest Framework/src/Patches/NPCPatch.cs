/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Harmony;
using PurrplingCore.Patching;
using QuestFramework.Framework;
using QuestFramework.Offers;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    class NPCPatch : Patch<NPCPatch>
    {
        public override string Name => nameof(NPCPatch);

        QuestManager QuestManager { get; }
        QuestOfferManager ScheduleManager { get; }

        public NPCPatch(QuestManager questManager, QuestOfferManager scheduleManager)
        {
            this.QuestManager = questManager;
            this.ScheduleManager = scheduleManager;
            Instance = this;
        }

        private static bool Before_checkAction(NPC __instance, Farmer who, ref bool __result)
        {
            try
            {
                if (__instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove || who.isRidingHorse())
                    return true;

                if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
                    return true;

                Instance.Monitor.VerboseLog($"Checking for new quest from NPC `{__instance.Name}`.");

                var schedules = Instance.ScheduleManager.GetMatchedOffers<NpcOfferAttributes>("NPC");
                var schedule = schedules.FirstOrDefault();
                var quest = schedule != null ? Instance.QuestManager.Fetch(schedule.QuestName) : null;

                if (quest != null)
                {
                    if (schedule.OfferDetails.NpcName != __instance.Name)
                        return true;

                    if (quest == null || Game1.player.hasQuest(quest.id))
                        return true;

                    if (string.IsNullOrEmpty(schedule.OfferDetails.DialogueText))
                        return true;

                    Game1.drawDialogue(__instance, $"{schedule.OfferDetails.DialogueText}[quest:{schedule.QuestName.Replace('@', ' ')}]");
                    __result = true;

                    Instance.Monitor.Log($"Getting new quest `{quest.GetFullName()}` to quest log from NPC `{__instance.Name}`.");

                    return false;
                }
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Instance.Before_checkAction));
            }

            return true;
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.Before_checkAction))
            );
        }
    }
}
