/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Integrations;
using System;

namespace StardewTravelSkill;

internal class ModConfig
{
    /// <summary>
    /// Exp gained for watering a single tile
    /// </summary>
    public float LevelMovespeedBonus { get; set; }

    /// <summary>
    /// Movespeed bonus granted by <see cref="ProfessionMovespeed"/>. Defaults to 0.05.
    /// </summary>
    public float MovespeedProfessionBonus { get; set; }

    /// <summary>
    /// Percentage of stamina that recovers every 10 minutes by <see cref="ProfessionRestoreStamina"/>. Defaults to 1%
    /// </summary>
    public float RestoreStaminaPercentage { get; set; }

    /// <summary>
    /// Bonus multiplier to movespeed that is applied by sprinting
    /// </summary>
    public float SprintMovespeedBonus { get; set; }

    /// <summary>
    /// Use chance for a totem when profession is unlocked
    /// </summary>
    public float TotemUseChance { get; set; }

    /// <summary>
    /// Number of steps to walk before getting 1 Exp
    /// </summary>
    public int StepsPerExp { get; set; }

    /// <summary>
    /// Number of steps to walk before getting sprint bonus
    /// </summary>
    public int SprintSteps { get; set; }

    /// <summary>
    /// Add exp in steps of ExpGainStepThreshold. Ie, with 25 steps per exp and threshold = 1, the player gains 1 exp after 25 steps. With increment 5, the player gains 5 exp after 25*5 steps
    /// </summary>
    public int ExpGainStepThreshold;

    public ModConfig()
    {
        // Changable by player
        this.LevelMovespeedBonus = 0.01f;
        this.MovespeedProfessionBonus = 0.05f;
        this.RestoreStaminaPercentage = 0.01f;
        this.SprintMovespeedBonus = 0.15f;
        this.TotemUseChance = 0.5f;
        this.StepsPerExp = 20;

        // Unchangable by player via in game menu
        this.SprintSteps = 5;
        this.ExpGainStepThreshold = 10;
    }

    /// <summary>
    /// Constructs config menu for GenericConfigMenu mod
    /// </summary>
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
            text: I18n.CfgSection_Travelskill,
            tooltip: null
        );

        // Steps per Exp
        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgExpgain_Name,
            tooltip: I18n.CfgExpgain_Desc,
            getValue: () => StepsPerExp.ToString(),
            setValue: value => StepsPerExp = int.Parse(value),
            allowedValues: new string[] { "5", "10", "20", "50", "100" },
            formatAllowedValue: displayExpGainValues
         );

        // Level movespeed bonus
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgLevelmovespeed_Name,
            tooltip: I18n.CfgLevelmovespeed_Desc,
            getValue: () => LevelMovespeedBonus,
            setValue: value => LevelMovespeedBonus = value,
            min: 0f / 100f,
            max: 2f / 100f,
            interval: 0.05f / 100f,
            formatValue: displayAsPercentage
         );

        /// profession settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_Professions,
            tooltip: null
        );

        // Movespeed profession bonus
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgMovespeedbonus_Name,
            tooltip: I18n.CfgMovespeedbonus_Desc,
            getValue: () => MovespeedProfessionBonus,
            setValue: value => MovespeedProfessionBonus = value,
            min: 0f / 100f,
            max: 10f / 100f,
            interval: 0.5f / 100f,
            formatValue: displayAsPercentage
         );

        // Sprint profession bonus
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgSprintbonus_Name,
            tooltip: I18n.CfgSprintbonus_Desc,
            getValue: () => SprintMovespeedBonus,
            setValue: value => SprintMovespeedBonus = value,
            min: 0f / 100f,
            max: 30f / 100f,
            interval: 0.5f / 100f,
            formatValue: displayAsPercentage
         );

        // Restore stamina percentage
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgRestorestamina_Name,
            tooltip: I18n.CfgRestorestamina_Desc,
            getValue: () => RestoreStaminaPercentage,
            setValue: value => RestoreStaminaPercentage = value,
            min: 0f / 100f,
            max: 2f / 100f,
            interval: 0.05f / 100f,
            formatValue: displayAsPercentage
         );

        // Totem reuse
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgTotemreuse_Name,
            tooltip: I18n.CfgTotemreuse_Desc,
            getValue: () => TotemUseChance,
            setValue: value => TotemUseChance = value,
            min: 25f / 100f,
            max: 75f / 100f,
            interval: 5f / 100f,
            formatValue: displayAsPercentage
         );

        // TODO add options for cheap recipes/obelisks
    }

    private static string displayExpGainValues(string expgain_option)
    {
        switch (expgain_option)
        {
            case "5": return "5 (Very Fast)";
            case "10": return "10 (Fast)";
            case "20": return "20 (Normal)";
            case "50": return "50 (Slow)";
            case "100": return "100 (Very Slow)";
        }
        // should be unreachable, if this ever appears then you made a mistake sir programmer
        return "Something went wrong... :(";
    }

    /// <summary>
    /// Displays <paramref name="value"/> as a percentage, rounded to two decimals.
    /// <c>ModConfig.displayAsPercentage(0.02542); // returns 2.54%</c>
    /// </summary>
    public static string displayAsPercentage(float value)
    {
        return Math.Round(100f * value, 2).ToString() + "%";
    }
}
