/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMultiplayerInfo.Helpers
{
    public static class DictionaryExtensions
    {
        public static TTypeB GetOrCreateDefault<TTypeA, TTypeB>(this Dictionary<TTypeA, TTypeB> input, TTypeA key)
        where TTypeB : class, new()
        {
            if (!input.ContainsKey(key))
            {
                input.Add(key, new TTypeB());
            }

            return input[key];
        }

    }
}
