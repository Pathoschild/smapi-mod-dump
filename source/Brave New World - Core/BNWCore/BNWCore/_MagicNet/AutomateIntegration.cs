/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Pathoschild.Stardew.Automate;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;

using Entry = BNWCore.ModEntry;

namespace BNWCore.Automate
{
    public interface IAutomateApi
    {
        void AddFactory(IAutomationFactory factory);
    }
    public class BNWCoreMagicNetMachine : IMachine
    {
        private readonly BNWCoreMagicNet magicNet;

        public GameLocation Location { get; }

        public Rectangle TileArea { get; }
        public string MachineTypeID => $"DiogoAlbano.BNWCore/BNWCoreMagicNet";
        public BNWCoreMagicNetMachine(BNWCoreMagicNet entity, GameLocation location, in Vector2 tile)
        {
            magicNet = entity;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }
        public MachineState GetState()
        {
            MachineState state;
            if (magicNet.heldObject.Value is null)
                state = MachineState.Empty;
            else
                state = magicNet.readyForHarvest.Value ? MachineState.Done : MachineState.Processing;

            if (state == MachineState.Empty && (magicNet.bait.Value is not null || Game1.getFarmer(magicNet.owner.Value).professions.Contains(11)))
                state = MachineState.Processing;

            return state;
        }
        public ITrackedStack GetOutput() => new TrackedItem(magicNet.heldObject.Value, onEmpty: (i) =>
        {
            Farmer owner = Game1.getFarmer(magicNet.owner.Value);
            owner.gainExperience(1, 5);
            owner.caughtFish(i.ParentSheetIndex, -1);
            magicNet.heldObject.Value = null;
            magicNet.readyForHarvest.Value = false;
            magicNet.bait.Value = null;
        });
        public bool SetInput(IStorage input)
        {
            if (input.TryGetIngredient(Object.baitCategory, 1, out IConsumable? bait))
            {
                magicNet.bait.Value = (Object)bait.Take();
                return true;
            }
            return false;
        }
    }
    public class BNWCoreMagicNetFactory : IAutomationFactory
    {
        public IAutomatable GetFor(Object obj, GameLocation location, in Vector2 tile)
        {
            if (obj.ParentSheetIndex == Entry.BNWCoreMagicNetId)
                return new BNWCoreMagicNetMachine((BNWCoreMagicNet)obj, location, tile);
            return null;
        }
        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile) => null;
        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile) => null;
        public IAutomatable GetForTile(GameLocation location, in Vector2 tile) => null;
    }
}
