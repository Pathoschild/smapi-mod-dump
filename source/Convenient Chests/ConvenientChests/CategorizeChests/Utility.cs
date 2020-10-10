/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;

namespace ConvenientChests.CategorizeChests
{
    static class Utility
    {
        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        private static IEnumerable<T> YieldBatchElements<T>(
            IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }

        public static IDictionary<Key, IEnumerable<Value>> KeyBy<Key, Value>(this IEnumerable<Value> values,
            Func<Value, Key> makeKey)
        {
            var dict = new Dictionary<Key, IEnumerable<Value>>();

            foreach (var value in values)
            {
                var key = makeKey(value);

                if (!dict.ContainsKey(key))
                    dict[key] = new List<Value>();

                ((List<Value>) dict[key]).Add(value);
            }

            return dict;
        }
        
        /// <summary>Get all game locations.</summary>
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
    }
}