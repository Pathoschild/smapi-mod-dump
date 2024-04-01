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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BushBloomMod.Patches {
    internal static class Bushes {
        public static IMonitor Monitor;
        private static Config Config;

        public static void Register(IModHelper helper, IMonitor monitor, Config config) {
            Monitor = monitor;
            Config = config;
            var harmony = new Harmony(helper.ModContent.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "dayUpdate"),
                prefix: new HarmonyMethod(typeof(Bushes), nameof(Prefix_Bush_dayUpdate)),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_dayUpdate))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "seasonUpdate"),
                prefix: new HarmonyMethod(typeof(Bushes), nameof(Prefix_Bush_seasonUpdate)),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_seasonUpdate))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "inBloom"),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_inBloom))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "GetShakeOffItem"),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_GetShakeOffItem))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "setUpSourceRect"),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_setUpSourceRect))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "draw", new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(Bushes), nameof(Postfix_Bush_draw))
            );
        }

        // track custom texures associated with each bush
        private static readonly Dictionary<Bush, Texture2D> CustomTextures = new();

        // our custom winter berry
        private static readonly Texture2D WinterBerryBush;

        static Bushes() {
            try {
                WinterBerryBush = Texture2D.FromFile(Game1.graphics.GraphicsDevice, Path.Combine(new FileInfo(Assembly.GetAssembly(typeof(Bushes)).Location).Directory.FullName, "assets", "winter.png"));
            } catch {
                Monitor.Log($"Failed to load texture: {Path.Combine("assets", "winter.png")}", LogLevel.Error);
            }
        }

        private static void Prefix_Bush_dayUpdate(
            Bush __instance
        ) {
            if (__instance.IsAbleToBloom()) {
                // this sets up the schedule for the bush for the current day
                __instance.IsBloomingToday();
            }
        }

        private static void Postfix_Bush_dayUpdate(
            Bush __instance
        ) {
            if (__instance.IsAbleToBloom()) {
                // use our blooming logic to setup bush texture
                __instance.tileSheetOffset.Value = __instance.HasBloomedToday() ? 1 : 0;
                __instance.setUpSourceRect();
            }
        }

        private static void Prefix_Bush_seasonUpdate(
            Bush __instance,
            ref int __state
        ) {
            if (__instance.IsAbleToBloom()) {
                // save offset to prevent overwrite during season change
                __state = __instance.tileSheetOffset.Value;
            }
        }

        private static void Postfix_Bush_seasonUpdate(
            Bush __instance,
            int __state
        ) {
            if (__instance.IsAbleToBloom()) {
                // restore offset to allow custom blooming logic to work
                __instance.tileSheetOffset.Value = __state;
            }
        }

        private static void Postfix_Bush_inBloom(
            Bush __instance,
            ref bool __result
        ) {
            if (__instance.IsAbleToBloom()) {
                // use our own blooming logic
                __result = __instance.HasBloomedToday();
            }
        }

        private static void Postfix_Bush_GetShakeOffItem(
            Bush __instance,
            ref string __result
        ) {
            if (__instance.IsAbleToBloom()) {
                // overwrite with our item shake logic
                __result = __instance.GetShakeOffId();
                // clear schedule after shaking
                Schedule.SetSchedule(__instance, null);
            }
        }

        private static void Postfix_Bush_setUpSourceRect(
            Bush __instance
        ) {
            if (__instance.IsAbleToBloom()) {
                var season = ((!__instance.IsSheltered()) ? __instance.Location.GetSeason() : Season.Spring);
                var sheetOffset = __instance.tileSheetOffset.Value;
                if (__instance.tileSheetOffset.Value == 1) {
                    // if blooming, cache any custom blooming texture
                    var t = Schedule.GetExistingSchedule(__instance)?.Texture
                        // use our default winter berry if no other is specified
                        ?? (season == Season.Winter && Config.UseCustomWinterBerrySprite ? WinterBerryBush : null);
                    // switch to non-blooming texture if using a custom texture
                    if (t != null) {
                        sheetOffset = 0;
                    }
                    CustomTextures[__instance] = t;
                }
                // switch summer texture to spring, if configured
                if (season == Season.Summer && Config.UseSpringBushForSummer) {
                    season = Season.Spring;
                }
                var xOffset = (int)season * 16 * 4 + sheetOffset * 16 * 2;
                __instance.sourceRect.Value = new Rectangle(xOffset % Bush.texture.Value.Bounds.Width, xOffset / Bush.texture.Value.Bounds.Width * 3 * 16, 32, 48);
            }
        }

        private static void Postfix_Bush_draw(
            Bush __instance,
            SpriteBatch spriteBatch,
            float ___yDrawOffset, float ___shakeRotation
        ) {
            if (__instance.IsAbleToBloom()) {
                // if blooming and using a custom texture
                if (__instance.tileSheetOffset.Value == 1 && CustomTextures.TryGetValue(__instance, out var texture) && texture != null) {
                    spriteBatch.Draw(
                        texture: texture,
                        position: Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.Tile.X * 64f + 64, (__instance.Tile.Y + 1f) * 64f - 64 + ___yDrawOffset)),
                        sourceRectangle: new(0, 0, 32, 48),
                        color: Color.White,
                        rotation: ___shakeRotation,
                        origin: new Vector2(16, 32f),
                        scale: 4f,
                        effects: __instance.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        // make sure to add small extra here so the sprite doesn't flicker
                        layerDepth: (__instance.getBoundingBox().Center.Y + 48) / 10000f - __instance.Tile.X / 1000000f + 0.0001f
                    );
                }
            }
        }
    }
}
