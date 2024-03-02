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
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework.Machines.Buildings
{
    /// <summary>A mill machine that accepts input and provides output.</summary>
    internal class MillMachine : BaseMachineForBuilding<Building>
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mill's input chest.</summary>
        private Chest Input => this.Machine.GetBuildingChest("Input");

        /// <summary>The mill's output chest.</summary>
        private Chest Output => this.Machine.GetBuildingChest("Output");

        /// <summary>The maximum input stack size to allow per qualified item ID, if different from <see cref="Item.maximumStackSize"/>.</summary>
        private readonly IDictionary<string, int> MaxInputStackSize;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="mill">The underlying mill.</param>
        /// <param name="location">The location which contains the machine.</param>
        public MillMachine(Building mill, GameLocation location)
            : base(mill, location, BaseMachine.GetTileAreaFor(mill))
        {
            this.MaxInputStackSize = new Dictionary<string, int>
            {
                ["(O)284"] = ItemRegistry.Create("(O)284").maximumStackSize() / 3 // beet => 3 sugar (reduce stack to avoid overfilling output)
            };
        }

        /// <summary>Get the machine's processing state.</summary>
        public override MachineState GetState()
        {
            if (this.Machine.isUnderConstruction())
                return MachineState.Disabled;

            if (this.Output.Items.Any(item => item != null))
                return MachineState.Done;

            return this.InputFull()
                ? MachineState.Processing
                : MachineState.Empty; // 'empty' insofar as it will accept more input, not necessarily empty
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack? GetOutput()
        {
            return this.GetTracked(this.Output.Items.FirstOrDefault(item => item != null), onEmpty: this.OnOutputTaken);
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool SetInput(IStorage input)
        {
            if (this.InputFull())
                return false;

            // fill input with wheat (262), beets (284), and rice (271)
            bool anyPulled = false;
            foreach (ITrackedStack stack in input.GetItems().Where(i => i.Sample.QualifiedItemId is "(O)262" or "(O)284" or "(O)271"))
            {
                // add item
                bool anyAdded = this.TryAddInput(stack);
                if (!anyAdded)
                    continue;
                anyPulled = true;

                // stop if full
                if (this.InputFull())
                    return true;
            }

            return anyPulled;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Try to add an item to the input queue, and adjust its stack size accordingly.</summary>
        /// <param name="item">The item stack to add.</param>
        /// <returns>Returns whether any items were taken from the stack.</returns>
        private bool TryAddInput(ITrackedStack item)
        {
            // nothing to add
            if (item.Count <= 0)
                return false;

            // clean up input bin
            this.Input.clearNulls();

            // try adding to input
            int originalSize = item.Count;
            IList<Item> slots = this.Input.Items;
            int maxStackSize = this.GetMaxInputStackSize(item.Sample);
            for (int i = 0; i < Chest.capacity; i++)
            {
                // done
                if (item.Count <= 0)
                    break;

                // add to existing slot
                if (slots.Count > i)
                {
                    Item slot = slots[i];
                    if (item.Sample.canStackWith(slot) && slot.Stack < maxStackSize)
                    {
                        Item sample = item.Sample.getOne();
                        sample.Stack = Math.Min(item.Count, maxStackSize - slot.Stack); // the most items we can add to the stack (in theory)
                        int actualAdded = sample.Stack - slot.addToStack(sample); // how many items were actually added to the stack
                        item.Reduce(actualAdded);
                    }
                    continue;
                }

                // add to new slot
                slots.Add(item.Take(Math.Min(item.Count, maxStackSize))!);
            }

            return item.Count < originalSize;
        }

        /// <summary>Get whether the mill's input bin is full.</summary>
        private bool InputFull()
        {
            Inventory slots = this.Input.Items;

            // free slots
            if (slots.Count < Chest.capacity)
                return false;

            // free space in stacks
            foreach (Item? slot in slots)
            {
                if (slot == null || slot.Stack < this.GetMaxInputStackSize(slot))
                    return false;
            }
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Remove an output item once it's been taken.</summary>
        /// <param name="item">The removed item.</param>
        private void OnOutputTaken(Item item)
        {
            this.Output.clearNulls();
            this.Output.Items.Remove(item);
        }

        /// <summary>Get the maximum input stack size to allow for an item.</summary>
        /// <param name="item">The input item to check.</param>
        private int GetMaxInputStackSize(Item? item)
        {
            if (item == null)
                return 0;

            return this.MaxInputStackSize.TryGetValue(item.QualifiedItemId, out int max)
                ? max
                : item.maximumStackSize();
        }
    }
}
