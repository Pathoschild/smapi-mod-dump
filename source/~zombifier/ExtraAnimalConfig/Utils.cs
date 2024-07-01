/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using HarmonyLib;
using System;
using System.Linq;
using xTile.Dimensions;

using SObject = StardewValley.Object;

namespace Selph.StardewMods.ExtraAnimalConfig;

sealed class SiloUtils {
  static string SiloCapacityKeyPrefix = $"{ModEntry.UniqueId}.SiloCapacity.";
  static string FeedCountKeyPrefix = $"{ModEntry.UniqueId}.FeedCount.";

  public static int GetFeedCapacityFor(GameLocation location, string itemId) {
    int totalCapacity = 0;
    foreach (Building building in location.buildings) {
      if ((building.GetData()?.CustomFields?.TryGetValue(SiloCapacityKeyPrefix + itemId, out var capacityStr) ?? false) &&
          int.TryParse(capacityStr, out var capacity)) {
        totalCapacity += capacity;
      }
    }
    return totalCapacity;
  }

  public static int GetFeedCountFor(GameLocation location, string itemId) {
    if (location.modData.TryGetValue(FeedCountKeyPrefix + itemId, out var countStr) &&
        int.TryParse(countStr, out var count) &&
        count > 0) {
      return count;
    }
    return 0;
  }

  // Every function assumes itemId is qualified and valid object ID
  public static SObject GetFeedFromAnySilo(string itemId, int itemCount = 1) {
    SObject feedObj = null;
    Utility.ForEachLocation((GameLocation location) => {
        var totalCount = GetFeedCountFor(location, itemId);
        var count = Math.Min(totalCount, itemCount);
        if (count > 0) {
          totalCount -= count;
          location.modData[FeedCountKeyPrefix + itemId] = totalCount.ToString();
          feedObj = ItemRegistry.Create<SObject>(itemId, count);
          return true;
        }
        return false;
    });
    return feedObj;
  }

  // Saves the feed to the current location.
  // Returns the number of feed that can't be stored
  public static int StoreFeedInAnySilo(string itemId, int count) {
    Utility.ForEachLocation((GameLocation location) => {
      var currentCount = GetFeedCountFor(location, itemId);
      var newCount = Math.Min(currentCount + count, GetFeedCapacityFor(location, itemId));
      location.modData[FeedCountKeyPrefix + itemId] = newCount.ToString();
      count -= newCount - currentCount;
      if (count <= 0) {
        return true;
      }
      return false;
    });
    return count;
  }
}
