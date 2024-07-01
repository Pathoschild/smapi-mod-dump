/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretNoteFramework
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using System;
using System.Collections.Generic;

namespace ichortower.SNF
{
    internal class SecretModNotes
    {
        public static string NotesAsset = $"Mods/{SNF.ModId}/Notes";
        public static string DefaultObjectId = $"(O){SNF.ModId}_DefaultNote";

        public static HashSet<string> ActiveObjectIds = new();
        public static HashSet<string> AvailableNoteIds = new();

        private static Dictionary<string, SecretModNoteData> _data;
        public static Dictionary<string, SecretModNoteData> Data
        {
            get {
                return _data;
            }
            set {
                _data = value;
                ActiveObjectIds.Clear();
                foreach (var entry in _data) {
                    ActiveObjectIds.Add(ItemRegistry.QualifyItemId(
                            entry.Value.ObjectId ?? DefaultObjectId));
                }
            }
        }

        public static void RefreshAvailableNotes()
        {
            AvailableNoteIds.Clear();
            if (Data is null) {
                return;
            }
            foreach (var entry in Data) {
                if (GameStateQuery.CheckConditions(entry.Value.Conditions)) {
                    AvailableNoteIds.Add(entry.Key);
                }
            }
        }

        public static Dictionary<string, SecretModNoteData> Load(
                LocalizedContentManager content)
        {
            var data = content.Load<Dictionary<string, SecretModNoteData>>(NotesAsset);
            foreach (var entry in data) {
                if (String.IsNullOrEmpty(entry.Value.ObjectId)) {
                    continue;
                }
                string qid = ItemRegistry.QualifyItemId(entry.Value.ObjectId);
                if (qid == null) {
                    Log.Warn($"In note entry '{entry.Key}': failed to qualify" +
                            $" item id '{entry.Value.ObjectId}'");
                    continue;
                }
                entry.Value.ObjectId = qid;
            }
            return data;
        }

        public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(NotesAsset)) {
                e.LoadFrom(() => new Dictionary<string, SecretModNoteData>(),
                        AssetLoadPriority.Exclusive);
            }
            else if (e.Name.IsEquivalentTo("Data/Objects")) {
                var modAsset = SecretNoteFramework.instance.Helper.ModContent.Load
                        <Dictionary<string, ObjectData>>("assets/items.json");
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, ObjectData>();
                    foreach (var entry in modAsset) {
                        dict.Data[entry.Key] = entry.Value;
                    }
                });
            }
            else if (e.Name.IsEquivalentTo("Strings/Objects")) {
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, string>();
                    dict.Data[$"{SNF.ModId}_DefaultNote_Name"] =
                            TR.Get("Objects.DefaultNote.Name");
                    dict.Data[$"{SNF.ModId}_DefaultNote_Description"] =
                            TR.Get("Objects.DefaultNote.Description");
                });
            }
            else if (e.Name.IsEquivalentTo($"Mods/{SNF.ModId}/DefaultNoteTexture")) {
                e.LoadFrom(() => {
                    return SecretNoteFramework.instance.Helper.ModContent.Load
                            <Texture2D>("assets/icon_default.png");
                }, AssetLoadPriority.Medium);
            }
        }

        public static void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo(NotesAsset)) {
                Data = Load(Game1.content);
            }
        }

        public static void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var name in e.Names) {
                if (name.IsEquivalentTo(NotesAsset)) {
                    Log.Trace("Invalidating note data");
                    Data = Load(Game1.content);
                }
            }
        }

    }

    internal class SecretModNoteData
    {
        public string Contents = "";
        public string Title = null;
        public string Conditions = null;
        public string LocationContext = "!Island";
        public string ObjectId = null;
        public string NoteTexture = null;
        public int NoteTextureIndex = 0;
        public string NoteTextColor = null;
        public string NoteImageTexture = null;
        public int NoteImageTextureIndex = -1;
        public List<string> ActionsOnFirstRead = new();
    }
}
