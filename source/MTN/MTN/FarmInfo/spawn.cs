using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo {
    public enum spawningSeason {
        allYear = 0,
        springOnly = 1,
        summerOnly,
        fallOnly,
        winterOnly,
        notWinter,
        notFall,
        notSummer,
        notSpring,
        firstHalf,
        secondHalf,
        springFall,
        summerWinter,
        springWinter,
        summerFall
    }

    public enum spawnType {
        noSpawn,
        pathTileBound,
        areaBound
    }

    public class Area {
        public int x;
        public int y;
        public int width;
        public int height;
        public Area() { }
        public Area(int X, int Y, int Width, int Height) {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }
    }

    public abstract class spawn<T> {
        //English setting names
        public string itemName;
        public string seasons;
        public string spawntype;

        //Systematic setting values
        public int itemId = 0;
        public spawningSeason SeasonsToSpawn = spawningSeason.allYear;
        public spawnType SpawnType = spawnType.areaBound;

        //Map
        public string mapName;

        //Area Binding
        public Area area = null;

        //Tile Binding
        public int TileIndex = 0;

        //Chance Probability
        public float chance = 0.70f;

        //Additive Probability
        public float rainAddition = 0;
        public float newMonthAddition = 0;
        public float newYearAddition = 0;

        //Amount
        public int minimumAmount = 1;
        public int maximumAmount = 0;

        //Amount Multiplers
        public int rainMultipler = 1;
        public int newMonthMultipler = 1;
        public int newYearMultipler = 1;

        //Cooldown
        public int minCooldown = 1;
        public int maxCooldown = 0;
        public int daysTilNextSpawn = 1;

        //Tile Requirements
        public string tileType = "All";
        public bool diggable = true;

        //Validation
        protected bool valid = false;
        public bool isValid {
            get {
                return valid;
            }
        }

        //Season converter
        public string validSeasons {
            get {
                switch (SeasonsToSpawn) {
                    case spawningSeason.allYear:
                        return "all";
                    case spawningSeason.springOnly:
                        return "spring";
                    case spawningSeason.summerOnly:
                        return "summer";
                    case spawningSeason.fallOnly:
                        return "fall";
                    case spawningSeason.winterOnly:
                        return "winter";
                    case spawningSeason.notWinter:
                        return "spring_summer_fall";
                    case spawningSeason.notFall:
                        return "spring_summer_winter";
                    case spawningSeason.notSummer:
                        return "spring_fall_winter";
                    case spawningSeason.notSpring:
                        return "summer_fall_winter";
                    case spawningSeason.firstHalf:
                        return "spring_summer";
                    case spawningSeason.secondHalf:
                        return "fall_winter";
                    case spawningSeason.springFall:
                        return "spring_fall";
                    case spawningSeason.summerWinter:
                        return "summer_winter";
                    case spawningSeason.springWinter:
                        return "spring_winter";
                    case spawningSeason.summerFall:
                        return "summer_fall";
                    default:
                        return "none";
                }
            }
        }

        //Blueprint method
        public abstract void loadItem();
        public abstract T getItem(Vector2 tile);
        public abstract bool canSpawnAtTile(GameLocation location, Vector2 tile);
        public abstract void executeSpawn(int attempts);
        protected abstract void checkIntegrity();

        public virtual int generateAmount() {
            if (maximumAmount == 0) {
                return minimumAmount;
            } else {
                return Game1.random.Next(minimumAmount - 1, maximumAmount);
            }
        }

        public virtual bool tickAndCheckCooldown() {
            daysTilNextSpawn--;
            if (daysTilNextSpawn == 0) {
                setCooldown();
                return true;
            }
            return false;
        }

        protected virtual void setCooldown() {
            if (maxCooldown == 0) {
                daysTilNextSpawn = minCooldown;
            } else {
                daysTilNextSpawn = Game1.random.Next(minCooldown - 1, maxCooldown);
            }
        }

        protected virtual Vector2 generateTile(GameLocation location) {
            Vector2 results;
            if (area != null) {
                results = new Vector2(Game1.random.Next(area.x - 1, area.width), Game1.random.Next(area.y, area.height));
            } else {
                results = new Vector2(Game1.random.Next(-1, location.map.Layers[0].LayerWidth), Game1.random.Next(-1, location.map.Layers[0].LayerHeight));
            }
            return results;
        }
    }
}
