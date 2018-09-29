using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using Object = System.Object;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class Commands
    {
        public static IMonitor ModMonitor = EverlastingBaitsAndUnbreakableTacklesModEntery.ModMonitor;

        public static void AddAllBaitTackleRecipes(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                foreach (BaitTackle baitTackle in Enum.GetValues(typeof(BaitTackle)))
                {
                    if (!Game1.player.craftingRecipes.ContainsKey(baitTackle.GetDescription()))
                    {
                        Game1.player.craftingRecipes.Add(baitTackle.GetDescription(), 0);
                        ModMonitor.Log($"Added {baitTackle.GetDescription()} recipe to the player.", LogLevel.Info);
                    }
                }
            }
            else
            {
                ModMonitor.Log("No player loaded to add the recipes.", LogLevel.Info);
            }
        }

        public static void GetAllBaitTackle(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu == null)
                {
                    IList<Item> baitsTackles = new List<Item>();
                    foreach (BaitTackle baitTackle in Enum.GetValues(typeof(BaitTackle)))
                    {
                        baitsTackles.Add(new StardewValley.Object((int) baitTackle, 1, false, -1, 4));
                    }

                    Game1.activeClickableMenu = new ItemGrabMenu(baitsTackles);
                }
                else
                {
                    ModMonitor.Log("Close all menus to use this command.", LogLevel.Info);
                }
            }
            else
            {
                ModMonitor.Log("No player loaded to get the baits and tackles.", LogLevel.Info);
            }
        }

        public static void AddsQuestForTackle(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                if (args.Length > 0)
                {
                    string tackleName = String.Join(" ", args);
                    BaitTackle? baitTackle = BaitTackleExtension.GetFromDescription(tackleName);
                    if (baitTackle.HasValue)
                    {
                        DataLoader.LoadTackleQuest(baitTackle.Value);
                        ModMonitor.Log($"Quest for {baitTackle.Value.GetDescription()} added to the player.",
                            LogLevel.Info);
                    }
                    else
                    {
                        ModMonitor.Log($"Invalid tackle name: {tackleName}", LogLevel.Info);
                    }
                }
                else
                {
                    ModMonitor.Log($"No tackle name was given to the command.", LogLevel.Info);
                }
            }
            else
            {
                ModMonitor.Log("No player add quest for.", LogLevel.Info);
            }
        }

        public static void RemoveBlankQuests(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                int quantity = 0;
                List<Quest> questsToRemove = new List<Quest>();
                foreach (Quest quest in Game1.player.questLog)
                {
                    if (quest.id.Value == 0
                        && quest._currentObjective == ""
                        && quest._questDescription == ""
                        && quest._questTitle == "")
                    {
                        questsToRemove.Add(quest);
                        quantity++;
                    }
                }

                questsToRemove.ForEach(q => Game1.player.questLog.Remove(q));
                ModMonitor.Log($"{quantity} quests removed.", LogLevel.Info);

            }
            else
            {
                ModMonitor.Log("No player remove quest from.", LogLevel.Info);
            }
        }
    }
}
