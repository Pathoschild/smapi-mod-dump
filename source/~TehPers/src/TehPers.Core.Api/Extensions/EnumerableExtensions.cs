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
using System.Text;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/> and some subtypes.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        public static void Shuffle<T>(this IList<T> source)
        {
            source.Shuffle(new Random());
        }

        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        /// <param name="rand">The <see cref="Random"/> to use while shuffling.</param>
        public static void Shuffle<T>(this IList<T> source, Random rand)
        {
            _ = rand ?? throw new ArgumentNullException(nameof(rand));
            _ = source ?? throw new ArgumentNullException(nameof(source));

            var n = source.Count;
            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                (source[k], source[n]) = (source[n], source[k]);
            }
        }

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <param name="comparer">The comparer for the hash set.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed.</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(
            this IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer
        )
        {
            return new HashSet<TSource>(source.ToArray(), comparer);
        }

        /// <summary>Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="Dictionary{TKey,TValue}"/>.</summary>
        /// <typeparam name="TKey">The type of the keys in the <paramref name="source" />.</typeparam>
        /// <typeparam name="TValue">The type of the values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <returns>A dictionary containing all the <see cref="KeyValuePair{TKey,TValue}"/> entries in the <paramref name="source"/>.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source
        )
            where TKey : notnull
        {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> to a <see cref="Dictionary{TKey,TValue}"/> using a custom <see cref="IEqualityComparer{T}"/>.</summary>
        /// <typeparam name="TKey">The type of the keys in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TValue">The type of the values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparer">The comparer used to compare keys in the dictionary.</param>
        /// <returns>A dictionary containing all the <see cref="KeyValuePair{TKey,TValue}"/> entries in the <paramref name="source"/>.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source,
            IEqualityComparer<TKey> comparer
        )
            where TKey : notnull
        {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value, comparer);
        }

        /// <summary>Swaps two elements in a list.</summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="first">The index of the first element.</param>
        /// <param name="second">The index of the second element.</param>
        public static void Swap<T>(this IList<T> source, int first, int second)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            if (first < 0 || first > source.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(first));
            }

            if (second < 0 || second > source.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(second));
            }

            (source[first], source[second]) = (source[second], source[first]);
        }

        /// <summary>Retrieves a value from a <see cref="IDictionary{TKey,TValue}"/> with the given fallback value.</summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey,TValue}"/>'s key type.</typeparam>
        /// <typeparam name="TVal">The <see cref="IDictionary{TKey,TValue}"/>'s value type.</typeparam>
        /// <param name="source">The dictionary to try to retrieve the value from.</param>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="fallback">The fallback value if the key doesn't exist in the dictionary.</param>
        /// <returns>If the key exists in <paramref name="source"/>, the value associated with <paramref name="key"/>, otherwise <paramref name="fallback"/>.</returns>
        public static TVal? GetDefault<TKey, TVal>(
            this IDictionary<TKey, TVal> source,
            TKey key,
            TVal? fallback = default
        )
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.TryGetValue(key, out var value) ? value : fallback;
        }

        /// <summary>Splits an <see cref="IEnumerable{T}"/> into several smaller ones, each containing at most a certain number of elements.</summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to split.</param>
        /// <param name="size">The number of elements each group should have. The last group may contain fewer elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements from <paramref name="source"/> split into groups of at most <paramref name="source"/> elements.</returns>
        public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int size)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            // Keep track of whether there are any items left
            bool itemsLeft;

            // Enumerate the source manually
            using (var enumerator = source.GetEnumerator())
            {
                // Move to the first item
                itemsLeft = enumerator.MoveNext();

                // Make groups until out if items
                while (itemsLeft)
                {
                    yield return GetGroup(enumerator);
                }
            }

            // Creates a group of items from the given enumerator
            IEnumerable<T> GetGroup(IEnumerator<T> e)
            {
                // Keep track of the current size of this group
                var itemsRemaining = size;

                // Yield items until either the group has reached the given size or there are no more items
                while (itemsRemaining-- > 0 && itemsLeft)
                {
                    yield return e.Current;
                    itemsLeft = e.MoveNext();
                }
            }
        }

        /// <summary>Tries to get a value out of a dictionary. If it fails, uses a factory function to generate a new value, returning that instead and adding it to the dictionary.</summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">A function which returns a value to put in the dictionary if the key doesn't exist.</param>
        /// <returns>The existing item with the given key in the source dictionary, or the factory-generated value if the key doesn't already exist.</returns>
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            Func<TValue> factory
        )
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _ = source ?? throw new ArgumentNullException(nameof(source));

            if (source.TryGetValue(key, out var value))
            {
                return value;
            }

            value = factory();
            source.Add(key, value);
            return value;
        }

        /// <summary>Creates a new string containing one string repeated any number of times.</summary>
        /// <param name="input">The string to repeat.</param>
        /// <param name="count">How many times to repeat it.</param>
        /// <returns><paramref name="input"/> repeated <paramref name="count"/> times.</returns>
        /// <remarks>Based on this SO answer: https://stackoverflow.com/a/3754626. </remarks>
        public static string Repeat(this string input, int count)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(input.Length * count);
            while (count-- > 0)
            {
                builder.Append(input);
            }

            return builder.ToString();
        }

        /// <summary>Wraps this object instance into an <see cref="IEnumerable{T}"/> consisting of a single item.</summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="item">The instance that will be wrapped.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> consisting of a single item.</returns>
        /// <remarks>Based on this SO question: https://stackoverflow.com/q/1577822. </remarks>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Enumerates the items in a sequence along with their index in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="source">The source items.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the source items and their index in the sequence.</returns>
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> source)
        {
            var curIndex = 0;
            foreach (var item in source)
            {
                yield return (curIndex, item);
                curIndex = checked(curIndex + 1);
            }
        }
    }
}