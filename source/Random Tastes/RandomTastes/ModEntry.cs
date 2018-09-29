using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace RandomTastes
{
    public class ModData
    {
        public class ModDataSave
        {
            public class ModDataEntry
            {
                public string id = "";
                public int[] love = { };
                public int[] like = { };
                public int[] neutral = { };
                public int[] dislike = { };
                public int[] hate = { };

                public ModDataEntry(string id, int[] love, int[] like, int[] neutral, int[] dislike, int[] hate)
                {
                    this.id = id;
                    this.love = love;
                    this.like = like;
                    this.neutral = neutral;
                    this.dislike = dislike;
                    this.hate = hate;
                }

                public ModDataEntry(string id, List<int> love, List<int> like, List<int> neutral, List<int> dislike, List<int> hate)
                {
                    this.id = id;
                    this.love = love.ToArray();
                    this.like = like.ToArray();
                    this.neutral = neutral.ToArray();
                    this.dislike = dislike.ToArray();
                    this.hate = hate.ToArray();
                }

                public ModDataEntry()
                {

                }
            }

            public ModDataEntry[] entries = { };
            public string saveName = "";
            public bool enabled = true;

            public ModDataSave() { }
            public ModDataSave(ModDataEntry[] entries) { this.entries = entries; }
            public ModDataSave(List<ModDataEntry> entries) { this.entries = entries.ToArray(); }
        }

        public ModDataSave[] saves = { };

        public ModData() { }
    }

    /// <summary>The XNB editor.</summary>
    public class Editor : IAssetEditor
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public Editor(IModHelper helper, IMonitor monitor = null)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/NPCGiftTastes"); // change gift tastes
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (!Context.IsWorldReady) return; // If we aren't in-game, don't edit anything.

            ModData modData = Helper.ReadJsonFile<ModData>("saves.json") ?? new ModData(); // Load save file, or create new one.
            ModData.ModDataSave dataSave = Array.Find<ModData.ModDataSave>(modData.saves, x => x.saveName == Constants.SaveFolderName); // Load data for specific save.

            bool elemNotFound = false;
            bool shouldUpdate = false;

            if (dataSave == null) // If the savefile element was not found
            {
                dataSave = new ModData.ModDataSave();
                dataSave.saveName = Constants.SaveFolderName; // if element wasn't found, prepare one.
                elemNotFound = true;
            }

            if (!dataSave.enabled) return; // If RandomTastes isn't enabled for this save.

            asset
                .AsDictionary<string, string>()
                .Set((id, data) =>
                {
                    if (id.StartsWith("Universal_")) return data; // leave universal tastes alone

                    ModData.ModDataSave.ModDataEntry entry = Array.Find<ModData.ModDataSave.ModDataEntry>(dataSave.entries, x => x.id == id); // find entry

                    bool entryNotFound = false;

                    if (entry == null) // If the entry was not found
                    {
                        // generate new entry
                        entry = GenerateTastes(id);
                        entryNotFound = true;
                    }

                    string[] fields = data.Split('/');

                    for (int i = 0; i < 5; i++)
                    {
                        List<int> selected = new List<int>();
                        int fieldIndex = 1;

                        switch (i)
                        {
                            case 0:
                                selected = new List<int>(entry.love);
                                fieldIndex = 1;
                                break;
                            case 1:
                                selected = new List<int>(entry.like);
                                fieldIndex = 3;
                                break;
                            case 2:
                                selected = new List<int>(entry.neutral);
                                fieldIndex = 9;
                                break;
                            case 3:
                                selected = new List<int>(entry.dislike);
                                fieldIndex = 5;
                                break;
                            case 4:
                                selected = new List<int>(entry.hate);
                                fieldIndex = 7;
                                break;
                            default:
                                this.Monitor.Log($"Uhh... This doesn't seem right... ({i})", LogLevel.Error);
                                break;
                        }

                        this.Monitor.Log($"{i} : {string.Join(" ", selected)}", LogLevel.Trace);

                        fields[fieldIndex] = string.Join(" ", selected);
                    }

                    if (entryNotFound) // If the entry was not found
                    {
                        // add entry to savefile
                        List<ModData.ModDataSave.ModDataEntry> entryList = new List<ModData.ModDataSave.ModDataEntry>(dataSave.entries);
                        entryList.Add(entry);
                        dataSave.entries = entryList.ToArray();
                        shouldUpdate = true;
                    }

                    return string.Join("/", fields);
                });

            if (elemNotFound) // If the savefile element was not found
            {
                // add entry to savefile
                List<ModData.ModDataSave> modSaves = new List<ModData.ModDataSave>(modData.saves);
                modSaves.Add(dataSave);
                modData.saves = modSaves.ToArray();
            }

            shouldUpdate = shouldUpdate || elemNotFound; // Calculate elemNotFound into shouldUpdate

            if (shouldUpdate)
            {
                this.Helper.WriteJsonFile("saves.json", modData); // Update mod data file
            }
        }

        private ModData.ModDataSave.ModDataEntry GenerateTastes(string id)
        {
            List<int> love = new List<int>();
            List<int> like = new List<int>();
            List<int> neutral = new List<int>();
            List<int> dislike = new List<int>();
            List<int> hate = new List<int>();

            Random rnd = new Random();
            int count = Helper.Content.Load<Dictionary<int, string>>(@"Data\ObjectInformation", ContentSource.GameContent).Count;

            for (int i = 0; i < 5; i++) // generate in in 5 taste categories
            {
                for (int x = 0; x < 6; x++) // generate 6 tastes
                {
                    int selectedGift = rnd.Next(count);

                    if (love.Contains(selectedGift) ||
                        like.Contains(selectedGift) ||
                        neutral.Contains(selectedGift) ||
                        dislike.Contains(selectedGift) ||
                        hate.Contains(selectedGift)) // If taste was already decided for this item
                    {
                        x--; // Try again.
                    }
                    else
                    {
                        // category decision
                        if (i == 0) love.Add(selectedGift);
                        else if (i == 1) like.Add(selectedGift);
                        else if (i == 2) neutral.Add(selectedGift);
                        else if (i == 3) dislike.Add(selectedGift);
                        else if (i == 4) hate.Add(selectedGift);

                        if (this.Monitor != null) Monitor.Log($"{id}-{i}-{x}: {selectedGift}", LogLevel.Trace);
                    }
                }
            }

            return new ModData.ModDataSave.ModDataEntry(id, love, like, neutral, dislike, hate);
        }
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        Editor editor;
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            editor = new Editor(helper, Monitor);
            Helper.Content.AssetEditors.Add(editor);
            SaveEvents.AfterLoad += this.AfterLoad;
        }

        private void AfterLoad(object sender, EventArgs e)
        {
            Helper.Content.InvalidateCache("Data/NPCGiftTastes"); // Clear old NPCGiftTastes cache, to be updated w/ the Editor.
        }
    }
}