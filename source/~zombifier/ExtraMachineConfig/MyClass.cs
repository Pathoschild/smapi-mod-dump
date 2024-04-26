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
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Inventories;
using StardewValley.GameData.Machines;
using StardewValley.GameData.BigCraftables;
using StardewValley.TokenizableStrings;
using HarmonyLib;
using System.Collections.Generic;

namespace ExtraMachineConfig {

  // State to pass to PlaceInMachine postfix
  internal record struct GetOutputDataState {
    public GetOutputDataState() {}
    public IDictionary<string, int> extrafuelToRemove = new Dictionary<string, int>();
  }

  internal sealed class ModEntry : Mod {
    internal new static IModHelper Helper { get;
    set;
  }

  internal static IMonitor Mmonitor { get; set; }

  // Keys for the CustomData map
  internal static Regex RequirementIdKeyRegex =
      new Regex(@"selph.ExtraMachineConfig\.RequirementId\.(\d+)");
  internal static string RequirementCountKeyPrefix = "selph.ExtraMachineConfig.RequirementCount";
  internal static string RequirementInvalidMsgKey = "selph.ExtraMachineConfig.RequirementInvalidMsg";
  internal static string InheritPreserveIdKey = "selph.ExtraMachineConfig.InheritPreserveId";
  internal static string CopyColorKey = "selph.ExtraMachineConfig.CopyColor";

  // Legacy versions, no mod IDs because I'm stupid
  internal static Regex RequirementIdKeyRegex_Legacy =
      new Regex(@"ExtraMachineConfig\.RequirementId\.(\d+)");
  internal static string RequirementCountKeyPrefix_Legacy = "ExtraMachineConfig.RequirementCount";
  internal static string RequirementInvalidMsgKey_Legacy = "ExtraMachineConfig.RequirementInvalidMsg";
  internal static string InheritPreserveIdKey_Legacy = "ExtraMachineConfig.InheritPreserveId";
  internal static string CopyColorKey_Legacy = "ExtraMachineConfig.CopyColor";

