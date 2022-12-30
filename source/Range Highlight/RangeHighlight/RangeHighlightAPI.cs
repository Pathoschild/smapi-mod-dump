/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020-2022 Jamie Taylor
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace RangeHighlight {
    public class RangeHighlightAPI : IRangeHighlightAPI {
        private readonly TheMod theMod;
        private ModConfig config => theMod.config;
        private RangeHighlighter rangeHighlighter => theMod.highlighter;

        public RangeHighlightAPI(TheMod mod) {
            theMod = mod;
        }

        // -----------------------------------------------------------
        // ----- Getters for the currently configured tint colors ----
        // -----------------------------------------------------------

        public Color GetJunimoRangeTint() {
            return config.JunimoRangeTint;
        }

        public Color GetScarecrowRangeTint() {
            return config.ScarecrowRangeTint;
        }

        public Color GetSprinklerRangeTint() {
            return config.SprinklerRangeTint;
        }

        public Color GetBeehouseRangeTint() {
            return config.BeehouseRangeTint;
        }

        // -----------------------------------------------
        // ----- Helpers for making highlight shapes -----
        // -----------------------------------------------

        private bool[,] GetCircle(int radius, bool excludeCenter, Func<int, int, int> CalcDistance) {
            int size = 2 * radius + 1;
            bool[,] result = new bool[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    int dist = CalcDistance(i, j);
                    result[i, j] = dist <= radius;
                }
            }
            if (excludeCenter) {
                result[radius, radius] = false;
            }
            return result;
        }

        [Obsolete("GetCartesianCircle is deprecated.  Use GetCartesianCircleWithTruncate instead.")]
        public bool[,] GetCartesianCircle(uint radius, bool excludeCenter = true) {
            return GetCartesianCircleWithTruncate(radius, excludeCenter);
        }

        public bool[,] GetCartesianCircleWithTruncate(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)Math.Truncate(Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j))));
        }

        public bool[,] GetCartesianCircleWithCeiling(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)Math.Ceiling(Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j))));
        }

        public bool[,] GetCartesianCircleWithRound(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)Math.Round(Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j))));
        }

        public bool[,] GetManhattanCircle(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)(Math.Abs(r - i) + Math.Abs(r - j)));
        }

        public bool[,] GetSquareCircle(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)(Math.Max(Math.Abs(r - i), Math.Abs(r - j))));
        }

        // ----------------------------------------
        // ----- Hooks for applying highlights ----
        // ----------------------------------------

        // ----- Building Highlighters ----

        public void AddBuildingRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<BluePrint, List<Tuple<Color, bool[,], int, int>>?>? blueprintHighlighter, Func<Building, List<Tuple<Color, bool[,], int, int>>?> buildingHighlighter) {
            rangeHighlighter.AddBuildingHighlighter(uniqueId, isEnabled, hotkey, blueprintHighlighter, buildingHighlighter);
        }

        public void AddBuildingRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<BluePrint, Tuple<Color, bool[,], int, int>?>? blueprintHighlighter, Func<Building, Tuple<Color, bool[,], int, int>?> buildingHighlighter) {
            Func<Building, List<Tuple<Color, bool[,], int, int>>?> wrappedBuildingHighlighter =
                (Building b) => {
                    var orig = buildingHighlighter(b);
                    if (orig is null) return null;
                    return new List<Tuple<Color, bool[,], int, int>>(1) { orig };
                };
            Func<BluePrint, List<Tuple<Color, bool[,], int, int>>?>? wrappedBpHighlighter =
                blueprintHighlighter is null ? null : (BluePrint bp) => {
                    var orig = blueprintHighlighter(bp);
                    if (orig is null) return null;
                    return new List<Tuple<Color, bool[,], int, int>>(1) { orig };
                };
            rangeHighlighter.AddBuildingHighlighter(uniqueId, isEnabled, hotkey, wrappedBpHighlighter, wrappedBuildingHighlighter);
        }

        [Obsolete("This AddBuildingRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        public void AddBuildingRangeHighlighter(string uniqueId, KeybindList hotkey, Func<Building, Tuple<Color, bool[,], int, int>?> highlighter) {
            AddBuildingRangeHighlighter(uniqueId, () => true, () => hotkey, null, highlighter);
        }

        [Obsolete("This AddBuildingRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddBuildingRangeHighlighter(string uniqueId, SButton? hotkey, Func<Building, Tuple<Color, bool[,], int, int>?> highlighter) {
            AddBuildingRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), highlighter);
        }

        [Obsolete("This AddBuildingRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        public void AddBuildingRangeHighlighter(string uniqueId, KeybindList hotkey,
                Func<BluePrint, Tuple<Color, bool[,], int, int>?> blueprintHighlighter,
                Func<Building, Tuple<Color, bool[,], int, int>?> buildingHighlighter) {
            AddBuildingRangeHighlighter(uniqueId, () => true, () => hotkey, blueprintHighlighter, buildingHighlighter);
        }

        [Obsolete("This AddBuildingRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddBuildingRangeHighlighter(string uniqueId, SButton? hotkey,
                Func<BluePrint, Tuple<Color, bool[,], int, int>?> blueprintHighlighter,
                Func<Building, Tuple<Color, bool[,], int, int>?> buildingHighlighter) {
            AddBuildingRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), blueprintHighlighter, buildingHighlighter);
        }

        // ----- Item Highlighters ----

        public void AddItemRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<bool> highlightOthersWhenHeld, Action? onRangeCalculationStart, Func<Item, int, string, List<Tuple<Color, bool[,]>>?> highlighter, Action? onRangeCalculationFinish) {
            rangeHighlighter.AddItemHighlighter(uniqueId, isEnabled, hotkey, highlightOthersWhenHeld, highlighter, onRangeCalculationStart, onRangeCalculationFinish);
        }

        public void AddItemRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<bool> highlightOthersWhenHeld, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter) {
            Func<Item, int, string, List<Tuple<Color, bool[,]>>?> wrappedHighlighter =
                (Item i, int idx, string name) => {
                    var orig = highlighter(i, idx, name);
                    if (orig is null) return null;
                    return new List<Tuple<Color, bool[,]>>(1) { orig };
                };
            AddItemRangeHighlighter(uniqueId, isEnabled, hotkey, highlightOthersWhenHeld, null, wrappedHighlighter, null);
        }


        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddItemRangeHighlighter(string uniqueId, SButton? hotkey, Func<string, Tuple<Color, bool[,]>?> highlighter) {
            AddItemRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), true, (Item item, int itemID, string lowerName) => { return highlighter(lowerName); });
        }

        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddItemRangeHighlighter(string uniqueId, SButton? hotkey, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter) {
            AddItemRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), true, highlighter);
        }

        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        public void AddItemRangeHighlighter(string uniqueId, KeybindList hotkey, bool highlightOthersWhenHeld, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter) {
            AddItemRangeHighlighter(uniqueId, () => true, () => hotkey, () => highlightOthersWhenHeld, highlighter);
        }

        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddItemRangeHighlighter(string uniqueId, SButton? hotkey, bool highlightOthersWhenHeld, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter) {
            AddItemRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), highlightOthersWhenHeld, highlighter);
        }

        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        public void AddItemRangeHighlighter(string uniqueId, KeybindList hotkey, bool highlightOthersWhenHeld, Action? onRangeCalculationStart, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter, Action? onRangeCalculationFinish) {
            Func<Item, int, string, List<Tuple<Color, bool[,]>>?> wrappedHighlighter =
                (Item i, int idx, string name) => {
                    var orig = highlighter(i, idx, name);
                    if (orig is null) return null;
                    return new List<Tuple<Color, bool[,]>>(1) { orig };
                };
            rangeHighlighter.AddItemHighlighter(uniqueId, () => true, () => hotkey, () => highlightOthersWhenHeld, wrappedHighlighter, onRangeCalculationStart, onRangeCalculationFinish);
        }

        [Obsolete("This AddItemRangeHighlighter signature is deprecated.  Use the non-deprecated one instead.")]
        void IRangeHighlightAPI.AddItemRangeHighlighter(string uniqueId, SButton? hotkey, bool highlightOthersWhenHeld, Action? onRangeCalculationStart, Func<Item, int, string, Tuple<Color, bool[,]>?> highlighter, Action? onRangeCalculationFinish) {
            AddItemRangeHighlighter(uniqueId, KeybindList.ForSingle(hotkey ?? SButton.None), highlightOthersWhenHeld, onRangeCalculationStart, highlighter, onRangeCalculationFinish);
        }

        public void RemoveBuildingRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveBuildingHighlighter(uniqueId);
        }

        public void RemoveItemRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveItemHighlighter(uniqueId);
        }

        // ----- Temporary Animated Sprite Highlighters ----

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, Func<bool> isEnabled, Func<TemporaryAnimatedSprite, Tuple<Color, bool[,]>?> highlighter) {
            Func<TemporaryAnimatedSprite, List<Tuple<Color, bool[,]>>?> wrappedHighlighter =
                (TemporaryAnimatedSprite tas) => {
                    var orig = highlighter(tas);
                    if (orig is null) return null;
                    return new List<Tuple<Color, bool[,]>>(1) { orig };
                };
            rangeHighlighter.AddTemporaryAnimatedSpriteHighlighter(uniqueId, isEnabled, wrappedHighlighter);
        }

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, Func<bool> isEnabled, Func<TemporaryAnimatedSprite, List<Tuple<Color, bool[,]>>?> highlighter) {
            rangeHighlighter.AddTemporaryAnimatedSpriteHighlighter(uniqueId, isEnabled, highlighter);
        }

        public void RemoveTemporaryAnimatedSpriteRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveTemporaryAnimatedSpriteHighlighter(uniqueId);
        }
    }
}
