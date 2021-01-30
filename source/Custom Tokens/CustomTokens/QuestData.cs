/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System.Collections;
using System.Collections.Generic;
using StardewValley.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class QuestData
    {
        public ArrayList QuestlogidsOld = new ArrayList();

        public ArrayList QuestlogidsNew = new ArrayList();

        internal void AddCompletedQuests(PerScreen<PlayerData> data, PlayerDataToWrite datatowrite)
        {            
            var questlog = Game1.player.questLog;

            var questdata = data.Value.QuestsCompleted;

            // Get quests in questlog and add to an array temporarily for determining previously completed quests
            foreach (Quest quest in questlog)
            {
                QuestlogidsNew.Add(quest.id.Value);
            }

            // Get additional quests completed, manually added quests
            foreach(long questid in datatowrite.AdditionalQuestsCompleted)
            {
                if (questdata.Contains((int)questid) == false)
                {
                    questdata.Add((int)questid);
                }
            }

            // Method for determining whether a quest received in the mail is completed
            void MailQuests(int questid, string mailname)
            {
                if (true
                    // Player has or will receive the mail to begin the quest
                    && Game1.player.hasOrWillReceiveMail(mailname) == true
                    // Quest is not present in the questlog
                    && QuestlogidsNew.Contains(questid) == false
                    // Player's mailbox does not contain the mail, they have actually begun and finished the quest
                    && Game1.player.mailbox.Contains(mailname) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Method for determining whether a quest that involves an event is completed
            void EventQuest(int questid, int eventid)
            {
                if (true 
                    // Player has seen the event that finishes or begins the quest
                    && Game1.player.eventsSeen.Contains(eventid) == true
                    // Quest is not present in the questlog
                    && QuestlogidsNew.Contains(questid) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Method for determining whether a quest that requires another quest to have been completed is completed
            void QuestPreReq(int questid, int questprereq)
            {
                if (true
                    // questdata contains the id of the quest that must be completed before this quest can be done
                    && questdata.Contains(questprereq) == true
                    // Quest is not present in the questlog
                    && QuestlogidsNew.Contains(questid) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Method for determining whether a quest that requires a secret note to be seen is complete
            void NoteQuest(int questid, int noteseen)
            {
                if (Game1.player.secretNotesSeen.Contains(noteseen) == true && QuestlogidsNew.Contains(questid) == false && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Introductions quest, if it's not in the log, it would have been completed
            if (QuestlogidsNew.Contains(9) == false && questdata.Contains(9) == false)
            {
                questdata.Add(9);
            }

            // How to win friends quest
            QuestPreReq(25, 9);

            // Raising animals quest, 6 must be added manually for old saves
            QuestPreReq(7, 6);

            // To the beach quest
            EventQuest(13, 739330);

            // Advancement quest, 6 must be added manually for old saves
            QuestPreReq(8, 6);

            // Explore the mines quest
            EventQuest(14, 100162);

            // Deeper in the mine quest
            QuestPreReq(17, 14);

            // To the bottom quest
            QuestPreReq(18, 17);

            // The skull key quest
            if (Game1.player.hasUnlockedSkullDoor == true && questdata.Contains(19) == false)
            {
                questdata.Add(19);
            }

            // Archaeology quest pt 1
            EventQuest(23, 0);

            // Archaeology quest pt 2
            QuestPreReq(24, 23);

            // Rat problem quest
            EventQuest(26, 611439);

            // Forging ahead quest
            EventQuest(11, 992553);

            // Smelting quest
            QuestPreReq(12, 11);

            // Initiation quest
            QuestPreReq(15, 14);

            // Errand for your wife quest
            EventQuest(126, 3917601);

            // Haley's cake-walk quest
            EventQuest(127, 6184644);
            
            if (true
                /* 
                Any quest from The mysterious Mr. Qi questline present in the questlog or if the player has the clubcard
                indicates that quest with id 2 has been completed 
                */
                && questdata.Contains(2) == false 
                && (false 
                || QuestlogidsNew.Contains(3) == true 
                || QuestlogidsNew.Contains(4) == true 
                || QuestlogidsNew.Contains(5) == true 
                || Game1.player.hasClubCard == true))
            {
                questdata.Add(2);
            }

            // The mysterious Mr. Qi quests
            QuestPreReq(3, 2);
            QuestPreReq(4, 3);
            QuestPreReq(5, 4);

            // Cryptic note quest
            NoteQuest(30, 10);

            // Strange note quest
            EventQuest(29, 2120303);

            // A winter mystery quest (Is the figure Krobus? You decide.)
            EventQuest(31, 520702);

            // Marnie's request quest
            EventQuest(21, 92);

            // Fish casserole quest, both possible events
            EventQuest(22, 94);
            EventQuest(22, 95);

            // Dark talisman quest
            if (Game1.player.hasDarkTalisman == true && questdata.Contains(28) == false)
            {
                questdata.Add(28);
            }

            // Goblin problem quest
            if (Game1.player.hasMagicInk == true && questdata.Contains(27) == false)
            {
                questdata.Add(27);
            }

            // Meet the wizard quest
            EventQuest(1, 112);

            // Mail quests
            MailQuests(100, "spring_11_1");
            MailQuests(101, "spring_19_1");
            MailQuests(102, "summer_3_1");
            MailQuests(103, "summer_14_1");
            MailQuests(104, "summer_20_1");
            MailQuests(105, "summer_25_1");
            MailQuests(106, "fall_3_1");
            MailQuests(107, "fall_8_1");
            MailQuests(108, "fall_19_1");
            MailQuests(109, "winter_2_1");
            MailQuests(110, "winter_6_1");
            MailQuests(111, "winter_12_1");
            MailQuests(112, "winter_17_1");
            MailQuests(113, "winter_21_1");
            MailQuests(114, "winter_26_1");
            MailQuests(115, "spring_6_2");
            MailQuests(116, "spring_15_2");
            MailQuests(117, "spring_21_2");
            MailQuests(118, "summer_6_2");
            MailQuests(119, "summer_15_2");
            MailQuests(120, "summer_21_2");
            MailQuests(121, "fall_6_2");
            MailQuests(122, "fall_19_2");
            MailQuests(123, "winter_5_2");
            MailQuests(124, "winter_13_2");
            MailQuests(125, "winter_19_2");

            // Qi's challenge quest
            if (Game1.player.hasOrWillReceiveMail("QiChallengeComplete") == true && questdata.Contains(20) == false)
            {
                questdata.Add(20);
            }

            // Quests 130, 129, 128 and 16 must also be added manually for old saves            

            // Sort array and clear temporary data
            questdata.Sort();
            QuestlogidsNew.Clear();

        }

        /// <summary>Updates quest log to add new quests, without removing previously held quests. Use to check completed quests with no reward.</summary>
        public void UpdateQuestLog()
        {
            foreach (Quest quest in Game1.player.questLog)
            {
                if (QuestlogidsOld.Contains(quest.id.Value) == false)
                {
                    QuestlogidsOld.Add(quest.id.Value);
                }
            }
        }

        /// <summary>Determines whether a quest is complete.</summary>
        /// <param name="data">Where to save data</param>
        /// <param name="datatowrite">Where to write data that will be written</param>
        /// <param name="monitor">Provides access to the SMAPI monitor</param>
        internal void CheckForCompletedQuests(PerScreen<PlayerData> data, PlayerDataToWrite datatowrite, IMonitor monitor)
        {
            // Clear QuestlogidsNew array
            QuestlogidsNew.Clear();

            var questlog = Game1.player.questLog;

            var questdata = data.Value.QuestsCompleted;

            // Iterate through each active quest
            foreach (Quest quest in questlog)
            {              
                // Is the quest complete?
                if (true
                    // Quest has an id
                    && quest.id != null
                    // Quest has been completed
                    && quest.completed == true
                    // Quest has not already been added to array list
                    && questdata.Contains(quest.id.Value) == false)
                {
                    // Yes, add it to quest array if it hasn't been added already
                    questdata.Add(quest.id.Value);
                    monitor.Log($"Quest with id {quest.id.Value} has been completed");

                    // If quest with id 6 is completed, add it to PlayerDataToWrite if it isn't already an element
                    if (quest.id.Value == 6 && datatowrite.AdditionalQuestsCompleted.Contains(quest.id.Value) == false)
                    {
                        datatowrite.AdditionalQuestsCompleted.Add(quest.id.Value);
                    }
                }

                // Add current quests to QuestlogidsNew
                else if (QuestlogidsNew.Contains(quest.id.Value) == false && quest.completed == false)
                {
                    QuestlogidsNew.Add(quest.id.Value);
                }
            }

            // Check for completed quests with no rewards by comparing two arrays

            // Iterate through each quest id recorded in QuestlogidsOld
            foreach (int questid in QuestlogidsOld)
            {
                // If QuestlogidsOld contains an id that QuestlogidsNew doesn't, the quest with that id is completed
                if (QuestlogidsNew.Contains(questid) == false && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                    monitor.Log($"Quest with id {questid} has been completed");                                       
                   
                    if (true
                       && (false
                       // If these quests are completed, add it to PlayerDataToWrite if it isn't already an element
                       || questid == 16
                       || questid == 128
                       || questid == 129
                       || questid == 130)
                       && datatowrite.AdditionalQuestsCompleted.Contains(questid) == false)
                    {
                        datatowrite.AdditionalQuestsCompleted.Add(questid);
                    }
                }               
            }

            // Necklace given to Abigail, remove alternate quest so it won't be marked as completed
            if (questdata.Contains(128) == true)
            {
                QuestlogidsOld.Remove(129);
            }

            // Necklace given to Caroline, remove alternate quest so it won't be marked as completed
            else if (questdata.Contains(129) == true)
            {
                QuestlogidsOld.Remove(128);
            }
        }

        internal void CheckForCompletedSpecialOrders(PerScreen<PlayerData> data, IMonitor monitor)
        {
            var order = Game1.player.team.completedSpecialOrders;

            // Check for completed special orders
            if (data.Value.SpecialOrdersCompleted.Count < order.Count())
            {
                foreach (string questkey in new List<string>(order.Keys))
                {
                    if (data.Value.SpecialOrdersCompleted.Contains(questkey) == false)
                    {
                        data.Value.SpecialOrdersCompleted.Add(questkey);
                        monitor.Log($"Special Order with key {questkey} has been completed");
                    }
                }
            }
        }
    }

    
}