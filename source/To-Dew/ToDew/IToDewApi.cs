/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2023 Jamie Taylor
//
// To facilitate other mods which would like to use the To-Dew API,
// the license for this file (and only this file) is modified by removing the
// notice requirements for binary distribution.  The license (as amended)
// is included below, making this file self-contained.
//
// In other words, anyone may copy this file into their own mod (and edit
// it if they want, e.g. to remove the methods they are not using, so long
// as the license comment is retained).  (If all you want in your mod is
// _just_ the function declarations and not any comments or other creative
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
using System.Collections.Generic;

namespace ToDew {
    /// <summary>
    /// The main interface for the ToDew API.
    /// 
    /// <para>
    ///   The API currently contains definitions related to just one bit
    ///   of functionality:  the ability to provide items to display in
    ///   the overlay.  The overlay can display items from multiple data
    ///   sources; the to-do list mainained by To-Dew is just one data
    ///   source.
    /// </para>
    /// </summary>
    public interface IToDewApi {

        /// <summary>
        ///   Add a source for items to display in the overlay.
        /// </summary>
        /// <param name="src">The data source</param>
        /// <returns>
        ///   A handle by which the data source may be removed later
        ///   (via <c cref="RemoveOverlayDataSource(uint)">RemoveOverlayDataSource)</c>
        /// </returns>
        /// <seealso cref="IToDewOverlayDataSource"/>
        uint AddOverlayDataSource(IToDewOverlayDataSource src);

        /// <summary>
        ///   Remove a previously added data source.
        /// </summary>
        /// <param name="handle">
        ///   The handle for the data source to remove, as returned from
        ///   <c cref="AddOverlayDataSource(IToDewOverlayDataSource)">AddOverlayDataSource</c>
        /// </param>
        void RemoveOverlayDataSource(uint handle);

        /// <summary>
        ///   Tell the overlay that it should refresh its data.  This must
        ///   be called for changes in the items returned by a data source
        ///   to be visible in the overlay.  It is automatically called when
        ///   a data source is added or removed.
        /// </summary>
        void RefreshOverlay();
    }

    /// <summary>
    /// A data source for the To-Dew overlay.
    /// </summary>
    public interface IToDewOverlayDataSource {

        /// <summary>
        ///   Return the section title for this data source.  If there are any
        ///   items returned by <c cref="GetItems(int)">GetItems</c> then the
        ///   section title will be shown with an underline, and the items will
        ///   appear beneath it.
        /// </summary>
        /// <returns>the section title for this data source</returns>
        string GetSectionTitle();

        /// <summary>
        ///   Return a list of items to display in the overlay.
        ///
        /// <para>
        ///   Each item is a tuple containing the text to be displayed, whether
        ///   that text should be rendered in bold face, and an optional action
        ///   to take when that item's "done" button is clicked.  If <c>onDone</c>
        ///   is <c>null</c> then no "done" button will be displayed when hovering
        ///   over the item.  Remember to call <c cref="IToDewApi.RefreshOverlay">RefreshOverlay</c>
        ///   at the end of the action if it changes the value returned from
        ///   <c cref="GetItems(int)">GetItems</c>.
        /// </para>
        /// <para>
        ///   The <paramref name="limit"/> argument provides a hint about the
        ///   number of items to return in the list.  The returned list can
        ///   contain more than <paramref name="limit"/> items, but additional
        ///   items will not be displayed in the overlay.
        /// </para>
        /// </summary>
        /// <param name="limit">a hint about the maximum number of items to include in the list</param>
        /// <returns>
        ///   A list of items to display in the overlay.  Each item is a tuple
        ///   containing the text to be displayed, whether
        ///   that text should be rendered in bold face, and an optional action
        ///   to take when that item's "done" button is clicked.
        /// </returns>
        List<(string text, bool isBold, Action? onDone)> GetItems(int limit);
    }
}

