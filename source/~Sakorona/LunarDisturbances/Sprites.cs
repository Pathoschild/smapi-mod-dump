using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using System;
using System.IO;

namespace TwilightShards.LunarDisturbances
{
    public class Sprites
    {
        /// <summary>Sprites used to draw a letter.</summary>
        public static class Letter
        {
            /// <summary>The sprite sheet containing the letter sprites.</summary>
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");

            /// <summary>The letter background (including edges and corners).</summary>
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);
        }

        /// <summary> Sprites used for drawing various weather stuff </summary>
        public class Icons
        {
            public Texture2D MoonSource;
            public static Texture2D source2;

            public Icons(IContentHelper helper)
            {
                MoonSource = helper.Load<Texture2D>(Path.Combine("assets", "MoonPhases.png"));
                source2 = Game1.mouseCursors;
            }

            public static Rectangle GetNightMoonSprite(MoonPhase currPhase)
            {
                switch (currPhase)
                {
                    case MoonPhase.BloodMoon:
                        return Icons.BloodMoon;
                    case MoonPhase.NewMoon:
                        return Icons.NewMoon;
                    case MoonPhase.WaxingCrescent:
                        return Icons.WaxingCrescent2;
                    case MoonPhase.FirstQuarter:
                        return Icons.FirstQuarter;
                    case MoonPhase.WaxingGibbeous:
                        return Icons.WaxingGibbeous;
                    case MoonPhase.FullMoon:
                        return Icons.FullMoon;
                    case MoonPhase.WaningGibbeous:
                        return Icons.WaningGibbeous;
                    case MoonPhase.ThirdQuarter:
                        return Icons.ThirdQuarter;
                    case MoonPhase.WaningCrescent:
                        return Icons.WaningCrescent2;
                }

                return Icons.NewMoon;
            }

            public static Rectangle GetMoonSprite(MoonPhase moon)
            {
                if (moon == MoonPhase.FirstQuarter)
                    return Icons.FirstQuarter;
                if (moon == MoonPhase.FullMoon)
                    return Icons.FullMoon;
                if (moon == MoonPhase.NewMoon)
                    return Icons.NewMoon;
                if (moon == MoonPhase.ThirdQuarter)
                    return Icons.ThirdQuarter;
                if (moon == MoonPhase.WaningCrescent)
                    return Icons.WaningCrescent1;
                if (moon == MoonPhase.WaxingCrescent)
                    return Icons.WaxingCrescent1;
                if (moon == MoonPhase.WaningGibbeous)
                    return Icons.WaningGibbeous;                    
                if (moon == MoonPhase.WaxingGibbeous)
                    return Icons.WaxingGibbeous;

                return Icons.NewMoon;
            }

            // These are the positions of each sprite on their sheet.
            public static readonly Rectangle NewMoon = new Rectangle(7, 23, 34, 36);
            public static readonly Rectangle WaxingCrescent1 = new Rectangle(53, 22, 38, 38);
            public static readonly Rectangle WaxingCrescent2 = new Rectangle(101, 20, 38, 40);
            public static readonly Rectangle WaxingCrescent3 = new Rectangle(151, 23, 35, 38);
            public static readonly Rectangle FirstQuarter = new Rectangle(198, 21, 38, 41);
            public static readonly Rectangle FullMoon = new Rectangle(5, 86, 37, 38);
            public static readonly Rectangle ThirdQuarter = new Rectangle(54, 86, 36, 38);
            public static readonly Rectangle WaningCrescent1 = new Rectangle(104, 89, 32, 34);
            public static readonly Rectangle WaningCrescent2 = new Rectangle(149, 87, 38, 37);
            public static readonly Rectangle WaningCrescent3 = new Rectangle(208, 89, 33, 35);
            public static readonly Rectangle WaxingGibbeous = new Rectangle(262, 91, 34, 33);
            public static readonly Rectangle WaningGibbeous = new Rectangle(257, 22, 36, 39);
            public static readonly Rectangle BloodMoonIntensifies = new Rectangle(312, 30, 41, 39);
            public static readonly Rectangle BloodMoon = new Rectangle(317, 90, 37, 37);         
        }

        public static Texture2D Pixel => LazyPixel.Value;

        private static readonly Lazy<Texture2D> LazyPixel = new Lazy<Texture2D>(() =>
        {
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });
    }
}
