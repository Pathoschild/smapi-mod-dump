using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace CustomNPCFramework.Framework.ModularNpcs
{
    /// <summary>Used to hold all of the sprites for a single asset such as hair or bodies.</summary>
    public class AnimatedSpriteCollection
    {
        /// <summary>The left sprite for this sprite asset part.</summary>
        AnimatedSpriteExtended leftSprite;

        /// <summary>The right sprite for this sprite asset part.</summary>
        AnimatedSpriteExtended rightSprite;

        /// <summary>The up sprite for this sprite asset part.</summary>
        AnimatedSpriteExtended upSprite;

        /// <summary>The down sprite for this sprite asset part.</summary>
        AnimatedSpriteExtended downSprite;

        /// <summary>The current sprite for this sprite collection. This is one of the four directions for this collection.</summary>
        public AnimatedSpriteExtended currentSprite;

        /// <summary>Construct an instance.</summary>
        /// <param name="LeftSprite">Left animated sprite for this piece.</param>
        /// <param name="RightSprite">Right animated sprite for this piece.</param>
        /// <param name="UpSprite">Up animated sprite for this piece.</param>
        /// <param name="DownSprite">Down animated sprite for this piece.</param>
        /// <param name="startingSpriteDirection">The sprite's initial facing direction.</param>
        public AnimatedSpriteCollection(AnimatedSpriteExtended LeftSprite, AnimatedSpriteExtended RightSprite, AnimatedSpriteExtended UpSprite, AnimatedSpriteExtended DownSprite, Direction startingSpriteDirection)
        {
            this.leftSprite = LeftSprite;
            this.rightSprite = RightSprite;
            this.upSprite = UpSprite;
            this.downSprite = DownSprite;
            switch (startingSpriteDirection)
            {
                case Direction.down:
                    this.setDown();
                    break;

                case Direction.left:
                    this.setLeft();
                    break;

                case Direction.right:
                    this.setRight();
                    break;

                case Direction.up:
                    this.setUp();
                    break;
            }
        }

        /// <summary>Reloads all of the directional textures for this texture collection.</summary>
        public virtual void reload()
        {
            this.leftSprite.reload();
            this.rightSprite.reload();
            this.upSprite.reload();
            this.downSprite.reload();
        }

        /// <summary>Sets the current sprite direction to face left.</summary>
        public void setLeft()
        {
            this.currentSprite = this.leftSprite;
        }

        /// <summary>Sets the current sprite direction to face right.</summary>
        public void setRight()
        {
            this.currentSprite = this.rightSprite;
        }

        /// <summary>Sets the current sprite direction to face down.</summary>
        public void setDown()
        {
            this.currentSprite = this.downSprite;
        }

        /// <summary>Sets the current sprite direction to face up.</summary>
        public void setUp()
        {
            this.currentSprite = this.upSprite;
        }

        /// <summary>Used to draw the sprite to the screen.</summary>
        public void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth)
        {
            b.Draw(this.currentSprite.sprite.Texture, screenPosition, new Rectangle?(this.currentSprite.sprite.sourceRect), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, this.currentSprite.sprite.currentAnimation == null || !this.currentSprite.sprite.currentAnimation[this.currentSprite.sprite.currentAnimationIndex].flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally, layerDepth);
        }

        /// <summary>Used to draw the sprite to the screen.</summary>
        public void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth, int xOffset, int yOffset, Color c, bool flip = false, float scale = 1f, float rotation = 0.0f, bool characterSourceRectOffset = false)
        {
            b.Draw(this.currentSprite.sprite.Texture, screenPosition, new Rectangle?(new Rectangle(this.currentSprite.sprite.sourceRect.X + xOffset, this.currentSprite.sprite.sourceRect.Y + yOffset, this.currentSprite.sprite.sourceRect.Width, this.currentSprite.sprite.sourceRect.Height)), c, rotation, characterSourceRectOffset ? new Vector2((float)(this.currentSprite.sprite.SpriteWidth / 2), (float)((double)this.currentSprite.sprite.SpriteHeight * 3.0 / 4.0)) : Vector2.Zero, scale, flip || this.currentSprite.sprite.currentAnimation != null && this.currentSprite.sprite.currentAnimation[this.currentSprite.sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        /// <summary>A very verbose asset drawer.</summary>
        public void draw(SpriteBatch b, ExtendedNpc npc, Vector2 position, Rectangle sourceRectangle, Color color, float alpha, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            //DEFINITELY FIX THIS PART. Something is wrong with how these two functions handle the drawing of my npc to the scene.
            //this.draw(b, position, layerDepth);
            b.Draw(this.currentSprite.sprite.Texture, position, this.currentSprite.sprite.sourceRect, color, 0.0f, origin, scale, effects, layerDepth);
            //b.Draw(this.Sprite.Texture, npc.getLocalPosition(Game1.viewport) + new Vector2((float)(this.sprite.spriteWidth * Game1.pixelZoom / 2), (float)(this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float)Game1.random.Next(-1, 2), (float)Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * alpha, this.rotation, new Vector2((float)(this.sprite.spriteWidth / 2), (float)((double)this.sprite.spriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale) * (float)Game1.pixelZoom, this.flip || this.sprite.currentAnimation != null && this.sprite.currentAnimation[this.sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
        }

        /// <summary>Animate the current sprite. Theoreticlly works from index offset to how many frames</summary>
        /// <param name="intervalDelay">The delay in milliseconds between frames.</param>
        public void Animate(float intervalDelay, bool loop = true)
        {
            this.Animate(Game1.currentGameTime, 0, 2, intervalDelay, this.currentSprite.sprite, loop);
        }

        /// <summary>Animate the current sprite.</summary>
        /// <param name="gameTime">The game time from Monogames/XNA</param>
        /// <param name="startFrame">The starting frame of the animation on the sprite sheet.</param>
        /// <param name="numberOfFrames">The number of frames to animate the sprite.</param>
        /// <param name="interval">The delay between frames in milliseconds.</param>
        /// <param name="sprite">The animated sprite from the npc.</param>
        /// <param name="loop">If true, the animation plays over and over again.</param>
        public virtual bool Animate(GameTime gameTime, int startFrame, int numberOfFrames, float interval, AnimatedSprite sprite, bool loop = true)
        {
            if (sprite.CurrentFrame >= startFrame + numberOfFrames + 1 || sprite.CurrentFrame < startFrame)
                sprite.CurrentFrame = startFrame + sprite.CurrentFrame % numberOfFrames;
            sprite.timer = sprite.timer + (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if ((double)sprite.timer > (double)interval)
            {
                sprite.CurrentFrame = sprite.CurrentFrame + 1;
                sprite.timer = 0.0f;
                if (sprite.CurrentFrame == startFrame + numberOfFrames + 1 || sprite.currentFrame * sprite.SpriteWidth >= sprite.Texture.Width)
                {
                    if (loop)
                        sprite.CurrentFrame = startFrame;
                    sprite.UpdateSourceRect();
                    return true;
                }
            }
            this.UpdateSourceRect(sprite);
            return false;
        }

        /// <summary>Update the source rectangle on the sprite sheet. Needed for animation.</summary>
        public virtual void UpdateSourceRect(AnimatedSprite sprite)
        {
            if (sprite.ignoreSourceRectUpdates)
                return;
            sprite.sourceRect.X = sprite.CurrentFrame * sprite.SpriteWidth;
            sprite.sourceRect.Y = 0;
            //sprite.SourceRect = new Rectangle(, 0, sprite.spriteWidth, sprite.spriteHeight);
        }

        /// <summary>Animate the current sprite. Theoreticlly works from index offset to how many frames</summary>
        public void Animate(float intervalFromCharacter, int startFrame, int endFrame, bool loop)
        {
            this.currentSprite.sprite.loop = loop;
            this.currentSprite.sprite.Animate(Game1.currentGameTime, startFrame, endFrame, intervalFromCharacter);
        }
    }
}
