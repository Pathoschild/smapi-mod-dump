/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;

namespace StardewArchipelago.Locations
{
    public class LocationChecker
    {
        private static IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private Dictionary<string, long> _checkedLocations;

        public LocationChecker(IMonitor monitor, ArchipelagoClient archipelago, List<string> locationsAlreadyChecked)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _checkedLocations = locationsAlreadyChecked.ToDictionary(x => x, x => (long)-1);
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

        public bool IsLocationMissingAndExists(string locationName)
        {
            return _archipelago.LocationExists(locationName) && IsLocationNotChecked(locationName);
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
                _monitor.Log($"Location \"{locationName}\" could not be converted to an Archipelago id", LogLevel.Error);
            }

            _checkedLocations.Add(locationName, locationId);
            SendAllLocationChecks();
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
            }
        }
    }
}
