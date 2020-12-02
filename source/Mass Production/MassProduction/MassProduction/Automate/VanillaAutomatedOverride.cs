/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using MassProduction.VanillaOverrides;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction.Automate
{
    /// <summary>
    /// Overrides the vanilla machine implementations used in the base Automate.
    /// </summary>
    public class VanillaAutomatedOverride : IMachine
    {
        public string MachineTypeID { get { return "MassProduction.Override." + OriginalMachine.MachineTypeID; } }
        public GameLocation Location { get { return OriginalMachine.Location; } }
        public Rectangle TileArea { get { return OriginalMachine.TileArea; } }

        public IMachine OriginalMachine;
        public SObject OriginalMachineObject;
        public MPMAutomated OverridingMachine;

        public VanillaAutomatedOverride(IMachine original)
        {
            OriginalMachine = original;
            OriginalMachineObject = ModEntry.Instance.Helper.Reflection.GetProperty<SObject>(original, "Machine").GetValue();
            OverridingMachine = new MPMAutomated(OriginalMachineObject, Location, OriginalMachineObject.TileLocation);
        }

        public ITrackedStack GetOutput()
        {
            ITrackedStack overrideOutput = OverridingMachine.GetOutput();

            if (overrideOutput != null)
            {
                return overrideOutput;
            }
            else
            {
                MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(OriginalMachineObject.name, OriginalMachineObject.GetMassProducerKey());

                if (mpm != null)
                {
                    // Check for special overriding logic, if any is required
                    string originalClassName = OriginalMachine.GetType().FullName;

                    if (VanillaOverrideList.GetFor(originalClassName) != null)
                    {
                        IVanillaOverride vanillaOverride = VanillaOverrideList.GetFor(originalClassName);
                        overrideOutput = vanillaOverride.Automate_GetOutput(mpm, OriginalMachine, OriginalMachineObject);

                        return overrideOutput;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return OriginalMachine.GetOutput();
                }
            }
        }

        public MachineState GetState()
        {
            return OverridingMachine.GetState();
        }

        public bool SetInput(IStorage input)
        {
            if (OverridingMachine.SetInput(input))
            {
                return true;
            }
            else
            {
                MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(OriginalMachineObject.name, OriginalMachineObject.GetMassProducerKey());

                if (mpm != null)
                {
                    //Check for special overriding logic, if any is required
                    string originalClassName = OriginalMachine.GetType().FullName;

                    if (VanillaOverrideList.GetFor(originalClassName) != null)
                    {
                        IVanillaOverride vanillaOverride = VanillaOverrideList.GetFor(originalClassName);

                        return vanillaOverride.Automate_SetInput(input, mpm, OriginalMachine, OriginalMachineObject);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return OriginalMachine.SetInput(input);
                }
            }
        }
    }
}
