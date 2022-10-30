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
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable InconsistentNaming

namespace Slothsoft.Challenger.Objects;

/// <summary>
/// This class patches <code>StardewValley.Game1</code>.
/// </summary>
internal static class MagicalGame1 {
    internal static void PatchObject(Harmony harmony) {
        harmony.Patch(
            original: AccessTools.Method(
                typeof(Game1),
                nameof(Game1.getArbitrarySourceRect),
                new[] {
                    typeof(Texture2D),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                }),
            prefix: new HarmonyMethod(typeof(MagicalGame1), nameof(GetArbitrarySourceRect))
        );
    }

    private static bool GetArbitrarySourceRect(GameLocation __instance, ref Rectangle __result,
        Texture2D tileSheet, int tileWidth, int tileHeight, int tilePosition) {
        if (tilePosition == MagicalObject.ObjectId) {
            var magicalReplacement = ChallengerMod.Instance.GetApi()!.ActiveChallengeMagicalReplacement;
            __result = Game1.getArbitrarySourceRect(tileSheet, tileWidth, tileHeight, magicalReplacement.ParentSheetIndex);
            return false;
        }
        return true;
    }
}