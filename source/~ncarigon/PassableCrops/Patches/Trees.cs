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
    internal static class Trees {
        private static ModEntry? Mod;

        public static void Register(ModEntry mod) {
            Mod = mod;

            var harmony = new Harmony(Mod?.ModManifest?.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.isPassable), new Type[] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(Trees), nameof(Postfix_Tree_isPassable))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(Trees), nameof(Prefix_Tree_draw))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.getBoundingBox)),
                postfix: new HarmonyMethod(typeof(Trees), nameof(Postfix_Tree_getBoundingBox))
            );
        }

        private static bool AnyPassable(Tree tree) {
            return Mod?.Config is not null && !(tree?.stump.Value ?? true) && Mod.Config.PassableTreeGrowth >= (tree?.growthStage.Value ?? 0);
        }

        private static void Postfix_Tree_isPassable(
            Tree __instance,
            ref bool __result, ref float ___maxShake, ref NetBool ___shakeLeft,
            Character c
        ) {
            try {
                if (AnyPassable(__instance)) {
                    var farmer = c as Farmer;
                    if (farmer is not null || (Mod?.Config?.PassableByAll ?? false)) {
                        __result = true;
                        if (farmer is not null && (Mod?.Config?.SlowDownWhenPassing ?? false)) {
                            farmer.temporarySpeedBuff = farmer.stats.Get("Book_Grass") == 0 ? -1f : -0.33f;
                        }
                        if ((Mod?.Config?.ShakeWhenPassing ?? true) && c is not null && ___maxShake == 0f && __instance.growthStage.Value > 0) {
                            ___shakeLeft.Value = c.StandingPixel.X > (__instance.Tile.X + 0.5f) * 64f || (c.Tile.X == __instance.Tile.X && Game1.random.NextBool());
                            ___maxShake = (float)(Math.PI / 64.0);
                            Mod?.PlayRustleSound(__instance.Tile, __instance.Location);
                        }
                    }
                }
            } catch { }
        }

        private static bool isDrawing = false;

        private static void Prefix_Tree_draw(
            Tree __instance
        ) {
            if (AnyPassable(__instance)) {
                isDrawing = true;
            }
        }

        private static void Postfix_Tree_getBoundingBox(
            Tree __instance,
            ref Rectangle __result
        ) {
            if (isDrawing) {
                isDrawing = false;
                var skew = __instance.growthStage.Value switch {
                    0 => -46,
                    1 => -46,
                    2 => -34,
                    3 => -30,
                    4 => -30,
                    _ => 0
                };
                __result = new Rectangle(__result.X, __result.Y + skew, __result.Width, __result.Height);
            }
        }
    }
}

