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
using StardewValley.TerrainFeatures;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CustomTapperFramework;

using SObject = StardewValley.Object;

public class JunimaticPatcher {
  public static void ApplyPatches(Harmony harmony) {
    var dataBasedMachineType = AccessTools.TypeByName("NermNermNerm.Junimatic.ObjectMachine");

    harmony.Patch(
        original: AccessTools.Method(dataBasedMachineType, "OnOutputCollected"),
        postfix: new HarmonyMethod(typeof(JunimaticPatcher),
          nameof(JunimaticPatcher.DataBasedMachine_OnOutputCollected_Postfix)));
  }

	static void DataBasedMachine_OnOutputCollected_Postfix(object __instance, Item item) {
    try {
      var machine = ModEntry.Helper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
      if (machine.IsTapper()) {
        Utils.UpdateTapperProduct(machine);
      }
    } catch (Exception e) {
      ModEntry.StaticMonitor.Log(e.Message, LogLevel.Error);
    }
  }
}
