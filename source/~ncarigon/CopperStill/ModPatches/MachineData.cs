/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.Machines;
using SObject = StardewValley.Object;
using SMachineData = StardewValley.GameData.Machines.MachineData;
using System.Linq;
using System;

namespace CopperStill.ModPatches {
    internal static class MachineData {
        public static void Register() {
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), "GetOutputItem"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_MachineDataUtility_GetOutputItem))
            );
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(SObject), "PlaceInMachine"),
                prefix: new HarmonyMethod(typeof(MachineData), nameof(Prefix_Object_PlaceInMachine)),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_Object_PlaceInMachine))
            );
        }

        private static void Postfix_MachineDataUtility_GetOutputItem(
            ref Item __result,
            MachineItemOutput outputData, Item inputItem
        ) {
            if ((outputData?.PreserveId ?? "") == "DROP_IN_PRESERVE"
                && __result is SObject item1 && inputItem is SObject item2
                && item1 is not null & item2 is not null
            ) {
                item1!.preservedParentSheetIndex.Value = item2!.preservedParentSheetIndex.Value;
            }
        }

        private static bool FromOverride = false;

        private static void Prefix_Object_PlaceInMachine(
            ref bool probe, out Tuple<bool, bool>? __state
        ) {
            __state = new(false, probe);
            // need to detect whether the call came from the base game or our logic below
            if (!FromOverride) {
                FromOverride = true;
                // state = (useOutLogic?, wasProbeCall?)
                __state = new(true, probe);
                // override original value to prevent any actions from the base game call
                probe = true;
            }
        }

        private static void Postfix_Object_PlaceInMachine(
           SObject __instance, ref bool __result, Tuple<bool, bool>? __state,
           SMachineData machineData, Item inputItem, Farmer who, bool showMessages, bool playSounds
        ) {
            // only use our logic if called from base game
            if (__state?.Item1 ?? false) {
                // search for all machines
                foreach (var machineOverride in Game1.content.Load<Dictionary<string, SMachineData>>("Data\\Machines")
                    // which start with 'QualifiedMachineId:' which will be alternate operations
                    .Where(m => m.Key?.StartsWith($"{__instance.QualifiedItemId}:") ?? false)
                    // only need the MachineData
                    .Select(m => m.Value)
                ) {
                    // test wheterh alternate machine data can successfully process our item
                    if (MachineDataUtility.TryGetMachineOutputRule(
                        __instance, machineOverride, MachineOutputTrigger.ItemPlacedInMachine, inputItem, who, __instance.Location,
                        out _, out _, out _, out var triggerIgnoringCount)
                        || triggerIgnoringCount is not null
                    ) {
                        // if yes, perform real call and update results for original game call
                        __result = (__state?.Item2 ?? false) || __instance.PlaceInMachine(machineOverride, inputItem, __state?.Item2 ?? false, who, showMessages, playSounds);
                        // disable our logic
                        FromOverride = false;
                        // stop looking
                        return;
                    }
                }
                // no alternate operations were successful, perform original game logic
                __result = __instance.PlaceInMachine(machineData, inputItem, __state?.Item2 ?? false, who, showMessages, playSounds);
                // disable our logic
                FromOverride = false;
            }
        }
    }
}
