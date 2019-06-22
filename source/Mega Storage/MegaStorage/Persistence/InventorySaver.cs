using System.Collections.Generic;
using System.Linq;
using MegaStorage.Mapping;
using MegaStorage.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Persistence
{
    public class InventorySaver : ISaver
    {
        public string SaveDataKey => "InventoryNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<int, CustomChest> _inventoryCustomChests;

        public InventorySaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveCustomChests()
        {
            _monitor.VerboseLog("InventorySaver: HideAndSaveCustomChests");
            _inventoryCustomChests = new Dictionary<int, CustomChest>();
            var deserializedChests = new List<DeserializedChest>();
            var customChests = Game1.player.Items.OfType<CustomChest>();
            foreach (var customChest in customChests)
            {
                var chest = customChest.ToChest();
                var index = Game1.player.Items.IndexOf(customChest);
                Game1.player.Items[index] = chest;
                var deserializedChest = customChest.ToDeserializedChest(index);
                _monitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                deserializedChests.Add(deserializedChest);
                _inventoryCustomChests.Add(index, customChest);
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
            _monitor.VerboseLog("InventorySaver: ReAddCustomChests");

            if (_inventoryCustomChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var customChestIndex in _inventoryCustomChests)
            {
                var index = customChestIndex.Key;
                var customChest = customChestIndex.Value;
                _monitor.VerboseLog($"Re-adding: {customChest.Name} ({index})");
                Game1.player.Items[index] = customChest;
            }
        }

        public void LoadCustomChests()
        {
            _monitor.VerboseLog("InventorySaver: LoadCustomChests");
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
                var chest = (Chest)Game1.player.Items[deserializedChest.InventoryIndex];
                var customChest = chest.ToCustomChest(deserializedChest.ChestType);
                _monitor.VerboseLog($"Loading: {deserializedChest}");
                Game1.player.Items[deserializedChest.InventoryIndex] = customChest;
            }

        }

    }
}
