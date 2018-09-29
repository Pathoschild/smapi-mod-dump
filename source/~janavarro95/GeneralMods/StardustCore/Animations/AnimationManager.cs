using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Animations
{
    /// <summary>
    /// Used to play animations for Stardust.CoreObject type objects and all objects that extend from it. In draw code of object make sure to use this info instead.
    /// </summary>
   public class AnimationManager
    {
       public Dictionary<string, List<Animation>> animations = new Dictionary<string, List<Animation>>();
       public string currentAnimationName;
       public int currentAnimationListIndex;
       public List<Animation> currentAnimationList = new List<Animation>();
       public Texture2DExtended objectTexture; ///Might not be necessary if I use the CoreObject texture sheet.
       public Animation defaultDrawFrame;
       public Animation currentAnimation;
       bool enabled;

        /// <summary>
        /// Constructor for Animation Manager class.
        /// </summary>
        /// <param name="ObjectTexture">The texture that will be used for the animation. This is typically the same as the object this class is attached to.</param>
        /// <param name="DefaultFrame">This is used if no animations will be available to the animation manager.</param>
        /// <param name="EnabledByDefault">Whether or not animations play by default. Default value is true.</param>
        public AnimationManager (Texture2DExtended ObjectTexture,Animation DefaultFrame, bool EnabledByDefault=true)
        {
            currentAnimationListIndex = 0;
            this.objectTexture = ObjectTexture;
            this.defaultDrawFrame = DefaultFrame;
            this.enabled = EnabledByDefault;
            currentAnimation = this.defaultDrawFrame;
        }
    
        public AnimationManager(Texture2DExtended ObjectTexture,Animation DefaultFrame ,Dictionary<string, List<Animation>> animationsToPlay, string startingAnimationKey, int startingAnimationFrame=0,bool EnabledByDefault=true)
        {
            currentAnimationListIndex = 0;
            this.objectTexture = ObjectTexture;
            this.defaultDrawFrame = DefaultFrame;
            this.enabled = EnabledByDefault;

            this.animations = animationsToPlay;
            bool f = animations.TryGetValue(startingAnimationKey, out currentAnimationList);
            if (f == true) {
                setAnimation(startingAnimationKey, startingAnimationFrame);
            }
            else currentAnimation = this.defaultDrawFrame;
        }

        /// <summary>
        /// Update the animation frame once after drawing the object.
        /// </summary>
        public void tickAnimation()
        {
            try
            {
                if (this.currentAnimation.frameDuration == -1 || this.enabled == false || this.currentAnimation == this.defaultDrawFrame) return; //This is if this is a default animation or the animation stops here.
                if (this.currentAnimation.frameCountUntilNextAnimation == 0) getNextAnimation();
                this.currentAnimation.tickAnimationFrame();
            }
            catch(Exception err)
            {
                ModCore.ModMonitor.Log("An internal error occured when trying to tick the animation.");
                ModCore.ModMonitor.Log(err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
        }

        /// <summary>
        /// Get the next animation in the list of animations.
        /// </summary>
        public void getNextAnimation()
        {
            currentAnimationListIndex++;
           if(currentAnimationListIndex==currentAnimationList.Count) //If the animation frame I'm tryting to get is 1 outside my list length, reset the list.
            {
                currentAnimationListIndex = 0;
            }
           
           //Get the next animation from the list and reset it's counter to the starting frame value.
           this.currentAnimation = currentAnimationList[currentAnimationListIndex];
           this.currentAnimation.startAnimation();
        }

        /// <summary>
        /// Gets the animation from the dictionary of all animations available.
        /// </summary>
        /// <param name="AnimationName"></param>
        /// <param name="StartingFrame"></param>
        /// <returns></returns>
        public bool setAnimation(string AnimationName, int StartingFrame=0)
        {
            List<Animation> dummyList = new List<Animation>();
            bool f = animations.TryGetValue(AnimationName, out dummyList);
            if (f == true)
            {
                if (dummyList.Count != 0 || StartingFrame>=dummyList.Count)
                {
                    currentAnimationList = dummyList;
                    currentAnimation = currentAnimationList[StartingFrame];
                    currentAnimationName = AnimationName;
                    return true;
                }
                else
                {
                  if(dummyList.Count==0) ModCore.ModMonitor.Log("Error: Current animation " + AnimationName+ " has no animation frames associated with it.");
                  if (dummyList.Count > dummyList.Count) ModCore.ModMonitor.Log("Error: Animation frame "+ StartingFrame+ " is outside the range of provided animations. Which has a maximum count of "+ dummyList.Count);
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
        /// Sets the animation manager to an on state, meaning that this animation will update on the draw frame.
        /// </summary>
        public void enableAnimation()
        {
            this.enabled = true;
        }

        /// <summary>
        /// Sets the animation manager to an off state, meaning that this animation will no longer update on the draw frame.
        /// </summary>
        public void disableAnimation()
        {
            this.enabled = false;
        }

        public static Dictionary<string, List<Animation>> parseAnimationsFromXNB(string s)
        {
            string[] array = s.Split('*');
            Dictionary<string,List<Animation>> parsedDic = new Dictionary<string, List<Animation>>();
            foreach(var v in array)
            {
               // Log.AsyncC(v);
                string[] AnimationArray = v.Split(' ');
                if (parsedDic.ContainsKey(AnimationArray[0]))
                {
                    List<Animation> aniList = new List<Animation>();
                    aniList = parseAnimationFromString(v);
                    foreach(var ani in aniList) {
                        parsedDic[AnimationArray[0]].Add(ani);
                    }
                    
                }
                else
                {
                    parsedDic.Add(AnimationArray[0], new List<Animation>());
                    List<Animation> aniList = new List<Animation>();
                    aniList = parseAnimationFromString(v);
                    foreach (var ani in aniList)
                    {
                        parsedDic[AnimationArray[0]].Add(ani);
                    }
                }
            }
            return parsedDic;
        }

        public static List<Animation> parseAnimationFromString(string s)
        {
            List<Animation> ok = new List<Animation>(); 
            string[] array2 = s.Split('>');
            foreach(var q in array2) {
                string[] array = q.Split(' ');
                try
                {
                    Animation ani = new Animation(new Rectangle(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4])), Convert.ToInt32(array[5]));
                   // ModCore.ModMonitor.Log(ani.sourceRectangle.ToString());
                    ok.Add(ani);
                }
                catch(Exception err)
                {
                    err.ToString();
                    continue;
                }
            
            }
            return ok;
        }
        /// <summary>
        /// Used to handle general drawing functionality using the animation manager.
        /// </summary>
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
        public void draw(SpriteBatch spriteBatch,Texture2D texture, Vector2 Position, Rectangle? sourceRectangle,Color drawColor, float rotation, Vector2 origin, float scale,SpriteEffects spriteEffects, float LayerDepth)
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

    }
}
