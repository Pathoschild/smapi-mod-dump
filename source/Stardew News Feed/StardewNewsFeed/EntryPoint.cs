using StardewValley.Buildings;
using StardewModdingAPI;
using StardewNewsFeed.Services;
using StardewNewsFeed.Wrappers;

namespace StardewNewsFeed {
    public class EntryPoint : Mod {
    
        private ModConfig _modConfig;
        private IGameService _gameService;

        public override void Entry(IModHelper helper) {
            _modConfig = Helper.ReadConfig<ModConfig>();
            _gameService = new GameService(Helper.Translation, Monitor);

            if (_modConfig.CaveNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckFarmCave();
            }

            if (_modConfig.GreenhouseNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckGreenhouse();
            }

            if (_modConfig.CellarNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckCellar();
            }

            if (_modConfig.ShedNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckShed();
            }

            if (_modConfig.CoopCheckEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckFarmBuildings<Coop>();
            }

            if (_modConfig.BarnCheckEnabled) {
                helper.Events.GameLoop.DayStarted += (s, e) => _gameService.CheckFarmBuildings<Barn>();
            }

            if (_modConfig.BirthdayCheckEnabled) {
                helper.Events.Player.Warped += (s, e) => _gameService.CheckLocationForBirthdays(new Location(e.NewLocation, Helper.Translation));
            }
        }
    }
}
