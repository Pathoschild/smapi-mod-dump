/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

/*
MIT License

Copyright (c) 2020 Felicia Hummel

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// Modified version of Drachenkaetzchen's fps.cs
// See link for original source code: https://github.com/Drachenkaetzchen/StardewScript/blob/fc41926c9f896473fe0c11fabd912c37249c75cc/StardewScript/Examples/fps.cs

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace AlternativeTextures.Framework.Utilities
{
    class FpsCounter
    {
        /// <summary>
        /// The number of frames rendered since _lastRenderingCall
        /// </summary>
        private int _numFramesRendered = 0;

        /// <summary>
        /// The rendering time in milliseconds since _lastRenderingCall
        /// </summary>
        private double _frameRenderingTime = 0;

        /// <summary>
        /// The currently displayed FPS string
        /// </summary>
        private string _fpsString = "-";

        /// <summary>
        /// The date and time when the last rendering call was invoked.
        /// </summary>
        private DateTime _lastRenderingCall = DateTime.UtcNow;

        /// <summary>
        /// The date and time when the FPS string was last updated.
        /// </summary>
        private DateTime _lastFPSUpdate = DateTime.UtcNow;


        internal void OnRendered(object sender, RenderedEventArgs e)
        {
            var millisecondsSinceLastCall = DateTime.UtcNow.Subtract(_lastRenderingCall).TotalMilliseconds;
            _lastRenderingCall = DateTime.UtcNow;

            _numFramesRendered++;
            _frameRenderingTime += millisecondsSinceLastCall;

            // Check if the last FPS update was more than a second ago. If so,
            // recalculate the FPS and update the FPS string.
            if (DateTime.UtcNow.Subtract(_lastFPSUpdate).TotalSeconds >= 1)
            {
                _lastFPSUpdate = DateTime.UtcNow;

                var averageRenderingTimePerFrame = _frameRenderingTime / _numFramesRendered;
                _fpsString = $"{(1000 / averageRenderingTimePerFrame):F0}";


                _frameRenderingTime = 0;
                _numFramesRendered = 0;
            }

            Utility.drawTextWithColoredShadow(e.SpriteBatch, $"{_fpsString} FPS", Game1.smallFont, new Vector2(10, 10), Color.LawnGreen, Color.Black, 1);
        }
    }
}
