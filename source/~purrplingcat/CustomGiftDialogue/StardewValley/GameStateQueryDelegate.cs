/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using System;

namespace StardewValley.Delegates
{
    /// <summary>A <see cref="T:StardewValley.GameStateQuery" /> query resolver.</summary>
    /// <param name="query">The game state query split by space, including the query key.</param>
    /// <param name="location">The location for which to check the query.</param>
    /// <param name="player">The player for which to check the query.</param>
    /// <param name="item">The item for which to check the query, or <c>null</c> if not applicable.</param>
    /// <param name="random">The RNG to use for randomization.</param>
    /// <returns>Returns whether the query matches.</returns>
    public delegate bool GameStateQueryDelegate(string[] query, GameLocation location, Farmer player, Item item, Random random);
}