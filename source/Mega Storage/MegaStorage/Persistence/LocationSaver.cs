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
        private const string SaveDataKey = "LocationNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<GameLocation, Dictionary<Vector2, NiceChest>> _locationNiceChests;

        public LocationSaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveNiceChests()
        {
            _monitor.VerboseLog("LocationSaver: HideAndSaveNiceChests");
            _locationNiceChests = new Dictionary<GameLocation, Dictionary<Vector2, NiceChest>>();
            var deserializedChests = new List<DeserializedChest>();
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var niceChestPositions = location.objects.Pairs.Where(x => x.Value is NiceChest).ToDictionary(pair => pair.Key, pair => (NiceChest)pair.Value);
                if (!niceChestPositions.Any())
                    continue;
                var locationName = location.uniqueName?.Value ?? location.Name;
                _locationNiceChests.Add(location, niceChestPositions);
                foreach (var niceChestPosition in niceChestPositions)
                {
                    var position = niceChestPosition.Key;
                    var niceChest = niceChestPosition.Value;
                    var chest = niceChest.ToChest();
                    location.objects[position] = chest;
                    var deserializedChest = niceChest.ToDeserializedChest(locationName, position);
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

        public void ReAddNiceChests()
        {
            _monitor.VerboseLog("LocationSaver: ReAddNiceChests");
            if (_locationNiceChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var niceChestLocations in _locationNiceChests)
            {
                var location = niceChestLocations.Key;
                var niceChestPositions = niceChestLocations.Value;
                foreach (var niceChestPosition in niceChestPositions)
                {
                    var position = niceChestPosition.Key;
                    var niceChest = niceChestPosition.Value;
                    var locationName = location.uniqueName.Value ?? location.Name;
                    _monitor.VerboseLog($"Re-adding in {locationName}: {niceChest.Name} ({position})");
                    location.objects[position] = niceChest;
                }
            }
        }

        public void LoadNiceChests()
        {
            _monitor.VerboseLog("LocationSaver: LoadNiceChests");
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
                var niceChestsInLocation = saveData.DeserializedChests.Where(x => x.LocationName == locationName);
                foreach (var deserializedChest in niceChestsInLocation)
                {
                    var position = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
                    if (!location.objects.ContainsKey(position))
                    {
                        _monitor.VerboseLog("WARNING! Expected chest at position: " + position);
                        continue;
                    }
                    var chest = (Chest)location.objects[position];
                    var niceChest = chest.ToNiceChest(deserializedChest.ChestType);
                    _monitor.VerboseLog($"Loading: {deserializedChest}");
                    location.objects[position] = niceChest;
                }
            }
        }

    }
}
