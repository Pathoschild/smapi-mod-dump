using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StardustCore.Animations
{
    /// <summary>
    /// A custom class used to deal with custom animations/
    /// </summary>
   public class Animation
    {
       /// <summary>
       /// The source rectangle on the texture to display.
       /// </summary>
       public Rectangle sourceRectangle;
       /// <summary>
       /// The duration of the frame in length.
       /// </summary>
       public int frameDuration;
       /// <summary>
       /// The duration until the next frame.
       /// </summary>
       public int frameCountUntilNextAnimation;

       [XmlIgnore]
       public NetFields NetFields { get; } = new NetFields();

        public Animation()
        {
            this.sourceRectangle = new Rectangle(0,0,16,16);
            this.frameCountUntilNextAnimation = -1;
            this.frameDuration = -1;
        }

        /// <summary>
        /// Constructor that causes the animation frame count to be set to -1; This forces it to never change.
        /// </summary>
        /// <param name="SourceRectangle">The draw source for this animation.</param>
        public Animation(Rectangle SourceRectangle)
        {
            sourceRectangle = SourceRectangle;
            this.frameCountUntilNextAnimation = -1;
            frameDuration = -1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="SourceRectangle">The draw source for this animation.</param>
        /// <param name="existForXFrames">How many on screen frames this animation stays for. Every draw frame decrements an active animation by 1 frame. Set this to -1 to have it be on the screen infinitely.</param>
        public Animation(Rectangle SourceRectangle,int existForXFrames)
        {
            sourceRectangle = SourceRectangle;
            frameDuration = existForXFrames;
        }

        /// <summary>
        /// Decrements the amount of frames this animation is on the screen for by 1.
        /// </summary>
        public void tickAnimationFrame()
        {
            frameCountUntilNextAnimation--;
        }

        /// <summary>
        /// This sets the animation frame count to be the max duration. I.E restart the timer.
        /// </summary>
        public void startAnimation()
        {
            frameCountUntilNextAnimation = frameDuration;
        }


    }
}
