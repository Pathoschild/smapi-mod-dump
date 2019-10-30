using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using static StardewValley.Game1;

namespace JoysOfEfficiency.Automation
{
    internal class MailAutomation
    {
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;
        private static IMonitor Monitor => InstanceHolder.Monitor;

        public static void CollectMailAttachmentsAndQuests(LetterViewerMenu menu)
        {
            IReflectedField<int> questIdField = Reflection.GetField<int>(menu, "questID");
            int questId = questIdField.GetValue();

            if (menu.itemsLeftToGrab())
            {
                foreach (ClickableComponent component in menu.itemsToGrab.ToArray())
                {
                    if (component.item == null || !CanPlayerAcceptsItemPartially(component.item))
                    {
                        continue;
                    }

                    int stack = component.item.Stack;
                    playSound("coin");
                    int remain = Util.AddItemIntoInventory(component.item);

                    Monitor.Log($"You collected {component.item.DisplayName}{(stack - remain > 1 ? " x" + (stack - remain) : "")}.");
                    if (remain == 0)
                    {
                        component.item = null;
                    }
                    else
                    {
                        component.item.Stack = remain;
                    }
                }
            }

            if (questId == -1)
            {
                return;
            }

            Monitor.Log($"You started Quest: {Quest.getQuestFromId(questId).questTitle}''.");
            player.addQuest(questId);
            playSound("newArtifact");
            questIdField.SetValue(-1);
        }

        private static bool CanPlayerAcceptsItemPartially(Item item)
        {
            if (player.Items.Contains(null) || player.Items.Count < player.MaxItems)
            {
                // Inventory includes at least one free space.
                return true;
            }

            return player.Items.Any(stack => stack.canStackWith(item) && stack.Stack < stack.maximumStackSize());
        }
    }
}
