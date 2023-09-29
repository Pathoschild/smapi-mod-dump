/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using BetterSprinklersPlus.Framework.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterSprinklersPlus.Framework
{
  public class BetterSprinklersPlusConfig
  {
    public enum BalancedModeOptions
    {
      Off,
      Easy,
      Normal,
      Hard,
      VeryHard,
    }

    public enum CannotAffordOptions
    {
      Off,
      DoNotWater,
    }

    public enum DefaultTilesOptions
    {
      CostMoney,
      AreFree,
      SameNumberAreFree,
    }

    public static readonly string[] RangeAllowedValues = {
      "7",
      "11",
      "15",
      "21",
      "25"
    };

    /// <summary>The maximum grid size. This one is Scarecrow grids</summary>
    public const int ScarecrowGridSize = 19;

    private static readonly string[] BalancedModeOptionsText =
    {
      "Off",
      "Easy",
      "Normal",
      "Hard",
      "Very Hard",
    };

    public static readonly float[] BalancedModeOptionsMultipliers =
    {
      0f,
      0.1f,
      0.25f,
      0.5f,
      1f
    };

    private static readonly string[] CannotAffordOptionsText =
    {
      "Off",
      "Don't Water",
    };

    private static readonly string[] DefaultTilesOptionsText =
    {
      "Cost Money",
      "Are Free",
      "Same Number are Free",
    };

    public static BetterSprinklersPlusConfig Active { get; set; }
    public static IModHelper Helper { get; set; }
    public static IManifest Mod { get; set; }
    public SButton ShowSprinklerEditKey { get; set; } = SButton.K;
    public SButton ShowOverlayKey { get; set; } = SButton.F3;
    public SButton ActivateKey { get; set; } = SButton.MouseRight;
    public bool OverlayEnabledOnPlace { get; set; } = true;
    public int BalancedMode { get; set; } = (int)BalancedModeOptions.Normal;
    /// <summary>
    /// If true the default sprinkler tiles for each sprinkler are free.
    /// </summary>
    public int DefaultTiles { get; set; } = (int)DefaultTilesOptions.CostMoney;
    public int CannotAfford { get; set; } = (int)CannotAffordOptions.DoNotWater;
    public bool BalancedModeCostMessage { get; set; } = true;
    public bool BalancedModeCannotAffordWarning { get; set; } = true;

    public Dictionary<int, int> Range { get; set; } = new()
    {
      [599] = 7,
      [621] = 11,
      [645] = 15,
    };
    public Dictionary<int, float> CostMultiplier { get; set; } = new()
    {
      [599] = 1.0f,
      [621] = 0.5f,
      [645] = 0.25f,
    };

    public float PressureNozzleMultiplier { get; set; } = 0.5f;

    public int MaxGridSize => Range.Values.Prepend(ScarecrowGridSize).Max();

    /// <summary>
    /// The sprinkler default sprinkler shape config
    /// Be warned, this is rotated 90deg (top to bottom is left to right)
    /// Don't remove the 2s, they are required (at the moment).
    /// </summary>
    public Dictionary<int, int[,]> SprinklerShapes { get; set; } = new()
    {
      [599] = new[,]
      {
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 1, 0, 1, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 }
      },
      [621] = new[,]
      {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
      },
      [645] = new[,]
      {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
      }
    };

    public static void Init(IModHelper helper, IManifest mod)
    {
      Helper = helper;
      Mod = mod;

      ReadConfig();
      SetupGenericModConfigMenu();
    }

    /// <summary>
    /// Reads/Re-reads config
    /// </summary>
    public static void ReadConfig()
    {
      Active = Helper.ReadConfig<BetterSprinklersPlusConfig>();
    }

    /// <summary>Save changes to the mod configuration.</summary>
    public static void SaveChanges()
    {
      Helper.WriteConfig(Active);
      Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
    }

    public static void UpdateMaxCoverage(BetterSprinklersPlusConfig config, int sprinklerId, string value) {
      try
      {
        var asInt = int.Parse(value);
        config.Range[sprinklerId] = asInt;
        var grid = config.SprinklerShapes[sprinklerId];
        var resized = grid.Resize(asInt);
        config.SprinklerShapes[sprinklerId] = resized;
      }
      catch (Exception e)
      {
        Logger.Error($"Error changing sprinkler value for {sprinklerId}: {e.Message}");
      }
    }

    public static void SetupGenericModConfigMenu()
    {
      var configMenu = Helper.ModRegistry
        .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

      if (configMenu is null)
        return;

      configMenu.Register(
        mod: Mod,
        reset: () => { BetterSprinklersPlusConfig.Active = new BetterSprinklersPlusConfig(); },
        save: SaveChanges);
      
      configMenu.AddSectionTitle(mod: Mod, () => "Balance:");

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Mode",
        tooltip: () => "Changes the amount that water costs.",
        getValue: () =>
        {
          try
          {
            return BalancedModeOptionsText[Active.BalancedMode] ?? "Off";
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Getting Balanced Mode option {Active.BalancedMode}: {exception.Message}");
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(BalancedModeOptionsText, value);
            if (index == -1) index = 0;
            Active.BalancedMode = index;
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Setting Balanced Mode option {value}: {exception.Message}");
            Active.BalancedMode = (int)BalancedModeOptions.Off;
          }
        },
        allowedValues: BalancedModeOptionsText
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Can't Afford",
        tooltip: () => "What to do when you can't afford the cost",
        getValue: () =>
        {
          try
          {
            return CannotAffordOptionsText[Active.CannotAfford] ?? "Off";
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Getting Can't Afford option {Active.CannotAfford}: {exception.Message}");
            return "Off";
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(CannotAffordOptionsText, value);
            if (index == -1) index = 0;
            Active.CannotAfford = index;
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Setting Can't Afford option {value}: {exception.Message}");
            Active.CannotAfford = (int)CannotAffordOptions.Off;
          }
        },
        allowedValues: CannotAffordOptionsText
      );
      
      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Default Tiles",
        tooltip: () => "What should default tiles do?",
        getValue: () =>
        {
          try
          {
            return DefaultTilesOptionsText[Active.DefaultTiles] ?? DefaultTilesOptionsText[0];
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Getting Default Tiles option {Active.DefaultTiles}: {exception.Message}");
            return DefaultTilesOptionsText[0];
          }
        },
        setValue: value =>
        {
          try
          {
            var index = Array.IndexOf(DefaultTilesOptionsText, value);
            if (index == -1) index = 0;
            Active.DefaultTiles = index;
          }
          catch (Exception exception)
          {
            Logger.Error($"Error Setting Default Tiles option {value}: {exception.Message}");
            Active.CannotAfford = (int)CannotAffordOptions.Off;
          }
        },
        allowedValues: DefaultTilesOptionsText
      );

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Bills Message",
        tooltip: () => "In the morning you'll see how much your sprinklers have cost.",
        getValue: () => Active.BalancedModeCostMessage,
        setValue: value => Active.BalancedModeCostMessage = value
      );

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Can't Afford Warning",
        tooltip: () => "In the morning you'll be warned if watering did not finish.",
        getValue: () => Active.BalancedModeCannotAffordWarning,
        setValue: value => Active.BalancedModeCannotAffordWarning = value
      );

      configMenu.AddSectionTitle(mod: Mod, () => "Options:");

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Sprinkler Range",
        getValue: () => $"{Active.Range[SprinklerHelper.SprinklerObjectIds[0]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[0], value),
        allowedValues: RangeAllowedValues
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Quality Sprinkler Range",
        getValue: () => $"{Active.Range[SprinklerHelper.SprinklerObjectIds[1]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[1], value),
        allowedValues: RangeAllowedValues
      );

      configMenu.AddTextOption(
        mod: Mod,
        name: () => "Iridium Sprinkler Range",
        getValue: () => $"{Active.Range[SprinklerHelper.SprinklerObjectIds[2]]}",
        setValue: value => UpdateMaxCoverage(Active, SprinklerHelper.SprinklerObjectIds[2], value),
        allowedValues: RangeAllowedValues
      );

      configMenu.AddBoolOption(
        mod: Mod,
        name: () => "Show Placement Coverage",
        tooltip: () => "When checked the Overlay shows Sprinkler/Scarecrow reach when placing.",
        getValue: () => Active.OverlayEnabledOnPlace,
        setValue: value => Active.OverlayEnabledOnPlace = value
      );

      configMenu.AddSectionTitle(mod: Mod, () => "Key Bindings:");

      configMenu.AddKeybind(
        mod: Mod,
        name: () => "Activate Button",
        tooltip: () => "The button to press to activate a sprinkler",
        getValue: () => Active.ActivateKey,
        setValue: value => Active.ActivateKey = value
      );

      configMenu.AddKeybind(
        mod: Mod,
        name: () => "Show Config Key",
        tooltip: () => "The key to press to change the boundary of all sprinklers",
        getValue: () => Active.ShowSprinklerEditKey,
        setValue: value => Active.ShowSprinklerEditKey = value
      );

      configMenu.AddKeybind(
        mod: Mod,
        name: () => "Show Overlay Key",
        tooltip: () => "The key to press to show the boundary of the highlighted sprinkler/scarecrow",
        getValue: () => Active.ShowOverlayKey,
        setValue: value => Active.ShowOverlayKey = value
      );
    }
  }
}
