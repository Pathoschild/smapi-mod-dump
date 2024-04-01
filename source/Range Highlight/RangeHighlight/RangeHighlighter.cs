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
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

namespace RangeHighlight {
    using BlueprintHighlightFunction = Func<CarpenterMenu.BlueprintEntry, List<Tuple<Color, bool[,], int, int>>?>;
    using BuildingHighlightFunction = Func<Building, List<Tuple<Color, bool[,], int, int>>?>;
    using ItemHighlightFunction = Func<Item, List<Tuple<Color, bool[,]>>?>;
    using TASHighlightFunction = Func<TemporaryAnimatedSprite, List<Tuple<Color, bool[,]>>?>;

    internal class RangeHighlighter {
        private readonly TheMod theMod;
        private IModHelper helper => theMod.helper;
        private ModConfig config => theMod.config;
        private readonly Texture2D tileTexture;

        private readonly PerScreen<List<Tuple<Color, Point>>> highlightTiles = new PerScreen<List<Tuple<Color, Point>>>(createNewState: () => new List<Tuple<Color, Point>>());
        private readonly PerScreen<Mutex> highlightTilesMutex = new PerScreen<Mutex>(createNewState: () => new Mutex());
        private readonly PerScreen<bool> showAllDownLastState = new PerScreen<bool>();
        private readonly PerScreen<bool> showAllToggleState = new PerScreen<bool>();

        private class Highlighter<T> {
            public string uniqueId { get; }
            private readonly Func<KeybindList> getHotkey;
            public KeybindList Hotkey => getHotkey();
            private readonly Func<bool> isEnabled;
            public bool IsEnabled => isEnabled();
            public T highlighter { get; }
            private readonly PerScreen<bool> hotkeyDownLastState = new PerScreen<bool>();
            internal readonly PerScreen<bool> hotkeyToggleState = new PerScreen<bool>();

            public Highlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> getHotkey, T highlighter) {
                this.uniqueId = uniqueId;
                this.isEnabled = isEnabled;
                this.getHotkey = getHotkey;
                this.highlighter = highlighter;
            }

