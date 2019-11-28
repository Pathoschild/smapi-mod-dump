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
    public class Cloud
    {
        public static Texture2D cloudSheet;
        public static Random cloudRandom;

        public static int dawnTime = 600;
        public static Color dawnTint = Color.Wheat;
        public static int dayTime = 730;
        public static Color dayTint = Color.White;
        public static int eveningTime = 1800;
        public static Color sunsetTint = Color.Orange;
        public static int sunsetTime = 1900;
        public static int nightTime = 2000;
        public static Color nightTint = new Color((int)(Color.Navy.R * 0.5f), (int)(Color.Navy.G * 0.5f), (int)(Color.Navy.B * 0.5f));

        public Vector2 position;
        public float speed;
        public int sprite;

        public Color c;

        public static void init()
        {
            cloudSheet = (Texture2D)Loader.load<Texture2D>("Content/Sky/Clouds", "Content/Sky/Clouds.png");
            cloudRandom = new Random();
        }

        //Features: 
        //clouds have their own color ramping
        //clouds will move with a speed that depends on the weather?
        //Cloud count based on day
        //When cloud is entirely off-screen, it changes its sprite, its y value, and returns to off-screen on the other side

        public Cloud(Vector2 position)
        {
            sprite = cloudRandom.Next(5);
            c = Color.White;
            this.position = position;
            speed = 1.2f;
        }

        public void update(double horizon)
        {
            position.X -= (1 - (position.Y / 700f)) * speed;
            if (position.X < -168 * 4f)
            {
                position.X = Game1.viewport.Width + (168 * 4f);
                position.Y = (float)(cloudRandom.NextDouble() * horizon);
                sprite = cloudRandom.Next(5);
            }
            //Sun rising
            if (Game1.timeOfDay >= dawnTime && Game1.timeOfDay < dayTime)
            {
                c = DynamicSky.getRampedColor(dawnTint, dayTint, dawnTime, dayTime);
            }
            //Day time
            else if (Game1.timeOfDay >= dayTime && Game1.timeOfDay < eveningTime)
            {
                c = dayTint;
            }
            //Sun setting
            else if (Game1.timeOfDay >= eveningTime && Game1.timeOfDay < sunsetTime)
            {
                c = DynamicSky.getRampedColor(dayTint, sunsetTint, eveningTime, sunsetTime);
            }
            //Dusk
            else if (Game1.timeOfDay >= sunsetTime && Game1.timeOfDay < nightTime)
            {
                c = DynamicSky.getRampedColor(sunsetTint, nightTint, sunsetTime, nightTime);
            }
            //Night
            else
            {
                c = nightTint;
            }
        }

        public void draw(SpriteBatch b)
        {
            b.Draw(
                    cloudSheet,
                    position,
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle((sprite % 3) * 168, (sprite / 3) * 168, 168, 168)),
                    c,
                    0.0f,
                    new Vector2(0,168),
                    (1-(position.Y / 700f))*4f,
                    SpriteEffects.None,
                    7E-08f - (position.Y * 1E-10f)
            );
        }
    }
}
