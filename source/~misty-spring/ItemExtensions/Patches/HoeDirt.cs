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
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(HoeDirtPatches)}\": prefixing SDV method \"HoeDirt.canPlantThisSeedHere\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.canPlantThisSeedHere)),
            prefix: new HarmonyMethod(typeof(HoeDirtPatches), nameof(Pre_canPlantThisSeedHere))
        );
        
        Log($"Applying Harmony patch \"{nameof(HoeDirtPatches)}\": prefixing SDV method \"HoeDirt.plant\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant), new[]{typeof(string), typeof(Farmer), typeof(bool)}),
            prefix: new HarmonyMethod(typeof(HoeDirtPatches), nameof(Pre_plant))
        );
        
        Log($"Applying Harmony patch \"{nameof(HoeDirtPatches)}\": postfixing SDV method \"HoeDirt.plant\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant), new[]{typeof(string), typeof(Farmer), typeof(bool)}),
            postfix: new HarmonyMethod(typeof(HoeDirtPatches), nameof(Post_plant))
        );
    }
    
    private static void Pre_canPlantThisSeedHere(HoeDirt __instance, ref string itemId, bool isFertilizer = false)
    {
        itemId = CropPatches.ResolveSeedId(itemId, __instance.Location);
#if DEBUG
        Log($"Changed ItemId to {itemId}");
#endif
    }
    
    private static void Pre_plant(ref string itemId, Farmer who, bool isFertilizer)
    {
        if(CropPatches.Chosen)
            itemId = CropPatches.Cached;
    }
    
    private static void Post_plant(string itemId, Farmer who, bool isFertilizer)
    {
#if DEBUG
        Log("_____________________________________________");
#endif
        Log($"Clearing seed cache...(last item {itemId})");
        CropPatches.Cached = null;
        CropPatches.Chosen = false;
    }
}