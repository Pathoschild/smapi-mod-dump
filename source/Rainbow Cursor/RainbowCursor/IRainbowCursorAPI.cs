/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRainbowCursor
**
*************************************************/

// Copyright 2023 Jamie Taylor
//
// To facilitate other mods which would like to use the Rainbow Cursor API,
// the license for this file (and only this file) is modified by removing the
// notice requirements for binary distribution.  The license (as amended)
// is included below, making this file self-contained.
//
// In other words, anyone may copy this file into their own mod (and edit
// it if they want, e.g. to remove the methods they are not using, so long
// as the license comment is retained).  (If all you want in your mod is
// _just_ the function declaration and not any comments or other creative
// expression that may be in the file, then that is permissible fair use
// as a matter of law in the US according to Google v. Oracle, 593 U.S. ___ (2021),
// and does not require any license.)
//

//  Copyright(c) 2023, Jamie Taylor
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
using StardewModdingAPI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RainbowCursor {
    public interface IRainbowCursorAPI {
        /// <summary>
        ///   Add a color palette to Rainbow Cursors.
        ///
        ///   The color palette will be available for the user to choose in Rainbow Cursor's
        ///   GMCM menu.
        /// </summary>
        /// <param name="id">
        ///   The unique ID for this color palette.  Best practice is for this to start with
        ///   your mod's unique ID.
        /// </param>
        /// <param name="getName">
        ///   A function that returns the name to be displayed for this color palette.
        /// </param>
        /// <param name="colors">The colors in this palette.</param>
        /// <param name="getTitle">
        ///   A function to return a string to use as the tooltip title when hovering over the
        ///   palette image in the GMCM configuration menu (if GMCM Options is installed).
        ///   A <c>null</c> value or returning a <c>null</c> string will fall back to the value returned by
        ///   <paramref name="getName"/>, or the empty string if that is null.  Whether the tooltip is displayed
        ///   is controlled by the <paramref name="getDescription"/> parameter.
        /// </param>
        /// <param name="getDescription">
        ///   A function to return the string to use as the tooltip text when hovering over the
        ///   palette image in the GMCM configuration menu (if GMCM Options is installed).
        ///   A <c>null</c> value or returning a <c>null</c> string disables the tooltip.
        /// </param>
        void AddColorPalette(string id,
                             Func<string> getName,
                             List<Color> colors,
                             Func<string?>? getTitle = null,
                             Func<string?>? getDescription = null);
    }
}

