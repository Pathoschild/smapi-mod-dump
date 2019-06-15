using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Persistence
{
    public class LocationInventorySaver : ISaver
    {
        private const string SaveDataKey = "LocationInventoryNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<GameLocation, Dictionary<Vector2, Dictionary<int, NiceChest>>> _locationInventoryNiceChests;

        public LocationInventorySaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveNiceChests()
        {
            _monitor.VerboseLog("LocationInventorySaver: HideAndSaveNiceChests");
            _locationInventoryNiceChests = new Dictionary<GameLocation, Dictionary<Vector2, Dictionary<int, NiceChest>>>();
            var deserializedChests = new List<DeserializedChest>();
            foreach (var location in Game1.locations)
            {
                var chestPositions = location.objects.Pairs.Where(x => x.Value is Chest).ToDictionary(pair => pair.Key, pair => (Chest)pair.Value);
                if (!chestPositions.Any())
                    continue;
                var niceChestsInChestPositions = new Dictionary<Vector2, Dictionary<int, NiceChest>>();
                foreach (var chestPosition in chestPositions)
                {
                    var position = chestPosition.Key;
                    var chest = chestPosition.Value;
                    var niceChestsInChest = chest.items.OfType<NiceChest>().ToList();
                    if (!niceChestsInChest.Any())
                        continue;
                    var niceChestIndexes = new Dictionary<int, NiceChest>();
                    foreach (var niceChest in niceChestsInChest)
                    {
                        var index = chest.items.IndexOf(niceChest);
                        chest.items[index] = niceChest.ToChest();
                        niceChestIndexes.Add(index, niceChest);
                        var deserializedChest = niceChest.ToDeserializedChest(location.Name, position, index);
                        _monitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                        deserializedChests.Add(deserializedChest);
                    }
                    niceChestsInChestPositions.Add(position, niceChestIndexes);
                }
                _locationInventoryNiceChests.Add(location, niceChestsInChestPositions);
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
            _monitor.VerboseLog("LocationInventorySaver: ReAddNiceChests");
            if (_locationInventoryNiceChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var niceChestLocations in _locationInventoryNiceChests)
            {
                var location = niceChestLocations.Key;
                var niceChestsInChestPositions = niceChestLocations.Value;
                foreach (var niceChestsInChestPosition in niceChestsInChestPositions)
                {
                    var position = niceChestsInChestPosition.Key;
                    var chestIndexes = niceChestsInChestPosition.Value;
                    foreach (var chestIndex in chestIndexes)
                    {
                        var index = chestIndex.Key;
                        var niceChest = chestIndex.Value;
                        _monitor.VerboseLog($"Re-adding: {niceChest.Name} in chest at {position} ({index})");
                        var chest = (Chest)location.objects[position];
                        chest.items[index] = niceChest;
                    }
                }
            }
        }

        public void LoadNiceChests()
        {
            _monitor.VerboseLog("LocationInventorySaver: LoadNiceChests");
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
            foreach (var location in Game1.locations)
            {
                var niceChestsInLocation = saveData.DeserializedChests.Where(x => x.LocationName == location.Name);
                foreach (var deserializedChest in niceChestsInLocation)
                {
                    var position = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
                    if (!location.objects.ContainsKey(position))
                    {
                        _monitor.VerboseLog("WARNING! Expected chest at position: " + position);
                        continue;
                    }
                    var chest = (Chest)location.objects[position];
                    var index = deserializedChest.InventoryIndex;
                    var hiddenNiceChest = (Chest)chest.items[index];
                    var niceChest = hiddenNiceChest.ToNiceChest(deserializedChest.ChestType);
                    _monitor.VerboseLog($"Loading: {deserializedChest}");
                    chest.items[index] = niceChest;
                }
            }
        }

    }
}
