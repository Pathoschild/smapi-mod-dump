/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.TreasureHunts.Events;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntStartedEvent"/>.</summary>
public sealed class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntStartedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="profession">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="target">The coordinates of the target tile.</param>
    /// <param name="timeLimit">The time limit for the treasure hunt, in seconds.</param>
    internal TreasureHuntStartedEventArgs(Farmer player, TreasureHuntProfession profession, Vector2 target, uint timeLimit)
    {
        this.Player = player;
        this.Profession = profession;
        this.Target = target;
        this.TimeLimit = timeLimit;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntProfession Profession { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }

    /// <inheritdoc />
    public uint TimeLimit { get; }
}
