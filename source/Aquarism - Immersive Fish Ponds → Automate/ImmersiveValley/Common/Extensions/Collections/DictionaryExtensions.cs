/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Extensions.Collections;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Dictionary{TKey,TValue}"/> class.</summary>
public static class DictionaryExtensions
{
    /// <summary>Flatten pairs in a <see cref="Dictionary{TKey,TValue}" /> into a single string.</summary>
    /// <param name="keyValueSeparator">String inserted between key and value.</param>
    /// <param name="pairSeparator">String inserted between pairs.</param>
    public static string Stringify<TKey, TValue>(this Dictionary<TKey, TValue> d, string keyValueSeparator = ",",
        string pairSeparator = ";") where TKey : notnull
    {
        if (string.IsNullOrEmpty(keyValueSeparator) || string.IsNullOrEmpty(pairSeparator))
            throw new ArgumentException("Separator cannot be null or empty.");

        if (pairSeparator == keyValueSeparator)
            throw new ArgumentException("Pair separator must be different from key-value separator.");

        return !d.Any() ? string.Empty : string.Join(pairSeparator, d.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }
}