/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;
using StardewValley.Network;

namespace FarmExpansion
{
    static class Helpers
    {
        public static void ReplaceWith<T, TField>(this NetVector2Dictionary<T, TField> collection, NetVector2Dictionary<T, TField> source)
            where TField : NetField<T, TField>, new()
        {
            collection.Clear();
            foreach (var kvp in source.Pairs)
            {
                collection.Add(kvp.Key, kvp.Value);
            }
        }

        public static void ReplaceWith(this OverlaidDictionary collection, OverlaidDictionary source)
        {
            collection.Clear();
            foreach (var kvp in source.Pairs)
            {
                collection.Add(kvp.Key, kvp.Value);
            }
        }

        public static void ReplaceWith<T>(this Netcode.NetCollection<T> collection, Netcode.NetCollection<T> source)
            where T : class, Netcode.INetObject<Netcode.INetSerializable>
        {
            collection.Clear();
            foreach (var item in source)
            {
                collection.Add(item);
            }
        }
    }
}
