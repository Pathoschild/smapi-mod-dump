/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntEndedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets determines whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntType Type { get; }

    /// <summary>Gets a value indicating whether determines whether the player successfully discovered the treasure.</summary>
    bool TreasureFound { get; }
}
