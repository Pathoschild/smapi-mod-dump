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
using static MailServicesMod.GuildRecoveryController;

namespace MailServicesMod
{
    internal class GuildRecoveryOverrides
    {
        public static void answerDialogueAction(string questionAndAnswer)
        {
            try
            {
                switch (questionAndAnswer)
                {
                    case null:
                        break;
                    case RecoveryDialogKey + "_" + RecoveryResponseKeyOne:
                        DataLoader.SaveRecoveryConfig(Game1.player, true, false);
                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.AnswerSent"));
                        break;
                    case RecoveryDialogKey + "_" + RecoveryResponseKeyAll:
                        DataLoader.SaveRecoveryConfig(Game1.player, true, true);
                        Game1.drawObjectDialogue(DataLoader.I18N.Get("Delivery.Marlon.RecoveryOffer.AnswerSent"));
                        break;
                    case RecoveryDialogKey + "_" + RecoveryResponseKeyNone:
                        DataLoader.SaveRecoveryConfig(Game1.player, false);
                        break;
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to answer you recovery service letter.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }

        public static bool actionWhenPurchased(Item __instance)
        {
            try
            {
                if (__instance.isLostItem)
                {
                    MailController.UnloadLetterMailbox(DataLoader.ItemRecoveryMailId);
                }
            }
            catch (Exception e)
            {
                MailServicesModEntry.ModMonitor.Log("Error trying to remove you recovery letter from the mailbox.", LogLevel.Error);
                MailServicesModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailServicesModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
            return true;
        }
    }
}
