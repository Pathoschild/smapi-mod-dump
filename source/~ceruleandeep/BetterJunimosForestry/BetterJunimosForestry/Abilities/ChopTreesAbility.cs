/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using SObject = StardewValley.Object;

// bits of this are from Tractor Mod; https://github.com/Pathoschild/StardewMods/blob/68628a40f992288278b724984c0ade200e6e4296/TractorMod/Framework/BaseAttachment.cs#L132

namespace BetterJunimosForestry.Abilities {
    public class ChopTreesAbility : IJunimoAbility {

        private readonly IMonitor Monitor;
        private Axe FakeAxe = new();
        private FakeFarmer FakeFarmer = new();
        
        internal ChopTreesAbility(IMonitor Monitor) {
            this.Monitor = Monitor;
            FakeAxe.UpgradeLevel = 1;
            FakeAxe.IsEfficient = true;
        }

        public string AbilityName() {
            return "ChopTrees";
        }

        private static bool IsHarvestableTree(TerrainFeature t, string mode) {
            if (t is not Tree tree) return false;
            if (tree.tapped.Value) return false;
            switch (mode)
            {
                case Modes.Crops or Modes.Orchard:
                    return true;
                case Modes.Maze:
                    // don't cut any trees in Maze mode because they're already factored in as walls
                    return false;
            }

            if (tree.growthStage.Value < 5) return false;
            if (ModEntry.Config.SustainableWildTreeHarvesting && !tree.hasSeed.Value) return false;
            return true;
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            var mode = Util.GetModeForHut(Util.GetHutFromId(guid));
            if (mode == Modes.Normal) return false;

            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (location.terrainFeatures.ContainsKey(nextPos) && IsHarvestableTree(location.terrainFeatures[nextPos], mode)) {
                    // Monitor.Log($"Pos {nextPos} contains tree", LogLevel.Debug);
                    return true;
                }
            }
            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            var mode = Util.GetModeForHut(Util.GetHutFromId(guid));

            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            var direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (var nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (location.terrainFeatures.ContainsKey(nextPos) && IsHarvestableTree(location.terrainFeatures[nextPos], mode)) {
                    junimo.faceDirection(direction);
                    return UseToolOnTile(FakeAxe, nextPos, Game1.currentLocation);
                }
                direction++;
            }
            return false;
        }

        private bool UseToolOnTile(Tool tool, Vector2 tile, GameLocation location) {
            var (x, y) = GetToolPixelPosition(tile);
            tool.DoFunction(location, (int)x, (int)y, 0, FakeFarmer);
            return true;
        }

        private static Vector2 GetToolPixelPosition(Vector2 tile) {
            return (tile * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
        }

        public List<int> RequiredItems() {
            return new();
        }
        
        
        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid) {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }
        
        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}