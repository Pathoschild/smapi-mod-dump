/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.GameLaunched"/> allowing dynamic enabling / disabling.</summary>
internal abstract class GameLaunchedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected GameLaunchedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.GameLaunched -= this.OnGameLaunched;
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.OnGameLaunchedImpl(sender, e);
        this.Dispose();
    }

    /// <inheritdoc cref="OnGameLaunched"/>
    protected abstract void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e);

    protected sealed override void OnEnabled()
    {
    }

    protected sealed override void OnDisabled()
    {
    }
}
