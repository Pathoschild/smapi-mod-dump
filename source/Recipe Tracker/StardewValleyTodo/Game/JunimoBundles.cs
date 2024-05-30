/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValleyTodo.Helpers;

namespace StardewValleyTodo.Game {
    public class JunimoBundles {
        private CommunityCenter _communityCenter;
        private List<JunimoBundle> _junimoBundles;

        /// <summary>
        /// Invokes when an item was donated into bundle.
        /// </summary>
        public event EventHandler<JunimoBundleIngredient> ItemDonated;

        /// <summary>
        /// Invokes when a bundle was completed.
        /// </summary>
        public event EventHandler<JunimoBundle> BundleCompleted;

        public JunimoBundles() {
            Startup();
        }

        private void Startup() {
            _communityCenter = (CommunityCenter) Game1.getLocationFromName("CommunityCenter");
            _junimoBundles = new List<JunimoBundle>();

            var raw = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");

            foreach (var rawBundle in raw) {
                // Bulletin Board/35
                var parsedKey = BundleStringParser.ParseKey(rawBundle.Key);
                var roomId = parsedKey.SpriteIndex;

                // Fodder/BO 104 1/262 10 0 178 10 0 613 3 0/3///Кормовой
                var parsedValue = BundleStringParser.ParseValue(rawBundle.Value);
                var ingredientsString = parsedValue.Ingredients;
                var slots = parsedValue.NumberOfItems;
                var localeName = parsedValue.DisplayName;

                var bundle = new JunimoBundle(roomId, localeName, slots);
                _junimoBundles.Add(bundle);

                var netbundle = _communityCenter.bundles[roomId];
                var rawIngredients = ingredientsString.Split(' ');
                for (var i = 0; i < rawIngredients.Length; i += 3) {
                    var objectId = rawIngredients[i];
                    var objectCount = int.Parse(rawIngredients[i + 1]);
                    var objectQuality = int.Parse(rawIngredients[i + 2]);

                    // Skip Money bundles
                    if (objectId == "-1") {
                        continue;
                    }

                    var displayNameRaw = Game1.objectData[objectId].DisplayName;
                    var displayName = LocalizedStringLoader.Load(displayNameRaw);

                    var isDonated = netbundle[i / 3];
                    var ingredient = new JunimoBundleIngredient(objectId, i / 3, displayName, objectCount, objectQuality, isDonated);
                    bundle.AddIngredient(ingredient);
                }

                if (bundle.Slots == 0) {
                    bundle.Slots = bundle.Ingredients.Count;
                }

                if (netbundle.All(x => x)) {
                    bundle.IsComplete = true;
                }
            }
        }

        public void Update() {
            var net = _communityCenter.bundles;

            foreach (var bundle in _junimoBundles.Where(x => !x.IsComplete)) {
                var netbundle = net[bundle.RoomId];
                foreach (var ingredient in bundle.Ingredients) {
                    if (netbundle[ingredient.Index] && !ingredient.IsDonated) {
                        ingredient.IsDonated = true;
                        ItemDonated?.Invoke(this, ingredient);
                    }
                }

                if (bundle.Ingredients.Count(x => x.IsDonated) >= bundle.Slots) {
                    bundle.IsComplete = true;
                    BundleCompleted?.Invoke(this, bundle);
                }
            }
        }

        /// <summary>
        /// Returns a bundle with specified name.
        /// </summary>
        /// <param name="displayName">Localized name</param>
        /// <returns>Bundle</returns>
        public JunimoBundle Find(string displayName) {
            return _junimoBundles.Find(x => x.Name == displayName);
        }
    }
}
