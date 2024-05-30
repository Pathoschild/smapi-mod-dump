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

/// <summary>The arguments for a <see cref="LimitFullyChargedEvent"/>.</summary>
public sealed class LimitFullyChargedEventArgs : EventArgs, ILimitFullyChargedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitFullyChargedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal LimitFullyChargedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
