
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace StardewNewsFeed.Wrappers {
    public class Farm : IFarm {

        private readonly StardewValley.Farm _farm;

        public Farm(StardewValley.Farm farm) {
            _farm = farm;
        }

        public IList<ILocation> GetBuildings<T>(ITranslationHelper translationHelper) {
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
