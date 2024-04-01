/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ItemExtensions.Patches;

public class HoeDirtPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif

    internal static string Cached { get; set; }
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static bool HasCropsAnytime { get; set; }
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(HoeDirtPatches)}\": postfixing SDV method \"HoeDirt.plant\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant), new[]{typeof(string), typeof(Farmer), typeof(bool)}),
            postfix: new HarmonyMethod(typeof(HoeDirtPatches), nameof(Post_plant))
        );
    }

    private static void Post_plant(string itemId, Farmer who, bool isFertilizer)
    {
        #if DEBUG
        Log($"Clearing seed cache...(last item {itemId})");
        #endif
        CropPatches.Cached = null;
    }
}