/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SkillPrestige.Framework.JsonNet.PrivateSettersContractResolvers;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>Represents the old save file data for the Skill Prestige Mod.</summary>
    [Obsolete]
    [Serializable]
    internal class PrestigeSaveData
    {
        private const string DataFileName = "Data.json";
        private static readonly string DataFilePath = Path.Combine(ModEntry.ModPath, DataFileName);
        private static bool NoDataFileFound { get; set; }

        public static bool MigrationCompleted { get; set; }
        public static PrestigeSet CurrentlyLoadedPrestigeSet => Instance.PrestigeSaveFiles[CurrentlyLoadedSaveFileUniqueId];
        private static ulong CurrentlyLoadedSaveFileUniqueId { get; set; }

        // ReSharper disable once InconsistentNaming
        private static PrestigeSaveData _instance;

        /// <summary>Set of prestige data saved per save file unique ID.</summary>
        // ReSharper disable once MemberCanBePrivate.Global - no, it can't be made private or it won't be serialized.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global - setter used by deserializer.
        public IDictionary<ulong, PrestigeSet> PrestigeSaveFiles { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global - used publically, resharper is wrong.
        public static PrestigeSaveData Instance
        {
            get => _instance ??= new PrestigeSaveData();
            // ReSharper disable once UnusedMember.Global - used by deseralizer.
            set => _instance = value;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static")]
        public void Save()
        {
            Logger.LogInformation("Writing prestige save data to disk...");
            File.WriteAllLines(DataFilePath, new[] { JsonConvert.SerializeObject(Instance) });
            Logger.LogInformation("Prestige save data written to disk.");
        }

        public static void CleanupDataFile()
        {
            if (NoDataFileFound) return;
            if (MigrationCompleted)
            {
                Logger.LogInformation($"Data migration for game id {CurrentlyLoadedSaveFileUniqueId} complete, deleting the entry from data.json...");
                if (Instance.PrestigeSaveFiles.Remove(CurrentlyLoadedSaveFileUniqueId))
                {
                    Instance.Save();
                    Logger.LogInformation($"entry deleted.");
                }
                else
                    Logger.LogWarning($"After migration, system was unable to delete entry in data.json file for {CurrentlyLoadedSaveFileUniqueId}, please consider manually removing it from the data file.");
                MigrationCompleted = false;
            }
            if(Instance.PrestigeSaveFiles.Count != 0) return;
            Logger.LogDisplay("No further legacy data found, attempting to remove data.json file...");
            try
            {
                File.Delete(DataFilePath);
                Logger.LogDisplay($"Data file deleted.");
            }
            catch (IOException ex)
            {
                Logger.LogWarning(
                    "Unable to delete data.json, please manually delete the file after confirming it has no contents.");
                Logger.LogWarning(ex.Message);
            }
        }

        public static void MigrateData()
        {
            if (Instance.PrestigeSaveFiles.Count <= 0
                || CurrentlyLoadedSaveFileUniqueId != Game1.uniqueIDForThisGame) return;
            var queuedPrestigeEntriesToAdd = new List<Prestige>();
            if (!Instance.PrestigeSaveFiles.ContainsKey(CurrentlyLoadedSaveFileUniqueId)) return;
            PrestigeSet.Instance ??= PrestigeSet.CompleteEmptyPrestigeSet();
            Logger.LogInformation($"Beginning data migration for game id: {Game1.uniqueIDForThisGame}");
            foreach (var entry in CurrentlyLoadedPrestigeSet.Prestiges)
            {
                Logger.LogVerbose($"Migrating prestige information for {entry.SkillType.Name} skill");
                var newPrestigeDataSet = PrestigeSet.Instance.Prestiges.ToList();
                var newPrestigeData = newPrestigeDataSet.FirstOrDefault(x => x.SkillType.Name == entry.SkillType.Name);
                if (newPrestigeData != null)
                {
                    Logger.LogVerbose($"Migrating prestige points: {entry.PrestigePoints} from original data.");
                    newPrestigeData.PrestigePoints = Math.Max(entry.PrestigePoints, newPrestigeData.PrestigePoints);
                    Logger.LogVerbose($"Points set to {newPrestigeData.PrestigePoints}");
                    Logger.LogVerbose("Migrating professions...");
                    foreach (int profession in entry.PrestigeProfessionsSelected.Where(x => !newPrestigeData.PrestigeProfessionsSelected.Contains(x)))
                    {
                        Logger.LogVerbose($"Found profession id {profession} in data file, migrating to new data structure.");
                        newPrestigeData.PrestigeProfessionsSelected.Add(profession);
                    }
                    Logger.LogVerbose("Migrating crafting recipes...");
                    foreach (var craftingRecipeCount in entry.CraftingRecipeAmountsToSave.Where(x => !newPrestigeData.CraftingRecipeAmountsToSave.ContainsKey(x.Key)))
                    {
                        Logger.LogVerbose($"Found crafting recipe count data for {craftingRecipeCount.Key}, total of {craftingRecipeCount.Value}, migrating to new data structure.");
                        newPrestigeData.CraftingRecipeAmountsToSave.Add(craftingRecipeCount.Key, craftingRecipeCount.Value);
                    }
                    Logger.LogVerbose("Migrating cooking recipes...");
                    foreach (var cookingRecipeCount in entry.CookingRecipeAmountsToSave.Where(x => !newPrestigeData.CookingRecipeAmountsToSave.ContainsKey(x.Key)))
                    {
                        Logger.LogVerbose($"Found cooking recipe count for {cookingRecipeCount.Key}, total of {cookingRecipeCount.Value}, migrating to new data structure.");
                        newPrestigeData.CookingRecipeAmountsToSave.Add(cookingRecipeCount.Key, cookingRecipeCount.Value);
                    }
                }
                else
                {
                    Logger.LogVerbose($"Prestige data for {entry.SkillType.Name} not found, adding to queue for new data.");
                    queuedPrestigeEntriesToAdd.Add(entry);
                }

            }

            if (queuedPrestigeEntriesToAdd.Any())
            {
                Logger.LogVerbose("Adding queued prestige entries to data.");
                var newPrestigeList = PrestigeSet.Instance.Prestiges.ToList();
                newPrestigeList.AddRange(queuedPrestigeEntriesToAdd);
                PrestigeSet.Instance.Prestiges = newPrestigeList;
            }
            MigrationCompleted = true;
        }

        public static void Read()
        {
            if (!File.Exists(DataFilePath))
            {
                Logger.LogInformation("No Old Save Method Data file found, using new save method");
                NoDataFileFound = true;
                return;
            }
            Logger.LogInformation("Deserializing prestige save data...");
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            _instance = JsonConvert.DeserializeObject<PrestigeSaveData>(File.ReadAllText(DataFilePath), settings);
            Logger.LogInformation("Prestige save data loaded from data.json file.");
        }

        public void UpdateCurrentSaveFileInformation()
        {
            if (CurrentlyLoadedSaveFileUniqueId == Game1.uniqueIDForThisGame)
                return;
            Logger.LogInformation("Save file change detected.");
            CurrentlyLoadedSaveFileUniqueId = Game1.uniqueIDForThisGame;
            Read();
        }

        private PrestigeSaveData()
        {
            this.PrestigeSaveFiles = new Dictionary<ulong, PrestigeSet>();
            Logger.LogInformation("Created new prestige save data instance.");
        }
    }
}
