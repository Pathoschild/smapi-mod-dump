/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.Characters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    internal class ThinHorsePatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
                   original: AccessTools.Method(typeof(Character), nameof(Character.GetSpriteWidthForPositioning)),
                   prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(SetOneTileWide)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.showRiding)),
               prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixRidingPosition)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.squeezeForGate)),
               prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(DoNothing)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(PreventBaseEmoteDraw)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(DrawEmoteAndSaddleBags)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               transpiler: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixHeadAndHatPosition)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.update), new Type[] { typeof(GameTime), typeof(GameLocation) }),
               prefix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(DoMountingAnimation)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "setMount"),
                postfix: new HarmonyMethod(typeof(ThinHorsePatches), nameof(FixSetMount)));
        }

        // transpiler checked for 1.6
        public static IEnumerable<CodeInstruction> FixHeadAndHatPosition(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                bool foundHead = false;
                bool foundHat = false;

                for (int i = 0; i < instructionsList.Count; i++)
                {
                    if (!foundHead)
                    {
                        // exact 48f check does not work reliably
                        if (instructionsList[i].opcode == OpCodes.Ldc_R4
                            && (float)instructionsList[i].operand >= 47.9f
                            && (float)instructionsList[i].operand <= 48.1f)
                        {
                            var info = typeof(ThinHorsePatches).GetMethod(nameof(GetHorseHeadXPosition));
                            var oldLables = instructionsList[i].labels;
                            instructionsList[i] = new CodeInstruction(OpCodes.Call, info)
                            {
                                labels = oldLables
                            };

                            foundHead = true;
                        }
                    }

                    if (!foundHat)
                    {
                        if (i + 2 < instructionsList.Count
                            && instructionsList[i].opcode == OpCodes.Call
                            && instructionsList[i].operand.ToString().ToLower().Contains("get_zero")
                            && instructionsList[i + 1].opcode == OpCodes.Stloc_1
                            && instructionsList[i + 2].opcode == OpCodes.Ldarg_0)
                        {
                            var info = typeof(ThinHorsePatches).GetMethod(nameof(GetHatVector));
                            var oldLables = instructionsList[i].labels;
                            instructionsList[i] = new CodeInstruction(OpCodes.Call, info)
                            {
                                labels = oldLables
                            };

                            foundHat = true;
                        }
                    }
                }

                return instructionsList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        public static bool PreventBaseEmoteDraw(ref Horse __instance, ref bool __state)
        {
            if (mod.Config.ThinHorse)
            {
                __state = __instance.IsEmoting;
                __instance.IsEmoting = false;
            }

            return true;
        }

        public static void DrawEmoteAndSaddleBags(ref Horse __instance, ref SpriteBatch b, ref bool __state)
        {
            if (__instance.IsTractor())
            {
                return;
            }

            Horse horse = __instance;

            if (mod.Config.SaddleBag && mod.Config.VisibleSaddleBags != SaddleBagOption.Disabled.ToString())
            {
                float yOffset = -80f;
                float xOffset = mod.Config.ThinHorse ? -32f : 0f;

                // all player sprites being off by 1 is really obvious if using horsemanship and facing north
                if (horse.FacingDirection == Game1.up && mod.IsUsingHorsemanship && mod.Config.ThinHorse)
                {
                    xOffset += 1;
                }

                // draw one layer above the usual sprite of the horse so there is no z-fighting
                float layer = horse.StandingPixel.Y + 1;

                // draw on top of the player instead of below them, uses the same value as the head of the horse
                if (horse.FacingDirection == Game1.up && horse.rider != null)
                {
                    layer = horse.Position.Y + 64f;
                }

                bool shouldFlip = horse.Sprite.CurrentAnimation != null && horse.Sprite.CurrentAnimation[horse.Sprite.currentAnimationIndex].flip;

                if (horse.FacingDirection == Game1.left)
                {
                    shouldFlip = true;
                }

                if (mod.SaddleBagOverlay != null)
                {
                    b.Draw(mod.SaddleBagOverlay, horse.getLocalPosition(Game1.viewport) + new Vector2(xOffset, yOffset), horse.Sprite.SourceRect, Color.White, 0f, Vector2.Zero, 4f, shouldFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer / 10000f);
                }
            }

            if (!mod.Config.ThinHorse)
            {
                return;
            }

            __instance.IsEmoting = __state;

            if (horse.IsEmoting)
            {
                Vector2 emotePosition = horse.getLocalPosition(Game1.viewport);

                emotePosition.Y -= 96f;

                switch (horse.FacingDirection)
                {
                    case Game1.up:
                        emotePosition.Y -= 40f;
                        break;

                    case Game1.right:
                        emotePosition.X += 40f;
                        emotePosition.Y -= 30f;
                        break;

                    case Game1.down:
                        emotePosition.Y += 5f;
                        break;

                    case Game1.left:
                        emotePosition.X -= 40f;
                        emotePosition.Y -= 30f;
                        break;

                    default:
                        break;
                }

                b.Draw(Game1.emoteSpriteSheet, emotePosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((horse.CurrentEmoteIndex * 16) % Game1.emoteSpriteSheet.Width, horse.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, horse.StandingPixel.Y / 10000f);
            }
        }

        public static bool DoNothing()
        {
            return !mod.Config.ThinHorse;
        }

        public static bool SetOneTileWide(Character __instance, ref int __result)
        {
            if (mod.Config.ThinHorse && __instance is Horse)
            {
                __result = 16;
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool FixRidingPosition(Farmer __instance)
        {
            if (!mod.Config.ThinHorse)
            {
                return true;
            }

            if (!__instance.isRidingHorse())
            {
                return false;
            }

            switch (__instance.FacingDirection)
            {
                case Game1.up:
                    __instance.FarmerSprite.setCurrentSingleFrame(113, 32000, false, false);
                    __instance.xOffset = 4f; // old: -6f, diff: +10
                    break;

                case Game1.right:
                    __instance.FarmerSprite.setCurrentSingleFrame(106, 32000, false, false);
                    __instance.xOffset = 7f; // old: -3f, diff: +10
                    break;

                case Game1.down:
                    __instance.FarmerSprite.setCurrentSingleFrame(107, 32000, false, false);
                    __instance.xOffset = 4f; // old: -6f, diff: +10
                    break;

                case Game1.left:
                    __instance.FarmerSprite.setCurrentSingleFrame(106, 32000, false, true);
                    __instance.xOffset = -2f; // old: -12f, diff: +10
                    break;
            }

            if (!__instance.isMoving())
            {
                __instance.yOffset = 0f;
                return false;
            }

            switch (__instance.mount.Sprite.currentAnimationIndex)
            {
                case 0:
                    __instance.yOffset = 0f;
                    return false;

                case 1:
                    __instance.yOffset = -4f;
                    return false;

                case 2:
                    __instance.yOffset = -4f;
                    return false;

                case 3:
                    __instance.yOffset = 0f;
                    return false;

                case 4:
                    __instance.yOffset = 4f;
                    return false;

                case 5:
                    __instance.yOffset = 4f;
                    return false;

                default:
                    return false;
            }
        }

        public static bool DoMountingAnimation(ref Horse __instance)
        {
            Horse horse = __instance;

            // all the vanilla conditions to get to the case in question
            if (!mod.Config.ThinHorse || horse.rider == null || horse.rider.mount != null || !horse.rider.IsLocalPlayer || !horse.mounting.Value || (horse.rider != null && horse.rider.hidden.Value))
            {
                return true;
            }

            if (horse.FacingDirection == Game1.left)
            {
                horse.rider.xOffset = 0f;
            }
            else
            {
                horse.rider.xOffset = 4f;
            }

            var positionDifference = horse.rider.Position.X - horse.Position.X;

            if (Math.Abs(positionDifference) < 4)
            {
                horse.rider.position.X = horse.Position.X;
            }
            else if (positionDifference < 0)
            {
                horse.rider.position.X += 4f;
            }
            else if (positionDifference > 0)
            {
                horse.rider.position.X -= 4f;
            }

            // invert whatever the overridden method will do
            if (horse.rider.Position.X < (horse.GetBoundingBox().X + 16 - 4))
            {
                horse.rider.position.X -= 4f;
            }
            else if (horse.rider.Position.X > (horse.GetBoundingBox().X + 16 + 4))
            {
                horse.rider.position.X += 4f;
            }

            return true;
        }

        public static void FixSetMount(ref Farmer __instance, ref Horse mount)
        {
            if (mod.Config.ThinHorse && mount != null)
            {
                __instance.xOffset = mount.FacingDirection switch
                {
                    Game1.right => -4f, // counteracts the +8 from the horse update method to arrive at +4
                    Game1.left => 0,
                    _ => 4f,
                };
            }
        }

        public static float GetHorseHeadXPosition()
        {
            return mod.Config.ThinHorse ? 16f : 48f;
        }

        private static readonly Vector2 thinHorseHatVector = new Vector2(-8f, 0f);

        public static Vector2 GetHatVector()
        {
            return mod.Config.ThinHorse ? thinHorseHatVector : Vector2.Zero;
        }
    }
}