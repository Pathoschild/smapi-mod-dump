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
using PurrplingCore;
using PurrplingCore.Patching;
using QuestFramework.Framework;
using QuestFramework.Framework.Events;
using QuestFramework.Framework.Quests;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;

namespace QuestFramework.Patches
{
    internal class QuestPatch : Patch<QuestPatch>
    {
        public override string Name => nameof(QuestPatch);

        QuestManager QuestManager { get; }
        EventManager EventManager { get; }
        HashSet<Quest> QuestCheckerLock { get; }

        public QuestPatch(QuestManager questManager, EventManager eventManager)
        {
            this.QuestManager = questManager;
            this.EventManager = eventManager;
            this.QuestCheckerLock = new HashSet<Quest>();
            Instance = this;
        }

        private static void After_getQuestFromId(int id, ref Quest __result)
        {
            try
            {
                var customQuest = Instance.QuestManager.GetById(id);

                if (__result != null && customQuest != null)
                {
                    if (customQuest.BaseType == QuestType.Monster && __result is SlayMonsterQuest)
                    {
                        // This is fix for use custom dialogue text for slay monster quest type
                        (__result as SlayMonsterQuest).dialogueparts.Clear();
                    }

                    if (!Game1.player.hasQuest(id))
                        customQuest.Reset();

                    __result.dailyQuest.Value = customQuest.IsDailyQuest();
                    __result.daysLeft.Value = customQuest.DaysLeft;
                    __result.canBeCancelled.Value = customQuest.Cancelable;
                    __result.questType.Value = customQuest.CustomTypeId != -1
                            ? customQuest.CustomTypeId
                            : customQuest.BaseType.ToVanillaTypeId();
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

                if (managedQuest?.Objective != null)
                {
                    __instance._currentObjective = managedQuest.Objective;
                    __instance.reloadObjective();
                }

                // Fix item harvest quest type objective (like 'x/y item harvested', affects only managed harvest quests)
                if (managedQuest != null && __instance is ItemHarvestQuest harvestQuest)
                {
                    string[] triggerTokens = managedQuest.Trigger.Split(' ');
                    int startNumber = triggerTokens.Length > 1 ? int.Parse(triggerTokens[1]) : 1;

                    if (harvestQuest.number.Value < startNumber)
                    {
                        harvestQuest._currentObjective = QuestFrameworkMod.Instance.Helper.Translation.Get("quest.itemHarvest.objective", new {
                            harvested = startNumber - harvestQuest.number.Value,
                            total = startNumber,
                            itemName = Game1.objectInformation[harvestQuest.itemIndex.Value].Split('/')[4]
                        });
                    }
                }

                // Call observer objective updater (if this managed quest is observed)
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
                if (!__instance.completed.Value || __instance.completed.Value == __state)
                    return; // Do nothing if completion status wasn't changed or completion status is false

                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest == null)
                {
                    Instance.EventManager.QuestCompleted.Fire(new Events.QuestEventArgs(__instance), Instance);
                    return; // Only fire global uest completed event if quest is unmanaged
                }

                managedQuest.ConfirmComplete(new QuestInfo(__instance, Game1.player));

                if (managedQuest.NextQuests?.Count > 0)
                {
                    foreach (var next in managedQuest.NextQuests)
                    {
                        Instance.QuestManager.AcceptQuest(managedQuest.NormalizeName(next));
                    }
                }

                Instance.EventManager.QuestCompleted.Fire(new Events.QuestEventArgs(__instance), Instance);
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

                if (managedQuest?.Description != null) 
                {
                    __instance._questDescription = managedQuest.Description;
                }

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

        private static void After_get_questTitle(Quest __instance, ref string __result)
        {
            try
            {
                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest?.Title != null)
                {
                    __instance._questTitle = managedQuest.Title;
                }

                if (managedQuest is IQuestObserver observer)
                {
                    observer.UpdateTitle(
                        new QuestInfo(__instance, Game1.player),
                        ref __instance._questTitle);
                }

                if (__instance._questTitle == null)
                    __instance._questTitle = "";

                __result = __instance._questTitle;
            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(After_get_questTitle));
            }
        }

        private static bool Before_checkIfComplete(Quest __instance, ref bool __result, NPC n, int number1, int number2, Item item, string str)
        {
            try
            {
                if (Instance.QuestCheckerLock.Contains(__instance))
                    return true; // Always call only the original method for locked quests

                var managedQuest = Instance.QuestManager.GetById(__instance.id.Value);

                if (managedQuest is IQuestObserver observer)
                {
                    Instance.QuestCheckerLock.Add(__instance);
                    var goingComplete = observer.CheckIfComplete(
                        new QuestInfo(__instance, Game1.player),
                        new CompletionArgs(n, number1, number2, item, str));

                    if (goingComplete && !__instance.completed.Value)
                        __instance.questComplete();

                    __result = goingComplete;
                    Instance.QuestCheckerLock.Remove(__instance);

                    // For observed managed quest we don't call the original method Quest.checkIfComplete
                    return false;
                }

                return true;

            } catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Before_checkIfComplete));
                Instance.QuestCheckerLock.Remove(__instance);

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
                original: AccessTools.Property(typeof(Quest), nameof(Quest.questTitle)).GetGetMethod(),
                postfix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.After_get_questTitle))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.checkIfComplete)),
                prefix: new HarmonyMethod(typeof(QuestPatch), nameof(QuestPatch.Before_checkIfComplete))
            );
        }
    }
}
