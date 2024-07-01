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
using StardewValley.Internal;
using StardewValley.Menus;
using StardewValley.Tools;
using static MailServicesMod.ToolUpgradeController;

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
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }

        public static bool addItemsByMenuIfNecessary(ref List<Item> itemsToAdd)
        {
            try
            {
                itemsToAdd.RemoveAll(i => i is GenericTool g && g.ItemId.EndsWith("TrashCan"));
                if (itemsToAdd.Count == 0)
                {
                    itemsToAdd = null;
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to avoid the trash can to be added to the inventory.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
            return true;
        }

        public static bool mailbox()
        {
            try
            {
                if (!DataLoader.ModConfig.DisableToolShipmentService && Game1.player.mailbox.Count == 0 && Game1.player.CurrentTool is { } tool)
                {
                    if (Game1.player.toolBeingUpgraded.Value == null)
                    {
                        if (!ShopBuilder.GetShopStock("ClintUpgrade").FirstOrDefault(s => ((Tool)s.Key).GetToolData().ConventionalUpgradeFrom == tool.QualifiedItemId).Equals(default(KeyValuePair<ISalable, ItemStockInformation>)))
                        {
                            if (DataLoader.ModConfig.EnableAskToUpgradeTool)
                            {
                                ToolUpgradeController.OpenUpgradeDialog();
                                return false;
                            }
                            else
                            {
                                if (ToolUpgradeController.TryToSendTool()) return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to send the tool to upgrade.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above:{e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
            return true;
        }

        public static void answerDialogueAction(string questionAndAnswer)
        {
            try
            {
                switch (questionAndAnswer)
                {
                    case null:
                        break;
                    case UpgradeDialogKey + "_" + UpgradeResponseKeyYes:
                        ToolUpgradeController.TryToSendTool();
                        break;
                    case UpgradeDialogKey + "_" + UpgradeResponseKeyNo:
                        break;
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to answer your tool upgrade service shipping confirmation.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }
    }
}
