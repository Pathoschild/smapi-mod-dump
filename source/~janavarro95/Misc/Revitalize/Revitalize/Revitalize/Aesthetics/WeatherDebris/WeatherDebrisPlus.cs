using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Revitalize.Aesthetics.WeatherDebris
{
    


    public class WeatherDebrisPlus
    {
        public Rectangle sourceRect;
        public bool blowing;
        public Vector2 position;
        public int which;
        public float dx;
        public float dy;
        public int animationIntervalOffset;
        public Texture2D debrisTexture;

        public WeatherDebrisPlus()
        {

        }
        
        /// <summary>
        /// Create a new debris particle to be shown across the screen.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="SourceRect">Source on the texture sprite sheet.</param>
        /// <param name="animationOffset"></param>
        /// <param name="which">Not really used, but can be used for default list.</param>
        /// <param name="rotationVelocity"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="texture">If NULL, then Game1.mouseCursors is used.</param>
        public WeatherDebrisPlus(Vector2 position,Rectangle SourceRect, int animationOffset, int which, float rotationVelocity, float dx, float dy, Texture2D texture=null) 
        {
            if (texture == null) debrisTexture = Game1.mouseCursors;
            else debrisTexture = texture;
            this.position = position;
            this.which = which;
            this.dx = dx;
            this.dy = dy;
            sourceRect = SourceRect;
            animationIntervalOffset = animationOffset;
        }

        /// <summary>
        /// Default system that uses some presets. Might or might not use.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="SourceRect">Source on the texture sprite sheet.</param>
        /// <param name="animationOffset"></param>
        /// <param name="which">Not really used, but can be used for default list.</param>
        /// <param name="rotationVelocity"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public WeatherDebrisPlus(Vector2 position, Rectangle SourceRect, int animationOffset, int which, float rotationVelocity, float dx, float dy)
        {
            this.position = position;
            this.which = which;
            this.dx = dx;
            this.dy = dy;
          //  sourceRect = SourceRect;
            animationIntervalOffset = animationOffset;
            switch (which)
            {
                case 0:
                    this.sourceRect = new Rectangle(352, 1184, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    return;
                case 1:
                    this.sourceRect = new Rectangle(352, 1200, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    return;
                case 2:
                    this.sourceRect = new Rectangle(352, 1216, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    return;
                case 3:
                    this.sourceRect = new Rectangle(391 + 4 * Game1.random.Next(5), 1236, 4, 4);
                    return;
                case 4:
                    this.sourceRect = new Rectangle(338, 400, 8, 8);
                    return;

                default:
                    return;
            }
        }

        public new void update()
        {
            this.update(false);
        }

        public new void update(bool slow)
        {
            this.position.X = this.position.X + (this.dx + (slow ? 0f :StardewValley.WeatherDebris.globalWind));
            this.position.Y = this.position.Y + (this.dy - (slow ? 0f : -0.5f));
            if (this.dy < 0f && !this.blowing)
            {
                this.dy += 0.01f;
            }
            if (!Game1.fadeToBlack && Game1.fadeToBlackAlpha <= 0f)
            {
                if (this.position.X < (float)(-(float)Game1.tileSize - Game1.tileSize / 4))
                {
                    this.position.X = (float)Game1.viewport.Width;
                    this.position.Y = (float)Game1.random.Next(0, Game1.viewport.Height - Game1.tileSize);
                }
                if (this.position.Y > (float)(Game1.viewport.Height + Game1.tileSize / 4))
                {
                    this.position.X = (float)Game1.random.Next(0, Game1.viewport.Width);
                    this.position.Y = (float)(-(float)Game1.tileSize);
                    this.dy = (float)Game1.random.Next(-15, 10) / (slow ? ((Game1.random.NextDouble() < 0.1) ? 5f : 200f) : 50f);
                    this.dx = (float)Game1.random.Next(-10, 0) / (slow ? 200f : 50f);
                }
                else if (this.position.Y < (float)(-(float)Game1.tileSize))
                {
                    this.position.Y = (float)Game1.viewport.Height;
                    this.position.X = (float)Game1.random.Next(0, Game1.viewport.Width);
                }
            }
            if (this.blowing)
            {
                this.dy -= 0.01f;
                if (Game1.random.NextDouble() < 0.006 || this.dy < -2f)
                {
                    this.blowing = false;
                }
            }
            else if (!slow && Game1.random.NextDouble() < 0.001 && Game1.currentSeason != null && (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")))
            {
                this.blowing = true;
            }


            
        }

        public void draw(SpriteBatch b)
        {
            b.Draw(this.debrisTexture, this.position, new Rectangle?(this.sourceRect), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1E-06f);

        }
    }
}
