using MegaStorage.Framework.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    public class FarmhandInventorySaver : ISaver
    {
        public string SaveDataKey => "FarmhandInventoryNiceChests";

        private Dictionary<long, Dictionary<int, CustomChest>> _farmhandInventoryCustomChests;

        public void HideAndSaveCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("FarmhandInventorySaver: HideAndSaveCustomChests");
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
                    MegaStorageMod.ConvenientChests?.CopyChestData(customChest, chest);
                    player.Items[index] = chest;
                    var deserializedChest = customChest.ToDeserializedChest(playerId, index);
                    MegaStorageMod.ModMonitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                    deserializedChests.Add(deserializedChest);
                    playerCustomChests.Add(index, customChest);
                }
                _farmhandInventoryCustomChests.Add(playerId, playerCustomChests);
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
            MegaStorageMod.ModMonitor.VerboseLog("FarmhandInventorySaver: ReAddCustomChests");
            if (_farmhandInventoryCustomChests == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var playerCustomChest in _farmhandInventoryCustomChests)
            {
                var playerId = playerCustomChest.Key;
                foreach (var customChestIndex in playerCustomChest.Value)
                {
                    var inventoryIndex = customChestIndex.Key;
                    var customChest = customChestIndex.Value;
                    MegaStorageMod.ModMonitor.VerboseLog($"Re-adding: {customChest.Name} ({inventoryIndex})");
                    if (!Game1.otherFarmers.ContainsKey(playerId))
                    {
                        MegaStorageMod.ModMonitor.VerboseLog($"Other player isn't loaded: {playerId}");
                        continue;
                    }
                    var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
                    player.Items[inventoryIndex] = customChest;
                }
            }
        }

        public void LoadCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("FarmhandInventorySaver: LoadCustomChests");
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
            foreach (var deserializedChest in saveData.DeserializedChests)
            {
                var playerId = deserializedChest.PlayerId;
                if (!Game1.otherFarmers.ContainsKey(playerId))
                {
                    MegaStorageMod.ModMonitor.VerboseLog($"Other player isn't loaded: {playerId}");
                    continue;
                }
                var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
                var chest = (Chest)player.Items[deserializedChest.InventoryIndex];
                var customChest = chest.ToCustomChest(deserializedChest.ChestType);
                MegaStorageMod.ModMonitor.VerboseLog($"Loading: {deserializedChest}");
                MegaStorageMod.ConvenientChests?.CopyChestData(chest, customChest);
                player.Items[deserializedChest.InventoryIndex] = customChest;
            }
        }

    }
}
