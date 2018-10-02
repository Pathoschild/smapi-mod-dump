using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace MTN.FarmInfo {
    public class resourceSpawns : spawn<ResourceClump> {
        public int width = 2;
        public int height = 2;

        public override ResourceClump getItem(Vector2 tile) {
            if (itemId == 0) return null;
            return new ResourceClump(itemId, width, height, tile);
        }

        public override void loadItem() {
            checkIntegrity();
            if (itemId == 0) {
                //Error msg
                return;
            }
            valid = true;
        }

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
            GameLocation temp = Game1.getLocationFromName(mapName);
            Farm location;
            Vector2 point;
            int randomAmt;
            float dice;

            if (temp is Farm) {
                location = (Farm)temp;
            } else {
                return;
            }

            if (SeasonsToSpawn == spawningSeason.allYear || validSeasons.Contains(Game1.currentSeason)) {
                if (!tickAndCheckCooldown()) {
                    return;
                }

                if (SpawnType == spawnType.areaBound) {
                    randomAmt = generateAmount();
                    for (int a = 0; a < randomAmt; a++) {
                        dice = (float)Game1.random.NextDouble();
                        if (dice > chance) continue;
                        for (int i = 0; i < attempts; i++) {
                            point = generateTile(location);
                            if (areaBoundLogic(location, point)) {
                                location.resourceClumps.Add(getItem(point));
                            }
                        }
                    }
                } else if (SpawnType == spawnType.pathTileBound) {
                    point = new Vector2();
                    for (point.X = 0; point.X < location.map.Layers[0].LayerWidth; point.X++) {
                        for (point.Y = 0; point.Y < location.map.Layers[0].LayerHeight; point.Y++) {
                            if (tileBoundLogic(location, point)) {
                                location.resourceClumps.Add(getItem(point));
                            }
                        }
                    }
                }
                return;
            }
        }

        protected bool tileBoundLogic(GameLocation location, Vector2 tile) {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            Tile target = location.map.GetLayer("Paths").Tiles[x, y];

            if (target != null && target.TileIndex == TileIndex) {
                for (int i = 0; i < width; i++) {
                    for (int j = 0; j < height; j++) {
                        if (!location.isTileLocationTotallyClearAndPlaceable(x + i, y + j)) {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        protected bool areaBoundLogic(GameLocation location, Vector2 tile) {
            int x = (int)tile.X;
            int y = (int)tile.Y;
            string propertyType;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (location.doesTileHaveProperty(x + i, y + j, "NoSpawn", "Back") != null) {
                        return false;
                    }

                    if (diggable) {
                        if (location.doesTileHaveProperty(x + i, y + j, "Diggable", "Back") == null) {
                            return false;
                        }
                    }

                    propertyType = location.doesTileHaveProperty(x + i, y + j, "Type", "Back");
                    if (tileType != "All") {
                        if (propertyType != tileType) {
                            return false;
                        }
                    } else {
                        if (propertyType != "Grass" && propertyType != "Dirt") {
                            return false;
                        }
                    }

                    if (!location.isTileLocationTotallyClearAndPlaceable(new Vector2(x + i, y + j))) {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void checkIntegrity() {
            if (itemId > 0) return;
            if (itemId < 0) {
                itemId = 0;
                return;
            }
            switch (itemName) {
                case "Stump":
                    itemId = 600;
                    break;
                case "Log":
                    itemId = 602;
                    break;
                case "Boulder":
                    itemId = 672;
                    break;
            }
            if (minimumAmount < 1) minimumAmount = 1;
            if (maximumAmount < 0) maximumAmount = 0;
            if (minCooldown < 1) minCooldown = 1;
            if (maxCooldown < 0) maxCooldown = 0;
            return;
        }
    }
}
