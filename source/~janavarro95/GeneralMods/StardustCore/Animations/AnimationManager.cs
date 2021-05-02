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
        public Dictionary<string, List<Animation>> animations = new SerializableDictionary<string, List<Animation>>();
        public string currentAnimationName;
        private int currentAnimationListIndex;
        public List<Animation> currentAnimationList = new List<Animation>();
        public Texture2DExtended objectTexture; ///Might not be necessary if I use the CoreObject texture sheet.
        public Animation defaultDrawFrame;
        public Animation currentAnimation;
        public bool enabled;
        public bool loopAnimation;

        public string animationDataString;

        [JsonIgnore]
        public bool requiresUpdate;
        public bool IsNull => this.defaultDrawFrame == null && this.objectTexture == null;

        public bool hasRecievedUpdateTick;

        public string startingAnimationName;

        /// <summary>
        /// Checks to see if there is an animation playing.
        /// </summary>
        public bool IsAnimationPlaying
        {
            get
            {
                return !(this.defaultDrawFrame == this.currentAnimation);
            }
        }

        /// <summary>Construct an instance.</summary>
        public AnimationManager() { }


        /// <summary>Constructor for Animation Manager class.</summary>
        /// <param name="ObjectTexture">The texture that will be used for the animation. This is typically the same as the object this class is attached to.</param>
        /// <param name="DefaultFrame">This is used if no animations will be available to the animation manager.</param>
        /// <param name="EnabledByDefault">Whether or not animations play by default. Default value is true.</param>
        public AnimationManager(Texture2DExtended ObjectTexture, Animation DefaultFrame, bool EnabledByDefault = true)
        {
            this.currentAnimationListIndex = 0;
            this.objectTexture = ObjectTexture;
            this.defaultDrawFrame = DefaultFrame;
            this.enabled = EnabledByDefault;
            this.currentAnimation = this.defaultDrawFrame;
            this.currentAnimationName = "";
            this.animationDataString = "";
            this.startingAnimationName = "";
        }

        public AnimationManager(Texture2DExtended ObjectTexture, Animation DefaultFrame, string animationString, string startingAnimationKey, int startingAnimationFrame = 0, bool EnabledByDefault = true)
        {
            this.currentAnimationListIndex = 0;
            this.objectTexture = ObjectTexture;
            this.defaultDrawFrame = DefaultFrame;
            this.enabled = EnabledByDefault;

            this.animationDataString = animationString;
            this.animations = parseAnimationsFromXNB(animationString);
            this.startingAnimationName = startingAnimationKey;
            if (this.animations.TryGetValue(startingAnimationKey, out this.currentAnimationList))
                this.setAnimation(startingAnimationKey, startingAnimationFrame);
            else
            {
                this.currentAnimation = this.defaultDrawFrame;
                this.currentAnimationName = "";
            }
        }

        public AnimationManager(Texture2DExtended ObjectTexture, Animation DefaultFrame, Dictionary<string, List<Animations.Animation>> animationString, string startingAnimationKey, int startingAnimationFrame = 0, bool EnabledByDefault = true)
        {
            this.currentAnimationListIndex = 0;
            this.objectTexture = ObjectTexture;
            this.defaultDrawFrame = DefaultFrame;
            this.enabled = EnabledByDefault;

            this.animations = animationString;
            this.startingAnimationName = startingAnimationKey;
            if (this.animations != null)
            {
                if (string.IsNullOrEmpty(startingAnimationKey) == false)
                {
                    if (this.animations.TryGetValue(startingAnimationKey, out this.currentAnimationList))
                    {
                        this.setAnimation(startingAnimationKey, startingAnimationFrame);
                        this.playAnimation(startingAnimationKey, true, startingAnimationFrame);
                    }

                    else
                    {
                        this.currentAnimation = this.defaultDrawFrame;
                        this.currentAnimationName = "";
                    }
                }
                else
                {
                    this.currentAnimation = this.defaultDrawFrame;
                    this.currentAnimationName = "";
                }
            }
            else
            {
                this.currentAnimation = this.defaultDrawFrame;
                this.currentAnimationName = "";
            }
        }

        /// <summary>Update the animation frame once after drawing the object.</summary>
        public void tickAnimation()
        {
            try
            {
                if (this.currentAnimation.frameDuration == -1 || !this.enabled || this.currentAnimation == this.defaultDrawFrame)
                    return; //This is if this is a default animation or the animation stops here.
                if (this.currentAnimation.finished())
                    this.getNextAnimationFrame();
                this.currentAnimation.tickAnimationFrame();
                //this.requiresUpdate = true;
                this.hasRecievedUpdateTick = true;
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log("An internal error occured when trying to tick the animation.");
                ModCore.ModMonitor.Log(err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }

        public void prepareForNextUpdateTick()
        {
            this.hasRecievedUpdateTick = false;
        }

        public bool canTickAnimation()
        {
            return this.hasRecievedUpdateTick == false;
        }

        /// <summary>Get the next animation frame in the list of animations.</summary>
        private void getNextAnimationFrame()
        {
            this.currentAnimationListIndex++;
            if (this.currentAnimationListIndex == this.currentAnimationList.Count)
            { //If the animation frame I'm tryting to get is 1 outside my list length, reset the list.
                if (this.loopAnimation)
                {
                    this.currentAnimationListIndex = 0;
                    this.currentAnimation = this.currentAnimationList[this.currentAnimationListIndex];
                    this.currentAnimation.startAnimation();
                    return;
                }
                else
                {
                    //this.requiresUpdate = true;
                    this.playDefaultAnimation();
                    return;
                }
            }

            //Get the next animation from the list and reset it's counter to the starting frame value.
            this.currentAnimation = this.currentAnimationList[this.currentAnimationListIndex];
            this.currentAnimation.startAnimation();
            //this.requiresUpdate = true;
        }

        /// <summary>Gets the animation from the dictionary of all animations available.</summary>
        /// <param name="AnimationName"></param>
        /// <param name="StartingFrame"></param>
        public bool setAnimation(string AnimationName, int StartingFrame = 0)
        {
            if (this.animations.TryGetValue(AnimationName, out List<Animation> dummyList))
            {
                if (dummyList.Count != 0 || StartingFrame >= dummyList.Count)
                {
                    this.currentAnimationList = dummyList;
                    this.currentAnimation = this.currentAnimationList[StartingFrame];
                    this.currentAnimationName = AnimationName;
                    this.requiresUpdate = true;
                    return true;
                }
                else
                {
                    if (dummyList.Count == 0)
                        ModCore.ModMonitor.Log("Error: Current animation " + AnimationName + " has no animation frames associated with it.");
                    if (dummyList.Count > dummyList.Count)
                        ModCore.ModMonitor.Log("Error: Animation frame " + StartingFrame + " is outside the range of provided animations. Which has a maximum count of " + dummyList.Count);
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
        /// Plays the animation for the animation manager.
        /// </summary>
        /// <param name="AnimationName"></param>
        /// <param name="overrideSameAnimation"></param>
        /// <param name="StartingFrame"></param>
        /// <returns></returns>
        public bool playAnimation(string AnimationName, bool overrideSameAnimation = false, int StartingFrame = 0)
        {
            if (this.animations.TryGetValue(AnimationName, out List<Animation> dummyList))
            {
                if (overrideSameAnimation == false)
                {
                    if (this.currentAnimationName == AnimationName) return true;
                }
                if (dummyList.Count != 0 || StartingFrame >= dummyList.Count)
                {
                    this.currentAnimationList = dummyList;
                    this.currentAnimation = this.currentAnimationList[StartingFrame];
                    this.currentAnimationName = AnimationName;
                    this.currentAnimation.startAnimation();
                    this.loopAnimation = true;
                    this.requiresUpdate = true;
                    return true;
                }
                else
                {
                    if (dummyList.Count == 0)
                        ModCore.ModMonitor.Log("Error: Current animation " + AnimationName + " has no animation frames associated with it.");
                    if (dummyList.Count > dummyList.Count)
                        ModCore.ModMonitor.Log("Error: Animation frame " + StartingFrame + " is outside the range of provided animations. Which has a maximum count of " + dummyList.Count);
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
        /// Plays the animation for the animation manager only once.
        /// </summary>
        /// <param name="AnimationName"></param>
        /// <param name="overrideSameAnimation"></param>
        /// <param name="StartingFrame"></param>
        /// <returns></returns>
        public bool playAnimationOnce(string AnimationName, bool overrideSameAnimation = false, int StartingFrame = 0)
        {
            if (this.animations.TryGetValue(AnimationName, out List<Animation> dummyList))
            {
                if (overrideSameAnimation == false)
                {
                    if (this.currentAnimationName == AnimationName) return true;
                }
                if (dummyList.Count != 0 || StartingFrame >= dummyList.Count)
                {
                    this.currentAnimationList = dummyList;
                    this.currentAnimation = this.currentAnimationList[StartingFrame];
                    this.currentAnimationName = AnimationName;
                    this.currentAnimation.startAnimation();
                    this.loopAnimation = false;
                    this.requiresUpdate = true;
                    return true;
                }
                else
                {
                    if (dummyList.Count == 0)
                        ModCore.ModMonitor.Log("Error: Current animation " + AnimationName + " has no animation frames associated with it.");
                    if (dummyList.Count > dummyList.Count)
                        ModCore.ModMonitor.Log("Error: Animation frame " + StartingFrame + " is outside the range of provided animations. Which has a maximum count of " + dummyList.Count);
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
            this.currentAnimation = this.defaultDrawFrame;
            this.currentAnimationName = "";
            this.currentAnimationListIndex = 0;
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

        public static Dictionary<string, List<Animation>> parseAnimationsFromXNB(string s)
        {
            string[] array = s.Split('*');
            Dictionary<string, List<Animation>> parsedDic = new Dictionary<string, List<Animation>>();
            foreach (string v in array)
            {
                // Log.AsyncC(v);
                string[] animationArray = v.Split(' ');
                if (parsedDic.ContainsKey(animationArray[0]))
                {
                    List<Animation> animations = parseAnimationFromString(v);
                    foreach (var animation in animations)
                    {
                        parsedDic[animationArray[0]].Add(animation);
                    }
                }
                else
                {
                    parsedDic.Add(animationArray[0], new List<Animation>());
                    List<Animation> aniList = new List<Animation>();
                    aniList = parseAnimationFromString(v);
                    foreach (var ani in aniList)
                    {
                        parsedDic[animationArray[0]].Add(ani);
                    }
                }
            }
            return parsedDic;
        }

        public static List<Animation> parseAnimationFromString(string s)
        {
            List<Animation> ok = new List<Animation>();
            string[] array2 = s.Split('>');
            foreach (string q in array2)
            {
                string[] array = q.Split(' ');
                try
                {
                    Animation ani = new Animation(new Rectangle(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4])), Convert.ToInt32(array[5]));
                    // ModCore.ModMonitor.Log(ani.sourceRectangle.ToString());
                    ok.Add(ani);
                }
                catch { }
            }
            return ok;
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
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
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
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
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
            b.Draw(this.objectTexture.texture, Position, this.currentAnimation.sourceRectangle, drawColor, 0f, Vector2.Zero, scale, flipped, depth);
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
        }

        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, float scale, float Rotation, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.currentAnimation.sourceRectangle, drawColor, Rotation, Vector2.Zero, scale, flipped, depth);
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
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
            b.Draw(this.objectTexture.texture, Position, this.currentAnimation.sourceRectangle, drawColor, 0f, Vector2.Zero, scale, flipped, depth);
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
        }

        public void draw(SpriteBatch b, Vector2 Position, Color drawColor, Vector2 scale, float Rotation, SpriteEffects flipped, float depth)
        {
            b.Draw(this.objectTexture.texture, Position, this.currentAnimation.sourceRectangle, drawColor, Rotation, Vector2.Zero, scale, flipped, depth);
            try
            {
                this.tickAnimation();
                // Log.AsyncC("Tick animation");
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
            }
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
            return this.objectTexture.getTexture();
        }

        public AnimationManager Copy()
        {
            return new AnimationManager(this.objectTexture, this.defaultDrawFrame, this.animations, this.startingAnimationName, 0, this.enabled);
        }
    }
}
