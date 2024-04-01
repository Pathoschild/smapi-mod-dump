/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FindAnything.Framework.Services.Finders;

using Microsoft.Xna.Framework;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.FindAnything;
using StardewMods.FindAnything.Framework.Models;

internal sealed class ObjectFinder
{
    public ObjectFinder(IEventSubscriber eventSubscriber) =>
        eventSubscriber.Subscribe<ISearchSubmitted>(this.OnSearchSubmitted);

    private void OnSearchSubmitted(ISearchSubmitted e)
    {
        foreach (var obj in e.Location.Objects.Values)
        {
            if (this.TryGetResult(obj, e.SearchTerm, out var result))
            {
                e.AddResult(result);
            }
        }
    }

    private bool TryGetResult(SObject obj, string searchTerm, [NotNullWhen(true)] out ISearchResult? result)
    {
        // Search by name
        if (obj.DisplayName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            result = new SearchResult(obj.DisplayName, Game1.mouseCursors, Rectangle.Empty, obj.TileLocation);
            return true;
        }

        // Search by context tags
        if (obj.GetContextTags().Any(tag => tag.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
        {
            result = new SearchResult(obj.DisplayName, Game1.mouseCursors, Rectangle.Empty, obj.TileLocation);
            return true;
        }

        result = null;
        return false;
    }
}