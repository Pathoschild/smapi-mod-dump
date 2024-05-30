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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.ItemTypeDefinitions;
using SObject = StardewValley.Object;

namespace BetterHoneyMead.ModPatches {
    internal static class MachineData {
        public static void Register() {
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), "GetOutputItem"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_MachineDataUtility_GetOutputItem))
            );
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(ObjectDataDefinition), "CreateFlavoredHoney"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_ObjectDataDefinition_CreateFlavoredHoney))
            );
        }

        private static void Postfix_MachineDataUtility_GetOutputItem(
            ref Item __result,
            SObject machine, MachineItemOutput outputData, Item inputItem
        ) {
            if ((machine?.QualifiedItemId?.Equals("(BC)12") ?? false) // keg
                && (outputData?.Id?.Equals("(O)459") ?? false) // mead
                && __result is SObject item1 && item1 is not null
                && inputItem is SObject item2 && item2 is not null
            ) {
                if (outputData?.PreserveId?.Equals("DROP_IN_PRESERVE") ?? false) {
                    item1.preservedParentSheetIndex.Value = item2.preservedParentSheetIndex.Value ?? "-1";
                }
                if (item1.DisplayName?.StartsWith(ItemRegistry.GetErrorItemName()) ?? false) {
                    __result = CreateItems.CreateFlavoredMead(null, item1.Stack, item1.Quality);
                }
            }
        }

        public static void Postfix_ObjectDataDefinition_CreateFlavoredHoney(
            ref SObject __result,
            SObject? ingredient
        ) {
            if (__result is not null) {
                var honey = CreateItems.CreateFlavoredHoney(ingredient, __result.Stack, __result.Quality);
                honey.Name = __result.Name;
                honey.Price = __result.Price;
                honey.preserve.Value = __result.preserve.Value;
                honey.preservedParentSheetIndex.Value = __result.preservedParentSheetIndex.Value ?? "-1";
                __result = honey;
            }
        }
    }
}
