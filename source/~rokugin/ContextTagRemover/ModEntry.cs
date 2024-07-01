/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;

namespace ContextTagRemover {
    internal class ModEntry : Mod {

        readonly string objectTagsPath = "assets/objectTags.json";

        TagRemoverContentLoader tagRemoverContentLoader = null!;

        Dictionary<string, List<string>> dataObjectsTags = new Dictionary<string, List<string>>();

        bool dictionariesFilled = false;

        public override void Entry(IModHelper helper) {
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
            if (!dictionariesFilled) FillDictionaries();

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, ObjectData>().Data;

                    foreach ((string itemID, ObjectData itemData) in data) {
                        if (dataObjectsTags.ContainsKey(itemID)) {
                            foreach (var tag in dataObjectsTags[itemID]) {
                                itemData.ContextTags.Remove(tag);
                            }
                        }
                    }
                });
            }
        }

        void FillDictionaries() {
            dataObjectsTags.Add("74", new List<string> { "crystalarium_banned" });
            //DataObjectsTags.Add("MysteryBox", new List<string> { "geode_crusher_ignored" });
            //DataObjectsTags.Add("GoldenMysteryBox", new List<string> { "geode_crusher_ignored" });
            dataObjectsTags.Add("275", new List<string> { "geode_crusher_ignored" });
            dataObjectsTags.Add("791", new List<string> { "geode_crusher_ignored" });

            dictionariesFilled = true;
        }

    }
}
