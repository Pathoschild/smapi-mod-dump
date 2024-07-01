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
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Framework;

internal class GenericModConfigMenuIntegrationForAutoBreakGeode
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForAutoBreakGeode(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.configMenu.GetConfig().OpenConfigMenuKeybind.JustPressed() && Context.IsPlayerFree) this.configMenu.OpenMenu();
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;

        this.configMenu
            .Register()
            .AddKeybindList(
                config => config.OpenConfigMenuKeybind,
                (config, value) => config.OpenConfigMenuKeybind = value,
                I18n.Config_OpenConfigMenuKeybind_Name
            )
            .AddKeybindList(
                config => config.AutoBreakGeodeKeybind,
                (config, value) => config.AutoBreakGeodeKeybind = value,
                I18n.Config_AutoBreakGeodeKeybind_Name
            )
            .AddBoolOption(
                config => config.DrawBeginButton,
                (config, value) => config.DrawBeginButton = value,
                I18n.Config_DrawBeginButton_Name,
                I18n.Config_DrawBeginButton_Tooltip
            )
            .AddNumberOption(
                config => config.BreakGeodeSpeed,
                (config, value) => config.BreakGeodeSpeed = value,
                I18n.Config_BreakGeodeSpeed_Name
            );
    }
}