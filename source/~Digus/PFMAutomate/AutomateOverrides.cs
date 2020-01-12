using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using PFMAutomate.Automate;
using ProducerFrameworkMod;
using StardewValley;
using SObject = StardewValley.Object;

namespace PFMAutomate
{
    public class AutomateOverrides
    {
        public static readonly HashSet<string> SupportedVanillaMachines = new HashSet<string>()
        {
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CharcoalKilnMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.FurnaceMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SlimeEggPressMachine"
        };

        [HarmonyPriority(800)]
        public static void GetFor(ref object __result, SObject obj)
        {
            string machineFullName = __result?.GetType().FullName;
            if (SupportedVanillaMachines.Contains(machineFullName) && ProducerController.HasProducerRule(obj.Name))
            {
                __result = new VanillaProducerMachine((IMachine)__result);
            }
        }
    }
}