/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using System.Collections.Generic;

namespace UIInfoSuite.Infrastructure.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue SafeGet<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary, Tkey key, TValue defaultValue = default)
        {
            TValue value = defaultValue;

            if (dictionary != null)
            {
                if (!dictionary.TryGetValue(key, out value))
                    value = defaultValue;
            }

            return value;
        }
    }
}
