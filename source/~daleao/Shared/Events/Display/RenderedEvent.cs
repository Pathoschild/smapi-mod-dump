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

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendered"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.Rendered += this.OnRendered;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.Rendered -= this.OnRendered;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisplayEvents.Rendered"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRendered"/>
    protected abstract void OnRenderedImpl(object? sender, RenderedEventArgs e);
}
