/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreated"/> allowing dynamic enabling / disabling.</summary>
public abstract class SaveCreatedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SaveCreatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SaveCreatedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.SaveCreated += this.OnSaveCreated;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.SaveCreated -= this.OnSaveCreated;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveCreated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnSaveCreatedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnSaveCreated"/>
    protected abstract void OnSaveCreatedImpl(object? sender, SaveCreatedEventArgs e);
}
