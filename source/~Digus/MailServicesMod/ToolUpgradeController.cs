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
using StardewValley;
using StardewValley.Menus;

namespace MailServicesMod
{
    internal class ToolUpgradeController
    {
        internal const string UpgradeResponseKeyYes = "Yes";
        internal const string UpgradeResponseKeyNo = "No";
        internal const string UpgradeDialogKey = "MailServicesMod_UpgradeTool";

        internal static void OpenUpgradeDialog()
        {
            List<Response> options = new List<Response>
            {
                new Response(UpgradeResponseKeyYes, DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.Yes")),
                new Response(UpgradeResponseKeyNo, DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.No"))
            };
            Game1.player.currentLocation.createQuestionDialogue(DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.Question"),
                options.ToArray(), UpgradeDialogKey);
        }

        internal static bool TryToSendTool()
        {
            if (Game1.player.CurrentTool is Tool tool)
            {
                Dictionary<ISalable, int[]> blacksmithUpgradeStock = Utility.getBlacksmithUpgradeStock(Game1.player);
                int[] cost = blacksmithUpgradeStock
                    .Where(s => s.Key.GetType() == tool.GetType())
                    .Select(s => s.Value)
                    .FirstOrDefault();
                if (cost != null)
                {
                    int price = (int)Math.Round(cost[0] * (1 + DataLoader.ModConfig.ToolShipmentServicePercentFee / 100d), MidpointRounding.AwayFromZero) + DataLoader.ModConfig.ToolShipmentServiceFee;
                    int barIndex = cost[2];
                    int barCount = cost.Length >= 4 ? cost[3] : 5;
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
                    return true;
                }
            }
            return false;
        }
    }
}
