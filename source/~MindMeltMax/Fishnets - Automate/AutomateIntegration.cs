/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Pathoschild.Stardew.Automate;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;

using Entry = Fishnets.ModEntry;

namespace Fishnets.Automate
{
    public interface IAutomateApi
    {
        void AddFactory(IAutomationFactory factory);
    }

    public class FishNetMachine : IMachine
    {
        private readonly FishNet fishNet;

        public GameLocation Location { get; }

        public Rectangle TileArea { get; }

        public string MachineTypeID => $"MindMeltMax.Fishnets/FishNet";

        public FishNetMachine(FishNet entity, GameLocation location, in Vector2 tile)
        {
            fishNet = entity;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            MachineState state;
            if (fishNet.heldObject.Value is null)
                state = MachineState.Empty;
            else
                state = fishNet.readyForHarvest.Value ? MachineState.Done : MachineState.Processing;

            if (state == MachineState.Empty && (fishNet.bait.Value is not null || Game1.getFarmer(fishNet.owner.Value).professions.Contains(11)))
                state = MachineState.Processing;

            return state;
        }

        public ITrackedStack GetOutput() => new TrackedItem(fishNet.heldObject.Value, onEmpty: (i) =>
        {
            Farmer owner = Game1.getFarmer(fishNet.owner.Value);
            owner.gainExperience(1, 5);
            owner.caughtFish(i.ParentSheetIndex, -1);
            fishNet.heldObject.Value = null;
            fishNet.readyForHarvest.Value = false;
            fishNet.bait.Value = null;
        });

        public bool SetInput(IStorage input)
        {
            if (input.TryGetIngredient(Object.baitCategory, 1, out IConsumable? bait))
            {
                fishNet.bait.Value = (Object)bait.Take();
                return true;
            }

            return false;
        }
    }

    public class FishNetFactory : IAutomationFactory
    {
        public IAutomatable GetFor(Object obj, GameLocation location, in Vector2 tile)
        {
            if (obj.ParentSheetIndex == Entry.FishNetId)
                return new FishNetMachine((FishNet)obj, location, tile);
            return null;
        }

        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile) => null;

        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile) => null;

        public IAutomatable GetForTile(GameLocation location, in Vector2 tile) => null;
    }
}
