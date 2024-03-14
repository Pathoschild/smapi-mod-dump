/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass;

/// <summary>Contains miscellaneous helper methods.</summary>
public static class Utilities
{
    /*********
    ** Fields
    *********/
    /// <summary>The cached results of <see cref="ShouldForceGrassToDefault(string)"/>.</summary>
    private static readonly Dictionary<string, bool> ShouldForceGrassToDefaultCache = [];

    /// <summary>The cached results of <see cref="ContainsLocation(string, List{string})"/>.</summary>
    private static readonly Dictionary<ContainsLocationData, bool> ContainsLocationCache = [];


    /*********
    ** Public Methods
    *********/
    /// <summary>Gets whether grass should be forced to default for a specified location.</summary>
    /// <param name="locationName">The name of the location to check.</param>
    /// <returns><see langword="true"/>, if the grass should be default grass; otherwise, <see langword="false"/>.</returns>
    public static bool ShouldForceGrassToDefault(string locationName)
    {
        locationName = locationName.ToLower();

        // check if location has been cached
        if (ShouldForceGrassToDefaultCache.TryGetValue(locationName, out var forceDefaultGrass))
            return forceDefaultGrass;

        // calculate whether grass should be forced and cache value
        var whiteListLocations = ModEntry.Instance.Config.LocationsWhiteList ?? [];
        var blackListLocations = ModEntry.Instance.Config.LocationsBlackList ?? [];
        forceDefaultGrass = (whiteListLocations.Count > 0 && !Utilities.ContainsLocation(locationName, whiteListLocations))
                         || (blackListLocations.Count > 0 && Utilities.ContainsLocation(locationName, blackListLocations));

        ShouldForceGrassToDefaultCache[locationName] = forceDefaultGrass;
        return forceDefaultGrass;
    }

    /// <summary>Gets whether a list of locations contains a specified location.</summary>
    /// <param name="locationName">The name of the location to check if it's in <paramref name="locations"/>.</param>
    /// <param name="locations">The list of locations to check against.</param>
    /// <returns><see langword="true"/>, if <paramref name="locationName"/> is specified in <paramref name="locations"/>; otherwise, <see langword="false"/>.</returns>
    /// <remarks><paramref name="locations"/> can contain special syntax, these are: <c>"s:{value}"</c> to check if the <paramref name="locationName"/> starts with <c>value</c>, <c>"c:{value}"</c> to check if <paramref name="locationName"/> contains <c>value</c>, and <c>"e:{value}"</c> to check if <paramref name="locationName"/> ends with <c>value</c>.</remarks>
    public static bool ContainsLocation(string locationName, List<string> locations)
    {
        locationName = locationName.ToLower();
        var containsLocationData = new ContainsLocationData(locationName, locations);

        // check if value has been cached
        if (ContainsLocationCache.TryGetValue(containsLocationData, out var containsLocation))
            return containsLocation;

        // calculate whether the location is contained and cache value
        containsLocation = false;
        foreach (var location in locations.Select(location => location.ToLower()))
        {
            if (locationName == location
                || (location.StartsWith("s:") && locationName.StartsWith(location[2..]))
                || (location.StartsWith("c:") && locationName.Contains(location[2..]))
                || (location.StartsWith("e:") && locationName.EndsWith(location[2..])))
            {
                containsLocation = true;
                break;
            }
        }

        ContainsLocationCache[containsLocationData] = containsLocation;
        return containsLocation;
    }
}
