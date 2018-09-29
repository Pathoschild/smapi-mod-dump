using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace StardewValleyMods.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// The class responsible for reading saved data and using it to restore
    /// the mod state.
    /// </summary>
    class Loader
    {
        private readonly IChestDataManager ChestDataManager;
        private readonly IChestFinder ChestFinder;
        private readonly IItemDataManager ItemDataManager;

        public Loader(IChestDataManager chestDataManager, IChestFinder chestFinder, IItemDataManager itemDataManager)
        {
            ChestDataManager = chestDataManager;
            ChestFinder = chestFinder;
            ItemDataManager = itemDataManager;
        }

        /// <summary>
        /// Read the given save data and use it to reconstruct the mod state.
        /// </summary>
        /// <param name="token">The save data.</param>
        public void LoadData(JToken token)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            var data = token.ToObject<SaveData>(serializer);

            foreach (var entry in data.ChestEntries)
            {
                var chest = ChestFinder.GetChestByAddress(entry.Address);
                var chestData = ChestDataManager.GetChestData(chest);

                chestData.AcceptedItemKinds = entry.AcceptedItemKinds
                    .Where(itemKey => ItemDataManager.HasItem(itemKey));
            }
        }
    }
}