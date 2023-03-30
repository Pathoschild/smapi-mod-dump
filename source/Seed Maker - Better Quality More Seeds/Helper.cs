/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vabrell/sdw-seed-maker-mod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;

namespace SM_bqms
{
    public class SM_Helper
    {
        private static int LastCacheCount;
        private static int LastCacheTick;
        public static readonly IDictionary<int, int> SeedLookupCache = new Dictionary<int, int>();
        private ModConfig Config;
        public static void UpdateSeedLookupCache()
        {
            if (Game1.ticks > LastCacheTick)
            {
                IDictionary<int, int> cache = SeedLookupCache;
                Dictionary<int, string> crops = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
                if (LastCacheCount != crops.Count)
                {
                    cache.Clear();

                    foreach (KeyValuePair<int, string> entry in crops)
                    {
                        int seedId = entry.Key;
                        int produceId = Convert.ToInt32(entry.Value.Split('/')[3]);
                        if (!cache.ContainsKey(produceId)) // use first crop found per game logic
                            cache[produceId] = seedId;
                    }
                }
                LastCacheCount = crops.Count;
                LastCacheTick = Game1.ticks;
            }
        }
        public static int generateSeedAmountBasedOnQuality(Vector2 location, int quality, Boolean enableDebug, IMonitor monitor)
        {
            int modifier;
            switch (quality)
            {
                case 0: 
                    modifier = ModEntry.Config.NormalModifier;
                break; 
                case 1: 
                    modifier = ModEntry.Config.SilverModifier;
                break; 
                case 2: 
                case 3: 
                    modifier = ModEntry.Config.GoldModifier;
                break; 
                case 4: 
                    modifier = ModEntry.Config.IridiumModifier;
                break; 
                default:
                    modifier = 0;
                break;
            }
            Random r2 = new Random(
                (int)Game1.stats.DaysPlayed
                + (int)Game1.uniqueIDForThisGame / 2
                + (int)location.X
                + (int)location.Y * 77
                + Game1.timeOfDay);

            int amount = r2.Next(1 + modifier, 4 + modifier);

            if (enableDebug) {
                monitor.Log($"\nSeedMaker at {location.ToString()}\nQuanity: {quality}\nModifier: {modifier}\nSeeds: {amount}\n", LogLevel.Debug);
            }

            return amount;
        }
    }
}