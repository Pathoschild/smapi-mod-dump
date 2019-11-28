using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ClimatesOfFerngillRebuild.Patches
{
    public static class WeatherDebrisPatches
    {
        public const int SingleLeafA = 0;
        public const int SingleLeafB = 1;
        public const int DoubleLeafA = 2;
        public const int RareSeason = 3;

#pragma warning disable IDE0051 // Remove unused private members
        static void CtorPostfix(WeatherDebris __instance)
#pragma warning restore IDE0051 // Remove unused private members
        {
            double prob = ClimatesOfFerngill.Dice.NextDouble();
            int which;
            if (prob < .6)
                which = SingleLeafA;
            else if (prob >= .6 && prob < .8)
                which = SingleLeafB;
            else if (prob >= .8 && prob < .9)
                which = DoubleLeafA;
            else
                which = RareSeason;

            int offset = 0;
            switch (which)
            {
                case SingleLeafA:
                    if (Game1.currentSeason == "spring") offset = 0;
                    if (Game1.currentSeason == "summer") offset = 80;
                    if (Game1.currentSeason == "fall") offset = 112;
                    if (Game1.currentSeason == "winter") offset = 176;
                    break;
                case SingleLeafB:
                    if (Game1.currentSeason == "spring") offset = 16;
                    if (Game1.currentSeason == "summer") offset = 96;
                    if (Game1.currentSeason == "fall") offset = 128;
                    if (Game1.currentSeason == "winter") offset = 192;
                    break;
                case DoubleLeafA:
                    if (Game1.currentSeason == "spring") offset = 32;
                    if (Game1.currentSeason == "summer") offset = 64;
                    if (Game1.currentSeason == "fall") offset = 144;
                    if (Game1.currentSeason == "winter") offset = 208;
                    break;
                case RareSeason:
                    if (Game1.currentSeason == "spring") offset = 48;
                    if (Game1.currentSeason == "summer") offset = 224;
                    if (Game1.currentSeason == "fall") offset = 48;
                    if (Game1.currentSeason == "winter") offset = 160;
                    break;
                  default:
                    offset = 0;
                    break;
            } 
            Rectangle sourceRect = new Rectangle(0, offset, 16, 16);
            ClimatesOfFerngill.Reflection.GetField<Rectangle>(__instance, "sourceRect").SetValue(sourceRect);
        }

#pragma warning disable IDE0051 // Remove unused private members
        static bool UpdatePrefix(WeatherDebris __instance, bool slow, ref Rectangle ___sourceRect, ref bool ___blowing)
#pragma warning restore IDE0051 // Remove unused private members

        {
            __instance.position.X += __instance.dx + (slow ? 0.0f : WeatherDebris.globalWind);
            __instance.position.Y += __instance.dy - (slow ? 0.0f : -0.5f);
            if (__instance.dy < 0.0 && !___blowing)
                __instance.dy += 0.01f;
            if (!Game1.fadeToBlack && Game1.fadeToBlackAlpha <= 0.0)
            {
                if (__instance.position.X < -80.0)
                {
                    __instance.position.X = Game1.viewport.Width;
                    __instance.position.Y = Game1.random.Next(0, Game1.viewport.Height - 64);
                }
                if ((double)__instance.position.Y > (Game1.viewport.Height + 16))
                {
                    __instance.position.X = Game1.random.Next(0, Game1.viewport.Width);
                    __instance.position.Y = -64f;
                    __instance.dy = Game1.random.Next(-15, 10) / (slow ? (Game1.random.NextDouble() < 0.1 ? 5f : 200f) : 50f);
                    __instance.dx = Game1.random.Next(-10, 0) / (slow ? 200f : 50f);
                }
                else if (__instance.position.Y < -64.0)
                {
                    __instance.position.Y = Game1.viewport.Height;
                    __instance.position.X = Game1.random.Next(0, Game1.viewport.Width);
                }
            }
            if (___blowing)
            {
                __instance.dy -= 0.01f;
                if (Game1.random.NextDouble() < 0.006 || __instance.dy < -2.0)
                    ___blowing = false;
                
            }
            else if (!slow && Game1.random.NextDouble() < 0.001 && Game1.currentSeason != null && (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")))
               ___blowing = true;
            switch (__instance.which)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    __instance.animationTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                    if (__instance.animationTimer > 0)
                        break;
                    __instance.animationTimer = 100 + __instance.animationIntervalOffset;
                    __instance.animationIndex += __instance.animationDirection;
                    if (__instance.animationDirection == 0)
                        __instance.animationDirection = __instance.animationIndex < 9 ? 1 : -1;
                    if (__instance.animationIndex > 10)
                    {
                        if (Game1.random.NextDouble() < 0.82)
                        {
                            --__instance.animationIndex;
                            __instance.animationDirection = 0;
                            __instance.dx += 0.1f;
                            __instance.dy -= 0.2f;
                        }
                        else
                            __instance.animationIndex = 0;
                    }
                    else if (__instance.animationIndex == 4 && __instance.animationDirection == -1)
                    {
                        ++__instance.animationIndex;
                        __instance.animationDirection = 0;
                        __instance.dx -= 0.1f;
                        __instance.dy -= 0.1f;
                    }
                    if (__instance.animationIndex == 7 && __instance.animationDirection == -1)
                        __instance.dy -= 0.2f;
                    if (__instance.which == 3)
                        break;
                    ___sourceRect.X = 0 + __instance.animationIndex * 16;
                    break;
            }

            return false;
        }

#pragma warning disable IDE0051 // Remove unused private members
        static bool DrawPrefix(SpriteBatch b, WeatherDebris __instance, Rectangle ___sourceRect)
#pragma warning restore IDE0051 // Remove unused private members
        {
            b.Draw(ClimatesOfFerngill.OurIcons.LeafSprites, __instance.position, new Rectangle?(___sourceRect), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1E-06f);
            return false;
        }
    }
}
