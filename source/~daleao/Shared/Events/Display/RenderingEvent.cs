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

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendering"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.Rendering += this.OnRendering;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.Rendering -= this.OnRendering;
    }

    /// <inheritdoc cref="IDisplayEvents.Rendering"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRendering(object? sender, RenderingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRendering"/>
    protected abstract void OnRenderingImpl(object? sender, RenderingEventArgs e);
}
