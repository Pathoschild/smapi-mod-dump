/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>The arguments for a <see cref="LimitChargeInitiatedEvent"/>.</summary>
internal sealed class LimitChargeInitiatedEventArgs : EventArgs, ILimitChargeInitiatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitChargeInitiatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="newValue">The new charge value.</param>
    internal LimitChargeInitiatedEventArgs(Farmer player, double newValue)
    {
        this.Player = player;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }
}
