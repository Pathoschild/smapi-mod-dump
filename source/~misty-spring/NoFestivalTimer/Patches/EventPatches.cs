/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace NoFestivalTimer.Patches;

public class EventPatches
{
    #if DEBUG
    private static LogLevel Level =>  LogLevel.Debug;
    #else
    private static LogLevel Level =>  LogLevel.Trace;
    #endif
    
    private static IModHelper Helper => ModEntry.Help;
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    private static Dictionary<string, ExclusionData> Festivals => ModEntry.Exclusions;

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.receiveActionPress\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.receiveActionPress)),
            postfix: new HarmonyMethod(typeof(EventPatches), nameof(Post_receiveActionPress))
        );
        
        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.festivalUpdate\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.festivalUpdate)),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(Post_festivalUpdate))
        );
    }
    
    public static void Post_receiveActionPress(ref Event __instance, int xTile, int yTile)
    {
        if(__instance.playerControlSequenceID == null)
            return;

        if(!Festivals.ContainsKey(__instance.playerControlSequenceID))
            return;

        CheckProgress(__instance);
    }

    private static void CheckProgress(Event __instance)
    {
        var score = Festivals[__instance.playerControlSequenceID].OnScore;
        var useProps = Festivals[__instance.playerControlSequenceID].Props;

        if (score > 0 && score <= Game1.player.festivalScore)
        {
            __instance.festivalTimer = 1;
        }

        if (useProps && __instance.festivalProps.Count == 0)
        {
            __instance.festivalTimer = 1;
        }
    }

    public static void Post_festivalUpdate(ref Event __instance, GameTime time)
    {
        if (__instance == null)
            return;
        
        if (__instance.festivalTimer <= 1)
            return;

        if(__instance.playerControlSequenceID == null)
            return;
        
        if(!Festivals.ContainsKey(__instance.playerControlSequenceID))
            return;
        
        if (Festivals[__instance.playerControlSequenceID].IgnoreTimer)
            __instance.festivalTimer += time.ElapsedGameTime.Milliseconds;
        
        CheckProgress(__instance);
    }
}