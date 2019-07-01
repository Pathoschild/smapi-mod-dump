using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using System;
using System.IO;

namespace ClimatesOfFerngillRebuild
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
            public Texture2D WeatherSource;
            public Texture2D FogTexture;
            public Texture2D DarudeTexture;
            public static Texture2D source2;

            public Icons(IContentHelper helper)
            {
                WeatherSource = helper.Load<Texture2D>(Path.Combine("assets","WeatherIcons.png"));
                FogTexture = helper.Load<Texture2D>(Path.Combine("assets", "ThickerFog.png"));
                DarudeTexture = helper.Load<Texture2D>(Path.Combine("assets", "Sandstorm.png"));
                source2 = Game1.mouseCursors;
            }

            public Rectangle GetWeatherSprite(CurrentWeather condition)
            {
                if (condition.HasFlag(CurrentWeather.Blizzard))
                    return Icons.WeatherBlizzard;

                if (condition.HasFlag(CurrentWeather.Wind))
                    return Icons.WeatherWindy;

                if (condition.HasFlag(CurrentWeather.Festival))
                    return Icons.WeatherFestival;

                if (condition.HasFlag(CurrentWeather.Sunny) && !condition.HasFlag(CurrentWeather.Lightning))
                    return Icons.WeatherSunny;

                if (condition.HasFlag(CurrentWeather.Sunny) && condition.HasFlag(CurrentWeather.Lightning))
                    return Icons.WeatherDryLightning;

                if (condition.HasFlag(CurrentWeather.Wedding))
                    return Icons.WeatherWedding;

                if (condition.HasFlag(CurrentWeather.Snow) && !condition.HasFlag(CurrentWeather.Lightning))
                    return Icons.WeatherSnowy;

                if (condition.HasFlag(CurrentWeather.Snow) && condition.HasFlag(CurrentWeather.Lightning))
                    return Icons.WeatherThundersnow;

                if (condition.HasFlag(CurrentWeather.Rain))
                    return Icons.WeatherRainy;

                return Icons.WeatherSunny;
            }

           //Weather
            public static readonly Rectangle WeatherSunny = new Rectangle(1, 32, 39, 38);
            public static readonly Rectangle WeatherRainy = new Rectangle(40, 32, 35, 40);
            public static readonly Rectangle WeatherStormy = new Rectangle(77, 32, 39, 40);
            public static readonly Rectangle WeatherSnowy = new Rectangle(116, 32, 38, 41);
            public static readonly Rectangle WeatherWindy = new Rectangle(155, 30, 42, 44);
            public static readonly Rectangle WeatherWedding = new Rectangle(198, 32, 37, 38);
            public static readonly Rectangle WeatherFestival = new Rectangle(235, 32, 47, 45);
            public static readonly Rectangle WeatherBlizzard = new Rectangle(281, 32, 40, 42);
            public static readonly Rectangle WeatherDryLightning = new Rectangle(321,32,33,39);
            public static readonly Rectangle WeatherThundersnow = new Rectangle(355,31,39,40);            
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
