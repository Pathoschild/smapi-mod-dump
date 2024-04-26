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
using SObject = StardewValley.Object;

namespace BetterHoneyMead.ModPatches {
    internal static class MachineData {
        public static void Register() {
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), "GetOutputItem"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_MachineDataUtility_GetOutputItem))
            );
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(StardewValley.ItemTypeDefinitions.ObjectDataDefinition), "CreateFlavoredHoney"),
                postfix: new HarmonyMethod(typeof(MachineData), nameof(Postfix_ObjectDataDefinition_CreateFlavoredHoney))
            );
        }

        private static void Postfix_MachineDataUtility_GetOutputItem(
            ref Item __result,
            MachineItemOutput outputData, Item inputItem
        ) {
            if (__result is SObject item1 && item1 is not null) {
                if ((outputData?.PreserveId ?? "") == "DROP_IN_PRESERVE"
                    && inputItem is SObject item2 && item2 is not null
                ) {
                    item1.preservedParentSheetIndex.Value = item2.preservedParentSheetIndex.Value;
                }
            }
        }

        private static void Postfix_ObjectDataDefinition_CreateFlavoredHoney(
            ref SObject __result,
            SObject ingredient
        ) {
            var color = TailoringMenu.GetDyeColor(ingredient) ?? Color.Yellow;
            var honey = new ColoredObject("340", 1, color) {
                Name = __result.Name,
                Price = __result.Price
            };
            honey.preserve.Value = __result.preserve.Value;
            honey.preservedParentSheetIndex.Value = __result.preservedParentSheetIndex.Value;
            __result = honey;
        }
    }
}
