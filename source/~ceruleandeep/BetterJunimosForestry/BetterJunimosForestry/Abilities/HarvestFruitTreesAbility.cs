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
using System.Linq;
using BetterJunimos.Abilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace BetterJunimosForestry.Abilities {
    public class HarvestFruitTreesAbility : IJunimoAbility {
        private readonly IMonitor Monitor;

        internal HarvestFruitTreesAbility(IMonitor Monitor) {
            this.Monitor = Monitor;
        }
        
        public string AbilityName() {
            return "HarvestFruitTrees";
        }

        private static bool IsHarvestableFruitTree(TerrainFeature tf) {
            return tf is FruitTree tree && tree.fruitsOnTree.Value > 0;
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            var up = new Vector2(pos.X, pos.Y + 1);
            var right = new Vector2(pos.X + 1, pos.Y);
            var down = new Vector2(pos.X, pos.Y - 1);
            var left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = { up, right, down, left };
            return positions
                .Where(nextPos => Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos))
                .Any(nextPos => location.terrainFeatures.ContainsKey(nextPos) 
                                && IsHarvestableFruitTree(location.terrainFeatures[nextPos]));
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            var up = new Vector2(pos.X, pos.Y + 1);
            var right = new Vector2(pos.X + 1, pos.Y);
            var down = new Vector2(pos.X, pos.Y - 1);
            var left = new Vector2(pos.X - 1, pos.Y);

            var direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (var nextPos in positions) {
                if (!Util.IsWithinRadius(location, Util.GetHutFromId(guid), nextPos)) continue;
                if (location.terrainFeatures.ContainsKey(nextPos) && IsHarvestableFruitTree(location.terrainFeatures[nextPos])) {
                    var tree = location.terrainFeatures[nextPos] as FruitTree;

                    junimo.faceDirection(direction);

                    return HarvestFromTree(pos, junimo, tree);
                }
                direction++;
            }

            return false;
        }

        /// <summary>Harvest fruit from a FruitTree and update the tree accordingly.</summary>
        private static SObject GetFruitFromTree(FruitTree tree) {
            if (tree.fruitsOnTree.Value == 0)
                return null;

            var quality = 0;
            if (tree.daysUntilMature.Value <= -112)
                quality = 1;
            if (tree.daysUntilMature.Value <= -224)
                quality = 2;
            if (tree.daysUntilMature.Value <= -336)
                quality = 4;
            if (tree.struckByLightningCountdown.Value > 0)
                quality = 0;

            tree.fruitsOnTree.Value --;

            var result = new SObject(Vector2.Zero, tree.struckByLightningCountdown.Value > 0 ? 382 : tree.indexOfFruit.Value, 1) { Quality = quality };
            return result;
        }

        private static bool HarvestFromTree(Vector2 pos, JunimoHarvester junimo, FruitTree tree) {
            //shake the tree without it releasing any fruit
            var fruitsOnTree = tree.fruitsOnTree.Value;
            tree.fruitsOnTree.Value = 0;
            tree.performUseAction(pos, junimo.currentLocation);
            tree.fruitsOnTree.Value = fruitsOnTree;
            var result = GetFruitFromTree(tree);
            if (result == null) return false;
            junimo.tryToAddItemToHut(result);
            return true;
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