/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingHud"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingHudEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingHudEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderingHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        if (IsHooked) OnRenderingHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingHud" />
    protected abstract void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e);
}