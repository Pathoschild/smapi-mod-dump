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
using Omegasis.Revitalize.Framework.World.Objects;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.MachineWrappers
{
    /// <summary>
    /// A generic machine class for CustomObjects for revitalize.
    /// </summary>
    /// <typeparam name="T">The type of custom object wrapped by this wrapper.</typeparam>
    public class CustomObjectMachineWrapper<T> : BaseMachineWrapper where T:CustomObject
    {

        /// <summary>
        /// The custom object underlying this machine wrapper.
        /// </summary>
        public T customObject;

        /// <summary>
        /// The id for this machine which will be the same one used to identify objects in revitalize.
        /// </summary>
        public override string MachineTypeID => this.customObject.basicItemInformation.id;
        /// <summary>
        /// Empty constructor.
        /// </summary>

        public CustomObjectMachineWrapper()
        {

        }

        /// <summary>
        /// Used to automate <see cref="CustomObject"/>s for the mod Revitalize.
        /// </summary>
        /// <param name="CustomObject"></param>
        public CustomObjectMachineWrapper(T CustomObject, GameLocation location, in Vector2 TileLocation)
        {
            this.customObject = CustomObject;
            this.Location = location;
            this.TileArea = new Rectangle((int)TileLocation.X, (int)TileLocation.Y,(int) this.customObject.basicItemInformation.boundingBoxTileDimensions.X,(int) this.customObject.basicItemInformation.boundingBoxTileDimensions.Y);
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack GetOutput()
        {
            //Returns a tracked object which is set to modify the machine's held object value to be null when complete.
            return new TrackedItem(this.customObject.heldObject.Value, onEmpty: item =>
            {
                this.customObject.heldObject.Value = null;
                this.customObject.readyForHarvest.Value = false;
            });
        }

        /// <summary>
        /// Sets the input for the machine. Used for inputing recipes and such.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool SetInput(IStorage input)
        {
            return false;
        }

        /// <summary>
        /// Gets the current state of this machine.
        /// </summary>
        /// <returns></returns>
        public override MachineState GetState()
        {
            if (this.customObject.heldObject.Value == null && this.customObject.MinutesUntilReady==0)
                return MachineState.Empty;

            if (this.customObject.heldObject.Value != null && this.customObject.MinutesUntilReady == 0)
                return MachineState.Done;

            return this.customObject.MinutesUntilReady==0
                ? MachineState.Done
                : MachineState.Processing;

            //Could use the following for machines that are always producing.
            /*
             *  return this.Machine.heldObject.Value != null
                ? MachineState.Done
                : MachineState.Processing;
             */
        }
    }
}
