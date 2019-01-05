using System.Linq;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewNewsFeed {
    public class EntryPoint : Mod {

        #region Private Properties
        private ModConfig _modConfig;
        #endregion

        #region StardewModdingApi.Mod Overrides
        public override void Entry(IModHelper helper) {
            _modConfig = Helper.ReadConfig<ModConfig>();

            if(_modConfig.CaveNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += CheckFarmCave;
            }

            if(_modConfig.GreenhouseNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += CheckGreenhouse;
            }

            if(_modConfig.CellarNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += CheckCellar;
            }

            if(_modConfig.ShedNotificationsEnabled) {
                helper.Events.GameLoop.DayStarted += CheckSheds;
            }

            if (_modConfig.BirthdayCheckEnabled) {
                helper.Events.Player.Warped += CheckBirthdays;
            }
        }
        #endregion

        #region Private Methods
        private void Log(string message) {
            if(_modConfig.DebugMode) {
                Monitor.Log(message);
            }
        }

        private void CheckFarmCave(object sender, DayStartedEventArgs e) {
            var farmCave = Game1.getLocationFromName(Constants.FARM_CAVE_LOCATION_NAME);
            Log($"Player Cave Choice: {Game1.player.caveChoice}");
            if(Game1.player.caveChoice == new NetInt(2)) {
                CheckLocationForHarvestableObjects(farmCave);
            } else {
                ScanLocationForFruit(farmCave);
            }

        }

        private void CheckGreenhouse(object sender, DayStartedEventArgs e) {
            var greenhouse = Game1.locations.SingleOrDefault(l => l.isGreenhouse == new NetBool(true));
            CheckLocationForHarvestableTerrain(greenhouse);
        }

        private void CheckCellar(object sender, DayStartedEventArgs e) {
            var cellar = Game1.getLocationFromName(Constants.CELLAR_LOCATION_NAME);
            CheckLocationForHarvestableObjects(cellar);
        }

        private void CheckSheds(object sender, DayStartedEventArgs e) {
            var sheds = Game1.getFarm()
                .buildings
                .Select(b => b.indoors.Value)
                .Where(i => i is Shed);

            foreach(var shed in sheds) {
                CheckLocationForHarvestableObjects(shed);
            }
        }

        private void CheckBirthdays(object sender, WarpedEventArgs e) {
            Log("Checking for birthdays: ");
            foreach(var npc in e.NewLocation.getCharacters()) {
                Log($"Checking {npc.displayName} for a birthday today");
                if (npc.isBirthday(Game1.Date.Season, Game1.Date.DayOfMonth)) {
                    var message = Helper.Translation.Get("news-feed.birthday-notice", new { npcName = npc.getName() });
                    Game1.addHUDMessage(new HUDMessage(message, 2));
                }
            }
        }

        private void CheckLocationForHarvestableObjects(GameLocation location) {
            var numberOfItemsReadyForHarvest = location.Objects.Values.Count(o => o.readyForHarvest == new NetBool(true));

            if (numberOfItemsReadyForHarvest > 0) {
                var message = Helper.Translation.Get("news-feed.harvest-items-found-in-location-notice", new {
                    numberOfItems = numberOfItemsReadyForHarvest,
                    locationName = location.getDisplayName(Helper.Translation),
                });
                Game1.addHUDMessage(new HUDMessage(message, 2));
                Log($"{numberOfItemsReadyForHarvest} items found in the {location.getDisplayName(Helper.Translation)}");
            } else {
                Log($"No items found in the {location.getDisplayName(Helper.Translation)}");
            }
        }

        private void CheckLocationForHarvestableTerrain(GameLocation location) {
            var numberOfDirtTilesReadyForHarvest = location.terrainFeatures.Pairs
                .Where(p => p.Value is HoeDirt)
                .Select(p => p.Value as HoeDirt)
                .Count(hd => hd.readyForHarvest());

            if(numberOfDirtTilesReadyForHarvest > 0) {
                var message = Helper.Translation.Get("news-feed.harvest-items-found-in-location-notice", new {
                    numberOfItems = numberOfDirtTilesReadyForHarvest,
                    locationName = location.getDisplayName(Helper.Translation),
                });
                Game1.addHUDMessage(new HUDMessage(message, 2));
            } else {
                Log($"No items found in the {location.getDisplayName(Helper.Translation)}");
            }
        }

        private void ScanLocationForFruit(GameLocation location) {
            for (int height = 4; height < 9; height++) {
                for (int width = 2; width < 11; width++) {
                    if (TileIsHarvestable(location, height, width)) {
                        var message = Helper.Translation.Get("news-feed.bats-dropped-fruit-notice");
                        Game1.addHUDMessage(new HUDMessage(message, 2));
                        return;
                    }
                }
            }
        }

        private bool TileIsHarvestable(GameLocation location, int height, int width) {
            var tile = location.getObjectAtTile(height, width);
            if (tile == null) {
                return false;
            }

            return tile.readyForHarvest == new NetBool(true);
        }
        #endregion

    }
}
