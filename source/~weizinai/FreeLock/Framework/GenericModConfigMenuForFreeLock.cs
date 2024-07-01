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

namespace weizinai.StardewValleyMod.FreeLock.Framework;

internal class GenericModConfigMenuForFreeLock
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuForFreeLock(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.FreeLockKeybind,
                (config, value) => config.FreeLockKeybind = value,
                I18n.Config_FreeLockKeybind_Name
            )
            .AddNumberOption(
                config => config.MoveSpeed,
                (config, value) => config.MoveSpeed = value,
                I18n.Config_MoveSpeed_Name,
                I18n.Config_MoveSpeed_Tooltip
            )
            .AddNumberOption(
                config => config.MoveThreshold,
                (config, value) => config.MoveThreshold = value,
                I18n.Config_MoveThreshold_Name,
                I18n.Config_MoveThreshold_Tooltip
            )
            ;
    }
}