/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> getValue)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                dictionary.Add(key, value = getValue());
            }
            return value;
        }
    }
}