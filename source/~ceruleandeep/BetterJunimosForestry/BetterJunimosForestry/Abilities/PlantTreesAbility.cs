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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace BetterJunimosForestry.Abilities
{
    public class PlantTreesAbility : IJunimoAbility
    {
        private readonly IMonitor Monitor;

        internal PlantTreesAbility(IMonitor Monitor)
        {
            this.Monitor = Monitor;
        }

        public string AbilityName()
        {
            return "PlantTrees";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid)
        {
            var mode = Util.GetModeForHut(Util.GetHutFromId(guid));
            if (mode != Modes.Forest) return false;

            var hut = Util.GetHutFromId(guid);

            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            // if (location.IsGreenhouse)
            // {
            //     Monitor.Log($"PlantTrees IsActionAvailable {location.Name} {x} {y}", LogLevel.Debug);
            // }
            
            Vector2[] positions = {up, right, down, left};
            foreach (var nextPos in positions)
            {
                // if (location.IsGreenhouse)
                // {
                //     Monitor.Log($"    PlantTrees IsActionAvailable {location.Name} {nextPos.X} {nextPos.Y}" +
                //                 $"within radius {Util.IsWithinRadius(location, hut, nextPos)} " +
                //                 $"should plant {ShouldPlantWildTreeHere(location, hut, nextPos)} ", LogLevel.Debug);
                // }
                if (!Util.IsWithinRadius(location, hut, nextPos)) continue;
                if (ShouldPlantWildTreeHere(location, hut, nextPos)) return true;
            }

            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            var hut = Util.GetHutFromId(guid);
            var chest = hut.output.Value;
            var foundItem = chest.items.FirstOrDefault(item =>
                item != null && Util.WildTreeSeeds.Keys.Contains(item.ParentSheetIndex));
            if (foundItem == null) return false;

            var (x, y) = pos;
            var up = new Vector2(x, y + 1);
            var right = new Vector2(x + 1, y);
            var down = new Vector2(x, y - 1);
            var left = new Vector2(x - 1, y);

            Vector2[] positions = {up, right, down, left};
            foreach (var nextPos in positions)
            {
                if (!Util.IsWithinRadius(location, hut, nextPos)) continue;
                if (!ShouldPlantWildTreeHere(location, hut, nextPos)) continue;
                if (!Plant(location, nextPos, foundItem.ParentSheetIndex)) continue;
                Util.RemoveItemFromChest(chest, foundItem);
                return true;
            }

            return false;
        }

        // is this tile plantable? 
        internal static bool ShouldPlantWildTreeHere(GameLocation location, JunimoHut hut, Vector2 pos)
        {
            if (Util.BlocksDoor(location, hut, pos)) return false;

            // is this tile in the planting pattern?
            return IsTileInPattern(pos) && Plantable(location, pos);
        }

        internal static bool IsTileInPattern(Vector2 pos)
        {
            var (x, y) = pos;
            return ModEntry.Config.WildTreePattern switch
            {
                "tight" => x % 2 == 0,
                "loose" => x % 2 == 0 && y % 2 == 0,
                "fruity-tight" => x % 3 == 0 && y % 3 == 0,
                "fruity-loose" when x % 4 == 2 => y % 2 == 0,
                "fruity-loose" when x % 4 == 0 => y % 2 == 0,
                "fruity-loose" => false,
                _ => throw new ArgumentOutOfRangeException(
                    $"Pattern '{ModEntry.Config.WildTreePattern}' not recognized")
            };
        }

        // is this tile plantable?
        private static bool Plantable(GameLocation location, Vector2 pos)
        {
            // is something standing on it? an impassable building? a terrain feature?
            // we want to use the game's occupied check, but also allow for un-hoeing empty hoedirt
            if (location.isTileOccupied(pos) && !Util.IsHoed(location, pos)) return false;

            if (Util.HasCrop(location, pos)) return false;
            if (Util.IsOccupied(location, pos)) return false;
            if (Util.SpawningTreesForbidden(location, pos)) return false;
            if (!Util.CanBeHoed(location, pos)) return false;
            return true;
        }

        private bool Plant(GameLocation location, Vector2 pos, int index)
        {
            // check if the tile needs to be un-hoed
            if (location.terrainFeatures.TryGetValue(pos, out var feature))
            {
                if (feature is HoeDirt {crop: null})
                {
                    location.terrainFeatures.Remove(pos);
                }
            }

            if (location.terrainFeatures.Keys.Contains(pos))
            {
                Monitor.Log($"    Plant: {pos.X} {pos.Y} still somethign on tile", LogLevel.Debug);
                return false;
            }

            var tree = new Tree(Util.WildTreeSeeds[index], ModEntry.Config.PlantWildTreesSize);
            location.terrainFeatures.Add(pos, tree);

            if (Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, location))
            {
                location.playSound("stoneStep");
                location.playSound("dirtyHit");
            }

            ++Game1.stats.SeedsSown;
            return true;
        }


        public List<int> RequiredItems()
        {
            return Util.WildTreeSeeds.Keys.ToList();
        }


        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid)
        {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}