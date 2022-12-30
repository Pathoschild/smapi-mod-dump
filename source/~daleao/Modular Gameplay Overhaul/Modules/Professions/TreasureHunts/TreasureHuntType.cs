/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

/// <summary>
///     The type of <see cref="ITreasureHunt"/>; either <see cref="Profession.Scavenger"/> or
///     <see cref="Profession.Prospector"/>.
/// </summary>
public enum TreasureHuntType
{
    /// <summary>A <see cref="Profession.Scavenger"/> hunt.</summary>
    Scavenger,

    /// <summary>A <see cref="Profession.Prospector"/> hunt.</summary>
    Prospector,
}
