/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Extensions;


namespace Selph.StardewMods.MachineTerrainFramework;

using SObject = StardewValley.Object;

public static class Utils {
  public static bool GetFeatureAt(GameLocation location, Vector2 pos, out TerrainFeature feature, out Vector2 centerPos) {
    centerPos = pos;
    if (location.terrainFeatures.TryGetValue(pos, out feature) &&
        (feature is Tree || feature is FruitTree)) {
      centerPos = pos;
      return true;
    }
    foreach (var resourceClump in location.resourceClumps) {
      if (resourceClump.occupiesTile((int)pos.X, (int)pos.Y)) {
        centerPos = GetTapperLocationForClump(resourceClump);
        feature = resourceClump;
        return true;
      }
    }
    return false;
  }

  public static IList<ExtendedTapItemData> GetOutputRulesForPlacedTapper(SObject tapper, out TerrainFeature feature, string ruleId = null) {
    if (GetFeatureAt(tapper.Location, tapper.TileLocation, out feature, out var centerPos)) {
      return GetOutputRules(tapper, feature, out var _unused, ruleId);
    }
    return null;
  }

  // Legacy Tapper API:
  // Get the modded output rules for this tapper.
  // NOTE: If this function returns null, its consumers should not update the tapper.
  // If a list, then touch it.
  public static IList<ExtendedTapItemData> GetOutputRules(SObject tapper, TerrainFeature feature, out bool disallowBaseTapperRules, string ruleId = null) {
    disallowBaseTapperRules = false;
    if (ModEntry.assetHandler.data.TryGetValue(tapper.QualifiedItemId, out var data)) {
      disallowBaseTapperRules = !data.AlsoUseBaseGameRules;
      IList<ExtendedTapItemData> outputRules = feature switch {
        Tree tree => tree.growthStage.Value >= 5 && !tree.stump.Value ?
          data.TreeOutputRules : null,
        FruitTree fruitTree => fruitTree.growthStage.Value >= 4 && !fruitTree.stump.Value ?
          data.FruitTreeOutputRules : null,
        GiantCrop giantCrop => data.GiantCropOutputRules,
          _ => null,
      };
      if (outputRules == null) return null;
      string sourceId = feature switch {
        Tree tree => tree.treeType.Value,
        FruitTree fruitTree => fruitTree.treeId.Value,
        GiantCrop giantCrop => giantCrop.Id,
          _ => null,
      };
      IEnumerable<ExtendedTapItemData> filteredOutputRules;
      if (ruleId != null) {
        filteredOutputRules = from outputRule in outputRules
          where outputRule.Id == ruleId
          select outputRule;
      } else {
        filteredOutputRules = from outputRule in outputRules
        where outputRule.SourceId == null || outputRule.SourceId == sourceId
        select outputRule;
      }
      return filteredOutputRules.ToList();
    }
    return null;
  }

