using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using StardewValley;

namespace ProducerFrameworkMod
{
    public class GameUtils
    {
        /// <summary>Get the current weather from the game state.</summary>
        public static Weather GetCurrentWeather()
        {
            Weather weather;
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || (SaveGame.loaded?.weddingToday ?? Game1.weddingToday))
            {
                weather = Weather.Sunny;
            }
            else if (Game1.isSnowing)
            {
                weather = Weather.Snowy;
            }
            else if (Game1.isRaining)
            {
                weather = (Game1.isLightning ? Weather.Stormy : Weather.Rainy);
            }
            else if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
            {
                weather = Weather.Windy;
            }
            else
            {
                weather = Weather.Sunny;
            }
            return weather;
        }
    }
}
