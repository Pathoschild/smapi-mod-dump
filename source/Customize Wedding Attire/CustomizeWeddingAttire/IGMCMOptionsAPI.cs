/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire
**
*************************************************/

// See https://github.com/jltaylor-us/StardewGMCMOptions/blob/default/StardewGMCMOptions/IGMCMOptionsAPI.cs for latest

// Copyright 2022 Jamie Taylor
//
// To facilitate other mods which would like to use the GMCMOptions API,
// the license for this file (and only this file) is modified by removing the
// notice requirements for binary distribution.  The license (as amended)
// is included below, making this file self-contained.
//
// In other words, anyone may copy this file into their own mod.
//

//  Copyright(c) 2022, Jamie Taylor
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met:
//
//1.Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
//2. [condition removed for this file]
//
//3. Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CustomizeWeddingAttire {
    public interface IGMCMOptionsAPI {
        /// <summary>
        /// Add an image picker option.  This is really an "array index picker" where you can specify what to draw
        /// for each index.  The underlying value is always a <c>uint</c> (the index).
        /// </summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="getMaxValue">
        ///   The maximum value this option can have, and thus the maximum value that will be passed to
        ///   <paramref name="drawImage"/> and <paramref name="label"/>.  Note that this is a function, so
        ///   theoretically the number of options does not have to be fixed.  Should this function return a
        ///   value greater than the option's current value then the option's current value will be clamped.
        ///   In common usage, this parameter should be a function that returns one less than the number
        ///   of images.
        /// </param>
        /// <param name="maxImageHeight">
        ///   A function that returns the maximum image height.  Used to report the option's height to GMCM (which
        ///   will not recompute how much space to reserve for the option until the page is re-opened) and to center
        ///   arrows vertically in the <c cref="ImageOptionArrowLocation.Sides">Sides</c> arrow placement option.
        /// </param>
        /// <param name="maxImageWidth">
        ///   A function that returns the maximum image width.  This is used to place the arrows and label.
        /// </param>
        /// <param name="drawImage">A function which draws the image for the given index at the given location</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="label">A function to return the string to display given the image index, or <c>null</c> to disable that display.</param>
        /// <param name="arrowLocation">Where to render the arrows.  Use a value from the <c cref="ImageOptionArrowLocation">ImageOptionArrowLocation</c> enum.</param>
        /// <param name="labelLocation">Where to render the label.  Use a value from the <c cref="ImageOptionLabelLocation">ImageOptionLabelLocation</c> enum.</param>
        /// <param name="fieldId">The unique field ID for use with GMCM's <c>OnFieldChanged</c>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddImageOption(IManifest mod,
                            Func<uint> getValue,
                            Action<uint> setValue,
                            Func<string> name,
                            Func<uint> getMaxValue,
                            Func<int> maxImageHeight,
                            Func<int> maxImageWidth,
                            Action<uint, SpriteBatch, Vector2> drawImage,
                            Func<string> tooltip = null,
                            Func<uint, String> label = null,
                            int arrowLocation = (int)ImageOptionArrowLocation.Top,
                            int labelLocation = (int)ImageOptionLabelLocation.Top,
                            string fieldId = null);

        /// <summary>
        /// Valid values for the <c>arrowLocation</c> parameter of <c>AddImageOption</c>
        /// </summary>
        public enum ImageOptionArrowLocation {
            Top = -1,
            Sides = 0,
            Bottom = 1
        }
        /// <summary>
        /// Valid values for the <c>labelLocation</c> parameter of <c>AddImageOption</c>
        /// </summary>
        public enum ImageOptionLabelLocation {
            Top = -1,
            None = 0,
            Bottom = 1
        }

        /// <summary>Add a <c cref="Color">Color</c> option at the current position in the GMCM form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to show in the form.</param>
        /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
        /// <param name="showAlpha">Whether the color picker should allow setting the Alpha channel</param>
        /// <param name="colorPickerStyle">Flags to control how the color picker is rendered.  <see cref="ColorPickerStyle"/></param>
        /// <param name="fieldId">The unique field ID for use with GMCM's <c>OnFieldChanged</c>, or <c>null</c> to auto-generate a randomized ID.</param>
        void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string>? tooltip = null, bool showAlpha = true, uint colorPickerStyle = 0, string? fieldId = null);


    }

}
