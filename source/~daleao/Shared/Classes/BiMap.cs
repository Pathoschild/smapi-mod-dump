/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Classes;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>Represents a collection of forward/reverse key pairs with bidirectional mapping.</summary>
/// <typeparam name="TForward">The forward mapping key type.</typeparam>
/// <typeparam name="TReverse">The reverse mapping key type.</typeparam>
public class BiMap<TForward, TReverse> : IEnumerable<KeyValuePair<TForward, TReverse>>
    where TForward : notnull
    where TReverse : notnull
{
    private readonly Dictionary<TForward, TReverse> _forward = new();
    private readonly Dictionary<TReverse, TForward> _reverse = new();

    /// <summary>Initializes a new instance of the <see cref="BiMap{TForwardKey, TReverseKey}"/> class.</summary>
    public BiMap()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="BiMap{TForwardKey, TReverseKey}"/> class by copying <see cref="KeyValuePair" />s from a one-way <see cref="IDictionary"/>.</summary>
    /// <param name="oneWayMap">A one-way <see cref="IDictionary" />.</param>
    /// <remarks>Throws <see cref="ArgumentException" /> if <paramref name="oneWayMap" /> contains repeated values.</remarks>
    public BiMap(IDictionary<TForward, TReverse> oneWayMap)
    {
        this._forward = new Dictionary<TForward, TReverse>(oneWayMap);
        this._reverse = new Dictionary<TReverse, TForward>();

        foreach (var forward in oneWayMap.Keys)
        {
            var reverse = this._forward[forward];
            if (this._reverse.ContainsKey(reverse))
            {
                ThrowHelper.ThrowArgumentException(
                    "Cannot construct a bidirectional map from a dictionary with non-unique values.");
            }

            this._reverse.Add(reverse, forward);
        }
    }

    /// <summary>Adds the specified set of <typeparamref name="TForward"/> and <typeparamref name="TReverse"/> values to the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="forward">A <typeparamref name="TForward"/> value.</param>
    /// <param name="reverse">A <typeparamref name="TReverse"/> value.</param>
    /// <exception cref="ArgumentException">Thrown if either <paramref name="forward" /> or <paramref name="reverse" /> already exists in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</exception>>
    public void Add(TForward forward, TReverse reverse)
    {
        if (this._forward.ContainsKey(forward))
        {
            ThrowHelper.ThrowArgumentException(
                $"An entry with the same key {forward.ToString()} already exists.");
        }

        if (this._reverse.ContainsKey(reverse))
        {
            ThrowHelper.ThrowArgumentException(
                $"An entry with the same key {reverse.ToString()} already exists.");
        }

        this._forward.Add(forward, reverse);
        this._reverse.Add(reverse, forward);
    }

    /// <summary>Adds the specified <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> to the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="pair">A <see cref="KeyValuePair{TForwardKey, TReverseKey}"/>.</param>
    /// <exception cref="ArgumentException">Thrown if either the key or value already exists in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</exception>>
    public void Add(KeyValuePair<TForward, TReverse> pair)
    {
        this.Add(pair.Key, pair.Value);
    }

    /// <summary>Tries to add the specified set of <typeparamref name="TForward"/> and <typeparamref name="TReverse"/> values to the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="forward">A <typeparamref name="TForward"/> value.</param>
    /// <param name="reverse">A <typeparamref name="TReverse"/> value.</param>
    /// <returns><see langword="true"/> if successfully added, otherwise <see langword="false"/>.</returns>
    public bool TryAdd(TForward forward, TReverse reverse)
    {
        if (this._forward.ContainsKey(forward) || this._reverse.ContainsKey(reverse))
        {
            return false;
        }

        this._forward.Add(forward, reverse);
        this._reverse.Add(reverse, forward);
        return true;
    }

    /// <summary>Try Adds the specified <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> to the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="pair">A <see cref="KeyValuePair{TForwardKey, TReverseKey}"/>.</param>
    /// <returns><see langword="true"/> if successfully added, otherwise <see langword="false"/>.</returns>
    public bool TryAdd(KeyValuePair<TForward, TReverse> pair)
    {
        return this.TryAdd(pair.Key, pair.Value);
    }

    /// <summary>Removes a <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> entry from the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="forward">A <typeparamref name="TForward"/> value.</param>
    /// <returns><see langword="true"/> if successfully removed, otherwise <see langword="false"/>.</returns>
    public bool Remove(TForward forward)
    {
        if (!this._forward.TryGetValue(forward, out var forwardedValue) ||
            !this._reverse.TryGetValue(forwardedValue, out var reversedValue) ||
            !this._forward.Comparer.Equals(reversedValue, forward))
        {
            return false;
        }

        return this._reverse.Remove(this._forward[forward]) && this._forward.Remove(forward);
    }

    /// <summary>Removes a <see cref="KeyValuePair{TForwardKey, TReverseKey}" /> entry from the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="reverse">A <typeparamref name="TReverse"/> value.</param>
    /// <returns><see langword="true"/> if successfully removed, otherwise <see langword="false"/>.</returns>
    public bool Remove(TReverse reverse)
    {
        if (!this._reverse.TryGetValue(reverse, out var reversedValue) ||
            !this._forward.TryGetValue(reversedValue, out var forwardedValue) ||
            !this._reverse.Comparer.Equals(forwardedValue, reverse))
        {
            return false;
        }

        return this._forward.Remove(this._reverse[reverse]) && this._reverse.Remove(reverse);
    }

    /// <summary>Checks if a <typeparamref name="TForward"/> is present in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="key">The <typeparamref name="TForward"/> object to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> exists.</returns>
    public bool Contains(TForward key)
    {
        return this._forward.ContainsKey(key);
    }

    /// <summary>Checks if a <typeparamref name="TReverse"/> is present in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <param name="key">The <typeparamref name="TReverse"/> object to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> exists.</returns>
    public bool Contains(TReverse key)
    {
        return this._reverse.ContainsKey(key);
    }

    /// <summary>
    ///     Checks if a <typeparamref name="TForward"/> is present in the <see cref="BiMap{TForwardKey,TReverseKey}"/> and returns the corresponding
    ///     <typeparamref name="TReverse"/> value.
    /// </summary>
    /// <param name="forward">The <typeparamref name="TForward" /> object to check.</param>
    /// <param name="reverse">The corresponding <typeparamref name="TReverse"/>, if any.</param>
    /// <returns><see langword="true"/> if a value was retrieved, otherwise <see langword="false"/>.</returns>
    public bool TryGetForward(TForward forward, [NotNullWhen(true)] out TReverse? reverse)
    {
        return this._forward.TryGetValue(forward, out reverse);
    }

    /// <summary>
    ///     Checks if a <typeparamref name="TReverse"/> is present in the <see cref="BiMap{TForwardKey,TReverseKey}"/> and returns the corresponding
    ///     <typeparamref name="TForward"/> value.
    /// </summary>
    /// <param name="reverse">The <typeparamref name="TReverse"/> object to check.</param>
    /// <param name="forward">The corresponding <typeparamref name="TForward"/>, if any.</param>
    /// <returns><see langword="true"/> if a value was retrieved, otherwise <see langword="false"/>.</returns>
    public bool TryGetReverse(TReverse reverse, [NotNullWhen(true)] out TForward? forward)
    {
        return this._reverse.TryGetValue(reverse, out forward);
    }

    /// <summary>Clears the instance of all entries.</summary>
    public void Clear()
    {
        this._forward.Clear();
        this._reverse.Clear();
    }

    /// <summary>Gets the number of entries in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</summary>
    /// <returns>The number of entries in the <see cref="BiMap{TForwardKey,TReverseKey}"/>.</returns>
    public int Count()
    {
        return this._forward.Count;
    }

    /// <inheritdoc />
    public IEnumerator GetEnumerator()
    {
        return this._forward.GetEnumerator();
    }

    IEnumerator<KeyValuePair<TForward, TReverse>> IEnumerable<KeyValuePair<TForward, TReverse>>.GetEnumerator()
    {
        return this._forward.GetEnumerator();
    }
}
