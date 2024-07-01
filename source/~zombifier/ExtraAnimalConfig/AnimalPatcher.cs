/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using Netcode;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Events;
using StardewValley.Tools;
using StardewValley.Internal;
using StardewValley.Menus;
using StardewValley.GameData.Machines;
using StardewValley.GameData.FarmAnimals;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using xTile.Dimensions;

using SObject = StardewValley.Object;

namespace Selph.StardewMods.ExtraAnimalConfig;

// Contains Harmony patches related to animals.
sealed class AnimalDataPatcher {
  static string CustomTroughTileProperty = $"{ModEntry.UniqueId}.CustomTrough";
  static string CachedAnimalIdKey = $"{ModEntry.UniqueId}.CachedAnimalId";

  public static void ApplyPatches(Harmony harmony) {
    // Animal patches
    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.isMale)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_isMale_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.CanGetProduceWithTool)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_CanGetProduceWithTool_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.GetTexturePath),
          new Type[] {typeof(FarmAnimalData)}),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_GetTexturePath_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.TryGetAnimalDataFromEgg)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_TryGetAnimalDataFromEgg_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.minutesElapsed)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.SObject_minutesElapsed_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.dayUpdate)),
        prefix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_dayUpdate_Prefix)),
        transpiler: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_dayUpdate_Transpiler)));

    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.GetHarvestType)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_GetHarvestType_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(AnimalHouse),
          nameof(AnimalHouse.adoptAnimal)),
        prefix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.AnimalHouse_adoptAnimal_Prefix)));

    // Animal house patches for the non-hay food functionality
    harmony.Patch(
        original: AccessTools.Method(typeof(AnimalHouse),
          nameof(AnimalHouse.checkAction)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.AnimalHouse_checkAction_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(AnimalHouse),
          nameof(AnimalHouse.dropObject)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.AnimalHouse_dropObject_Postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(AnimalHouse),
          nameof(AnimalHouse.feedAllAnimals)),
        postfix: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.AnimalHouse_feedAllAnimals_Postfix)));

    // Transpilers to override animal produce
    harmony.Patch(
        original: AccessTools.Method(typeof(FarmAnimal),
          nameof(FarmAnimal.behaviors)),
        transpiler: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.FarmAnimal_behaviors_Transpiler)));

    harmony.Patch(
        original: AccessTools.Method(typeof(SObject),
          nameof(SObject.DayUpdate)),
        transpiler: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.SObject_DayUpdate_Transpiler)));

    harmony.Patch(
        original: AccessTools.Method(typeof(MilkPail),
          nameof(MilkPail.DoFunction)),
        transpiler: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.MilkPail_DoFunction_Transpiler)));

    harmony.Patch(
        original: AccessTools.Method(typeof(Shears),
          nameof(Shears.DoFunction)),
        transpiler: new HarmonyMethod(typeof(AnimalDataPatcher), nameof(AnimalDataPatcher.Shears_DoFunction_Transpiler)));
  }

  static void FarmAnimal_isMale_Postfix(FarmAnimal __instance, ref bool __result) {
    if (ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(__instance.type.Value, out var animalExtensionData) &&
        animalExtensionData.MalePercentage >= 0) {
      __result = __instance.myID.Value % 100 < animalExtensionData.MalePercentage;
    }
  }

  static void FarmAnimal_CanGetProduceWithTool_Postfix(FarmAnimal __instance, ref bool __result, Tool tool) {
    if (__instance.currentProduce.Value != null &&
        ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(__instance.type.Value, out var animalExtensionData) &&
        animalExtensionData.AnimalProduceExtensionData.TryGetValue(ItemRegistry.QualifyItemId(__instance.currentProduce.Value), out var animalProduceExtensionData) &&
        tool != null && tool.BaseName != null && animalProduceExtensionData.HarvestTool != null) {
      // In extremely rare cases (eg debug mode) an animal may spawn with DropOvernight produce in its body.
      // To help get the produce out, always allow them to harvest
      __result = (animalProduceExtensionData.HarvestTool == "DropOvernight") ||
        (animalProduceExtensionData.HarvestTool == tool.BaseName);
    }
  }

	static void FarmAnimal_GetTexturePath_Postfix(FarmAnimal __instance, ref string __result, FarmAnimalData data) {
    if (__instance.currentProduce.Value != null &&
        ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(__instance.type.Value, out var animalExtensionData) &&
        animalExtensionData.AnimalProduceExtensionData.TryGetValue(ItemRegistry.QualifyItemId(__instance.currentProduce.Value), out var animalProduceExtensionData)) {
      if (animalProduceExtensionData.ProduceTexture != null) {
        __result = animalProduceExtensionData.ProduceTexture;
      }
      if (__instance.skinID.Value != null &&
          animalProduceExtensionData.SkinProduceTexture.TryGetValue(__instance.skinID.Value, out var skinTexture)) {
        __result = skinTexture;
      }
    }
  }

  static void FarmAnimal_TryGetAnimalDataFromEgg_Postfix(ref bool __result, Item eggItem, GameLocation location, ref string id, ref FarmAnimalData data) {
    // If there's a result from the incubator being ready return it
    if (eggItem.modData.TryGetValue(CachedAnimalIdKey, out var cachedAnimalId) &&
        Game1.farmAnimalData.TryGetValue(cachedAnimalId, out var animalData)) {
      id = eggItem.modData[CachedAnimalIdKey];
      data = animalData;
      __result = true;
    }
  }

  static void SObject_minutesElapsed_Postfix(SObject __instance, int minutes) {
    if ((__instance.GetMachineData()?.IsIncubator ?? false) &&
        __instance.heldObject.Value != null &&
        __instance.MinutesUntilReady <= 0 &&
        !__instance.modData.ContainsKey(CachedAnimalIdKey) &&
        ModEntry.eggExtensionDataAssetHandler.data.TryGetValue(__instance.heldObject.Value.QualifiedItemId, out var eggExtensionData)) {
      foreach (var animalSpawnData in eggExtensionData.AnimalSpawnList) {
        if (animalSpawnData.Condition != null && !GameStateQuery.CheckConditions(animalSpawnData.Condition, __instance.Location)) {
          continue;
        }
        if (Game1.farmAnimalData.TryGetValue(animalSpawnData.AnimalId, out var animalData2)) {
          __instance.heldObject.Value.modData[CachedAnimalIdKey] = animalSpawnData.AnimalId;
          return;
        }
      }
    }
  }

  // If there are custom non-hay feed for this animal inside the building, feed the animal
	static void FarmAnimal_dayUpdate_Prefix(FarmAnimal __instance, GameLocation environment) {
    if (ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(__instance.type.Value, out var animalExtensionData) &&
        animalExtensionData.FeedItemId != null &&
        environment is AnimalHouse) {
      bool isFed = false;
      bool eatsGrass = (__instance.GetAnimalData()?.GrassEatAmount ?? 0) > 0;
      foreach (var pair in environment.objects.Pairs.ToArray()) {
        if (pair.Value.QualifiedItemId == ItemRegistry.QualifyItemId(animalExtensionData.FeedItemId)) {
          isFed = true;
          __instance.fullness.Value = 255;
          environment.objects.Remove(pair.Key);
          if (!eatsGrass) {
            __instance.happiness.Value = 255;
          }
          break;
        }
      }
      if (!isFed && eatsGrass) {
        __instance.fullness.Value = 0;
      }
    }
  }

  static void FarmAnimal_GetHarvestType_Postfix(FarmAnimal __instance, ref FarmAnimalHarvestType? __result) {
    if (__instance.currentProduce.Value != null &&
        ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(__instance.type.Value, out var animalExtensionData) &&
        animalExtensionData.AnimalProduceExtensionData.TryGetValue(ItemRegistry.QualifyItemId(__instance.currentProduce.Value), out var animalProduceExtensionData)) {
      switch (animalProduceExtensionData.HarvestTool) {
        case "DigUp":
          __result = FarmAnimalHarvestType.DigUp;
          break;
        case "Milk Pail":
        case "Shears":
          __result = FarmAnimalHarvestType.HarvestWithTool;
          break;
        // NOTE: This branch should NEVER happen (the produce should have been dropped last night) but I'm including it anyway just in case
        case "DropOvernight":
          __result = FarmAnimalHarvestType.DropOvernight;
          break;
      }
    }
  }

  // The following 2 patches allow placing feed on custom troughs.
	static void AnimalHouse_checkAction_Postfix(AnimalHouse __instance, ref bool __result, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who) {
		if (who.ActiveObject is not null &&
        who.ActiveObject.QualifiedItemId == __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, CustomTroughTileProperty, "Back") &&
         !__instance.objects.ContainsKey(new Vector2(tileLocation.X, tileLocation.Y))) {
			__instance.objects.Add(new Vector2(tileLocation.X, tileLocation.Y), (SObject)who.ActiveObject.getOne());
			who.reduceActiveItemByOne();
			who.currentLocation.playSound("coin");
			Game1.haltAfterCheck = false;
			__result = true;
		}
  }

  static void AnimalHouse_dropObject_Postfix(AnimalHouse __instance, ref bool __result, SObject obj, Vector2 location, xTile.Dimensions.Rectangle viewport, bool initialPlacement, Farmer who = null) {
    Vector2 key = new Vector2((int)(location.X / 64f), (int)(location.Y / 64f));
    if (obj.QualifiedItemId == __instance.doesTileHaveProperty((int)key.X, (int)key.Y, CustomTroughTileProperty, "Back")) {
      __result = __instance.objects.TryAdd(key, obj);
    }
  }

  static void AnimalHouse_feedAllAnimals_Postfix(AnimalHouse __instance) {
    GameLocation rootLocation = __instance.GetRootLocation();
    int num = 0;
    for (int i = 0; i < __instance.map.Layers[0].LayerWidth; i++) {
      for (int j = 0; j < __instance.map.Layers[0].LayerHeight; j++) {
        var feedId = __instance.doesTileHaveProperty(i, j, CustomTroughTileProperty, "Back");
        if (feedId is null) {
          continue;
        }
        Vector2 key = new Vector2(i, j);
        if (!__instance.objects.ContainsKey(key)) {
          SObject feedObj = SiloUtils.GetFeedFromAnySilo(feedId);
          if (feedObj == null) {
            return;
          }
          __instance.objects.Add(key, feedObj);
          num++;
        }
        if (num >= __instance.animalLimit.Value) {
          return;
        }
      }
    }
  }

  // When a new animal gives birth, maybe change it to a different animal depending on the custom spawn data
  static void AnimalHouse_adoptAnimal_Prefix(AnimalHouse __instance, ref FarmAnimal animal) {
    // NamingMenu should only be active for newly birthed animals... hopefully.
    if (Game1.activeClickableMenu is NamingMenu &&
        (ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(animal.type.Value, out var animalExtensionData) &&
         animalExtensionData.AnimalSpawnList != null)) {
      foreach (var animalSpawnData in animalExtensionData.AnimalSpawnList) {
        if (animalSpawnData.Condition != null && !GameStateQuery.CheckConditions(animalSpawnData.Condition, __instance)) {
          continue;
        }
        if (animalSpawnData.AnimalId == animal.type.Value) {
          continue;
        }
        string name = animal.Name;
        long previousParentId = animal.parentId.Value;
        animal = new FarmAnimal(animalSpawnData.AnimalId, animal.myID.Value, animal.ownerID.Value) {
          Name = name,
          displayName = name,
        };
        animal.parentId.Value = previousParentId;
        return;
      }
    }
  }

  static SObject CreateProduce(string produceId, FarmAnimal animal) {
    if (ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(animal.type.Value, out var animalExtensionData) &&
        animalExtensionData.AnimalProduceExtensionData.TryGetValue(ItemRegistry.QualifyItemId(produceId), out var animalProduceExtensionData) &&
        animalProduceExtensionData.ItemQuery != null) {
      var context = new ItemQueryContext(animal.home?.GetIndoors(), Game1.getFarmer(animal.ownerID.Value), Game1.random);
      var item = ItemQueryResolver.TryResolveRandomItem(animalProduceExtensionData.ItemQuery, context);
      if (item is SObject obj) {
        return obj;
      }
    }
    // Vanilla fallback
    return ItemRegistry.Create<SObject>(produceId);
  }

  static readonly MethodInfo ItemRegistryCreateObjectType = AccessTools
    .GetDeclaredMethods(typeof(ItemRegistry))
    .First(method => method.Name == nameof(ItemRegistry.Create) && method.IsGenericMethod)
    .MakeGenericMethod(typeof(SObject));

  static readonly MethodInfo CreateProduceType = AccessTools.Method(
      typeof(AnimalDataPatcher),
      nameof(AnimalDataPatcher.CreateProduce));


  static IEnumerable<CodeInstruction> FarmAnimal_behaviors_Transpiler(IEnumerable<CodeInstruction> instructions) {
    CodeMatcher matcher = new(instructions);
		// Old: ItemRegistry.Create<Object>(this.currentProduce.Value);
    // New: AnimalDataPatcher.CreateProduce(this.currentProduce.Value, this);
    matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldarg_0),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.currentProduce))),
        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetFieldBase<string, NetString>), nameof(NetFieldBase<string, NetString>.Value))),
        new CodeMatch(OpCodes.Ldc_I4_1),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Call, ItemRegistryCreateObjectType)
        )
      .ThrowIfNotMatch($"Could not find entry point for {nameof(FarmAnimal_behaviors_Transpiler)}")
      .Advance(3)
      .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldarg_0),
          new CodeInstruction(OpCodes.Call, CreateProduceType)
          )
      .RemoveInstructions(4);
    return matcher.InstructionEnumeration();
  }

  static IEnumerable<CodeInstruction> SObject_DayUpdate_Transpiler(IEnumerable<CodeInstruction> instructions) {
    CodeMatcher matcher = new(instructions);
		// Old: ItemRegistry.Create<Object>("(O)" + pair2.Value.currentProduce.Value);
    // New: AnimalDataPatcher.CreateProduce("(O)" + pair2.currentProduce.Value, pair2);
    matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldstr, "(O)"),
        new CodeMatch(OpCodes.Ldloca_S),
        new CodeMatch(OpCodes.Call),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.currentProduce))),
        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetFieldBase<string, NetString>), nameof(NetFieldBase<string, NetString>.Value))),
        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(String), nameof(String.Concat), new Type[] {typeof(string), typeof(string)})),
        new CodeMatch(OpCodes.Ldc_I4_1),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Call, ItemRegistryCreateObjectType)
        )
      .ThrowIfNotMatch($"Could not find entry point for {nameof(SObject_DayUpdate_Transpiler)}")
      .Advance(6)
      .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldloca_S, 14),
          new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(KeyValuePair<long, FarmAnimal>), nameof(KeyValuePair<long, FarmAnimal>.Value))),
          new CodeInstruction(OpCodes.Call, CreateProduceType)
          )
      .RemoveInstructions(4);
    return matcher.InstructionEnumeration();
  }

  static IEnumerable<CodeInstruction> MilkPail_DoFunction_Transpiler(IEnumerable<CodeInstruction> instructions) {
    CodeMatcher matcher = new(instructions);
		// Old: ItemRegistry.Create<Object>("(O)" + this.animal.currentProduce.Value);
    // New: AnimalDataPatcher.CreateProduce("(O)" + this.animal.currentProduce.Value);
    matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldstr, "(O)"),
        new CodeMatch(OpCodes.Ldarg_0),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MilkPail), nameof(MilkPail.animal))),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.currentProduce))),
        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetFieldBase<string, NetString>), nameof(NetFieldBase<string, NetString>.Value))),
        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(String), nameof(String.Concat), new Type[] {typeof(string), typeof(string)})),
        new CodeMatch(OpCodes.Ldc_I4_1),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Call, ItemRegistryCreateObjectType)
        )
      .ThrowIfNotMatch($"Could not find entry point for {nameof(MilkPail_DoFunction_Transpiler)}")
      .Advance(6)
      .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldarg_0),
          new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MilkPail), nameof(MilkPail.animal))),
          new CodeInstruction(OpCodes.Call, CreateProduceType)
          )
      .RemoveInstructions(4);
    return matcher.InstructionEnumeration();
  }

  static IEnumerable<CodeInstruction> Shears_DoFunction_Transpiler(IEnumerable<CodeInstruction> instructions) {
    CodeMatcher matcher = new(instructions);
		// Old: ItemRegistry.Create<Object>("(O)" + this.animal.currentProduce.Value);
    // New: AnimalDataPatcher.CreateProduce("(O)" + this.animal.currentProduce.Value);
    matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldstr, "(O)"),
        new CodeMatch(OpCodes.Ldarg_0),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Shears), nameof(Shears.animal))),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.currentProduce))),
        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetFieldBase<string, NetString>), nameof(NetFieldBase<string, NetString>.Value))),
        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(String), nameof(String.Concat), new Type[] {typeof(string), typeof(string)})),
        new CodeMatch(OpCodes.Ldc_I4_1),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Call, ItemRegistryCreateObjectType)
        )
      .ThrowIfNotMatch($"Could not find entry point for {nameof(Shears_DoFunction_Transpiler)}")
      .Advance(6)
      .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldarg_0),
          new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Shears), nameof(Shears.animal))),
          new CodeInstruction(OpCodes.Call, CreateProduceType)
          )
      .RemoveInstructions(4);
    return matcher.InstructionEnumeration();
  }

  // Returns whether this animal only eats modded food and not hay/grass
  // This must false by default
  static bool AnimalOnlyEatsModdedFood(FarmAnimal animal) {
    return (ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(animal.type.Value, out var animalExtensionData) &&
        animalExtensionData.FeedItemId != null &&
        (animal.GetAnimalData()?.GrassEatAmount ?? 0) <= 0);
  }

  // Returns whether the animal's current produce is hardcoded to drop instead of harvested by tool
  static bool DoNotDropCurrentProduce(FarmAnimal animal, string produceId) {
    if (produceId != null &&
        ModEntry.animalExtensionDataAssetHandler.data.TryGetValue(animal.type.Value, out var animalExtensionData) &&
        animalExtensionData.AnimalProduceExtensionData.TryGetValue(ItemRegistry.QualifyItemId(produceId), out var animalProduceExtensionData) &&
        animalProduceExtensionData.HarvestTool != null) {
      return animalProduceExtensionData.HarvestTool != "DropOvernight";
    }
    return animal.GetHarvestType() != FarmAnimalHarvestType.DropOvernight;
  }

  // This transpiler does 3 things:
  // * Disallow eating hay if not a hay eater
  // * Override the item create call with the override item query
  // * Don't drop produce on the ground if override is specified
  static IEnumerable<CodeInstruction> FarmAnimal_dayUpdate_Transpiler(IEnumerable<CodeInstruction> instructions) {
    CodeMatcher matcher = new(instructions);
      // Old: (int)this.fullness < 200 && environment is AnimalHouse
      // New: ... && !AnimalOnlyEatsModdedFood(this)
      matcher.MatchEndForward(
        new CodeMatch(OpCodes.Ldarg_0),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.fullness))),
        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(NetInt), "op_Implicit")),
        new CodeMatch(OpCodes.Ldc_I4, 200),
        new CodeMatch(OpCodes.Bge_S),
        new CodeMatch(OpCodes.Ldarg_1),
        new CodeMatch(OpCodes.Isinst, typeof(AnimalHouse)),
        new CodeMatch(OpCodes.Brfalse_S)
          )
      .ThrowIfNotMatch($"Could not find entry point for hunger portion of {nameof(FarmAnimal_dayUpdate_Transpiler)}");
      var label = (Label)matcher.Operand;
      matcher.Advance(1)
        .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldarg_0),
          new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AnimalDataPatcher), nameof(AnimalOnlyEatsModdedFood))),
          new CodeInstruction(OpCodes.Brtrue_S, label)
          );

      // Old: animalData.HarvestType != FarmAnimalHarvestType.DropOvernight
      // New: DoNotDropCurrentProduce(animal, text)
      matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldloc_0),
        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FarmAnimalData), nameof(FarmAnimalData.HarvestType))),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Cgt_Un)
          )
        .ThrowIfNotMatch($"Could not find entry point for drop harvest type check portion of {nameof(FarmAnimal_dayUpdate_Transpiler)}");
      var labels = matcher.Labels;
      matcher.RemoveInstructions(4)
        .InsertAndAdvance(
          new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
          new CodeInstruction(OpCodes.Ldloc_S, 7),
          new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AnimalDataPatcher), nameof(DoNotDropCurrentProduce))));


      // Old: ItemRegistry.Create<Object>("(O)" + text);
      // New: AnimalDataPatcher.CreateProduce("(O)" + text, this);
      matcher.MatchStartForward(
        new CodeMatch(OpCodes.Ldstr, "(O)"),
        new CodeMatch(OpCodes.Ldloc_S),
        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(String), nameof(String.Concat), new Type[] {typeof(string), typeof(string)})),
        new CodeMatch(OpCodes.Ldc_I4_1),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Ldc_I4_0),
        new CodeMatch(OpCodes.Call, ItemRegistryCreateObjectType)
      )
      .ThrowIfNotMatch($"Could not find entry point for item create {nameof(FarmAnimal_dayUpdate_Transpiler)}")
      .Advance(3)
      .InsertAndAdvance(
        new CodeInstruction(OpCodes.Ldarg_0),
        new CodeInstruction(OpCodes.Call, CreateProduceType))
      .RemoveInstructions(4);
    return matcher.InstructionEnumeration();
  }
}
