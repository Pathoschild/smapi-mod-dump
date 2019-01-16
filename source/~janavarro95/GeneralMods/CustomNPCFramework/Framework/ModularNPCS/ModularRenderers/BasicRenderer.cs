using System.Collections.Generic;
using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.ModularNpcs.CharacterAnimationBases;
using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CustomNPCFramework.Framework.ModularNpcs.ModularRenderers
{
    /// <summary>A class used to hold all of the textures/animations/information to make modular npc rendering possible.</summary>
    public class BasicRenderer
    {
        /// <summary>Dictionary that holds key pair values of (animationName,Animation). USed to manage multiple animations. </summary>
        public Dictionary<string, StandardCharacterAnimation> animationList;

        /// <summary>Used to keep track of what animation is currently being used.</summary>
        public StandardCharacterAnimation currentAnimation;

        /// <summary>Construct an instance.</summary>
        /// <param name="standingAnimation">The animation information to be used when the character is standing.</param>
        /// <param name="walkingAnimation">The animation information to be used when the character is walking/moving.</param>
        /// <param name="swimmingAnimation">The animation information to be used when the character is walking/moving.</param>
        public BasicRenderer(StandardCharacterAnimation standingAnimation, StandardCharacterAnimation walkingAnimation, StandardCharacterAnimation swimmingAnimation)
        {
            this.animationList = new Dictionary<string, StandardCharacterAnimation>();
            this.animationList.Add(AnimationKeys.standingKey, standingAnimation);
            this.animationList.Add(AnimationKeys.walkingKey, walkingAnimation);
            this.animationList.Add(AnimationKeys.swimmingKey, swimmingAnimation);
            this.setAnimation(AnimationKeys.standingKey);
        }

        /// <summary>Sets the animation associated with the key name; If it fails the npc will just default to standing.</summary>
        /// <param name="key">The name of the animation to swap the current animation to.</param>
        public virtual void setAnimation(string key)
        {
            this.currentAnimation = this.animationList[key];
            if (this.currentAnimation == null)
            {
                Class1.ModMonitor.Log("ERROR SETTING AN ANIMATION: " + key);
                this.setAnimation(AnimationKeys.standingKey);
            }
        }

        /// <summary>Sets the direction of the current animated sprite respectively.</summary>
        /// <param name="facingDirection">The direction to face. 0=up, 1=right, 2= down, 3=left.</param>
        public virtual void setDirection(int facingDirection)
        {
            this.setDirection((Direction)facingDirection);
        }

        /// <summary>Sets the direction of the current animated sprite respectively to the direction passed in.</summary>
        /// <param name="direction">The direction to face.</param>
        public virtual void setDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.up:
                    this.setUp();
                    break;

                case Direction.right:
                    this.setRight();
                    break;

                case Direction.down:
                    this.setDown();
                    break;

                case Direction.left:
                    this.setLeft();
                    break;
            }
        }

        /// <summary>Sets the current animated sprite to face left.</summary>
        public virtual void setLeft()
        {
            this.currentAnimation.setLeft();
        }

        /// <summary>Sets the current animated sprite to face right.</summary>
        public virtual void setRight()
        {
            this.currentAnimation.setRight();
        }

        /// <summary>Sets the current animated sprite to face up.</summary>
        public virtual void setUp()
        {
            this.currentAnimation.setUp();
        }

        /// <summary>Sets the current animated sprite to face down.</summary>
        public virtual void setDown()
        {
            this.currentAnimation.setDown();
        }

        /// <summary>Used to reload all of the sprites pertaining to all of the animations stored in this renderer.</summary>
        public virtual void reloadSprites()
        {
            foreach (var v in this.animationList)
                v.Value.reload();
        }

        /// <summary>Used to draw the sprite to the screen.</summary>
        public virtual void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth)
        {
            this.currentAnimation.draw(b, screenPosition, layerDepth);
        }

        /// <summary>Used to draw the sprite to the screen.</summary>
        public virtual void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth, int xOffset, int yOffset, Color c, bool flip = false, float scale = 1f, float rotation = 0.0f, bool characterSourceRectOffset = false)
        {
            this.currentAnimation.draw(b, screenPosition, layerDepth, xOffset, yOffset, c, flip, scale, rotation, characterSourceRectOffset);
        }

        /// <summary>A very verbose asset drawer.</summary>
        public virtual void draw(SpriteBatch b, ExtendedNpc npc, Vector2 position, Rectangle sourceRectangle, Color color, float alpha, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            this.currentAnimation.draw(b, npc, position, sourceRectangle, color, alpha, origin, scale, effects, layerDepth);
        }


        /// <summary>Animates the current animation for the current sprite.</summary>
        public virtual void Animate(float interval, bool loop = true)
        {
            this.currentAnimation.Animate(interval, loop);
        }

        /// <summary>Wrapper for a draw function that accepts rectangles to be null.</summary>
        public virtual void draw(SpriteBatch b, ExtendedNpc extendedNPC, Vector2 position, Rectangle? v1, Color white, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float v3)
        {
            this.draw(b, extendedNPC, position, new Rectangle(0, 0, 16, 32), white, rotation, origin, scale, spriteEffects, v3);
        }
    }
}
