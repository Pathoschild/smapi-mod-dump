using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyMods.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// The class responsible for producing data to be saved.
    /// </summary>
    class Saver
    {
        private readonly ISemanticVersion Version;
        private readonly IChestDataManager ChestDataManager;

        public Saver(ISemanticVersion version, IChestDataManager chestDataManager)
        {
            Version = version;
            ChestDataManager = chestDataManager;
        }

        /// <summary>
        /// Generates save data for the current state of the world and returns
        /// a JSON string representing that save data.
        /// </summary>
        public string DumpData()
        {
            var data = GetSerializableData();
            return JsonConvert.SerializeObject(data, new StringEnumConverter());
        }

        /// <summary>
        /// Build save data for the current game state.
        /// </summary>
        private SaveData GetSerializableData()
        {
            return new SaveData
            {
                Version = Version.ToString(),
                ChestEntries = BuildChestEntries()
            };
        }

        private IEnumerable<ChestEntry> BuildChestEntries()
        {
            foreach (var location in Game1.locations)
            {
                foreach (var pair in GetLocationChests(location))
                {
                    yield return new ChestEntry
                    {
                        Address = new ChestAddress
                        {
                            LocationType = ChestLocationType.Normal,
                            LocationName = location.Name,
                            Tile = pair.Key,
                        },
                        AcceptedItemKinds = ChestDataManager.GetChestData(pair.Value).AcceptedItemKinds,
                    };
                }

                // chests in constructed buildings
                if (location is BuildableGameLocation buildableLocation)
                {
                    var buildings = buildableLocation.buildings.Where(b => b.indoors != null);

                    foreach (var building in buildings)
                    {
                        foreach (var pair in GetLocationChests(building.indoors))
                        {
                            yield return new ChestEntry
                            {
                                Address = new ChestAddress
                                {
                                    LocationType = ChestLocationType.Building,
                                    LocationName = location.Name,
                                    BuildingName = building.nameOfIndoors,
                                    Tile = pair.Key,
                                },
                                AcceptedItemKinds = ChestDataManager.GetChestData(pair.Value).AcceptedItemKinds,
                            };
                        }
                    }
                }

                if (location is FarmHouse farmHouse && Game1.player.HouseUpgradeLevel >= 1)
                {
                    var chest = farmHouse.fridge;

                    yield return new ChestEntry
                    {
                        Address = new ChestAddress
                        {
                            LocationType = ChestLocationType.Refrigerator
                        },
                        AcceptedItemKinds = ChestDataManager.GetChestData(chest).AcceptedItemKinds
                    };
                }
            }
        }

        /// <summary>
        /// Retrieve a collection of the chest objects present in the given
        /// location, keyed by their tile location.
        /// </summary>
        private IDictionary<Vector2, Chest> GetLocationChests(GameLocation location)
        {
            return location.Objects
                .Where(pair => pair.Value is Chest && ((Chest) pair.Value).playerChest)
                .ToDictionary(pair => pair.Key, pair => (Chest) pair.Value);
        }
    }
}