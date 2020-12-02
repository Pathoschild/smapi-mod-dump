/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Pathoschild.Stardew.Automate;
using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace MassProduction.VanillaOverrides
{
    /// <summary>
    /// Defines methods for overriding a vanilla machine's functionality to implement the effects of mass production upgrades.
    /// </summary>
    public interface IVanillaOverride
    {
        /// <summary>
        /// Replacement for Automate's IMachine.SetInput(). 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mpm"></param>
        /// <param name="originalMachine"></param>
        /// <param name="originalMachineObject"></param>
        /// <returns></returns>
        bool Automate_SetInput(IStorage input, MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject);

        /// <summary>
        /// Replacement for Automate's IMachine.GetOutput().
        /// </summary>
        /// <param name="mpm"></param>
        /// <param name="originalMachine"></param>
        /// <param name="originalMachineObject"></param>
        /// <returns></returns>
        ITrackedStack Automate_GetOutput(MassProductionMachineDefinition mpm, IMachine originalMachine, SObject originalMachineObject);

        /// <summary>
        /// Replacement for the object drop in action.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="input"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <param name="mpm"></param>
        /// <returns></returns>
        bool Manual_PerformObjectDropInAction(SObject machine, SObject input, bool probe, Farmer who, MassProductionMachineDefinition mpm);

        /// <summary>
        /// Replacement for the object drop down action. Used for no-input machines.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="mpm"></param>
        /// <returns></returns>
        bool Manual_PerformDropDownAction(SObject machine, MassProductionMachineDefinition mpm);
    }

    public class VanillaOverrideList
    {
        public static readonly Dictionary<string, IVanillaOverride> MACHINE_OVERRIDES = new Dictionary<string, IVanillaOverride>()
        {
            { "Seed Maker", new SeedMakerOverride() },
            { "Worm Bin", new WormBinOverride() },
        };

        public static readonly Dictionary<string, string> AUTOMATE_OVERRIDES = new Dictionary<string, string>()
        {
            { "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine", "Seed Maker" },
            { "Pathoschild.Stardew.Automate.Framework.Machines.Objects.WormBinMachine", "Worm Bin" },
        };

        /// <summary>
        /// Gets the appropriate override for the given machine.
        /// </summary>
        /// <param name="machineName">Name of the base machine as appears in-game or the full namespace qualified class name of an Automate.IMachine</param>
        /// <returns>Null if no override exists.</returns>
        public static IVanillaOverride GetFor(string machineName)
        {
            if (MACHINE_OVERRIDES.ContainsKey(machineName))
            {
                return MACHINE_OVERRIDES[machineName];
            }
            else if (AUTOMATE_OVERRIDES.ContainsKey(machineName) && MACHINE_OVERRIDES.ContainsKey(AUTOMATE_OVERRIDES[machineName]))
            {
                return MACHINE_OVERRIDES[AUTOMATE_OVERRIDES[machineName]];
            }

            return null;
        }
    }
}
