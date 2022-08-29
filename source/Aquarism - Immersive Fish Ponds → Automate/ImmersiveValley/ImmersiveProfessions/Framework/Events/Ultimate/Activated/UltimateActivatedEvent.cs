/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"> is activated.</summary>
internal sealed class UltimateActivatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateActivatedEventArgs> _OnActivatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    internal UltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback, bool alwaysEnabled = false)
        : base(ModEntry.Events)
    {
        _OnActivatedImpl = callback;
        AlwaysEnabled = alwaysEnabled;
    }

    /// <summary>Raised when a player activates the combat <see cref="Ultimate.IUltimate"/>.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnActivated(object? sender, IUltimateActivatedEventArgs e)
    {
        if (IsEnabled) _OnActivatedImpl(sender, e);
    }
}