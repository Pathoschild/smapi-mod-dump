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
using StardewValley.Objects;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CustomTapperFramework;

using SObject = StardewValley.Object;

public class CustomBushPatcher {
  public static void ApplyPatches(Harmony harmony) {
    var CustomBushModPatchesType = AccessTools.TypeByName("StardewMods.CustomBush.Framework.Services.ModPatches");

    harmony.Patch(
        original: AccessTools.Method(CustomBushModPatchesType,
          "IndoorPot_performObjectDropInAction_postfix"),
        prefix: new HarmonyMethod(typeof(CustomBushPatcher),
          nameof(CustomBushPatcher.IndoorPot_performObjectDropInAction_Prefix)));
  }

  // Disallow custom bushes in water planters (for now)
  static bool IndoorPot_performObjectDropInAction_Prefix(IndoorPot __0, ref bool __1, Item dropInItem, bool probe) {
    if (!probe &&
        WaterIndoorPotUtils.isWaterPlanter(__0) &&
        dropInItem is SObject obj &&
        obj.IsTeaSapling()) {
      __1 = false;
      return false;
    }
    return true;
  }
}
