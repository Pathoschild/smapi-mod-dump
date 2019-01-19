using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewNewsFeed.Extensions;
using StardewValley;
using StardewValley.Buildings;

namespace StardewNewsFeed.Wrappers {
    public class Farm : IFarm {
        private readonly StardewValley.Farm _farm;

        public IEnumerable<FarmAnimal> BarnAnimalsWithAvailableProduce {
            get {
                var list = Game1.getFarm().animals.Values.ToList();
                foreach (Building building in Game1.getFarm().buildings.Where(_ => _.buildingType.Value.EndsWith("Barn"))) {
                    if (building.indoors.Value != null && building.indoors.Value.GetType() == typeof(AnimalHouse))
                        list.AddRange(((AnimalHouse)building.indoors.Value).animals.Values.ToList());
                }
                return list.Where(_ => _.HasAvailableProduce());
            }
        }

        public Farm(StardewValley.Farm farm) {
            _farm = farm;
        }

        public IEnumerable<ILocation> GetBuildings<T>(ITranslationHelper translationHelper) {
            var buildings = _farm.buildings
                .Select(b => b.indoors.Value)
                .Where(i => i is T)
                .ToList();

            foreach(var b in _farm.buildings) {
                if(b is T) {
                    buildings.Add(b.indoors.Value);
                }
            }

            return buildings.Select(b => new Location(b, translationHelper) as ILocation).ToList();
        }
    }
}
