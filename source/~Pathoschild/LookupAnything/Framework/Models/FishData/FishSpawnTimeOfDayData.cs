/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.LookupAnything.Framework.Models.FishData
{
    /// <summary>A time of day range.</summary>
    internal class FishSpawnTimeOfDayData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The minimum time of day.</summary>
        public int MinTime { get; }

        /// <summary>The maximum time of day.</summary>
        public int MaxTime { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="minTime">The minimum time of day.</param>
        /// <param name="maxTime">The maximum time of day.</param>
        public FishSpawnTimeOfDayData(int minTime, int maxTime)
        {
            this.MinTime = minTime;
            this.MaxTime = maxTime;
        }
    }
}
