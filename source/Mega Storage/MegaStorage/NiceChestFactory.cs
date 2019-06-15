using System;
using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;

namespace MegaStorage
{
    public static class NiceChestFactory
    {
        public static List<NiceChest> NiceChests { get; }

        static NiceChestFactory()
        {
            NiceChests = new List<NiceChest>
            {
                new LargeChest(),
                new MagicChest()
            };
        }

        public static bool IsNiceChest(int id)
        {
            return NiceChests.Any(x => x.ItemId == id);
        }

        public static NiceChest Create(int id)
        {
            var chestType = NiceChests.Single(x => x.ItemId == id).ChestType;
            return Create(chestType);
        }

        public static NiceChest Create(ChestType chestType)
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
