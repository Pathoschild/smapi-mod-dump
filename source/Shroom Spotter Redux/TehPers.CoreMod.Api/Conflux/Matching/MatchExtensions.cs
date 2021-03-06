/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Conflux.Matching {
    public static class MatchExtensions {

        /// <summary>Maps this value to another value through matching operators.</summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source object being matched.</param>
        /// <returns>An operator to perform matches on the source to generate a final result.</returns>
        public static MatchOperator<TSource, TResult> Match<TSource, TResult>(this TSource source) {
            return new MatchOperator<TSource, TResult>(source);
        }

        /// <summary>Maps each value in this <see cref="IEnumerable{T}"/> to another value through matching operators.</summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source objects being matched.</param>
        /// <returns>An operator to perform matches on the sources to generate final results.</returns>
        public static MatchAllOperator<TSource, TResult> MatchAll<TSource, TResult>(this IEnumerable<TSource> source) {
            return new MatchAllOperator<TSource, TResult>(source);
        }
    }
}
