/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework.Machines.Objects
{
    /// <summary>A hay hopper that accepts input and provides output.</summary>
    /// <remarks>Derived from <see cref="SObject.performObjectDropInAction"/> (search for 'Feed Hopper').</remarks>
    internal class FeedHopperMachine : BaseMachine
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="location">The location containing the machine.</param>
        /// <param name="tile">The tile covered by the machine.</param>
        public FeedHopperMachine(GameLocation location, Vector2 tile)
            : base(location, BaseMachine.GetTileAreaFor(tile)) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="silo">The silo to automate.</param>
        /// <param name="location">The location containing the machine.</param>
        public FeedHopperMachine(Building silo, GameLocation location)
            : base(location, BaseMachine.GetTileAreaFor(silo)) { }

        /// <summary>Get the machine's processing state.</summary>
        public override MachineState GetState()
        {
            return this.GetFreeSpace(this.Location) > 0
                ? MachineState.Empty // 'empty' insofar as it will accept more input, not necessarily empty
                : MachineState.Disabled;
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack? GetOutput()
        {
            return null;
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool SetInput(IStorage input)
        {
            GameLocation location = this.Location;

            // skip if full
            if (this.GetFreeSpace(location) <= 0)
                return false;

            // try to add hay (178) until full
            bool anyPulled = false;
            foreach (ITrackedStack stack in input.GetItems().Where(p => p.Sample.QualifiedItemId == "(O)178"))
            {
                // get free space
                int space = this.GetFreeSpace(location);
                if (space <= 0)
                    return anyPulled;

                // pull hay
                int maxToAdd = Math.Min(stack.Count, space);
                int added = maxToAdd - location.tryToAddHay(maxToAdd);
                stack.Reduce(added);
                if (added > 0)
                    anyPulled = true;
            }

            return anyPulled;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the amount of hay the hopper can still accept before it's full.</summary>
        /// <param name="location">The location to check.</param>
        /// <remarks>Derived from <see cref="GameLocation.tryToAddHay"/>.</remarks>
        private int GetFreeSpace(GameLocation location)
        {
            return location.GetHayCapacity() - location.piecesOfHay.Value;
        }
    }
}
