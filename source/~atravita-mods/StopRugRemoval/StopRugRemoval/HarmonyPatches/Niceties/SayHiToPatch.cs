/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Patches for SayHiTo...
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class SayHiToPatch
{
    // Short circuit this if there's no player on the map. It handles only the text NPCs say when they enter locations.
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)] // run after every other prefix.
    [HarmonyPatch(nameof(NPC.arriveAt))]
    private static bool PrefixArriveAt(GameLocation l)
        => l == Game1.player.currentLocation;

    // Short circuit this if there's no player on the map. It literally only handles saying hi to people.
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)] // run after every other prefix.
    [HarmonyPatch(nameof(NPC.performTenMinuteUpdate))]
    private static bool PrefixTenMinuteUpdate(GameLocation l)
        => l == Game1.player.currentLocation;

    // Get the NPCs to pretty frequently greet each other in the saloon?
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(nameof(NPC.performTenMinuteUpdate))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void PostfixTenMinuteUpdate(NPC __instance, GameLocation l, int ___textAboveHeadTimer)
    {
        if (Game1.player.currentLocation == l && l.Name.Equals("Saloon", StringComparison.OrdinalIgnoreCase)
            && __instance.isVillager())
        {
            if (__instance.isMoving() && ___textAboveHeadTimer < 0 && Game1.random.NextDouble() < 0.5)
            {
                // Invert the check here to favor the farmer. :(
                // Goddamnit greet me more often plz.
                Character? c = Utility.isThereAFarmerWithinDistance(__instance.getTileLocation(), 4, l);
                if (c is null)
                {
                    Vector2 loc = __instance.getTileLocation();
                    foreach (NPC npc in l.characters)
                    {
                        if ((npc.getTileLocation() - loc).LengthSquared() <= 16 && __instance.isFacingToward(npc.getTileLocation()))
                        {
                            c = npc;
                            break;
                        }
                    }
                }

                if (c is not null)
                {
                    __instance.sayHiTo(c);
                }
            }
        }
    }
}
