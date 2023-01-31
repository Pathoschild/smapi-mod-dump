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

/// <summary>Wrapper for <see cref="IInputEvents.ButtonReleased"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ButtonReleasedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ButtonReleasedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ButtonReleasedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.ButtonReleased += this.OnButtonReleased;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Input.ButtonReleased -= this.OnButtonReleased;
    }

    /// <inheritdoc cref="IInputEvents.ButtonReleased"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnButtonReleasedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnButtonReleased"/>
    protected abstract void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e);
}
