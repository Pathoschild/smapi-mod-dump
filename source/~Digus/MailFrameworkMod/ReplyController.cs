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
using System.Linq;
using MailFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;

namespace MailFrameworkMod
{
    public class ReplyController
    {
        private const string MfmDialogKeyPrefix = "MFM_Dialog::";

        public static void OpenReplyDialog(ReplyConfig replyConfig, ITranslationHelper i18n = null)
        {
            var replyConfigQuestionKey = MfmDialogKeyPrefix + replyConfig.QuestionKey;
            var options = replyConfig.Replies
                .Select(i => CreateResponse(replyConfigQuestionKey, i, i18n))
                .ToArray();
            Game1.player.currentLocation.createQuestionDialogue(
                TranslateOrDefault(i18n, replyConfig.QuestionDialog)
                , options
                , replyConfigQuestionKey);
        }

        public static Response CreateResponse(string questionKey, ReplyOption replyOption, ITranslationHelper i18n = null)
        {
            var answerConfigQuestionKey = questionKey + "_" + replyOption.ReplyKey;
            Action<string> replyOptionAction = (k) =>
            {
                Game1.player.mailReceived.AddRange(replyOption.MailReceivedToAdd);
                Game1.drawObjectDialogue((string)TranslateOrDefault(i18n, replyOption.ReplyResponseDialog));
            };
            ReplyRepository.AddQuestionAndAnswerAction(answerConfigQuestionKey, replyOptionAction);
            return new Response(
                replyOption.ReplyKey
                , TranslateOrDefault(i18n, replyOption.ReplyOptionDialog));
        }

        public static void answerDialogueAction(string questionAndAnswer)
        {
            try
            {
                Action<string> action = ReplyRepository.GetQuestionAndAnswerAction(questionAndAnswer);
                action?.Invoke(questionAndAnswer);
            }
            catch (Exception e)
            {
                MailFrameworkModEntry.ModMonitor.Log("Error trying to answer an MFM letter.", LogLevel.Error);
                MailFrameworkModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailFrameworkModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }

        private static string TranslateOrDefault(ITranslationHelper i18n, string key)
        {
            return i18n?.Get(key).Default(key) ?? key;
        }
    }
}