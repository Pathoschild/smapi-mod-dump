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
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace RangeHighlight {
    internal class Integrations {
        private ModEntry theMod;
        public Integrations(ModEntry theMod) {
            this.theMod = theMod;
            IntegratePrismaticTools();
            IntegrateBetterJunimos();
            IntegrateBetterSprinklers();
        }

        private void IntegratePrismaticTools() {
            IPrismaticToolsAPI api = theMod.helper.ModRegistry.GetApi<IPrismaticToolsAPI>("stokastic.PrismaticTools");
            if (api == null) return;
            theMod.defaultShapes.prismaticSprinkler = theMod.api.GetSquareCircle((uint)api.SprinklerRange);
        }
        private void IntegrateBetterJunimos() {
            IBetterJunimosAPI api = theMod.helper.ModRegistry.GetApi<IBetterJunimosAPI>("hawkfalcon.BetterJunimos");
            if (api == null) return;
            int r = api.GetJunimoHutMaxRadius();
            if (r > 1) {
                theMod.defaultShapes.SetJunimoRange((uint)r);
            } else {
                theMod.Monitor.Log($"ignoring nonsense value {r} from Better Junimos for Junimo Hut radius", LogLevel.Info);
            }
        }
        private void IntegrateBetterSprinklers() {
            IBetterSprinklersApi api = theMod.helper.ModRegistry.GetApi<IBetterSprinklersApi>("Speeder.BetterSprinklers");
            if (api == null) return;
            theMod.api.RemoveItemRangeHighlighter("jltaylor-us.RangeHighlight/sprinkler");
            theMod.api.AddItemRangeHighlighter("jltaylor-us.RangeHighlight/better-sprinkler",
                theMod.config.ShowSprinklerRangeKey,
                theMod.config.ShowOtherSprinklersWhenHoldingSprinkler,
                (item, itemID, itemName) => {
                    Vector2[] tiles;
                    if (api.GetSprinklerCoverage().TryGetValue(itemID, out tiles)) {
                        return new Tuple<Color, bool[,]>(theMod.config.SprinklerRangeTint, PointsToMask(tiles));
                    } else {
                        return null;
                    }
                });
        }
        private bool[,] PointsToMask(Vector2[] points) {
            int maxX = 0;
            int maxY = 0;
            foreach (var point in points) {
                maxX = Math.Max(maxX, Math.Abs((int)point.X));
                maxY = Math.Max(maxY, Math.Abs((int)point.Y));
            }
            bool[,] result = new bool[maxX * 2 + 1, maxY * 2 + 1];
            foreach (var point in points) {
                result[(int)point.X + maxX, (int)point.Y + maxY] = true;
            }
            return result;
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

}
