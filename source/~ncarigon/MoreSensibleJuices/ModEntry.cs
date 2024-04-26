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
using StardewModdingAPI;
using StardewValley.GameData.Machines;
using StardewValley;
using SObject = StardewValley.Object;

namespace MoreSensibleJuices {
    internal sealed class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            var harmony = new Harmony(helper.ModContent.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Item), "_PopulateContextTags"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_Item__PopulateContextTags))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), "GetOutputItem"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Postfix_MachineDataUtility_GetOutputItem))
            );
        }

        private static void Postfix_Item__PopulateContextTags(
            Item __instance, HashSet<string> tags
        ) {
            if (__instance.Name.Contains("_Juice_Fruit_")) {
                tags.Add("juice_fruit");
            } else if (__instance.Name.Contains("_Juice_Vegetable_")) {
                tags.Add("juice_vegetable");
            }
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
    }
}
