/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Alchemy.Events.Toxicity.Changed;

#region using directives

using System;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a player's Toxicity value changes.</summary>
internal class ToxicityChangedEvent : ManagedEvent
{
    protected readonly Action<object?, IToxicityChangedEventArgs> OnChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="ToxicityChangedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityChangedEvent(Action<object?, IToxicityChangedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this.OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChanged(object? sender, IToxicityChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnChargeInitiatedImpl(sender, e);
        }
    }
}
