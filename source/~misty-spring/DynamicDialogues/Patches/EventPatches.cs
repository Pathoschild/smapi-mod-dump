/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using DynamicDialogues.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace DynamicDialogues.Patches;

internal class EventPatches
{
    internal static void Apply(Harmony harmony)
    {
        ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(EventPatches)}\": prefixing SDV method \"Event.tryEventCommand(GameLocation location, GameTime time, string[] args)\".");

        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.tryEventCommand)),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(PrefixTryGetCommandH))
        );
    }
    
    //for AddScene
    internal static bool PrefixTryGetCommandH(Event __instance, GameLocation location, GameTime time, string[] args) =>
        PrefixTryGetCommand(__instance, location, time, args);

    private static bool PrefixTryGetCommand(Event __instance, GameLocation location, GameTime time, string[] split)
    {
        if (split.Length <= 1) //scene has optional parameters, so its 2 OR more
        {
            return true;
        }
        else if (split[0].Equals(ModEntry.AddScene, StringComparison.Ordinal))
        {
            EventScene.Add(__instance, location, time, split);
            return false;
        }
        else if (split[0].Equals(ModEntry.RemoveScene, StringComparison.Ordinal))
        {
            EventScene.Remove(__instance, location, time, split);
            return false;
        }
        else if(split[0].Equals(ModEntry.PlayerFind, StringComparison.Ordinal))
        {
            Finder.ObjectHunt(__instance, location, time, split);
            return false;
        }
        return true;
    }
}