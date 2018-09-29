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