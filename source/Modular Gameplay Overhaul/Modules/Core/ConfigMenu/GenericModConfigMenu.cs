/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Tools;
using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Integrations.GMCM;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed class GenericModConfigMenu : GMCMBuilder<GenericModConfigMenu>
{
    private readonly Dictionary<string, object> _changedFields = new();

    private bool _reload;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenu"/> class.</summary>
    internal GenericModConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, ModEntry.Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.SetTitleScreenOnlyForNextOptions(true);
        if (!LocalData.InitialSetupComplete)
        {
            this.AddParagraph(I18n.Gmcm_Core_Initial);
        }
        else
        {
            this.AddParagraph(I18n.Gmcm_Core_Choose);
        }

        this.AddModuleSelectionOption();
        if (!LocalData.InitialSetupComplete)
        {
            return;
        }

        this
            .AddHorizontalRule()
            .SetTitleScreenOnlyForNextOptions(false)
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Core_Modules,
                pages: EnumerateModules().Skip(1).Where(m => m._ShouldEnable).ToArray(),
                getPageId: module => module.Namespace,
                getPageName: module => module.Name,
                getColumnsFromWidth: _ => 2);

        this.BuildImplicitly(() => Config);

        this.OnFieldChanged((name, value) =>
        {
            this._changedFields[name] = value;
            this._reload = true;
        });
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new ModConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);

        if (this._changedFields.Count > 0)
        {
            var changedText = this._changedFields.Aggregate(
                "[Config]: Changed the following config settings:",
                (current, next) => current + $"\n\"{next.Key}\": \"{next.Value}\"");
            Log.D(changedText);
            this._changedFields.Clear();
        }

        if (!this._reload)
        {
            return;
        }

        this.Reload();
        this._reload = false;
    }

    #region GMCM overrides

    [UsedImplicitly]
    private static void ProfessionConfigSkillExpMulitpliersOverride()
    {
        Instance!.AssertRegistered();
        foreach (var (skillId, multiplier) in Config.Professions.SkillExpMultipliers)
        {
            if (Skill.TryFromName(skillId, out var skill))
            {
                Instance.AddFloatSlider(
                    () => I18n.Gmcm_SkillExpMultipliers_Title(skill.DisplayName),
                    () => I18n.Gmcm_SkillExpMultipliers_Desc(skill.DisplayName),
                    config => config.SkillExpMultipliers[skill.Name],
                    (config, value) => config.SkillExpMultipliers[skill.Name] = value,
                    () => Config.Professions,
                    0.2f,
                    2f,
                    id: "SkillExpMultipliers." + skill.Name);
                continue;
            }

            if (SCSkill.Loaded.TryGetValue(skillId, out var scSkill))
            {
                Instance.AddFloatSlider(
                    () => I18n.Gmcm_SkillExpMultipliers_Title(scSkill.DisplayName),
                    () => I18n.Gmcm_SkillExpMultipliers_Desc(scSkill.DisplayName),
                    config => config.SkillExpMultipliers[scSkill.StringId],
                    (config, value) => config.SkillExpMultipliers[scSkill.StringId] = value,
                    () => Config.Professions,
                    0.2f,
                    2f,
                    id: "SkillExpMultipliers." + scSkill.StringId);
            }
        }
    }

    [UsedImplicitly]
    private static void CombatConfigColorByTierOverride()
    {
        Instance!.AssertRegistered();
        for (var i = 0; i < Config.Combat.ColorByTier.Length; i++)
        {
            var tier = (WeaponTier)i;
            var @default = Config.Combat.ColorByTier[i];
            Instance.AddColorPicker(
                () => Instance._I18n.Get($"gmcm.color_by_tier.{tier.Name.ToLower()}.title"),
                () => Instance._I18n.Get($"gmcm.color_by_tier.{tier.Name.ToLower()}.desc"),
                config => config.ColorByTier[tier],
                (config, value) => config.ColorByTier[tier] = value,
                () => Config.Combat,
                @default,
                false,
                (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders,
                "ColorByTier." + tier.Name);
        }
    }

    [UsedImplicitly]
    private static void AxeRequiredUpgradeForChargingOverride()
    {
        Instance!.AssertLoaded();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance.AddDropdown(
            I18n.Gmcm_RequiredUpgradeForCharging_Title,
            I18n.Gmcm_RequiredUpgradeForCharging_Desc,
            config => config.RequiredUpgradeForCharging.ToStringFast(),
            (config, value) => config.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
            () => Config.Tools.Axe,
            UpgradeLevelExtensions.GetNames().Take(maxUpgradeLevel + 1).ToArray(),
            value => ModEntry._I18n.Get($"gmcm.upgrade_level.{value.ToLower()}"),
            "RequiredUpgradeForCharging.Axe");
    }

    [UsedImplicitly]
    private static void AxeConfigRadiusAtEachLevelOverride()
    {
        Instance!.AssertRegistered();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Desc,
                config => (int)config.RadiusAtEachPowerLevel[0],
                (config, value) => config.RadiusAtEachPowerLevel[0] = (uint)value,
                () => Config.Tools.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Copper")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Desc,
                config => (int)config.RadiusAtEachPowerLevel[1],
                (config, value) => config.RadiusAtEachPowerLevel[1] = (uint)value,
                () => Config.Tools.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Steel")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Desc,
                config => (int)config.RadiusAtEachPowerLevel[2],
                (config, value) => config.RadiusAtEachPowerLevel[2] = (uint)value,
                () => Config.Tools.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Gold")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Desc,
                config => (int)config.RadiusAtEachPowerLevel[3],
                (config, value) => config.RadiusAtEachPowerLevel[3] = (uint)value,
                () => Config.Tools.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Iridium");

        if (maxUpgradeLevel > 5 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[4],
                    (config, value) => config.RadiusAtEachPowerLevel[4] = (uint)value,
                    () => Config.Tools.Axe,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Axe.Radioactive");
        }

        if (maxUpgradeLevel > 6 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[5],
                    (config, value) => config.RadiusAtEachPowerLevel[5] = (uint)value,
                    () => Config.Tools.Axe,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Axe.Mythicite");
        }

        Instance
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Reaching_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Reaching_Desc,
                config => (int)config.RadiusAtEachPowerLevel[maxUpgradeLevel - 1],
                (config, value) => config.RadiusAtEachPowerLevel[maxUpgradeLevel - 1] = (uint)value,
                () => Config.Tools.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Reaching");
    }

    [UsedImplicitly]
    private static void PickaxeRequiredUpgradeForChargingOverride()
    {
        Instance!.AssertLoaded();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance.AddDropdown(
            I18n.Gmcm_RequiredUpgradeForCharging_Title,
            I18n.Gmcm_RequiredUpgradeForCharging_Desc,
            config => config.RequiredUpgradeForCharging.ToStringFast(),
            (config, value) => config.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
            () => Config.Tools.Pick,
            UpgradeLevelExtensions.GetNames().Take(maxUpgradeLevel + 1).ToArray(),
            value => ModEntry._I18n.Get($"gmcm.upgrade_level.{value.ToLower()}"),
            "RequiredUpgradeForCharging.Pick");
    }

    [UsedImplicitly]
    private static void PickaxeConfigRadiusAtEachLevelOverride()
    {
        Instance!.AssertRegistered();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Desc,
                config => (int)config.RadiusAtEachPowerLevel[0],
                (config, value) => config.RadiusAtEachPowerLevel[0] = (uint)value,
                () => Config.Tools.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Copper")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Desc,
                config => (int)config.RadiusAtEachPowerLevel[1],
                (config, value) => config.RadiusAtEachPowerLevel[1] = (uint)value,
                () => Config.Tools.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Steel")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Desc,
                config => (int)config.RadiusAtEachPowerLevel[2],
                (config, value) => config.RadiusAtEachPowerLevel[2] = (uint)value,
                () => Config.Tools.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Gold")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Desc,
                config => (int)config.RadiusAtEachPowerLevel[3],
                (config, value) => config.RadiusAtEachPowerLevel[3] = (uint)value,
                () => Config.Tools.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Iridium");

        if (maxUpgradeLevel > 5 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[4],
                    (config, value) => config.RadiusAtEachPowerLevel[4] = (uint)value,
                    () => Config.Tools.Pick,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Pick.Radioactive");
        }

        if (maxUpgradeLevel > 6 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[5],
                    (config, value) => config.RadiusAtEachPowerLevel[5] = (uint)value,
                    () => Config.Tools.Pick,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Pick.Mythicite");
        }

        Instance
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Reaching_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Reaching_Desc,
                config => (int)config.RadiusAtEachPowerLevel[maxUpgradeLevel - 1],
                (config, value) => config.RadiusAtEachPowerLevel[maxUpgradeLevel - 1] = (uint)value,
                () => Config.Tools.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Reaching");
    }

    [UsedImplicitly]
    private static void HoeConfigAffectedTilesAtEachPowerLevelOverride()
    {
        Instance!.AssertRegistered();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Copper_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Copper_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                () => Config.Tools.Hoe,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Copper")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Copper_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Copper_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                () => Config.Tools.Hoe,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Copper")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Steel_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Steel_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                () => Config.Tools.Hoe,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Steel")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Steel_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Steel_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                () => Config.Tools.Hoe,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Steel")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Gold_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Gold_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                () => Config.Tools.Hoe,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Gold")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Gold_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Gold_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                () => Config.Tools.Hoe,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Gold")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Iridium_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Iridium_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                () => Config.Tools.Hoe,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Iridium")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Iridium_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Iridium_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                () => Config.Tools.Hoe,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Iridium");

        if (maxUpgradeLevel > 5 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Radioactive_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Radioactive_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    () => Config.Tools.Hoe,
                    0,
                    15,
                    id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Radioactive")
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Radioactive_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Radioactive_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    () => Config.Tools.Hoe,
                    0,
                    7,
                    id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Radioactive");
        }

        if (maxUpgradeLevel > 6 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Mythicite_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Mythicite_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    () => Config.Tools.Hoe,
                    0,
                    15,
                    id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Mythicite")
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Mythicite_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Mythicite_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    () => Config.Tools.Hoe,
                    0,
                    7,
                    id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Mythicite");
        }

        Instance
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Reaching_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Reaching_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Length = (uint)value,
                () => Config.Tools.Hoe,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Length.Reaching")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Reaching_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Reaching_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Radius = (uint)value,
                () => Config.Tools.Hoe,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Hoe.Radius.Reaching");
    }

    [UsedImplicitly]
    private static void WateringCanConfigAffectedTilesAtEachPowerLevelOverride()
    {
        Instance!.AssertRegistered();

        var maxUpgradeLevel = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading ? 6 : 5;
        Instance
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Copper_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Copper_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                () => Config.Tools.Can,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Can.Length.Copper")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Copper_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Copper_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                () => Config.Tools.Can,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Can.Radius.Copper")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Steel_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Steel_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                () => Config.Tools.Can,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Can.Length.Steel")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Steel_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Steel_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                () => Config.Tools.Can,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Can.Radius.Steel")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Gold_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Gold_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                () => Config.Tools.Can,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Can.Length.Gold")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Gold_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Gold_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                () => Config.Tools.Can,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Can.Radius.Gold")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Iridium_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Iridium_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                () => Config.Tools.Can,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Can.Length.Iridium")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Iridium_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Iridium_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                () => Config.Tools.Can,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Can.Radius.Iridium");

        if (maxUpgradeLevel > 5 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Radioactive_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Radioactive_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    () => Config.Tools.Can,
                    0,
                    15,
                    id: "AffectedTilesAtEachPowerLevel.Can.Length.Radioactive")
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Radioactive_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Radioactive_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    () => Config.Tools.Can,
                    0,
                    7,
                    id: "AffectedTilesAtEachPowerLevel.Can.Radius.Radioactive");
        }

        if (maxUpgradeLevel > 6 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Mythicite_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Mythicite_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    () => Config.Tools.Can,
                    0,
                    15,
                    id: "AffectedTilesAtEachPowerLevel.Can.Length.Mythicite")
                .AddIntSlider(
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Mythicite_Title,
                    I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Mythicite_Desc,
                    config => (int)config.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    () => Config.Tools.Can,
                    0,
                    7,
                    id: "AffectedTilesAtEachPowerLevel.Can.Radius.Radioactive");
        }

        Instance
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Reaching_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Length_Reaching_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Length,
                (config, value) => config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Length = (uint)value,
                () => Config.Tools.Can,
                0,
                15,
                id: "AffectedTilesAtEachPowerLevel.Can.Length.Reaching")
            .AddIntSlider(
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Reaching_Title,
                I18n.Gmcm_AffectedTilesAtEachPowerLevel_Radius_Reaching_Desc,
                config => (int)config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Radius,
                (config, value) => config.AffectedTilesAtEachPowerLevel[maxUpgradeLevel - 1].Radius = (uint)value,
                () => Config.Tools.Can,
                0,
                7,
                id: "AffectedTilesAtEachPowerLevel.Can.Radius.Reaching");
    }

    [UsedImplicitly]
    private static void TaxConfigTaxByIncomeBracketOverride()
    {
        Instance!.AssertRegistered();
        Instance
            .AddTextBox(
                I18n.Gmcm_IncomeBrackets_Title,
                I18n.Gmcm_IncomeBrackets_Desc,
                config => string.Join(", ", config.Taxes.TaxByIncomeBracket.Keys),
                (config, value) =>
                {
                    var keys = value.TrimAll()
                        .Split(new[] { ',', ';' }, StringSplitOptions.None)
                        .Select(int.Parse)
                        .ToArray();
                    var values = config.Taxes.TaxByIncomeBracket.Values.ToArray();
                    if (keys.Length != values.Length)
                    {
                        Log.E(
                            "Failed updating income brackets. Please make sure that all income brackets have a corresponding tax value.");
                        return;
                    }

                    var dict = new Dictionary<int, float>();
                    for (var i = 0; i < keys.Length; i++)
                    {
                        if (dict.TryAdd(keys[i], values[i]))
                        {
                            continue;
                        }

                        Log.E($"Failed adding income bracket {keys[i]}. Please make sure the bracket is unique.");
                        return;
                    }

                    config.Taxes.TaxByIncomeBracket = dict;

                },
                () => Config,
                "TaxPerIncomeBracket.Keys")

            .AddTextBox(
                I18n.Gmcm_TaxPerBracket_Title,
                I18n.Gmcm_TaxPerBracket_Desc,
                config => string.Join(", ", config.TaxByIncomeBracket.Values),
                (config, value) =>
                {
                    var keys = config.TaxByIncomeBracket.Keys.ToArray();
                    var values = value.TrimAll()
                        .Split(',')
                        .Select(float.Parse)
                        .ToArray();
                    if (keys.Length != values.Length)
                    {
                        Log.E(
                            "Failed updating tax per bracket. Please make sure that all income brackets have a corresponding tax value.");
                        return;
                    }

                    var dict = new Dictionary<int, float>();
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (dict.TryAdd(keys[i], values[i]))
                        {
                            continue;
                        }

                        Log.E($"Failed adding tax value {values[i]}. Please make sure the bracket is unique.");
                        return;
                    }

                    config.TaxByIncomeBracket = dict;

                },
                () => Config.Taxes,
                "TaxPerIncomeBracket.Values");
    }

    [UsedImplicitly]
    private static void TweexConfigSpawnCrowsOnTheseMapsOverride()
    {
        Instance!.AssertRegistered();

        var farmMaps = new List<string> { "IslandWest" };
        if (ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            farmMaps.AddRange(new[] { "Custom_Garden", "Custom_GrampletonFields" });
        }

        if (ModHelper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
        {
            farmMaps.Add("Custom_Ridgeside_SummitFarm");
        }

        if (ModHelper.ModRegistry.IsLoaded("LemurKat.EastScarpe.SMAPI"))
        {
            farmMaps.Add("Custom_ESMeadowFarm");
        }

        Instance.AddMultiCheckboxOption(
            I18n.Gmcm_SpawnCrowsOnTheseMaps_Title,
            farmMaps.ToArray(),
            map => TweexModule.Config.SpawnCrowsOnTheseMaps.Contains(map),
            (map, value) =>
            {
                if (value)
                {
                    TweexModule.Config.SpawnCrowsOnTheseMaps.Add(map);
                    if (map == "Custom_GrampletonFields")
                    {
                        TweexModule.Config.SpawnCrowsOnTheseMaps.Add("Custom_GrampletonFields_Small");
                    }
                }
                else
                {
                    TweexModule.Config.SpawnCrowsOnTheseMaps.Remove(map);
                    if (map == "Custom_GrampletonFields")
                    {
                        TweexModule.Config.SpawnCrowsOnTheseMaps.Remove("Custom_GrampletonFields_Small");
                    }
                }
            },
            _ => 2,
            map => Instance._I18n.Get("gmcm.spawn_crows_on_these_maps." + map.ToLower()));
    }

    #endregion GMCM overrides

    /// <summary>Adds a new instance of <see cref="ModuleSelectionOption"/> to this mod menu.</summary>
    private GenericModConfigMenu AddModuleSelectionOption()
    {
        this.AssertRegistered();
        new ModuleSelectionOption(this.Reload).AddToMenu(this.ModApi, this.Manifest);
        return this;
    }
}