  public override void Entry(IModHelper helper) {
    Helper = helper;
    Mmonitor = this.Monitor;

    var harmony = new Harmony(this.ModManifest.UniqueID);

    harmony.Patch(
        original: AccessTools.Method(
            typeof(StardewValley.MachineDataUtility),
            nameof(StardewValley.MachineDataUtility.GetOutputData),
            new Type[] { typeof(List<MachineItemOutput>), typeof(bool), typeof(Item),
                         typeof(Farmer), typeof(GameLocation) }),
        prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.GetOutputDataPatchPrefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(StardewValley.MachineDataUtility),
                                     nameof(StardewValley.MachineDataUtility.GetOutputItem)),
        postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.GetOutputItemPatchPostfix)));
  }

  // Extract the additional fuel data from the output data as a list of fuel IDs to fuel count.
  private static IList<(string, int)> GetExtraRequirements(MachineItemOutput outputData) {
    IList<(string, int)> extraRequirements = new List<(string, int)>();
    if (outputData?.CustomData == null) {
      return extraRequirements;
    }
    foreach (var entry in outputData.CustomData) {
      var match = RequirementIdKeyRegex.Match(entry.Key);
      if (!match.Success) {
        match = RequirementIdKeyRegex_Legacy.Match(entry.Key);
      }
      if (match.Success) {
        string countKey = RequirementCountKeyPrefix + "." + match.Groups[1].Value;
        string countKey_Legacy = RequirementCountKeyPrefix_Legacy + "." + match.Groups[1].Value;
        string countString;
        if ((outputData.CustomData.TryGetValue(countKey, out countString) ||
             outputData.CustomData.TryGetValue(countKey_Legacy, out countString)) &&
            Int32.TryParse(countString, out int count)) {
          extraRequirements.Add((entry.Value, count));
        } else {
          extraRequirements.Add((entry.Value, 1));
        }
      }
    }
    return extraRequirements;
  }

  // Removes items with the specified ID from the inventory.
  // This differs from ReduceId is that itemId can also be category IDs.
  // TODO: Port functionality from ExtraFuelConfig
  private static bool RemoveItemFromInventory(IInventory inventory, string itemId, int count) {
    for (int index = 0; index < inventory.Count; ++index) {
      if (inventory[index] != null &&
          (StardewValley.CraftingRecipe.ItemMatchesForCrafting(inventory[index], itemId))) {
        if (inventory[index].Stack > count) {
          inventory[index].Stack -= count;
          return true;
        }
        count -= inventory[index].Stack;
        inventory[index] = (Item)null;
      }
      if (count <= 0) {
        return true;
      }
    }
    return false;
  }

  // This patch:
  // * Checks for additional fuel requirements specified in the output rule's custom data, and
  // removes rules that cannot be satisfied
  private static void GetOutputDataPatchPrefix(ref List<MachineItemOutput> outputs,
                                               bool useFirstValidOutput, Item inputItem, Farmer who,
                                               GameLocation location) {
    if (outputs == null || outputs.Count < 0) {
      return;
    }
    string invalidMessage = null;
    IInventory inventory = StardewValley.Object.autoLoadFrom ?? who.Items;
    List<MachineItemOutput> newOutputs = new List<MachineItemOutput>();
    foreach (MachineItemOutput output in outputs) {
      if (output.CustomData == null) {
        newOutputs.Add(output);
        continue;
      }
      bool valid = true;
      var extraRequirements = GetExtraRequirements(output);
      foreach (var entry in extraRequirements) {
        if (Game1.player.getItemCountInList(inventory, entry.Item1) < entry.Item2) {
          valid = false;
        }
      }
      if (valid) {
        newOutputs.Add(output);
      } else {
        string msg;
        output.CustomData.TryGetValue(RequirementInvalidMsgKey, out msg);
        output.CustomData.TryGetValue(RequirementInvalidMsgKey_Legacy, out msg);
        invalidMessage ??= msg;
      }
    }
    outputs = newOutputs;
    if (outputs.Count == 0 && invalidMessage != null && who.IsLocalPlayer &&
        StardewValley.Object.autoLoadFrom == null) {
      Game1.showRedMessage(invalidMessage);
    }
  }

  // This patch:
  // * Checks for additional fuel requirements specified in the output rule's custom data, and
  // removes them from inventory
  // * Checks if preserve ID is set to inherit the input item's preserve ID, and applies it
  // * Checks if a colored item should be created and apply the changes
  private static void GetOutputItemPatchPostfix(ref Item __result, StardewValley.Object machine,
                                                MachineItemOutput outputData, Item inputItem,
                                                Farmer who, bool probe,
                                                ref int? overrideMinutesUntilReady) {
    if (__result == null || outputData == null || inputItem == null) {
      return;
    }
    IInventory inventory = StardewValley.Object.autoLoadFrom ?? who.Items;
    // Inherit preserve ID
    if ((outputData.PreserveId == "INHERIT" ||
         (outputData.CustomData != null &&
          (outputData.CustomData.ContainsKey(InheritPreserveIdKey) ||
           outputData.CustomData.ContainsKey(InheritPreserveIdKey_Legacy)))) &&
        inputItem is StardewValley.Object inputObject &&
        inputObject.preservedParentSheetIndex.Value != "-1" &&
        __result is StardewValley.Object resultObject) {
      resultObject.preservedParentSheetIndex.Value = inputObject.preservedParentSheetIndex.Value;
    }
    if (outputData.CustomData == null) {
      return;
    }
    // Remove extra fuel
    var extraRequirements = GetExtraRequirements(outputData);
    foreach (var entry in extraRequirements) {
      RemoveItemFromInventory(inventory, entry.Item1, entry.Item2);
    }
    // Color the item
    if ((outputData.CustomData.ContainsKey(CopyColorKey) ||
        outputData.CustomData.ContainsKey(CopyColorKey_Legacy)) &&
          __result is StardewValley.Object) {
      StardewValley.Objects.ColoredObject newColoredObject;
      if (__result is StardewValley.Objects.ColoredObject coloredObject) {
        newColoredObject = coloredObject;
      } else {
        newColoredObject = new StardewValley.Objects.ColoredObject(
            __result.QualifiedItemId,
            __result.Stack,
            Color.White
            );
        Helper.Reflection.GetMethod(newColoredObject, "GetOneCopyFrom").Invoke(__result);
        newColoredObject.Stack = __result.Stack;
      }
      var color = inputItem is StardewValley.Objects.ColoredObject coloredInput ?
        coloredInput.color.Value :
        TailoringMenu.GetDyeColor(inputItem);
      if (color != null) {
        newColoredObject.color.Value = (Color)color;
        __result = newColoredObject;
      }
    }
  }
}
}
