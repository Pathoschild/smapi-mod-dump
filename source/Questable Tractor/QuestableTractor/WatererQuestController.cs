/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using StardewValley;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class WatererQuestController
        : TractorPartQuestController<WatererQuestState>
    {
        public WatererQuestController(ModEntry mod) : base(mod) { }

        public override void Fix()
        {
            this.EnsureInventory(ObjectIds.BustedWaterer, this.OverallQuestState == OverallQuestState.InProgress && this.State < WatererQuestState.WaitForMaruDay1);
            this.EnsureInventory(ObjectIds.WorkingWaterer, this.OverallQuestState == OverallQuestState.InProgress && this.State == WatererQuestState.InstallPart);
        }

        protected override WatererQuest CreatePartQuest() => new WatererQuest(this);

        protected override string QuestCompleteMessage => L("Awesome!  You've now got a way to water your crops with your tractor!#$b#HINT: To use it, equip the watering can while on the tractor.");

        protected override string ModDataKey => ModDataKeys.WateringQuestStatus;

        public override string WorkingAttachmentPartId => ObjectIds.WorkingWaterer;

        public override string BrokenAttachmentPartId => ObjectIds.BustedWaterer;

        public override string HintTopicConversationKey => ConversationKeys.WatererNotFound;

        public override void AnnounceGotBrokenPart(Item brokenPart)
        {
            // We want to act a lot differently than we do in the base class, as we got the item through fishing, holding it up would look dumb
            Spout(L("Whoah that was heavy!  Looks like an irrigator attachment for a tractor under all that mud!"));
        }

        protected override WatererQuestState AdvanceStateForDayPassing(WatererQuestState oldState)
        {
            if (oldState == WatererQuestState.WaitForMaruDay1)
            {
                Game1.player.mailForTomorrow.Add(MailKeys.WatererRepaired);
                return WatererQuestState.WaitForMaruDay2;
            }
            else
            {
                return oldState;
            }
        }
    }
}
