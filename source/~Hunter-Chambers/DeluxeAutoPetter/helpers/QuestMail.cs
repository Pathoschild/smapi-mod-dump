/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace DeluxeAutoPetter.helpers
{
    internal static class QuestMail
    {
        /** **********************
         * Class Variables
         ********************** **/
        private static string? QUEST_MAIL_ID;
        private static string? QUEST_REWARD_MAIL_ID;
        private static string? QUEST_MAIL_DETAILS;
        private static string? QUEST_REWARD_MAIL_DETAILS;

        /** **********************
         * Variable Getters
         ********************** **/
        public static string GetQuestMailID()
        {
            if (QUEST_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_MAIL_ID;
        }

        public static string GetQuestRewardMailID()
        {
            if (QUEST_REWARD_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_REWARD_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_REWARD_MAIL_ID;
        }

        public static string GetQuestMailDetails()
        {
            if (QUEST_MAIL_DETAILS is null) throw new ArgumentNullException($"{nameof(QUEST_MAIL_DETAILS)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_MAIL_DETAILS;
        }

        public static string GetQuestRewardMailDetails()
        {
            if (QUEST_REWARD_MAIL_DETAILS is null) throw new ArgumentNullException($"{nameof(QUEST_REWARD_MAIL_DETAILS)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_REWARD_MAIL_DETAILS;
        }

        /** **********************
         * Public Methods
         ********************** **/
        public static void Initialize(string modID)
        {
            QUEST_MAIL_ID = $"{modID}.Mail0";
            QUEST_REWARD_MAIL_ID = $"{modID}.Mail1";
            QUEST_MAIL_DETAILS = $"{I18n.QuestMailStart()}^{I18n.QuestMailDetails()}^   - {I18n.RobinName()} %item quest {QuestDetails.GetQuestID()} %%[#]{I18n.QuestMailTitle()}";
            QUEST_REWARD_MAIL_DETAILS = $"@,^{I18n.QuestRewardMailDetails()}^   - {I18n.MarnieName()} [#]{I18n.QuestRewardMailTitle()}";
        }

        public static void LoadMail(string mailID, string mailDetails)
        {
            DataLoader.Mail(Game1.content)[mailID] = mailDetails;
        }
    }
}
