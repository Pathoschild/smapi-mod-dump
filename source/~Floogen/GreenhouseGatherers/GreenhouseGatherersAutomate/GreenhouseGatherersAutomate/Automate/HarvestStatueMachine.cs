/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using GreenhouseGatherers.GreenhouseGatherers.Objects;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System.Linq;

namespace GreenhouseGatherersAutomate.GreenhouseGatherersAutomate.Automate
{
    public class HarvestStatueMachine : IMachine
    {
        private readonly HarvestStatue Entity;
        private int[] validOutputCategories = new int[] { -75, -79, -81 }; // Vegetables, Fruit, Forage

        /// <summary>The location which contains the machine.</summary>
        public GameLocation Location { get; }

        /// <summary>The tile area covered by the machine.</summary>
        public Rectangle TileArea { get; }

        /// <summary>A unique ID for the machine type.</summary>
        /// <remarks>This value should be identical for two machines if they have the exact same behavior and input logic. For example, if one machine in a group can't process input due to missing items, Automate will skip any other empty machines of that type in the same group since it assumes they need the same inputs.</remarks>
        public string MachineTypeID { get; } = "PeacefulEnd.GreenhouseGatherers/HarvestStatue";

        /// <summary>Construct an instance.</summary>
        /// <param name="entity">The underlying entity.</param>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="tile">The tile covered by the machine.</param>
        public HarvestStatueMachine(Object entity, GameLocation location, in Vector2 tile)
        {
            Entity = entity as HarvestStatue;
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        /// <summary>Get the machine's processing state.</summary>
        public MachineState GetState()
        {
            if (Entity.items.Where(i => validOutputCategories.Contains(i.Category)).Count() == 0)
            {
                return MachineState.Empty;
            }

            return MachineState.Done;
        }

        /// <summary>Get the output item.</summary>
        public ITrackedStack GetOutput()
        {
            Item validSelectedItem = Entity.items.First(i => validOutputCategories.Contains(i.Category));
            return new TrackedItem(validSelectedItem, onEmpty: item =>
            {
                Entity.items.Remove(validSelectedItem);
            });
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public bool SetInput(IStorage input)
        {
            // Harvest Statue accepts no input, purely output
            return false;
        }
    }
}
