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

namespace NermNermNerm.Stardew.QuestableTractor
{

    // Find a place to put this on day start when quest is not active
    //this.HideStarterItemIfNeeded();
    //this.MonitorInventoryForItem(this.BrokenAttachmentPartId, this.PlayerGotBrokenPart);


    internal class LoaderQuestController
        : TractorPartQuestController<LoaderQuestState>
    {
        public LoaderQuestController(ModEntry mod) : base(mod) { }

        protected override string QuestCompleteMessage => "Sweet!  You've now got a front-end loader attachment for your tractor to clear out debris!#$b#HINT: To use it, equip the pick or the axe while on the tractor.";
        protected override string ModDataKey => ModDataKeys.LoaderQuestStatus;
        public override string WorkingAttachmentPartId => ObjectIds.WorkingLoader;
        public override string BrokenAttachmentPartId => ObjectIds.BustedLoader;
        public override string HintTopicConversationKey => ConversationKeys.LoaderNotFound;

        protected override LoaderQuest CreatePartQuest()
        {
            return new LoaderQuest(this);
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
            var quest = Game1.player.questLog.OfType<LoaderQuest>().FirstOrDefault();
            if (quest is null)
            {
                this.LogWarning($"Player found {oldShoes.ItemId} when the Loader quest was not active");
            }
            else
            {
                quest.OnPlayerGotOldShoes(oldShoes);
            }
        }

        protected new LoaderQuest? GetQuest() => (LoaderQuest?)base.GetQuest();

        private void OnPlayerGotDisguisedShoes(Item dyedShoes)
        {
            this.StopMonitoringInventoryFor(ObjectIds.DisguisedShoe);
            var quest = this.GetQuest();
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
                this.LogTrace("Added shoes to trashcan loot table");
                gcd.GarbageCans["Evelyn"].Items.Add(new GarbageCanItemData()
                {
                    ItemId = ObjectIds.AlexesOldShoe,
                    IgnoreBaseChance = true,
                    Condition = "RANDOM 0.3 @addDailyLuck",
                    Id = "QuestableTractor.AlexesOldShoe",
                });
            }
            else
            {
                this.LogTrace("Left Evelyn's garbage can alone");
            }
        }
    }
}
