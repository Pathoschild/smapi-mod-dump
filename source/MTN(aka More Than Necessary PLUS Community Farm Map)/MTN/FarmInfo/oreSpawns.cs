using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace MTN.FarmInfo {
    public class oreSpawns : spawn<SObject> {

        public bool enableGemNodes = false;
        public bool enableArtifacts = false;
        private int current;

        Dictionary<string, oreNode> itemList = new Dictionary<string, oreNode>() {
            {"Rock1", new oreNode() {itemName = "Rock1", itemId = 668, levelRequirement = 0, chance = 0.5f, minutesUntilReady = 2} },
            {"Rock2", new oreNode() {itemName = "Rock2", itemId = 670, levelRequirement = 0, chance = 0.5f, minutesUntilReady = 2} },
            {"Copper", new oreNode() {itemName = "Copper", itemId = 751, levelRequirement = 0, chance = 0.21f, minutesUntilReady = 3} },
            {"Geode", new oreNode() {itemName = "Geode", itemId = 75, levelRequirement = 0, chance = 0.1f, minutesUntilReady = 3} },
            {"Steel", new oreNode() {itemName = "Steel", itemId = 290, levelRequirement = 4, chance = 0.15f, minutesUntilReady = 4} },
            {"FrozenGeode", new oreNode() {itemName = "FrozenGeode", itemId = 76, levelRequirement = 5, chance = 0.05f, minutesUntilReady = 5} },
            {"Gold", new oreNode() {itemName = "Gold", itemId = 764, levelRequirement = 7, chance = 0.1f, minutesUntilReady = 8} },
            {"MagmaGeode", new oreNode() {itemName = "MagmaGeode", itemId = 77, levelRequirement = 8, chance = 0.033f, minutesUntilReady = 7} },
            {"Iridium", new oreNode() {itemName = "Iridium", itemId = 765, levelRequirement = 10, chance = 0.01f, minutesUntilReady = 16} },
        };

        Dictionary<string, oreNode> gemList = new Dictionary<string, oreNode>() {
            
        };

        public override bool canSpawnAtTile(GameLocation location, Vector2 tile) {
            switch (SpawnType) {
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

                randomAmt = generateAmount();

                for (int a = 0; a < randomAmt; a++) {

                    for (int b = 0; b < itemList.Count; b++) {

                    }
                    if ((float)Game1.random.NextDouble() < chance) {



                        for (int i = 0; i < attempts; i++) {
                            point = generateTile(location);
                            if (areaBoundLogic(location, point)) {
                                location.objects.Add(point, getItem(point));
                            }
                        }
                    }
                }


            }
            return;
        }

        public override SObject getItem(Vector2 tile) {
            if (itemId == 0) return null;
            return new SObject(tile, current, 10);
        }

        public override void loadItem() {
            throw new NotImplementedException();
        }

        protected override void checkIntegrity() {
            throw new NotImplementedException();
        }

        protected bool areaBoundLogic(GameLocation location, Vector2 tile) {
            int x = (int)tile.X;
            int y = (int)tile.Y;

            if (location.doesTileHaveProperty(x, y, "NoSpawn", "Back") != null) {
                return false;
            }

            if (location.doesTileHaveProperty(x, y, "Diggable", "Back") == null) {
                return false;
            }
            
            if (!location.doesTileHaveProperty(x, y, "Type", "Back").Equals("Dirt")) {
                return false;
            }

            if (!location.isTileLocationTotallyClearAndPlaceable(tile)) {
                return false;
            }

            return true;
        }
    }

    public class oreNode {
        public string itemName;
        public int itemId;

        public int levelRequirement;
        public float chance;

        public int minutesUntilReady;
    }
}
