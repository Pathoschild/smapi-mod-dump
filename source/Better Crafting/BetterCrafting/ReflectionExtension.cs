/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RedstoneBoy/BetterCrafting
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrafting
{
    static class ReflectionExtension
    {
        public static TValue GetFieldValue<TValue>(this IReflectionHelper reflection, object obj, string name, bool required = true)
        {
            return reflection.GetPrivateField<TValue>(obj, name, required).GetValue();
        }
    }
}
