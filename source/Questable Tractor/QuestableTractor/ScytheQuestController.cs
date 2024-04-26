/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class ScytheQuestController : TractorPartQuestController<ScytheQuestState>
    {
        public ScytheQuestController(ModEntry mod) : base(mod)
        {
            this.AddPetFinder();
        }

        public override void Fix()
        {
            // Assume that the player can't get the part because it's somewhere crazy
            this.PickUpBrokenAttachmentPart();
            this.EnsureInventory(ObjectIds.WorkingScythe, this.OverallQuestState == OverallQuestState.InProgress && this.State.Progress == ScytheQuestStateProgress.InstallPart);
            this.EnsureInventory(ObjectIds.BustedScythe, this.OverallQuestState == OverallQuestState.NotStarted
                || (this.OverallQuestState == OverallQuestState.InProgress && this.State.Progress < ScytheQuestStateProgress.InstallPart));
            this.EnsureInventory(ObjectIds.ScythePart1, this.OverallQuestState == OverallQuestState.InProgress && this.State.Progress < ScytheQuestStateProgress.InstallPart && this.State.VincentPartGot);
            this.EnsureInventory(ObjectIds.ScythePart2, this.OverallQuestState == OverallQuestState.InProgress && this.State.Progress < ScytheQuestStateProgress.InstallPart && this.State.JasPartGot);
        }

        protected override ScytheQuest CreatePartQuest() => new ScytheQuest(this);

        public override string WorkingAttachmentPartId => ObjectIds.WorkingScythe;
        public override string BrokenAttachmentPartId => ObjectIds.BustedScythe;
        public override string HintTopicConversationKey => ConversationKeys.ScytheNotFound;
        protected override string QuestCompleteMessage => "Sweet!  You've now got a harvester attachment for your tractor!#$b#HINT: To use it, equip the scythe while on the tractor.";
        protected override string ModDataKey => ModDataKeys.ScytheQuestStatus;
        protected override void HideStarterItemIfNeeded() => base.PlaceBrokenPartUnderClump(ResourceClump.hollowLogIndex);


        protected override ScytheQuestState AdvanceStateForDayPassing(ScytheQuestState oldState) => oldState;

        protected override bool TryParse(string rawState, out ScytheQuestState result) => ScytheQuestState.TryParse(rawState, out result);
    }
}
