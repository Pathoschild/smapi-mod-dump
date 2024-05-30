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
using HarmonyLib;
using StardewValley;
using Object = StardewValley.Object;
using StardewValley.GameData.Machines;
using Microsoft.Xna.Framework;

namespace ContextTagRemover {
    internal class ModEntry : Mod {

        Dictionary<string, List<string>> DataObjectsTags = new Dictionary<string, List<string>>();

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
                        if (DataObjectsTags.ContainsKey(itemID)) {
                            foreach (var tag in DataObjectsTags[itemID]) {
                                itemData.ContextTags.Remove(tag);
                            }
                        }
                    }
                });
            }
        }

        void FillDictionaries() {
            DataObjectsTags.Add("74", new List<string> { "crystalarium_banned" });
            //DataObjectsTags.Add("MysteryBox", new List<string> { "geode_crusher_ignored" });
            //DataObjectsTags.Add("GoldenMysteryBox", new List<string> { "geode_crusher_ignored" });
            DataObjectsTags.Add("275", new List<string> { "geode_crusher_ignored" });
            DataObjectsTags.Add("791", new List<string> { "geode_crusher_ignored" });

            dictionariesFilled = true;
        }

    }
}
