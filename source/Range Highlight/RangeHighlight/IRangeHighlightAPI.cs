// Copyright 2020 Jamie Taylor
//
// To facilitate other mods which would like to use the RangeHighlight API,
// the license for this file (and only this file) is modified by removing the
// notice requirements for binary distribution.  The license (as amended)
// is included below, making this file self-contained.
//
// In other words, anyone may copy this file into their own mod.
//

//  Copyright(c) 2020, Jamie Taylor
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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace RangeHighlight {
    public interface IRangeHighlightAPI {

        // ----- Helpers for making highlight shapes -----

        /// <summary>
        ///   An deprecated alias for <c>GetCartesianCircleWithTruncate</c>.
        /// </summary>
        /// <param name="radius">The circle radius</param>
        /// <param name="excludeCenter">whether the center tile should be excluded</param>
        [Obsolete("GetCartesianCircle is deprecated.  Use GetCartesianCircleWithTruncate instead.")]
        bool[,] GetCartesianCircle(uint radius, bool excludeCenter = true);

        /// <summary>
        ///   Return a circle shape (i.e., the shape of a scarecrow's range),
        ///   where a tile's distance from the center is truncated to an integer
        ///   before comparing against the radius
        /// </summary>
        /// <param name="radius">The circle radius</param>
        /// <param name="excludeCenter">whether the center tile should be excluded</param>
        bool[,] GetCartesianCircleWithTruncate(uint radius, bool excludeCenter = true);

        /// <summary>
        ///   Return a more accurate circle shape (i.e., the shape of a bomb's range),
        ///   where a tile's distance from the center is rounded to the nearest integer
        ///   before comparing against the radius
        /// </summary>
        /// <param name="radius">The circle radius</param>
        /// <param name="excludeCenter">whether the center tile should be excluded</param>
        bool[,] GetCartesianCircleWithRound(uint radius, bool excludeCenter = true);

        /// <summary>
        ///   Return a "circle" using Manhattan distance from the center (i.e., the shape of a beehouse's range)
        /// </summary>
        /// <param name="radius">The Manhattan distance from the center to be included in the shape</param>
        /// <param name="excludeCenter">whether the center tile should be excluded</param>
        bool[,] GetManhattanCircle(uint radius, bool excludeCenter = true);

        /// <summary>
        ///   Return a square with each side  2 * radius + 1 tiles (i.e., the shape of a sprinkler's range)
        /// </summary>
        /// <param name="radius">The x or y distance from the center to be included in the shape</param>
        /// <param name="excludeCenter">whether the center tile should be excluded</param>
        bool[,] GetSquareCircle(uint radius, bool excludeCenter = true);

        // ----- Getters for the currently configured tint colors ----

        /// <summary>Returns the currently configured tint for Junimo hut range highlighting</summary>
        Color GetJunimoRangeTint();
        /// <summary>Returns the currently configured tint for sprinkler range highlighting</summary>
        Color GetSprinklerRangeTint();
        /// <summary>Returns the currently configured tint for scarecrow range highlighting</summary>
        Color GetScarecrowRangeTint();
        /// <summary>Returns the currently configured tint for beehouse range highlighting</summary>
        Color GetBeehouseRangeTint();

        // ----- Hooks for applying highlights ----

        /// <summary>Add a highlighter for buildings.</summary>
        /// <param name="uniqueId">
        ///   An ID by which the highlighter can be removed later.
        ///   Best practice is for it to contain your mod's unique ID.
        /// </param>
        /// <param name="hotkey">Also apply the highlighter when this key is held</param>
        /// <param name="highlighter">
        ///   A function that evaluates whether the <c>Building</c> matches
        ///   this highlighter, and if so returns a <c>Tuple</c> containing the tint
        ///   color, highlight shape, and x and y offset for the building "center".
        ///   If the building does not match then
        ///   the function should return <c>null</c>.  (Note that returning an
        ///   empty <c>bool[,]</c> will result in no highlighting, but counts
        ///   as a match so that no other highlighters will be processed for the building</param>
        void AddBuildingRangeHighlighter(string uniqueId, SButton? hotkey, Func<Building,Tuple<Color,bool[,],int,int>> highlighter);
        /// <summary>Add a highlighter for buildings that also allows for highlighting during placement.</summary>
        /// <param name="uniqueId">
        ///   An ID by which the highlighter can be removed later.
        ///   Best practice is for it to contain your mod's unique ID.
        /// </param>
        /// <param name="hotkey">Also apply the highlighter when this key is held</param>
        /// <param name="blueprintHighlighter">
        ///   A function that evaluates whether the <c>BluePrint</c> (for a building
        ///   currently being placed) matches
        ///   this highlighter, and if so returns a <c>Tuple</c> containing the tint
        ///   color, highlight shape, and x and y offset for the building "center".
        ///   If the building does not match then
        ///   the function should return <c>null</c>.  (Note that returning an
        ///   empty <c>bool[,]</c> will result in no highlighting, but counts
        ///   as a match so that no other highlighters will be processed for the blueprint</param>
        /// <param name="buildingHighlighter">
        ///   A function that evaluates whether the <c>Building</c> matches
        ///   this highlighter, and if so returns a <c>Tuple</c> containing the tint
        ///   color, highlight shape, and x and y offset for the building "center".
        ///   If the building does not match then
        ///   the function should return <c>null</c>.  (Note that returning an
        ///   empty <c>bool[,]</c> will result in no highlighting, but counts
        ///   as a match so that no other highlighters will be processed for the building</param>
        void AddBuildingRangeHighlighter(string uniqueId, SButton? hotkey,
                Func<BluePrint, Tuple<Color, bool[,], int, int>> blueprintHighlighter,
                Func<Building, Tuple<Color, bool[,], int, int>> buildingHighlighter);
        /// <summary>
        ///   Remove any building range highlighters added with the given <c>uniqueId</c>
        /// </summary>
        void RemoveBuildingRangeHighlighter(string uniqueId);
        /// <summary>Add a highlighter for items.</summary>
        /// <param name="uniqueId">
        ///   An ID by which the highlighter can be removed later.
        ///   Best practice is for it to contain your mod's unique ID.
        /// </param>
        /// <param name="hotkey">Also apply the highlighter when this key is held</param>
        /// <param name="highlighter">
        ///   A function that evaluates whether the lower-cased item name matches
        ///   this highlighter, and if so returns a <c>Tuple</c> containing the tint
        ///   color and highlight shape.  If the item name does not match then
        ///   the function should return <c>null</c>.  (Note that returning an
        ///   empty <c>bool[,]</c> will result in no highlighting, but counts
        ///   as a match so that no other highlighters will be processed for the item</param>
        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the other instead.")]
        void AddItemRangeHighlighter(string uniqueId, SButton? hotkey, Func<string, Tuple<Color, bool[,]>> highlighter);
        /// <summary>Add a highlighter for items.</summary>
        /// <param name="uniqueId">
        ///   An ID by which the highlighter can be removed later.
        ///   Best practice is for it to contain your mod's unique ID.
        /// </param>
        /// <param name="hotkey">Also apply the highlighter when this key is held</param>
        /// <param name="highlighter">
        ///   A function that evaluates whether the given item matches
        ///   this highlighter, and if so returns a <c>Tuple</c> containing the tint
        ///   color and highlight shape.  The function parameters are the <c>Item</c>
        ///   object, its item ID ("parent sheet index"), and the lower-cased item name.
        ///   If the item does not match then
        ///   the function should return <c>null</c>.  (Note that returning an
        ///   empty <c>bool[,]</c> will result in no highlighting, but counts
        ///   as a match so that no other highlighters will be processed for the item</param>
        void AddItemRangeHighlighter(string uniqueId, SButton? hotkey, Func<Item, int, string, Tuple<Color, bool[,]>> highlighter);
        /// <summary>
        ///   Remove any item range highlighters added with the given <c>uniqueId</c>
        /// </summary>
        void RemoveItemRangeHighlighter(string uniqueId);

    }
}
