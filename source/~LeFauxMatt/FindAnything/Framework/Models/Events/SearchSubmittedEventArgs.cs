/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FindAnything.Framework.Models.Events;

using StardewMods.Common.Services.Integrations.FindAnything;

/// <inheritdoc cref="ISearchSubmitted" />
internal sealed class SearchSubmittedEventArgs(string searchTerm, GameLocation location) : EventArgs, ISearchSubmitted
{
    /// <summary>Gets the search results.</summary>
    public List<ISearchResult> SearchResults { get; } = [];

    /// <inheritdoc />
    public string SearchTerm { get; } = searchTerm;

    /// <inheritdoc />
    public GameLocation Location { get; } = location;

    /// <inheritdoc />
    public void AddResult(ISearchResult result) => this.SearchResults.Add(result);
}