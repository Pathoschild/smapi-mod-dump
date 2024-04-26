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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Inventories;
using StardewValley.GameData.Machines;
using StardewValley.GameData.BigCraftables;
using StardewValley.TokenizableStrings;
using HarmonyLib;
using System.Collections.Generic;

namespace ExtraFuelConfig {
  internal record struct PlaceInMachineState {
    public string displayName;
    public string itemId;
    public string preservedParentSheetIndex;
    public int priceIncrease;
    public int nonPrimaryFuelToRemoveCount;
  }

  /// <summary>The mod entry point.</summary>
  internal sealed class ModEntry : Mod {
    internal new static IModHelper Helper { get; set; }
    internal static IMonitor Mmonitor { get; set; }

    private static ModConfig Config;

  //  static readonly string ModId = "selph.ExtraFuelConfig";
  //
  //private static string GetCustomField(Dictionary<string, string> customFields, string itemId,
  //                                     string configKey) {
  //  return customFields.GetValueOrDefault(ModId + "." + itemId + "." + configKey, "");
  //}

  // Append the extra fuel into the item ID
  //private void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
  //  if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines")) {
  //    e.Edit(asset => {
  //      var data = asset.AsDictionary<string, MachineData>().Data;
  //      foreach (var entry in data) {
  //        if (entry.Value.AdditionalConsumedItems != null && entry.Value.CustomFields != null) {
  //          foreach (var fuel in entry.Value.AdditionalConsumedItems) {
  //            this.Monitor.Log("BOOM1: " + GetCustomField(entry.Value.CustomFields, fuel.ItemId, "ExtraAcceptableFuels"));
  //            fuel.ItemId +=
  //                GetCustomField(entry.Value.CustomFields, fuel.ItemId, "ExtraAcceptableFuels");
  //          }
  //        }
  //      }
  //    });
  //  }
  //}

  // Save custom fuel settings for lookup
  //private void OnAssetReady(object sender, AssetReadyEventArgs e) {
  //  if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines")) {
  //    fuelConfigData = new Dictionary<string, IDictionary<string, FuelConfig>>();
  //    foreach (KeyValuePair<string, MachineData> entry in DataLoader.Machines(Game1.content)) {
  //      if (entry.Value.AdditionalConsumedItems != null && entry.Value.CustomFields != null) {
  //        fuelConfigData.Add(entry.Key, new Dictionary<string, FuelConfig>());
  //        var machineConfig = fuelConfigData[entry.Key];
  //        foreach (MachineItemAdditionalConsumedItems fuel in entry.Value.AdditionalConsumedItems) {
  //          if (GetCustomField(entry.Value.CustomFields, fuel.ItemId, "Enable") == "true") {
  //            machineConfig.Add(fuel.ItemId, new FuelConfig {
  //                AddCostDifference = GetCustomField(entry.Value.CustomFields, fuel.ItemId,
  //                    "AddCostDifference") == "true",
  //                });
  //          }
  //        }
  //      }
  //    }
  //  }
  //}

