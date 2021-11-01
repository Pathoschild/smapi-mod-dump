/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace CryptOfTheNecrodancerEnemies.Framework.Extensions {
  public static class DictionaryExtension {

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The generic collection of key/value pairs.</param>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
    /// with the specified <paramref name="key"/>, otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetFromNullableKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value) {
      value = default;
      return key != null && dictionary.TryGetValue(key, out value);
    }
  }
}
