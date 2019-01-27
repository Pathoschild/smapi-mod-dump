using System;
using System.Linq;
using StardewValley;
using StardewModdingAPI;
using System.Collections.Generic;

namespace StardewNewsFeed.Wrappers {
    public class Location : ILocation {

        private readonly GameLocation _gameLocation;
        private readonly ITranslationHelper _translationHelper;

        public Location(GameLocation gameLocation, ITranslationHelper translationHelper) {
            _gameLocation = gameLocation ?? throw new ArgumentNullException(nameof(gameLocation));
            _translationHelper = translationHelper ?? throw new ArgumentNullException(nameof(translationHelper));
        }

        public string GetLocationName() {
            return _gameLocation.Name;
        }

        public string GetDisplayName() {
            if (_gameLocation.Name == Constants.FARM_CAVE_LOCATION_NAME) {
                return _translationHelper.Get("news-feed.cave-display-name");
            }
            return _gameLocation.Name;
        }

        public IStardewObject GetObjectAtTile(int height, int width) {
            return new StardewObject(_gameLocation.getObjectAtTile(height, width));
        }

        private IList<IStardewObject> GetObjects() {
            return _gameLocation.Objects.Values.Select(o => new StardewObject(o) as IStardewObject).ToList();
        }

        private IList<ITerrainFeature> GetTerrainFeatures<T>() where T : StardewValley.TerrainFeatures.TerrainFeature {
            return _gameLocation.terrainFeatures
                .Pairs
                .Where(p => p.Value is T)
                .Select(p => p.Value as T)
                .Select(tf => new TerrainFeature(new StardewObject(tf)) as ITerrainFeature)
                .ToList();
        }

        public bool IsGreenhouse() {
            return _gameLocation.IsGreenhouse;
        }

        public IList<NPC> GetCharacters() {
            var npcs = _gameLocation.getCharacters().Where(c => c.isVillager()).Select(c => new NPC(c)).ToList();
            return npcs;
        }

        public IList<NPC> GetCharactersWithBirthdays() {
            var birthdayCharacters = _gameLocation
                .getCharacters()
                .Where(c => c.isBirthday(Game1.Date.Season, Game1.Date.DayOfMonth));

            return birthdayCharacters.Select(c => new NPC(c)).ToList();
        }

        public int GetNumberOfHarvestableObjects() {
            return GetObjects().Count(o => o.IsReadyForHarvest());
        }

        public int GetNumberOfHarvestableTerrainFeatures<T>() where T : StardewValley.TerrainFeatures.TerrainFeature {
            return GetTerrainFeatures<T>().Count(tf => tf.IsReadyForHarvest());
        }

    }
}
