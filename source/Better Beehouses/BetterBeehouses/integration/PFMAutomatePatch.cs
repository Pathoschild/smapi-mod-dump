/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace BetterBeehouses.integration
{
    internal class PFMAutomatePatch
    {
        private static bool isPatched = false;
        private const string beehouseMachineName = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine";
        internal static bool Setup()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Digus.PFMAutomate"))
                return false;

            ModEntry.monitor.Log($"PFMAutomate integration {(ModEntry.config.PatchPFM && ModEntry.config.PatchAutomate ? "Enabling" : "Disabling")}.");

            var target = AccessTools.TypeByName("PFMAutomate.AutomateOverrides").MethodNamed("GetFor");

            if (!isPatched && ModEntry.config.PatchPFM && ModEntry.config.PatchAutomate)
            {
                ModEntry.harmony.Patch(target, prefix: new(typeof(PFMAutomatePatch),nameof(Prefix)));
                isPatched = true;
            }
            else if(isPatched && !(ModEntry.config.PatchPFM && ModEntry.config.PatchAutomate))
            {
                ModEntry.harmony.Unpatch(target, HarmonyPatchType.Prefix, ModEntry.ModID);
                isPatched = false;
            }

            return true;
        }
        internal static bool Prefix(ref object __0)
            => __0?.GetType().FullName != beehouseMachineName;
    }
}
