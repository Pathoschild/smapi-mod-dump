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

namespace DynamicFestivalRewards.Patches;

public class EventPatches
{
    #if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
    #endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static ModConfig Config => ModEntry.Config;

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.DefaultCommands.AwardFestivalPrize\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
            postfix: new HarmonyMethod(typeof(EventPatches), nameof(Post_AwardFestivalPrize))
        );
    }
    
    public static void Post_AwardFestivalPrize(Event __instance, string[] args, EventContext context)
    {
        if(__instance.id is not ("festival_spring13" or "festival_winter8"))
            return;

        string type;
        var individual = Config.Randomize ? Game1.random.Next(Config.MinValue, Config.MaxValue) : Config.MaxValue;
        var total = individual * Game1.player.festivalScore;
        
        switch (__instance.id)
        {
            case "festival_spring13":
                Game1.player.Money -= 1000;
                type = "egg";
                break;
            case "festival_winter8":
                Game1.player.Money -= 2000;
                type = "fish";
                total *= 2;
                break;
            default:
                return;
        }
        
        //either random value btwn custom-1k OR custom
        Log($"Adding {total} to player money...({individual} per {type}. {Game1.player.festivalScore} found)");
        
        Game1.player.Money += total;
    }
}