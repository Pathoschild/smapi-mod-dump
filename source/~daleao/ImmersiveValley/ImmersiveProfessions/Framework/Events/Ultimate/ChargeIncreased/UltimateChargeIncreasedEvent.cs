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

internal class UltimateChargeIncreasedEvent : BaseEvent
{
    private readonly Action<object, IUltimateChargeIncreasedEventArgs> _OnChargeIncreasedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeIncreasedEvent(Action<object, IUltimateChargeIncreasedEventArgs> callback)
    {
        _OnChargeIncreasedImpl = callback;
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeIncreased(object sender, IUltimateChargeIncreasedEventArgs e)
    {
        if (enabled.Value) _OnChargeIncreasedImpl(sender, e);
    }
}