/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Objects;

/// <summary>
/// This class patches <code>StardewValley.Debris</code>.
/// </summary>

internal static class MagicalDebris {
    internal static void PatchObject(Harmony harmony) {
        harmony.Patch(
            original: AccessTools.Method(
                typeof(Debris),
                nameof(Debris.collect),
                new[] {
                    typeof(Farmer),
                    typeof(Chunk),
                }),
            prefix: new HarmonyMethod(typeof(MagicalDebris), nameof(Collect))
        );
    }

    private static bool Collect(Debris __instance, ref bool __result, Farmer farmer, Chunk? chunk = null) {
        if (__instance.chunkType.Value == -MagicalObject.ObjectId) {
            __result = farmer.addItemToInventoryBool(new SObject(Vector2.Zero, MagicalObject.ObjectId));
            return false;
        }
        return true;
    }
}