using Harmony;
using PurrplingCore.Patching;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    class Game1Patch : Patch<Game1Patch>
    {
        public override string Name => nameof(Game1Patch);

        QuestManager QuestManager { get; }
        QuestOfferManager ScheduleManager { get; }

        public Game1Patch(QuestManager questManager, QuestOfferManager scheduleManager)
        {
            this.QuestManager = questManager;
            this.ScheduleManager = scheduleManager;
            Instance = this;
        }

        private static bool Before_RefreshQuestOfTheDay()
        {
            try
            {
                Instance.Monitor.VerboseLog("Refresh quest of the day");

                if (QuestFrameworkMod.Instance.Status == State.LAUNCHED)
                {
                    var schedules = Instance.ScheduleManager.GetMatchedOffers("Bulletinboard");
                    var schedule = schedules.FirstOrDefault();
                    var quest = schedule != null ? Instance.QuestManager.Fetch(schedule.QuestName) : null;

                    if (quest == null || Game1.player.hasQuest(quest.id))
                        return true;

                    Game1.questOfTheDay = Quest.getQuestFromId(quest.id);
                    Instance.Monitor.Log($"Added quest `{quest.Name}` to bulletin board as quest of the day.");

                    if (schedules.Count() > 1)
                    {
                        Instance.Monitor.Log("Multiple quests scheduled for this time to add on buletin board. First on the list was added, others are ignored.", LogLevel.Warn);
                    }

                    return false;
                }

            } catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Instance.Before_RefreshQuestOfTheDay));
            }

            return true;
        }

        private static void After_CanAcceptDailyQuest(ref bool __result)
        {
            try
            {
                if (Game1.questOfTheDay != null && Game1.questOfTheDay.IsManaged() && Game1.player.hasQuest(Game1.questOfTheDay.id.Value))
                    __result = false;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Game1Patch.After_CanAcceptDailyQuest));
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.RefreshQuestOfTheDay)),
                prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.Before_RefreshQuestOfTheDay))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.CanAcceptDailyQuest)),
                postfix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.After_CanAcceptDailyQuest))
            );
        }
    }
}
