
namespace StardewNewsFeed.Wrappers {
    public class TerrainFeature : ITerrainFeature {

        private readonly StardewObject _object;

        public TerrainFeature(StardewObject obj) {
            _object = obj;
        }

        public bool IsReadyForHarvest() {
            return _object.IsReadyForHarvest();
        }
    }
}
