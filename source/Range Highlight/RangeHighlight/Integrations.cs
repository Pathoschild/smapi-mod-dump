/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020-2024 Jamie Taylor
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using static RangeHighlight.TheMod;

namespace RangeHighlight {
    internal class Integrations {
        private TheMod theMod;
        public Integrations(TheMod theMod) {
            this.theMod = theMod;
            IntegratePrismaticTools();
            IntegrateRadioactiveTools();
            IntegrateBetterJunimos();
            IntegrateBetterBeehouses();
            IntegrateBetterSprinklers();
            IntegrateSimpleSprinklers();
            IntegrateLineSprinklers();
        }

        private void IntegratePrismaticTools() {
            IPrismaticToolsAPI? api = theMod.helper.ModRegistry.GetApi<IPrismaticToolsAPI>("stokastic.PrismaticTools");
            if (api == null) return;
            theMod.defaultShapes.prismaticSprinkler = theMod.api.GetSquareCircle((uint)api.SprinklerRange);
        }
        private void IntegrateRadioactiveTools() {
            RadioactiveToolsAPI? api = theMod.helper.ModRegistry.GetApi<RadioactiveToolsAPI>("kakashigr.RadioactiveTools");
            if (api == null) return;
            theMod.defaultShapes.radioactiveSprinkler = theMod.api.GetSquareCircle((uint)api.SprinklerRange);
        }
        private void IntegrateBetterJunimos() {
            IBetterJunimosAPI? api = theMod.helper.ModRegistry.GetApi<IBetterJunimosAPI>("hawkfalcon.BetterJunimos");
            if (api == null) return;
            // Lots of duplicated code here, but it's the best we can do without adding something to the
            // api just for the purpose of letting us fiddle with internal structures
            void setRange() {
                int r = api.GetJunimoHutMaxRadius();
                if (r > 1) {
                    theMod.defaultShapes.SetJunimoRange((uint)r);
                } else {
                    theMod.Monitor.LogOnce($"ignoring nonsense value {r} from Better Junimos for Junimo Hut radius", LogLevel.Info);
                }
            }
            theMod.api.RemoveBuildingRangeHighlighter("jltaylor-us.RangeHighlight/junimoHut");
            theMod.api.AddBuildingRangeHighlighter("jltaylor-us.RangeHighlight/better-junimoHut",
                () => theMod.config.ShowJunimoRange,
                () => theMod.config.ShowJunimoRangeKey,
                blueprint => {
                    if (blueprint.Id == "Junimo Hut") {
                        setRange();
                        return new Tuple<Color, bool[,], int, int>(theMod.config.JunimoRangeTint, theMod.defaultShapes.junimoHut, 1, 1);
                    } else {
                        return null;
                    }
                },
                building => {
                    setRange();
                    if (building is JunimoHut) {
                        // junimoHut.cropHarvestRadius can be set per-building in SDV 1.6, but who knows what Better Junimos is doing with that
                        setRange();
                        return new Tuple<Color, bool[,], int, int>(theMod.config.JunimoRangeTint, theMod.defaultShapes.junimoHut, 1, 1);
                    } else {
                        return null;
                    }
                });
        }
        private void IntegrateBetterBeehouses() {
            IBetterBeehousesAPI? api = theMod.helper.ModRegistry.GetApi<IBetterBeehousesAPI>("tlitookilakin.BetterBeehouses");
            if (api == null) return;
            theMod.api.RemoveItemRangeHighlighter("jltaylor-us.RangeHighlight/beehouse");
            bool[,] beehouseShape = { };
            int lastVal = 0;
            // lots of dupilicated code with the default highlighter...
            // probably could simplify this a lot by changing defaultShapes.beehouse instead
            theMod.api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/better-beehouses",
                () => theMod.config.ShowBeehouseRange,
                () => theMod.config.ShowBeehouseRangeKey,
                () => theMod.config.ShowOtherBeehousesWhenHoldingBeehouse,
                () => {
                    int r = api.GetSearchRadius();
                    if (r != lastVal) {
                        lastVal = r;
                        if (r > 1) {
                            beehouseShape = theMod.api.GetManhattanCircle((uint)r);
                        } else {
                            theMod.Monitor.Log($"ignoring nonsense value {r} from Better Beehouses for Flower search radius", LogLevel.Info);
                            beehouseShape = theMod.defaultShapes.beehouse;
                        }
                    }
                },
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
                                            return new List<Tuple<Color, bool[,]>>(1) { new(theMod.config.BeehouseRangeTint, beehouseShape) };
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
                },
                () => { });

        }
        private void IntegrateSprinklerCommon(string highlighterName, Func<IDictionary<int,Vector2[]>> getCoverage, bool fallbackToDefault) {
            theMod.api.RemoveItemRangeHighlighter("jltaylor-us.RangeHighlight/sprinkler");
            IDictionary<int, bool[,]> coverageMask = new Dictionary<int, bool[,]>();
            theMod.api.AddItemRangeHighlighter(highlighterName,
                () => theMod.config.ShowSprinklerRange,
                () => theMod.config.ShowSprinklerRangeKey,
                () => theMod.config.ShowOtherSprinklersWhenHoldingSprinkler,
                () => {
                    foreach(var entry in getCoverage()) {
                        coverageMask[entry.Key] = theMod.PointsToMask(entry.Value);
                    }
                },
                (item) => {
                    if (coverageMask.TryGetValue(item.ParentSheetIndex, out bool[,]? tiles)) {
                        return new List<Tuple<Color, bool[,]>>(1) { new (theMod.config.SprinklerRangeTint, tiles) };
                    } else if (fallbackToDefault) {
                        var x = theMod.GetDefaultSprinklerHighlight(item);
                        if (x is null) return null;
                        return new List<Tuple<Color, bool[,]>>(1) { x };
                    } else {
                        return null;
                    }
                },
                () => {
                    coverageMask.Clear();
                });
        }
        private void IntegrateBetterSprinklers() {
            IBetterSprinklersApi? api = theMod.helper.ModRegistry.GetApi<IBetterSprinklersApi>("Speeder.BetterSprinklers");
            if (api == null) return;
            IntegrateSprinklerCommon("jltaylor-us.RangeHighlight/better-sprinkler", api.GetSprinklerCoverage, false);
        }
        private void IntegrateSimpleSprinklers() {
            ISimplerSprinklerApi? api = theMod.helper.ModRegistry.GetApi<ISimplerSprinklerApi>("tZed.SimpleSprinkler");
            if (api == null) return;
            IntegrateSprinklerCommon("jltaylor-us.RangeHighlight/simple-sprinkler", api.GetNewSprinklerCoverage, false);
        }
        private void IntegrateLineSprinklers() {
            ILineSprinklersApi? api = theMod.helper.ModRegistry.GetApi<ILineSprinklersApi>("hootless.LineSprinklers");
            if (api == null) return;
            IntegrateSprinklerCommon("jltaylor-us.RangeHighlight/line-sprinkler", api.GetSprinklerCoverage, true);
        }
    }

    public interface IPrismaticToolsAPI {
        int SprinklerRange { get; }
        int SprinklerIndex { get; }
    }

    public interface IBetterJunimosAPI {
        int GetJunimoHutMaxRadius();
    }

    public interface IBetterSprinklersApi {
        int GetMaxGridSize();
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
    public interface ISimplerSprinklerApi {
        IDictionary<int, Vector2[]> GetNewSprinklerCoverage();
    }
    public interface ILineSprinklersApi {
        int GetMaxGridSize();
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
    public interface RadioactiveToolsAPI {
        int SprinklerRange { get; }
        int SprinklerIndex { get; }
    }
    public interface IBetterBeehousesAPI {
        // removed in 2.0.0
        // public bool GetEnabledHere(GameLocation location, bool isWinter);
        public int GetSearchRadius();
    }
}