  /*********
   ** Public methods
   *********/
  /// <summary>The mod entry point, called after the mod is first
  /// loaded.</summary> <param name="helper">Provides simplified APIs for writing
  /// mods.</param>
  public override void Entry(IModHelper helper) {
    Helper = helper;
    Mmonitor = this.Monitor;

    Config = helper.ReadConfig<ModConfig>();

    //helper.Events.Content.AssetReady += this.OnAssetReady;
    //helper.Events.Content.AssetRequested += this.OnAssetRequested;

    var harmony = new Harmony(this.ModManifest.UniqueID);

    harmony.Patch(
        original: AccessTools.Method(typeof(StardewValley.Object),
                                     nameof(StardewValley.Object.PlaceInMachine)),
        prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PlaceInMachinePatchPrefix)),
        postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PlaceInMachinePatchPostfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(MachineDataUtility),
                                     nameof(MachineDataUtility.HasAdditionalRequirements)),
        prefix: new HarmonyMethod(typeof(ModEntry),
                                  nameof(ModEntry.HasAdditionalRequirementsPrefix)));
  }

  // Preemptively calculate the consumed fuels, as well as their relevant datas
  // To be passed to the postfix for updating
  private static void PlaceInMachinePatchPrefix(
      StardewValley.Object __instance, out IDictionary<string, PlaceInMachineState> __state,
      ref bool __result, MachineData machineData, Item inputItem, bool probe, Farmer who,
      bool showMessages = true, bool playSounds = true) {
    IInventory inventory = StardewValley.Object.autoLoadFrom ?? who.Items;
    __state = new Dictionary<string, PlaceInMachineState>();
    if (machineData.AdditionalConsumedItems != null) {
      foreach (MachineItemAdditionalConsumedItems additionalConsumedItem in machineData
                   .AdditionalConsumedItems) {
        if (Config.flavoredItems.TryGetValue(additionalConsumedItem.ItemId, out var config)) {
          List<string> itemIds = new List<string>() {additionalConsumedItem.ItemId};
          itemIds.AddRange(config.extraFuel);
          // Preemptively calculate the price increase, since we can't do it in the postfix if the
          // fuels are already removed.
          if (RemoveItemsFromInventory(inventory, itemIds, additionalConsumedItem.RequiredCount,
                                       out var placeInMachineState, true)) {
            int basePrice = ObjectDataDefinition.GetRawPrice(ItemRegistry.GetMetadata(additionalConsumedItem.ItemId).GetParsedData());
            placeInMachineState.priceIncrease -= basePrice * additionalConsumedItem.RequiredCount;
            // If non-standard fuel will be consumed, save the amount so we can remove it in the postfix
            placeInMachineState.nonPrimaryFuelToRemoveCount = Math.Max(0,
                additionalConsumedItem.RequiredCount - inventory.CountId(additionalConsumedItem.ItemId));
            __state.Add(additionalConsumedItem.ItemId, placeInMachineState);
          }
        }
      }
    }
  }

  // Remove items with the specified ID/category.
  // Returns:
  // * The total cost of the removed items
  // * The relevant IDs of the first item to be removed.
  private static bool RemoveItemsFromInventory(IInventory inventory, IList<string> itemIds,
                                               int stack, out PlaceInMachineState state, bool dryRun = false) {
    state = new PlaceInMachineState {
     displayName = null,
     itemId = null,
     preservedParentSheetIndex = null,
     priceIncrease = 0
    };
    foreach (var itemId in itemIds) {
      if (Game1.player.getItemCountInList(inventory, itemId) >= stack) {
        for (int index1 = 0; index1 < inventory.Count; ++index1) {
          if (inventory[index1] != null && inventory[index1] is StardewValley.Object object1 &&
              (StardewValley.CraftingRecipe.ItemMatchesForCrafting(object1, itemId))) {
            state.itemId ??= inventory[index1].QualifiedItemId;
            state.preservedParentSheetIndex ??=
                object1.preservedParentSheetIndex.Value;
            state.displayName ??= inventory[index1].DisplayName;
            if (inventory[index1].Stack > stack) {
              state.priceIncrease += inventory[index1].salePrice(true) * stack;
              if (!dryRun)
                inventory[index1].Stack -= stack;
              return true;
            }
            stack -= inventory[index1].Stack;
            state.priceIncrease += inventory[index1].salePrice(true) * inventory[index1].Stack;
            if (!dryRun)
              inventory[index1] = (Item)null;
          }
          if (stack <= 0)
            return true;
        }
      }
    }
    return false;
  }

  // Does 2 things:
  // Remove category items that weren't removed in the main function
  // Apply the price increase and name change from the prefix
  private static void PlaceInMachinePatchPostfix(StardewValley.Object __instance,
                                                 IDictionary<string, PlaceInMachineState> __state,
                                                 ref bool __result, MachineData machineData,
                                                 Item inputItem, bool probe, Farmer who,
                                                 bool showMessages = true, bool playSounds = true) {
    // If original function didn't run then nothing to do
    if (probe || !__result || __state.Count == 0)
      return;
    IInventory inventory = StardewValley.Object.autoLoadFrom ?? who.Items;
    // Remove category items that weren't removed in the main function
    foreach (var entry in __state) {
      // If non-standard fuel is supposed to be used, remove them
      if (entry.Value.nonPrimaryFuelToRemoveCount > 0) {
        RemoveItemsFromInventory(inventory,
            Config.flavoredItems[entry.Key].extraFuel,
            entry.Value.nonPrimaryFuelToRemoveCount, out var __unused);
      }
      // Apply the price increase and name change from the prefix
      if (__instance.heldObject.Value == null)
        continue;
      if (entry.Value.priceIncrease > 0) {
        __instance.heldObject.Value.Price += entry.Value.priceIncrease;
      }
      __instance.heldObject.Value.name += "_" + entry.Value.itemId + "_" + entry.Value.preservedParentSheetIndex;
      __instance.heldObject.Value.displayNameFormat = __instance.heldObject.Value.DisplayName + Helper.Translation.Get("selph.ExtraFuelConfig.Flavor",
          new {displayName = entry.Value.displayName} );
    }
  }

  // Patch the HasAdditionalRequirements function to allow checking for both the original fuel and any extra specified fuels.
  private static bool HasAdditionalRequirementsPrefix(
      ref bool __result, IInventory inventory,
      IList<MachineItemAdditionalConsumedItems> requirements,
      out MachineItemAdditionalConsumedItems failedRequirement) {
    if (requirements != null && requirements.Count > 0) {
      foreach (MachineItemAdditionalConsumedItems requirement in requirements) {
        List<string> itemIds = new List<string>() {requirement.ItemId};
        if (Config.flavoredItems.TryGetValue(requirement.ItemId, out var config)) {
          itemIds.AddRange(config.extraFuel);
        }
        int count = 0;
        foreach (var itemId in itemIds) {
          count += Game1.player.getItemCountInList(inventory, itemId);
        }
        if (count < requirement.RequiredCount) {
            failedRequirement = requirement;
            // pass to base function just in case.
            return true;
        }
      }
    }
    failedRequirement = null;
    __result = true;
    return false;
  }
}
}
