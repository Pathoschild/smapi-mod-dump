/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using Common.Events;
using Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PondGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondGameLaunchedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModEntry.ModHelper.ModRegistry;

        // add Generic Mod Config Menu integration
        if (registry.IsLoaded("spacechase0.GenericModConfigMenu"))
            new GenericModConfigMenuIntegrationForImmersivePonds(
                getConfig: () => ModEntry.Config,
                reset: () =>
                {
                    ModEntry.Config = new();
                    ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                },
                saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
                modRegistry: registry,
                manifest: ModEntry.Manifest
            ).Register();

        // add Immersive Professions integration
        if (registry.IsLoaded("DaLion.ImmersiveProfessions"))
            new ImmersiveProfessionsIntegration(registry).Register();
    }
}