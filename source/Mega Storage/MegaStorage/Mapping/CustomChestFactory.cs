using System;
using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;
using StardewValley;
using Object = StardewValley.Object;

namespace MegaStorage.Mapping
{
    public static class CustomChestFactory
    {
        public static List<CustomChest> CustomChests { get; }

        static CustomChestFactory()
        {
            CustomChests = new List<CustomChest>
            {
                new LargeChest(),
                new MagicChest()
            };
        }

        public static bool IsCustomChest(Item item)
        {
            if (!(item is Object)) return false;
            if (!((Object)item).bigCraftable.Value) return false;
            return CustomChests.Any(x => x.Config.Id == item.ParentSheetIndex);
        }

        public static CustomChest Create(int id)
        {
            var chestType = CustomChests.Single(x => x.Config.Id == id).ChestType;
            return Create(chestType);
        }

        public static CustomChest Create(ChestType chestType)
        {
            switch (chestType)
            {
                case ChestType.LargeChest:
                    return new LargeChest();
                case ChestType.MagicChest:
                    return new MagicChest();
                default:
                    throw new InvalidOperationException("Invalid ChestType");
            }
        }

    }
}
