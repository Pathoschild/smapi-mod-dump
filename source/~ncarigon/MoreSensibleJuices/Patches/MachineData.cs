/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.GameData.Machines;
using StardewValley;
using SObject = StardewValley.Object;

namespace MoreSensibleJuices.Patches {
    internal static class MachineData {
        public static void Register() {
            ModEntry.Instance?.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(Item), "_PopulateContextTags"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_Item__PopulateContextTags))
            );
            ModEntry.Instance?.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), "GetOutputItem"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_MachineDataUtility_GetOutputItem))
            );
        }

        private static void Postfix_Item__PopulateContextTags(
            Item __instance, HashSet<string> tags
        ) {
            if (__instance?.Name?.Contains("_Juice_Fruit_") ?? false) {
                tags?.Add("juice_fruit");
            } else if (__instance?.Name?.Contains("_Juice_Vegetable_") ?? false) {
                tags?.Add("juice_vegetable");
            }
        }

        private static void Postfix_MachineDataUtility_GetOutputItem(
            ref Item __result,
            MachineItemOutput outputData, Item inputItem
        ) {
            if ((outputData?.PreserveId?.Equals("DROP_IN_PRESERVE") ?? false)
                && __result is SObject item1 && item1 is not null
                && inputItem is SObject item2 && item2?.preservedParentSheetIndex?.Value is not null
            ) {
                item1.preservedParentSheetIndex.Value = item2.preservedParentSheetIndex.Value;
            }
        }
    }
}