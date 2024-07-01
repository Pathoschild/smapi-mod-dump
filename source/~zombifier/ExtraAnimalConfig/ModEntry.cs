/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using System;
using StardewValley;
using Microsoft.Xna.Framework;

namespace Selph.StardewMods.ExtraAnimalConfig;

internal sealed class ModEntry : Mod {
  internal new static IModHelper Helper { get;
    set;
  }

  internal static IMonitor StaticMonitor { get; set; }
  internal static AnimalExtensionDataAssetHandler animalExtensionDataAssetHandler;
  internal static EggExtensionDataAssetHandler eggExtensionDataAssetHandler;
  internal static string UniqueId;

  public override void Entry(IModHelper helper) {
    Helper = helper;
    StaticMonitor = this.Monitor;
    UniqueId = this.ModManifest.UniqueID;

    animalExtensionDataAssetHandler = new AnimalExtensionDataAssetHandler();
    eggExtensionDataAssetHandler = new EggExtensionDataAssetHandler();

    var harmony = new Harmony(this.ModManifest.UniqueID);

    animalExtensionDataAssetHandler.RegisterEvents(Helper);
    eggExtensionDataAssetHandler.RegisterEvents(Helper);

    AnimalDataPatcher.ApplyPatches(harmony);

    GameLocation.RegisterTileAction($"{UniqueId}.CustomFeedSilo", CustomFeedSilo);
    GameLocation.RegisterTileAction($"{UniqueId}.CustomFeedHopper", CustomFeedHopper);
  }

  private static bool CustomFeedSilo(GameLocation location, string[] args, Farmer player, Point tile) {
    if (!ArgUtility.TryGet(args, 1, out var itemId, out var error)) {
      ModEntry.StaticMonitor.Log(error, LogLevel.Warn);
      return false;
    }
    if (player.ActiveObject?.QualifiedItemId == itemId) {
      int remainingCount = SiloUtils.StoreFeedInAnySilo(itemId, player.ActiveObject.Stack);
      if (remainingCount < player.ActiveObject.Stack) {
        Game1.playSound("Ship");
        DelayedAction.playSoundAfterDelay("grassyStep", 100);
        Game1.drawObjectDialogue(Helper.Translation.Get($"{UniqueId}.AddedToSiloMsg",
              new {
              count = player.ActiveObject.Stack - remainingCount,
              displayName = player.ActiveObject.DisplayName,
              }));
        player.ActiveObject.Stack = remainingCount;
        if (player.ActiveObject.Stack <= 0) {
          player.removeItemFromInventory(player.ActiveObject);
        }
      }
    }
    else {
      Game1.drawObjectDialogue(Helper.Translation.Get($"{UniqueId}.SiloCountMsg",
            new {
            displayName = ItemRegistry.GetDataOrErrorItem(itemId).DisplayName,
            count = SiloUtils.GetFeedCountFor(location, itemId),
            maxCount = SiloUtils.GetFeedCapacityFor(location, itemId),
            }));
    }
    return true;
  }

  private static bool CustomFeedHopper(GameLocation location, string[] args, Farmer player, Point tile) {
    if (!ArgUtility.TryGet(args, 1, out var itemId, out var error)) {
      ModEntry.StaticMonitor.Log(error, LogLevel.Warn);
      return false;
    }
    if (player.ActiveObject?.QualifiedItemId == itemId) {
      int remainingCount = SiloUtils.StoreFeedInAnySilo(itemId, player.ActiveObject.Stack);
      if (remainingCount < player.ActiveObject.Stack) {
        Game1.playSound("Ship");
        DelayedAction.playSoundAfterDelay("grassyStep", 100);
        Game1.drawObjectDialogue(Helper.Translation.Get($"{UniqueId}.AddedToSiloMsg",
              new {
              count = player.ActiveObject.Stack - remainingCount,
              displayName = player.ActiveObject.DisplayName,
              }));
        player.ActiveObject.Stack = remainingCount;
        if (player.ActiveObject.Stack <= 0) {
          player.removeItemFromInventory(player.ActiveObject);
        }
      }
    } else if (player.ActiveObject == null) {
      if (location is AnimalHouse animalHouse) {
        player.addItemToInventory(SiloUtils.GetFeedFromAnySilo(itemId, animalHouse.animalsThatLiveHere.Count));
        Game1.playSound("shwip");
      }
    }
    return true;
  }
}
