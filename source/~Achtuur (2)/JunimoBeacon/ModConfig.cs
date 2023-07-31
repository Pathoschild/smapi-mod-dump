/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using StardewModdingAPI;
using System;

namespace JunimoBeacon;

internal class ModConfig
{
    private readonly static SliderRange BeaconRangeSlider = new SliderRange(3, 8, 1);
    private readonly static SliderRange ExtraJunimoPerBeaconSlider = new SliderRange(0, 2, 1);

    /// <summary>
    /// Range of beacons (extends in all 4 directions)
    /// </summary>
    public int BeaconRange { get; set; }

    public int ExtraJunimoPerBeacon { get; set; }

    public ModConfig()
    {
        BeaconRange = 4;
        ExtraJunimoPerBeacon = 1;
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

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgBeaconRange_Name,
            tooltip: I18n.CfgBeaconRange_Desc,
            getValue: () => BeaconRange,
            setValue: (val) => BeaconRange = val,
            min: (int)BeaconRangeSlider.min,
            max: (int)BeaconRangeSlider.max,
            interval: (int)BeaconRangeSlider.interval
        );

        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgExtraJunimoPerBeacon_Name,
            tooltip: I18n.CfgExtraJunimoPerBeacon_Desc,
            getValue: () => ExtraJunimoPerBeacon,
            setValue: (val) => ExtraJunimoPerBeacon = val,
            min: (int)ExtraJunimoPerBeaconSlider.min,
            max: (int)ExtraJunimoPerBeaconSlider.max,
            interval: (int)ExtraJunimoPerBeaconSlider.interval
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

/// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
    void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
    void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
    void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
}


