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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using StardustCore.UIUtilities;

namespace StardustCore.Animations
{
    /// <summary>
    /// A class used to store animation information for a single frame.
    /// </summary>
    public class AnimationFrame
    {
        /// <summary>The source rectangle on the texture to display.</summary>
        public Rectangle sourceRectangle;

        /// <summary>The duration of the frame in length.</summary>
        public int frameDuration;

        /// <summary>The duration until the next frame.</summary>
        private int frameCountUntilNextAnimation;


        [XmlIgnore]
        public NetFields NetFields { get; } = new NetFields();

        public AnimationFrame()
        {
            this.sourceRectangle = new Rectangle(0, 0, 16, 16);
            this.frameCountUntilNextAnimation = -1;
            this.frameDuration = -1;
        }

        public AnimationFrame(int xPos, int yPos, int width, int height)
        {
            this.sourceRectangle = new Rectangle(xPos, yPos, width, height);
            this.frameCountUntilNextAnimation = -1;
            this.frameDuration = -1;
        }
        public AnimationFrame(int xPos, int yPos, int width, int height, int existsForXFrames)
        {
            this.sourceRectangle = new Rectangle(xPos, yPos, width, height);
            this.frameDuration = existsForXFrames;
            this.frameCountUntilNextAnimation = this.frameDuration;
        }

        /// <summary>Constructor that causes the animation frame count to be set to -1; This forces it to never change.</summary>
        /// <param name="SourceRectangle">The draw source for this animation.</param>
        public AnimationFrame(Rectangle SourceRectangle)
        {
            this.sourceRectangle = SourceRectangle;
            this.frameCountUntilNextAnimation = -1;
            this.frameDuration = -1;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="SourceRectangle">The draw source for this animation.</param>
        /// <param name="existForXFrames">How many on screen frames this animation stays for. Every draw frame decrements an active animation by 1 frame. Set this to -1 to have it be on the screen infinitely.</param>
        public AnimationFrame(Rectangle SourceRectangle, int existForXFrames)
        {
            this.sourceRectangle = SourceRectangle;
            this.frameDuration = existForXFrames;
            this.frameCountUntilNextAnimation = this.frameDuration;
        }

        /// <summary>Decrements the amount of frames this animation is on the screen for by 1.</summary>
        public void tickAnimationFrame()
        {
            if (this.frameDuration == -1) return;
            this.frameCountUntilNextAnimation--;
        }

        /// <summary>This sets the animation frame count to be the max duration. I.E restart the timer.</summary>
        public void startAnimation()
        {
            this.frameCountUntilNextAnimation = this.frameDuration;
        }
        public bool isFinished()
        {
            if (this.frameDuration == -1) return true;
            return this.frameCountUntilNextAnimation == 0;
        }

        /// <summary>
        /// Resets the animation frame.
        /// </summary>
        public void reset()
        {
            this.frameCountUntilNextAnimation = this.frameDuration;
        }
    }
}
