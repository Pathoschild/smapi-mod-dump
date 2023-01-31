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

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingWorld"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderingWorldEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderingWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingWorldEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderingWorld += this.OnRenderingWorld;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderingWorld -= this.OnRenderingWorld;
    }

    /// <inheritdoc cref="IDisplayEvents.RenderingWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingWorld(object? sender, RenderingWorldEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderingWorldImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRenderingWorld"/>
    protected abstract void OnRenderingWorldImpl(object? sender, RenderingWorldEventArgs e);
}
