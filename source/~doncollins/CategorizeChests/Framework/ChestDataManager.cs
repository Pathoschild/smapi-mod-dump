/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

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