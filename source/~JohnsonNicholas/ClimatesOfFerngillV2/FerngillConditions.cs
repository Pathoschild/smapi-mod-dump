using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley;

namespace TwilightShards.ClimatesOfFerngillV2
{
    internal class FerngillConditions
    {
        private Dictionary<GameLocation, FerngillWeather> savedLocalWeathers;

        /// <summary> Default constructor </summary>
        internal FerngillConditions()
        {
            savedLocalWeathers = new Dictionary<GameLocation, FerngillWeather>();
        }


        /// <summary> This function handles the game returning to the title </summary>
        internal void ReturnToTitle()
        {
            savedLocalWeathers.Clear();
        }
    }
}
