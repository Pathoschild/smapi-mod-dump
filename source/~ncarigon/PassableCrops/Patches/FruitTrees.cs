/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;

namespace PassableCrops.Patches {
    internal static class FruitTrees {
        private static ModEntry? Mod;

        public static void Register(ModEntry mod) {
            Mod = mod;

            var harmony = new Harmony(Mod?.ModManifest?.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.isPassable), new Type[] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(FruitTrees), nameof(Postfix_FruitTree_isPassable))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(FruitTrees), nameof(Prefix_FruitTree_draw))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.getBoundingBox)),
                postfix: new HarmonyMethod(typeof(FruitTrees), nameof(Postfix_FruitTree_getBoundingBox))
            );
        }

        private static bool AnyPassable(FruitTree tree) {
            return Mod?.Config is not null && !(tree?.stump.Value ?? true) && Mod.Config.PassableFruitTreeGrowth >= (tree?.growthStage.Value ?? 0);
        }

        private static void Postfix_FruitTree_isPassable(
            FruitTree __instance,
            ref bool __result, ref float ___maxShake, ref NetBool ___shakeLeft,
            Character c
        ) {
            try {
                if (AnyPassable(__instance) && c is Farmer farmer) {
                    __result = true;
                    if (Mod?.Config?.SlowDownWhenPassing ?? false)
                        farmer.temporarySpeedBuff = farmer.stats.Get("Book_Grass") == 0 ? -1f : -0.33f;
                    if (___maxShake == 0f) {
                        ___shakeLeft.Value = Game1.player.StandingPixel.X > (__instance.Tile.X + 0.5f) * 64f || (Game1.player.Tile.X == __instance.Tile.X && Game1.random.NextBool());
                        ___maxShake = (float)(Math.PI / 64.0);
                    }
                }
            } catch { }
        }

        private static bool isDrawing = false;

        private static void Prefix_FruitTree_draw(
            FruitTree __instance
        ) {
            if (AnyPassable(__instance)) {
                isDrawing = true;
            }
        }

        private static void Postfix_FruitTree_getBoundingBox(
            FruitTree __instance,
            ref Rectangle __result
        ) {
            if (isDrawing) {
                isDrawing = false;
                var skew = __instance.growthStage.Value switch {
                    0 => -36,
                    _ => 0
                };
                __result = new Rectangle(__result.X, __result.Y + skew, __result.Width, __result.Height);
            }
        }
    }
}

