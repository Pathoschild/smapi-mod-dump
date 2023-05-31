/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for a <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> which executes exactly once, after two seconds of game time has elapsed.</summary>
/// <remarks>Useful for set-up code which requires third-party mod integrations to be registered.</remarks>
internal abstract class SecondSecondUpdateTickedEvent : ManagedEvent
{
    private int _elapsed;

    /// <summary>Initializes a new instance of the <see cref="SecondSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SecondSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.OneSecondUpdateTicked += this.OnSecondSecondUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.OneSecondUpdateTicked -= this.OnSecondSecondUpdateTicked;
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnSecondSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (++this._elapsed < 2)
        {
            return;
        }

        this.OnSecondSecondUpdateTickedImpl(sender, e);
        this.Manager.Unmanage(this);
    }

    /// <inheritdoc cref="OnSecondSecondUpdateTicked"/>
    protected abstract void OnSecondSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e);

    protected sealed override void OnEnabled()
    {
    }

    protected sealed override void OnDisabled()
    {
    }
}
