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

/// <summary>Wrapper for <see cref="IInputEvents.MouseWheelScrolled"/> allowing dynamic enabling / disabling.</summary>
internal abstract class MouseWheelScrolledEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="MouseWheelScrolledEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected MouseWheelScrolledEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;
    }

    /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnMouseWheelScrolledImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnMouseWheelScrolled"/>
    protected abstract void OnMouseWheelScrolledImpl(object? sender, MouseWheelScrolledEventArgs e);
}
