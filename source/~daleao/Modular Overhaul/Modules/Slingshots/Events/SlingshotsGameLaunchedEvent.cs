/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotsGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotsGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotsGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        OverhaulModule.Slingshots.RegisterIntegrations();
    }
}
