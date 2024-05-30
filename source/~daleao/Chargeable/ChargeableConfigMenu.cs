/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Chargeable;

#region using directives

using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class ChargeableConfigMenu : GMCMBuilder<ChargeableConfigMenu>
{
    /// <summary>Initializes a new instance of the <see cref="ChargeableConfigMenu"/> class.</summary>
    internal ChargeableConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, ChargeableMod.Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.BuildImplicitly(() => Config);
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new ChargeableConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
    }

    [UsedImplicitly]
    private static void AxeRadiusAtEachLevelOverride()
    {
        const int maxUpgradeLevel = 5;
        Instance!
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Desc,
                config => (int)config.RadiusAtEachPowerLevel[0],
                (config, value) => config.RadiusAtEachPowerLevel[0] = (uint)value,
                () => Config.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Copper")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Desc,
                config => (int)config.RadiusAtEachPowerLevel[1],
                (config, value) => config.RadiusAtEachPowerLevel[1] = (uint)value,
                () => Config.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Steel")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Desc,
                config => (int)config.RadiusAtEachPowerLevel[2],
                (config, value) => config.RadiusAtEachPowerLevel[2] = (uint)value,
                () => Config.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Gold")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Desc,
                config => (int)config.RadiusAtEachPowerLevel[3],
                (config, value) => config.RadiusAtEachPowerLevel[3] = (uint)value,
                () => Config.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Iridium");

        if (maxUpgradeLevel > 5 && Config.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[4],
                    (config, value) => config.RadiusAtEachPowerLevel[4] = (uint)value,
                    () => Config.Axe,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Axe.Radioactive");
        }

        if (maxUpgradeLevel > 6 && Config.Axe.RadiusAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[5],
                    (config, value) => config.RadiusAtEachPowerLevel[5] = (uint)value,
                    () => Config.Axe,
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
                () => Config.Axe,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Axe.Reaching");
    }

    [UsedImplicitly]
    private static void PickaxeRadiusAtEachLevelOverride()
    {
        const int maxUpgradeLevel = 5;
        Instance!
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Copper_Desc,
                config => (int)config.RadiusAtEachPowerLevel[0],
                (config, value) => config.RadiusAtEachPowerLevel[0] = (uint)value,
                () => Config.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Copper")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Steel_Desc,
                config => (int)config.RadiusAtEachPowerLevel[1],
                (config, value) => config.RadiusAtEachPowerLevel[1] = (uint)value,
                () => Config.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Steel")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Gold_Desc,
                config => (int)config.RadiusAtEachPowerLevel[2],
                (config, value) => config.RadiusAtEachPowerLevel[2] = (uint)value,
                () => Config.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Gold")
            .AddIntSlider(
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Title,
                I18n.Gmcm_RadiusAtEachPowerLevel_Iridium_Desc,
                config => (int)config.RadiusAtEachPowerLevel[3],
                (config, value) => config.RadiusAtEachPowerLevel[3] = (uint)value,
                () => Config.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Iridium");

        if (maxUpgradeLevel > 5 && Config.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Radioactive_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[4],
                    (config, value) => config.RadiusAtEachPowerLevel[4] = (uint)value,
                    () => Config.Pick,
                    1,
                    10,
                    id: "RadiusAtEachPowerLevel.Pick.Radioactive");
        }

        if (maxUpgradeLevel > 6 && Config.Pick.RadiusAtEachPowerLevel.Length > 6)
        {
            Instance
                .AddIntSlider(
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Title,
                    I18n.Gmcm_RadiusAtEachPowerLevel_Mythicite_Desc,
                    config => (int)config.RadiusAtEachPowerLevel[5],
                    (config, value) => config.RadiusAtEachPowerLevel[5] = (uint)value,
                    () => Config.Pick,
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
                () => Config.Pick,
                1,
                10,
                id: "RadiusAtEachPowerLevel.Pick.Reaching");
    }
}
