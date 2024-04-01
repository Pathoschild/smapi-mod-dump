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

namespace GingerIslandStart.Patches;

public class FarmerPatches
{
        
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(Farmer)}\": postfixing SDV method \"Farmer.performPassOut\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), "performPassOut"),
            postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(Post_performPassOut))
        );
    }

    /// <summary>
    /// Warp farmer back to their tent, if required.
    /// </summary>
    /// <param name="__instance"></param>
    internal static void Post_performPassOut(Farmer __instance)
    {
        var hasHouse = Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse");
        if (hasHouse || !Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;
        
        var passOutLocation = __instance.currentLocation;
        if (!passOutLocation.InIslandContext())
            return;

        ModEntry.NeedsWarp = true;
    }
}