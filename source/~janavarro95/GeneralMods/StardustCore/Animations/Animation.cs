/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;

namespace StardustCore.Animations
{
    /// <summary>A custom class used to deal with custom animations/</summary>
    public class Animation
    {
        /// <summary>
        /// The list of animation frames present for this animation.
        /// </summary>
        public List<AnimationFrame> animationFrames;
        /// <summary>
        /// Should this animation loop around when finished?
        /// </summary>
        public bool shouldLoopAnimation;
        /// <summary>
        /// The current index of the <see cref="animationFrames"/> list.
        /// </summary>
        public int currentAnimationFrameIndex;

        [XmlIgnore]
        public NetFields NetFields { get; } = new NetFields();

        public Animation()
        {
        }

        public Animation(int sourceRectX, int sourceRectY, int sourceRectWidth, int sourceRectHeight):this(new Rectangle(sourceRectX,sourceRectY,sourceRectWidth, sourceRectHeight))
        {

        }

        public Animation(Rectangle SourceRectangleForStaticAnimation):this(new AnimationFrame(SourceRectangleForStaticAnimation))
        {

        }

        public Animation(AnimationFrame animationFrame, bool ShouldLoop = true):this(new List<AnimationFrame>() {animationFrame },ShouldLoop)
        {

        }

        public Animation(List<AnimationFrame> animationFrames, bool ShouldLoop, int CurrentFrameIndex=0)
        {
            this.animationFrames = animationFrames;
            this.shouldLoopAnimation = ShouldLoop;
            this.currentAnimationFrameIndex = CurrentFrameIndex;
        }

        /// <summary>
        /// Gets the current, or last played <see cref="AnimationFrame"/> from the list of <see cref="animationFrames"/>
        /// </summary>
        /// <returns></returns>
        public virtual AnimationFrame getCurrentAnimationFrame()
        {
            if (this.animationFrames.Count == 0) return null;

            if (this.currentAnimationFrameIndex >= this.animationFrames.Count) return this.animationFrames[this.animationFrames.Count - 1];

            return this.animationFrames[this.currentAnimationFrameIndex];
        }

        /// <summary>
        /// Gets the current <see cref="Rectangle"/> for the currently playing animation frame, or a zeroed out Rectangle.
        /// </summary>
        /// <returns></returns>
        public virtual Rectangle getCurrentAnimationFrameRectangle()
        {
            AnimationFrame currentAnimationFrame = this.getCurrentAnimationFrame();
            if (currentAnimationFrame == null) return default(Rectangle);

            return this.getCurrentAnimationFrame().sourceRectangle;
        }

        public virtual void reset()
        {
            //Reset old animation frame.
            this.getCurrentAnimationFrame().reset();
            //Reposition index to 0.
            this.currentAnimationFrameIndex = 0;
            //Reset new animation frame.
            this.getCurrentAnimationFrame().reset();
        }

        /// <summary>Decrements the amount of frames this animation is on the screen for by 1.</summary>
        public void tickAnimation()
        {
            this.getCurrentAnimationFrame().tickAnimationFrame();

            if (this.getCurrentAnimationFrame().isFinished())
            {
                this.currentAnimationFrameIndex++;
                if (this.currentAnimationFrameIndex >= this.animationFrames.Count && this.shouldLoopAnimation)
                {
                    this.reset();
                }
                else
                {
                    this.currentAnimationFrameIndex = this.animationFrames.Count; //Prevent out of bounds index exception.
                }
            }
        }

        /// <summary>This sets the animation frame count to be the max duration. I.E restart the timer.</summary>
        public void startAnimation()
        {
            this.currentAnimationFrameIndex = 0;
        }

        /// <summary>
        /// Is this animation finished playing?
        /// </summary>
        /// <returns></returns>
        public bool isFinished()
        {
            return this.currentAnimationFrameIndex == this.animationFrames.Count && this.getCurrentAnimationFrame().isFinished();
        }
    }
}
