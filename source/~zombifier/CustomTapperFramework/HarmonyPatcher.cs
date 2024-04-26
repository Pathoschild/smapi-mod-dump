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
using StardewValley.Internal;
using StardewValley.TerrainFeatures;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CustomTapperFramework;

using SObject = StardewValley.Object;

public class HarmonyPatcher {
  public static void ApplyPatches(Harmony harmony) {
    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.canBePlacedHere)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_canBePlacedHere_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FruitTree),
          nameof(FruitTree.performToolAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.FruitTree_performToolAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(Tree),
          nameof(Tree.performToolAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.Tree_performToolAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(GiantCrop),
          nameof(GiantCrop.performToolAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.GiantCrop_performToolAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.checkForAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_checkForAction_Prefix)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_checkForAction_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.draw),
          new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_draw_Prefix)));
  }

	static void SObject_canBePlacedHere_Postfix(SObject __instance, ref bool __result, GameLocation l, Vector2 tile, CollisionMask collisionMask = CollisionMask.All, bool showError = false) {
    if (!__instance.IsTapper()) return;
    bool disallowBaseTapperRules = false;
    if (Utils.GetFeatureAt(l, tile, out var feature, out var centerPos)) {
        if (!l.objects.ContainsKey(centerPos) &&
            Utils.GetOutputRules(__instance, feature, out disallowBaseTapperRules) != null) {
          __result = true;
        }
        else if (disallowBaseTapperRules) {
          __result = false;
        }
    }
  }

  // If a tapper is present, only shake and remove the tapper instead of damaging the tree.
  static bool FruitTree_performToolAction_Prefix(FruitTree __instance, ref bool __result, Tool t, int explosion, Vector2 tileLocation) {
    if (__instance.Location.objects.TryGetValue(tileLocation, out SObject obj) &&
        obj.IsTapper()) {
      __instance.shake(tileLocation, false);
      return false;
    }
    return true;
  }

  // If a tapper is present, only shake and remove the tapper instead of damaging the tree.
  static bool Tree_performToolAction_Prefix(Tree __instance, ref bool __result, Tool t, int explosion, Vector2 tileLocation) {
    if (__instance.Location.objects.TryGetValue(tileLocation, out SObject obj) &&
        obj.IsTapper()) {
      __instance.shake(tileLocation, false);
      return false;
    }
    return true;
  }

  // If a tapper is present, only shake and remove the tapper instead of damaging the tree.
  static bool GiantCrop_performToolAction_Prefix(GiantCrop __instance, ref bool __result, Tool t, int damage, Vector2 tileLocation) {
    Vector2 centerPos = __instance.Tile;
        centerPos.X = (int)centerPos.X + (int)__instance.width.Value / 2;
        centerPos.Y = (int)centerPos.Y + (int)__instance.height.Value - 1;
    if (__instance.Location.objects.TryGetValue(centerPos, out SObject obj) &&
        obj.IsTapper() && t.isHeavyHitter()) {
      // Has tapper, try to dislodge it
      // For some reason performToolAction on the object directly doesn't work
      obj.playNearbySoundAll("hammer");
      obj.performRemoveAction();
      __instance.Location.objects.Remove(centerPos);
      Game1.createItemDebris(obj, centerPos * 64f, -1);
      // Shake the crop
      __instance.shakeTimer = 100f;
      __instance.NeedsUpdate = true;
      return false;
    }
    return true;
  }

  // Save the currently held item so the PreviousItemId rule can work, and regenerate the output if that is enabled
  static void SObject_checkForAction_Prefix(SObject __instance, out Item __state, Farmer who, bool justCheckingForActivity) {
    __state = null;
    if (!__instance.IsTapper() || justCheckingForActivity || !__instance.readyForHarvest.Value) return;
    __state = __instance.heldObject.Value;
    var rules = Utils.GetOutputRulesForPlacedTapper(__instance, out var unused, __instance.lastOutputRuleId.Value);
    if (rules != null && rules.Count > 0 && rules[0].RecalculateOnCollect) {
      Item newItem = ItemQueryResolver.TryResolveRandomItem(rules[0], new ItemQueryContext(__instance.Location, who, null),
          avoidRepeat: false, null, (string id) =>
          id.Replace("DROP_IN_ID", /*inputItem?.QualifiedItemId ??*/ "0")
          .Replace("NEARBY_FLOWER_ID", MachineDataUtility.GetNearbyFlowerItemId(__instance) ?? "-1"));
      if (newItem is SObject newObject)
      __instance.heldObject.Value = newObject;
    }
  }

  // Update the tapper product after collection
  static void SObject_checkForAction_Postfix(SObject __instance, Item __state, bool __result, Farmer who, bool justCheckingForActivity) {
    if (__state == null || !__result) return;
    Utils.UpdateTapperProduct(__instance);
  }

  // Patch the draw code so the tapper draws on top of the fruit tree. Ugh...
  static void SObject_draw_Prefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f) {
    if (__instance.IsTapper() && __instance.Location != null &&
        Utils.GetFeatureAt(__instance.Location, __instance.TileLocation, out var feature, out var unused) &&
        feature is FruitTree) {
			float layer = (float)((y + 1) * 64) / 10000f + __instance.TileLocation.X / 50000f;
	  	layer += 1e-06f;
      __instance.draw(spriteBatch, x*64, (y-1)*64, layer, alpha);
    }
  }
}
