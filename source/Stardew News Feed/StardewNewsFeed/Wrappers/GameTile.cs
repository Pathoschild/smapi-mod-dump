/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/


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
