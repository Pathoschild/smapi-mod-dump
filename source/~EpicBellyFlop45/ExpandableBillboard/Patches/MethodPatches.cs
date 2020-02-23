using Harmony;
using StardewValley;
using StardewValley.Quests;
using System.Linq;
using static ExpandableBillboard.Enums;

namespace ExpandableBillboard.Patches
{
    public static class MethodPatches
    {
        public static void ApplyHarmonyPatches(string uniqueId)
        {
            HarmonyInstance harmony = HarmonyInstance.Create(uniqueId);

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemDeliveryQuest), "loadQuestInfo"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MethodPatches), nameof(ItemDeliveryQuestLoadPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FishingQuest), "loadQuestInfo"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MethodPatches), nameof(FishingQuestLoadPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SlayMonsterQuest), "loadQuestInfo"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MethodPatches), nameof(SlayMonsterQuestLoadPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceCollectionQuest), "loadQuestInfo"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MethodPatches), nameof(ResourceCollectionQuestLoadPrefix)))
            );
        }

        private static bool ItemDeliveryQuestLoadPrefix(ref ItemDeliveryQuest __instance)
        {
            // pick random quest from loaded quests
            var quests = ModEntry.AddedQuests
                .Where(q => q.Type == QuestType.ItemDelivery)
                .ToList();

            var quest = quests[Game1.random.Next(quests.Count())];
            if (quest == null)
            {
                return false;
            }

            quest = ModEntry.ResolveQuestTextTags(quest);

            // don't set the quest title here otherwise it will be used in the questlog. we want to default title, 'Delivery', not the user submitted one in the quest log
            __instance.questDescription = ModEntry.ConstructDescriptionString(quest);
            __instance.currentObjective = quest.Objective;
            __instance.dailyQuest.Value = true;
            __instance.daysLeft.Value = quest.DaysToComplete;
            __instance.moneyReward.Value = quest.MoneyReward;
            __instance.deliveryItem.Value = new Object(quest.DeliveryItem, 1);
            __instance.target.Value = quest.Requester;

            return false;
        }

        private static bool FishingQuestLoadPrefix()
        {
            return true;
        }

        private static bool SlayMonsterQuestLoadPrefix()
        {
            return true;
        }

        private static bool ResourceCollectionQuestLoadPrefix()
        {
            return true;
        }
    }
}
