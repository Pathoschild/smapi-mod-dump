/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.Events;

using StardewMods.BetterChests.Framework.Interfaces;

/// <summary>Represents the event arguments for when the search term changes.</summary>
internal sealed class SearchChangedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="SearchChangedEventArgs" /> class.</summary>
    /// <param name="searchExpression">The search expression.</param>
    public SearchChangedEventArgs(ISearchExpression? searchExpression) => this.SearchExpression = searchExpression;

    /// <summary>Gets the search expression.</summary>
    public ISearchExpression? SearchExpression { get; }
}