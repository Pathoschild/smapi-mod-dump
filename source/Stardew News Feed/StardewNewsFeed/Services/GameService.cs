using StardewModdingAPI;
using StardewNewsFeed.Wrappers;
using StardewNewsFeed.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewNewsFeed.Services {
    public class GameService : IGameService {

        private readonly IGame _game;
        private readonly ILocationService _locationService;
        private ITranslationHelper _translationHelper;
        private IMonitor _monitor;

        public GameService(ITranslationHelper translationHelper, IMonitor monitor) {
            _game = new Game();
            _locationService = new LocationService(translationHelper);
            _translationHelper = translationHelper;
            _monitor = monitor;
        }

        #region IGameService Implementation
        public void CheckFarmCave() {
            var farmCave = _locationService.GetLocationByName(Constants.FARM_CAVE_LOCATION_NAME);

            if (PlayerChoseMushrooms()) {
                CheckLocationForHarvestableObjects(farmCave);
            } else {
                CheckLocationForHarvestableTerrain<HoeDirt>(farmCave);
            }
        }

        public void CheckGreenhouse() {
            var greenhouse = _locationService.GetGreenhouse();
            CheckLocationForHarvestableTerrain<HoeDirt>(greenhouse);
        }

        public void CheckCellar() {
            var cellar = _locationService.GetLocationByName(Constants.CELLAR_LOCATION_NAME);
            CheckLocationForHarvestableObjects(cellar);
        }

        public void CheckShed() {
            var farm = GetFarm();
            var buildings = farm.GetBuildings<Shed>(_translationHelper);
            foreach (var building in buildings) {
                CheckLocationForHarvestableObjects(building);
            }
        }

        public void CheckFarmBuildings<T>() where T : StardewValley.Buildings.Building {
            var farm = GetFarm();
            var buildings = farm.GetBuildings<T>(_translationHelper);
            _monitor.Log($"Found {buildings.Count} {typeof(T)}(s)");
            foreach(var building in buildings) {
                CheckLocationForHarvestableObjects(building);
            }
        }

        public void CheckLocationForBirthdays(ILocation location) {
            foreach (var npc in location.GetCharacters()) {
                if (npc.IsMyBirthday(new GameDate(Game1.Date.Season, Game1.Date.DayOfMonth))) {
                    var message = _translationHelper.Get("news-feed.birthday-notice", new { npcName = npc.GetName() });
                    DisplayMessage(new HudMessage(message, HudMessageType.NewQuest));
                }
            }
        }
        #endregion

        #region Private Methods
        private void DisplayMessage(IHudMessage hudMessage) {
            _game.DisplayMessage(hudMessage);
        }

        private bool PlayerChoseMushrooms() {
            return _game.GetFarmCaveChoice() == FarmCaveChoice.Mushrooms;
        }

        private IFarm GetFarm() {
            return _game.GetFarm();
        }

        private void CheckLocationForHarvestableObjects(ILocation location) {
            var numberOfItemsReadyForHarvest = location.GetNumberOfHarvestableObjects();
            if (numberOfItemsReadyForHarvest > 0) {
                var message = _translationHelper.Get("news-feed.harvest-items-found-in-location-notice", new {
                    numberOfItems = numberOfItemsReadyForHarvest,
                    locationName = location.GetDisplayName(),
                });
                DisplayMessage(new HudMessage(message, HudMessageType.NewQuest));
            }
        }

        private void CheckLocationForHarvestableTerrain<T>(ILocation location) where T : StardewValley.TerrainFeatures.TerrainFeature {
            var numberOfItemsReadyForHarvest = location.GetNumberOfHarvestableTerrainFeatures<T>();
            if (numberOfItemsReadyForHarvest > 0) {
                var message = _translationHelper.Get("news-feed.harvest-items-found-in-location-notice", new {
                    numberOfItems = numberOfItemsReadyForHarvest,
                    locationName = location.GetDisplayName(),
                });
                DisplayMessage(new HudMessage(message, HudMessageType.NewQuest));
            }
        }
        #endregion
    }
}
