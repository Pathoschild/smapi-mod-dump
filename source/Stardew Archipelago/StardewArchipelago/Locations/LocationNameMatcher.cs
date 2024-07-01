/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Locations.GingerIsland.Parrots;

namespace StardewArchipelago.Locations
{
    public class LocationNameMatcher
    {
        private Dictionary<string, string[]> _wordFilterCache;

        private static readonly Dictionary<string, string[]> _exceptions = new()
        {
            { "Snail", new[] { IslandNorthInjections.AP_PROF_SNAIL_CAVE } },
            {
                "Stone",
                new[]
                {
                    FestivalLocationNames.STRENGTH_GAME, "Lemon Stone",
                    "Ocean Stone", "Fairy Stone", "Swirl Stone"
                }
            },
            { "Trash", new[] { "Trash Can Upgrade" } },
            { "Hardwood", new[] { "Hardwood Display" } },
            { "Anchor", new[] { "Boat Anchor" } },
            { "Carp", new[] { "Midnight Carp", "Scorpion Carp", "Mutant Carp" } },
            { "Salmon", new[] { "Void Salmon", "King Salmon" } },
            { "Egg", new[] { "Dinosaur Egg", "Golden Egg", "Void Egg", "Thunder Egg" } },
            { "Clay", new[] { "Land Of Clay" } },
            { "Slime", new[] { "Slime Hutch" } },
        };

        public LocationNameMatcher()
        {
            _wordFilterCache = new Dictionary<string, string[]>();
        }

        public IEnumerable<string> GetAllLocationsMatching(IEnumerable<string> allLocations, string filter)
        {
            return allLocations.Where(x => x.Contains(filter, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> GetAllLocationsStartingWith(IEnumerable<string> allLocations, string prefix)
        {
            return allLocations.Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }

        public string[] GetAllLocationsContainingWord(IEnumerable<string> allLocations, string wordFilter)
        {
            if (_wordFilterCache.ContainsKey(wordFilter))
            {
                return _wordFilterCache[wordFilter];
            }

            var filteredLocations = FilterForWord(GetAllLocationsMatching(allLocations, wordFilter), wordFilter);

            if (_exceptions.ContainsKey(wordFilter))
            {
                foreach (var exceptionName in _exceptions[wordFilter])
                {
                    filteredLocations = filteredLocations.Where(x =>
                        !x.Contains(exceptionName, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            var filteredArray = filteredLocations.ToArray();
            _wordFilterCache.Add(wordFilter, filteredArray);
            return filteredArray;
        }

        public bool IsAnyLocationMatching(IEnumerable<string> allLocations, string filter)
        {
            return GetAllLocationsMatching(allLocations, filter).Any();
        }

        public bool IsAnyLocationStartingWith(IEnumerable<string> allLocations, string prefix)
        {
            return GetAllLocationsStartingWith(allLocations, prefix).Any();
        }

        private static IEnumerable<string> FilterForWord(IEnumerable<string> locations, string filterWord)
        {
            foreach (var location in locations)
            {
                if (ItemIsRelevant(filterWord, location))
                {
                    yield return location;
                }
            }
        }

        private static bool ItemIsRelevant(string itemName, string locationName)
        {
            var startOfItemName = locationName.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase);
            if (startOfItemName == -1)
            {
                return false;
            }

            var charBefore = startOfItemName == 0 ? ' ' : locationName[startOfItemName - 1];
            var charAfter = locationName.Length <= startOfItemName + itemName.Length ? ' ' : locationName[startOfItemName + itemName.Length];
            return charBefore == ' ' && charAfter == ' ';
        }

        public void ClearCache()
        {
            _wordFilterCache.Clear();
        }
    }
}
