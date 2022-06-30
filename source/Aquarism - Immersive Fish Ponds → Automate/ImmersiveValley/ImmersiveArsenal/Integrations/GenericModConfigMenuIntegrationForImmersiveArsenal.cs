/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;
using System;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Arsenal.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveArsenal
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveArsenal(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu = new(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()
            .AddCheckbox(
                () => "Rebalanced Enchants",
                () => "Improves certain underwhelming enchantments.",
                config => config.RebalancedEnchants,
                (config, value) => config.RebalancedEnchants = value
            )
            .AddCheckbox(
                () => "Weapons Cost Stamina",
                () => "Weapons should cost energy to use.",
                config => config.WeaponsCostStamina,
                (config, value) => config.WeaponsCostStamina = value
            )
            .AddCheckbox(
                () => "Remove Slingshot Grace Period",
                () => "Projectiles should not be useless for the first 100ms.",
                config => config.RemoveSlingshotGracePeriod,
                (config, value) => config.RemoveSlingshotGracePeriod = value
            )
            .AddCheckbox(
                () => "Remove Defense Soft Cap",
                () => "Damage mitigation should not be soft-capped at 50%.",
                config => config.RemoveDefenseSoftCap,
                (config, value) => config.RemoveDefenseSoftCap = value
            )
            .AddCheckbox(
                () => "Truly Legendary Galaxy Sword",
                () => "The Galaxy Sword should not be so easy to get.",
                config => config.TrulyLegendaryGalaxySword,
                (config, value) => config.TrulyLegendaryGalaxySword = value
            );
    }
}