/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/


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
