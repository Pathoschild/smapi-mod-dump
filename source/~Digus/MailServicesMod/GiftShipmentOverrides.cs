/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MailServicesMod.GiftShipmentController;

namespace MailServicesMod
{
    internal class GiftShipmentOverrides
    {
        public static bool mailbox()
        {
            try
            {
                if (!DataLoader.ModConfig.DisableGiftService && Game1.player.mailbox.Count == 0 && Game1.player.ActiveObject != null && Game1.player.ActiveObject.canBeGivenAsGift() && !Game1.player.ActiveObject.questItem.Value)
                {
                    if (Game1.player.Money >= DataLoader.ModConfig.GiftServiceFee)
                    {
                        return GiftShipmentController.CreateResponsePage(0);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Gift.NoMoney"));
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to send gift to a villager.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
            return true;
        }

        public static void answerDialogueAction(string questionAndAnswer)
        {
            try
            {
                if (questionAndAnswer.StartsWith(GiftShipmentController.GiftDialogKey))
                {
                    string npcName = questionAndAnswer.Remove(0, GiftShipmentController.GiftDialogKey.Length + 1);
                    if (npcName == GiftResponseKeyNone) return;
                    if (npcName.StartsWith(GiftResponseKeyPrevious))
                    {
                        GiftShipmentController.CreateResponsePage(int.Parse(npcName.Remove(0, GiftResponseKeyPrevious.Length)));
                    }
                    else if (npcName.StartsWith(GiftResponseKeyNext))
                    {
                        GiftShipmentController.CreateResponsePage(int.Parse(npcName.Remove(0, GiftResponseKeyNext.Length)));
                    }
                    else
                    {
                        GiftShipmentController.GiftToNpc(npcName);
                    }
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to answer your gift choice.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }
    }
}
