/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class RestoreTractorQuestController
        : BaseQuestController<RestorationState>
    {
        public RestoreTractorQuestController(ModEntry mod) : base(mod) { }

        public record EngineRequirement(string itemId, string displayName, int quantity);
        public static readonly IReadOnlyCollection<EngineRequirement> engineRequirements = new EngineRequirement[]
        {
            new EngineRequirement(ObjectIds.BustedEngine, "the engine", 1),
            new EngineRequirement("92", "20 sap", 20),
            new EngineRequirement("770", "5 mixed seeds", 5),
            new EngineRequirement("62", "an aquamarine", 1),
        };

        protected override string ModDataKey => ModDataKeys.MainQuestStatus;

        private Stable? GetGarage() => Game1.getFarm().buildings.OfType<Stable>().FirstOrDefault(s => s.buildingType.Value == TractorModConfig.GarageBuildingId);

        protected override RestorationState AdvanceStateForDayPassing(RestorationState oldState)
        {
            RestorationState newState = oldState;
            switch (oldState)
            {
                case RestorationState.WaitingForMailFromRobinDay1:
                    newState = RestorationState.WaitingForMailFromRobinDay2;
                    break;
                case RestorationState.WaitingForMailFromRobinDay2:
                    newState = RestorationState.BuildTractorGarage;
                    Game1.addMail(MailKeys.BuildTheGarage);
                    break;
                case RestorationState.BuildTractorGarage:
                    var garage = this.GetGarage();
                    if (garage?.isUnderConstruction() == false)
                    {
                        newState = RestorationState.WaitingForSebastianDay1;
                    }
                    break;
                case RestorationState.WaitingForSebastianDay1:
                    newState = RestorationState.WaitingForSebastianDay2;
                    break;
                case RestorationState.WaitingForSebastianDay2:
                    Game1.addMail(MailKeys.FixTheEngine);
                    newState = RestorationState.TalkToWizard;
                    break;
                case RestorationState.BringStuffToForest:
                    if (CheckForest())
                    {
                        newState = RestorationState.BringEngineToSebastian;
                    }
                    break;
                case RestorationState.WaitForEngineInstall:
                    Game1.player.mailbox.Add(MailKeys.TractorDoneMail);
                    newState = RestorationState.Complete;
                    break;
            }

            return newState;
        }

        protected override void OnStateChanged()
        {
            if (!this.IsStarted)
            {
                return;
            }

            // Doing these invalidations here rather than when the state is set is kindof roundabout,
            // but at least in the case of AdvanceStateForDayPassing, that doesn't work because when
            // you invalidate the cache state of buildings, it immediately reloads them, and thus the
            // state that we have hasn't actually propogated to out to the persistent state yet,
            // so we read the wrong value.
            switch (this.State)
            {
                case RestorationState.BuildTractorGarage:
                    this.Mod.TractorModConfig.TractorGarageBuildingCostChanged();
                    break;
                case RestorationState.WaitingForSebastianDay1:
                    this.Mod.TractorModConfig.TractorGarageBuildingCostChanged();
                    break;

            }
        }

        protected override BaseQuest CreateQuest() => new RestoreTractorQuest(this);

        protected override void OnDayStartedQuestNotStarted()
        {
            DerelictTractorTerrainFeature.PlaceInField(this.Mod);
        }

        protected override void OnDayStartedQuestInProgress()
        {
            base.OnDayStartedQuestInProgress();

            var state = this.State;
            if (state.IsDerelictInTheFields())
            {
                DerelictTractorTerrainFeature.PlaceInField(this.Mod);
            }
            else if (state.IsDerelictInTheGarage())
            {
                var garage = this.GetGarage();
                if (garage is null || garage.isUnderConstruction())
                {
                    // Could happen I suppose if the user deleted the garage while on the quest.  They can fix it themselves by rebuilding the garage...
                    this.LogError($"Tractor main quest state is {state} but there's no garage??");
                }
                else
                {
                    DerelictTractorTerrainFeature.PlaceInGarage(this.Mod, garage);
                }
            }
        }

        private static bool CheckForest()
        {
            var magicChest = Game1.getLocationFromName("Woods").objects.Values.OfType<Chest>()
                .FirstOrDefault(chest => engineRequirements.All(er => chest.Items.Any(i => i.ItemId == er.itemId && i.Stack >= er.quantity)));

            if (magicChest == null)
            {
                return false;
            }

            foreach (var requirement in engineRequirements)
            {
                var item = magicChest.Items.First(i => i.ItemId == requirement.itemId && i.Stack >= requirement.quantity);
                if (item.Stack > requirement.quantity)
                {
                    item.Stack -= requirement.quantity;
                }
                else
                {
                    magicChest.Items.Remove(item);
                }
            }
            magicChest.Items.Add(new StardewValley.Object(ObjectIds.WorkingEngine, 1));

            return true;
        }

    }
}
