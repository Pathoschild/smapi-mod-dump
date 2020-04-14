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
                        return Icons.BloodMoonIntensifies;
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
                    case MoonPhase.BlueMoon:
                        return Icons.BlueMoon;
                    case MoonPhase.HarvestMoon:
                        return Icons.HarvestMoon;
                    case MoonPhase.SpiritsMoon:
                            return Icons.SpiritsEve;
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
                if (moon == MoonPhase.BlueMoon)
                    return Icons.BlueMoon;
                if (moon == MoonPhase.HarvestMoon)
                    return Icons.HarvestMoon;
                if (moon == MoonPhase.SpiritsMoon)
                    return Icons.SpiritsEve;

                return Icons.NewMoon;
            }

            // These are the positions of each sprite on their sheet.
            public static readonly Rectangle NewMoon = new Rectangle(0, 0, 40, 40);
            public static readonly Rectangle WaxingCrescent1 = new Rectangle(40, 0, 40, 40);
            public static readonly Rectangle WaxingCrescent2 = new Rectangle(80, 0, 40, 40);
            public static readonly Rectangle WaxingCrescent3 = new Rectangle(120, 0, 40, 40);
            public static readonly Rectangle FirstQuarter = new Rectangle(160, 0, 40, 40);
            public static readonly Rectangle WaxingGibbeous = new Rectangle(200, 0, 40, 40);
            public static readonly Rectangle FullMoon = new Rectangle(240, 0, 40, 40);
            public static readonly Rectangle BloodMoonIntensifies = new Rectangle(280, 0, 40, 40);
            public static readonly Rectangle WaningGibbeous = new Rectangle(320, 0, 40, 40);
            public static readonly Rectangle WaningGibbeous2 = new Rectangle(360, 0, 40, 40);
            public static readonly Rectangle ThirdQuarter = new Rectangle(0, 40, 40, 40);
            public static readonly Rectangle WaningCrescent1 = new Rectangle(40, 40, 40, 40);
            public static readonly Rectangle WaningCrescent2 = new Rectangle(80, 40, 40, 40);
            public static readonly Rectangle WaningCrescent3 = new Rectangle(120, 40, 40, 40);
            public static readonly Rectangle BloodMoon = new Rectangle(160, 40, 40, 40);
            public static readonly Rectangle BlueMoon = new Rectangle(200, 40, 40, 40);
            public static readonly Rectangle HarvestMoon = new Rectangle(240, 40, 40, 40);
            public static readonly Rectangle SpiritsEve = new Rectangle(280, 40, 40, 40);
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
