using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Linq;
using StardewValleyMods.CategorizeChests.Framework.Persistence;

namespace StardewValleyMods.CategorizeChests.Framework
{
    class ChestFinder : IChestFinder
    {
        public Chest GetChestByAddress(ChestAddress address)
        {
            if (address.LocationType == ChestLocationType.Refrigerator)
            {
                var farmHouse = (FarmHouse) Game1.locations.First(l => l is FarmHouse);

                if (Game1.player.HouseUpgradeLevel < 1)
                    throw new InvalidSaveDataException(
                        "Chest save data contains refrigerator data but no refrigerator exists");

                return farmHouse.fridge;
            }
            else
            {
                var location = GetLocationFromAddress(address);

                if (location.objects.ContainsKey(address.Tile) && location.objects[address.Tile] is Chest chest)
                {
                    return location.objects[address.Tile] as Chest;
                }
                else
                {
                    throw new InvalidSaveDataException($"Can't find chest in {location.Name} at {address.Tile}");
                }
            }
        }

        private GameLocation GetLocationFromAddress(ChestAddress address)
        {
            var location = Game1.locations.FirstOrDefault(l => l.Name == address.LocationName);

            if (location == null)
                throw new InvalidSaveDataException($"Can't find location named {address.LocationName}");

            if (address.LocationType == ChestLocationType.Building)
            {
                if (location is BuildableGameLocation buildableLocation)
                {
                    var building = buildableLocation.buildings // TODO: check
                        .FirstOrDefault(b => b.nameOfIndoors == address.BuildingName);

                    if (building == null)
                        throw new InvalidSaveDataException(
                            $"Can't find building named {address.BuildingName} in location named {location.Name}");

                    return building.indoors;
                }
                else
                {
                    throw new InvalidSaveDataException($"Can't find any buildings in location named {location.Name}");
                }
            }
            else
            {
                return location;
            }
        }
    }
}