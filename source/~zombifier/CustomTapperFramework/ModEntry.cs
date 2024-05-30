/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

using SObject = StardewValley.Object;

namespace CustomTapperFramework;

internal sealed class ModEntry : Mod {
  internal static new IModHelper Helper;
  internal static IMonitor StaticMonitor;
  public static string UniqueId;
  internal static AssetHandler assetHandler;

  public override void Entry(IModHelper helper) {
    Helper = helper;
    StaticMonitor = this.Monitor;
    UniqueId = this.ModManifest.UniqueID;

    assetHandler = new AssetHandler();

    helper.Events.Content.AssetRequested += assetHandler.OnAssetRequested;
    helper.Events.Content.AssetReady += assetHandler.OnAssetReady;
    helper.Events.Content.AssetsInvalidated += assetHandler.OnAssetsInvalidated;
    helper.Events.GameLoop.GameLaunched += assetHandler.OnGameLaunched;

    helper.Events.GameLoop.DayStarted += OnDayStarted;
    helper.Events.Input.ButtonPressed += OnButtonPressed;

    var harmony = new Harmony(ModEntry.UniqueId);
    HarmonyPatcher.ApplyPatches(harmony);
    try {
      if (Helper.ModRegistry.IsLoaded("Pathoschild.Automate")) {
        this.Monitor.Log("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.", LogLevel.Debug);
        AutomatePatcher.ApplyPatches(harmony);
      }
    } catch (Exception e) {
      Monitor.Log("Failed patching Automate. Detail: " + e.Message, LogLevel.Error);
    }

    try {
      if (Helper.ModRegistry.IsLoaded("NermNermNerm.Junimatic")) {
        this.Monitor.Log("This mod patches Junimatic. If you notice issues with Junimatic, make sure it happens without this mod before reporting it to the Junimatic page.", LogLevel.Debug);
        JunimaticPatcher.ApplyPatches(harmony);
      }
    } catch (Exception e) {
      Monitor.Log("Failed patching Junimatic. Detail: " + e.Message, LogLevel.Error);
    }

    try {
      if (Helper.ModRegistry.IsLoaded("furyx639.CustomBush")) {
        this.Monitor.Log("This mod patches Custom Bush. If you notice issues with Custom Bush, make sure it happens without this mod before reporting it to the Custom Bush page.", LogLevel.Debug);
        CustomBushPatcher.ApplyPatches(harmony);
      }
    } catch (Exception e) {
      Monitor.Log("Failed patching Custom Bush. Detail: " + e.Message, LogLevel.Error);
    }
  }

  public void OnDayStarted(object sender, DayStartedEventArgs e) {
    foreach (var location in Game1.locations) {
      foreach (var obj in location.objects.Values) {
        if (obj.IsTapper() && obj.heldObject.Value == null) {
          Utils.UpdateTapperProduct(obj);
        }
        // Water the pots automatically since they're, well, water pots.
        if (obj is IndoorPot pot &&
            (pot.QualifiedItemId == WaterIndoorPotUtils.WaterPotQualifiedItemId ||
             pot.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId)) {
          pot.hoeDirt.Value.state.Value = 1;
        }
      }
    }

    // Learn the water crop recipes
    bool hasAquaticCrops = false;
		foreach (KeyValuePair<string, CropData> cropData in Game1.cropData) {
      if ((cropData.Value.CustomFields?.ContainsKey(WaterIndoorPotUtils.CropIsWaterCustomFieldsKey) ?? false)
          || (cropData.Value.CustomFields?.ContainsKey(WaterIndoorPotUtils.CropIsAmphibiousCustomFieldsKey) ?? false)) {
        hasAquaticCrops = true;
      }
      if (hasAquaticCrops) {
        Game1.player.craftingRecipes.TryAdd(WaterIndoorPotUtils.WaterPlanterItemId, 0);

        if (Game1.player.craftingRecipes.ContainsKey("Garden Pot")) {
          Game1.player.craftingRecipes.TryAdd(WaterIndoorPotUtils.WaterPotItemId, 0);
        }
      } else {
        Game1.player.craftingRecipes.Remove(WaterIndoorPotUtils.WaterPlanterItemId);
        Game1.player.craftingRecipes.Remove(WaterIndoorPotUtils.WaterPotItemId);
      }
    }
  }

  private bool IsNormalGameplay() {
    return StardewModdingAPI.Context.CanPlayerMove
      && Game1.player != null
      && !Game1.player.isRidingHorse()
      && Game1.currentLocation != null
      && !Game1.eventUp
      && !Game1.isFestival()
      && !Game1.IsFading();
  }

  public void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
    if (!IsNormalGameplay()) return;
    if ((e.Button.IsUseToolButton() || e.Button.IsActionButton()) &&
        Game1.player.ActiveObject is SObject obj) {
      // Place Tapper
      if (obj.IsTapper() &&
        Utils.IsModdedTapperPlaceableAt(obj, Game1.currentLocation, e.Cursor.GrabTile, out var unused, out var feature, out var centerPos)) {
        // Place tapper if able
        SObject @object = (SObject)obj.getOne();
        @object.heldObject.Value = null;
        @object.TileLocation = centerPos;
        Game1.currentLocation.objects.Add(centerPos, @object);
        Utils.UpdateTapperProduct(@object);
        @object.performDropDownAction(Game1.player);
        Game1.currentLocation.playSound("axe");
        Game1.player.reduceActiveItemByOne();
        Utils.Shake(feature, centerPos);
      }
      // Place Crab Pot if able
      // NOTE: Handled in placementAction patch
      //if (Utils.IsCrabPot(obj) &&
      //    CrabPot.IsValidCrabPotLocationTile(Game1.currentLocation,
      //      (int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y)) {
      //  SObject @object = (SObject)obj.getOne();
      //  CustomCrabPotUtils.placementAction(obj, Game1.currentLocation, (int)e.Cursor.GrabTile.X*64, (int)e.Cursor.GrabTile.Y*64, Game1.player);
      //  Game1.player.reduceActiveItemByOne();
      //}
    }
    // When taking a machine output, also shake the fruit tree for fruits
    if (e.Button.IsActionButton() &&
        Game1.currentLocation.objects.TryGetValue(e.Cursor.GrabTile, out var machine) &&
        machine.heldObject.Value != null &&
        Game1.currentLocation.terrainFeatures.TryGetValue(e.Cursor.GrabTile, out var feature2) &&
        feature2 is FruitTree fruitTree) {
      fruitTree.shake(e.Cursor.GrabTile, false);
    }
  }
}
