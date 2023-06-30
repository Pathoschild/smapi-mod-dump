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
using System;

namespace BetterRods;

internal class ModConfig
{

    private readonly SliderRange nibbleTimeMultiplierSlider = new SliderRange(min: 0.75f, max: 1.25f, interval: 0.05f);
    private readonly SliderRange speedMultiplierSlider = new SliderRange(min: 0.85f, max: 1.15f, interval: 0.05f);
    private readonly SliderRange distanceGainMultiplierSlider = new SliderRange(min: 0.75f, max: 1.25f, interval: 0.05f);
    private readonly SliderRange distanceLossMultiplierSlider = new SliderRange(min: 0.75f, max: 1.25f, interval: 0.05f);

    public bool enableNibbleTimeMultiplier;
    public bool enableSpeedMultiplier;
    public bool enableDistanceGainMultiplier;
    public bool enableDistanceLossMultiplier;

    public float bambooNibbleTimeMultiplier;
    public float bambooSpeedMultiplier;
    public float bambooDistanceGainMultiplier;
    public float bambooDistanceLossMultiplier;

    public float fiberglassNibbleTimeMultiplier;
    public float fiberglassSpeedMultiplier;
    public float fiberglassDistanceGainMultiplier;
    public float fiberglassDistanceLossMultiplier;

    public float iridiumNibbleTimeMultiplier;
    public float iridiumSpeedMultiplier;
    public float iridiumDistanceGainMultiplier;
    public float iridiumDistanceLossMultiplier;

