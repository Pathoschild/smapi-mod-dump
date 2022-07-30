/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Helpers;

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

/// <summary>
///     Provides location functions across mods.
/// </summary>
internal static class LocationHelper
{
    /// <inheritdoc cref="IMultiplayerHelper" />
    public static IMultiplayerHelper? Multiplayer { get; set; }

    /// <summary>
    ///     Gets all accessible game locations and sub-locations
    /// </summary>
    public static IEnumerable<GameLocation> AllLocations
    {
        get
        {
            IEnumerable<GameLocation> IterateLocations(IEnumerable<GameLocation>? locations = null, HashSet<GameLocation>? excluded = null)
            {
                locations ??= Context.IsMainPlayer ? Game1.locations : LocationHelper.Multiplayer!.GetActiveLocations();
                excluded ??= new();

                foreach (var location in locations)
                {
                    if (excluded.Contains(location))
                    {
                        continue;
                    }

                    excluded.Add(location);
                    yield return location;

                    if (location is BuildableGameLocation buildableGameLocation)
                    {
                        var indoors = buildableGameLocation.buildings
                                                           .Select(building => building.indoors.Value)
                                                           .Where(indoors => indoors is not null);
                        foreach (var indoor in IterateLocations(indoors, excluded))
                        {
                            yield return indoor;
                        }
                    }
                }
            }

            foreach (var location in IterateLocations())
            {
                yield return location;
            }
        }
    }
}