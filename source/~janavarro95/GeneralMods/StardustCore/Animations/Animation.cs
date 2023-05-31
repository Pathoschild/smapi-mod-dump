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
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;

namespace Omegasis.StardustCore.Animations
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
            this.animationFrames = new List<AnimationFrame>();
        }

        public Animation(int sourceRectX, int sourceRectY, int sourceRectWidth, int sourceRectHeight, float scale=1f):this(new Rectangle(sourceRectX,sourceRectY,sourceRectWidth, sourceRectHeight),scale)
        {

        }

        public Animation(Rectangle SourceRectangleForStaticAnimation, float scale=1f):this(new AnimationFrame(SourceRectangleForStaticAnimation,scale))
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

            if (this.currentAnimationFrameIndex >= this.animationFrames.Count)
            {

                return this.animationFrames[this.animationFrames.Count - 1];
            }

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

        public virtual Animation appendAnimation(Animation other)
        {
            this.animationFrames.AddRange(other.animationFrames);
            return this;
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
                //Reset old animation frame.
                this.getCurrentAnimationFrame().reset();
                //Move to the next frame.
                this.currentAnimationFrameIndex++;
                //Reset the new frame just in case.
                this.getCurrentAnimationFrame().reset();
                if (this.currentAnimationFrameIndex >= this.animationFrames.Count && this.shouldLoopAnimation)
                {
                    this.reset();
                }
            }
        }

        /// <summary>This sets the animation frame count to be the max duration. I.E restart the timer.</summary>
        public void startAnimation(int StartingFrameIndex=0)
        {
            this.currentAnimationFrameIndex = StartingFrameIndex;
        }

        /// <summary>
        /// Is this animation finished playing?
        /// </summary>
        /// <returns></returns>
        public bool isFinished()
        {
            return this.currentAnimationFrameIndex == this.animationFrames.Count && this.getCurrentAnimationFrame().isFinished();
        }

        public virtual Animation readAnimation(BinaryReader reader)
        {
            int animationFramesCount = reader.ReadInt32();
            if (this.animationFrames == null)
            {
                this.animationFrames = new List<AnimationFrame>();
            }

            this.animationFrames.Clear();
            for (int i = 0; i < animationFramesCount; i++)
            {
                AnimationFrame frame = new AnimationFrame();
                this.animationFrames.Add(frame.readAnimationFrame(reader));
            }

            this.shouldLoopAnimation = reader.ReadBoolean();
            this.currentAnimationFrameIndex = reader.ReadInt32();
            return this;
        }

        public virtual void writeAnimation(BinaryWriter writer)
        {
            writer.Write(this.animationFrames.Count);
            foreach(AnimationFrame frame in this.animationFrames)
            {
                frame.writeAnimationFrame(writer);
            }
            writer.Write(this.shouldLoopAnimation);

            //Maybe exclude this?
            writer.Write(this.currentAnimationFrameIndex);
        }

        public virtual Animation Copy()
        {
            List<AnimationFrame> copyFrames = new List<AnimationFrame>();

            foreach(AnimationFrame frame in this.animationFrames)
            {
                copyFrames.Add(frame.Copy());
            }

            return new Animation(copyFrames, this.shouldLoopAnimation, 0);

        }

        /// <summary>
        /// Creates a list of animation frames starting at a given position and generates the frame position data from left to right.
        /// </summary>
        /// <param name="startingPosX">The starting x position on the texture.</param>
        /// <param name="startingPosY">The starting Y position on the texture.</param>
        /// <param name="FrameWidth">The width of a given frame/</param>
        /// <param name="FrameHeight">The height of a given frame.</param>
        /// <param name="NumberOfFrames">The number of frames. Must be 1 or greater!</param>
        /// <returns></returns>
        public static Animation CreateAnimationFromTextureSequence(int startingPosX, int startingPosY, int FrameWidth, int FrameHeight, int NumberOfFrames, int ExistsForXFrames=-1, bool shouldLoop=true)
        {
            List<AnimationFrame> frames=new List<AnimationFrame>();

            for(int i=0; i < NumberOfFrames; i++)
            {
                AnimationFrame frame = new AnimationFrame(startingPosX + (FrameWidth * i), startingPosY, FrameWidth, FrameHeight, ExistsForXFrames);

                frames.Add(frame);
            }
            return new Animation(frames,shouldLoop);
        }

        public static Animation CreateAnimationFromReverseTextureSequence(int startingPosX, int startingPosY, int FrameWidth, int FrameHeight, int NumberOfFrames, int ExistsForXFrames = -1, bool shouldLoop = true)
        {
            List<AnimationFrame> frames = new List<AnimationFrame>();

            for (int i = 0; i < NumberOfFrames; i++)
            {
                AnimationFrame frame = new AnimationFrame(startingPosX + (FrameWidth * i), startingPosY, FrameWidth, FrameHeight, ExistsForXFrames);

                frames.Add(frame);
            }
            frames.Reverse();
            return new Animation(frames, shouldLoop);
        }
    }
}
