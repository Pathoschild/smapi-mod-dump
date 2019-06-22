using MegaStorage.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Mapping
{
    public static class MappingExtensions
    {

        public static Chest ToChest(this CustomChest customChest)
        {
            var chest = new Chest(true);
            chest.items.AddRange(customChest.items);
            chest.playerChoiceColor.Value = customChest.playerChoiceColor.Value;
            chest.name = customChest.name;
            return chest;
        }

        public static CustomChest ToCustomChest(this Chest chest, ChestType chestType)
        {
            var customChest = CustomChestFactory.Create(chestType);
            customChest.items.AddRange(chest.items);
            customChest.playerChoiceColor.Value = chest.playerChoiceColor.Value;
            customChest.name = chest.name;
            return customChest;
        }

        public static DeserializedChest ToDeserializedChest(this CustomChest customChest, int inventoryIndex)
        {
            return new DeserializedChest
            {
                InventoryIndex = inventoryIndex,
                ChestType = customChest.ChestType,
                Name = customChest.name
            };
        }

        public static DeserializedChest ToDeserializedChest(this CustomChest customChest, string locationName, Vector2 position)
        {
            return new DeserializedChest
            {
                LocationName = locationName,
                PositionX = position.X,
                PositionY = position.Y,
                ChestType = customChest.ChestType,
                Name = customChest.name
            };
        }

        public static DeserializedChest ToDeserializedChest(this CustomChest customChest, long playerId, int inventoryIndex)
        {
            var deserializedChest = customChest.ToDeserializedChest(inventoryIndex);
            deserializedChest.PlayerId = playerId;
            return deserializedChest;
        }

        public static DeserializedChest ToDeserializedChest(this CustomChest customChest, string locationName, Vector2 position, int inventoryIndex)
        {
            var deserializedChest = customChest.ToDeserializedChest(locationName, position);
            deserializedChest.InventoryIndex = inventoryIndex;
            return deserializedChest;
        }

        public static CustomChest ToCustomChest(this Item item)
        {
            var customChest = CustomChestFactory.Create(item.ParentSheetIndex);
            customChest.name = item.Name;
            return customChest;
        }

    }
}
