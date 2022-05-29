/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

internal class UltimateActivatedEvent : BaseEvent
{
    private readonly Action<object, IUltimateActivatedEventArgs> _OnActivatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateActivatedEvent(Action<object, IUltimateActivatedEventArgs> callback)
    {
        _OnActivatedImpl = callback;
    }

    /// <summary>Raised when a player activates their combat Ultimate.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnActivated(object sender, IUltimateActivatedEventArgs e)
    {
        if (enabled.Value) _OnActivatedImpl(sender, e);
    }
}