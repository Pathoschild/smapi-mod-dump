/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of additional requirements needed to spawn objects in a given area.</summary>
        private class ExtraConditions
        {
            public string[] Years { get; set; } = new string[0];
            public string[] Seasons { get; set; } = new string[0];
            public string[] Days { get; set; } = new string[0];
            public string[] WeatherYesterday { get; set; } = new string[0];
            public string[] WeatherToday { get; set; } = new string[0];
            public string[] WeatherTomorrow { get; set; } = new string[0];
            public string[] EPUPreconditions { get; set; } = new string[0];
            public int? LimitedNumberOfSpawns { get; set; } = null;

            public ExtraConditions()
            {

            }
        }
    }
}