/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System.Linq;
using StardewValley;
using StardewValley.GameData.GarbageCans;
using StardewValley.TerrainFeatures;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;


namespace NermNermNerm.Stardew.QuestableTractor
{
    internal class LoaderQuestController
        : TractorPartQuestController<LoaderQuestState>
    {
        public LoaderQuestController(ModEntry mod) : base(mod)
        {
            this.AddPetFinder();
        }

        protected override string QuestCompleteMessage => L("Sweet!  You've now got a front-end loader attachment for your tractor to clear out debris!#$b#HINT: To use it, equip the pick or the axe while on the tractor.");
        protected override string ModDataKey => ModDataKeys.LoaderQuestStatus;
        public override string WorkingAttachmentPartId => ObjectIds.WorkingLoader;
        public override string BrokenAttachmentPartId => ObjectIds.BustedLoader;
        public override string HintTopicConversationKey => ConversationKeys.LoaderNotFound;

        protected override LoaderQuest CreatePartQuest()
        {
            return new LoaderQuest(this);
        }

        public override void Fix()
        {
            // Assume that the player can't get the part because it's somewhere crazy - clear it off the farm because we're always going to start the quest.
            this.PickUpBrokenAttachmentPart();

            this.EnsureInventory(ObjectIds.BustedLoader, this.OverallQuestState == OverallQuestState.NotStarted
                || (this.OverallQuestState == OverallQuestState.InProgress && this.State < LoaderQuestState.WaitForClint1));
            this.EnsureInventory(ObjectIds.WorkingLoader, this.OverallQuestState == OverallQuestState.InProgress && this.State == LoaderQuestState.InstallTheLoader);
            this.EnsureInventory(ObjectIds.AlexesOldShoe, this.OverallQuestState == OverallQuestState.InProgress && this.State == LoaderQuestState.DisguiseTheShoes);
            this.EnsureInventory(ObjectIds.DisguisedShoe, this.OverallQuestState == OverallQuestState.InProgress && this.State == LoaderQuestState.GiveShoesToClint);
        }

        protected override LoaderQuestState AdvanceStateForDayPassing(LoaderQuestState oldState)
        {
            switch (oldState)
            {
                case LoaderQuestState.LinusSniffing1:
                    return LoaderQuestState.LinusSniffing2;
                case LoaderQuestState.LinusSniffing2:
                    return LoaderQuestState.LinusSniffing3;
                case LoaderQuestState.LinusSniffing3:
                    return LoaderQuestState.LinusSniffing4;
                case LoaderQuestState.LinusSniffing4:
                    Game1.player.mailForTomorrow.Add(MailKeys.LinusFoundShoes);
                    return LoaderQuestState.LinusSniffing5;
                case LoaderQuestState.WaitForClint1:
                    return LoaderQuestState.WaitForClint2;
                case LoaderQuestState.WaitForClint2:
                    return LoaderQuestState.PickUpLoader;
                default:
                    return oldState;
            }
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            if (this.OverallQuestState == OverallQuestState.InProgress && this.State >= LoaderQuestState.FindSomeShoes && this.State < LoaderQuestState.GiveShoesToClint)
            {
                this.MonitorInventoryForItem(ObjectIds.AlexesOldShoe, this.OnPlayerGotOldShoes);
                this.MonitorInventoryForItem(ObjectIds.DisguisedShoe, this.OnPlayerGotDisguisedShoes);
            }
            else
            {
                this.StopMonitoringInventoryFor(ObjectIds.AlexesOldShoe);
                this.StopMonitoringInventoryFor(ObjectIds.DisguisedShoe);
            }
        }

        private void OnPlayerGotOldShoes(Item oldShoes)
        {
            //  MonitorInventoryForItem guarantees Game1.player.IsMainPlayer
            var quest = FakeQuest.GetFakeQuestByType<LoaderQuest>(Game1.player);
            if (quest is null)
            {
                this.LogWarning($"Player found {oldShoes.ItemId} when the Loader quest was not active");
            }
            else
            {
                quest.OnPlayerGotOldShoes(oldShoes);
            }
        }

        protected new LoaderQuest? GetQuest(Farmer player) => (LoaderQuest?)base.GetQuest(player);

        private void OnPlayerGotDisguisedShoes(Item dyedShoes)
        {
            this.StopMonitoringInventoryFor(ObjectIds.DisguisedShoe);
            var quest = this.GetQuest(Game1.player);
            if (quest is null)
            {
                this.LogWarning($"Player found {dyedShoes.ItemId}, when the quest was not active");
            }
            else
            {
                quest.OnPlayerGotDisguisedShoes(dyedShoes);
            }
        }

        protected override void HideStarterItemIfNeeded()
        {
            this.PlaceBrokenPartUnderClump(ResourceClump.boulderIndex);
        }

        internal void EditGarbageCanAsset(GarbageCanData gcd)
        {
            if (this.OverallQuestState == OverallQuestState.InProgress &&
                this.State >= LoaderQuestState.FindSomeShoes && this.State < LoaderQuestState.DisguiseTheShoes)
            {
                this.LogTrace($"Added shoes to trashcan loot table");
                gcd.GarbageCans["Evelyn"].Items.Add(new GarbageCanItemData()
                {
                    ItemId = ObjectIds.AlexesOldShoe,
                    IgnoreBaseChance = true,
                    Condition = I("RANDOM 0.3 @addDailyLuck"),
                    Id = "QuestableTractor.AlexesOldShoe",
                });
            }
            else
            {
                this.LogTrace($"Left Evelyn's garbage can alone");
            }
        }
    }
}
