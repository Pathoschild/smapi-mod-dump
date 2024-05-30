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

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedActiveMenu"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderedActiveMenuEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderedActiveMenuEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedActiveMenuEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisplayEvents.RenderedActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedActiveMenuImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRenderedActiveMenu"/>
    protected abstract void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e);
}
