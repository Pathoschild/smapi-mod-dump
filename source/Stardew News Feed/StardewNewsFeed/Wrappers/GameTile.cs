
using Netcode;

namespace StardewNewsFeed.Wrappers {
    public class GameTile : IGameTile {

        private readonly IStardewObject _objectAtTile;

        public GameTile(ILocation location, int height, int width) {
            _objectAtTile = location.GetObjectAtTile(height, width);
        }

        public bool IsReadyForHarvest() {
            if(_objectAtTile == null) {
                return false;
            }

            return _objectAtTile.IsReadyForHarvest();
        }
    }
}
