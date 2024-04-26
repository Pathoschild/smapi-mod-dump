/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chsiao58/EvenBetterArtisanGoodIcons
**
*************************************************/

using System;
using System.Collections.Generic;

namespace BetterArtisanGoodIcons.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>Create dictionaries with tuples as values more easily.</summary>
        public static void Add<TKey, T1, T2>(this Dictionary<TKey, Tuple<T1, T2>> dictionary, TKey key, T1 item1, T2 item2)
        {
            dictionary.Add(key, new Tuple<T1, T2>(item1, item2));
        }
    }
}