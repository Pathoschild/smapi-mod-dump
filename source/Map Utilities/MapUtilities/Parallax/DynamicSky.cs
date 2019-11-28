using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Parallax
{
    public class DynamicSky
    {
        public Texture2D skyGradient;
        public Texture2D starMap;

        public int dawnTime = 600;
        public Color dawnTint = Color.Salmon;
        public int dayTime = 700;
        public Color dayTint = Color.LightSkyBlue;
        public int eveningTime = 1700;
        public Color sunsetTint = Color.Coral;
        public int sunsetTime = 1900;
        public int nightTime = 2000;
        public Color nightTint = Color.Navy * 0.5f;

        public Color starsTransparent = Color.Transparent;
        public Color starsOpaque = Color.White;

        public Color c;
        public Color starsC;

        public List<Cloud> clouds;

        public DynamicSky(Texture2D gradient, Texture2D stars)
        {
            skyGradient = gradient;
            starMap = stars;
            c = dayTint;
            c = starsTransparent;
            clouds = new List<Cloud>();
            for(int i = 0; i < 15; i++)
            {
                float randomX = (float)(Cloud.cloudRandom.NextDouble() * (float)Game1.viewport.Width);
                float randomY = (float)Cloud.cloudRandom.NextDouble() * 640f;
                Cloud newCloud = new Cloud(new Vector2(randomX, randomY));
                clouds.Add(newCloud);
            }
        }

        public void update()
        {
            if (Game1.timeOfDay < 620)
            {
                starsC = getRampedColor(starsOpaque, starsTransparent, 600, 620);
            }
            else if(Game1.timeOfDay < sunsetTime)
            {
                starsC = starsTransparent;
            }

            //Sun rising
            if(Game1.timeOfDay >= dawnTime && Game1.timeOfDay < dayTime)
            {
                c = getRampedColor(dawnTint, dayTint, dawnTime, dayTime);
            }
            //Day time
            else if(Game1.timeOfDay >= dayTime && Game1.timeOfDay < eveningTime)
            {
                c = dayTint;
                starsC = starsTransparent;
            }
            //Sun setting
            else if(Game1.timeOfDay >= eveningTime && Game1.timeOfDay < sunsetTime)
            {
                c = getRampedColor(dayTint, sunsetTint, eveningTime, sunsetTime);
                starsC = starsTransparent;
            }
            //Dusk
            else if(Game1.timeOfDay >= sunsetTime && Game1.timeOfDay < nightTime)
            {
                c = getRampedColor(sunsetTint, nightTint, sunsetTime, nightTime);
                starsC = getRampedColor(starsTransparent, starsOpaque, sunsetTime, nightTime);
            }
            //Night
            else
            {
                c = nightTint;
                starsC = starsOpaque;
            }
            foreach(Cloud cloud in clouds)
            {
                cloud.update(640);
            }
        }

        public void draw(SpriteBatch b)
        {
            b.Draw(
                    skyGradient,
                    new Rectangle(0, 0, (int)(Game1.viewport.Width * Game1.options.zoomLevel), (int)(Game1.viewport.Height * Game1.options.zoomLevel)),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, skyGradient.Width, skyGradient.Height)),
                    c,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    5E-08f
            );
            b.Draw(
                    starMap,
                    new Rectangle(0, 0, (int)(Game1.viewport.Width * Game1.options.zoomLevel), (int)(Game1.viewport.Height * Game1.options.zoomLevel)),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, starMap.Width, starMap.Height)),
                    starsC,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    6E-08f
            );
            foreach(Cloud cloud in clouds)
            {
                cloud.draw(b);
            }
        }

        internal static Color getRampedColor(Color a, Color b, int timeA, int timeB)
        {
            double factor = (double)(timeToDecimal(Game1.timeOfDay) - timeToDecimal(timeA)) / (double)(timeToDecimal(timeB) - timeToDecimal(timeA));
            return new Color(
                    a.R + (int)(factor * (b.R - a.R)),
                    a.G + (int)(factor * (b.G - a.G)),
                    a.B + (int)(factor * (b.B - a.B)),
                    a.A + (int)(factor * (b.A - a.A)));
        }

        internal static int timeToDecimal(int time)
        {
            return (time / 100) * 100 + (int)(((time % 100) / 60f) * 100);
        }
    }
}