    public ModConfig()
    {
        this.enableNibbleTimeMultiplier = true;
        this.enableSpeedMultiplier = true;
        this.enableDistanceGainMultiplier = true;
        this.enableDistanceLossMultiplier = true;


        this.bambooNibbleTimeMultiplier = 1.0f;
        this.bambooSpeedMultiplier = 0.9f;
        this.bambooDistanceGainMultiplier = 0.9f;
        this.bambooDistanceLossMultiplier = 1.1f;

        this.fiberglassNibbleTimeMultiplier = 0.95f;
        this.fiberglassSpeedMultiplier = 1.0f;
        this.fiberglassDistanceGainMultiplier = 1.0f;
        this.fiberglassDistanceLossMultiplier = 1.0f;

        this.iridiumNibbleTimeMultiplier = 0.9f;
        this.iridiumSpeedMultiplier = 1.1f;
        this.iridiumDistanceGainMultiplier = 1.1f;
        this.iridiumDistanceLossMultiplier = 0.9f;
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

        /// General settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_General,
            tooltip: null
        );

        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnableNibbleTimeMultiplier_Name,
            tooltip: I18n.CfgEnableNibbleTimeMultiplier_Desc,
            getValue: () => this.enableNibbleTimeMultiplier,
            setValue: (val) => this.enableNibbleTimeMultiplier = val
        );

        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnableSpeedMultiplier_Name,
            tooltip: I18n.CfgEnableSpeedMultiplier_Desc,
            getValue: () => this.enableSpeedMultiplier,
            setValue: (val) => this.enableSpeedMultiplier = val
        );

        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnableDistanceGainMultiplier_Name,
            tooltip: I18n.CfgEnableDistanceGainMultiplier_Desc,
            getValue: () => this.enableSpeedMultiplier,
            setValue: (val) => this.enableSpeedMultiplier = val
        );

        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnableDistanceLossMultiplier_Name,
            tooltip: I18n.CfgEnableDistanceLossMultiplier_Desc,
            getValue: () => this.enableSpeedMultiplier,
            setValue: (val) => this.enableSpeedMultiplier = val
        );


        // Bamboo rod
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_BambooPole,
            tooltip: null
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgNibbleTimeMultiplier_Name,
            tooltip: I18n.CfgNibbleTimeMultiplier_Desc,
            getValue: () => this.bambooNibbleTimeMultiplier,
            setValue: (val) => this.bambooNibbleTimeMultiplier = val,
            min: nibbleTimeMultiplierSlider.min,
            max: nibbleTimeMultiplierSlider.max,
            interval: nibbleTimeMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgSpeedMultiplier_Name,
            tooltip: I18n.CfgSpeedMultiplier_Desc,
            getValue: () => this.bambooSpeedMultiplier,
            setValue: (val) => this.bambooSpeedMultiplier = val,
            min: speedMultiplierSlider.min,
            max: speedMultiplierSlider.max,
            interval: speedMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceGainMultiplier_Name,
            tooltip: I18n.CfgDistanceGainMultiplier_Desc,
            getValue: () => this.bambooDistanceGainMultiplier,
            setValue: (val) => this.bambooDistanceGainMultiplier = val,
            min: distanceGainMultiplierSlider.min,
            max: distanceGainMultiplierSlider.max,
            interval: distanceGainMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceLossMultiplier_Name,
            tooltip: I18n.CfgDistanceLossMultiplier_Desc,
            getValue: () => this.bambooDistanceLossMultiplier,
            setValue: (val) => this.bambooDistanceLossMultiplier = val,
            min: distanceLossMultiplierSlider.min,
            max: distanceLossMultiplierSlider.max,
            interval: distanceLossMultiplierSlider.interval
        );


        // Fiberglass rod
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_Fiberglass,
            tooltip: null
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgNibbleTimeMultiplier_Name,
            tooltip: I18n.CfgNibbleTimeMultiplier_Desc,
            getValue: () => this.fiberglassNibbleTimeMultiplier,
            setValue: (val) => this.fiberglassNibbleTimeMultiplier = val,
            min: nibbleTimeMultiplierSlider.min,
            max: nibbleTimeMultiplierSlider.max,
            interval: nibbleTimeMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgSpeedMultiplier_Name,
            tooltip: I18n.CfgSpeedMultiplier_Desc,
            getValue: () => this.fiberglassSpeedMultiplier,
            setValue: (val) => this.fiberglassSpeedMultiplier = val,
            min: speedMultiplierSlider.min,
            max: speedMultiplierSlider.max,
            interval: speedMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceGainMultiplier_Name,
            tooltip: I18n.CfgDistanceGainMultiplier_Desc,
            getValue: () => this.fiberglassDistanceGainMultiplier,
            setValue: (val) => this.fiberglassDistanceGainMultiplier = val,
            min: distanceGainMultiplierSlider.min,
            max: distanceGainMultiplierSlider.max,
            interval: distanceGainMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceLossMultiplier_Name,
            tooltip: I18n.CfgDistanceLossMultiplier_Desc,
            getValue: () => this.fiberglassDistanceLossMultiplier,
            setValue: (val) => this.fiberglassDistanceLossMultiplier = val,
            min: distanceLossMultiplierSlider.min,
            max: distanceLossMultiplierSlider.max,
            interval: distanceLossMultiplierSlider.interval
        );


        // iridium rod
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_Iridium,
            tooltip: null
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgNibbleTimeMultiplier_Name,
            tooltip: I18n.CfgNibbleTimeMultiplier_Desc,
            getValue: () => this.iridiumNibbleTimeMultiplier,
            setValue: (val) => this.iridiumNibbleTimeMultiplier = val,
            min: nibbleTimeMultiplierSlider.min,
            max: nibbleTimeMultiplierSlider.max,
            interval: nibbleTimeMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgSpeedMultiplier_Name,
            tooltip: I18n.CfgSpeedMultiplier_Desc,
            getValue: () => this.iridiumSpeedMultiplier,
            setValue: (val) => this.iridiumSpeedMultiplier = val,
            min: speedMultiplierSlider.min,
            max: speedMultiplierSlider.max,
            interval: speedMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceGainMultiplier_Name,
            tooltip: I18n.CfgDistanceGainMultiplier_Desc,
            getValue: () => this.iridiumDistanceGainMultiplier,
            setValue: (val) => this.iridiumDistanceGainMultiplier = val,
            min: distanceGainMultiplierSlider.min,
            max: distanceGainMultiplierSlider.max,
            interval: distanceGainMultiplierSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgDistanceLossMultiplier_Name,
            tooltip: I18n.CfgDistanceLossMultiplier_Desc,
            getValue: () => this.iridiumDistanceLossMultiplier,
            setValue: (val) => this.iridiumDistanceLossMultiplier = val,
            min: distanceLossMultiplierSlider.min,
            max: distanceLossMultiplierSlider.max,
            interval: distanceLossMultiplierSlider.interval
        );

    }

    private static string displayExpGainValues(string expgain_option)
    {
        switch (expgain_option)
        {
            case "0.25": return "0.25 (Every 4 tiles)";
            case "0.50": return "0.50 (Every other tile)";
            case "0.75": return "0.75 (2 Exp for 3 tiles)";
            case "1": return "1 (Every tile)";
            case "2": return "2 (Every tile gives two exp)";
            case "5000": return "100 (debug option)";
        }
        return "Something went wrong... :(";
    }

    public static string displayAsPercentage(float value)
    {
        return Math.Round(100f * value, 2).ToString() + "%";
    }



}


