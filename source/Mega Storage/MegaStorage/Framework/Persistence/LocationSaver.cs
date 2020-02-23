using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    public class LocationSaver : ISaver
    {
        public string SaveDataKey => "LocationNiceChests";

        private Dictionary<GameLocation, Dictionary<Vector2, CustomChest>> _locationCustomChests;

        public void HideAndSaveCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationSaver: HideAndSaveCustomChests");
            _locationCustomChests = new Dictionary<GameLocation, Dictionary<Vector2, CustomChest>>();
            var deserializedChests = new List<DeserializedChest>();
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var customChestPositions = location.objects.Pairs.Where(x => x.Value is CustomChest).ToDictionary(pair => pair.Key, pair => (CustomChest)pair.Value);
                if (!customChestPositions.Any())
                {
                    continue;
                }

                var locationName = location.uniqueName?.Value ?? location.Name;
                _locationCustomChests.Add(location, customChestPositions);
                foreach (var customChestPosition in customChestPositions)
                {
                    var position = customChestPosition.Key;
                    var customChest = customChestPosition.Value;
                    var chest = customChest.ToChest();
                    MegaStorageMod.ConvenientChests?.CopyChestData(customChest, chest);
                    location.objects[position] = chest;
                    var deserializedChest = customChest.ToDeserializedChest(locationName, position);
                    MegaStorageMod.ModMonitor.VerboseLog($"Hiding and saving in {locationName}: {deserializedChest}");
                    deserializedChests.Add(deserializedChest);
                }
            }
            if (!Context.IsMainPlayer)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Not main player!");
                return;
            }

            var saveData = new SaveData
            {
                DeserializedChests = deserializedChests
            };
            MegaStorageMod.ModHelper.Data.WriteSaveData(SaveDataKey, saveData);
        }

        public void ReAddCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationSaver: ReAddCustomChests");
            if (_locationCustomChests == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to re-add");
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
                    MegaStorageMod.ModMonitor.VerboseLog($"Re-adding in {locationName}: {customChest.Name} ({position})");
                    location.objects[position] = customChest;
                }
            }
        }

        public void LoadCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationSaver: LoadCustomChests");
            if (!Context.IsMainPlayer)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Not main player!");
                return;
            }
            var saveData = MegaStorageMod.ModHelper.Data.ReadSaveData<SaveData>(SaveDataKey);
            if (saveData == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to load");
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
                        MegaStorageMod.ModMonitor.VerboseLog("WARNING! Expected chest at position: " + position);
                        continue;
                    }
                    var chest = (Chest)location.objects[position];
                    var customChest = chest.ToCustomChest(deserializedChest.ChestType, position);
                    MegaStorageMod.ModMonitor.VerboseLog($"Loading: {deserializedChest}");
                    MegaStorageMod.ConvenientChests?.CopyChestData(chest, customChest);
                    location.objects[position] = customChest;
                }
            }
        }

    }
}
