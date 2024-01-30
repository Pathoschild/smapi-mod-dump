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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewModdingAPI;

namespace StardewArchipelago.Locations
{
    public class LocationChecker
    {
        private static IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private readonly LocationNameMatcher _locationNameMatcher;
        private Dictionary<string, long> _checkedLocations;
        private Dictionary<string, string[]> _wordFilterCache;

        public LocationChecker(IMonitor monitor, ArchipelagoClient archipelago, List<string> locationsAlreadyChecked)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationNameMatcher = new LocationNameMatcher();
            _checkedLocations = locationsAlreadyChecked.ToDictionary(x => x, x => (long)-1);
            _wordFilterCache = new Dictionary<string, string[]>();
        }

        public List<string> GetAllLocationsAlreadyChecked()
        {
            return _checkedLocations.Keys.ToList();
        }

        public bool IsLocationChecked(string locationName)
        {
            return _checkedLocations.ContainsKey(locationName);
        }

        public bool IsLocationNotChecked(string locationName)
        {
            return !IsLocationChecked(locationName);
        }

        public bool IsLocationMissing(string locationName)
        {
            return _archipelago.LocationExists(locationName) && IsLocationNotChecked(locationName);
        }

        public IReadOnlyCollection<long> GetAllMissingLocations()
        {
            return _archipelago.GetAllMissingLocations();
        }

        public IReadOnlyCollection<string> GetAllMissingLocationNames()
        {
            return _archipelago.GetAllMissingLocations().Select(x => _archipelago.GetLocationName(x)).ToArray();
        }

        public void AddCheckedLocation(string locationName)
        {
            if (_checkedLocations.ContainsKey(locationName))
            {
                return;
            }

            var locationId = _archipelago.GetLocationId(locationName);

            if (locationId == -1)
            {
                var alternateName = GetAllLocationsNotChecked().FirstOrDefault(x => x.Equals(locationName, StringComparison.InvariantCultureIgnoreCase));
                if (alternateName == null)
                {
                    _monitor.Log($"Location \"{locationName}\" could not be converted to an Archipelago id", LogLevel.Error);
                    return;
                }

                locationId = _archipelago.GetLocationId(alternateName);
                _monitor.Log($"Location \"{locationName}\" not found, checking location \"{alternateName}\" instead", LogLevel.Warn);
            }

            _checkedLocations.Add(locationName, locationId);
            _wordFilterCache.Clear();
            SendAllLocationChecks();
            GoalCodeInjection.CheckAllsanityGoalCompletion();
        }

        public void SendAllLocationChecks()
        {
            if (!_archipelago.IsConnected)
            {
                return;
            }

            TryToIdentifyUnknownLocationNames();

            var allCheckedLocations = new List<long>();
            allCheckedLocations.AddRange(_checkedLocations.Values);

            allCheckedLocations = allCheckedLocations.Distinct().Where(x => x > -1).ToList();

            _archipelago.ReportCheckedLocations(allCheckedLocations.ToArray());
        }

        public void VerifyNewLocationChecksWithArchipelago()
        {
            var allCheckedLocations = _archipelago.GetAllCheckedLocations();
            foreach (var (locationName, locationId) in allCheckedLocations)
            {
                if (!_checkedLocations.ContainsKey(locationName))
                {
                    _checkedLocations.Add(locationName, locationId);
                    _wordFilterCache.Clear();
                }
            }
        }

        private void TryToIdentifyUnknownLocationNames()
        {
            foreach (var locationName in _checkedLocations.Keys)
            {
                if (_checkedLocations[locationName] > -1)
                {
                    continue;
                }

                var locationId = _archipelago.GetLocationId(locationName);
                if (locationId == -1)
                {
                    continue;
                }

                _checkedLocations[locationName] = locationId;
            }
        }

        public void ForgetLocations(IEnumerable<string> locations)
        {
            foreach (var location in locations)
            {
                if (!_checkedLocations.ContainsKey(location))
                {
                    continue;
                }

                _checkedLocations.Remove(location);
                _wordFilterCache.Clear();
            }
        }

        public IEnumerable<string> GetAllLocationsNotChecked()
        {
            if (!_archipelago.IsConnected)
            {
                return Enumerable.Empty<string>();
            }

            return _archipelago.Session.Locations.AllMissingLocations.Select(_archipelago.GetLocationName);
        }

        public IEnumerable<string> GetAllLocationsNotChecked(string filter)
        {
            return _locationNameMatcher.GetAllLocationsMatching(GetAllLocationsNotChecked(), filter);
        }

        public IEnumerable<string> GetAllLocationsNotCheckedStartingWith(string prefix)
        {
            return _locationNameMatcher.GetAllLocationsStartingWith(GetAllLocationsNotChecked(), prefix);
        }

        public string[] GetAllLocationsNotCheckedContainingWord(string wordFilter)
        {
            return _locationNameMatcher.GetAllLocationsContainingWord(GetAllLocationsNotChecked(), wordFilter);
        }

        public bool IsAnyLocationNotChecked(string filter)
        {
            return _locationNameMatcher.IsAnyLocationMatching(GetAllLocationsNotChecked(), filter);
        }

        public bool IsAnyLocationNotCheckedStartingWith(string prefix)
        {
            return _locationNameMatcher.IsAnyLocationStartingWith(GetAllLocationsNotChecked(), prefix);
        }
    }
}
