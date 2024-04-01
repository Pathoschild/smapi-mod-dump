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

public class Game1Patches
{
    private static string PirateKey => $"{ModEntry.Id}_PirateRecovery";
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(Game1Patches)}\": prefixing SDV method \"Game1.createMultipleObjectDebris(string, int, int, int, GameLocation)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), nameof(Game1.createMultipleObjectDebris), new[]{typeof(string),typeof(int),typeof(int),typeof(int),typeof(GameLocation)}),
            prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Pre_createMultipleObjectDebris))
        );
    }
    
    internal static void Pre_createMultipleObjectDebris(ref string id, int xTile, int yTile, ref int number, GameLocation location)
    {
        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;
        
        if(Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
            return;
        
        if (id != "(O)688")
            return;

        //if hasnt unlocked boat yet, turn farm warps into mixed seeds
        id = "(O)770";
        number = 5;
    }
}