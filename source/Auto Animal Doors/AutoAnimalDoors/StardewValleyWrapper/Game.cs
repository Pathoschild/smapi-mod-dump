
using System.Collections.Generic;

namespace AutoAnimalDoors.StardewValleyWrapper
{
    public enum Weather { SNOWING, RAINING, LIGHTNING, SUNNY, WINDY };
    
    public enum Season { SPRING, SUMMER, FALL, WINTER };

    class Game
    {
        private static Game instance;

        private Game()
        {

        }

        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game();
                }

                return instance;
            }
        }

        public List<Farm> Farms
        {
            get
            {
                List<Farm> farms = new List<Farm>();
                farms.Add(new Farm(StardewValley.Game1.getFarm()));

                // Look for custom farms as well
                foreach (StardewValley.GameLocation location in StardewValley.Game1.locations) {
                    if (location.GetType().IsSubclassOf(typeof(StardewValley.Farm)))
                    {
                        farms.Add(new Farm((StardewValley.Farm)location));
                    }
                }
                return farms;
            }

        }

        public bool IsLoaded()
        {
            return StardewValley.Game1.hasLoadedGame;
        }

        public Weather Weather
        {
            get
            {
                if (StardewValley.Game1.isSnowing)
                {
                    return Weather.SNOWING;
                }
                else if (StardewValley.Game1.isLightning)
                {
                    return Weather.LIGHTNING;
                }
                else if (StardewValley.Game1.isRaining)
                {
                    return Weather.RAINING;
                }
                else if (StardewValley.Game1.isDebrisWeather)
                {
                    return Weather.WINDY;
                }

                return Weather.SUNNY;
            }
        }

        public Season Season
        {
            get
            {
                switch (StardewValley.Game1.currentSeason)
                {
                    case "summer":
                        return Season.SUMMER;
                    case "fall":
                        return Season.FALL;
                    case "winter":
                        return Season.WINTER;
                    case "spring":
                        return Season.SPRING;
                    default:
                        return Season.SPRING;
                }
            }
        }
    }
}
