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
using System;

namespace WateringCanGiveExp;

internal class ModConfig
{
    /// <summary>
    /// Bonus multiplier to total movespeed per level of Travelling skill. Defaults to 0.5.
    /// </summary>
    public float ExpforWateringSoil { get; set; }

    /// <summary>
    /// Multiplier applied to exp gained from harvesting. Defaults to 0.75;
    /// </summary>
    public float HarvestingExpMultiplier { get; set; }


    public ModConfig()
    {
        this.ExpforWateringSoil = 0.5f;
        this.HarvestingExpMultiplier = 0.75f;
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

        // Harvesting exp multiplier
        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgExpperwatersoil_Name,
            tooltip: I18n.CfgExpperwatersoil_Desc,
            getValue: () => ExpforWateringSoil.ToString(),
            setValue: value => ExpforWateringSoil = float.Parse(value),
            allowedValues: new string[] { "0.25", "0.5", "0.75", "1", "2" },
            formatAllowedValue: displayExpGainValues
         );

        // Exp per watered soil
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgHarvestexpmultiplier_Name,
            tooltip: I18n.CfgHarvestexpmultiplier_Desc,
            getValue: () => HarvestingExpMultiplier,
            setValue: value => HarvestingExpMultiplier = value,
            min: 50f / 100f,
            max: 100f / 100f,
            interval: 5f / 100f,
            formatValue: displayAsPercentage
         );

    }

    private static string displayExpGainValues(string expgain_option)
    {
        switch (expgain_option)
        {
            case "0.25": return "0.25 (Every 4 tiles)";
            case "0.5": return "0.50 (Every other tile)";
            case "0.75": return "0.75 (2 Exp for 3 tiles)";
            case "1": return "1 (Every tile)";
            case "2": return "2 (Every tile gives two exp)";
        }
        return "Something went wrong... :(";
    }

    public static string displayAsPercentage(float value)
    {
        return Math.Round(100f * value, 2).ToString() + "%";
    }
}


