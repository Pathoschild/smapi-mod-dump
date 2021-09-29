/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/


using System;
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
                Console.Write(String.Format("First farm {0}", StardewValley.Game1.getFarm().name));
                // Look for custom farms as well
                foreach (StardewValley.GameLocation location in StardewValley.Game1.locations) {
                    if (IsLocationAFarm(location))
                    {
                        Console.Write(String.Format("Got farm {0}", location.name));
                        farms.Add(new Farm((StardewValley.Farm)location));
                    }
                }
                return farms;
            }
        }
        
        private bool IsLocationAFarm(StardewValley.GameLocation location)
        {
            return location.GetType().IsSubclassOf(typeof(StardewValley.Farm));
        }

        public bool IsPlayerAtTheFarm()
        {
            return IsLocationAFarm(StardewValley.Game1.currentLocation);
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
