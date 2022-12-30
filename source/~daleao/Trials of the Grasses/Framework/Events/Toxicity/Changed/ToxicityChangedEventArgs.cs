/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;
using System;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityChangedEvent"/>.</summary>
internal class ToxicityChangedEventArgs : EventArgs, IToxicityChangedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }

    /// <inheritdoc />
    public double OldValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old toxicity value.</param>
    /// <param name="newValue">The old charge value.</param>
    internal ToxicityChangedEventArgs(Farmer player, double oldValue, double newValue)
    {
        Player = player;
        OldValue = oldValue;
        NewValue = newValue;
    }
}