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
using ItemExtensions.Additions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ItemExtensions.Patches;

public class FarmTypeManagerPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    { 
        var spawnForageMethod = AccessTools.Method("FarmTypeManager.ModEntry+Utility:SpawnForage",
            new[] { typeof(string),typeof(GameLocation),typeof(Vector2),typeof(bool) });

        if (spawnForageMethod is null) //if the method isn't found, return
        {
            Log($"Method not found. (FarmTypeManager.ModEntry+Utility:SpawnForage)", LogLevel.Warn);
            return;
        }        
        Log($"Applying Harmony patch \"{nameof(FarmTypeManagerPatches)}\": postfixing mod method \"FarmTypeManager.ModEntry.Utility:SpawnForage\".");
        
        harmony.Patch(
            original: spawnForageMethod,
            postfix: new HarmonyMethod(typeof(FarmTypeManagerPatches), nameof(Post_SpawnForage))
        );
    }

    private static void Post_SpawnForage(string index, GameLocation location, Vector2 tile, bool indestructible,
        bool __result)
    {
        if (__result == false)
            return;
        
        location.getObjectAtTile((int)tile.X, (int)tile.Y).modData.Add(ModKeys.IsFtm, "true");
    }
}