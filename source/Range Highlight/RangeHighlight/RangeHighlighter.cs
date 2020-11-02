/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
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
        private readonly IModHelper helper;
        private readonly ModConfig config;
        private readonly List<Tuple<Color, Point>> highlightTiles = new List<Tuple<Color, Point>>();
        private readonly Mutex highlightTilesMutex = new Mutex();
        private readonly Texture2D tileTexture;
        private bool showAllDownLastState = false;
        private bool showAllToggleState = false;

        private class Highlighter<T> {
            public string uniqueId { get; }
            public SButton? hotkey { get; }
            public T highlighter { get; }
            public bool hotkeyDownLastState = false;
            public bool hotkeyToggleState = false;

            public Highlighter(string uniqueId, SButton? hotkey, T highlighter) {
                this.uniqueId = uniqueId;
                this.hotkey = hotkey;
                this.highlighter = highlighter;
            }

            public void UpdateHotkeyToggleState(IModHelper helper) {
                if (this.hotkey is SButton hotkey) {
                    bool isDown = helper.Input.IsDown(hotkey);
                    if (isDown && !hotkeyDownLastState)
                        hotkeyToggleState = !hotkeyToggleState;
                    hotkeyDownLastState = isDown;
                }
            }
        }
        private class ItemHighlighter : Highlighter<ItemHighlightFunction> {
            public bool highlightOthersWhenHeld { get; }
            public ItemHighlighter(string uniqueId, SButton? hotkey, bool highlightOthersWhenHeld, ItemHighlightFunction highlighter)
                : base(uniqueId, hotkey, highlighter) {
                this.highlightOthersWhenHeld = highlightOthersWhenHeld;
            }
        }
        // NB: blueprintHighlighters and buildingHighlighters are parallel lists.  The highlighter in a blueprintHighlighter may be null.
        private readonly List<Highlighter<BlueprintHighlightFunction>> blueprintHighlighters = new List<Highlighter<BlueprintHighlightFunction>>();
        private readonly List<Highlighter<BuildingHighlightFunction>> buildingHighlighters = new List<Highlighter<BuildingHighlightFunction>>();
        private readonly List<ItemHighlighter> itemHighlighters = new List<ItemHighlighter>();
        private readonly List<Highlighter<TASHighlightFunction>> tasHighlighters = new List<Highlighter<TASHighlightFunction>>();

        public RangeHighlighter(IModHelper helper, ModConfig config) {
            this.helper = helper;
            this.config = config;
            tileTexture = helper.Content.Load<Texture2D>("tile.png");
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnRendered(object sender, RenderedEventArgs e) {
            if (highlightTilesMutex.WaitOne(0)) {
                try {
                    foreach (var tuple in highlightTiles) {
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
                    highlightTilesMutex.ReleaseMutex();
                }
            }
        }

        internal void AddHighlightTiles(Color color, bool[,] shape, int xOrigin, int yOrigin) {
            int xOffset = shape.GetLength(0) / 2;
            int yOffset = shape.GetLength(1) / 2;
            if (highlightTilesMutex.WaitOne(0)) {
                try {
                    for (int x = 0; x < shape.GetLength(0); ++x) {
                        for (int y = 0; y < shape.GetLength(1); ++y) {
                            if (shape[x, y])
                                highlightTiles.Add(new Tuple<Color, Point>(color, new Point(xOrigin + x - xOffset, yOrigin + y - yOffset)));
                        }
                    }
                } finally {
                    highlightTilesMutex.ReleaseMutex();
                }
            }
        }

        public void AddBuildingHighlighter(string uniqueId, SButton? hotkey,
                BlueprintHighlightFunction blueprintHighlighter, BuildingHighlightFunction buildingHighlighter) {
            blueprintHighlighters.Insert(0, new Highlighter<BlueprintHighlightFunction>(uniqueId, hotkey, blueprintHighlighter));
            buildingHighlighters.Insert(0, new Highlighter<BuildingHighlightFunction>(uniqueId, hotkey, buildingHighlighter));
        }

        public void RemoveBuildingHighlighter(string uniqueId) {
            blueprintHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
            buildingHighlighters.RemoveAll(elt => elt.uniqueId == uniqueId);
        }

        public void AddItemHighlighter(string uniqueId, SButton? hotkey, bool highlightOthersWhenHeld, ItemHighlightFunction highlighter) {
            itemHighlighters.Insert(0, new ItemHighlighter(uniqueId, hotkey, highlightOthersWhenHeld, highlighter));
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

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (!e.IsMultipleOf(6)) return; // only do this once every 0.1s or so

            if (highlightTilesMutex.WaitOne()) {
                try {
                    highlightTiles.Clear();
                } finally {
                    highlightTilesMutex.ReleaseMutex();
                }
            }

            if (Game1.eventUp || Game1.currentLocation == null) return;
            bool[] runBuildingHighlighter = new bool[buildingHighlighters.Count];
            bool[] runItemHighlighter = new bool[itemHighlighters.Count];
            bool iterateBuildings = false;
            bool iterateItems = false;

            if (Game1.activeClickableMenu != null) {
                if (Game1.activeClickableMenu is CarpenterMenu carpenterMenu && Game1.currentLocation is BuildableGameLocation) {
                    for (int i = 0; i < blueprintHighlighters.Count; ++i) {
                        if (blueprintHighlighters[i].highlighter != null) {
                            var ret = blueprintHighlighters[i].highlighter(carpenterMenu.CurrentBlueprint);
                            if (ret != null) {
                                var cursorTile = helper.Input.GetCursorPosition().Tile;
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
                int itemID = item.parentSheetIndex;
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    var ret = itemHighlighters[i].highlighter(item, itemID, itemName);
                    if (ret != null) {
                        var cursorTile = helper.Input.GetCursorPosition().Tile;
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
                bool showAllDown = helper.Input.IsDown(config.ShowAllRangesKey);
                if (showAllDown && !showAllDownLastState)
                    showAllToggleState = !showAllToggleState;
                showAllDownLastState = showAllDown;
                for (int i = 0; i < buildingHighlighters.Count; ++i) {
                    buildingHighlighters[i].UpdateHotkeyToggleState(helper);
                    if (showAllToggleState || buildingHighlighters[i].hotkeyToggleState) {
                        runBuildingHighlighter[i] = true;
                        iterateBuildings = true;
                    }
                }
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    itemHighlighters[i].UpdateHotkeyToggleState(helper);
                    if (showAllToggleState || itemHighlighters[i].hotkeyToggleState) {
                        runItemHighlighter[i] = true;
                        iterateItems = true;
                    }
                }
            } else {
                bool showAll = helper.Input.IsDown(config.ShowAllRangesKey);
                for (int i = 0; i < buildingHighlighters.Count; ++i) {
                    if (showAll || buildingHighlighters[i].hotkey is SButton hotkey && helper.Input.IsDown(hotkey)) {
                        runBuildingHighlighter[i] = true;
                        iterateBuildings = true;
                    }
                }
                for (int i = 0; i < itemHighlighters.Count; ++i) {
                    if (showAll || itemHighlighters[i].hotkey is SButton hotkey && helper.Input.IsDown(hotkey)) {
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
                    int itemID = item.parentSheetIndex;
                    for (int i = 0; i < itemHighlighters.Count; ++i) {
                        if (runItemHighlighter[i]) {
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
        }

    }
}
