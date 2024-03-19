/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/


#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

using HarmonyLib;
using StardewValley;

namespace Calcifer.Features;

[HarmonyPatch]
class NPCSwimmingPatches
{
    [HarmonyPatch(typeof(NPC), "finishRouteBehavior")]
    [HarmonyPostfix]
    public static void finishRouteBehavior_Postfix(NPC __instance, string behaviorName)
    {
        if (behaviorName == $"{__instance.Name}_startSwimming")
        {
            __instance.swimming.Value = true;
        }
    }

    [HarmonyPatch(typeof(NPC), "startRouteBehavior")]
    [HarmonyPostfix]
    public static void startRouteBehavior_Postfix(NPC __instance, string behaviorName)
    {
        if (behaviorName == $"{__instance.Name}_stopSwimming")
        {
            __instance.swimming.Value = false;
        }
    }
}

#pragma warning restore IDE1006 // Naming Styles
