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
            //class added in version 1.3; defaults used here to automatically fill in values with SMAPI's json interface
            public string[] Years { get; set; } = new string[0];
            public string[] Seasons { get; set; } = new string[0];
            public string[] Days { get; set; } = new string[0];
            public string[] WeatherYesterday { get; set; } = new string[0];
            public string[] WeatherToday { get; set; } = new string[0];
            public string[] WeatherTomorrow { get; set; } = new string[0];
            public int? LimitedNumberOfSpawns { get; set; } = null;

            public ExtraConditions()
            {

            }

            public ExtraConditions(string[] years, string[] seasons, string[] days, string[] wyesterday, string[] wtoday, string[] wtomorrow, int? spawns)
            {
                Years = years; //a list of years on which objects are allowed to spawn
                Seasons = seasons; //a list of seasons in which objects are allowed to spawn
                Days = days; //a list of days (individual or ranges) on which objects are allowed to spawn
                WeatherYesterday = wyesterday; //if yesterday's weather is listed here, objects are allowed to spawn
                WeatherToday = wtoday; //if yesterday's weather is listed here, objects are allowed to spawn
                WeatherTomorrow = wtomorrow; //if yesterday's weather is listed here, objects are allowed to spawn
                LimitedNumberOfSpawns = spawns; //a number that will count down each day until 0, preventing any further spawns once that is reached
            }
        }
    }
}