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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using StardewModdingAPI;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace BetterJunimosForestry.Abilities
{
    public class PlantFruitTreesAbility : IJunimoAbility
    {
        private List<int> _RequiredItems;
        private readonly IMonitor Monitor;

        internal PlantFruitTreesAbility(IMonitor Monitor)
        {
            this.Monitor = Monitor;
        }
        
        public string AbilityName()
        {
            return "PlantFruitTrees";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid)
        {
            JunimoHut hut = Util.GetHutFromId(guid);

            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = {up, right, down, left};
            foreach (var nextPos in positions)
            {
                if (!Util.IsWithinRadius(location, hut, pos)) continue;
                if (ShouldPlantFruitTreeOnTile(location, hut, nextPos)) return true;
            }

            return false;
        }

        internal static bool ShouldPlantFruitTreeOnTile(GameLocation location, JunimoHut hut, Vector2 pos)
        {
            if (Util.BlocksDoor(location, hut, pos)) return false;
            return IsTileInPattern(pos) && CanPlantFruitTreeOnTile(location, pos);
        }

        private static bool CanPlantFruitTreeOnTile(GameLocation farm, Vector2 pos)
        {
            if (FruitTree.IsGrowthBlocked(pos, farm)) return false;
            if (Util.IsOccupied(farm, pos)) return false;
            if (Util.IsHoed(farm, pos)) return false;
            return true;
        }

        internal static bool TileIsNextToAPlantableTile(GameLocation farm, JunimoHut hut, Vector2 pos)
        {
            // why isn't IsGrowthBlocked enough?
            for (var x = -1; x < 2; x++)
            {
                for (var y = -1; y < 2; y++)
                {
                    var v = new Vector2(pos.X + x, pos.Y + y);
                    if (ShouldPlantFruitTreeOnTile(farm, hut, v))
                    {
                        // Monitor.Log($"TileIsNextToAPlantableTile [{pos.X}, {pos.Y}]: neighbour tile [{v.X}, {v.Y}] should be planted", LogLevel.Info);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsTileInPattern(Vector2 pos)
        {
            return ModEntry.Config.FruitTreePattern switch
            {
                "rows" => pos.X % 3 == 0 && pos.Y % 3 == 0,
                "diagonal" when pos.X % 4 == 2 => pos.Y % 4 == 2,
                "diagonal" when pos.X % 4 == 0 => pos.Y % 4 == 0,
                "diagonal" => false,
                "tight" when pos.Y % 2 == 0 => pos.X % 4 == 0,
                "tight" when pos.Y % 2 == 1 => pos.X % 4 == 2,
                "tight" => false,
                _ => throw new ArgumentOutOfRangeException(
                    $"Pattern '{ModEntry.Config.FruitTreePattern}' not recognized")
            };
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            var hut = Util.GetHutFromId(guid);
            var chest = hut.output.Value;
            var foundItem = chest.items.FirstOrDefault(item => item != null && RequiredItems().Contains(item.ParentSheetIndex));
            if (foundItem == null) return false;

            var up = new Vector2(pos.X, pos.Y + 1);
            var right = new Vector2(pos.X + 1, pos.Y);
            var down = new Vector2(pos.X, pos.Y - 1);
            var left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = {up, right, down, left};
            if (!positions.Where(nextPos => Util.IsWithinRadius(location, hut, pos))
                .Where(nextPos => ShouldPlantFruitTreeOnTile(location, hut, nextPos))
                .Any(nextPos => Plant(location, nextPos, foundItem.ParentSheetIndex))) return false;
            Util.RemoveItemFromChest(chest, foundItem);
            return true;
        }

        private static bool Plant(GameLocation farm, Vector2 pos, int index)
        {
            if (farm.terrainFeatures.Keys.Contains(pos))
            {
                return false;
            }

            var tree = new FruitTree(index, ModEntry.Config.PlantFruitTreesSize);
            farm.terrainFeatures.Add(pos, tree);

            if (!Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, farm)) return true;
            farm.playSound("stoneStep");
            farm.playSound("dirtyHit");

            return true;
        }
        
        public List<int> RequiredItems()
        {
            // this is heavy, cache it
            if (_RequiredItems is not null) return _RequiredItems;
            var saplings = Game1.objectInformation.Where(pair => pair.Value.Split('/')[0].Contains("Sapling"));
            _RequiredItems = (from kvp in saplings select kvp.Key).ToList();
            return _RequiredItems;
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