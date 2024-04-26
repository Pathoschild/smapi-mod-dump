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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley;
using SObject = StardewValley.Object;
using StardewValley.Extensions;
using StardewValley.Monsters;

namespace PassableCrops.Patches {
    internal static class Objects {
        private static string KeyDataShake = null!;

        private static ModEntry? Mod;

        public static void Register(ModEntry mod) {
            Mod = mod;

            KeyDataShake = $"{Mod?.ModManifest?.UniqueID}/shake";

            var harmony = new Harmony(Mod?.ModManifest?.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "isPassable"),
                postfix: new HarmonyMethod(typeof(Objects), nameof(Postfix_Object_isPassable))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Character), "MovePosition"),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_Character_MovePosition)),
                postfix: new HarmonyMethod(typeof(Objects), nameof(Postfix_Character_MovePosition))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "MovePosition"),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_Farmer_MovePosition)),
                postfix: new HarmonyMethod(typeof(Objects), nameof(Postfix_Character_MovePosition))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Monster), "MovePosition"),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_Monster_MovePosition)),
                postfix: new HarmonyMethod(typeof(Objects), nameof(Postfix_Character_MovePosition))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "draw", new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_Object_draw)),
                postfix: new HarmonyMethod(typeof(Objects), nameof(Postfix_Object_draw))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Rectangle), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_SpriteBatch_Draw1))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SpriteBatch), "Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(Vector2), typeof(SpriteEffects), typeof(float) }),
                prefix: new HarmonyMethod(typeof(Objects), nameof(Prefix_SpriteBatch_Draw2))
            );
        }

        private const float shakeRate = (float)Math.PI / 100f;
        private const float maxShake_normal = (float)Math.PI / 12f;
        private const float maxShake_stiff = (float)Math.PI / 16f;

        private enum ObjType {
            None = 0,
            Scarecrow = 1,
            Sprinkler = 2,
            Forage = 3,
            Weed = 4
        }

        private static ObjType GetObjType(SObject o) {
            if ((Mod?.Config?.PassableSprinklers ?? false) && (o?.IsSprinkler() ?? false))
                return ObjType.Sprinkler;
            if ((Mod?.Config?.PassableScarecrows ?? false) && (o?.IsScarecrow() ?? false))
                return ObjType.Scarecrow;
            if ((Mod?.Config?.PassableForage ?? false) && (o?.isForage() ?? false) && (o?.Category != -9) && (o?.ParentSheetIndex != 590))
                return ObjType.Forage;
            if ((Mod?.Config?.PassableWeeds ?? false)
                && (o?.GetContextTags()?.Any(c => c.Contains("item_weeds") || c.Contains("item_greenrainweeds")) ?? false)
                && !(new int[] { 319,320,321 }.Any(c => c == (o?.ParentSheetIndex ?? 0))) //Ice Crystals are labeled as weeds so ignore them
            ) {
                return ObjType.Weed;
            }
            return ObjType.None;
        }

        private struct TempShakeData {
            public bool Passable;
            public ObjType ObjType;
            public float MaxShake, ShakeRotation;
            public bool ShakeLeft;
            public Character Character;
        }

        private static TempShakeData LastShakeData;

        private static bool AnyPassable() =>
            Mod?.Config is not null
            && (Mod.Config.PassableScarecrows || Mod.Config.PassableSprinklers || Mod.Config.PassableForage || Mod.Config.PassableWeeds);

        private static void Prefix_Character_MovePosition(
            Character __instance
        ) {
            LastShakeData.Character = __instance;
        }

        private static void Prefix_Farmer_MovePosition(
            Farmer __instance
        ) {
            LastShakeData.Character = __instance;
        }

        private static void Prefix_Monster_MovePosition(
            Monster __instance
        ) {
            LastShakeData.Character = __instance;
        }

        private static void Postfix_Character_MovePosition() {
            LastShakeData.Character = null!;
        }

        private static void Postfix_Object_isPassable(
            SObject __instance,
            ref bool __result
        ) {
            if (!AnyPassable())
                return;
            var ot = GetObjType(__instance);
            if (ot != ObjType.None) {
                var c = LastShakeData.Character ?? Game1.player;
                var bb = c.GetBoundingBox();
                var ib = __instance.GetBoundingBoxAt((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y);
                if (Mod?.Config?.PassableByAll ?? false) {
                    __result = true;
                } else {
                    // slightly larger box to allow intersection before actually touching it
                    var eb = bb.Clone();
                    eb.Inflate(16, 16);
                    __result = eb.Intersects(ib);
                }
                LastShakeData.Passable = __result;
                if (bb.Intersects(ib)) {
                    if ((Mod?.Config?.SlowDownWhenPassing ?? false) && c == Game1.player) {
                        Game1.player.temporarySpeedBuff = Game1.player.stats.Get("Book_Grass") == 0 ? -1f : -0.33f;
                    }
                    if (LastShakeData.Character is not null // only trigger if something moved
                        && (!__instance!.modData.TryGetValue(KeyDataShake, out var data)
                        || !float.TryParse((data ?? "").Split(';')[0], out var maxShake)
                        || maxShake <= 0f)
                    ) {
                        maxShake = ot == ObjType.Scarecrow || ot == ObjType.Sprinkler ? maxShake_stiff : maxShake_normal;
                        var shakeLeft = bb.Center.X > __instance.TileLocation.X * 64f + 32f;
                        var shakeRotation = 0f;
                        __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                        Mod?.PlayRustleSound(__instance.TileLocation, __instance.Location);
                    }
                }
            }
        }

        private static void Prefix_Object_draw(
            SObject __instance
        ) {
            if (!(Mod?.Config?.UseCustomDrawing ?? false) || !AnyPassable()) {
                return;
            }
            var ot = GetObjType(__instance);
            if (ot != ObjType.None
                && __instance!.modData.TryGetValue(KeyDataShake, out var data)
            ) {
                var s = (data ?? "").Split(';');
                if (s.Length == 3
                    && float.TryParse(s[0], out var maxShake)
                    && float.TryParse(s[1], out var shakeRotation)
                    && bool.TryParse(s[2], out var shakeLeft)) {
                    // calc new shake data
                    if (maxShake > 0f) {
                        if (shakeLeft) {
                            shakeRotation -= shakeRate;
                            if (Math.Abs(shakeRotation) >= maxShake) {
                                shakeLeft = false;
                            }
                        } else {
                            shakeRotation += shakeRate;
                            if (shakeRotation >= maxShake) {
                                shakeLeft = true;
                                shakeRotation -= shakeRate;
                            }
                        }
                        maxShake = Math.Max(0f, maxShake - (float)Math.PI / 300f);
                    } else {
                        shakeRotation /= 2f;
                        if (shakeRotation <= 0.01f) {
                            shakeRotation = 0f;
                        }
                    }
                    // update tracking values
                    __instance.modData[KeyDataShake] = $"{maxShake};{shakeRotation};{shakeLeft}";
                    LastShakeData.ObjType = ot;
                    LastShakeData.MaxShake = maxShake;
                    LastShakeData.ShakeRotation = shakeRotation;
                    LastShakeData.ShakeLeft = shakeLeft;
                }
            }
        }

        private static void Postfix_Object_draw() => LastShakeData.ObjType = ObjType.None;

        private static void MoveRotation(ref Rectangle destinationRectangle, ref Vector2 origin, Vector2 move) {
            destinationRectangle = new Rectangle(destinationRectangle.Location.X + (int)move.X * 4, destinationRectangle.Location.Y + (int)move.Y * 4, destinationRectangle.Width, destinationRectangle.Height);
            origin += move;
        }

        private static void MoveRotation(ref Vector2 position, ref Vector2 origin, Vector2 move) {
            position += move * 4;
            origin += move;
        }

        private static void Prefix_SpriteBatch_Draw1(
            ref Rectangle destinationRectangle, ref float rotation, ref Vector2 origin
        ) {
            if (LastShakeData.ObjType != ObjType.None) {
                if ((Mod?.Config?.ShakeWhenPassing ?? true)) {
                    rotation = LastShakeData.ShakeRotation;
                }
                switch (LastShakeData.ObjType) {
                    case ObjType.Scarecrow:
                        // move rotation to bottom of post
                        MoveRotation(ref destinationRectangle, ref origin, new Vector2(8f, 30f));
                        break;
                }
            }
        }

        private static void Prefix_SpriteBatch_Draw2(
            ref Vector2 position, ref float rotation, ref Vector2 origin, ref float layerDepth
        ) {
            if (LastShakeData.ObjType != ObjType.None) {
                if ((Mod?.Config?.ShakeWhenPassing ?? true)) {
                    rotation = LastShakeData.ShakeRotation;
                }
                switch (LastShakeData.ObjType) {
                    case ObjType.Weed:
                        layerDepth += (LastShakeData.Passable ? 24 : -40) / 10000f;
                        // move rotation near bottom
                        MoveRotation(ref position, ref origin, new Vector2(0f, 8f));
                        break;
                    case ObjType.Sprinkler:
                        layerDepth += (LastShakeData.Passable ? 45 : -19) / 10000f;
                        // rotation stays centered
                        break;
                    case ObjType.Forage:
                        layerDepth += (LastShakeData.Passable ? 32 : -32) / 10000f;
                        // move rotation near bottom
                        MoveRotation(ref position, ref origin, new Vector2(0f, 8f));
                        break;
                }
            }
        }
    }
}
