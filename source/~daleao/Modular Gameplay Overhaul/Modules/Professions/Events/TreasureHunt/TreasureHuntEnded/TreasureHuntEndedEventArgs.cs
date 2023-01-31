/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntEndedEvent"/>.</summary>
public sealed class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="type">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    internal TreasureHuntEndedEventArgs(Farmer player, TreasureHuntType type, bool found)
    {
        this.Player = player;
        this.Type = type;
        this.TreasureFound = found;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }
}
