/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Integration;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

public class GenericModConfigMenuIntegrationForSomeMultiplayerFeature
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForSomeMultiplayerFeature(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;
        this.configMenu
            .Register()
            // 显示商店信息
            .AddSectionTitle(I18n.Config_ShowAccessShopInfo_Name)
            .AddBoolOption(
                config => config.ShowAccessShopInfo,
                (config, value) => config.ShowAccessShopInfo = value,
                I18n.Config_ShowAccessShopInfo_Name,
                I18n.Config_ShowAccessShopInfo_Tooltip
            )
            // 显示延迟玩家
            .AddSectionTitle(I18n.Config_ShowDelayedPlayer_Name)
            .AddBoolOption(
                config => config.ShowDelayedPlayer,
                (config, value) => config.ShowDelayedPlayer = value,
                I18n.Config_ShowDelayedPlayer_Name,
                I18n.Config_ShowDelayedPlayer_Tooltip
            )
            .AddNumberOption(
                config => config.ShowInterval,
                (config, value) => config.ShowInterval = value,
                I18n.Config_ShowIntervalName
            )
            .AddSectionTitle(I18n.Config_AutoSetIpConnection_Name)
            // 自动设置Ip连接
            .AddBoolOption(
                config => config.AutoSetIpConnection,
                (config, value) => config.AutoSetIpConnection = value,
                I18n.Config_AutoSetIpConnection_Name
            )
            .AddNumberOption(
                config => config.EnableTime,
                (config, value) => config.EnableTime = value,
                I18n.Config_EnableTime_Name,
                null,
                6,
                26
            )
            .AddNumberOption(
                config => config.DisableTime,
                (config, value) => config.DisableTime = value,
                I18n.Config_DisableTime_Name,
                null,
                6,
                26
            )
            .AddBoolOption(
                config => config.DisableWhenFestival,
                (config, value) => config.DisableWhenFestival = value,
                I18n.Config_DisableWhenFestival_Name
            )
            // 显示玩家数量
            .AddSectionTitle(I18n.Config_ShowPlayerCount_Name)
            .AddBoolOption(
                config => config.ShowPlayerCount,
                (config, value) => config.ShowPlayerCount = value,
                I18n.Config_ShowPlayerCount_Name,
                I18n.Config_ShowPlayerCount_Tooltip
            )
            // 显示提示
            .AddSectionTitle(I18n.Config_ShowTip_Name)
            .AddBoolOption(
                config => config.ShowTip,
                (config, value) => config.ShowTip = value,
                I18n.Config_ShowTip_Name,
                I18n.Config_ShowTip_Tooltip
            )
            .AddTextOption(
                config => config.TipText,
                (config, value) => config.TipText = value,
                I18n.Config_TipText_Name
            )
            // 踢出未准备玩家
            .AddSectionTitle(I18n.Config_KickUnreadyPlayer_Name)
            .AddBoolOption(
                config => config.KickUnreadyPlayer,
                (config, value) => config.KickUnreadyPlayer = value,
                I18n.Config_KickUnreadyPlayer_Name,
                I18n.Config_KickUnreadyPlayer_Tooltip
            )
            .AddKeybindList(
                config => config.KickUnreadyPlayerKey,
                (config, value) => config.KickUnreadyPlayerKey = value,
                I18n.Config_KickUnreadyPlayerKey_Name
            )
            // 版本限制
            .AddSectionTitle(I18n.Config_VersionLimit_Name)
            .AddBoolOption(
                config => config.VersionLimit,
                (config, value) => config.VersionLimit = value,
                I18n.Config_VersionLimit_Name,
                I18n.Config_VersionLimit_Tooltip
            )
            ;
    }
}