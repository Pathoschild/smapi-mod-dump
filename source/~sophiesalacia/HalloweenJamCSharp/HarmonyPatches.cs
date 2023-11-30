/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace HalloweenJamCSharp;

[HarmonyPatch]
class EventPatches
{
    [HarmonyPatch(typeof(Event), nameof(Event.command_null))]
    [HarmonyPrefix]
    private static void command_null_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
    {
        Log.Trace($"Null command triggered. Parameters: {string.Join('/', split)}");

        if (split.Length < 2)
            return;

        switch (split[1])
        {
            case "computerLight":
                FacilityManager.TurnOnComputerLight();
                break;

            case "initialLights":
                FacilityManager.TurnOnInitialLights();
                break;

            case "facilityLights":
                FacilityManager.TurnOnFacilityLights();
                break;

            case "waitForFacilityLights":
                if (FacilityManager.FacilityLightsDone)
                    break;
                else
                    return;

            case "killLights":
                FacilityManager.KillLights();
                break;

            case "saveTiles":
                FacilityManager.SaveTiles();
                break;

            case "fixTiles":
                FacilityManager.RestoreTiles();
                break;
        }

        __instance.CurrentCommand++;
    }
}
