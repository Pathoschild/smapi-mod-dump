using MegaStorage.Framework.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    public class InventorySaver : ISaver
    {
        public string SaveDataKey => "InventoryNiceChests";

        private Dictionary<int, CustomChest> _inventoryCustomChests;

        public void HideAndSaveCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("InventorySaver: HideAndSaveCustomChests");
            _inventoryCustomChests = new Dictionary<int, CustomChest>();
            var deserializedChests = new List<DeserializedChest>();
            var customChests = Game1.player.Items.OfType<CustomChest>();
            foreach (var customChest in customChests)
            {
                var chest = customChest.ToChest();
                var index = Game1.player.Items.IndexOf(customChest);
                MegaStorageMod.ConvenientChests?.CopyChestData(customChest, chest);
                Game1.player.Items[index] = chest;
                var deserializedChest = customChest.ToDeserializedChest(index);
                MegaStorageMod.ModMonitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                deserializedChests.Add(deserializedChest);
                _inventoryCustomChests.Add(index, customChest);
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
            MegaStorageMod.ModMonitor.VerboseLog("InventorySaver: ReAddCustomChests");

            if (_inventoryCustomChests == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var customChestIndex in _inventoryCustomChests)
            {
                var index = customChestIndex.Key;
                var customChest = customChestIndex.Value;
                MegaStorageMod.ModMonitor.VerboseLog($"Re-adding: {customChest.Name} ({index})");
                Game1.player.Items[index] = customChest;
            }
        }

        public void LoadCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("InventorySaver: LoadCustomChests");
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
                var chest = (Chest)Game1.player.Items[deserializedChest.InventoryIndex];
                var customChest = chest.ToCustomChest(deserializedChest.ChestType);
                MegaStorageMod.ModMonitor.VerboseLog($"Loading: {deserializedChest}");
                MegaStorageMod.ConvenientChests?.CopyChestData(chest, customChest);
                Game1.player.Items[deserializedChest.InventoryIndex] = customChest;
            }

        }

    }
}
