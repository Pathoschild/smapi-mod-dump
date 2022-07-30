/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Netcode;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Class that holds extension methods for NetFieldDictionary.
/// </summary>
public static class NetFieldDictionaryExtensions
{
    /// <summary>
    /// Helper method to try to add a value to a Stardew NetFieldDictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <typeparam name="TField">Used by NetFieldDictionary.</typeparam>
    /// <typeparam name="TSerialDict">Also used by NetFieldDictonary.</typeparam>
    /// <typeparam name="TSelf">For NetFieldDictionary.</typeparam>
    /// <param name="dictionary">Dictionary to add to.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    /// <returns>True if value added, false otherwise.</returns>
    /// <remarks>.Add throws an error if the item is already in the dictionary. This doesn't.</remarks>
    public static bool TryAdd<TKey, TValue, TField, TSerialDict, TSelf>(
        this NetFieldDictionary<TKey, TValue, TField, TSerialDict, TSelf> dictionary,
        TKey key,
        TValue value)
            where TField : NetField<TValue, TField>, new()
            where TSerialDict : IDictionary<TKey, TValue>, new()
            where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
    {
        if(dictionary.ContainsKey(key))
        {
            return false;
        }
        dictionary.Add(key, value);
        return true;
    }

    public static void Update<TKey, TValue, TField, TSerialDict, TSelf>(
        this NetFieldDictionary<TKey, TValue, TField, TSerialDict, TSelf> dictionary,
        IDictionary<TKey, TValue> other)
            where TField : NetField<TValue, TField>, new()
            where TSerialDict : IDictionary<TKey, TValue>, new()
            where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
    {
        foreach ((TKey k, TValue v) in other)
        {
            dictionary[k] = v;
        }
    }

    public static void Update<TKey, TValue, TField, TSerialDict, TSelf>(
        this NetFieldDictionary<TKey, TValue, TField, TSerialDict, TSelf> dictionary,
        IEnumerable<KeyValuePair<TKey, TValue>> other)
            where TField : NetField<TValue, TField>, new()
            where TSerialDict : IDictionary<TKey, TValue>, new()
            where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
    {
        foreach ((TKey k, TValue v) in other)
        {
            dictionary[k] = v;
        }
    }
}