/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardustCore.Animations
{
    /// <summary>Used to play animations for Stardust.CoreObject type objects and all objects that extend from it. In draw code of object make sure to use this info instead.</summary>
    public class AnimationManager
    {

        public Dictionary<string, Animation> animations = new SerializableDictionary<string, Animation>();
        public string currentAnimationName;
        public Texture2DExtended objectTexture; ///Might not be necessary if I use the CoreObject texture sheet.
        public bool enabled;

        [JsonIgnore]
        public bool requiresUpdate;
        public bool IsNull => this.objectTexture == null;

        public string defaultAnimationKey;

        public string startingAnimationKey;

        public const int StaticAnimationFrameIndex = -1;

        /// <summary>Construct an instance.</summary>
        public AnimationManager() { }


        /// <summary>Constructor for Animation Manager class.</summary>
        /// <param name="ObjectTexture">The texture that will be used for the animation. This is typically the same as the object this class is attached to.</param>
        /// <param name="DefaultFrame">This is used if no animations will be available to the animation manager.</param>
        /// <param name="EnabledByDefault">Whether or not animations play by default. Default value is true.</param>
        public AnimationManager(Texture2DExtended ObjectTexture, Animation DefaultAnimation, bool EnabledByDefault = true) : this(ObjectTexture, new Dictionary<string, Animation>() { { "Default", DefaultAnimation } }, "Default", "Default")
        {
        }

        public AnimationManager(Texture2DExtended ObjectTexture, Dictionary<string, Animation> Animations, string DefaultAnimationKey, string StartingAnimationKey, int startingAnimationFrame = 0, bool EnabledByDefault = true)
        {
            this.objectTexture = ObjectTexture;
            this.enabled = EnabledByDefault;

            this.animations = Animations;
            this.defaultAnimationKey = DefaultAnimationKey;
            if (this.animations != null && string.IsNullOrEmpty(StartingAnimationKey) == false && this.animations.ContainsKey(StartingAnimationKey))
            {
                this.startingAnimationKey = StartingAnimationKey;
                this.setAnimation(StartingAnimationKey, startingAnimationFrame);
                this.playAnimation(StartingAnimationKey, true, startingAnimationFrame);
            }
            else
            {
                this.currentAnimationName = DefaultAnimationKey;
                this.startingAnimationKey = DefaultAnimationKey;
                this.setAnimation(DefaultAnimationKey, startingAnimationFrame);
                this.playAnimation(DefaultAnimationKey, true, startingAnimationFrame);

            }

        }

        /// <summary>Update the animation frame once after drawing the object.</summary>
        public void tickAnimation()
        {
            if (!this.enabled || this.currentAnimationName.Equals(this.defaultAnimationKey))
                return; //This is if this is a default animation or the animation stops here.
            this.getCurrentAnimation().tickAnimation();

        }

        /// <summary>Gets the animation from the dictionary of all animations available.</summary>
        /// <param name="AnimationName"></param>
        /// <param name="StartingFrame"></param>
        public bool setAnimation(string AnimationName, int StartingFrame = 0)
        {
            if (this.animations.ContainsKey(AnimationName))
            {
                if (this.getCurrentAnimation() != null)
                {
                    this.getCurrentAnimation().reset();
                }
                this.currentAnimationName = AnimationName;
                return true;
            }
            else
            {
                ModCore.ModMonitor.Log("Error setting animation: " + AnimationName + " animation does not exist in list of available animations. Did you make sure to add it in?");
                return false;
            }
        }

        /// <summary>
        /// Plays the animation for the animation manager.
        /// </summary>
        /// <param name="AnimationName"></param>
        /// <param name="overrideSameAnimation"></param>
        /// <param name="StartingFrame"></param>
        /// <returns></returns>
        public bool playAnimation(string AnimationName, bool overrideSameAnimation = false, int StartingFrame = 0)
        {
            if (this.animations.ContainsKey(AnimationName))
            {
                if (this.animations.ContainsKey(AnimationName))
                {
                    this.getCurrentAnimation().reset();
                    this.currentAnimationName = AnimationName;
                    this.getCurrentAnimation().startAnimation();
                    return true;
                }
                else
                {
                    ModCore.ModMonitor.Log("Error setting animation: " + AnimationName + " animation does not exist in list of available animations. Did you make sure to add it in?");
                    return false;
                }
            }
            else
            {
                ModCore.ModMonitor.Log("Error setting animation: " + AnimationName + " animation does not exist in list of available animations. Did you make sure to add it in?");
                return false;
            }
        }

        /// <summary>
        /// Plays the default animation.
        /// </summary>
        public void playDefaultAnimation()
        {
            this.currentAnimationName = this.defaultAnimationKey;
            this.requiresUpdate = true;
        }

        /// <summary>Sets the animation manager to an on state, meaning that this animation will update on the draw frame.</summary>
        public void enableAnimation()
        {
            this.enabled = true;
        }

        /// <summary>Sets the animation manager to an off state, meaning that this animation will no longer update on the draw frame.</summary>
        public void disableAnimation()
        {
            this.enabled = false;
        }

        /// <summary>Used to handle general drawing functionality using the animation manager.</summary>
        /// <param name="spriteBatch">We need a spritebatch to draw.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="Position">The onscreen position to draw to.</param>
        /// <param name="sourceRectangle">The source rectangle on the texture to draw.</param>
        /// <param name="drawColor">The color to draw the thing passed in.</param>
        /// <param name="rotation">The rotation of the animation texture being drawn.</param>
        /// <param name="origin">The origin of the texture.</param>
        /// <param name="scale">The scale of the texture.</param>
        /// <param name="spriteEffects">Effects that get applied to the sprite.</param>
        /// <param name="LayerDepth">The dept at which to draw the texture.</param>
        public void draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 Position, Rectangle? sourceRectangle, Color drawColor, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float LayerDepth)
        {
            //Log.AsyncC("Animation Manager is working!");
            spriteBatch.Draw(texture, Position, sourceRectangle, drawColor, rotation, origin, scale, spriteEffects, LayerDepth);
            this.tickAnimation();
        }

        /// <summary>
        /// Draws the texture to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="Position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="drawColor"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="spriteEffects"></param>
        /// <param name="LayerDepth"></param>
        public void draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 Position, Rectangle? sourceRectangle, Color drawColor, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float LayerDepth)
        {
            //Log.AsyncC("Animation Manager is working!");
            spriteBatch.Draw(texture, Position, sourceRectangle, drawColor, rotation, origin, scale, spriteEffects, LayerDepth);
            this.tickAnimation();
            // Log.AsyncC("Tick animation");
        }

        /// <summary>
        /// Used to draw the current animation to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Position"></param>
        /// <param name="drawColor"></param>
        /// <param name="scale"></param>
        /// <param name="flipped"></param>
        /// <param name="depth"></param>
        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, float scale, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.getCurrentAnimation().getCurrentAnimationFrameRectangle(), drawColor, 0f, Vector2.Zero, scale, flipped, depth);
            this.tickAnimation();
        }

        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, float scale, float Rotation, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.getCurrentAnimation().getCurrentAnimationFrameRectangle(), drawColor, Rotation, Vector2.Zero, scale, flipped, depth);
            this.tickAnimation();
        }

        /// <summary>
        /// Draws the animated texture to the screen.
        /// </summary>
        /// <param name="b">The Sprite Batch used to draw.</param>
        /// <param name="Position">The position to draw the sprite to.</param>
        /// <param name="drawColor">The color to draw the sprite to.</param>
        /// <param name="scale">The scale for the sprite as a Vector2. (Width,Height)</param>
        /// <param name="flipped">If the sprite is flipped.</param>
        /// <param name="depth">The depth of the sprite.</param>
        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, Vector2 scale, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.getCurrentAnimation().getCurrentAnimationFrameRectangle(), drawColor, 0f, Vector2.Zero, scale, flipped, depth);
            this.tickAnimation();
        }

        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, Vector2 scale, float Rotation, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.getCurrentAnimation().getCurrentAnimationFrameRectangle(), drawColor, Rotation, Vector2.Zero, scale, flipped, depth);
            this.tickAnimation();
        }

        public Texture2DExtended getExtendedTexture()
        {
            return this.objectTexture;
        }

        public void setExtendedTexture(Texture2DExtended texture)
        {
            this.objectTexture = texture;
        }

        public void setEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        public Texture2D getTexture()
        {
            if (this.objectTexture == null)
            {
                return null;
            }
            return this.objectTexture.getTexture();
        }

        /// <summary>
        /// Gets the currently playing animation.
        /// </summary>
        /// <returns></returns>
        public virtual Animation getCurrentAnimation()
        {
            if (string.IsNullOrEmpty(this.currentAnimationName)) return null;

            if (this.animations.ContainsKey(this.currentAnimationName))
            {
                return this.animations[this.currentAnimationName];
            }
            else if (this.animations.ContainsKey(this.defaultAnimationKey))
            {
                return this.animations[this.defaultAnimationKey];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the default animation for this animation manager.
        /// </summary>
        /// <returns></returns>
        public virtual Animation getDefaultAnimation()
        {
            if (this.animations.ContainsKey(this.defaultAnimationKey))
            {
                return this.animations[this.defaultAnimationKey];
            }
            return null;
        }

        public virtual Rectangle getCurrentAnimationFrameRectangle()
        {
            Animation currentAnimation = this.getCurrentAnimation();
            if (currentAnimation == null) return default(Rectangle);
            return currentAnimation.getCurrentAnimationFrameRectangle();
        }

        public AnimationManager Copy()
        {
            return new AnimationManager(this.objectTexture, this.animations, this.defaultAnimationKey, this.startingAnimationKey, 0, this.enabled);
        }
    }
}
