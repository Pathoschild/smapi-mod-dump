/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Shared.Events;
using Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        // hard dependencies
        SpaceCoreIntegration.Instance!.Register();

        // soft dependencies or integrations
        JsonAssetsIntegration.Instance?.Register();
        StardewValleyExpandedIntegration.Instance?.Register();
        VanillaTweaksIntegration.Instance?.Register();
    }
}
