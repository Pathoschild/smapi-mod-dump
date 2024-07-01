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

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class GenericModConfigMenuForSpectatorMode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuForSpectatorMode(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            // 旁观者模式
            .AddSectionTitle(I18n.Config_SpectatorModeTitle_Name)
            .AddKeybindList(
                config => config.SpectateLocationKey,
                (config, value) => config.SpectateLocationKey = value,
                I18n.Config_SpectateLocationKey_Name
            )
            .AddKeybindList(
                config => config.SpectatePlayerKey,
                (config, value) => config.SpectatePlayerKey = value,
                I18n.Config_SpectatePlayerKey_Name
            )
            .AddKeybindList(
                config => config.ToggleStateKey,
                (config, value) => config.ToggleStateKey = value,
                I18n.Config_ToggleStateKey_Name
            )
            .AddNumberOption(
                config => config.MoveSpeed,
                (config, value) => config.MoveSpeed = value,
                I18n.Config_MoveSpeed_Name
            )
            .AddNumberOption(
                config => config.MoveThreshold,
                (config, value) => config.MoveThreshold = value,
                I18n.Config_MoveThreshold_Name
            )
            // 轮播玩家
            .AddSectionTitle(I18n.Config_RotatePlayerTitle_Name)
            .AddKeybindList(
                config => config.RotatePlayerKey,
                (config, value) => config.RotatePlayerKey = value,
                I18n.Config_RotatePlayerKey_Name
            )
            .AddNumberOption(
                config => config.RotationInterval,
                (config, value) => config.RotationInterval = value,
                I18n.Config_RotationInterval_Name
            )
            ;
    }
}