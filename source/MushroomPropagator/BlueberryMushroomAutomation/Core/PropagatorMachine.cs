/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace BlueberryMushroomAutomation
{
	public class PropagatorMachine : IMachine
	{
		private readonly BlueberryMushroomMachine.Propagator Entity;
		public string MachineTypeID { get; } = $"{BlueberryMushroomMachine.ModValues.PackageName}";
		public GameLocation Location { get; }
		public Rectangle TileArea { get; }

		/// <summary>Construct an instance.</summary>
		/// <param name="entity">The underlying entity.</param>
		/// <param name="location">The location which contains the machine.</param>
		/// <param name="tile">The tile covered by the machine.</param>
		public PropagatorMachine(BlueberryMushroomMachine.Propagator entity, GameLocation location, in Vector2 tile)
		{
			this.Entity = entity;
			this.Location = location;
			this.TileArea = new Rectangle(x: (int)tile.X, y: (int)tile.Y, width: 1, height: 1);
		}

		/// <summary>Get the machine's processing state.</summary>
		public MachineState GetState()
		{
			if (this.Entity.heldObject.Value is null)
			{
				return MachineState.Empty;
			}

			return this.Entity.readyForHarvest.Value ? MachineState.Done : MachineState.Processing;
		}

		/// <summary>Get the output item.</summary>
		public ITrackedStack GetOutput()
		{
			return new TrackedItem(item: this.Entity.heldObject.Value, onEmpty: (Item item) => this.Entity.PopHeldObject(giveNothing: true));
		}


		/// <summary>Provide input to the machine.</summary>
		/// <param name="input">The available items.</param>
		/// <returns>Returns whether the machine started processing an item.</returns>
		public bool SetInput(IStorage input)
		{
			return false;
		}
	}
}
