/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;

namespace TehPers.CoreMod.Api.Conflux {
    public static class OperatorExtensions {
        public static TResult Forward<TSource, TResult>(this TSource source, Func<TSource, TResult> f) => f(source);
        public static void Forward<TSource>(this TSource source, Action<TSource> f) => f(source);
    }
}
