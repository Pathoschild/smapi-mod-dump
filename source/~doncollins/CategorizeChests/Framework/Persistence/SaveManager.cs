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
using Newtonsoft.Json.Linq;
using System.IO;
using StardewValleyMods.CategorizeChests.Framework.Persistence.Legacy;

namespace StardewValleyMods.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// The class responsible for saving and loading the mod state.
    /// </summary>
    class SaveManager : ISaveManager
    {
        private readonly ISemanticVersion Version;
        private readonly IChestDataManager ChestDataManager;
        private readonly IChestFinder ChestFinder;
        private readonly IItemDataManager ItemDataManager;

        public SaveManager(ISemanticVersion version, IChestDataManager chestDataManager, IChestFinder chestFinder, IItemDataManager itemDataManager)
        {
            Version = version;
            ChestDataManager = chestDataManager;
            ChestFinder = chestFinder;
            ItemDataManager = itemDataManager;
        }

        /// <summary>
        /// Generate save data and write it to the given file path.
        /// </summary>
        /// <param name="path">The full path of the save file.</param>
        public void Save(string path)
        {
            var saver = new Saver(Version, ChestDataManager);
            var json = saver.DumpData();

            File.WriteAllText(path, json); // TODO: (use SMAPI classes if possible!)
        }

        /// <summary>
        /// Load save data from the given file path.
        /// </summary>
        public void Load(string path)
        {
            var json = File.ReadAllText(path);
            var token = JToken.Parse(json);

            token = ConvertVersion(token);

            var loader = new Loader(ChestDataManager, ChestFinder, ItemDataManager);
            loader.LoadData(token);
        }

        /// <summary>
        /// Detect which version produced the given save data and return an updated
        /// version of the data corresponding to the save format of the current version.
        /// </summary>
        /// <returns>The converted data suitable for consumption by the current version.</returns>
        /// <param name="data">The unconverted data from the save file.</param>
        // TODO: This should really return a SaveData object, since we know that
        // it's going to end up being valid current-version data.
        private JToken ConvertVersion(JToken data)
        {
            var version = ReadVersionNumber(data);

            if (version.IsOlderThan("1.1.0"))
                data = new Version102Converter().Convert(data);

            return data;
        }

        /// <summary>
        /// Figure out which version of the mod produced the given save data.
        /// </summary>
        /// <returns>Which version produced the given save data.</returns>
        private ISemanticVersion ReadVersionNumber(JToken token)
        {
            if (token is JObject jObject)
            {
                var versionString = jObject.Value<string>("Version");
                return new SemanticVersion(versionString);
            }
            // In 1.0.2 and older, the JSON data consisted of an array and not
            // an object, so if we encounter that, we know it's from one of 
            // those versions.
            else if (token is JArray)
            {
                return new SemanticVersion("1.0.2");
            }
            else
            {
                throw new InvalidSaveDataException("Cannot detect save data version");
            }
        }
    }
}