using MegaStorage.Models;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace MegaStorage.Mapping
{
    public static class MappingExtensions
    {

        public static Chest ToChest(this NiceChest niceChest)
        {
            var chest = new Chest(true);
            chest.items.AddRange(niceChest.items);
            chest.playerChoiceColor.Value = niceChest.playerChoiceColor.Value;
            return chest;
        }

        public static NiceChest ToNiceChest(this Chest chest, ChestType chestType)
        {
            var niceChest = NiceChestFactory.Create(chestType);
            niceChest.items.AddRange(chest.items);
            niceChest.playerChoiceColor.Value = chest.playerChoiceColor.Value;
            return niceChest;
        }

        public static DeserializedChest ToDeserializedChest(this NiceChest niceChest, int inventoryIndex)
        {
            return new DeserializedChest
            {
                InventoryIndex = inventoryIndex,
                ChestType = niceChest.ChestType
            };
        }

        public static DeserializedChest ToDeserializedChest(this NiceChest niceChest, long playerId, int inventoryIndex)
        {
            return new DeserializedChest
            {
                PlayerId = playerId,
                InventoryIndex = inventoryIndex,
                ChestType = niceChest.ChestType
            };
        }

        public static DeserializedChest ToDeserializedChest(this NiceChest niceChest, string locationName, Vector2 position)
        {
            return new DeserializedChest
            {
                LocationName = locationName,
                PositionX = position.X,
                PositionY = position.Y,
                ChestType = niceChest.ChestType
            };
        }

        public static DeserializedChest ToDeserializedChest(this NiceChest niceChest, string locationName, Vector2 position, int inventoryIndex)
        {
            return new DeserializedChest
            {
                LocationName = locationName,
                PositionX = position.X,
                PositionY = position.Y,
                InventoryIndex = inventoryIndex,
                ChestType = niceChest.ChestType
            };
        }

    }
}
