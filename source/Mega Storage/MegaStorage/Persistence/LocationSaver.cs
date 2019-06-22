using System.Collections.Generic;
using System.Linq;
using MegaStorage.Mapping;
using MegaStorage.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Persistence
{
    public class LocationSaver : ISaver
    {
        public string SaveDataKey => "LocationNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<GameLocation, Dictionary<Vector2, CustomChest>> _locationCustomChests;

        public LocationSaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveCustomChests()
        {
            _monitor.VerboseLog("LocationSaver: HideAndSaveCustomChests");
            _locationCustomChests = new Dictionary<GameLocation, Dictionary<Vector2, CustomChest>>();
            var deserializedChests = new List<DeserializedChest>();
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var customChestPositions = location.objects.Pairs.Where(x => x.Value is CustomChest).ToDictionary(pair => pair.Key, pair => (CustomChest)pair.Value);
                if (!customChestPositions.Any())
                    continue;
                var locationName = location.uniqueName?.Value ?? location.Name;
                _locationCustomChests.Add(location, customChestPositions);
                foreach (var customChestPosition in customChestPositions)
                {
                    var position = customChestPosition.Key;
                    var customChest = customChestPosition.Value;
                    var chest = customChest.ToChest();
                    location.objects[position] = chest;
                    var deserializedChest = customChest.ToDeserializedChest(locationName, position);
                    _monitor.VerboseLog($"Hiding and saving in {locationName}: {deserializedChest}");
                    deserializedChests.Add(deserializedChest);
                }
            }
            if (!Context.IsMainPlayer)
            {
                _monitor.VerboseLog("Not main player!");
                return;
            }
            var saveData = new SaveData
            {
                DeserializedChests = deserializedChests
            };
            _modHelper.Data.WriteSaveData(SaveDataKey, saveData);
        }

        public void ReAddCustomChests()
        {
            _monitor.VerboseLog("LocationSaver: ReAddCustomChests");
            if (_locationCustomChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var customChestLocations in _locationCustomChests)
            {
                var location = customChestLocations.Key;
                var customChestPositions = customChestLocations.Value;
                foreach (var customChestPosition in customChestPositions)
                {
                    var position = customChestPosition.Key;
                    var customChest = customChestPosition.Value;
                    var locationName = location.uniqueName.Value ?? location.Name;
                    _monitor.VerboseLog($"Re-adding in {locationName}: {customChest.Name} ({position})");
                    location.objects[position] = customChest;
                }
            }
        }

        public void LoadCustomChests()
        {
            _monitor.VerboseLog("LocationSaver: LoadCustomChests");
            if (!Context.IsMainPlayer)
            {
                _monitor.VerboseLog("Not main player!");
                return;
            }
            var saveData = _modHelper.Data.ReadSaveData<SaveData>(SaveDataKey);
            if (saveData == null)
            {
                _monitor.VerboseLog("Nothing to load");
                return;
            }
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var locationName = location.uniqueName?.Value ?? location.Name;
                var customChestsInLocation = saveData.DeserializedChests.Where(x => x.LocationName == locationName);
                foreach (var deserializedChest in customChestsInLocation)
                {
                    var position = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
                    if (!location.objects.ContainsKey(position))
                    {
                        _monitor.VerboseLog("WARNING! Expected chest at position: " + position);
                        continue;
                    }
                    var chest = (Chest)location.objects[position];
                    var customChest = chest.ToCustomChest(deserializedChest.ChestType);
                    _monitor.VerboseLog($"Loading: {deserializedChest}");
                    location.objects[position] = customChest;
                }
            }
        }

    }
}
