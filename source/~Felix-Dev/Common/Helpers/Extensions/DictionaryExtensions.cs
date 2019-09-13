using System;
using System.Collections.Generic;
using System.Text;

namespace FelixDev.StardewMods.Common.Helpers.Extensions
{
    /// <summary>
    /// The <see cref="DictionaryExtensions"/> class provides an API to simplify common tasks when working with 
    /// instances of the <see cref="Dictionary{TKey, TValue}"/> class.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Add the specified value to the dictionary which elements are lists of values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values contained in the dictionary's list elements.</typeparam>
        /// <param name="dictionary">The dictionary to add the specified value to.</param>
        /// <param name="key">The key of the list to add the specified <paramref name="value"/> to.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <remarks>
        /// If there is no <see cref="List{TValue}"/> instance available for the specified <paramref name="key"/> 
        /// a new list is created and added to the <paramref name="dictionary"/>.
        /// </remarks>
        public static void AddToList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryGetValue(key, out List<TValue> values))
            {
                dictionary.Add(key, values = new List<TValue>());
            }

            values.Add(value);
        }

        /// <summary>
        /// Remove the specified value from a dictionary which elements are lists of values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values contained in the dictionary's list elements.</typeparam>
        /// <param name="dictionary">The dictionary to remove the specified value from.</param>
        /// <param name="key">The key of the list to remove the specified <paramref name="value"/> from.</param>
        /// <param name="value">The value of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the specified value is successfully removed; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If the containing <see cref="List{TValue}"/> instance is empty after the value was removed it will be removed
        /// from the <paramref name="dictionary"/>.
        /// </remarks>
        public static bool RemoveFromList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            return !dictionary[key].Remove(value)
                ? false
                : dictionary[key].Count == 0
                    ? dictionary.Remove(key)
                    : true;
        }
    }
}
