/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bmarquismarkail/SV_BetterQualityMoreSeeds
**
*************************************************/


using System.Collections.Generic;
using SObject = StardewValley.Object;
using StardewModdingAPI;

namespace BetterQualityMoreSeeds.Framework
{
    class PatchCommon
    {
        public static void PostFix(KeyValuePair<SObject, SObject> __state, IMonitor Monitor)
        {
            if (__state.Key == null || __state.Value == null) return;
            Monitor.Log("Adding more seeds by quality", LogLevel.Debug);
            __state.Key.heldObject.Value.Stack += (__state.Value.Quality >= 4 ? __state.Value.Quality - 1 : __state.Value.Quality);

        }

    }
}