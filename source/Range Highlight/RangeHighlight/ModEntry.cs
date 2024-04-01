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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace RangeHighlight {
    // why the tiny wrapper class?  So I don't have to play games with the
    // nullability checking for all of the fields in TheMod.
    public class ModEntry : Mod {
        internal TheMod? theMod;
        public override void Entry(IModHelper helper) {
            theMod = new TheMod(helper, Monitor, ModManifest);
        }

        public override object? GetApi() {
            return theMod?.GetApi();
        }
    }
    public class TheMod {
        internal IMonitor Monitor { get; }
        internal IManifest ModManifest { get; }
        internal ModConfig config;
        internal RangeHighlighter highlighter;
        internal IRangeHighlightAPI api;
        private RangeHighlightAPI _api_private; // for testing non-public stuff
        internal DefaultShapes defaultShapes;
        internal IModHelper helper;
        internal IModHelper Helper => helper;
        private Integrations? integrations;

        public TheMod(IModHelper helper, IMonitor monitor, IManifest modManifest) {
            this.helper = helper;
            this.Monitor = monitor;
            this.ModManifest = modManifest;
            I18n.Init(helper.Translation);
            try {
                config = helper.ReadConfig<ModConfig>();
            } catch (Exception e) {
                Monitor.Log($"Error reading configuration file: {e}\nConfiguration will be replaced with the default configuration, overwriting your old configuration file on the next save.", LogLevel.Warn);
                config = new ModConfig();
            }
            highlighter = new RangeHighlighter(this);
            api = _api_private = new RangeHighlightAPI(this);
            defaultShapes = new DefaultShapes(api);
            helper.Events.GameLoop.GameLaunched += onLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        public object GetApi() {
            return api;
        }

        private void onLaunched(object? sender, GameLaunchedEventArgs e) {
            ModConfig.RegisterGMCM(this);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
            installDefaultHighlights();
            integrations = new Integrations(this);
            Helper.Events.GameLoop.UpdateTicked += UIInfoSuiteWarning;
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) {
            integrations = null;
            highlighter?.ClearAllHighlighters();
        }

        private void UIInfoSuiteWarning(object? sender, EventArgs e) {
            Helper.Events.GameLoop.UpdateTicked -= UIInfoSuiteWarning;
            if (!config.ShowBeehouseRange && !config.ShowJunimoRange && !config.ShowScarecrowRange && !config.ShowSprinklerRange) {
                return;
            }
            if (this.Helper.ModRegistry.Get("Annosz.UiInfoSuite2") is IModInfo info) {
                var bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static;
                IMod? mod = info.GetType().GetProperty("Mod", bindingFlags)?.GetValue(info) as IMod;
                object? opts = mod?.GetType().GetField("_modOptions", bindingFlags)?.GetValue(mod);
                bool? showRangesEnabled = opts?.GetType().GetProperty("ShowItemEffectRanges", bindingFlags)?.GetValue(opts) as bool?;
                if (showRangesEnabled is true) {
                    Monitor.Log($"Both {ModManifest.Name} and {info.Manifest.Name} are configured to show item effect ranges.  You probably don't want that.", LogLevel.Warn);
                }

            }
        }


        internal class DefaultShapes {
            private readonly IRangeHighlightAPI api;
            public uint junimoHutDefaultRange;
            public readonly bool[,] sprinkler = {
                { false, true, false},
                { true, false, true },
                { false, true, false}};
            public readonly bool[,] qualitySprinkler;
            public readonly bool[,] iridiumSprinkler;
            public readonly bool[,] iridiumSprinklerWithNozzle;
            private readonly Dictionary<uint, bool[,]> sprinklerRangeByRadius = new();
            public bool[,] prismaticSprinkler;
            public bool[,] radioactiveSprinkler;
            public readonly bool[,] beehouse;
            public readonly bool[,] mushroomLog;
            public const int scarecrowRadius = 8;
            public readonly bool[,] scarecrow;
            public const int deluxeScarecrowRadius = 16;
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
                sprinklerRangeByRadius.Add(0, sprinkler);
                sprinklerRangeByRadius.Add(1, qualitySprinkler);
                sprinklerRangeByRadius.Add(2, iridiumSprinkler);
                sprinklerRangeByRadius.Add(3, iridiumSprinklerWithNozzle);
                prismaticSprinkler = api.GetSquareCircle(3);
                radioactiveSprinkler = api.GetSquareCircle(3);
                beehouse = api.GetManhattanCircle(5);
                mushroomLog = api.GetSquareCircle(3);
                scarecrow = api.GetCartesianCircleWithTruncate(scarecrowRadius);
                deluxeScarecrow = api.GetCartesianCircleWithTruncate(deluxeScarecrowRadius);
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

            [MemberNotNull(nameof(junimoHut))]
            public void SetJunimoRange(uint r) {
                junimoHut = MakeJunimoShape(r);
                junimoHutDefaultRange = r;
            }

            public bool[,] MakeJunimoShape(uint r) {
                bool[,] result = api.GetSquareCircle(r);
                result[r - 1, r - 1] = result[r, r - 1] = result[r + 1, r - 1] = false;
                result[r - 1, r] = result[r + 1, r] = false;
                return result;
            }

            public bool[,] GetSprinkler(string name, bool hasPressureNozzleAttached) {
                if (name.Contains("iridium")) return hasPressureNozzleAttached ? iridiumSprinklerWithNozzle : iridiumSprinkler;
                if (name.Contains("quality")) return hasPressureNozzleAttached ? iridiumSprinkler : qualitySprinkler;
                if (name.Contains("prismatic")) return prismaticSprinkler;
                if (name.Contains("radioactive")) return radioactiveSprinkler;
                return hasPressureNozzleAttached ? qualitySprinkler : sprinkler;
            }

            public bool[,] GetSprinkler(uint radius) {
                bool[,]? result;
                if (!sprinklerRangeByRadius.TryGetValue(radius, out result)) {
                    result = api.GetSquareCircle(radius);
                    sprinklerRangeByRadius.Add(radius, result);
                }
                return result;
            }

            public BombRange GetBomb(string name) {
                if (name.Contains("mega")) return megaBomb;
                if (name.Contains("cherry")) return cherryBomb;
                return bomb;
            }
        }

        internal Tuple<Color, bool[,]>? GetDefaultSprinklerHighlight(Item item) {
            if (item is StardewValley.Object obj) {
                if (obj.IsSprinkler()) {
                    int radius = obj.GetModifiedRadiusForSprinkler();
                    if (radius < 0) {
                        // nonsense
                        return null;
                    }
                    return new Tuple<Color, bool[,]>(config.SprinklerRangeTint, defaultShapes.GetSprinkler((uint)radius));
                }
            }
            // Previous implementation:
            //string itemName = item.Name.ToLowerInvariant();
            //if (itemName.Contains("sprinkler")) {
            //    bool hasPressureNozzleAttached = false;
            //    if (item is StardewValley.Object obj) {
            //        var heldObj = obj.heldObject.Value;
            //        if (heldObj != null && heldObj.ParentSheetIndex == 915) {
            //            hasPressureNozzleAttached = true;
            //        }
            //    }
            //    return new Tuple<Color, bool[,]>(config.SprinklerRangeTint, defaultShapes.GetSprinkler(itemName, hasPressureNozzleAttached));
            //}
            return null;
        }
        private void installDefaultHighlights() {
            api.AddBuildingRangeHighlighter("jltaylor-us.RangeHighlight/junimoHut",
                () => config.ShowJunimoRange,
                () => config.ShowJunimoRangeKey,
                blueprint => {
                    if (blueprint.Id == "Junimo Hut") {
                        return new Tuple<Color, bool[,], int, int>(config.JunimoRangeTint, defaultShapes.junimoHut, 1, 1);
                    } else {
                        return null;
                    }
                },
                building => {
                    if (building is JunimoHut junimoHut) {
                        // junimoHut.cropHarvestRadius can be set per-building in SDV 1.6.
                        bool[,] shape = junimoHut.cropHarvestRadius == defaultShapes.junimoHutDefaultRange ? defaultShapes.junimoHut : defaultShapes.MakeJunimoShape((uint)junimoHut.cropHarvestRadius);
                        return new Tuple<Color, bool[,], int, int>(config.JunimoRangeTint, shape, 1, 1);
                    } else {
                        return null;
                    }
                });
            api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/scarecrow",
                () => config.ShowScarecrowRange,
                () => config.ShowScarecrowRangeKey,
                () => config.ShowOtherScarecrowsWhenHoldingScarecrow,
                (item) => {
                    if (item is StardewValley.Object sobj && sobj.IsScarecrow()) {
                        int r = sobj.GetRadiusForScarecrow() - 1;
                        if (r < 0) return null; // shouldn't happen?
                        return new Tuple<Color, bool[,]>(config.ScarecrowRangeTint,
                            r == DefaultShapes.scarecrowRadius ? defaultShapes.scarecrow
                                : r == DefaultShapes.deluxeScarecrowRadius ? defaultShapes.deluxeScarecrow
                                : api.GetCartesianCircleWithTruncate((uint)r));
                    }
                    return null;
                });
            api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/sprinkler",
                () => config.ShowSprinklerRange,
                () => config.ShowSprinklerRangeKey,
                () => config.ShowOtherSprinklersWhenHoldingSprinkler,
                GetDefaultSprinklerHighlight);
            api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/beehouse",
                () => config.ShowBeehouseRange,
                () => config.ShowBeehouseRangeKey,
                () => config.ShowOtherBeehousesWhenHoldingBeehouse,
                (item) => {
                    // This big mess finds machines that might use a nearby flower as input.
                    // Let's assume that they are beehouses, or at least something that the
                    // user probably wants to have highlighted like a beehouse.
                    if (item is StardewValley.Object obj) {
                        var machineData = obj.GetMachineData();
                        if (machineData is not null && machineData.OutputRules is not null) {
                            foreach (var rule in machineData.OutputRules) {
                                foreach (var outputItem in rule.OutputItem) {
                                    foreach (string s in ArgUtility.SplitBySpace(outputItem.ItemId)) {
                                        if (s == "NEARBY_FLOWER_ID") {
                                            return new Tuple<Color, bool[,]>(config.BeehouseRangeTint, defaultShapes.beehouse);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Previously we just matched on the name, which is a whole lot simpler.
                    //if (item.Name.ToLowerInvariant().Contains("bee house")) {
                    //    return new Tuple<Color, bool[,]>(config.BeehouseRangeTint, defaultShapes.beehouse);
                    //}
                    return null;
                });
            api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/mushroom-log",
                () => config.ShowMushroomLogRange,
                () => config.ShowMushroomLogRangeKey,
                () => config.ShowOtherMushroomLogsWhenHoldingMushroomLog,
                (item) => {
                    if (item.QualifiedItemId == "(BC)MushroomLog") {
                        return new Tuple<Color, bool[,]>(config.MushroomLogRangeTint, defaultShapes.mushroomLog);
                    }
                    // we _could_ instead search the object's MachineData (as beehouses do above) and look for
                    // things with an output rule that has an outputItem that has an OutputMethod that is
                    // StardewValley.Object.OutputMushroomLog, but that seems awfully complicated given that
                    // any modded item defined that way might not _want_ a highlight in the same color.
                    return null;
                });
            bool bombsShownThisFrame = false;
            api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/bomb",
                () => config.ShowBombRange && config.showHeldBombRange,
                () => new KeybindList(),
                () => true,
                () => { bombsShownThisFrame = false; },
                (item) => {
                    if (config.hideHeldBombWhenPlacedAreTicking && bombsShownThisFrame) return null;
                    if (!Utility.IsNormalObjectAtParentSheetIndex(item, item.ItemId)) return null;
                    return bombHelper(item.ParentSheetIndex, 0);
                },
                null);
            api.AddTemporaryAnimatedSpriteHighlighter("jltaylor-us.RangeHighlight/bomb",
                () => config.ShowBombRange && config.showPlacedBombRange,
                sprite => {
                    var result = bombHelper(sprite.initialParentTileIndex, sprite.bombRadius);
                    if (result is not null) {
                        bombsShownThisFrame = true;
                    }
                    return result;
                });
        }

        private List<Tuple<Color, bool[,]>>? bombHelper(int itemID, int defaultRadius) {
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
                    if (defaultRadius <= 0) return null;
                    range = new DefaultShapes.BombRange(api.GetCartesianCircleWithRound((uint)defaultRadius, false), new bool[0, 0], api.GetSquareCircle((uint)defaultRadius, false));
                    break;
            }
            List<Tuple<Color, bool[,]>> result = new();
            if (config.showBombInnerRange) {
                result.Add(new Tuple<Color, bool[,]>(config.BombInnerRangeTint, range.rangeInner));
            }
            if (config.showBombOuterRange) {
                result.Add(new Tuple<Color, bool[,]>(config.BombOuterRangeTint, range.rangeOuter));
            }
            result.Add(new Tuple<Color, bool[,]>(config.BombRangeTint, range.range));
            return result;
        }

    }
}
