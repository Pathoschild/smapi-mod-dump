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
    using BlueprintHighlightFunction = Func<BluePrint, Tuple<Color, bool[,], int, int>>;
    using BuildingHighlightFunction = Func<Building, Tuple<Color, bool[,], int, int>>;
    using ItemHighlightFunction = Func<Item, int, string, Tuple<Color, bool[,]>>;
    using TASHighlightFunction = Func<TemporaryAnimatedSprite, Tuple<Color, bool[,]>>;

    internal class RangeHighlighter {
        private readonly ModEntry theMod;
        private IModHelper helper => theMod.helper;
        private ModConfig config => theMod.config;
        private readonly Texture2D tileTexture;

        private readonly PerScreen<List<Tuple<Color, Point>>> highlightTiles = new PerScreen<List<Tuple<Color, Point>>>(createNewState: () => new List<Tuple<Color, Point>>());
        private readonly PerScreen<Mutex> highlightTilesMutex = new PerScreen<Mutex>(createNewState: () => new Mutex());
        private readonly PerScreen<bool> showAllDownLastState = new PerScreen<bool>();
        private readonly PerScreen<bool> showAllToggleState = new PerScreen<bool>();

        private class Highlighter<T> {
            public string uniqueId { get; }
            public KeybindList hotkey { get; }
            public T highlighter { get; }
            private readonly PerScreen<bool> hotkeyDownLastState = new PerScreen<bool>();
            internal readonly PerScreen<bool> hotkeyToggleState = new PerScreen<bool>();

            public Highlighter(string uniqueId, KeybindList hotkey, T highlighter) {
                this.uniqueId = uniqueId;
                this.hotkey = hotkey;
                this.highlighter = highlighter;
            }

            public void UpdateHotkeyToggleState(IModHelper helper) {
                if (this.hotkey.IsBound) {
                    bool isDown = hotkey.IsDown();
                    if (isDown && !hotkeyDownLastState.Value)
                        hotkeyToggleState.Value = !hotkeyToggleState.Value;
                    hotkeyDownLastState.Value = isDown;
                }
            }
        }
        private class ItemHighlighter : Highlighter<ItemHighlightFunction> {
            public bool highlightOthersWhenHeld { get; }
            public Action onStart { get; }
            public Action onFinish { get; }
            public ItemHighlighter(string uniqueId, KeybindList hotkey, bool highlightOthersWhenHeld, ItemHighlightFunction highlighter, Action onStart = null, Action onFinish = null)
                : base(uniqueId, hotkey, highlighter) {
                this.highlightOthersWhenHeld = highlightOthersWhenHeld;
                this.onStart = onStart;
                this.onFinish = onFinish;
            }
        }
        // NB: blueprintHighlighters and buildingHighlighters are parallel lists.  The highlighter in a blueprintHighlighter may be null.
        private readonly List<Highlighter<BlueprintHighlightFunction>> blueprintHighlighters = new List<Highlighter<BlueprintHighlightFunction>>();
        private readonly List<Highlighter<BuildingHighlightFunction>> buildingHighlighters = new List<Highlighter<BuildingHighlightFunction>>();
        private readonly List<ItemHighlighter> itemHighlighters = new List<ItemHighlighter>();
        private readonly List<Highlighter<TASHighlightFunction>> tasHighlighters = new List<Highlighter<TASHighlightFunction>>();

        public RangeHighlighter(ModEntry mod) {
            theMod = mod;
            tileTexture = helper.ModContent.Load<Texture2D>("tile.png");
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        public void ClearAllHighlighters() {
            blueprintHighlighters.Clear();
            buildingHighlighters.Clear();
            itemHighlighters.Clear();
            tasHighlighters.Clear();
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e) {
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

        public void AddBuildingHighlighter(string uniqueId, KeybindList hotkey,
                BlueprintHighlightFunction blueprintHighlighter, BuildingHighlightFunction buildingHighlighter) {
            blueprintHighlighters.Insert(0, new Highlighter<BlueprintHighlightFunction>(uniqueId, hotkey, blueprintHighlighter));
            buildingHighlighters.Insert(0, new Highlighter<BuildingHighlightFunction>(uniqueId, hotkey, buildingHighlighter));
        }

        public void RemoveBuildingHighlighter(string uniqueId) {
            blueprintHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
            buildingHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        public void AddItemHighlighter(string uniqueId, KeybindList hotkey, bool highlightOthersWhenHeld, ItemHighlightFunction highlighter, Action onStart = null, Action onFinish = null) {
            itemHighlighters.Insert(0, new ItemHighlighter(uniqueId, hotkey, highlightOthersWhenHeld, highlighter, onStart, onFinish));
        }

        public void RemoveItemHighlighter(string uniqueId) {
            itemHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        public void AddTemporaryAnimatedSpriteHighlighter(string uniqueId, TASHighlightFunction highlighter) {
            tasHighlighters.Insert(0, new Highlighter<TASHighlightFunction>(uniqueId, null, highlighter));
        }

        public void RemoveTemporaryAnimatedSpriteHighlighter(string uniqueId) {
            tasHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        internal Vector2 GetCursorTile() {
            return helper.Input.GetCursorPosition().Tile;
            // Work around bug in SMAPI 3.8.0 - fixed in 3.8.2
            //var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            //return new Vector2((Game1.viewport.X + mouse.X / Game1.options.zoomLevel) / Game1.tileSize,
            //    (Game1.viewport.Y + mouse.Y / Game1.options.zoomLevel) / Game1.tileSize);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (!e.IsMultipleOf(6)) return; // only do this once every 0.1s or so

            if (highlightTilesMutex.Value.WaitOne()) {
                try {
                    highlightTiles.Value.Clear();
                } finally {
                    highlightTilesMutex.Value.ReleaseMutex();
                }
            }

            if (Game1.eventUp || Game1.currentLocation == null) return;
            bool[] runBuildingHighlighter = new bool[buildingHighlighters.Count];
            bool[] runItemHighlighter = new bool[itemHighlighters.Count];
            bool[] itemHighlighterStartCalled = new bool[itemHighlighters.Count];
            bool iterateBuildings = false;
            bool iterateItems = false;

            if (Game1.activeClickableMenu != null) {
                if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu && Game1.currentLocation is BuildableGameLocation) {
                    for (int i = 0; i < blueprintHighlighters.Count; ++i) {
                        if (blueprintHighlighters[i].highlighter != null) {
                            var ret = blueprintHighlighters[i].highlighter(carpenterMenu.CurrentBlueprint);
                            if (ret != null) {
                                var cursorTile = GetCursorTile();
                                AddHighlightTiles(ret.Item1, ret.Item2, (int)cursorTile.X + ret.Item3, (int)cursorTile.Y + ret.Item4);
                                runBuildingHighlighter[i] = true;
                                iterateBuildings = true;
                                break;
                            }
                        }
                    }
                } else {
                    return;
                }
            }

            if (Game1.currentLocation is BuildableGameLocation buildableLocation) {
                // check to see if the cursor is over a building
                Building building = buildableLocation.getBuildingAt(Game1.currentCursorTile);
                if (building != null) {
                    for (int i = 0; i < buildingHighlighters.Count; ++i) {
                        var ret = buildingHighlighters[i].highlighter(building);
                        if (ret != null) {
                            // ignore return value; it will be re-computed later when we iterate buildings
                            runBuildingHighlighter[i] = true;
                            iterateBuildings = true;
                            break;
                        }
                    }
                }
            }

            if (Game1.player.CurrentItem != null) {
                Item item = Game1.player.CurrentItem;
                string itemName = item.Name.ToLower();
                int itemID = item.ParentSheetIndex;
                Utility.IsNormalObjectAtParentSheetIndex(item, itemID);
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    if (!itemHighlighterStartCalled[i]) {
                        itemHighlighters[i].onStart?.Invoke();
                        itemHighlighterStartCalled[i] = true;
                    }
                    var ret = itemHighlighters[i].highlighter(item, itemID, itemName);
                    if (ret != null) {
                        var cursorTile = GetCursorTile();
                        AddHighlightTiles(ret.Item1, ret.Item2, (int)cursorTile.X, (int)cursorTile.Y);
                        if (itemHighlighters[i].highlightOthersWhenHeld) {
                            runItemHighlighter[i] = true;
                        }
                        iterateItems = true;
                        break;
                    }
                }
            }

            if (config.hotkeysToggle) {
                bool showAllDown = config.ShowAllRangesKey.IsDown();
                if (showAllDown && !showAllDownLastState.Value)
                    showAllToggleState.Value = !showAllToggleState.Value;
                showAllDownLastState.Value = showAllDown;
                for (int i = 0; i < buildingHighlighters.Count; ++i) {
                    buildingHighlighters[i].UpdateHotkeyToggleState(helper);
                    if (showAllToggleState.Value || buildingHighlighters[i].hotkeyToggleState.Value) {
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
                    if (showAll || buildingHighlighters[i].hotkey.IsDown()) {
                        runBuildingHighlighter[i] = true;
                        iterateBuildings = true;
                    }
                }
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    if (showAll || itemHighlighters[i].hotkey.IsDown()) {
                        runItemHighlighter[i] = true;
                        iterateItems = true;
                    }
                }
            }

            if (iterateBuildings) {
                if (Game1.currentLocation is BuildableGameLocation bl) {
                    foreach (Building building in bl.buildings) {
                        for (int i = 0; i < buildingHighlighters.Count; ++i) {
                            var ret = buildingHighlighters[i].highlighter(building);
                            if (ret != null) {
                                AddHighlightTiles(ret.Item1, ret.Item2, building.tileX.Value + ret.Item3, building.tileY.Value + ret.Item4);
                                break;
                            }
                        }
                    }
                }
            }

            if (iterateItems) {
                foreach (var item in Game1.currentLocation.Objects.Values) {
                    string itemName = item.Name.ToLower();
                    int itemID = item.ParentSheetIndex;
                    for (int i = 0; i < itemHighlighters.Count; ++i) {
                        if (runItemHighlighter[i]) {
                            if (!itemHighlighterStartCalled[i]) {
                                itemHighlighters[i].onStart?.Invoke();
                                itemHighlighterStartCalled[i] = true;
                            }
                            var ret = itemHighlighters[i].highlighter(item, itemID, itemName);
                            if (ret != null) {
                                AddHighlightTiles(ret.Item1, ret.Item2, (int)item.TileLocation.X, (int)item.TileLocation.Y);
                                break;
                            }
                        }
                    }
                }
            }

            if (tasHighlighters.Count > 0) {
                foreach (var sprite in Game1.currentLocation.temporarySprites) {
                    foreach (var highlighter in tasHighlighters) {
                        var ret = highlighter.highlighter(sprite);
                        if (ret != null) {
                            AddHighlightTiles(ret.Item1, ret.Item2,
                                (int)(sprite.position.X / Game1.tileSize), (int)(sprite.position.Y / Game1.tileSize));
                            break;
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