  // Copied from base game's function
  // Update this tapper with produce from the base game, if any.
  public static void UpdateTapperProduct(SObject tapper) {
    if (tapper == null) {
      return;
    }
    var data = GetOutputRulesForPlacedTapper(tapper, out var feature);
    var farmer = feature switch {
      Tree tree => Game1.getFarmer(tree.lastPlayerToHit.Value),
      FruitTree fruitTree => Game1.getFarmer(fruitTree.lastPlayerToHit.Value),
      _ => null,
    };
    if (data != null) {
      float timeMultiplier = 1f;
      foreach (string contextTag in tapper.GetContextTags()) {
        if (contextTag.StartsWith("tapper_multiplier_") && float.TryParse(contextTag.Substring("tapper_multiplier_".Length), out var result)) {
          timeMultiplier = 1f / result;
          break;
        }
      }
      string previousItemId = ((tapper.lastInputItem?.Value?.QualifiedItemId != null) ? ItemRegistry.QualifyItemId(tapper.lastInputItem.Value.QualifiedItemId) : null);
      foreach (ExtendedTapItemData tapItem in data) {

        Random a = new Random();
        // Check if chance applies
        if (!Game1.random.NextBool(tapItem.Chance)) {
          continue;
        }

        // Allow game state and item queries to use an input item.
        // For trees, this will be their seeds.
        // For fruit trees and giant crops, this will be their first produce defined in the list.
        Item inputItem = null;
        switch (feature) {
          case Tree tree:
            inputItem = ItemRegistry.Create(tree.GetData().SeedItemId);
            break;
          case FruitTree fruitTree:
            if (fruitTree.GetData().Fruit.Count == 0) {
              break;
            }
            inputItem = ItemQueryResolver.TryResolveRandomItem(fruitTree.GetData().Fruit[0], new ItemQueryContext(tapper.Location, farmer, null));
            break;
          case GiantCrop giantCrop:
            if (giantCrop.GetData().HarvestItems.Count == 0) {
              break;
            }
            inputItem = ItemQueryResolver.TryResolveRandomItem(giantCrop.GetData().HarvestItems[0], new ItemQueryContext(tapper.Location, farmer, null));
            break;
          default:
            break;
        }

        if (!GameStateQuery.CheckConditions(tapItem.Condition, tapper.Location, farmer, targetItem: null, inputItem: inputItem)) {
          continue;
        }

        // Check if previousItemId matches
        if (tapItem.PreviousItemId != null) {
          bool flag = false;
          foreach (string item2 in tapItem.PreviousItemId) {
            flag = (string.IsNullOrEmpty(item2) ? (previousItemId == null) : string.Equals(previousItemId, ItemRegistry.QualifyItemId(item2), StringComparison.OrdinalIgnoreCase));
            if (flag) {
              break;
            }
          }
          if (!flag) {
            continue;
          }
        }

        // Check if product is in season
        if (tapItem.Season.HasValue && !tapper.Location.SeedsIgnoreSeasonsHere() && tapItem.Season != tapper.Location.GetSeason()) {
          continue;
        }

        Item item = ItemQueryResolver.TryResolveRandomItem(tapItem, new ItemQueryContext(tapper.Location, farmer, null),
            avoidRepeat: false, null, (string id) =>
            id.Replace("DROP_IN_ID", inputItem?.QualifiedItemId ?? "0")
            .Replace("NEARBY_FLOWER_ID", MachineDataUtility.GetNearbyFlowerItemId(tapper) ?? "-1"));
        if (item != null && item is SObject @object) {
          int num = (int)Utility.ApplyQuantityModifiers(tapItem.DaysUntilReady, tapItem.DaysUntilReadyModifiers, tapItem.DaysUntilReadyModifierMode, tapper.Location, Game1.player);
          var output = @object;
          var minutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1.0, Math.Floor((float)num * timeMultiplier)));
          tapper.heldObject.Value = output;
          tapper.MinutesUntilReady = minutesUntilReady;
          tapper.lastOutputRuleId.Value = tapItem.Id;
          tapper.showNextIndex.Value = false;
          break;
        }
      }
    }
  }

  public static void Shake(TerrainFeature feature, Vector2 tile) {
    if (feature is Tree tree) {
      tree.shake(tile, false);
    }
    if (feature is FruitTree fruitTree) {
      fruitTree.shake(tile, false);
    }
    if (feature is GiantCrop giantCrop) {
      // Shake the crop
      giantCrop.shakeTimer = 100f;
      giantCrop.NeedsUpdate = true;
    }
  }

  public static Vector2 GetTapperLocationForClump(ResourceClump resourceClump) {
    var centerPos = resourceClump.Tile;
    centerPos.X = (int)centerPos.X + (int)resourceClump.width.Value / 2;
    centerPos.Y = (int)centerPos.Y + (int)resourceClump.height.Value - 1;
    return centerPos;
  }

  public static bool IsCrabPot(Item item) {
    return item.HasContextTag("custom_crab_pot_item");
  }

  public static bool IsCustomTreeTappers(Item item) {
    return item.HasContextTag("custom_wild_tree_tapper_item");
  }

  public static bool DisallowWildTreePlacement(Item item) {
    return item.HasContextTag("disallow_wild_tree_placement");
  }

  public static bool IsFruitTreeTapper(Item item) {
    return item.HasContextTag("custom_fruit_tree_tapper_item");
  }

  public static bool IsGiantCropTapper(Item item) {
    return item.HasContextTag("custom_giant_crop_tapper_item");
  }

  // Return true if this item is a modded tapper and it is placeable on the terrain feature in question.
  // If isVanillaTapper is true, then it is a vanilla tapper
  public static bool IsModdedTapperPlaceableAt(SObject obj, GameLocation location, Vector2 tileLocation, out bool isVanillaTapper, out TerrainFeature feature, out Vector2 centerPos) {
    isVanillaTapper = true;
    if (!Utils.GetFeatureAt(location, tileLocation, out feature, out centerPos) || location.objects.ContainsKey(centerPos)) {
      return false;
    }

    // Context tags based
    switch (feature) {
      case Tree:
        if (DisallowWildTreePlacement(obj)) return false;
        if (IsCustomTreeTappers(obj)) {
          isVanillaTapper = false;
          return true;
        }
        break;
      case FruitTree:
        if (IsFruitTreeTapper(obj)) return true;
        break;
      case GiantCrop:
        if (IsGiantCropTapper(obj)) return true;
        break;
    }

    // Legacy API
    if (Utils.GetOutputRules(obj, feature, out bool disallowBaseTapperRules) is var outputRules &&
        outputRules != null) {
      return true;
    }
    isVanillaTapper = !disallowBaseTapperRules;
    return false;
  } 
}
