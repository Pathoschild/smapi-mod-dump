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
using StardewValley;
using StardewValley.Delegates;
using StardewValley.TerrainFeatures;
using System;

namespace Selph.StardewMods.MachineTerrainFramework;

using Helpers = GameStateQuery.Helpers;

enum TerrainFeatures {
  Tree,
  FruitTree,
  GiantCrop,
  Unknown,
}

public static class MachineTerrainGameStateQueries {
  public static bool TILE_HAS_TERRAIN_FEATURE(string[] query, GameStateQueryContext context) {
    if (!ArgUtility.TryGetEnum<TerrainFeatures>(query, 1, out var featureEnumCondition, out var error)) {
      return Helpers.ErrorResult(query, error);
    }
    if (!ArgUtility.TryGetOptional(query, 2, out var featureIdCondition, out var error2)) {
      return Helpers.ErrorResult(query, error2);
    }
    if (context.CustomFields == null ||
        context.CustomFields.TryGetValue("Tile", out object tileObj) ||
        tileObj is not Vector2 tile) {
      return Helpers.ErrorResult(query, "No tile found - called outside bigcraftable/machine definitions?");
    }
    if (Utils.GetFeatureAt(context.Location, tile, out var feature, out var unused)) {
      var featureEnum = feature switch {
        Tree => TerrainFeatures.Tree,
        FruitTree => TerrainFeatures.FruitTree,
        GiantCrop => TerrainFeatures.GiantCrop,
        _ => TerrainFeatures.Unknown,
      };
      if (featureEnum != featureEnumCondition) {
        return false;
      }
      var featureIdList = string.IsNullOrWhiteSpace(featureIdCondition) ? [] : featureIdCondition.Split(",");
      if (featureIdList.Length > 0) {
        string featureId = feature switch {
          Tree tree => tree.treeType.Value,
          FruitTree fruitTree => fruitTree.treeId.Value,
          GiantCrop giantCrop => giantCrop.Id,
          _ => null,
        };
        if (Array.IndexOf(featureIdList, featureId) == -1) {
          return false;
        }
      }
      return true;
    }
    return false;
  }
}
