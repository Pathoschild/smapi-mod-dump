/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AeroCore.Models
{
    public class Animation
    {
        public Rectangle Region { get; set; }
        public int BaseDelay
        {
            get => baseDelay;
            set => baseDelay = Math.Max(value, 1);
        }
        private int baseDelay = 1;
        public List<int> FrameDelay { get; set; }
        public int HFrames { get; set; }
        public int FrameCount { get; set; }
        public int LoopStart
        {
            get => loopStart;
            set => loopStart = Math.Max(value, 1);
        }
        private int loopStart = 1; // 1-indexed to be more user friendly
        public int CurrentFrame { get; private set; } = 0;
        public Rectangle FrameRegion { get; private set; }
        public bool Stopped => currentDelay == 0;

        private int frameTime = 0;
        private int currentDelay = 1;

        public void Reset()
        {
            CurrentFrame = 0;
            frameTime = 0;
            currentDelay = FrameDelay.Count > 0 && FrameDelay[0] >= 0 ? FrameDelay[0] : baseDelay;
        }
        /// <param name="elapsed">Elapsed time since last frame, in milliseconds.</param>
        public Rectangle Animate(int elapsed)
        {
            frameTime += elapsed;
            var prevFrame = CurrentFrame;
            while(frameTime >= currentDelay && currentDelay > 0)
            {
                frameTime -= currentDelay;
                CurrentFrame++;
                if (CurrentFrame >= FrameCount)
                    CurrentFrame = LoopStart > FrameCount ? 0 : LoopStart - 1;
                currentDelay = FrameDelay.Count > CurrentFrame && FrameDelay[CurrentFrame] >= 0 ? FrameDelay[CurrentFrame] : baseDelay;
            }
            if (CurrentFrame != prevFrame)
                FrameRegion = new(Region.X + CurrentFrame % HFrames, Region.Y + CurrentFrame / HFrames, Region.Width, Region.Height);
            return FrameRegion;
        }
        public bool FitsWithin(Rectangle bounds)
            => bounds.Width >= Region.X + HFrames * Region.Width && 
            bounds.Height >= Region.Y + (FrameCount / HFrames + (FrameCount % HFrames > 0 ? 1 : 0)) * Region.Height;
        public Rectangle GetBounds()
            => new(
                Region.X, 
                Region.Y, 
                Region.Width * HFrames, 
                Region.Height * (FrameCount / HFrames + (FrameCount % HFrames > 0 ? 1 : 0))
            );
    }
}
