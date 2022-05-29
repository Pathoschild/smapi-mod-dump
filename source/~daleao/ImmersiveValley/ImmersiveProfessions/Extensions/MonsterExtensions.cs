/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
public static class MonsterExtensions
{
    /// <summary>Whether the monster instance is close enough to see the given player.</summary>
    /// <param name="player">The target player.</param>
    public static bool IsWithinPlayerThreshold(this Monster monster, Farmer player = null)
    {
        player ??= Game1.player;
        return monster.DistanceToCharacter(player) <= monster.moveTowardPlayerThreshold.Value;
    }

    /// <summary>Whether the monster is an instance of <see cref="GreenSlime"/> or <see cref="BigSlime"/></summary>
    public static bool IsSlime(this Monster monster)
    {
        return monster is GreenSlime or BigSlime;
    }
}