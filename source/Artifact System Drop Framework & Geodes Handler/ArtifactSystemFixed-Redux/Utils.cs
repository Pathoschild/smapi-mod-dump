/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;

using StardewValley;

namespace ArtifactSystemFixed_Redux
    {
    internal static class Log
        {
        internal static void Error(string msg) => ASFR_Mod.ModMonitor.Log(msg, LogLevel.Error);
        internal static void Warn(string msg) => ASFR_Mod.ModMonitor.Log(msg, LogLevel.Warn);
        internal static void Info(string msg) => ASFR_Mod.ModMonitor.Log(msg, LogLevel.Info);
        internal static void Debug(string msg) => ASFR_Mod.ModMonitor.Log(msg, LogLevel.Debug);
        internal static void Trace(string msg) => ASFR_Mod.ModMonitor.Log(msg, LogLevel.Trace);

        }

    public static class RandomChooseExtensions
        {
        public static T Choose<T>(this Random rng, IList<T> collection) {
            return collection[rng.Next(collection.Count)];
            }

        public static KeyValuePair<TKey, TValue> Choose<TKey, TValue>(this Random rng, IDictionary<TKey, TValue> dict) {
            return rng.Choose(dict.ToList());
            }

        public static TKey ChooseWeighted<TKey>(this Random rng, ICollection<KeyValuePair<TKey, double>> weightedCollection, TKey fallback = default) {
            double sumWeights = weightedCollection.Sum(kvp => kvp.Value);
            double selector = rng.NextDouble() * sumWeights;  // map [0, 1) to [0, sumWeights)
            double accumulator = 0.0;
            foreach (var kvp in weightedCollection) {
                accumulator += kvp.Value;
                if (accumulator > selector) return kvp.Key;
                // this is guaranteed to trigger because selector < sumWeights, and at last element accumulator == sumWeights
                // EXCEPT for the diabolical situation where _all_ weights are 0.0
                }
            // _all_ weights are 0.0, so sumWeights is 0.0, so selector is 0.0, so (accumulator > selector) is false everywhere
            return fallback;
            }

        public static TKey ChooseOdds<TKey>(
            this Random rng, ICollection<KeyValuePair<TKey, double>> oddsCollection, double scale = 1.0, TKey fallback = default, bool checkOverflow = false
            ) {
            if (checkOverflow) {
                double sumOdds = oddsCollection.Sum(kvp => kvp.Value);
                if (sumOdds > 1.0) {
                    throw new OverflowException("Sum of odds > 1.0");
                    }
                }
            double selector = rng.NextDouble() * scale;
            double accumulator = 0.0;
            foreach (var kvp in oddsCollection) {
                accumulator += kvp.Value;
                if (accumulator > selector) return kvp.Key;
                }
            // since selector is not scaled, and sum of odds might be < 1.0, then there's a possibility that
            // at last element, accumulator is still <= selector
            return fallback;
            }
        }

    public static class FarmerExtension
        {
        public static int GetArchaeologyFound(this Farmer who, int itemID) {
            // Game code indicates that only [0] is actually used anywhere
            if (who.archaeologyFound.TryGetValue(itemID, out int[] count)) return count[0];
            return 0;
            }

        public static int GetMineralFound(this Farmer who, int itemID) {
            if (who.mineralsFound.TryGetValue(itemID, out int count)) return count;
            return 0;
            }
        }

    public static class IEnumerableExtensions
        {
        /// <summary>
        /// Enumerates over pairs of elements from a sequence, and transform each pair into a <see cref="KeyValuePair{TKey, TValue}"/>
        /// </summary>
        /// <param name="source">The source IEnumerable of <typeparamref name="TElem"/></param>
        /// <param name="transform">A function that accepts a pair of params of type <typeparamref name="TElem"/>,
        /// and returns a <see cref="KeyValuePair{TKey, TValue}"/></param>
        /// <typeparam name="TElem">Type of elements in the sequence</typeparam>
        /// <typeparam name="TKey">Type of Key in the KeyValuePair</typeparam>
        /// <typeparam name="TValue">Type of Value in the KeyValuePair</typeparam>
        /// <remarks>Adapted from <see href="https://stackoverflow.com/a/67605407/149900"/></remarks>
        public static IEnumerable<KeyValuePair<TKey, TValue>> PairwiseKVP<TElem, TKey, TValue>(
                this IEnumerable<TElem> source,
                Func<TElem, TElem, KeyValuePair<TKey, TValue>> transform
            ) {
            using var it = source.GetEnumerator();
            while (it.MoveNext()) {
                var first = it.Current;
                if (!it.MoveNext())
                    yield break;
                yield return transform(first, it.Current);
                }
            }

        /// <summary>
        /// Enumerates over pairs of elements from a sequence, and transform each pair into a <see cref="KeyValuePair{TElem, TElem}"/>
        /// where TKey and TValue are both of type <typeparamref name="TElem"/>
        /// </summary>
        /// <param name="source">The source IEnumerable of <typeparamref name="TElem"/></param>
        /// <param name="transform">A function that accepts a pair of params of type <typeparamref name="TElem"/>,
        /// and returns a <see cref="KeyValuePair{TKey, TValue}"/></param>
        /// <typeparam name="TElem">Type of elements in the sequence</typeparam>
        public static IEnumerable<KeyValuePair<TElem, TElem>> PairwiseKVP<TElem>(this IEnumerable<TElem> source) {
            return source.PairwiseKVP((a, b) => new KeyValuePair<TElem, TElem>(a, b));
            }
        }

    }
