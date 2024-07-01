/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class GenericModConfigMenuIntegrationForMultiplayerModLimit
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForMultiplayerModLimit(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
            // 启用模组
            .AddBoolOption(
                config => config.EnableMod,
                (config, value) => config.EnableMod = value,
                I18n.Config_EnableMod_Name
            )
            // 踢出玩家延迟时间
            .AddNumberOption(
                config => config.KickPlayerDelayTime,
                (config, value) => config.KickPlayerDelayTime = value,
                I18n.Config_KickPlayerDelayTime_Name,
                I18n.Config_KickPlayerDelayTIme_Tooltip
            )
            .AddSectionTitle(I18n.Config_LimitSettingTitle_Name)
            // 要求SMAPI
            .AddBoolOption(
                config => config.RequireSMAPI,
                (config, value) => config.RequireSMAPI = value,
                I18n.Config_RequireSMAPI_Name,
                I18n.Config_RequireSMAPI_Tooltip
            )
            // 限制模式
            .AddTextOption(
                config => config.LimitMode.ToString(),
                (config, value) => config.LimitMode = Enum.Parse<LimitMode>(value),
                I18n.Config_LimitMode_Name,
                I18n.Config_LimitMode_Tooltip,
                new[] { "WhiteListMode", "BlackListMode" },
                value => value switch
                {
                    "WhiteListMode" => I18n.Config_LimitMode_WhiteListMode(),
                    "BlackListMode" => I18n.Config_LimitMode_BlackListMode(),
                    _ => value
                }
            )
            .AddSectionTitle(I18n.Config_ModListSelectedTitle_Name)
            .AddParagraph(I18n.Config_ModListSelected_Paragraph)
            // 选择的模组列表
            .AddTextOption(
                config => config.AllowedModListSelected,
                (config, value) => config.AllowedModListSelected = value,
                I18n.Config_AllowedModListSelected_Name,
                I18n.Config_AllowedModListSelected_Tooltip,
                this.configMenu.GetConfig().AllowedModList.Keys.ToArray()
            )
            .AddTextOption(
                config => config.RequiredModListSelected,
                (config, value) => config.RequiredModListSelected = value,
                I18n.Config_RequiredModListSelected_Name,
                I18n.Config_RequiredModListSelected_Tooltip,
                this.configMenu.GetConfig().RequiredModList.Keys.ToArray()
            )
            .AddTextOption(
                config => config.BannedModListSelected,
                (config, value) => config.BannedModListSelected = value,
                I18n.Config_BannedModListSelected_Name,
                I18n.Config_BannedModListSelected_Tooltip,
                this.configMenu.GetConfig().BannedModList.Keys.ToArray()
            )
            ;
    }
}