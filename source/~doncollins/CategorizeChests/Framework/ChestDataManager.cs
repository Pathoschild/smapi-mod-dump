using StardewModdingAPI;
using StardewValley.Objects;
using System.Runtime.CompilerServices;

namespace StardewValleyMods.CategorizeChests.Framework
{
    class ChestDataManager : IChestDataManager
    {
        private readonly IItemDataManager ItemDataManager;
        private readonly IMonitor Monitor;

        private ConditionalWeakTable<Chest, ChestData> Table = new ConditionalWeakTable<Chest, ChestData>();

        public ChestDataManager(IItemDataManager itemDataManager, IMonitor monitor)
        {
            ItemDataManager = itemDataManager;
            Monitor = monitor;
        }

        public ChestData GetChestData(Chest chest)
        {
            return Table.GetValue(chest, c => new ChestData(c, ItemDataManager));
        }
    }
}