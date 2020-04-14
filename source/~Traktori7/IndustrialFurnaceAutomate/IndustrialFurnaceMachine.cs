using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using IndustrialFurnace;

/*
 Made using the template example provided by Pathoschild on
 https://github.com/Pathoschild/StardewMods/tree/develop/Automate

25.3.2020 
*/


namespace IndustrialFurnaceAutomate
{
    /// <summary>A machine that turns iron bars into gold bars.</summary>
    public class IndustrialFurnaceMachine : IMachine
    {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying entity.</summary>
        private readonly IndustrialFurnaceController controller;


        /*********
        ** Accessors
        *********/
        /// <summary>A unique ID for the machine type. Currently unused since the machine doesn't accept inputs.</summary>
        /// <remarks>From the Automate documentation:
        /// This value should be identical for two machines if they have the exact same behavior and input logic.
        /// For example, if one machine in a group can't process input due to missing items,
        /// Automate will skip any other empty machines of that type in the same group
        /// since it assumes they need the same inputs.
        /// </remarks>
        public string MachineTypeID { get; }
        /// <summary>The location which contains the machine.</summary>
        public GameLocation Location { get; }
        /// <summary>The tile area covered by the machine.</summary>
        /// <remarks>The area has to match the size of the building.</remarks>
        public Rectangle TileArea { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="entity">The underlying entity.</param>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="tile">The tile covered by the machine.</param>
        public IndustrialFurnaceMachine(IndustrialFurnaceController entity, GameLocation location, in Vector2 tile)
        {
            this.controller = entity;
            this.MachineTypeID = "Industrial Furnace";
            this.Location = location;
            this.TileArea = new Rectangle((int)tile.X, (int)tile.Y, controller.furnace.tilesWide.Value, controller.furnace.tilesHigh.Value);
        }


        /// <summary>Get the machine's processing state.</summary>
        public MachineState GetState()
        {
            //if (this.Entity.furnace.isUnderConstruction())
              //  return MachineState.Disabled;

            //this.Entity.output.clearNulls();

            if (this.controller.output.items.Any(item => item != null))
            {
                return MachineState.Done;
            }

            // Allow only taking stuff out
            return MachineState.Disabled;
        }


        /// <summary>Get the output item.</summary>
        public ITrackedStack GetOutput()
        {
            IList<Item> inventory = controller.output.items;

            return new TrackedItem(inventory.FirstOrDefault(item => item != null), onEmpty: this.OnOutputTaken);
        }


        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public bool SetInput(IStorage input)
        {
            // Do not allow input
            return false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Remove an output item once it's been taken.</summary>
        /// <param name="item">The removed item.</param>
        private void OnOutputTaken(Item item)
        {
            this.controller.TakeFromOutput(item, null);
        }
    }
}