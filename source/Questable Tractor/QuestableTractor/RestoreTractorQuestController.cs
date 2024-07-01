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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class RestoreTractorQuestController
        : BaseQuestController<RestorationState>
    {
#pragma warning disable IDE0052 // Remove unread private members -- This is here to ensure it lives as long as the surrounding class.  If this class wasn't permanent, we'd dispose it.
        private readonly MasterPlayerModDataMonitor monitor;
#pragma warning restore IDE0052 // Remove unread private members

        public RestoreTractorQuestController(ModEntry mod) : base(mod)
        {
            this.monitor = new MasterPlayerModDataMonitor(mod.Helper, ModDataKeys.MainQuestStatus, () => mod.TractorModConfig.TractorGarageBuildingCostChanged());

            this.Mod.PetFindsThings.AddObjectFinder(this.PetTargetFinder);
        }

        private IEnumerable<(Point tileLocation, double chance)> PetTargetFinder()
        {
            if (this.OverallQuestState == OverallQuestState.NotStarted)
            {
                foreach (var tf in Game1.currentLocation.terrainFeatures.Values.OfType<DerelictTractorTerrainFeature>())
                {
                    yield return new() { tileLocation = tf.Tile.ToPoint(), chance = .03 };
                    break;
                }
            }
        }

        public override void Fix()
        {
            this.EnsureInventory(ObjectIds.BustedEngine, this.OverallQuestState == OverallQuestState.InProgress && (this.State == RestorationState.TalkToWizard || this.State == RestorationState.BringStuffToForest));
            this.EnsureInventory(ObjectIds.WorkingEngine, this.OverallQuestState == OverallQuestState.InProgress && (this.State == RestorationState.BringEngineToMaru || this.State == RestorationState.BringEngineToSebastian));

            if (this.OverallQuestState == OverallQuestState.NotStarted)
            {
                // Assume the tractor is unreachable - start the quest.
                this.CreateQuestNew(Game1.player);
            }
        }

        public record EngineRequirement(string itemId, int quantity);
        public static readonly IReadOnlyCollection<EngineRequirement> engineRequirements = new EngineRequirement[]
        {
            new EngineRequirement(ObjectIds.BustedEngine, 1),
            new EngineRequirement("92" /* sap */, 20),
            new EngineRequirement("770" /* mixed seeds */, 5),
            new EngineRequirement("62" /* aquamarine */, 1),
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
                case RestorationState.WaitingForSebastianDay1:
                    this.Mod.TractorModConfig.TractorGarageBuildingCostChanged();
                    break;

            }
        }

        protected override BaseQuest CreateQuest() => new RestoreTractorQuest();

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
                .FirstOrDefault(chest => engineRequirements.All(er => chest.Items.Any(i => i is not null && i.ItemId == er.itemId && i.Stack >= er.quantity)));

            if (magicChest == null)
            {
                return false;
            }

            foreach (var requirement in engineRequirements)
            {
                var item = magicChest.Items.First(i => i is not null && i.ItemId == requirement.itemId && i.Stack >= requirement.quantity);
                if (item.Stack > requirement.quantity)
                {
                    item.Stack -= requirement.quantity;
                }
                else
                {
                    magicChest.Items.Remove(item);
                }
            }

            var workingEngine = ItemRegistry.Create<StardewValley.Object>(ObjectIds.WorkingEngine, 1);
            workingEngine.questItem.Value = true;
            magicChest.Items.Add(workingEngine);

            return true;
        }
    }
}
