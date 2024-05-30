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
using Microsoft.Xna.Framework.Graphics;
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

namespace ExtraMachineConfig; 

using SObject = StardewValley.Object;

internal sealed class ModEntry : Mod {
  internal new static IModHelper Helper { get;
    set;
  }

  internal static IMonitor Mmonitor { get; set; }
  internal static IExtraMachineConfigApi ModApi;

  // Keys for the CustomData map
  internal static Regex RequirementIdKeyRegex =
    new Regex(@"selph.ExtraMachineConfig\.RequirementId\.(\d+)");
  internal static Regex RequirementTagsKeyRegex =
    new Regex(@"selph.ExtraMachineConfig\.RequirementTags\.(\d+)");
  internal static string RequirementCountKeyPrefix = "selph.ExtraMachineConfig.RequirementCount";
  internal static string RequirementInvalidMsgKey = "selph.ExtraMachineConfig.RequirementInvalidMsg";
  internal static string InheritPreserveIdKey = "selph.ExtraMachineConfig.InheritPreserveId";
  internal static string CopyColorKey = "selph.ExtraMachineConfig.CopyColor";

  internal static string ExtraContextTagsKey = "selph.ExtraMachineConfig.ExtraContextTags";

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
    ModApi = new ExtraMachineConfigApi();

    var harmony = new Harmony(this.ModManifest.UniqueID);

    harmony.Patch(
        original: AccessTools.Method(
          typeof(StardewValley.MachineDataUtility),
          nameof(StardewValley.MachineDataUtility.GetOutputData),
          new Type[] { typeof(List<MachineItemOutput>), typeof(bool), typeof(Item),
          typeof(Farmer), typeof(GameLocation) }),
        prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MachineDataUtility_GetOutputData_prefix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(StardewValley.MachineDataUtility),
          nameof(StardewValley.MachineDataUtility.GetOutputItem)),
        postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.MachineDataUtility_GetOutputItem_postfix)));

    harmony.Patch(
        original: AccessTools.Method(typeof(Item),
          nameof(Item.GetContextTags)),
        postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Item_GetContextTags_postfix)));

    SmokedItemHarmonyPatcher.ApplyPatches(harmony);
    AnimalDataPatcher.ApplyPatches(harmony);
    Helper.Events.GameLoop.DayStarted += AnimalDataPatcher.OnDayStartedJunimoHut;
  }

  public override object GetApi() {
    return ModApi;
  }

  // This patch:
  // * Checks for additional fuel requirements specified in the output rule's custom data, and
  // removes rules that cannot be satisfied
  private static void MachineDataUtility_GetOutputData_prefix(ref List<MachineItemOutput> outputs,
      bool useFirstValidOutput, Item inputItem, Farmer who,
      GameLocation location) {
    if (outputs == null || outputs.Count < 0) {
      return;
    }
    string invalidMessage = null;
    IInventory inventory = SObject.autoLoadFrom ?? who.Items;
    List<MachineItemOutput> newOutputs = new List<MachineItemOutput>();
    foreach (MachineItemOutput output in outputs) {
      if (output.CustomData == null) {
        newOutputs.Add(output);
        continue;
      }
      bool valid = true;
      var extraRequirements = ModApi.GetExtraRequirements(output);
      foreach (var entry in extraRequirements) {
        if (Game1.player.getItemCountInList(inventory, entry.Item1) < entry.Item2) {
          valid = false;
        }
      }
      var extraTagsRequirements = ModApi.GetExtraTagsRequirements(output);
      foreach (var entry in extraTagsRequirements) {
        if (Utils.getItemCountInListByTags(inventory, entry.Item1) < entry.Item2) {
          valid = false;
        }
      }
      if (valid) {
        newOutputs.Add(output);
      } else {
        if (output.CustomData.TryGetValue(RequirementInvalidMsgKey, out var msg)) {
          invalidMessage ??= msg;
        }
        if (output.CustomData.TryGetValue(RequirementInvalidMsgKey_Legacy, out var msgLegacy)) {
          invalidMessage ??= msgLegacy;
        }
      }
    }
    outputs = newOutputs;
    if (outputs.Count == 0 && invalidMessage != null && who.IsLocalPlayer &&
        SObject.autoLoadFrom == null) {
      Game1.showRedMessage(invalidMessage);
    }
  }

  // This patch:
  // * Checks for additional fuel requirements specified in the output rule's custom data, and
  // removes them from inventory
  // * Checks if preserve ID is set to inherit the input item's preserve ID, and applies it
  // * Checks if a colored item should be created and apply the changes
  private static void MachineDataUtility_GetOutputItem_postfix(ref Item __result, SObject machine,
      MachineItemOutput outputData, Item inputItem,
      Farmer who, bool probe,
      ref int? overrideMinutesUntilReady) {
    if (__result == null || outputData == null || inputItem == null) {
      return;
    }
    IInventory inventory = SObject.autoLoadFrom ?? who.Items;
    // Inherit preserve ID
    if ((outputData.PreserveId == "INHERIT" ||
          (outputData.CustomData != null &&
           (outputData.CustomData.ContainsKey(InheritPreserveIdKey) ||
            outputData.CustomData.ContainsKey(InheritPreserveIdKey_Legacy)))) &&
        inputItem is SObject inputObject &&
        inputObject.preservedParentSheetIndex.Value != "-1" &&
        __result is SObject resultObject) {
      resultObject.preservedParentSheetIndex.Value = inputObject.preservedParentSheetIndex.Value;
    }
    if (outputData.CustomData == null) {
      return;
    }
    // Remove extra fuel
    var extraRequirements = ModApi.GetExtraRequirements(outputData);
    foreach (var entry in extraRequirements) {
      Utils.RemoveItemFromInventoryById(inventory, entry.Item1, entry.Item2);
    }
    var extraTagsRequirements = ModApi.GetExtraTagsRequirements(outputData);
    foreach (var entry in extraTagsRequirements) {
      Utils.RemoveItemFromInventoryByTags(inventory, entry.Item1, entry.Item2);
    }
    // Color the item
    if ((outputData.CustomData.ContainsKey(CopyColorKey) ||
          outputData.CustomData.ContainsKey(CopyColorKey_Legacy)) &&
        __result is SObject) {
      StardewValley.Objects.ColoredObject newColoredObject;
      if (__result is StardewValley.Objects.ColoredObject coloredObject) {
        newColoredObject = coloredObject;
      } else {
        newColoredObject = new StardewValley.Objects.ColoredObject(
            __result.ItemId,
            __result.Stack,
            Color.White
            );
        Helper.Reflection.GetMethod(newColoredObject, "GetOneCopyFrom").Invoke(__result);
        newColoredObject.Stack = __result.Stack;
      }
      var color = TailoringMenu.GetDyeColor(inputItem);
      if (color != null) {
        newColoredObject.color.Value = (Color)color;
        __result = newColoredObject;
      }
    }
  }

  private static void Item_GetContextTags_postfix(Item __instance, ref HashSet<string> __result) {
    if (__instance.modData.TryGetValue(ExtraContextTagsKey, out string contextTags)) {
      __result.UnionWith(contextTags.Split(","));
    }
  }
}
