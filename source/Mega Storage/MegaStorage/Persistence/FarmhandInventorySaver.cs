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
        private const string SaveDataKey = "FarmhandInventoryNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<long, Dictionary<int, NiceChest>> _farmhandInventoryNiceChests;

        public FarmhandInventorySaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveNiceChests()
        {
            _monitor.VerboseLog("FarmhandInventorySaver: HideAndSaveNiceChests");
            _farmhandInventoryNiceChests = new Dictionary<long, Dictionary<int, NiceChest>>();
            var deserializedChests = new List<DeserializedChest>();
            foreach (var otherFarmer in Game1.otherFarmers)
            {
                var playerId = otherFarmer.Key;
                var player = otherFarmer.Value;
                var playerNiceChests = new Dictionary<int, NiceChest>();
                var niceChests = player.Items.OfType<NiceChest>();
                foreach (var niceChest in niceChests)
                {
                    var chest = niceChest.ToChest();
                    var index = player.Items.IndexOf(niceChest);
                    player.Items[index] = chest;
                    var deserializedChest = niceChest.ToDeserializedChest(playerId, index);
                    _monitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                    deserializedChests.Add(deserializedChest);
                    playerNiceChests.Add(index, niceChest);
                }
                _farmhandInventoryNiceChests.Add(playerId, playerNiceChests);
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
            _monitor.VerboseLog("FarmhandInventorySaver: ReAddNiceChests");
            if (_farmhandInventoryNiceChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var playerNiceChest in _farmhandInventoryNiceChests)
            {
                var playerId = playerNiceChest.Key;
                foreach (var niceChestIndex in playerNiceChest.Value)
                {
                    var inventoryIndex = niceChestIndex.Key;
                    var niceChest = niceChestIndex.Value;
                    _monitor.VerboseLog($"Re-adding: {niceChest.Name} ({inventoryIndex})");
                    if (!Game1.otherFarmers.ContainsKey(playerId))
                    {
                        _monitor.VerboseLog($"Other player isn't loaded: {playerId}");
                        continue;
                    }
                    var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
                    player.Items[inventoryIndex] = niceChest;
                }
            }
        }

        public void LoadNiceChests()
        {
            _monitor.VerboseLog("FarmhandInventorySaver: LoadNiceChests");
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
                var niceChest = chest.ToNiceChest(deserializedChest.ChestType);
                _monitor.VerboseLog($"Loading: {deserializedChest}");
                player.Items[deserializedChest.InventoryIndex] = niceChest;
            }
        }

    }
}
