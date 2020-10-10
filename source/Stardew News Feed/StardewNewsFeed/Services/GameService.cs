/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using StardewModdingAPI;
using StardewNewsFeed.Wrappers;
using StardewNewsFeed.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

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

            switch(_game.GetFarmCaveChoice()) {
                case FarmCaveChoice.None:
                    break;
                default:
                    CheckLocationForHarvestableObjects(farmCave);
                    break;
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

        public void CheckBarnForAnimalProducts() {
            var groupsOfAnimalsPerBarn = GetFarm().BarnAnimalsWithAvailableProduce
                .GroupBy(_ => _.home.nameOfIndoorsWithoutUnique);

            foreach (var groupOfAnimalsInBarn in groupsOfAnimalsPerBarn) {
                DisplayMessage( MakeAnimalProduceHudMessage(groupOfAnimalsInBarn));    
            }
        }

        private HudMessage MakeAnimalProduceHudMessage(IGrouping<string, FarmAnimal> groupOfAnimalsInBarn) {
            var translationSuffix = groupOfAnimalsInBarn.Count() > 1 ? "plural" : "singular";
            var message = _translationHelper.Get($"news-feed.harvest-animals-found-in-location-notice.{translationSuffix}", new {
                numberOfItems = groupOfAnimalsInBarn.Count(),
                locationName = groupOfAnimalsInBarn.Key,
            });
            return new HudMessage(message, HudMessageType.NewQuest); 
        }
        
        public void CheckFarmBuildingsForHarvastableItems<T>() where T : StardewValley.Buildings.Building {
            var farm = GetFarm();
            var buildings = farm.GetBuildings<T>(_translationHelper).ToList();
            _monitor.Log($"Found {buildings.Count} {typeof(T)}(s)");
            foreach(var building in buildings) {
                CheckLocationForHarvestableObjects(building);
            }
        }

        public void CheckLocationForBirthdays(ILocation location) {
            
            foreach (var npc in location.GetCharactersWithBirthdays()) {
                var message = _translationHelper.Get("news-feed.birthday-notice", new { npcName = npc.GetName() });
                DisplayMessage(new HudMessage(message, HudMessageType.NewQuest));
            }
        }

        public void CheckSilos() {
            var farm = Game1.getFarm();
            var silos = farm.buildings.Where(b => b.buildingType == new Netcode.NetString("Silo"));
            var siloCount = silos.Count();

            if(siloCount == 0) {
                return;
            }

            var maxCapacity = siloCount * 240.0;
            var piecesOfHay = farm.piecesOfHay;
            var percentOfMax = piecesOfHay / maxCapacity;

            if(percentOfMax < 0.15) {
                var message = _translationHelper.Get("news-feed.silo-low-notice", new { piecesOfHay, maxCapacity });
                DisplayMessage(new HudMessage(message, HudMessageType.NewQuest));
            }
        }
        #endregion

        #region Private Methods
        private void DisplayMessage(IHudMessage hudMessage) {
            _game.DisplayMessage(hudMessage);
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
