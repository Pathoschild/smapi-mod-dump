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

/// <summary>Wrapper for a <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> which executes exactly once.</summary>
/// <remarks>Useful for set-up code which requires third-party mod integrations to be registered.</remarks>
internal abstract class FirstSecondUpdateTickedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="FirstSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected FirstSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.OneSecondUpdateTicked += this.OnFirstSecondUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.OneSecondUpdateTicked -= this.OnFirstSecondUpdateTicked;
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFirstSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        this.OnFirstSecondUpdateTickedImpl(sender, e);
        this.Dispose();
    }

    /// <inheritdoc cref="OnFirstSecondUpdateTicked"/>
    protected abstract void OnFirstSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e);

    protected sealed override void OnEnabled()
    {
    }

    protected sealed override void OnDisabled()
    {
    }
}
