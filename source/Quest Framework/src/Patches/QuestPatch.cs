using Harmony;
using PurrplingCore.Patching;
using QuestFramework.Framework;
using QuestFramework.Framework.Quests;
using QuestFramework.Quests;
using StardewValley;
using StardewValley.Quests;
using System;

namespace QuestFramework.Patches
{
    internal class QuestPatch : Patch<QuestPatch>
    {
        public override string Name => nameof(QuestPatch);

        QuestManager QuestManager { get; }

        public QuestPatch(QuestManager questManager)
        {
            this.QuestManager = questManager;
            Instance = this;
        }

        private static void After_getQuestFromId(int id, ref Quest __result)
        {
            try
            {
                var manager = QuestFrameworkMod.Instance.QuestManager;
                var customQuest = manager.GetById(id);

                if (__result != null && customQuest != null)
                {
                    if (customQuest.BaseType == QuestType.Monster)
                    {
                        (__result as SlayMonsterQuest).dialogueparts.Clear();
                    }

                    __result.dailyQuest.Value = customQuest.IsDailyQuest();
                    __result.daysLeft.Value = customQuest.DaysLeft;
                    __result.canBeCancelled.Value = customQuest.Cancelable;
                    __result.questType.Value = customQuest.CustomTypeId != -1
                            ? customQuest.CustomTypeId
                            : (int)customQuest.BaseType;
                }
            } catch (Exception e)
            {
                Instance.LogFailure(e, nameof(After_getQuestFromId));
            }
        }

        private static void After_get_currentObjective(Quest __instance, ref string __result)
        {
            try
            {
                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest is IQuestObserver observer)
                {
                    observer.UpdateObjective(
                        new QuestInfo(__instance, Game1.player), 
                        ref __instance._currentObjective);
                }

                if (__instance._currentObjective == null)
                    __instance._currentObjective = "";

                __result = __instance._currentObjective;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(After_get_currentObjective));
            }
        }

        private static bool Before_questComplete(Quest __instance, ref bool __state)
        {
            try
            {
                // save previous completed quest state (used in postfix)
                __state = __instance.completed.Value;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Before_questComplete));
            }

            return true;
        }

        private static void After_questComplete(Quest __instance, bool __state)
        {
            try
            {
                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest == null || __instance.completed.Value == __state)
                    return; // Do nothing if it's not managed quest or completion status wasn't changed

                if (managedQuest is IQuestObserver observer)
                {
                    observer.Completed(
                        new QuestInfo(__instance, Game1.player));
                }

                if (managedQuest is IStatefull statefullManagedQuest)
                {
                    // Reset to the inital state if the quest is completed
                    statefullManagedQuest.ResetState();
                }

                if (managedQuest.NextQuests?.Count > 0)
                {
                    foreach (var next in managedQuest.NextQuests)
                    {
                        Instance.QuestManager.AcceptQuest(managedQuest.NormalizeName(next));
                    }
                }

                Instance.Monitor.Log($"Quest `{managedQuest.GetFullName()}` #{__instance.id.Value} completed!"); 

            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(After_questComplete));
            }
        }

        private static void After_get_questDescription(Quest __instance, ref string __result)
        {
            try
            {
                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest is IQuestObserver observer)
                {
                    observer.UpdateDescription(
                        new QuestInfo(__instance, Game1.player),
                        ref __instance._questDescription);
                }

                if (__instance._questDescription == null)
                    __instance._questDescription = "";

                __result = __instance._questDescription;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(After_get_questDescription));
            }
        }

        private static bool Before_checkIfComplete(Quest __instance, ref bool __result, NPC n, int number1, int number2, Item item, string str)
        {
            try
            {
                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest == null || !(managedQuest is IQuestObserver))
                {
                    // It's vanilla quest, unmanaged quest or managed quest without observer. 
                    // We call the original method
                    return true;
                }

                var goingComplete = (managedQuest as IQuestObserver).CheckIfComplete(
                    new QuestInfo(__instance, Game1.player),
                    new CompletionArgs(n, number1, number2, item, str));

                if (goingComplete.HasValue && goingComplete.Value && !__instance.completed.Value)
                    __instance.questComplete();

                if (goingComplete.HasValue)
                    __result = goingComplete.Value;

                return !goingComplete.HasValue;

            } catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Before_checkIfComplete));
                return true;
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.getQuestFromId)),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_getQuestFromId))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                prefix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.Before_questComplete)),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_questComplete))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Quest), nameof(Quest.currentObjective)).GetGetMethod(),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_get_currentObjective))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Quest), nameof(Quest.questDescription)).GetGetMethod(),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_get_questDescription))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.checkIfComplete)),
                prefix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.Before_checkIfComplete))
            );
        }
    }
}
