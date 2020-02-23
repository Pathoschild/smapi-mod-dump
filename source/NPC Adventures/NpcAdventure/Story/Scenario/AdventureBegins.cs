using NpcAdventure.Events;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Linq;

namespace NpcAdventure.Story.Scenario
{
    internal class AdventureBegins : BaseScenario
    {
        const string LETTER_KEY = "npcAdventures.adventureBegins";
        const string DONE_LETTER_KEY = "npcAdventures.adventureContinue";

        private readonly ISpecialModEvents modEvents;
        private readonly IModEvents gameEvents;
        private readonly IContentLoader contentLoader;
        private readonly Config config;
        private readonly IMonitor monitor;

        public AdventureBegins(ISpecialModEvents modEvents, IModEvents gameEvents, IContentLoader contentLoader, Config config, IMonitor monitor) : base()
        {
            this.modEvents = modEvents;
            this.gameEvents = gameEvents;
            this.contentLoader = contentLoader;
            this.config = config;
            this.monitor = monitor;
        }

        public override void Dispose()
        {
            this.modEvents.MailboxOpen -= this.Events_MailboxOpen;
            this.modEvents.QuestCompleted -= this.ModEvents_QuestCompleted;
            this.gameEvents.Player.Warped -= this.Player_Warped;
            this.gameEvents.GameLoop.DayStarted -= this.GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (this.GameMaster.Mode != GameMasterMode.MASTER)
                return; // Master only logic. Master will process all known players.

            // We check all players if we can add Maron's invitation letter to their mailbox
            foreach (Farmer player in Game1.getAllFarmers())
            {
                if (player.mailReceived.Contains("guildMember") && player.deepestMineLevel >= 20 && !player.mailReceived.Contains(LETTER_KEY) && this.PlayerHasRequiredFriendship(player))
                {
                    if (player.mailForTomorrow.Contains(LETTER_KEY) || player.mailbox.Contains(LETTER_KEY))
                        return; // Don't send letter again when it's in mailbox or it's ready to be placed in tomorrow

                    // Marlon sends letter with invitation if player can't recruit and don't recieved Marlon's letter
                    player.mailbox.Add(LETTER_KEY);
                    this.monitor.Log($"Adventure Begins: Marlon's mail added immediately for {player.Name} ({player.UniqueMultiplayerID})");
                }
            }            
        }

        public override void Initialize()
        {
            this.modEvents.MailboxOpen += this.Events_MailboxOpen;
            this.modEvents.QuestCompleted += this.ModEvents_QuestCompleted;
            this.gameEvents.Player.Warped += this.Player_Warped;
            this.gameEvents.GameLoop.DayStarted += this.GameLoop_DayStarted;
        }

        private bool PlayerHasRequiredFriendship(Farmer farmer)
        {
            float askTwoThirds = this.config.HeartThreshold * .66f;
            float suggestTwoThirds = this.config.HeartSuggestThreshold * .66f;

            return farmer.friendshipData.Values.Any(
                fs => (fs.Points / 250) >= askTwoThirds || (fs.Points / 250) >= suggestTwoThirds
            );
        }

        private void ModEvents_QuestCompleted(object sender, IQuestCompletedArgs e)
        {
            int id = StoryHelper.ResolveId(e.Quest.id.Value);

            if (id == 4 && !Game1.player.hasOrWillReceiveMail(DONE_LETTER_KEY))
            {
                Game1.addMailForTomorrow(DONE_LETTER_KEY);
            }
        }

        /// <summary>
        /// Check for Marlon's mail read from mailbox
        /// Adds a introduction "Adventure begins" quest and allows to play Marlon's event in Adventurer's guild
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_MailboxOpen(object sender, IMailEventArgs e)
        {
            if (!e.LetterKey.Equals("adventureBegins"))
                return;

            Quest quest = this.StoryHelper.GetQuestById(1);
            quest.showNew.Value = true;
            quest.accept();

            Game1.player.questLog.Add(quest);
            Game1.addHUDMessage(new HUDMessage(this.contentLoader.LoadString("Strings/Strings:newObjective"), 2));
            Game1.playSound("questcomplete");
        }

        /// <summary>
        /// Check a parts of introduction event/quest when player warped.
        /// When player reach 10 floor of mines, then we got letter tomorrow
        /// When player got letter and go to Adventurer's guild, play Marlon event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name.Equals("AdventureGuild") && e.Player.mailReceived.Contains(LETTER_KEY) && !this.GameMaster.Data.GetPlayerState(e.Player).isEligible)
            {
                if (this.contentLoader.LoadStrings("Data/Events").TryGetValue("adventureBegins", out string eventData))
                {
                    e.NewLocation.startEvent(new Event(eventData));
                    this.GameMaster.Data.GetPlayerState().isEligible = true;
                    this.GameMaster.SyncData();
                    this.monitor.Log($"Player {e.Player.Name} is now eligible to recruit companions!", LogLevel.Info);
                }
            }
        }
    }
}
