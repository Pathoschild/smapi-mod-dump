using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework
{
    public static class CustomChestFactory
    {
        private static List<CustomChest> _customChests;
        public static List<CustomChest> CustomChests =>
            _customChests ?? (_customChests = new List<CustomChest>
            {
                new LargeChest(Vector2.Zero),
                new MagicChest(Vector2.Zero),
                new SuperMagicChest(Vector2.Zero)
            });

        public static bool ShouldBeCustomChest(Item item)
        {
            if (!(item is SObject obj))
            {
                return false;
            }

            return obj.bigCraftable.Value
                   && CustomChests.Any(x => x.ParentSheetIndex == item.ParentSheetIndex);
        }

        public static CustomChest Create(int id) => Create(id, Vector2.Zero);
        public static CustomChest Create(int id, Vector2 tileLocation)
        {
            var chestType = CustomChests.Single(x => x.ParentSheetIndex == id).ChestType;
            return Create(chestType, tileLocation);
        }

        public static CustomChest Create(ChestType chestType) => Create(chestType, Vector2.Zero);
        public static CustomChest Create(ChestType chestType, Vector2 tileLocation)
        {
            switch (chestType)
            {
                case ChestType.LargeChest:
                    return new LargeChest(tileLocation);
                case ChestType.MagicChest:
                    return new MagicChest(tileLocation);
                case ChestType.SuperMagicChest:
                    return new SuperMagicChest(tileLocation);
                default:
                    throw new InvalidOperationException("Invalid ChestType");
            }
        }

    }
}
