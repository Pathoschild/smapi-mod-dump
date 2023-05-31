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
using StardewValley;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityChangedEvent"/>.</summary>
internal class ToxicityChangedEventArgs : EventArgs, IToxicityChangedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ToxicityChangedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old toxicity value.</param>
    /// <param name="newValue">The old charge value.</param>
    internal ToxicityChangedEventArgs(Farmer player, double oldValue, double newValue)
    {
        this.Player = player;
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }

    /// <inheritdoc />
    public double OldValue { get; }
}
