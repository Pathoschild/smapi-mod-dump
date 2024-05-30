/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewModdingAPI.Events;
using HarmonyLib;
using System.Collections.Generic;

namespace ExtraMachineConfig; 

// Contains misc patches related to animals.
// Junimos are animals, right?
class AnimalDataPatcher {
  internal static string MalePercentageKey = "selph.ExtraMachineConfig.MalePercentage";
  internal static string JunimoLovedItemContextTag = "junimo_loved_item";

  public static void ApplyPatches(Harmony harmony) {
    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.isMale)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_isMale_Postfix)));

    //harmony.Patch(
    //    original: AccessTools.Method(typeof(JunimoHut),
    //      nameof(JunimoHut.dayUpdate)),
    //    postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.JunimoHut_dayUpdate_Postfix)));
  }

  static void FarmAnimal_isMale_Postfix(FarmAnimal __instance, ref bool __result) {
    if ((__instance.GetAnimalData()?.CustomFields?.ContainsKey(MalePercentageKey) ?? false)
        && Int32.TryParse(__instance.GetAnimalData()?.CustomFields[MalePercentageKey] ?? String.Empty, out var malePercentage)) {
      __result = __instance.myID.Value % 100 < malePercentage;
    }
  }

  public static void OnDayStartedJunimoHut(object? sender, DayStartedEventArgs e) {
    foreach (var location in Game1.locations) {
      foreach (var building in location.buildings) {
        if (building is JunimoHut hut &&
            hut.raisinDays.Value == 0 &&
            !Game1.IsWinter) {
          Chest outputChest = hut.GetOutputChest();
          if (Utils.getItemCountInListByTags(outputChest.Items, JunimoLovedItemContextTag) > 0) {
            hut.raisinDays.Value += 7;
            Utils.RemoveItemFromInventoryByTags(outputChest.Items, JunimoLovedItemContextTag, 1);
          }
        }
      }
    }
  }


  //static void JunimoHut_dayUpdate_Postfix(JunimoHut __instance, int dayOfMonth) {
  //  if ((int)__instance.raisinDays.Value == 0 && !Game1.IsWinter)
  //  {
  //    Chest outputChest = __instance.GetOutputChest();
  //    if (Utils.getItemCountInListByTags(outputChest.Items, JunimoLovedItemContextTag) > 0)
  //    {
  //      __instance.raisinDays.Value += 7;
  //      Utils.RemoveItemFromInventoryByTags(outputChest.Items, JunimoLovedItemContextTag, 1);
  //    }
  //  }
  //}
}
