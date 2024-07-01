/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace BZP_Allergies.HarmonyPatches
{
    internal class Machine_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.PlaceInMachine)),
                prefix: new HarmonyMethod(typeof(Machine_Patches), nameof(PlaceInMachine_Prefix)),
                postfix: new HarmonyMethod(typeof(Machine_Patches), nameof(PlaceInMachine_Postfix))
            );
        }

        public static void PlaceInMachine_Prefix(Farmer who, bool probe, out Dictionary<string, Item>? __state)
        {
            try
            {
                __state = !probe ? InventoryUtils.GetInventoryItemLookup(StardewValley.Object.autoLoadFrom ?? who.Items) : null;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(PlaceInMachine_Prefix)}:\n{ex}", LogLevel.Error);
                __state = null;
            }
        }

        public static void PlaceInMachine_Postfix(Farmer who, Dictionary<string, Item>? __state, StardewValley.Object __instance)
        {
            try
            {
                if (__state is null) return;

                Dictionary<string, Item> afterConsume = InventoryUtils.GetInventoryItemLookup(StardewValley.Object.autoLoadFrom ?? who.Items);
                List<Item> spentItems = InventoryUtils.InventoryUsedItems(__state, afterConsume);

                if (__instance.heldObject.Value is null) return;

                __instance.heldObject.Value.modData[Constants.ModDataMadeWith] = "";
                foreach (Item item in spentItems)
                {
                    if (item == null) continue;

                    // what allergens does it have?
                    ISet<string> allergens = AllergenManager.GetAllergensInObject(item);
                    foreach (string allergen in allergens)
                    {
                        AllergenManager.ModDataSetAdd(__instance.heldObject.Value, Constants.ModDataMadeWith, allergen);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(PlaceInMachine_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
