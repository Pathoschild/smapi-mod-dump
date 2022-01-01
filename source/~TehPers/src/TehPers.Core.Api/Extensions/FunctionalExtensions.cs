/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extension methods that allow for more functional programming.
    /// </summary>
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Projects the value to a new value if it is not null. This is similar to
        /// <see cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>.
        /// </summary>
        /// <typeparam name="TSource">The source value type.</typeparam>
        /// <typeparam name="TResult">The mapped type.</typeparam>
        /// <param name="source">The source value.</param>
        /// <param name="f">A function which maps the source to the result.</param>
        /// <returns>
        /// The result of mapping the value using <paramref name="f"/>. If
        /// <paramref name="source"/> is <see langword="null"/>, then the result is also
        /// <see langword="null"/>.
        /// </returns>
        public static TResult? Select<TSource, TResult>(
            this TSource? source,
            Func<TSource, TResult> f
        )
            where TSource : struct
            where TResult : struct
        {
            return source is { } s ? f(s) : null;
        }

        /// <summary>
        /// Filters a nullable value based on a predicate. This is similar to
        /// <see cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>.
        /// </summary>
        /// <typeparam name="T">The source value type.</typeparam>
        /// <param name="source">The source value.</param>
        /// <param name="f">A function which determines whether the source value should remain non-null.</param>
        /// <returns>
        /// The result of filtering the value using <paramref name="f"/>. If the predicate returned
        /// <see langword="false"/>, then the result is <see langword="null"/>. If the source was
        /// already <see langword="null"/>, then it remains unchanged.
        /// </returns>
        public static T? Where<T>(this T? source, Predicate<T> f)
            where T : struct
        {
            return source switch
            {
                { } s => f(s) ? s : null,
                _ => null
            };
        }

        /// <summary>
        /// Converts a nullable value to an <see cref="IEnumerable{T}"/>. If the value was
        /// <see langword="null"/>, then the resulting sequence is empty. Otherwise, it contains a
        /// single value containing the <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The source value type.</typeparam>
        /// <param name="source">The source value.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> representing the enumerated source value.
        /// </returns>
        public static IEnumerable<T> AsEnumerable<T>(this T? source)
            where T : struct
        {
            return source switch
            {
                { } s => s.Yield(),
                _ => Enumerable.Empty<T>()
            };
        }
    }
}