/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Interfaces;

/// <summary>Represents an interface for filtering items.</summary>
internal interface IItemFilter
{
    /// <summary>Determines whether the given item matches any filter conditions.</summary>
    /// <param name="item">The item to be checked.</param>
    /// <returns>True if the item matches, false otherwise.</returns>
    public bool MatchesFilter(Item item);
}