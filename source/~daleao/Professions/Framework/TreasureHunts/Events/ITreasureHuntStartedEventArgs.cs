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

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntStartedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntProfession Profession { get; }

    /// <summary>Gets the coordinates of the target tile.</summary>
    Vector2 Target { get; }

    /// <summary>Gets the time limit for the treasure hunt, in seconds.</summary>
    public uint TimeLimit { get; }
}
