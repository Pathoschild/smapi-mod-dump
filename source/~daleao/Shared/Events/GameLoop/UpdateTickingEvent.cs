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

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicking"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UpdateTickingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UpdateTickingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.UpdateTicking += this.OnUpdateTicking;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.UpdateTicking -= this.OnUpdateTicking;
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnUpdateTickingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnUpdateTicking"/>
    protected abstract void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e);
}
