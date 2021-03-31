/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace MailServicesMod
{
    internal class ToolUpgradeOverrides
    {

        public static void blacksmith()
        {
            try
            {
                if (Game1.player.toolBeingUpgraded.Value == null)
                {
                    MailController.UnloadLetterMailbox(DataLoader.ToolUpgradeMailId);
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to remove the tool delivery letter from the mailbox.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
            }
        }

        public static bool addItemsByMenuIfNecessary(ref List<Item> itemsToAdd)
        {
            try
            {
                itemsToAdd.RemoveAll(i => i is GenericTool);
                if (itemsToAdd.Count == 0)
                {
                    itemsToAdd = null;
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to avoid the trash can to be added to the inventory.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
            }
            return true;
        }

        public static bool mailbox()
        {
            try
            {
                if (!DataLoader.ModConfig.DisableToolShipmentService && Game1.player.mailbox.Count == 0 && Game1.player.CurrentTool is Tool tool)
                {
                    if (Game1.player.toolBeingUpgraded.Value == null)
                    {
                        if (tool is Axe || tool is Pickaxe || tool is Hoe || tool is WateringCan)
                        {
                            Dictionary<ISalable, int[]> blacksmithUpgradeStock = Utility.getBlacksmithUpgradeStock(Game1.player);
                            int[] cost = blacksmithUpgradeStock
                                .Where(s => s.Key.GetType() == tool.GetType())
                                .Select(s => s.Value)
                                .FirstOrDefault();
                            if (cost != null)
                            {
                                int price = cost[0] + DataLoader.ModConfig.ToolShipmentServiceFee;
                                int barIndex = cost[2];
                                const int barCount = 5;
                                if (Game1.player.Money >= price)
                                {
                                    if (Game1.player.hasItemInInventory(barIndex, barCount))
                                    {
                                        ShopMenu.chargePlayer(Game1.player, 0, price);
                                        Game1.player.removeItemsFromInventory(barIndex, barCount);

                                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.ToolSent", new { Tool = tool.DisplayName }));

                                        tool.UpgradeLevel++;
                                        Game1.player.removeItemFromInventory(tool);
                                        Game1.player.toolBeingUpgraded.Value = tool;
                                        Game1.player.daysLeftForToolUpgrade.Value = 2;
                                        Game1.playSound("parry");
                                    }
                                    else
                                    {
                                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.NoBars", new { Tool = tool.DisplayName }));
                                    }
                                }
                                else
                                {
                                    Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.NoMoney", new { Tool = tool.DisplayName }));
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to send the tool to upgrade.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above:{e.Message}", LogLevel.Trace);
            }
            return true;
        }
    }
}