            public void UpdateHotkeyToggleState(IModHelper helper) {
                var hotkey = this.Hotkey;
                if (hotkey.IsBound) {
                    bool isDown = hotkey.IsDown();
                    if (isDown && !hotkeyDownLastState.Value)
                        hotkeyToggleState.Value = !hotkeyToggleState.Value;
                    hotkeyDownLastState.Value = isDown;
                }
            }
        }
        private class ItemHighlighter : Highlighter<ItemHighlightFunction> {
            private readonly Func<bool> getHighlightOthersWhenHeld;
            public bool HighlightOthersWhenHeld => getHighlightOthersWhenHeld();
            public Action? onStart { get; }
            public Action? onFinish { get; }
            public ItemHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> getHotkey, Func<bool> highlightOthersWhenHeld, ItemHighlightFunction highlighter, Action? onStart = null, Action? onFinish = null)
                : base(uniqueId, isEnabled, getHotkey, highlighter) {
                this.getHighlightOthersWhenHeld = highlightOthersWhenHeld;
                this.onStart = onStart;
                this.onFinish = onFinish;
            }
        }
        private readonly List<Tuple<Highlighter<BuildingHighlightFunction>, BlueprintHighlightFunction?>> allBuildingHighlighters = new();
        private readonly List<ItemHighlighter> allItemHighlighters = new List<ItemHighlighter>();
        private readonly List<Highlighter<TASHighlightFunction>> allTasHighlighters = new List<Highlighter<TASHighlightFunction>>();

        public RangeHighlighter(TheMod mod) {
            theMod = mod;
            tileTexture = helper.ModContent.Load<Texture2D>("tile.png");
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        public void ClearAllHighlighters() {
            allBuildingHighlighters.Clear();
            allItemHighlighters.Clear();
            allTasHighlighters.Clear();
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e) {
            if (highlightTilesMutex.Value.WaitOne(0)) {
                try {
                    foreach (var tuple in highlightTiles.Value) {
                        var point = tuple.Item2;
                        var tint = tuple.Item1;
                        Game1.spriteBatch.Draw(
                            tileTexture,
                            Game1.GlobalToLocal(new Vector2(point.X * Game1.tileSize, point.Y * Game1.tileSize)),
                            null,
                            tint,
                            0.0f,
                            Vector2.Zero,
                            Game1.pixelZoom,
                            SpriteEffects.None,
                            0.01f);

                    }
                } finally {
                    highlightTilesMutex.Value.ReleaseMutex();
                }
            }
        }

        internal void AddHighlightTiles(Color color, bool[,] shape, int xOrigin, int yOrigin) {
            int xOffset = shape.GetLength(0) / 2;
            int yOffset = shape.GetLength(1) / 2;
            if (highlightTilesMutex.Value.WaitOne(0)) {
                try {
                    for (int x = 0; x < shape.GetLength(0); ++x) {
                        for (int y = 0; y < shape.GetLength(1); ++y) {
                            if (shape[x, y])
                                highlightTiles.Value.Add(new Tuple<Color, Point>(color, new Point(xOrigin + x - xOffset, yOrigin + y - yOffset)));
                        }
                    }
                } finally {
                    highlightTilesMutex.Value.ReleaseMutex();
                }
            }
        }

        internal void AddHighlightTiles(List<Tuple<Color, bool[,]>> ls, int xOrigin, int yOrigin) {
            foreach(var item in ls) {
                AddHighlightTiles(item.Item1, item.Item2, xOrigin, yOrigin);
            }
        }

        public void AddBuildingHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> getHotkey,
                BlueprintHighlightFunction? blueprintHighlighter, BuildingHighlightFunction buildingHighlighter) {
            allBuildingHighlighters.Insert(0, new (new Highlighter<BuildingHighlightFunction>(uniqueId, isEnabled, getHotkey, buildingHighlighter), blueprintHighlighter));
        }

        public void RemoveBuildingHighlighter(string uniqueId) {
            allBuildingHighlighters.RemoveAll(elt => elt.Item1.uniqueId == uniqueId);
        }

        public void AddItemHighlighter(string uniqueId, Func<bool> isEnabled, Func<KeybindList> getHotkey, Func<bool> highlightOthersWhenHeld, ItemHighlightFunction highlighter, Action? onStart = null, Action? onFinish = null) {
            allItemHighlighters.Insert(0, new ItemHighlighter(uniqueId, isEnabled, getHotkey, highlightOthersWhenHeld, highlighter, onStart, onFinish));
        }

        public void RemoveItemHighlighter(string uniqueId) {
            allItemHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, Func<bool> isEnabled, TASHighlightFunction highlighter) {
            allTasHighlighters.Insert(0, new Highlighter<TASHighlightFunction>(uniqueId, isEnabled, () => new KeybindList(), highlighter));
        }

        public void RemoveTemporaryAnimatedSpriteHighlighter(string uniqueId) {
            allTasHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        internal Vector2 GetCursorTile() {
            return helper.Input.GetCursorPosition().Tile;
            // Work around bug in SMAPI 3.8.0 - fixed in 3.8.2
            //var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            //return new Vector2((Game1.viewport.X + mouse.X / Game1.options.zoomLevel) / Game1.tileSize,
            //    (Game1.viewport.Y + mouse.Y / Game1.options.zoomLevel) / Game1.tileSize);
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
            if (!e.IsMultipleOf(theMod.config.RefreshInterval)) return;

            if (highlightTilesMutex.Value.WaitOne()) {
                try {
                    highlightTiles.Value.Clear();
                } finally {
                    highlightTilesMutex.Value.ReleaseMutex();
                }
            }

            if (Game1.eventUp || Game1.currentLocation == null) return;

            List<Tuple<Highlighter<BuildingHighlightFunction>, BlueprintHighlightFunction?>> buildingHighlighters = new(allBuildingHighlighters.Count);
            foreach(var hl in allBuildingHighlighters) if (hl.Item1.IsEnabled) buildingHighlighters.Add(hl);
            List<ItemHighlighter> itemHighlighters = new (allItemHighlighters.Count);
            foreach(var hl in allItemHighlighters) if (hl.IsEnabled) itemHighlighters.Add(hl);
            List<Highlighter<TASHighlightFunction>> tasHighlighters = new (allTasHighlighters.Count);
            foreach(var hl in allTasHighlighters) if (hl.IsEnabled) tasHighlighters.Add(hl);

            bool[] runBuildingHighlighter = new bool[buildingHighlighters.Count];
            bool[] runItemHighlighter = new bool[itemHighlighters.Count];
            bool[] itemHighlighterStartCalled = new bool[itemHighlighters.Count];
            bool iterateBuildings = false;
            bool iterateItems = false;

            if (Game1.activeClickableMenu != null) {
                if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu && Game1.currentLocation.IsBuildableLocation()) {
                    for (int i = 0; i < buildingHighlighters.Count; ++i) {
                        if (buildingHighlighters[i].Item2 is var highlighter && highlighter is not null) {
                            var rets = highlighter(carpenterMenu.Blueprint);
                            if (rets is not null) foreach(var ret in rets){
                                var cursorTile = GetCursorTile();
                                AddHighlightTiles(ret.Item1, ret.Item2, (int)cursorTile.X + ret.Item3, (int)cursorTile.Y + ret.Item4);
                                runBuildingHighlighter[i] = true;
                                iterateBuildings = true;
                                //break;
                            }
                        }
                    }
                } else {
                    return;
                }
            }

            if (Game1.currentLocation.IsBuildableLocation() && config.HighlightBuildingsOnMouseover) {
                // check to see if the cursor is over a building
                Building building = Game1.currentLocation.getBuildingAt(Game1.currentCursorTile);
                if (building != null) {
                    for (int i = 0; i < buildingHighlighters.Count; ++i) {
                        var ret = buildingHighlighters[i].Item1.highlighter(building);
                        if (ret != null) {
                            // ignore return value; it will be re-computed later when we iterate buildings
                            runBuildingHighlighter[i] = true;
                            iterateBuildings = true;
                            break;
                        }
                    }
                }
            }

            if (config.hotkeysToggle) {
                bool showAllDown = config.ShowAllRangesKey.IsDown();
                if (showAllDown && !showAllDownLastState.Value)
                    showAllToggleState.Value = !showAllToggleState.Value;
                showAllDownLastState.Value = showAllDown;
                for (int i = 0; i < buildingHighlighters.Count; ++i) {
                    buildingHighlighters[i].Item1.UpdateHotkeyToggleState(helper);
                    if (showAllToggleState.Value || buildingHighlighters[i].Item1.hotkeyToggleState.Value) {
                        runBuildingHighlighter[i] = true;
                        iterateBuildings = true;
                    }
                }
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    itemHighlighters[i].UpdateHotkeyToggleState(helper);
                    if (showAllToggleState.Value || itemHighlighters[i].hotkeyToggleState.Value) {
                        runItemHighlighter[i] = true;
                        iterateItems = true;
                    }
                }
            } else {
                bool showAll = config.ShowAllRangesKey.IsDown();
                for (int i = 0; i < buildingHighlighters.Count; ++i) {
                    if (showAll || buildingHighlighters[i].Item1.Hotkey.IsDown()) {
                        runBuildingHighlighter[i] = true;
                        iterateBuildings = true;
                    }
                }
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    if (showAll || itemHighlighters[i].Hotkey.IsDown()) {
                        runItemHighlighter[i] = true;
                        iterateItems = true;
                    }
                }
            }

            if (Game1.player.CurrentItem != null) {
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    itemHighlighters[i].onStart?.Invoke();
                    itemHighlighterStartCalled[i] = true;
                }
            } else if (iterateItems) {
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    if (runItemHighlighter[i]) {
                        itemHighlighters[i].onStart?.Invoke();
                        itemHighlighterStartCalled[i] = true;
                    }
                }
            }

            if (tasHighlighters.Count > 0) {
                foreach (var sprite in Game1.currentLocation.temporarySprites) {
                    foreach (var highlighter in tasHighlighters) {
                        var ret = highlighter.highlighter(sprite);
                        if (ret != null) {
                            AddHighlightTiles(ret,
                                (int)(sprite.position.X / Game1.tileSize), (int)(sprite.position.Y / Game1.tileSize));
                            //break;
                        }
                    }
                }

            }

            if (Game1.player.CurrentItem != null) {
                Item item = Game1.player.CurrentItem;
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    var ret = itemHighlighters[i].highlighter(item);
                    if (ret != null) {
                        if (itemHighlighters[i].HighlightOthersWhenHeld) {
                            runItemHighlighter[i] = true;
                        }
                        iterateItems = true;
                        var cursorTile = GetCursorTile();
                        var actionTile = cursorTile;
                        bool mouseHidden = !Game1.wasMouseVisibleThisFrame || Game1.mouseCursorTransparency == 0f;
                        bool showAtActionTile = config.HighlightActionLocation == HighlightActionLocationStyle.Always
                            || config.HighlightActionLocation == HighlightActionLocationStyle.WhenMouseHidden && mouseHidden;
                        if (mouseHidden || !Utility.tileWithinRadiusOfPlayer((int)cursorTile.X, (int)cursorTile.Y, 1, Game1.player)) {
                            var grabTile = Game1.player.GetGrabTile();
                            var oldVal = Game1.isCheckingNonMousePlacement;
                            Game1.isCheckingNonMousePlacement = true;
                            actionTile = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, item, (int)grabTile.X * 64 + 32, (int)grabTile.Y * 64 + 32) / 64;
                            Game1.isCheckingNonMousePlacement = oldVal;
                        }
                        if (showAtActionTile) AddHighlightTiles(ret, (int)actionTile.X, (int)actionTile.Y);
                        if ((!showAtActionTile || cursorTile != actionTile)
                            && !mouseHidden) {
                            AddHighlightTiles(ret, (int)cursorTile.X, (int)cursorTile.Y);
                        }
                        //break;
                    }
                }
            }

            if (iterateBuildings) {
                if (Game1.currentLocation.IsBuildableLocation()) {
                    foreach (Building building in Game1.currentLocation.buildings) {
                        for (int i = 0; i < buildingHighlighters.Count; ++i) {
                            var rets = buildingHighlighters[i].Item1.highlighter(building);
                            if (rets != null) foreach(var ret in rets) {
                                AddHighlightTiles(ret.Item1, ret.Item2, building.tileX.Value + ret.Item3, building.tileY.Value + ret.Item4);
                                //break;
                            }
                        }
                    }
                }
            }

            if (iterateItems) {
                foreach (var item in Game1.currentLocation.Objects.Values) {
                    for (int i = 0; i < itemHighlighters.Count; ++i) {
                        if (runItemHighlighter[i]) {
                            var ret = itemHighlighters[i].highlighter(item);
                            if (ret != null) {
                                AddHighlightTiles(ret, (int)item.TileLocation.X, (int)item.TileLocation.Y);
                                //break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < itemHighlighters.Count; ++i) {
                if (itemHighlighterStartCalled[i]) {
                    itemHighlighters[i].onFinish?.Invoke();
                }
            }
        }
    }
}
