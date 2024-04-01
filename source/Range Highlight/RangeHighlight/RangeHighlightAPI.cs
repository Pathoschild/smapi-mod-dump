/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020-2023 Jamie Taylor
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace RangeHighlight {
    using BuildingHighlighterResult = Tuple<Color, bool[,], int, int>;
    using ItemHighlighterResult = Tuple<Color, bool[,]>;
    using TASHighlighterResult = Tuple<Color, bool[,]>;
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


        public BuildingHighlighterResult BuildingHighlighterResult(Color tint, bool[,] shape, int centerOffsetX, int centerOffsetY) {
            return new(tint, shape, centerOffsetX, centerOffsetY);
        }

        public void AddBuildingRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<CarpenterMenu.BlueprintEntry, List<BuildingHighlighterResult>?>? blueprintHighlighter, Func<Building, List<Tuple<Color, bool[,], int, int>>?> buildingHighlighter) {
            rangeHighlighter.AddBuildingHighlighter(uniqueId, isEnabled, hotkey, blueprintHighlighter, buildingHighlighter);
        }

        public void AddBuildingRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<CarpenterMenu.BlueprintEntry, BuildingHighlighterResult?>? blueprintHighlighter, Func<Building, Tuple<Color, bool[,], int, int>?> buildingHighlighter) {
            Func<Building, List<BuildingHighlighterResult>?> wrappedBuildingHighlighter =
                (Building b) => {
                    var orig = buildingHighlighter(b);
                    if (orig is null) return null;
                    return new List<BuildingHighlighterResult>(1) { orig };
                };
            Func<CarpenterMenu.BlueprintEntry, List<BuildingHighlighterResult>?>? wrappedBpHighlighter =
                blueprintHighlighter is null ? null : (CarpenterMenu.BlueprintEntry bp) => {
                    var orig = blueprintHighlighter(bp);
                    if (orig is null) return null;
                    return new List<BuildingHighlighterResult>(1) { orig };
                };
            rangeHighlighter.AddBuildingHighlighter(uniqueId, isEnabled, hotkey, wrappedBpHighlighter, wrappedBuildingHighlighter);
        }

        public void RemoveBuildingRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveBuildingHighlighter(uniqueId);
        }

        // ----- Item Highlighters ----

        public ItemHighlighterResult ItemHighlighterResult(Color tint, bool[,] shape) {
            return new(tint, shape);
        }

        public void AddItemRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<bool> highlightOthersWhenHeld, Action? onRangeCalculationStart, Func<Item, List<ItemHighlighterResult>?> highlighter, Action? onRangeCalculationFinish) {
            rangeHighlighter.AddItemHighlighter(uniqueId, isEnabled, hotkey, highlightOthersWhenHeld, highlighter, onRangeCalculationStart, onRangeCalculationFinish);
        }

        public void AddItemRangeHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> hotkey, Func<bool> highlightOthersWhenHeld, Func<Item, ItemHighlighterResult?> highlighter) {
            Func<Item, List<ItemHighlighterResult>?> wrappedHighlighter =
                (Item i) => {
                    var orig = highlighter(i);
                    if (orig is null) return null;
                    return new List<ItemHighlighterResult>(1) { orig };
                };
            AddItemRangeHighlighter(uniqueId, isEnabled, hotkey, highlightOthersWhenHeld, null, wrappedHighlighter, null);
        }

        public void RemoveItemRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveItemHighlighter(uniqueId);
        }

        // ----- Temporary Animated Sprite Highlighters ----

        public TASHighlighterResult TASHighlighterResult(Color tint, bool[,] shape) {
            return new(tint, shape);
        }

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, Func<bool> isEnabled, Func<TemporaryAnimatedSprite, TASHighlighterResult?> highlighter) {
            Func<TemporaryAnimatedSprite, List<TASHighlighterResult>?> wrappedHighlighter =
                (TemporaryAnimatedSprite tas) => {
                    var orig = highlighter(tas);
                    if (orig is null) return null;
                    return new List<TASHighlighterResult>(1) { orig };
                };
            rangeHighlighter.AddTemporaryAnimatedSpriteHighlighter(uniqueId, isEnabled, wrappedHighlighter);
        }

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, Func<bool> isEnabled, Func<TemporaryAnimatedSprite, List<TASHighlighterResult>?> highlighter) {
            rangeHighlighter.AddTemporaryAnimatedSpriteHighlighter(uniqueId, isEnabled, highlighter);
        }

        public void RemoveTemporaryAnimatedSpriteRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveTemporaryAnimatedSpriteHighlighter(uniqueId);
        }
    }
}
