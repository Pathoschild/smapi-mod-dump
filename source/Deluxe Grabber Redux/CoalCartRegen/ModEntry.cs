/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace CoalCartRegen
{
    class ModEntry : Mod
    {
        public static Dictionary<string, int> seasonNumbers = new Dictionary<string, int>()
        {
            { "spring", 0 },
            { "summer", 1 },
            { "fall", 2 },
            { "winter", 3 }
        };

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        public void LogDebug(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            var seasonNumber = seasonNumbers[Game1.currentSeason];
            var calendarDay = seasonNumber * 28 + Game1.dayOfMonth;
            var mineChanges = MineShaft.permanentMineChanges;
            LogDebug($"Number of mine changes: {mineChanges.Count}");
            foreach (var kvp in mineChanges)
            {
                var mineLevel = kvp.Key;
                var mineLevelChanges = kvp.Value;
                if (calendarDay % 2 == 0 && mineLevelChanges.coalCartsLeft <= 0)
                {
                    mineLevelChanges.coalCartsLeft++;
                    LogDebug($"Refreshing coal carts for mine level {mineLevel}");
                }
                if (calendarDay % 6 == 0 && mineLevelChanges.platformContainersLeft <= 0)
                {
                    mineLevelChanges.platformContainersLeft += 12;
                    LogDebug($"Refreshing platform containers for mine level {mineLevel}");
                }
            }
        }
    }
}
