/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Runtime.CompilerServices;
using Common.Integrations;
using StardewModdingAPI;

namespace FastControlInput.Framework;

internal class GenericModConfigMenuIntegrationForFastControlInput
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForFastControlInput(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            .AddNumberOption(
                config => config.ActionButton,
                (config, value) => config.ActionButton = value,
                I18n.Config_ActionButton_Name,
                I18n.Config_ActionButton_Tooltip,
                1f,
                5f,
                0.25f
            )
            .AddNumberOption(
                config => config.UseToolButton,
                (config, value) => config.UseToolButton = value,
                I18n.Config_UseToolButton_Name,
                I18n.Config_UseToolButton_Tooltip,
                1f,
                5f,
                0.25f
            );
    }
}