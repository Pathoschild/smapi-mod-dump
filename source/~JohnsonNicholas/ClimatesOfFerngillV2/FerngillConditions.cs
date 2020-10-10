/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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
