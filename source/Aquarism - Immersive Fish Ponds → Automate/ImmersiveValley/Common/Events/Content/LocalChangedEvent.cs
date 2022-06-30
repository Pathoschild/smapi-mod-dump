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

/// <summary>Wrapper for <see cref="IContentEvents.LocaleChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LocaleChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LocaleChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IContentEvents.LocaleChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    {
        if (IsHooked) OnLocaleChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLocaleChanged" />
    protected abstract void OnLocaleChangedImpl(object? sender, LocaleChangedEventArgs e);
}