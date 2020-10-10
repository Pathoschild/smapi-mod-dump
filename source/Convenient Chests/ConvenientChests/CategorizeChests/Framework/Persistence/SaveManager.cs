/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Linq;
using StardewModdingAPI;

namespace ConvenientChests.CategorizeChests.Framework.Persistence {
    /// <summary>
    /// The class responsible for saving and loading the mod state.
    /// </summary>
    class SaveManager : ISaveManager {
        private readonly CategorizeChestsModule Module;
        private readonly ISemanticVersion       Version;

        public SaveManager(ISemanticVersion version, CategorizeChestsModule module) {
            Version = version;
            Module  = module;
        }

        /// <summary>
        /// Generate save data and write it to the given file path.
        /// </summary>
        /// <param name="relativePath">The path of the save file relative to the mod folder.</param>
        public void Save(string relativePath) {
            var saver = new Saver(Version, Module.ChestDataManager);
            Module.ModEntry.Helper.Data.WriteJsonFile(relativePath, saver.GetSerializableData());
        }

        /// <summary>
        /// Load save data from the given file path.
        /// </summary>
        /// <param name="relativePath">The path of the save file relative to the mod folder.</param>
        public void Load(string relativePath) {
            var model = Module.ModEntry.Helper.Data.ReadJsonFile<SaveData>(relativePath) ?? new SaveData();

            foreach (var entry in model.ChestEntries) {
                try {
                    var chest     = Module.ChestFinder.GetChestByAddress(entry.Address);
                    var chestData = Module.ChestDataManager.GetChestData(chest);

                    chestData.AcceptedItemKinds = entry.GetItemSet();
                    foreach (var key in chestData.AcceptedItemKinds.Where(k => !Module.ItemDataManager.Prototypes.ContainsKey(k)))
                        Module.ItemDataManager.Prototypes.Add(key, key.GetOne());
                }
                catch (InvalidSaveDataException e) {
                    Module.Monitor.Log(e.Message, LogLevel.Warn);
                }
            }
        }
    }
}