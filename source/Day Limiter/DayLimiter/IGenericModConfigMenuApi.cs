/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tadfoster/StardewValleyMods
**
*************************************************/

// Copyright 2022-2023 Jamie Taylor
//
// To facilitate other mods which would like to use the GMCMOptions API,
// the license for this file (and only this file) is modified by removing the
// notice requirements for binary distribution.  The license (as amended)
// is included below, making this file self-contained.
//
// In other words, anyone may copy this file into their own mod (and edit
// it if they want, e.g. to remove the methods they are not using, so long
// as the license comment is retained).(If all you want in your mod is
// _just_ the function declaration(s) and not any comments or other creative
// expression that may be in the file, then that is permissible fair use
// as a matter of law in the US according to Google v. Oracle, 593 U.S. ___ (2021),
// and does not require any license.)
//

//  Copyright(c) 2022-2023, Jamie Taylor
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

namespace GMCMOptions
{
    /// <summary>The API which lets other mods add a config UI using one of the complex options defined in GMCMOptions.</summary>
    public interface IGMCMOptionsAPI
    {
        /// <summary>
        ///   Add a dynamic paragraph.  A dynamic paragraph reflects changes in the text returned by
        ///   <paramref name="text"/> even while the GMCM window is open.  It also supports styled text.
        ///   <para>
        ///     Styled text supports simple HTML-like markup for specifying text formatting.  The text must
        ///     be valid XML fragment(s).  (I.e., if the text were enclosed in an XML tag, the result must be
        ///     a valid XML document.)  See https://github.com/jltaylor-us/StardewGMCMOptions/blob/default/README.md#dynamic-paragraph
        ///     for details about the tags supported.
        ///   </para>
        /// </summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="logName">
        ///   A name to identify <em>this</em> dynamic paragraph in the SMAPI log, should there be any errors in
        ///   the text returned by <paramref name="text"/>.  This string may appear in the log, but will not appear
        ///   in-game.
        /// </param>
        /// <param name="text">The paragraph text.</param>
        /// <param name="isStyledText">
        ///   If <c>true</c>, then the text returned by <paramref name="text"/> will be treated as styled text.
        /// </param>
        void AddDynamicParagraph(IManifest mod,
                                 string logName,
                                 Func<string> text,
                                 bool isStyledText);
    }

}