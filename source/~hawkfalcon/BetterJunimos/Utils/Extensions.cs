/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BetterJunimos.Utils {
    internal static class ListExtensions {
        /// <summary>Create lists of tuples more easily.</summary>
        public static void Add<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list, T1 item1, T2 item2, T3 item3) {
            list.Add(new Tuple<T1, T2, T3>(item1, item2, item3));
        }
    }
    
    public static class StringExtensions
    {
        public static string SplitCamelCase( this string str )
        {
            return Regex.Replace( 
                Regex.Replace( 
                    str, 
                    @"(\P{Ll})(\P{Ll}\p{Ll})", 
                    "$1 $2" 
                ), 
                @"(\p{Ll})(\P{Ll})", 
                "$1 $2" 
            ).Replace("  ", " ");
        }
    }
}
