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
        /// <summary>A group of settings that defines when an area's spawns will appear.</summary>
        private class SpawnTiming
        {
            public StardewTime StartTime { get; set; } = 600; //the stardew time value when spawning should start (inclusive)
            public StardewTime EndTime { get; set; } = 600; //the stardew time value when spawning should end (inclusive)

            private int? minimumTimeBetweenSpawns = 10; //the minimum amount of in-game time (in 10-minute increments) between each "set" of spawns (note: this may override min/max spawns per day from SpawnArea)
            public int? MinimumTimeBetweenSpawns
            {
                get
                {
                    return minimumTimeBetweenSpawns;
                }
                set
                {
                    minimumTimeBetweenSpawns = value;

                    if (minimumTimeBetweenSpawns == null || minimumTimeBetweenSpawns < 10) //if this is null or less than 10
                    {
                        minimumTimeBetweenSpawns = 10; //set it to 10
                    }
                    else if (minimumTimeBetweenSpawns % 10 > 0) //if this isn't a multiple of 10
                    {
                        minimumTimeBetweenSpawns = (minimumTimeBetweenSpawns - (minimumTimeBetweenSpawns % 10)) + 10; //round up to a multiple of 10
                    }
                }
            }

            public int? MaximumSimultaneousSpawns { get; set; } = null; //the maximum number of things this area will spawn at the same time (note: this may effectively limit an area's min/max spawns per day)

            public bool OnlySpawnIfAPlayerIsPresent { get; set; } = false; //if true, spawns will be skipped if no players are present at the target map

            public string SpawnSound { get; set; } = ""; //the name (if any) of the sound to be played when this area spawns objects

            public SpawnTiming()
            {

            }
        }
    }
}