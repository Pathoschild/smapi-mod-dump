
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