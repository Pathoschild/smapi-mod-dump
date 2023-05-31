/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.Objects.Machines
{
    public class MachineWrapper<T> : CustomObjectWrapper<T> where T : Machine
    {

        public MachineWrapper()
        {

        }

        /// <summary>
        /// Used to automate <see cref="CustomObject"/>s for the mod Revitalize.
        /// </summary>
        /// <param name="CustomObject"></param>
        public MachineWrapper(T CustomObject, GameLocation location, in Vector2 TileLocation) : base(CustomObject, location, TileLocation)
        {
        }

        /// <summary>
        /// Used to set the inputs for this machine to begin crafting.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool SetInput(IStorage input)
        {
            //Optimize to ensure that we don't do unnecessary logic for setting inputs.
            if (this.customObject.isWorking() || this.customObject.finishedProduction()) return false;

            IList<Item> validItems = new List<Item>();
            foreach (ITrackedStack obj in input.GetItems())
            {
                Item item = obj.Sample;
                if (item == null) continue;
                item.Stack = obj.Count;

                validItems.Add(item);
            }

            CraftingResult result = this.customObject.processInput(validItems, null, false);
            //Need a way to reduce stack size amounts again. Iterate across items that were taken???
            if (result.successful)
            {
                //obj.Take(result.consumedItems[0].StackSize);
                this.customObject.updateAnimation();

                foreach (ItemReference consumedItem in result.consumedItems)
                {
                    foreach (ITrackedStack obj in input.GetItems())
                    {
                        Item item = obj.Sample;
                        if (item == null) continue;
                        item.Stack = obj.Count;

                        ItemReference itemRef = new ItemReference(item);
                        if (itemRef.RegisteredObjectId.Equals(consumedItem.RegisteredObjectId))
                        {
                            obj.Reduce(consumedItem.StackSize);
                            break;
                        }
                    }
                }

                return true;
            }

            this.customObject.updateAnimation();

            return false;
        }

        /// <summary>
        /// Gets the current state of this machine.
        /// </summary>
        /// <returns></returns>
        public override MachineState GetState()
        {
            if (this.customObject.isIdle())
                return MachineState.Empty;

            if (this.customObject.finishedProduction())
                return MachineState.Done;

            if (this.customObject.isWorking()) return MachineState.Processing;

            return this.customObject.MinutesUntilReady == 0
                ? MachineState.Done
                : MachineState.Processing;

            //Could use the following for machines that are always producing.
            /*
             *  return this.Machine.heldObject.Value != null
                ? MachineState.Done
                : MachineState.Processing;
             */
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack GetOutput()
        {
            //Returns a tracked object which is set to modify the machine's held object value to be null when complete.
            return new TrackedItem(this.customObject.heldObject.Value, onReduced: item =>
            {
                this.customObject.heldObject.Value.Stack = item.Stack;
            }
            , onEmpty: item =>
            {
                this.customObject.clearHeldObject();
                this.customObject.heldObject.Value = (StardewValley.Object)this.customObject.getItemFromHeldItemQueue();
                this.customObject.readyForHarvest.Value= true;
            });
        }

    }
}
