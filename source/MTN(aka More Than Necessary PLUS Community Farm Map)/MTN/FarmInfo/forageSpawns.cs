using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Tiles;
using SObject = StardewValley.Object;

namespace MTN.FarmInfo {
    public class forageSpawns : spawn<SObject> {

        Dictionary<string, int> itemList = new Dictionary<string, int>() {
            {"Wild Horseradish", 16 },
            {"WildHorseradish", 16 },
            {"Horseradish", 16 },
            {"Daffodil", 18 },
            {"Leek", 20 },
            {"Dandelion", 22 },
            {"Coconut", 88 },
            {"Cactus Fruit", 90 },
            {"CactusFruit", 90 },
            {"Morel", 257 },
            {"Fiddlehead Fern", 259 },
            {"FiddleheadFern", 259 },
            {"Fiddlehead", 259 },
            {"Chanterelle", 281 },
            {"Holly", 283 },
            {"Spice Berry", 396 },
            {"SpiceBerry", 396 },
            {"Grape", 398 },
            {"Spring Onion", 399 },
            {"SpringOnion", 399 },
            {"Sweet Pea", 402 },
            {"SweetPea", 402 },
            {"Common Mushroom", 404 },
            {"CommonMushroom", 404 },
            {"Wild Plum", 406 },
            {"WildPlum", 406 },
            {"Hazelnut", 408 },
            {"Blackberry", 410 },
            {"Winter Root", 412 },
            {"WinterRoot", 412 },
            {"Crystal Fruit", 414 },
            {"CrystalFruit", 414 },
            {"Snow Yam", 416 },
            {"SnowYam", 416 },
            {"Crocus", 418 },
            {"Red Mushroom", 420 },
            {"RedMushroom", 420 },
            {"Purple Mushroom", 422 },
            {"PurpleMushroom", 422 }
        };

        public override bool canSpawnAtTile(GameLocation location, Vector2 tile) {
            switch (SpawnType) {
                case spawnType.noSpawn:
                    return false;
                case spawnType.pathTileBound:
                    return tileBoundLogic(location, tile);
                case spawnType.areaBound:
                    return areaBoundLogic(location, tile);
                default:
                    return false;
            }
        }

        public override void executeSpawn(int attempts) {
            GameLocation location = Game1.getLocationFromName(mapName);
            Vector2 point;
            int randomAmt;

            if (SeasonsToSpawn == spawningSeason.allYear || validSeasons.Contains(Game1.currentSeason)) {
                if (!tickAndCheckCooldown()) {
                    return;
                }

                if (SpawnType == spawnType.areaBound) {
                    randomAmt = generateAmount();
                    for (int a = 0; a < randomAmt; a++) {
                        if ((float)Game1.random.NextDouble() < chance) {
                            for (int i = 0; i < attempts; i++) {
                                point = generateTile(location);
                                if (areaBoundLogic(location, point)) {
                                    location.dropObject(getItem(point), point * 64f, Game1.viewport, true, null);
                                }
                            }
                        }
                    }
                } else if (SpawnType == spawnType.pathTileBound) {
                    point = new Vector2();
                    for (point.X = 0; point.X < location.map.Layers[0].LayerWidth; point.X++) {
                        for (point.Y = 0; point.Y < location.map.Layers[0].LayerHeight; point.Y++) {
                            if (tileBoundLogic(location, point)) {
                                location.dropObject(getItem(point), point * 64f, Game1.viewport, true, null);
                            }
                        }
                    }
                }
            }
            return;
        }

        public override SObject getItem(Vector2 tile) {
            if (itemId == 0) return null;
            return new SObject(tile, itemId, null, false, true, false, true);
        }

        public override void loadItem() {
            checkIntegrity();
            if (itemId == 0) {
                //Error msg
                return;
            }
            valid = true;
        }

        protected override void checkIntegrity() {
            int results;

            if (itemId > 0) return;
            if (itemId < 0) {
                itemId = 0;
                return;
            }
            if (itemList.TryGetValue(itemName, out results)) {
                itemId = results;
            } else {
                itemId = 0;
                return;
            }
            if (minimumAmount < 1) minimumAmount = 1;
            if (maximumAmount < 0) maximumAmount = 0;
            if (minCooldown < 1) minCooldown = 1;
            if (maxCooldown < 0) maxCooldown = 0;
            return;
        }

        protected bool tileBoundLogic(GameLocation location, Vector2 tile) {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            Tile target = location.map.GetLayer("Paths").Tiles[x, y];
            if (target != null && target.TileIndex == TileIndex) {
                if (!location.isTileLocationTotallyClearAndPlaceable(tile)) {
                    return false;
                }
                return true;
            }
            return false;
        }

        protected bool areaBoundLogic(GameLocation location, Vector2 tile) {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string propertyType;

            if (location.doesTileHaveProperty(x, y, "NoSpawn", "Back") != null) {
                return false;
            }

            if (diggable) {
                if (location.doesTileHaveProperty(x, y, "Diggable", "Back") == null) {
                    return false;
                }
            }

            propertyType = location.doesTileHaveProperty(x, y, "Type", "Back");
            if (tileType != "All") {
                if (propertyType != tileType) {
                    return false;
                }
            } else {
                if (propertyType != "Grass" && propertyType != "Dirt") {
                    return false;
                }
            }

            if (!location.isTileLocationTotallyClearAndPlaceable(tile)) {
                return false;
            }
            return true;
        }
    }
}
