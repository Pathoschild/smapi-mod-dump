/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace JoysOfEfficiency.Automation
{
    internal class MailAutomation
    {
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;

        private static readonly Logger Logger = new Logger("MailAutomation");

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
                    Game1.playSound("coin");
                    int remain = Util.AddItemIntoInventory(component.item);

                    Logger.Log($"You collected {component.item.DisplayName}{(stack - remain > 1 ? " x" + (stack - remain) : "")}.");
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

            Logger.Log($"You started Quest: '{Quest.getQuestFromId(questId).questTitle}'.");
            Game1.player.addQuest(questId);
            Game1.playSound("newArtifact");
            questIdField.SetValue(-1);
        }

        private static bool CanPlayerAcceptsItemPartially(Item item)
        {
            if (Game1.player.Items.Contains(null) || Game1.player.Items.Count < Game1.player.MaxItems)
            {
                // Inventory includes at least one free space.
                return true;
            }

            return Game1.player.Items.Any(stack => stack.canStackWith(item) && stack.Stack < stack.maximumStackSize());
        }
    }
}
