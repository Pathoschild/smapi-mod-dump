/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using MoreTrees.Models;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoreTrees
{
    /// <summary>Provides basic More Trees apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Get the tree type by tree name.</summary>
        /// <param name="name">The name of the tree.</param>
        /// <returns>The tree type.</returns>
        public int GetTreeType(string name)
        {
            var treeTypes = ReadTreeTypesFile();
            if (treeTypes == null)
                treeTypes = new Dictionary<int, string>();

            // check if the tree already has a type assigned
            if (treeTypes.Any())
            {
                int treeType = treeTypes
                    .Where(typeMap => typeMap.Value.ToLower() == name.ToLower())
                    .Select(typeMap => typeMap.Key)
                    .FirstOrDefault();

                if (treeType > 0)
                    return treeType;
            }

            // the tree hasn't been loaded before, generate a new typeId for it
            var typeId = -1;
            for (int i = 20; typeId == -1; i++) // start i at 20 to accomodate for base game trees
            {
                // ensure the id isn't already used by a different tree
                if (treeTypes.Where(typeMap => typeMap.Key == i).Any())
                    continue;

                typeId = i;
            }
            treeTypes.Add(typeId, name);

            SetTreeTypesFile(treeTypes);
            return typeId;
        }

        /// <summary>Get a <see cref="CustomTree"/> by type.</summary>
        /// <param name="type">The type of the tree.</param>
        /// <returns>The <see cref="CustomTree"/>.</returns>
        public CustomTree GetTreeByType(int type)
        {
            return ModEntry.Instance.LoadedTrees
                .Where(tree => tree.Type == type)
                .FirstOrDefault();
        }

        /// <summary>Get tree data of a tree at a specific tile location.</summary>
        /// <param name="tileLocation">The location of the tree data to get.</param>
        /// <returns>The tree data of the tree at the given location.</returns>
        public SavePersistantTreeData GetTreeDataByLocation(Vector2 tileLocation)
        {
            return ModEntry.Instance.SavedTreeData
                .Where(treeData => treeData.TileLocation == tileLocation)
                .FirstOrDefault();
        }

        /// <summary>Get whether the tree at a location has been debarked.</summary>
        /// <param name="tileLocation">The location of the tree to check.</param>
        /// <returns>Whether the tree at the given location has been debarked.</returns>
        public bool IsTreeDebarked(Vector2 tileLocation)
        {
            var daysTillNextBarkHarvest = ModEntry.Instance.SavedTreeData
                .Where(treeData => treeData.TileLocation == tileLocation)
                .Select(treeData => treeData.DaysTillNextBarkHarvest)
                .FirstOrDefault();

            return daysTillNextBarkHarvest > 0;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Get the contents of the treeTypes.json file.</summary>
        /// <returns>The contents of the treeTypes.json file.</returns>
        private Dictionary<int, string> ReadTreeTypesFile()
        {
            if (Constants.CurrentSavePath == null)
            {
                ModEntry.Instance.Monitor.Log("An attempt to read the Id map was made without loading a save.");
                return null;
            }

            // get the content of the treeTypes.json file
            var treeTypesPath = GetTreeTypesFilePath();
            var treeTypes = File.ReadAllText(treeTypesPath);

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<int, string>>(treeTypes);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"TreeTypes couldn't be deserialised. TreeTypes will be ignored, this could Id shifting (trees may be different from save). \nPath: {treeTypesPath}\n{ex}", LogLevel.Error);
                return null;
            }
        }

        /// <summary>Get the treeTypes.json path.</summary>
        /// <returns>The treeTypes.json path.</returns>
        private string GetTreeTypesFilePath()
        {
            // get/create the directory containing the treeTypesFile
            var treeTypesFileDirectory = Path.Combine(Constants.CurrentSavePath, "MoreTrees");
            if (!Directory.Exists(treeTypesFileDirectory))
                Directory.CreateDirectory(treeTypesFileDirectory);

            // get/create the treeTypesFile
            var treeTypesFilePath = Path.Combine(treeTypesFileDirectory, "treeTypes.json");
            if (!File.Exists(treeTypesFilePath))
                File.Create(treeTypesFilePath).Close();

            return treeTypesFilePath;
        }

        /// <summary>Set the treeTypes.json file content.</summary>
        /// <param name="treeTypes">The tree types to write to the file.</param>
        private void SetTreeTypesFile(Dictionary<int, string> treeTypes)
        {
            var treeTypesPath = GetTreeTypesFilePath();
            File.WriteAllText(treeTypesPath, JsonConvert.SerializeObject(treeTypes));
        }
    }
}
