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
using ProducerFrameworkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction.Automate
{
    public class AutomateOverrides
    {
        public static readonly HashSet<string> PFM_SUPPORTED_VANILLA_MACHINES = new HashSet<string>()
        {
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CharcoalKilnMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.FurnaceMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SlimeEggPressMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SodaMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.StatueOfPerfectionMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.WormBinMachine"
        };

        /// <summary>
        /// Overrides the base Automate machine factory's return value with one from this mod if that machine needs special support to
        /// make use of the mass production upgrades. Also replicated PFMAutomate's functionality.
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="obj"></param>
        public static void GetFor(ref object __result, SObject obj)
        {
            if (__result != null && __result is IMachine machine)
            {
                string machineFullName = __result.GetType().FullName;
                
                if (VanillaOverrides.VanillaOverrideList.AUTOMATE_OVERRIDES.ContainsKey(machineFullName) ||
                    PFM_SUPPORTED_VANILLA_MACHINES.Contains(machineFullName) && (ProducerController.HasProducerRule(obj.Name) || ProducerController.GetProducerConfig(obj.Name) != null))
                {
                    __result = new VanillaAutomatedOverride((IMachine)__result);
                }
            }
        }
    }
}
