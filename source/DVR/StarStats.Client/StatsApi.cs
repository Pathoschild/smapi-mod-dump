/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using StarStats.Common;

namespace StarStats.Client
{
    public class StatsApi
    {
        private Database db;

        public StatsApi(Database d)
        {
            db = d;
        }

        // Record a data point
        public void Record(SDate now, int timeofDay, double value, string metric, string tags="" )
        {
            var ts = ModEntry.TimeStamp(now, timeofDay);
            db.Add(ts, value, metric, tags);
        }

        // Get latest point from a time series 
        public double GetLatest(string metric)
        {
            db.Metric(metric);
            return 0;
        }
    }
}
