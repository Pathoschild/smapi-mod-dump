using Newtonsoft.Json;
using Outerwear.Models;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Outerwear
{
    /// <summary>Provides basic outerwear apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>The saved id map for setting the outerwear ids.</summary>
        private List<IdMap> IdMap;


        /*********
        ** Public Methods
        *********/
        /// <summary>Create an id for by the outerwear name, this is separate as it will also look in the file system for the static id map.</summary>
        /// <param name="outerwearName">The name of the outerwear to create the id of.</param>
        /// <returns>An id that is either from the static id map in the file system or an unused generated one.</returns>
        public int CreateId(string outerwearName)
        {
            // check id map file for a valid and present id
            ReadIdMapFile();

            if (IdMap == null)
                IdMap = new List<IdMap>();

            if (IdMap.Any())
            {
                int mappedId = IdMap
                    .Where(map => map.Name.ToLower() == outerwearName.ToLower())
                    .Select(map => map.Id)
                    .FirstOrDefault();

                if (mappedId > 0)
                {
                    return mappedId;
                }
            }

            // no id was valid / present in idMapFile, generate a unique id and add it to the idMapFile
            int id = -1;
            for (int i = 1; id == -1; i++)
            {
                if (!IdMap.Where(map => map.Id == i).Any())
                {
                    id = i;
                }
            }

            AddItemToIdMapFile(id, outerwearName);
            return id;
        }

        /// <summary>Get metadata for all currently loaded outerwear.</summary>
        /// <returns>A list of outwear objects.</returns>
        public List<Models.OuterwearData> GetAllOuterwearData()
        {
            return ModEntry.OuterwearData;
        }

        /// <summary>Get metadata for an outerwear by it's id.</summary>
        /// <param name="id">The id of the outerwear metadata to get.</param>
        /// <returns>The metadata for an outerwear with the respective id, or null if nothing was found.</returns>
        public Models.OuterwearData GetOuterwearDataById(int id)
        {
            return ModEntry.OuterwearData
                .Where(data => data.Id == id)
                .FirstOrDefault();
        }

        /// <summary>Get metadata for an outerwear by it's name.</summary>
        /// <param name="name">The name of the outerwear metadata to get.</param>
        /// <returns>The metadata for an outerwear with the respective name, or null if nothing was found.</returns>
        public Models.OuterwearData GetOuterwearDataByName(string name)
        {
            return ModEntry.OuterwearData
                .Where(data => data.DisplayName == name)
                .FirstOrDefault();
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Read / Create the IdMap file into the IdMap member.</summary>
        private void ReadIdMapFile()
        {
            if (Constants.CurrentSavePath == null)
            {
                ModEntry.ModMonitor.Log("An attempt to read the Id map was made without loading a save.");
                return;
            }

            string idMapPath = GetIdMapPath();
            string idMap = File.ReadAllText(idMapPath);
            try
            {
                IdMap = (List<IdMap>)JsonConvert.DeserializeObject(idMap, typeof(List<IdMap>));
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"IdMap couldn't be deserialized. IdMap will be ignored. This could cause Id shifts, items may be different from last save.\n Path:{idMapPath}\n{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
            }
        }

        /// <summary>Write / Create a new id to the IdMap file.</summary>
        /// <param name="id">The id to add to the idMap file.</param>
        /// <param name="name">The outerwear name corrosponding to the id.</param>
        private void AddItemToIdMapFile(int id, string name)
        {
            string idMapPath = GetIdMapPath();
            IdMap.Add(new IdMap(id, name));
            File.WriteAllText(idMapPath, JsonConvert.SerializeObject(IdMap));
        }

        /// <summary>Get the path to the idMap file.</summary>
        /// <returns>The idMap file path.</returns>
        private string GetIdMapPath()
        {
            var outerwearFolderPath = GetIdMapDirectory();
            var idsPath = Path.Combine(outerwearFolderPath, "ids.json");
            if (!File.Exists(idsPath))
            {
                File.Create(idsPath).Close();
            }

            return idsPath;
        }

        /// <summary>Get the idMap directory.</summary>
        /// <returns>The idMap directory.</returns>
        private string GetIdMapDirectory()
        {
            var outerwearPath = Path.Combine(Constants.CurrentSavePath, "Outerwear");
            if (!Directory.Exists(outerwearPath))
                Directory.CreateDirectory(outerwearPath);

            return outerwearPath;
        }
    }
}
