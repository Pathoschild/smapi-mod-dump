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

namespace TehPers.CoreMod.Api.Extensions {
    public static class EnumerableExtensions {
        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        public static void Shuffle<T>(this IList<T> source) => source.Shuffle(new Random());

        /// <summary>Shuffles a list.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IList{T}"/> to shuffle.</param>
        /// <param name="rand">The <see cref="Random"/> to use while shuffling.</param>
        public static void Shuffle<T>(this IList<T> source, Random rand) {
            int n = source.Count;
            while (n > 1) {
                n--;
                int k = rand.Next(n + 1);
                T value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(EqualityComparer<TSource>.Default);

        /// <summary>Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="TSource">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
        /// <param name="comparer">The comparer for the hash set.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type <see cref="TSource"/> selected from the input sequence.</returns>
        /// <remarks>In framework versions 4.7.2+, this method can be removed</remarks>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer) => new HashSet<TSource>(source.ToArray(), comparer);

        /// <summary>Swaps two elements in a list.</summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="first">The index of the first element.</param>
        /// <param name="second">The index of the second element.</param>
        public static void Swap<T>(this IList<T> source, int first, int second) {
            T tmp = source[first];
            source[first] = source[second];
            source[second] = tmp;
        }

        /// <summary>Retrieves a value from a <see cref="IDictionary{TKey,TValue}"/> with the given fallback value</summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey,TValue}"/>'s key type</typeparam>
        /// <typeparam name="TVal">The <see cref="IDictionary{TKey,TValue}"/>'s value type</typeparam>
        /// <param name="source">The dictionary to try to retrieve the value from</param>
        /// <param name="key">The key of the value to retrieve</param>
        /// <param name="fallback">The fallback value if the key doesn't exist in the dictionary</param>
        /// <returns>If the key exists in <see cref="source"/>, the value associated with <see cref="key"/>, otherwise <see cref="fallback"/></returns>
        public static TVal GetDefault<TKey, TVal>(this IDictionary<TKey, TVal> source, TKey key, TVal fallback = default(TVal)) => source.ContainsKey(key) ? source[key] : fallback;

        /// <summary>Splits an <see cref="IEnumerable{T}"/> into several smaller ones, each containing at most a certain number of elements.</summary>
        /// <typeparam name="T">The type of the elements of <see cref="source"/></typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to split.</param>
        /// <param name="size">The number of elements each group should have. The last group may contain fewer elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the elements from <see cref="source"/> split into groups of at most <see cref="source"/> elements.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size) {
            bool grouping;
            using (IEnumerator<T> enumerator = source.GetEnumerator()) {
                grouping = enumerator.MoveNext();
                while (grouping) {
                    yield return GetGroup(enumerator);
                }
            }

            IEnumerable<T> GetGroup(IEnumerator<T> e) {
                int n = size;
                while (n-- > 0 && grouping) {
                    yield return e.Current;
                    grouping = e.MoveNext();
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
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> factory) {
            if (source.TryGetValue(key, out TValue value)) {
                return value;
            }

            value = factory();
            source.Add(key, value);
            return value;
        }

        /// <summary>Creates a new string containing one string repeated any number of times.</summary>
        /// <param name="input">The string to repeat</param>
        /// <param name="count">How many times to repeat it</param>
        /// <returns><see cref="input"/> repeated <see cref="count"/> times</returns>
        /// <remarks>Based on this SO answer: https://stackoverflow.com/a/3754626</remarks>
        public static string Repeat(this string input, int count) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder builder = new StringBuilder(input.Length * count);
            for (int i = 0; i < count; i++) {
                builder.Append(input);
            }

            return builder.ToString();

        }

        /// <summary>Wraps this object instance into an <see cref="IEnumerable{T}"/> consisting of a single item.</summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="item">The instance that will be wrapped.</param>
        /// <returns>An IEnumerable&lt;T&gt; consisting of a single item.</returns>
        /// <remarks>Based on this SO question: https://stackoverflow.com/q/1577822</remarks>
        public static IEnumerable<T> Yield<T>(this T item) {
            yield return item;
        }

        /// <summary>Prepends an item to the beginning of an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="T">The type of elements in the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="item">The item to prepend to the source.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> with the item at the start of it.</returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item) {
            return item.Yield().Concat(source);
        }

        /// <summary>Appends an item to the end of an <see cref="IEnumerable{T}"/>.</summary>
        /// <typeparam name="T">The type of elements in the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="item">The item to append to the source.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> with the item at the end of it.</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item) {
            return source.Concat(item.Yield());
        }

        /// <summary>Groups the elements of an <see cref="IEnumerable{T}"/> into multiple <see cref="IEnumerable{T}"/> instances, each with up to a certain number of elements.</summary>
        /// <typeparam name="T">The type of elements in the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="countPerWindow">The number of items each window should have at most. The last window may contain fewer items in it.</param>
        /// <returns>A new <seealso cref="IEnumerable{T}"/> with the groups.</returns>
        public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int countPerWindow) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (countPerWindow <= 0) {
                throw new ArgumentOutOfRangeException(nameof(countPerWindow), "Must be greater than zero");
            }

            // Return as many windows as possible
            using (IEnumerator<T> enumerator = source.GetEnumerator()) {
                // Loop until there are no more items left
                while (enumerator.MoveNext()) {
                    // Since the enumerator was moved, pass the current element into the window and yield that window
                    yield return GetWindow(enumerator, enumerator.Current);
                }
            }

            IEnumerable<T> GetWindow(IEnumerator<T> enumerator, T buffered) {
                // Yield the buffered item
                yield return buffered;

                // Keep going
                int remaining = countPerWindow - 1;
                while (remaining-- > 0 && enumerator.MoveNext()) {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
