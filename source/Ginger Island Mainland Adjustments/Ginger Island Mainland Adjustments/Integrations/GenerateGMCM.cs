/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using AtraShared.Integrations;
using GingerIslandMainlandAdjustments.Configuration;

namespace GingerIslandMainlandAdjustments.Integrations;

/// <summary>
/// Class that generates the GMCM for this mod.
/// </summary>
internal static class GenerateGMCM
{
    /// <summary>
    /// Generates the GMCM for this mod.
    /// </summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="translation">The translation helper.</param>
    internal static void Build(IManifest manifest, ITranslationHelper translation)
    {
        GMCMHelper helper = new(Globals.ModMonitor, translation, Globals.ModRegistry, manifest);
        if (!helper.TryGetAPI())
        {
            return;
        }

        helper.Register(
                reset: () => Globals.Config = new ModConfig(),
                save: () => Globals.Helper.WriteConfig(Globals.Config))
            .AddParagraph(I18n.ModDescription)
            .AddBoolOption(
                name: I18n.Config_EnforceGITiming_Title,
                getValue: () => Globals.Config.EnforceGITiming,
                setValue: value => Globals.Config.EnforceGITiming = value,
                tooltip: I18n.Config_EnforceGITiming_Description)
            .AddEnumOption(
                name: I18n.Config_WearIslandClothing_Title,
                getValue: () => Globals.Config.WearIslandClothing,
                setValue: value => Globals.Config.WearIslandClothing = value,
                tooltip: I18n.Config_WearIslandClothing_Description)
            .AddBoolOption(
                name: I18n.Config_Scheduler_Title,
                getValue: () => Globals.Config.UseThisScheduler,
                setValue: value => Globals.Config.UseThisScheduler = value,
                tooltip: I18n.Config_Scheduler_Description)
            .AddParagraph(I18n.Config_Scheduler_Otheroptions)
            .AddNumberOption(
                name: I18n.Config_Capacity_Title,
                getValue: () => Globals.Config.Capacity,
                setValue: value => Globals.Config.Capacity = value,
                tooltip: I18n.Config_Capacity_Description,
                min: 0,
                max: 12)
            .AddNumberOption(
                name: I18n.Config_GroupChance_Title,
                getValue: () => Globals.Config.GroupChance,
                setValue: value => Globals.Config.GroupChance = value,
                tooltip: I18n.Config_GroupChance_Description,
                formatValue: TwoPlaceFixedPoint,
                min: 0f,
                max: 1f,
                interval: 0.01f)
            .AddNumberOption(
                name: I18n.Config_ExplorerChance_Title,
                getValue: () => Globals.Config.ExplorerChance,
                setValue: value => Globals.Config.ExplorerChance = value,
                tooltip: I18n.Config_ExplorerChance_Description,
                formatValue: TwoPlaceFixedPoint,
                min: 0f,
                max: 1f,
                interval: 0.01f)
            .AddEnumOption(
                name: I18n.Config_GusDay_Title,
                getValue: () => Globals.Config.GusDay,
                setValue: value => Globals.Config.GusDay = value,
                tooltip: I18n.Config_GusDay_Description)
            .AddNumberOption(
                name: I18n.Config_GusChance_Title,
                getValue: () => Globals.Config.GusChance,
                setValue: value => Globals.Config.GusChance = value,
                tooltip: I18n.Config_GusChance_Description,
                formatValue: TwoPlaceFixedPoint,
                min: 0f,
                max: 1f,
                interval: 0.01f)
            .AddBoolOption(
                name: I18n.Config_AllowWilly_Title,
                getValue: () => Globals.Config.AllowWilly,
                setValue: value => Globals.Config.AllowWilly = value,
                tooltip: I18n.Config_AllowWilly_Description)
            .AddBoolOption(
                name: I18n.Config_AllowSandy_Title,
                getValue: () => Globals.Config.AllowSandy,
                setValue: value => Globals.Config.AllowSandy = value,
                tooltip: I18n.Config_AllowSandy_Description)
            .AddBoolOption(
                name: I18n.Config_AllowGeorgeAndEvelyn_Title,
                getValue: () => Globals.Config.AllowGeorgeAndEvelyn,
                setValue: value => Globals.Config.AllowGeorgeAndEvelyn = value,
                tooltip: I18n.Config_AllowGeorgeAndEvelyn_Description);
    }

    private static string TwoPlaceFixedPoint(float f) => $"{f:f2}";
}