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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CustomTapperFramework;

using SObject = StardewValley.Object;

public class HarmonyPatcher {
  public static void ApplyPatches(Harmony harmony) {
    // Patch object interactions
    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.canBePlacedHere)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_canBePlacedHere_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.placementAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_placementAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.checkForAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_checkForAction_Prefix)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_checkForAction_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.performRemoveAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_performRemoveAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.actionOnPlayerEntry)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_actionOnPlayerEntry_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.updateWhenCurrentLocation)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_updateWhenCurrentLocation_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.draw),
          new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_draw_Prefix)),
        transpiler: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.SObject_draw_Transpiler)));

    // Water planter patches

    harmony.Patch(
        original: AccessTools.Method(typeof(IndoorPot),
          nameof(IndoorPot.checkForAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.IndoorPot_checkForAction_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(IndoorPot),
          nameof(IndoorPot.draw),
          new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)}),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.IndoorPot_draw_Prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(IndoorPot),
          nameof(IndoorPot.performObjectDropInAction)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.IndoorPot_performObjectDropInAction_Prefix)));

    // Patch tool actions
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
        original: AccessTools.Method(typeof(Tree),
          nameof(Tree.UpdateTapperProduct)),
        prefix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.Tree_UpdateTapperProduct_Prefix)));

    // Misc patches for water pot logic
    harmony.Patch(
        original: AccessTools.Method(typeof(GameLocation),
          nameof(GameLocation.doesTileSinkDebris)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.GameLocation_doesTileSinkDebris_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(HoeDirt),
          nameof(HoeDirt.canPlantThisSeedHere)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.HoeDirt_canPlantThisSeedHere_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(HoeDirt),
          nameof(HoeDirt.paddyWaterCheck)),
        postfix: new HarmonyMethod(typeof(HarmonyPatcher),
          nameof(HarmonyPatcher.HoeDirt_paddyWaterCheck_Postfix)));
  }

	static void SObject_canBePlacedHere_Postfix(SObject __instance, ref bool __result, GameLocation l, Vector2 tile, CollisionMask collisionMask = CollisionMask.All, bool showError = false) {
    // Check crab pots
    if (Utils.IsCrabPot(__instance)) {
			__result = CrabPot.IsValidCrabPotLocationTile(l, (int)tile.X, (int)tile.Y);
      return;
    }

    // Disallow bushes for now
    if (__instance.IsTeaSapling() && l.objects.TryGetValue(tile, out var pot) &&
        WaterIndoorPotUtils.isWaterPlanter(pot)) {
      __result = false;
      return;
    }

    // Check tappers
    if (!__instance.IsTapper()) return;
    __result = Utils.IsModdedTapperPlaceableAt(__instance, l, tile, out bool isVanillaTapper, out var unnused, out var unused2);
    if (isVanillaTapper) {
      __result = true;
    }
  }

  static bool SObject_placementAction_Prefix(SObject __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null) {
    Vector2 vector = new Vector2(x / 64, y / 64);
    //if (__instance.IsTapper() &&
    //    Utils.GetFeatureAt(location, vector, out var feature, out var centerPos) &&
    //    !location.objects.ContainsKey(centerPos) &&
    //    Utils.GetOutputRules(__instance, feature, TileFeature.REGULAR, out bool unused) is var outputRules &&
    //    outputRules != null) {
    //  // Place tapper if able
    //  SObject @object = (SObject)__instance.getOne();
    //  @object.heldObject.Value = null;
    //  @object.Location = location;
    //  @object.TileLocation = centerPos;
    //  location.objects.Add(centerPos, @object);
    //  Utils.UpdateTapperProduct(@object);
    //  location.playSound("axe");
    //  Utils.Shake(feature, centerPos);
    //  __result = true;
    //  return false;
    //}
    if (Utils.IsCrabPot(__instance) &&
        CrabPot.IsValidCrabPotLocationTile(location,
          (int)vector.X, (int)vector.Y)) {
      if (__instance.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId) {
        IndoorPot @object = new IndoorPot(vector);
        WaterIndoorPotUtils.transformIndoorPotToItem(@object, WaterIndoorPotUtils.WaterPlanterItemId);
        @object.hoeDirt.Value.state.Value = 1;
        @object.hoeDirt.Value.modData[WaterIndoorPotUtils.HoeDirtIsWaterModDataKey] = "true";
        @object.hoeDirt.Value.modData[WaterIndoorPotUtils.HoeDirtIsWaterPlanterModDataKey] = "true";
        __result = CustomCrabPotUtils.placementAction(@object, location, x, y, who);
      } else {
        SObject @object = (SObject)__instance.getOne();
        __result = CustomCrabPotUtils.placementAction(@object, location, x, y, who);
        @object.performDropDownAction(who);
      }
      return false;
    }
    if (__instance.QualifiedItemId == WaterIndoorPotUtils.WaterPotQualifiedItemId) {
        IndoorPot @object = new IndoorPot(vector);
        WaterIndoorPotUtils.transformIndoorPotToItem(@object, WaterIndoorPotUtils.WaterPotItemId);
        @object.hoeDirt.Value.state.Value = 1;
        @object.hoeDirt.Value.modData[WaterIndoorPotUtils.HoeDirtIsWaterModDataKey] = "true";
        location.objects.Add(vector, @object);
  			location.playSound("woodyStep");
        __result = true;
        return false;
    }
    return true;
  }

  static void SObject_performRemoveAction_Prefix(SObject __instance) {
    if (Utils.IsCrabPot(__instance)) {
      CustomCrabPotUtils.performRemoveAction(__instance.Location, __instance.TileLocation);
    }
  }

  static void SObject_actionOnPlayerEntry_Prefix(SObject __instance) {
    if (Utils.IsCrabPot(__instance)) {
      CustomCrabPotUtils.actionOnPlayerEntry(__instance.Location, __instance.TileLocation);
    }
  }

  static void SObject_updateWhenCurrentLocation_Prefix(SObject __instance, GameTime time) {
    if (Utils.IsCrabPot(__instance)) {
      CustomCrabPotUtils.updateWhenCurrentLocation(__instance, time);
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
        obj.IsTapper() && !__instance.tapped.Value) {
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
        obj.IsTapper() && t.isHeavyHitter() && !(t is MeleeWeapon)) {
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

  // For tappers: Save the currently held item so the PreviousItemId rule can work, and regenerate the output if that is enabled
  static bool SObject_checkForAction_Prefix(SObject __instance, out Item __state, ref bool __result, Farmer who, bool justCheckingForActivity) {
    __state = null;
    // Crab pot code
    if (Utils.IsCrabPot(__instance)) {
      if (CustomCrabPotUtils.checkForAction(__instance, who, justCheckingForActivity)) {
        __result = true;
        return false;
      }
      CustomCrabPotUtils.resetRemovalTimer(__instance);
    }
    // Common code
    if (!__instance.IsTapper() || justCheckingForActivity || !__instance.readyForHarvest.Value) return true;
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
    return true;
  }

  // Update the tapper product after collection
  static void SObject_checkForAction_Postfix(SObject __instance, Item __state, bool __result, Farmer who, bool justCheckingForActivity) {
    if (__state == null || !__result) return;
    Utils.UpdateTapperProduct(__instance);
  }

  static bool SObject_draw_Prefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f) {
    // Crab pot draw code
    if (Utils.IsCrabPot(__instance) && __instance.Location != null) {
      CustomCrabPotUtils.draw(__instance, spriteBatch, x, y, alpha);
      return false;
    }
    return true;
  }

  // Patch the draw code to push the tapper draw layer up a tiny amount. ugh...
  public static IEnumerable<CodeInstruction> SObject_draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
    var codes = new List<CodeInstruction>(instructions);
    bool afterIsTapperCall = false;
    for (var i = 0; i < codes.Count; i++) {
      if (codes[i].opcode == OpCodes.Callvirt &&
          codes[i].operand is MethodInfo method &&
          method == AccessTools.Method(typeof(SObject), nameof(SObject.IsTapper))) {
        afterIsTapperCall = true;
      }
      if (afterIsTapperCall &&
          codes[i].opcode == OpCodes.Call &&
          codes[i].operand is MethodInfo method2 &&
          method2 == AccessTools.Method(typeof(Math),
            nameof(Math.Max),
            new Type[] { typeof(float), typeof(float) })) {
        afterIsTapperCall = false;
        // 0.001f seems to work...
        // TODO: calc this better
        yield return new CodeInstruction(OpCodes.Ldc_R4, 0.001f);
        yield return new CodeInstruction(OpCodes.Add);
      }
      yield return codes[i];
    }
  }

  static bool IndoorPot_checkForAction_Prefix(IndoorPot __instance, ref bool __result, Farmer who, bool justCheckingForActivity) {
    if (__instance.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId &&
        CustomCrabPotUtils.checkForAction(__instance, who, justCheckingForActivity)) {
      __result = true;
      return false;
    }
    return true;
  }

  static bool IndoorPot_draw_Prefix(IndoorPot __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f) {
    if (__instance.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId &&
        __instance.Location != null) {
      WaterIndoorPotUtils.draw(__instance, spriteBatch, x, y, alpha);
      return false;
    }
    return true;
  }

  // Disallow tea bushes in water planters
	static bool IndoorPot_performObjectDropInAction_Prefix(IndoorPot __instance, ref bool __result, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed = false) {
    if (!probe &&
        (__instance.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId ||
         __instance.QualifiedItemId == WaterIndoorPotUtils.WaterPotQualifiedItemId) &&
        dropInItem.QualifiedItemId == "(O)251") {
      __result = false;
      return false;
    }
    return true;
  }

  static bool Tree_UpdateTapperProduct_Prefix(Tree __instance, SObject tapper, SObject previousOutput, bool onlyPerformRemovals) {
    // Context tag based
    if (Utils.IsCustomTreeTappers(tapper)) {
      return false;
    }

    // Legacy
    var rules = Utils.GetOutputRules(tapper, __instance, out var disallowBaseTapperRules);
    if (rules != null || disallowBaseTapperRules) {
      return false;
    }
    return true;
  }

  // Don't sink debris if there's a building at that tile or in the adjacent tiles
	static void GameLocation_doesTileSinkDebris_Postfix(GameLocation __instance, ref bool __result, int xTile, int yTile, Debris.DebrisType type) {
    if (__instance.objects.ContainsKey(new Vector2(xTile, yTile)) ||
        __instance.objects.ContainsKey(new Vector2(xTile+1, yTile)) ||
        __instance.objects.ContainsKey(new Vector2(xTile, yTile+1)) ||
        __instance.objects.ContainsKey(new Vector2(xTile-1, yTile)) ||
        __instance.objects.ContainsKey(new Vector2(xTile, yTile-1)) ||
        // diagonal
        __instance.objects.ContainsKey(new Vector2(xTile+1, yTile+1)) ||
        __instance.objects.ContainsKey(new Vector2(xTile+1, yTile-1)) ||
        __instance.objects.ContainsKey(new Vector2(xTile-1, yTile+1)) ||
        __instance.objects.ContainsKey(new Vector2(xTile-1, yTile-1))
              ) {
      __result = false;
    }
  }

	static void HoeDirt_canPlantThisSeedHere_Postfix(HoeDirt __instance, ref bool __result, string itemId, bool isFertilizer = false) {
    if (!__result || isFertilizer) return;
    WaterIndoorPotUtils.canPlant(__instance, itemId, ref __result);
  }

  // Make paddy crops inside water planters considered to be near water.
	static void HoeDirt_paddyWaterCheck_Postfix(HoeDirt __instance, ref bool __result, bool forceUpdate = false) {
    if (__result ||
        !__instance.modData.ContainsKey(WaterIndoorPotUtils.HoeDirtIsWaterPlanterModDataKey) ||
        !__instance.hasPaddyCrop()) return;
    __instance.nearWaterForPaddy.Value = 1;
    __result = true;
  }
}
