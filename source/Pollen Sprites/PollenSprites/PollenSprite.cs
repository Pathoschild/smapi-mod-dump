using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace PollenSprites
{
    /// <summary>A custom monster based on Stardew's Ghost class.</summary>
    /// <remarks> This subclass copies all of its contents from Ghost, except those added/overridden inside this class.</remarks>
    public class PollenSprite : Ghost
    {
        /// <summary>The readable version of this monster's name. Needs to match the address of its spritesheet, as in "Characters/Monsters/Pollen Sprite".</summary>
        private static readonly string customName = "Pollen Sprite";

        /// <summary>The color used by some of this monster's visual effects.</summary>
        public static Color effectColor = new Color(255, 183, 255);

        /// <summary>The total game time (in milliseconds) when this monster last used the "sprinkles" visual effect.</summary>
        protected double timeOfLastSprinkle = 0;

        /// <summary>True if this monster is currently turning right.</summary>
        /// <remarks>This is a more accessible duplicate of Ghost.turningRight for use in the updateAnimation override method.</remarks>
        protected bool turningRight = default(bool);

        /// <summary>Creates an non-functional instance of this monster for use by certain Stardew methods.</summary>
        /// <remarks>This should not be used by mods to create new monsters, as it avoids some necessary settings and methods.</remarks>
        public PollenSprite()
            : base() //call the default Ghost constructor
        {
        }

        /// <summary>Creates a new instance of this monster class.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <remarks>
        /// Farm Type Manager calls this constructor (i.e. the constructor with a Vector2 parameter and no others) when it creates a custom monster from another mod.
        /// </remarks>
        public PollenSprite(Vector2 position)
            : base(position, customName) //call the Ghost class's "position, name" constructor
        {
            HideShadow = true; //hide this monster's shadow, preventing a "double shadow" bug in most game locations
            Scale = (float)Game1.random.Next(70, 91) / 100; //randomly choose a size from 70-90%

            if (Game1.random.NextDouble() < 0.1) //10% chance
            {
                switch (Game1.random.Next(0, 6)) //based on a random number
                {
                    case 0:
                        objectsToDrop.Add(429); //jazz seeds
                        break;
                    case 1:
                        objectsToDrop.Add(427); //tulip bulb
                        break;
                    case 2:
                        objectsToDrop.Add(453); //poppy seeds
                        break;
                    case 3:
                        objectsToDrop.Add(455); //spangle seeds
                        break;
                    case 4:
                        objectsToDrop.Add(431); //sunflower seeds
                        break;
                    case 5:
                        objectsToDrop.Add(425); //fairy seeds
                        break;
                }
            }
        }

        /// <summary>This override forces instances of GameLocation to call drawAboveAllLayers, fixing a bug where flying monsters are invisible on some maps.</summary>
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b); //call the base version of this method
            drawAboveAllLayers(b); //call the method that draws this monster above everything else
        }

        /// <summary>This override is a modified copy of the DustSpirit version.</summary>
        /// <remarks>
        /// * A sound effect was added.
        /// * The source rectangle was modified to use this monster's spritesheet.
        /// * The number of chunks was multiplied.
        /// * The scale parameter was used instead of a local health-based value.
        /// </remarks>
        public override void shedChunks(int number, float scale)
        {
            this.currentLocation.localSound("leafrustle"); //play the leaf rustling sound effect
            GameLocation currentLocation = this.currentLocation;
            string textureName = (string)((NetFieldBase<string, NetString>)this.Sprite.textureName);
            Rectangle sourcerectangle = new Rectangle(0, 96, 16, 16); //use a different y value for this monster's custom spritesheet
            int sizeOfSourceRectSquares = 8;
            Rectangle boundingBox = this.GetBoundingBox();
            int x = boundingBox.Center.X;
            boundingBox = this.GetBoundingBox();
            int y1 = boundingBox.Center.Y;
            int numberOfChunks = number * 2; //multiply chunk count
            int y2 = (int)this.getTileLocation().Y;
            //remove locally chosen scale
            Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, sizeOfSourceRectSquares, x, y1, numberOfChunks, y2, Color.White, scale * 2); //multiply provided scale
        }

        /// <summary>This override is a modified copy of the DustSpirit version.</summary>
        /// <remarks>
        /// * A leaf rustle sound was added.
        /// * The temporary sprites were modified to use this monster's preset effect color.
        /// </remarks>
        protected override void localDeathAnimation()
        {
            this.currentLocation.localSound("leafrustle"); //play the leaf rustling sound effect
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position, effectColor, 10, false, 100f, 0, -1, -1f, -1, 0));
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), effectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 150,
                scale = 0.5f
            });
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), effectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 300,
                scale = 0.5f
            });
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), effectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 450,
                scale = 0.5f
            });
        }

        /// <summary>This override makes this monster's shadow transparent and scales it to the monster's size.</summary>
        public override void drawAboveAllLayers(SpriteBatch b)
        {
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float)(21 + this.yOffset)), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, (float)((NetFieldBase<float, NetFloat>)this.scale)) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
            SpriteBatch spriteBatch = b;
            Texture2D shadowTexture = Game1.shadowTexture;
            Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
            Color shadowColor = new Color(255, 255, 255, 63); //75% transparency
            double num1 = 0.0;
            Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
            double x = (double)bounds.Center.X;
            bounds = Game1.shadowTexture.Bounds;
            double y = (double)bounds.Center.Y;
            Vector2 origin = new Vector2((float)x, (float)y);
            double shadowScale = (3.0 + (double)this.yOffset / 20.0) * Scale; //multiply by the monster's scale
            int num3 = 0;
            double num4 = (double)(this.getStandingY() - 1) / 10000.0;
            spriteBatch.Draw(shadowTexture, position, sourceRectangle, shadowColor, (float)num1, origin, (float)shadowScale, (SpriteEffects)num3, (float)num4);
        }

        /// <summary>This override reworks several aspects of the Ghost version.</summary>
        /// <remarks>
        /// * A timed "sprinkle" visual effect was added.
        /// * yOffset now uses the public NPC.yOffset field, rather than Ghost's private field of the same name.
        /// * Lightsource generation was disabled. Note that lightsource removal is handled separately in the Ghost.takeDamage method.
        /// * Other Ghost private fields were removed or refactored due to being unused or redundant.
        /// * Redundant calls to bounding box methods were improved slightly.
        /// </remarks>
        protected override void updateAnimation(GameTime time)
        {
            if (time.TotalGameTime.TotalMilliseconds - timeOfLastSprinkle > 2000) //if it's been over 2 seconds since the previous sprinkle
            {
                timeOfLastSprinkle = time.TotalGameTime.TotalMilliseconds + Game1.random.Next(-500, 501); //record the current time, randomly offset by up to 0.5 seconds
                Color color = (Game1.random.NextDouble() < 0.5 ? effectColor : Color.White); //randomly choose between the monster's effect color or white
                Utility.addSprinklesToLocation(this.currentLocation, this.getTileX(), this.getTileY(), 2, 2, 101, 50, color, (string)null, false); //display a new sprinkle effect
            }

            this.yOffset = (int)(Math.Sin((double)time.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 20.0); //use NPC.yOffset and ignore the unused yOffsetExtra field
                                                                                                                        //remove lightsource generation process
            Rectangle playerBox = this.Player.GetBoundingBox();             //improve redundant calls to GetBoundingBox
            Rectangle monsterBox = this.GetBoundingBox();                   //
            int x1 = playerBox.Center.X;                                    //
            int x2 = monsterBox.Center.X;                                   //
            float num1 = (float)-(x1 - x2);                                 //
            float num2 = (float)(playerBox.Center.Y - monsterBox.Center.Y); //
            float num3 = 400f;
            float num4 = num1 / num3;
            float num5 = num2 / num3;
            //remove Ghost.wasHitCounter condition, which was always true
            float targetRotation = (float)Math.Atan2(-(double)num5, (double)num4) - 1.570796f; //declare targetRotation locally, which is only used in this method anyway
            if ((double)Math.Abs(targetRotation) - (double)Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextDouble() < 0.5)
                this.turningRight = true;
            else if ((double)Math.Abs(targetRotation) - (double)Math.Abs(this.rotation) < Math.PI / 8.0)
                this.turningRight = false;
            if (this.turningRight)
                this.rotation -= (float)Math.Sign(targetRotation - this.rotation) * ((float)Math.PI / 64f);
            else
                this.rotation += (float)Math.Sign(targetRotation - this.rotation) * ((float)Math.PI / 64f);
            this.rotation %= 6.283185f;
            //remove wasHitCounter = 0", which was redundant
            float num6 = Math.Min(4f, Math.Max(1f, (float)(5.0 - (double)num3 / 64.0 / 2.0)));
            float num7 = (float)Math.Cos((double)this.rotation + Math.PI / 2.0);
            float num8 = -(float)Math.Sin((double)this.rotation + Math.PI / 2.0);
            this.xVelocity += (float)(-(double)num7 * (double)num6 / 6.0 + (double)Game1.random.Next(-10, 10) / 100.0);
            this.yVelocity += (float)(-(double)num8 * (double)num6 / 6.0 + (double)Game1.random.Next(-10, 10) / 100.0);
            if ((double)Math.Abs(this.xVelocity) > (double)Math.Abs((float)(-(double)num7 * 5.0)))
                this.xVelocity -= (float)(-(double)num7 * (double)num6 / 6.0);
            if ((double)Math.Abs(this.yVelocity) > (double)Math.Abs((float)(-(double)num8 * 5.0)))
                this.yVelocity -= (float)(-(double)num8 * (double)num6 / 6.0);
            this.faceGeneralDirection(this.Player.getStandingPosition(), 0, false);
            this.resetAnimationSpeed();
        }

        /// <summary>This override reworks miscellaneous behaviors within the Ghost version.</summary>
        /// <remarks>
        /// * The "sprinkles" visual effect's color was changed.
        /// * Bonus damage and related effects for the "holy sword" weapon were removed.
        /// * Lightsource removal code was removed because this monster no longer generates lightsources.
        /// </remarks>
        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int num = Math.Max(1, damage - (int)(NetFieldBase<int, NetInt>)this.resilience);
            this.Slipperiness = 8;
            Color color = (Game1.random.NextDouble() < 0.5 ? effectColor : Color.White); //randomly choose between the monster's effect color or white
            Utility.addSprinklesToLocation(this.currentLocation, this.getTileX(), this.getTileY(), 2, 2, 101, 50, color, (string)null, false); //use the selected color
            if (Game1.random.NextDouble() < (double)(NetFieldBase<double, NetDouble>)this.missChance - (double)(NetFieldBase<double, NetDouble>)this.missChance * addedPrecision)
            {
                num = -1;
            }
            else
            {
                //remove "holy sword" bonus damage check
                this.Health -= num;
                if (this.Health <= 0)
                    this.deathAnimation();
                this.setTrajectory(xTrajectory, yTrajectory);
            }
            this.addedSpeed = -1;
            //remove lightsource removal code
            return num;
        }
    }
}
