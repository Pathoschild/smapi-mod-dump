/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Integrations;
using AchtuurCore.Utility;
using StardewModdingAPI;
using System;

namespace BetterPlanting;

internal class ModConfig
{
    private readonly SliderRange TileFillLimitSlider = new SliderRange(10f, 500f, 10f);

    public SButton IncrementModeKey { get; set; }
    public SButton DecrementModeKey { get; set; }
    public bool CanPlaceDiagonally { get; set; }

    public int TileFillLimit { get; set; }

    public FillMode DefaultFillMode { get; set; }

    public ModConfig()
    {
        // Initialise variables here

        IncrementModeKey = SButton.LeftControl;
        DecrementModeKey = SButton.RightControl;
        CanPlaceDiagonally = false;
        TileFillLimit = 200;
    }

    /// <summary>
    /// Constructs config menu for GenericConfigMenu mod
    /// </summary>
    /// <param name="instance"></param>
    public void createMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModEntry.Instance.ModManifest,
            reset: () => ModEntry.Instance.Config = new ModConfig(),
            save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
        );

        /// General travel skill settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_General,
            tooltip: null
        );

        // increment
        configMenu.AddKeybind(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgIncrementKeyBind_Name,
            tooltip: I18n.CfgIncrementKeyBind_Desc,
            getValue: () => this.IncrementModeKey,
            setValue: (key) => this.IncrementModeKey = key
        );

        // decrement
        configMenu.AddKeybind(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDecrementKeyBind_Name,
            tooltip: I18n.CfgDecrementKeyBind_Desc,
            getValue: () => this.DecrementModeKey,
            setValue: (key) => this.DecrementModeKey = key
        );

        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDefaultFillMode_Name,
            tooltip: I18n.CfgDefaultFillMode_Desc,
            getValue: () => GetFillModeString(this.DefaultFillMode),
            setValue: val => this.DefaultFillMode = SetFillModeFromString(val),
            allowedValues: new string[] {"Vanilla", "Row (3)", "Row (5)", "Square (3x3)", "Square (5x5)", "Square (7x7)", "Fill" }
        );

        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgPlaceDiagonally_Name,
            tooltip: I18n.CfgPlaceDiagonally_Desc,
            getValue: () => this.CanPlaceDiagonally,
            setValue: (val) => this.CanPlaceDiagonally = val
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgTileFillLimit_Name,
            tooltip: I18n.CfgTileFillLimit_Desc,
            getValue: () => this.TileFillLimit,
            setValue: (val) => this.TileFillLimit = val,
            min: (int)TileFillLimitSlider.min,
            max: (int)TileFillLimitSlider.max,
            interval: (int)TileFillLimitSlider.interval
        );

    }

    private FillMode SetFillModeFromString(string mode)
    {
        switch (mode)
        {
            case "Vanilla": 
                return FillMode.Disabled;
            case "Row (3)": 
                return FillMode.ThreeInARow;
            case "Row (5)": 
                return FillMode.FiveInARow;
            case "Square (3x3)": 
                return FillMode.ThreeSquare;
            case "Square (5x5)": 
                return FillMode.FiveSquare;
            case "Square (7x7)": 
                return FillMode.SevenSquare;
            case "Fill": 
                return FillMode.All;
        }
        return FillMode.Disabled;
    }

    private string GetFillModeString(FillMode mode)
    {
        switch (mode)
        {
            case FillMode.Disabled:
                return "Vanilla";
            case FillMode.ThreeInARow:
                return "Row (3)";
            case FillMode.FiveInARow:
                return "Row (5)";
            case FillMode.ThreeSquare:
                return "Square (3x3)";
            case FillMode.FiveSquare:
                return "Square (5x5)";
            case FillMode.SevenSquare:
                return "Square (7x7)";
            case FillMode.All:
                return "Fill";
                
        }
        return "";
    }
}


