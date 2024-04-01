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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Tools;
using StardewValley.Internal;
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
            List<Response> options = new()
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
                var stock = ShopBuilder.GetShopStock("ClintUpgrade");
                var toolStockData = stock
                    .Where(s => ((Tool)s.Key).GetToolData().ConventionalUpgradeFrom == tool.QualifiedItemId)
                    .FirstOrDefault();

                if (!default(KeyValuePair<ISalable, ItemStockInformation>).Equals(toolStockData))
                {
                    int price = (int)Math.Round(toolStockData.Value.Price * (1 + DataLoader.ModConfig.ToolShipmentServicePercentFee / 100d), MidpointRounding.AwayFromZero) + DataLoader.ModConfig.ToolShipmentServiceFee;
                    string barIndex = toolStockData.Value.TradeItem;
                    int barCount = toolStockData.Value.TradeItemCount??1;
                    if (Game1.player.Money >= price)
                    {
                        if (Game1.player.Items.ContainsId(barIndex, barCount))
                        {
                            ShopMenu.chargePlayer(Game1.player, 0, price);
                            Game1.player.Items.ReduceId(barIndex, barCount);

                            Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Clint.UpgradeLetter.ToolSent", new { Tool = tool.DisplayName }));

                            Tool newTool = ItemRegistry.Create<Tool>(toolStockData.Key.QualifiedItemId);
                            newTool.UpgradeFrom(tool);
                            Game1.player.removeItemFromInventory(tool);
                            Game1.player.toolBeingUpgraded.Value = newTool;
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
