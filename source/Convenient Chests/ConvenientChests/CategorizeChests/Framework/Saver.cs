/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using ConvenientChests.CategorizeChests.Framework.Persistence;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Framework {
    /// <summary>
    /// The class responsible for producing data to be saved.
    /// </summary>
    class Saver {
        private readonly ISemanticVersion Version;
        private readonly IChestDataManager ChestDataManager;

        public Saver(ISemanticVersion version, IChestDataManager chestDataManager) {
            Version = version;
            ChestDataManager = chestDataManager;
        }

        /// <summary>
        /// Build save data for the current game state.
        /// </summary>
        public SaveData GetSerializableData() {
            return new SaveData {
                Version = Version.ToString(),
                ChestEntries = BuildChestEntries().Where(e => e.AcceptedItems.Any()).ToList(),
            };
        }

        private IEnumerable<ChestEntry> BuildChestEntries() {
            foreach (var location in Game1.locations) {
                // chests
                foreach (var pair in GetLocationChests(location)) {
                    yield return new ChestEntry(ChestDataManager.GetChestData(pair.Value), new ChestAddress(location.Name, pair.Key));
                }

                switch (location) {

                    // fridges
                    case FarmHouse farmHouse when farmHouse.upgradeLevel >= 1:
                        yield return new ChestEntry(
                                                    ChestDataManager.GetChestData(farmHouse.fridge.Value),
                                                    new ChestAddress { LocationName = farmHouse.uniqueName?.Value ?? farmHouse.Name, LocationType = ChestLocationType.Refrigerator }
                                                   );
                        break;

                    // buildings
                    case GameLocation buildableLocation:
                        foreach (var building in buildableLocation.buildings.Where(b => b.indoors.Value != null))
                        foreach (var pair in GetLocationChests(building.indoors.Value))
                            yield return new ChestEntry(
                                                        ChestDataManager.GetChestData(pair.Value),
                                                        new ChestAddress(location.Name, pair.Key, ChestLocationType.Building, building.GetIndoorsName())
                                                       );
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieve a collection of the chest objects present in the given
        /// location, keyed by their tile location.
        /// </summary>
        private static IDictionary<Vector2, Chest> GetLocationChests(GameLocation location) =>
            location.Objects.Pairs
                    .Where(pair => pair.Value is Chest c && c.playerChest.Value)
                    .ToDictionary(
                                  pair => pair.Key,
                                  pair => (Chest) pair.Value
                                 );
    }
}