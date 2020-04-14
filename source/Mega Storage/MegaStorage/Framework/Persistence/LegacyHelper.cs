using furyx639.Common;
using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    internal static class LegacyHelper
    {
        /*********
        ** Public methods
        *********/
        internal static void FixLegacyOptions()
        {
            MegaStorageMod.ModMonitor.VerboseLog("SaveManager: FixLegacyOptions");
            if (!Context.IsMainPlayer)
                return;

            var legacySavers = new Dictionary<string, Action<DeserializedChest>>()
            {
                {"InventoryNiceChests", FixInventory},
                {"LocationNiceChests", FixLocation},
                {"FarmhandInventoryNiceChests", FixFarmhandInventory},
                {"LocationInventoryNiceChests", FixLocationInventory}
            };

            foreach (var legacySaver in legacySavers)
            {
                var saveData = MegaStorageMod.ModHelper.Data.ReadSaveData<SaveData>(legacySaver.Key);
                if (saveData is null)
                    continue;
                foreach (var deserializedChest in saveData.DeserializedChests)
                {
                    legacySaver.Value.Invoke(deserializedChest);
                }
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Reverts all Custom Chests in player inventory back to a regular Object
        /// </summary>
        /// <param name="deserializedChest">Save data for custom chest</param>
        private static void FixInventory(DeserializedChest deserializedChest)
        {
            var index = deserializedChest.InventoryIndex;
            var chestType = deserializedChest.ChestType;

            // Only revert chests in expected position
            if (!(Game1.player.Items[index] is Chest chest))
                return;
            Game1.player.Items[index] = chest.ToObject(chestType);
        }

        /// <summary>
        /// Reverts all Custom Chests in farmhand inventory back to a regular Object
        /// </summary>
        /// <param name="deserializedChest">Save data for custom chest</param>
        private static void FixFarmhandInventory(DeserializedChest deserializedChest)
        {
            var playerId = deserializedChest.PlayerId;
            var index = deserializedChest.InventoryIndex;
            var chestType = deserializedChest.ChestType;

            // Only revert chests in expected position
            if (!Game1.otherFarmers.ContainsKey(playerId))
                return;
            var player = Game1.otherFarmers.Single(x => x.Key == playerId).Value;
            if (!(player.Items[index] is Chest chest))
                return;
            player.Items[index] = chest.ToObject(chestType);
        }

        /// <summary>
        /// Updates all placed Custom Chests with correct ParentSheetIndex
        /// </summary>
        /// <param name="deserializedChest">Save data for custom chest</param>
        private static void FixLocation(DeserializedChest deserializedChest)
        {
            var locationName = deserializedChest.LocationName;
            var pos = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
            var chestType = deserializedChest.ChestType;

            var location = CommonHelper.GetLocations()
                .Single(l => (l.uniqueName?.Value ?? l.Name) == locationName);
            if (location is null
                || !location.objects.ContainsKey(pos)
                || !(location.objects[pos] is Chest chest))
            {
                return;
            }

            chest.ParentSheetIndex = CustomChestFactory.CustomChestIds[chestType];
        }

        /// <summary>
        /// Reverts all Custom Chests in placed chests back to a regular Object
        /// </summary>
        /// <param name="deserializedChest">Save data for custom chest</param>
        private static void FixLocationInventory(DeserializedChest deserializedChest)
        {
            var locationName = deserializedChest.LocationName;
            var pos = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
            var index = deserializedChest.InventoryIndex;
            var chestType = deserializedChest.ChestType;

            var location = CommonHelper.GetLocations()
                .Single(l => (l.uniqueName?.Value ?? l.Name) == locationName);
            if (location is null
                || !location.objects.ContainsKey(pos)
                || !(location.objects[pos] is Chest chest))
            {
                return;
            }

            chest.items[index] = chest.items[index].ToObject(chestType);
        }
    }
}
