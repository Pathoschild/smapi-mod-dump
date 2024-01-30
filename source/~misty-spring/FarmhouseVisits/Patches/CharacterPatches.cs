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
using StardewValley;

namespace FarmVisitors;

internal class CharacterPatches
{
    internal static void Apply(Harmony harmony)
    {
        ModEntry.Log($"Applying Harmony patch \"{nameof(CharacterPatches)}\": postfixing SDV method \"Character.shouldCollideWithBuildingLayer(Gamelocation)\".");
        harmony.Patch(
          original: AccessTools.Method(typeof(Character), nameof(Character.shouldCollideWithBuildingLayer)),
          postfix: new HarmonyMethod(typeof(CharacterPatches), nameof(Post_shouldCollideWithBuildingLayer))
        );
    }

    private static void Post_shouldCollideWithBuildingLayer(ref Character __instance, GameLocation location, ref bool __result)
    {
        if (location.Name != "Farm")
            return;

        if (!ModEntry.HasAnyVisitors)
            return;

        if (ModEntry.Visitor == null)
            return;

        if (__instance.Name != ModEntry.Visitor.Name)
            return;

        __result = true;
    }
}
