/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

namespace MassProduction.VanillaOverrides
{
    public class WormBinOverride : IVanillaOverride
    {
        /// <inheritdoc/>
        public ITrackedStack Automate_GetOutput(MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject)
        {
            if (mpm != null)
            {
                int baitID = 685;
                int quantityProduced = mpm.Settings.CalculateOutputProduced(Game1.random.Next(2, 6));
                int timeRequired = mpm.Settings.CalculateTimeRequired(2600 - Game1.timeOfDay);

                return new TrackedItem(originalMachineObject.heldObject.Value, item =>
                {
                    originalMachineObject.heldObject.Value = new SObject(baitID, quantityProduced);
                    originalMachineObject.MinutesUntilReady = timeRequired;
                    originalMachineObject.readyForHarvest.Value = false;
                    originalMachineObject.showNextIndex.Value = false;
                });
            }

            return originalMachine.GetOutput();
        }

        /// <inheritdoc/>
        public bool Automate_SetInput(IStorage input, MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject)
        {
            return false;
        }

        public bool Manual_PerformDropDownAction(SObject machine, MassProductionMachineDefinition mpm)
        {
            if (mpm != null)
            {
                int baitID = 685;
                int quantityProduced = mpm.Settings.CalculateOutputProduced(Game1.random.Next(2, 6));
                int timeRequired = mpm.Settings.CalculateTimeRequired(2600 - Game1.timeOfDay);

                machine.heldObject.Value = new SObject(baitID, quantityProduced);
                machine.MinutesUntilReady = timeRequired;
                machine.readyForHarvest.Value = false;
                machine.showNextIndex.Value = false;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Manual_PerformObjectDropInAction(SObject machine, SObject input, bool probe, Farmer who, MassProductionMachineDefinition mpm)
        {
            return false;
        }
    }
}
