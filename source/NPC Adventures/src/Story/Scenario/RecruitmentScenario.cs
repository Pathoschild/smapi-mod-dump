using NpcAdventure.Story.Messaging;
using NpcAdventure.Story.Quests;
using StardewValley;

namespace NpcAdventure.Story.Scenario
{
    internal class RecruitmentScenario : BaseScenario
    {
        public override void Dispose()
        {
            this.GameMaster.MessageReceived -= this.GameMaster_MessageReceived;
        }

        public override void Initialize()
        {
            this.GameMaster.MessageReceived += this.GameMaster_MessageReceived;
        }
        
        /// <summary>
        /// Check if any quest is completed from recieved GameMaster event massage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameMaster_MessageReceived(object sender, IGameMasterEventArgs e)
        {
            if (e.Player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                return; // Only client must to complete quest and inform a server about it (via NetFields in player's questlog internally in vanilla game)

            var ps = this.GameMaster.Data.GetPlayerState();

            if (e.Message is RecruitMessage recruitMessage && !ps.recruited.Contains(recruitMessage.CompanionName))
            {
                ps.recruited.Add(recruitMessage.CompanionName);
                this.GameMaster.SyncData();

                Game1.player.checkForQuestComplete(null, -1, -1, null, null, RecruitmentQuest.TYPE_ID);
                this.StoryHelper.CompleteQuest(2); // Immediatelly complete the first companion recruited quest
            }
        }
    }
}
