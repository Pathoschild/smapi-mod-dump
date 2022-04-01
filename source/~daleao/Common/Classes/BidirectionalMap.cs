/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Common.Classes;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;

#endregion using directives

/// <summary>Represents a collection of forward/reverse key pairs with bidirectional mapping.</summary>
/// <typeparam name="TForwardKey">Forward mapping key.</typeparam>
/// <typeparam name="TReverseKey">Reverse mapping key</typeparam>
public class BiMap<TForwardKey, TReverseKey> : IEnumerable<KeyValuePair<TForwardKey, TReverseKey>>
{
    /// <summary>Default constructor.</summary>
    public BiMap()
    {
    }

    /// <summary>Construct an instance by copying <see cref="KeyValuePairs" /> from a one-way <see cref="IDictionary" />.</summary>
    /// <param name="oneWayMap">A one-way <see cref="IDictionary" />.</param>
    /// <remarks>Throws <see cref="ArgumentException" /> if <paramref name="oneWayMap" /> contains repeated values.</remarks>
    public BiMap(IDictionary<TForwardKey, TReverseKey> oneWayMap)
    {
        Forward = new(oneWayMap);
        Reverse = new();

        foreach (var forwardKey in oneWayMap.Keys)
        {
            var reverseKey = Forward[forwardKey];
            if (Reverse.ContainsKey(reverseKey))
                throw new ArgumentException(
                    "Cannot construct bidirectional map from a dictionary with non-unique values.");

            Reverse.Add(reverseKey, forwardKey);
        }
    }

    public Indexer<TForwardKey, TReverseKey> Forward { get; } = new();
    public Indexer<TReverseKey, TForwardKey> Reverse { get; } = new();

    IEnumerator<KeyValuePair<TForwardKey, TReverseKey>> IEnumerable<KeyValuePair<TForwardKey, TReverseKey>>.GetEnumerator()
    {
        return Forward.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return Forward.GetEnumerator();
    }

    /// <summary>Add a new <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> entry to the instance.</summary>
    /// <param name="forwardKey">A <typeparamref name="TForwardKey" /> object.</param>
    /// <param name="reverseKey">A <typeparamref name="TReverseKey" /> object.</param>
    /// <remarks>
    ///     Throws <see cref="ArgumentException" /> if either <paramref name="forwardKey" /> or
    ///     <paramref name="reverseKey" /> already exist in the instance.
    /// </remarks>
    public void Add(TForwardKey forwardKey, TReverseKey reverseKey)
    {
        if (Forward.ContainsKey(forwardKey))
            throw new ArgumentException(
                $"An entry with the same key {forwardKey?.ToString() ?? string.Empty} already exists.");
        if (Reverse.ContainsKey(reverseKey))
            throw new ArgumentException(
                $"An entry with the same key {reverseKey?.ToString() ?? string.Empty} already exists.");

        Forward.Add(forwardKey, reverseKey);
        Reverse.Add(reverseKey, forwardKey);
    }

    /// <summary>Remove a <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> entry from the instance.</summary>
    /// <param name="forwardKey">Forward key.</param>
    /// <returns>Returns true if the pair exists and was successfully removed, otherwise returns false.</returns>
    public bool Remove(TForwardKey forwardKey)
    {
        if (!Forward.ContainsKey(forwardKey) || !Reverse.ContainsKey(Forward[forwardKey]))
            return false;

        return Reverse.Remove(Forward[forwardKey]) && Forward.Remove(forwardKey);
    }

    /// <summary>Remove a <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> entry from the instance.</summary>
    /// <param name="entry"><see cref="KeyValuePair{TForwardKey, TReserveKey}" /> entry to be removed.</param>
    /// <returns>Returns true if the pair exists and was successfully removed, otherwise returns false.</returns>
    public bool Remove(KeyValuePair<TForwardKey, TReverseKey> entry)
    {
        return Forward.Remove(entry.Key) && Reverse.Remove(entry.Value);
    }

    /// <summary>Check if a <typeparamref name="TForwardKey" /> is present in the instance.</summary>
    /// <param name="key">The <typeparamref name="TForwardKey" /> object to check.</param>
    public bool Contains(TForwardKey key)
    {
        return Forward.ContainsKey(key);
    }

    /// <summary>Check if a <typeparamref name="TReverseKey" /> is present in the instance.</summary>
    /// <param name="key">The <typeparamref name="TReverseKey" /> object to check.</param>
    public bool Contains(TReverseKey key)
    {
        return Reverse.ContainsKey(key);
    }

    /// <summary>
    ///     Check if a <typeparamref name="TForwardKey" /> is present in the instance and return the corresponding
    ///     <typeparamref name="TReverseKey" />.
    /// </summary>
    /// <param name="forwardKey">The <typeparamref name="TForwardKey" /> object to check.</param>
    /// <param name="reverseKey">The corresponding <typeparamref name="TReverseKey" /> object, if any.</param>
    public bool TryGetForwardValue(TForwardKey forwardKey, out TReverseKey reverseKey)
    {
        return Forward.TryGetValue(forwardKey, out reverseKey);
    }

    /// <summary>
    ///     Check if a <typeparamref name="TReverseKey" /> is present in the instance and return the corresponding
    ///     <typeparamref name="TForwardKey" />.
    /// </summary>
    /// <param name="reverseKey">The <typeparamref name="TReverseKey" /> object to check.</param>
    /// <param name="forwardKey">The corresponding <typeparamref name="TForwardKey" /> object, if any.</param>
    public bool TryGetReverseValue(TReverseKey reverseKey, out TForwardKey forwardKey)
    {
        return Reverse.TryGetValue(reverseKey, out forwardKey);
    }

    /// <summary>Clear the instance of all entries.</summary>
    public void Clear()
    {
        Forward.Clear();
        Reverse.Clear();
    }

    /// <summary>Get the number of entries in the instance.</summary>
    public int Count()
    {
        return Forward.Count();
    }

    /// <summary>Publicly read-only lookup to prevent inconsistent state between forward and reverse map lookup.</summary>
    public class Indexer<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public Indexer()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public Indexer(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public TValue this[TKey index] => _dictionary[index];

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public static implicit operator Dictionary<TKey, TValue>(Indexer<TKey, TValue> indexer)
        {
            return new(indexer._dictionary);
        }

        internal void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        internal bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        internal void Clear()
        {
            _dictionary.Clear();
        }

        internal int Count()
        {
            return _dictionary.Count;
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return new(_dictionary);
        }
    }
}