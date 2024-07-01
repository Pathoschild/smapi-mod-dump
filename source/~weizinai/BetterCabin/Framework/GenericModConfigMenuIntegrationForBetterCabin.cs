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
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal class GenericModConfigMenuIntegrationForBetterCabin
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForBetterCabin(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            // 拜访小屋信息
            .AddSectionTitle(I18n.Config_VisitCabinInfo_Name)
            .AddBoolOption(
                config => config.VisitCabinInfo,
                (config, value) => config.VisitCabinInfo = value,
                I18n.Config_VisitCabinInfo_Name,
                I18n.Config_VisitCabinInfo_Tooltip
            )
            // 小屋主人名字标签
            .AddSectionTitle(I18n.Config_CabinOwnerNameTag_Name)
            .AddBoolOption(
                config => config.CabinOwnerNameTag,
                (config, value) => config.CabinOwnerNameTag = value,
                I18n.Config_CabinOwnerNameTag_Name,
                I18n.Config_CabinOwnerNameTag_Tooltip
            )
            .AddNumberOption(
                config => config.NameTagXOffset,
                (config, value) => config.NameTagXOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.NameTagYOffset,
                (config, value) => config.NameTagYOffset = value,
                I18n.Config_YOffset_Name
            )
            // 总在线时间标签
            .AddSectionTitle(I18n.Config_TotalOnlineTimeTag_Name)
            .AddBoolOption(
                config => config.TotalOnlineTime.Enable,
                (config, value) => config.TotalOnlineTime.Enable = value,
                I18n.Config_TotalOnlineTimeTag_Name,
                I18n.Config_TotalOnlineTimeTag_Tooltip
            )
            .AddNumberOption(
                config => config.TotalOnlineTime.XOffset,
                (config, value) => config.TotalOnlineTime.XOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.TotalOnlineTime.YOffset,
                (config, value) => config.TotalOnlineTime.YOffset = value,
                I18n.Config_YOffset_Name
            )
            // 上次在线时间标签
            .AddSectionTitle(I18n.Config_LastOnlineTimeTag_Name)
            .AddBoolOption(
                config => config.LastOnlineTime.Enable,
                (config, value) => config.LastOnlineTime.Enable = value,
                I18n.Config_LastOnlineTimeTag_Name,
                I18n.Config_LastOnlineTimeTag_Tooltip
            )
            .AddNumberOption(
                config => config.LastOnlineTime.XOffset,
                (config, value) => config.LastOnlineTime.XOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.LastOnlineTime.YOffset,
                (config, value) => config.LastOnlineTime.YOffset = value,
                I18n.Config_YOffset_Name
            )
            // 小屋面板
            .AddSectionTitle(I18n.Config_CabinMenu_Name)
            .AddBoolOption(
                config => config.CabinMenu,
                (config, value) => config.CabinMenu = value,
                I18n.Config_CabinMenu_Name,
                I18n.Config_CabinMenu_Tooltip
            )
            .AddKeybindList(
                config => config.CabinMenuKeybind,
                (config, value) => config.CabinMenuKeybind = value,
                I18n.Config_CabinMenuKeybind_Name
            )
            .AddBoolOption(
                config => config.BuildCabinContinually,
                (config, value) => config.BuildCabinContinually = value,
                I18n.Config_BuildCabinContinually_Name
            )
            // 上锁小屋
            .AddSectionTitle(I18n.Config_LockCabin_Name)
            .AddBoolOption(
                config => config.LockCabin,
                (config, value) => config.LockCabin = value,
                I18n.Config_LockCabin_Name,
                I18n.Config_LockCabin_Tooltip
            )
            .AddKeybindList(
                config => config.LockCabinKeybind,
                (config, value) => config.LockCabinKeybind = value,
                I18n.Config_LockCabinKeybind_Name
            )
            // 删除小屋主人
            .AddSectionTitle(I18n.Config_ResetCabin_Name)
            .AddBoolOption(
                config => config.ResetCabinPlayer,
                (config, value) => config.ResetCabinPlayer = value,
                I18n.Config_ResetCabin_Name,
                I18n.Config_ResetCabin_Tooltip
            )
            .AddKeybindList(
                config => config.ResetCabinPlayerKeybind,
                (config, value) => config.ResetCabinPlayerKeybind = value,
                I18n.Config_ResetCabinKeybind_Name
            )
            ;
    }
}