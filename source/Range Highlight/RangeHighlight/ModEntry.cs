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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace RangeHighlight {
    public class ModEntry : Mod {
        internal ModConfig config;
        internal RangeHighlighter highlighter;
        internal IRangeHighlightAPI api;
        private RangeHighlightAPI _api_private; // for testing non-public stuff
        internal DefaultShapes defaultShapes;
        internal IModHelper helper;
        private Integrations integrations;

        public override void Entry(IModHelper helper) {
            this.helper = helper;
            config = helper.ReadConfig<ModConfig>();
            highlighter = new RangeHighlighter(this.Monitor, helper, config);
            api = _api_private = new RangeHighlightAPI(this);
            defaultShapes = new DefaultShapes(api);
            installDefaultHighlights();
            helper.Events.GameLoop.GameLaunched += onLaunched;
        }

        public override object GetApi() {
            return api;
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e) {
            integrations = new Integrations(this);
        }


        internal class DefaultShapes {
            private readonly IRangeHighlightAPI api;
            public readonly bool[,] sprinkler = {
                { false, true, false},
                { true, false, true },
                { false, true, false}};
            public readonly bool[,] qualitySprinkler;
            public readonly bool[,] iridiumSprinkler;
            public readonly bool[,] iridiumSprinklerWithNozzle;
            public bool[,] prismaticSprinkler;
            public readonly bool[,] beehouse;
            public readonly bool[,] scarecrow;
            public readonly bool[,] deluxeScarecrow;
            public bool[,] junimoHut;
            public readonly struct BombRange {
                public bool[,] range { get; }
                public bool[,] rangeInner { get; }
                public bool[,] rangeOuter { get; }

                public BombRange(bool[,] range, bool[,] rangeInner, bool[,] rangeOuter) {
                    this.range = range;
                    this.rangeInner = rangeInner;
                    this.rangeOuter = rangeOuter;
                }
            }
            public readonly BombRange cherryBomb;
            public readonly BombRange bomb;
            public readonly BombRange megaBomb;

            public DefaultShapes(IRangeHighlightAPI api) {
                this.api = api;
                qualitySprinkler = api.GetSquareCircle(1);
                iridiumSprinkler = api.GetSquareCircle(2);
                iridiumSprinklerWithNozzle = api.GetSquareCircle(3);
                prismaticSprinkler = api.GetSquareCircle(3);
                beehouse = api.GetManhattanCircle(5);
                scarecrow = api.GetCartesianCircleWithTruncate(8);
                deluxeScarecrow = api.GetCartesianCircleWithTruncate(16);
                SetJunimoRange(8);
                cherryBomb = new BombRange(
                    api.GetCartesianCircleWithRound(3, false),
                    new bool[,] {
                        { false, true, false},
                        { true, true, true },
                        { false, true, false}
                    },
                    api.GetSquareCircle(3, false));
                bomb = new BombRange(
                    api.GetCartesianCircleWithRound(5, false),
                    api.GetCartesianCircleWithRound(2, false),
                    api.GetSquareCircle(5, false));
                bool[,] mb = api.GetCartesianCircleWithRound(7, false);
                // yeah, it's strange; but I have the screenshots showing this shape
                mb[1, 5] = mb[1, 6] = mb[1, 7] = mb[1, 8] = mb[1, 9] = false;
                mb[13, 5] = mb[13, 6] = mb[13, 7] = mb[13, 8] = mb[13, 9] = false;
                megaBomb = new BombRange(
                    mb,
                    cherryBomb.range,
                    api.GetSquareCircle(7, false));
            }

            public void SetJunimoRange(uint r) {
                junimoHut = api.GetSquareCircle(r);
                junimoHut[r - 1, r - 1] = junimoHut[r, r - 1] = junimoHut[r + 1, r - 1] = false;
                junimoHut[r - 1, r] = junimoHut[r + 1, r] = false;
            }

            public bool[,] GetSprinkler(string name, bool hasPressureNozzleAttached) {
                if (name.Contains("iridium")) return hasPressureNozzleAttached ? iridiumSprinklerWithNozzle : iridiumSprinkler;
                if (name.Contains("quality")) return hasPressureNozzleAttached ? iridiumSprinkler : qualitySprinkler;
                if (name.Contains("prismatic")) return prismaticSprinkler;
                return hasPressureNozzleAttached ? qualitySprinkler : sprinkler;
            }

            public BombRange GetBomb(string name) {
                if (name.Contains("mega")) return megaBomb;
                if (name.Contains("cherry")) return cherryBomb;
                return bomb;
            }
        }

        internal Tuple<Color, bool[,]> GetDefaultSprinklerHighlight(Item item, int itemID, string itemName) {
            if (itemName.Contains("sprinkler")) {
                bool hasPressureNozzleAttached = false;
                if (item is StardewValley.Object obj) {
                    var heldObj = obj.heldObject.Value;
                    if (heldObj != null && heldObj.ParentSheetIndex == 915) {
                        hasPressureNozzleAttached = true;
                    }
                }
                return new Tuple<Color, bool[,]>(config.SprinklerRangeTint, defaultShapes.GetSprinkler(itemName, hasPressureNozzleAttached));
            } else {
                return null;
            }
        }
        private void installDefaultHighlights() {
            if (config.ShowJunimoRange) {
                api.AddBuildingRangeHighlighter("jltaylor-us.RangeHighlight/junimoHut", config.ShowJunimoRangeKey,
                    blueprint => {
                        if (blueprint.name == "Junimo Hut") {
                            return new Tuple<Color, bool[,], int, int>(config.JunimoRangeTint, defaultShapes.junimoHut, 1, 1);
                        } else {
                            return null;
                        }
                    },
                    building => {
                        if (building is JunimoHut) {
                            return new Tuple<Color, bool[,], int, int>(config.JunimoRangeTint, defaultShapes.junimoHut, 1, 1);
                        } else {
                            return null;
                        }
                    });
            }
            if (config.ShowScarecrowRange) {
                api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/scarecrow", config.ShowScarecrowRangeKey,
                    config.ShowOtherScarecrowsWhenHoldingScarecrow,
                    (item, itemID, itemName) => {
                        if (itemName.Contains("arecrow")) {
                            return new Tuple<Color, bool[,]>(config.ScarecrowRangeTint,
                                itemName.Contains("deluxe") ? defaultShapes.deluxeScarecrow : defaultShapes.scarecrow);
                        } else {
                            return null;
                        }
                    });
            }
            if (config.ShowSprinklerRange) {
                api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/sprinkler", config.ShowSprinklerRangeKey,
                    config.ShowOtherSprinklersWhenHoldingSprinkler, GetDefaultSprinklerHighlight);
            }
            if (config.ShowBeehouseRange) {
                api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/beehouse", config.ShowBeehouseRangeKey,
                    config.ShowOtherBeehousesWhenHoldingBeehouse,
                    (item, itemID, itemName) => {
                        if (itemName.Contains("bee house")) {
                            return new Tuple<Color, bool[,]>(config.BeehouseRangeTint, defaultShapes.beehouse);
                        } else {
                            return null;
                        }
                    });
            }
            if (config.ShowBombRange) {
                api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/bomb", null, true,
                    (item, itemID, itemName) => {
                        DefaultShapes.BombRange range;
                        switch (itemID) {
                            case 286:
                                range = defaultShapes.cherryBomb;
                                break;
                            case 287:
                                range = defaultShapes.bomb;
                                break;
                            case 288:
                                range = defaultShapes.megaBomb;
                                break;
                            default:
                                return null;
                        }
                        // This relies on the fact that placed bombs are not an item, so this
                        // can use the cursor position for the location
                        var cursorTile = highlighter.GetCursorTile();
                        return bombHelper(range, (int)cursorTile.X, (int)cursorTile.Y);
                    });

                if (config.showPlacedBombRange) {
                    // not sure about this API yet, so keeping it private for now
                    highlighter.AddTemporaryAnimatedSpriteHighlighter("jltaylor-us.RangeHighlight/bomb",
                        sprite => {
                            DefaultShapes.BombRange range;
                            switch (sprite.initialParentTileIndex) {
                                case 286:
                                    range = defaultShapes.cherryBomb;
                                    break;
                                case 287:
                                    range = defaultShapes.bomb;
                                    break;
                                case 288:
                                    range = defaultShapes.megaBomb;
                                    break;
                                default:
                                    if (sprite.bombRadius > 0) {
                                        range = new DefaultShapes.BombRange(api.GetCartesianCircleWithRound((uint)sprite.bombRadius, false), new bool[0, 0], api.GetSquareCircle((uint)sprite.bombRadius, false));
                                        break;
                                    } else {
                                        return null;
                                    }
                            }
                            return bombHelper(range,
                                (int)(sprite.position.X / Game1.tileSize), (int)(sprite.position.Y / Game1.tileSize));
                        });
                }
            }
        }

        private Tuple<Color, bool[,]> bombHelper(DefaultShapes.BombRange range, int posX, int posY) {
            if (config.showBombInnerRange) {
                highlighter.AddHighlightTiles(config.BombInnerRangeTint, range.rangeInner, posX, posY);
            }
            if (config.showBombOuterRange) {
                // yes, the effective area is actually offset from center
                highlighter.AddHighlightTiles(config.BombOuterRangeTint, range.rangeOuter, posX - 1, posY - 1);
            }
            return new Tuple<Color, bool[,]>(config.BombRangeTint, range.range);
        }

    }
}
