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

    internal class HorseDrawPatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               prefix: new HarmonyMethod(typeof(HorseDrawPatches), nameof(PreventBaseEmoteDraw)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(HorseDrawPatches), nameof(DrawEmoteAndSaddleBags)));
        }

        public static void PreventBaseEmoteDraw(Horse __instance, ref bool __state)
        {
            __state = false;

            if (mod.Config.FixHorseEmotePosition)
            {
                __state = __instance.IsEmoting;
                __instance.IsEmoting = false;
            }
        }

        public static void DrawEmoteAndSaddleBags(Horse __instance, SpriteBatch b, ref bool __state)
        {
            if (mod.Config.FixHorseEmotePosition)
            {
                __instance.IsEmoting = __state;
            }

            DrawEmote(__instance, b);

            if (__instance.IsTractor())
            {
                return;
            }

            DrawSaddleBags(__instance, b);
        }

        private static void DrawEmote(Horse horse, SpriteBatch b)
        {
            if (!horse.IsEmoting || !mod.Config.FixHorseEmotePosition)
            {
                return;
            }

            Vector2 emotePosition = horse.getLocalPosition(Game1.viewport);

            emotePosition.X += mod.Config.ThinHorse ? 0f : 32f;
            emotePosition.Y -= 96f;

            // draw one layer above the usual sprite of the horse so there is no z-fighting
            float layer = horse.StandingPixel.Y + 1;

            // draw on top of the player instead of below them, uses the same value as the head of the horse
            if (horse.rider != null)
            {
                layer = horse.Position.Y + 64f;
            }

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

            b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle?(new Rectangle((horse.CurrentEmoteIndex * 16) % Game1.emoteSpriteSheet.Width, horse.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, layer / 10000f);
        }

        private static void DrawSaddleBags(Horse horse, SpriteBatch b)
        {
            if (!mod.Config.SaddleBag || mod.Config.VisibleSaddleBags == SaddleBagOption.Disabled.ToString())
            {
                return;
            }

            if (!SaddleBagAccess.HasAccessToSaddleBag(horse))
            {
                return;
            }

            float xOffset = mod.Config.ThinHorse ? -32f : 0f;
            float yOffset = -80f;

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
    }
}