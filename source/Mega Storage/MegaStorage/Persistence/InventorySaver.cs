using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Persistence
{
    public class InventorySaver : ISaver
    {
        private const string SaveDataKey = "InventoryNiceChests";

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private Dictionary<int, NiceChest> _inventoryNiceChests;

        public InventorySaver(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
        }

        public void HideAndSaveNiceChests()
        {
            _monitor.VerboseLog("InventorySaver: HideAndSaveNiceChests");
            _inventoryNiceChests = new Dictionary<int, NiceChest>();
            var deserializedChests = new List<DeserializedChest>();
            var niceChests = Game1.player.Items.OfType<NiceChest>();
            foreach (var niceChest in niceChests)
            {
                var chest = niceChest.ToChest();
                var index = Game1.player.Items.IndexOf(niceChest);
                Game1.player.Items[index] = chest;
                var deserializedChest = niceChest.ToDeserializedChest(index);
                _monitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                deserializedChests.Add(deserializedChest);
                _inventoryNiceChests.Add(index, niceChest);
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
            _monitor.VerboseLog("InventorySaver: ReAddNiceChests");

            if (_inventoryNiceChests == null)
            {
                _monitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var niceChestIndex in _inventoryNiceChests)
            {
                var index = niceChestIndex.Key;
                var niceChest = niceChestIndex.Value;
                _monitor.VerboseLog($"Re-adding: {niceChest.Name} ({index})");
                Game1.player.Items[index] = niceChest;
            }
        }

        public void LoadNiceChests()
        {
            _monitor.VerboseLog("InventorySaver: LoadNiceChests");
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
                var niceChest = chest.ToNiceChest(deserializedChest.ChestType);
                _monitor.VerboseLog($"Loading: {deserializedChest}");
                Game1.player.Items[deserializedChest.InventoryIndex] = niceChest;
            }

        }

    }
}
