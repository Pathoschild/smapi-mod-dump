/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/GiftTasteHelper
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace GiftTasteHelper.Framework
{
    /// <summary>Database for storing NPC gift tastes.</summary>
    internal class GiftDatabase : IGiftDatabase
    {
        public event DataSourceChangedDelegate? DatabaseChanged;

        public GiftDatabaseModel Database { get; protected set; }
        protected readonly IModHelper Helper;

        public GiftDatabase(IModHelper helper)
        {
            this.Helper = helper;
            this.Database = new GiftDatabaseModel();
        }

        public GiftDatabase(IModHelper helper, GiftDatabaseModel database)
        {
            this.Helper = helper;
            this.Database = database;
        }

        /// <summary>Returns if the database has an item for a particular NPC stored.</summary>
        public bool ContainsGift(string npcName, string itemId, GiftTaste taste)
        {
            if (taste == GiftTaste.MAX)
            {
                return false;
            }
            return Database.Entries[npcName].Contains(taste, itemId);
        }

        /// <summary>Adds an item for an npc to the database.</summary>
        public virtual bool AddGift(string npcName, string itemId, GiftTaste taste)
        {
            if (taste == GiftTaste.MAX)
            {
                return false;
            }

            bool check = true;
            if (!Database.Entries.ContainsKey(npcName))
            {
                Database.Entries.Add(npcName, new CharacterTasteModel());
                check = false;
            }

            if (!check || !ContainsGift(npcName, itemId, taste))
            {
                Utils.DebugLog($"Adding {itemId} to {npcName}'s {taste} tastes.");
                Database.Entries[npcName].Add(taste, new GiftModel(itemId));

                DatabaseChanged?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>Adds a range of items for an npc to the database.</summary>
        public virtual bool AddGifts(string npcName, GiftTaste taste, string[] itemIds)
        {
            if (taste == GiftTaste.MAX)
            {
                return false;
            }

            if (!Database.Entries.ContainsKey(npcName))
            {
                Database.Entries.Add(npcName, new CharacterTasteModel());
            }

            // Add only the gifts that are not already in the DB.
            var unique = itemIds.Where(id => !ContainsGift(npcName, id, taste)).Select(id => id);
            if (unique.Any())
            {
                Database.Entries[npcName].AddRange(taste, itemIds.Select(id => new GiftModel(id)));
                DatabaseChanged?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>Returns all the gifts of the given taste in the database for that npc.</summary>
        public string[] GetGiftsForTaste(string npcName, GiftTaste taste)
        {
            if (Database.Entries.ContainsKey(npcName))
            {
                if (Database.Entries[npcName].Entries.TryGetValue(taste, out var entryForTaste))
                {
                    return entryForTaste.Select(model => model.ItemId).ToArray();
                }
            }
            return Array.Empty<string>();
        }
    }

    /// <summary>A gift database that is stored on disk.</summary>
    internal class StoredGiftDatabase : GiftDatabase
    {
        public static string DBRoot => "DB";
        public static string DBFileName => "GiftDatabase.json";

        private readonly string DBPath;

        public StoredGiftDatabase(IModHelper helper, string path)
            : base(helper, helper.Data.ReadJsonFile<GiftDatabaseModel>(path) ?? new GiftDatabaseModel())
        {
            Utils.DebugLog($"Setting DB path to {path}", LogLevel.Info);
            this.DBPath = path;

            Utils.DebugLog($"Database version: {this.Database.Version}", LogLevel.Info);
            if (this.Database.Version.IsOlderThan(GiftDatabaseModel.CurrentVersion))
            {
                // Any upgrade path stuff that would need to happen can go here.
                // Likelyhood of needing to do this is pretty rare though.
                Utils.DebugLog($"Upgrading DB version from {this.Database.Version} to {GiftDatabaseModel.CurrentVersion}", LogLevel.Info);
                this.Database.Version = GiftDatabaseModel.CurrentVersion;
                Write();
            }

            // Load existing gift data.
            AddSaveGameGifts();
        }

        private void AddSaveGameGifts()
        {
            foreach (var npcName in Game1.NPCGiftTastes.Keys)
            {
                if (!Game1.player.giftedItems.ContainsKey(npcName))
                {
                    continue;
                }

                var tasteItems = new Dictionary<GiftTaste, List<string>>();
                foreach (var item in Game1.player.giftedItems[npcName].Keys)
                {
                    var taste = Utils.GetTasteForGift(npcName, item);
                    if (taste is GiftTaste.MAX)
                    {
                        continue;
                    }

                    if (tasteItems.TryGetValue(taste, out var tastes))
                    {
                        tastes.Add(item);
                    }
                    else
                    {
                        tasteItems[taste] = new List<string> { item };
                    }
                }

                foreach (var (taste, gifts) in tasteItems)
                {
                    AddGifts(npcName, taste, gifts.ToArray());
                }
            }
        }

        public static void MigrateDatabase(IModHelper helper, string fromPath, ref StoredGiftDatabase newDb)
        {
            GiftDatabaseModel? fromDatabaseModel = helper.Data.ReadJsonFile<GiftDatabaseModel>(fromPath);
            if (fromDatabaseModel is null)
            {
                return;
            }

            if (newDb.Database.Entries.Keys.Count == 0)
            {
                newDb.Database = fromDatabaseModel;
            }
            else
            {
                // Merge
                // TODO: Clean this up a bit.
                foreach (string npcName in fromDatabaseModel.Entries.Keys)
                {
                    CharacterTasteModel fromTasteModel = fromDatabaseModel.Entries[npcName];
                    if (!newDb.Database.Entries.ContainsKey(npcName))
                    {
                        newDb.Database.Entries.Add(npcName, fromTasteModel);
                        continue;
                    }

                    Dictionary<GiftTaste, List<GiftModel>> newEntries = newDb.Database.Entries[npcName].Entries;
                    Dictionary<GiftTaste, List<GiftModel>> fromEntries = fromTasteModel.Entries;
                    foreach (GiftTaste taste in fromEntries.Keys)
                    {
                        if (!newEntries.ContainsKey(taste))
                        {
                            newEntries.Add(taste, fromEntries[taste]);
                            continue;
                        }

                        newEntries[taste] = fromEntries[taste]
                            .Concat(newEntries[taste])
                            .DistinctBy(gift => gift.ItemId)
                            .ToList();
                    }
                }
            }
            newDb.Write();
        }

        public override bool AddGift(string npcName, string itemId, GiftTaste taste)
        {
            if (base.AddGift(npcName, itemId, taste))
            {
                Write();
                return true;
            }
            return false;
        }

        public override bool AddGifts(string npcName, GiftTaste taste, string[] itemIds)
        {
            if (base.AddGifts(npcName, taste, itemIds))
            {
                Write();
                return true;
            }
            return false;
        }

        private void Write()
        {
            Utils.DebugLog($"Writing gift database to: {this.DBPath}");
            Helper.Data.WriteJsonFile(DBPath, Database);
        }
    }
}
