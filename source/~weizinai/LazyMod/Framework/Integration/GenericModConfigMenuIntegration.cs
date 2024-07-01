/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.LazyMod;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

// ReSharper disable once CheckNamespace
namespace weizinai.StardewValleyMod.Common.Integration;

/// <summary>Handles the other logic for integrating with the Generic Mod Config Menu mod.</summary>
/// <typeparam name="TConfig">The mod config type.</typeparam>
internal partial class GenericModConfigMenuIntegration<TConfig> where TConfig : new()
{
    public GenericModConfigMenuIntegration<TConfig> AddBaseAutomationConfig(Func<TConfig, BaseAutomationConfig> get, Func<string> text, Func<string>? tooltip, int minRange)
    {
        this.AddSectionTitle(text, tooltip);
        this.AddBoolOption(
            config => get(config).IsEnable,
            (config, value) => get(config).IsEnable = value,
            I18n.Config_Enable_Name
        );
        this.AddNumberOption(
            config => get(config).Range,
            (config, value) => get(config).Range = value,
            I18n.Config_Range_Name,
            null,
            minRange,
            3
        );

        return this;
    }
    
    public GenericModConfigMenuIntegration<TConfig> AddToolAutomationConfig(Func<TConfig, ToolAutomationConfig> get, Func<string> text, Func<string>? tooltip, int minRange)
    {
        this.AddBaseAutomationConfig(get, text, tooltip, minRange);
        this.AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return this;
    }
    
    public GenericModConfigMenuIntegration<TConfig> AddStaminaToolAutomationConfig(Func<TConfig, StaminaToolAutomationConfig> get, 
        Func<string> text, Func<string>? tooltip, int minRange)
    {
        this.AddBaseAutomationConfig(get, text, tooltip, minRange);
        this.AddNumberOption(
            config => get(config).StopStamina,
            (config, value) => get(config).StopStamina = value,
            I18n.Config_StopStamina_Name
        );
        this.AddBoolOption(
            config => get(config).FindToolFromInventory,
            (config, value) => get(config).FindToolFromInventory = value,
            I18n.Config_FindToolFromInventory_Name,
            I18n.Config_FindToolFromInventory_Tooltip
        );

        return this;
    }
}