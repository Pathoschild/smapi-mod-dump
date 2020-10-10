/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmAutomation.Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace FarmAutomation.ItemCollector.Processors
{
    class MachinesProcessor
    {
        private readonly List<string> _machineNamesToProcess;
        private readonly List<string> _gameLocationsToSearch;
        private readonly bool _allowDiagonalConnectionsForAllItems;
        Dictionary<string, Dictionary<Vector2, Chest>> _connectedChestsCache = new Dictionary<string, Dictionary<Vector2, Chest>>();
        private readonly MaterialHelper _materialHelper;

        public bool AddBuildingsToLocations { get; set; }

        public int MuteWhileCollectingFromMachines { get; set; }

        public MachinesProcessor(List<string> machineNamesToProcess, List<string> gameLocationsToSearch, bool addBuildingsToLocations, bool allowDiagonalConnectionsForAllItems)
        {
            AddBuildingsToLocations = addBuildingsToLocations;
            _machineNamesToProcess = machineNamesToProcess;
            _gameLocationsToSearch = gameLocationsToSearch;
            _allowDiagonalConnectionsForAllItems = allowDiagonalConnectionsForAllItems;
            _gameLocationsToSearch.ForEach(gl => _connectedChestsCache.Add(gl, new Dictionary<Vector2, Chest>()));
            _materialHelper = new MaterialHelper();
            DailyReset();
        }

        public IEnumerable<GameLocation> GetLocations()
        {
            List<GameLocation> gameLocations = new List<GameLocation>();
            lock (_gameLocationsToSearch)
            {
                foreach (var locationName in _gameLocationsToSearch)
                {
                    var location = Game1.getLocationFromName(locationName);
                    if (location != null)
                    {
                        gameLocations.Add(location);

                        var farm = location as Farm;
                        if (farm != null && AddBuildingsToLocations)
                        {
                            gameLocations.AddRange(farm.buildings.Where(building => building?.indoors != null).Select(building => building.indoors));
                        }
                    }
                }
            }
            return gameLocations;
        }

        public void ValidateGameLocations()
        {
            var locations = string.Join(", ", Game1.locations.Select(l => l.Name));
            Log.Info($"Loading locations. These are all the currently known locations in the game:\r\n{locations}");

            lock (_gameLocationsToSearch)
            {
                foreach (var locationName in _gameLocationsToSearch.ToList())
                {
                    var location = Game1.getLocationFromName(locationName);
                    if (location == null)
                    {
                        Log.Error($"Could not find a location with the name of '{locationName}'");
                        _gameLocationsToSearch.Remove(locationName);
                    }
                }
            }
        }


        private void BuildCacheForLocation(GameLocation gameLocation)
        {
            if (gameLocation != null)
            {
                var cacheToAdd = new Dictionary<Vector2, Chest>();
                Log.Debug($"Starting search for connected locations at {LocationHelper.GetName(gameLocation)}");
                var items = ItemFinder.FindObjectsWithName(gameLocation, _machineNamesToProcess);
                foreach (var valuePair in items)
                {
                    Vector2 location = valuePair.Key;
                    if (cacheToAdd.ContainsKey(location))
                    {
                        //already found in another search
                        continue;
                    }

                    List<ConnectedTile> processedLocations = new List<ConnectedTile>
                    {
                        new ConnectedTile {Location = location, Object = valuePair.Value}
                    };

                    ItemFinder.FindConnectedLocations(gameLocation, location, processedLocations, _allowDiagonalConnectionsForAllItems);
                    var chest = processedLocations.FirstOrDefault(c => c.Chest != null)?.Chest;
                    foreach (var connectedLocation in processedLocations)
                    {
                        cacheToAdd.Add(connectedLocation.Location, chest);
                    }
                }
                lock (_connectedChestsCache)
                {
                    if (_connectedChestsCache.ContainsKey(LocationHelper.GetName(gameLocation)))
                    {
                        // already ran?
                        _connectedChestsCache.Remove(LocationHelper.GetName(gameLocation));
                    }
                    _connectedChestsCache.Add(LocationHelper.GetName(gameLocation), new Dictionary<Vector2, Chest>());
                    foreach (var cache in cacheToAdd)
                    {
                        _connectedChestsCache[LocationHelper.GetName(gameLocation)].Add(cache.Key, cache.Value);
                    }
                }
                Log.Debug($"Searched your {LocationHelper.GetName(gameLocation)} for machines to collect from and found a total of {_connectedChestsCache[LocationHelper.GetName(gameLocation)].Count} locations to look for");
            }
        }

        public void ProcessMachines()
        {
            if (_connectedChestsCache == null)
            {
                _connectedChestsCache = new Dictionary<string, Dictionary<Vector2, Chest>>();
                Parallel.ForEach(GetLocations(), BuildCacheForLocation);
            }
            if (MuteWhileCollectingFromMachines > 0)
            {
                SoundHelper.MuteTemporary(MuteWhileCollectingFromMachines);
            }
            foreach (var gameLocation in GetLocations())
            {
                MachineHelper.Who.currentLocation = gameLocation;
                lock (_connectedChestsCache)
                {
                    if (!_connectedChestsCache.ContainsKey(LocationHelper.GetName(gameLocation)))
                    {
                        // cache got invalidated
                        BuildCacheForLocation(gameLocation);
                    }
                }
                foreach (var valuePair in _connectedChestsCache[LocationHelper.GetName(gameLocation)])
                {
                    Vector2 location = valuePair.Key;
                    Chest connectedChest = valuePair.Value;
                    if (connectedChest == null)
                    {
                        // no chest connected
                        continue;
                    }
                    if (!gameLocation.objects.ContainsKey(location))
                    {
                        // skip connection without objects like floortiles etc
                        continue;
                    }
                    if (!_machineNamesToProcess.Contains(gameLocation.objects[location].Name))
                    {
                        continue;
                    }
                    MachineHelper.ProcessMachine(gameLocation.objects[location], connectedChest, _materialHelper);
                }
            }
        }

        public void DailyReset()
        {
            if (Game1.hasLoadedGame)
            {
                MachineHelper.DailyReset();
            }
            _connectedChestsCache = null;
        }

        public void InvalidateCacheForLocation(GameLocation location)
        {
            if (_connectedChestsCache != null && _connectedChestsCache.ContainsKey(LocationHelper.GetName(location)))
            {
                _connectedChestsCache.Remove(LocationHelper.GetName(location));
            }
        }
    }
}
