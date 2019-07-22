using System.Collections.Generic;
using System.Linq;
using MegaStorage.Mapping;
using MegaStorage.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Persistence
{
    public class FarmhandInventorySaver : ISaver
    {
        public string SaveDataKey => "FarmhandInventoryNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;
        private readonly IConvenientChestsApi _convenientChestsApi;

        private Dictionary<long, Dictionary<int, CustomChest>> _farmhandInventoryCustomChests;

        public FarmhandInventorySaver(IModHelper modHelper, IMonitor monitor, IConvenientChestsApi convenientChestsApi)
        {
            _modHelper = modHelper;
            _monitor = monitor;
            _convenientChestsApi = convenientChestsApi;
        }

        public void HideAndSaveCustomChests()
        {
            _monitor.VerboseLog("FarmhandInventorySaver: HideAndSaveCustomChests");
            _farmhandInventoryCustomChests = new Dictionary<long, Dictionary<int, CustomChest>>();
            var deserializedChests = new List<DeserializedChest>();
            foreach (var otherFarmer in Game1.otherFarmers)
            {
                var playerId = otherFarmer.Key;
                var player = otherFarmer.Value;
                var playerCustomChests = new Dictionary<int, CustomChest>();
                var customChests = player.Items.OfType<CustomChest>();
                foreach (var customChest in customChests)
                {
                    var chest = customChest.ToChest();
                    var index = player.Items.IndexOf(customChest);
                    _convenientChestsApi?.CopyChestData(customChest, chest);
                    player.Items[index] = chest;
                    var deserializedChest = customChest.ToDeserializedChest(playerId, index);
                    _monitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                    deserializedChests.Add(deserializedChest);
                    playerCustomChests.Add(index, customChest);
                }
                _farmhandInventoryCustomChests.Add(playerId, playerCustomChests);
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
            _monitor.VerboseLog("FarmhandInventorySaver: ReAddCustomChests");
            if (_farmhandInventoryCustomChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var playerCustomChest in _farmhandInventoryCustomChests)
            {
                var playerId = playerCustomChest.Key;
                foreach (var customChestIndex in playerCustomChest.Value)
                {
                    var inventoryIndex = customChestIndex.Key;
                    var customChest = customChestIndex.Value;
                    _monitor.VerboseLog($"Re-adding: {customChest.Name} ({inventoryIndex})");
                    if (!Game1.otherFarmers.ContainsKey(playerId))
                    {
                        _monitor.VerboseLog($"Other player isn't loaded: {playerId}");
                        continue;
                    }
                    var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
                    player.Items[inventoryIndex] = customChest;
                }
            }
        }

        public void LoadCustomChests()
        {
            _monitor.VerboseLog("FarmhandInventorySaver: LoadCustomChests");
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
            foreach (var deserializedChest in saveData.DeserializedChests)
            {
                var playerId = deserializedChest.PlayerId;
                if (!Game1.otherFarmers.ContainsKey(playerId))
                {
                    _monitor.VerboseLog($"Other player isn't loaded: {playerId}");
                    continue;
                }
                var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
                var chest = (Chest)player.Items[deserializedChest.InventoryIndex];
                var customChest = chest.ToCustomChest(deserializedChest.ChestType);
                _monitor.VerboseLog($"Loading: {deserializedChest}");
                _convenientChestsApi?.CopyChestData(chest, customChest);
                player.Items[deserializedChest.InventoryIndex] = customChest;
            }
        }

    }
}
