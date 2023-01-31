/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using DynamicGameAssets.Game;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OrnithologistsGuild.Game.Critters;
using StardewValley;
using StardewValley.TerrainFeatures;
using OrnithologistsGuild.Content;
using System.Linq;
using HarmonyLib;
using OrnithologistsGuild.Models;
using StardewValley.Locations;

namespace OrnithologistsGuild.Game
{
    public enum PerchType
    {
        MapTile,
        Bath,
        Feeder,
        Tree
    }

    public class Perch
    {
        public Vector3 Position; // NOT tile

        public Vector2? MapTile;
        public CustomBigCraftable Bath;
        public CustomBigCraftable Feeder;
        public Tree Tree;

        public PerchType? Type
        {
            get
            {
                if (MapTile.HasValue) return PerchType.MapTile;
                else if (Bath != null) return PerchType.Bath;
                else if (Feeder != null) return PerchType.Feeder;
                else if (Tree != null) return PerchType.Tree;
                return null;
            }
        }

        public Perch(Vector2 mapTile, int zOffset)
        {
            MapTile = mapTile;
            Position = new Vector3(
                MapTile.Value.X * Game1.tileSize,
                MapTile.Value.Y * Game1.tileSize,
                zOffset);

            // Center on tile
            Position.X += Game1.tileSize / 2;
            Position.Y += Game1.tileSize / 2;

            Position.Z = zOffset;
        }
        public Perch(Tree tree)
        {
            Tree = tree;

            var height = tree.getRenderBounds(tree.currentTileLocation).Height;
            Position = new Vector3(
                tree.currentTileLocation.X * Game1.tileSize,
                tree.currentTileLocation.Y * Game1.tileSize,
                -MathF.Ceiling((float)(height) / 1.65f));

            // Center on tile
            Position.X += Game1.tileSize / 2;
            Position.Y += Game1.tileSize / 2;
        }
        public Perch(CustomBigCraftable customBigCraftable)
        {
            var perchType = GetCustomBigCraftablePerchType(customBigCraftable);

            if (perchType == PerchType.Bath)
            {
                Bath = customBigCraftable;

                var bathDef = BathDef.FromBath(Bath);
                Position = new Vector3(
                    Bath.TileLocation.X * Game1.tileSize,
                    Bath.TileLocation.Y * Game1.tileSize,
                    bathDef.ZOffset);

                // Center on tile
                Position.X += Game1.tileSize / 2;
                Position.Y += Game1.tileSize / 2;
            } else if (perchType == PerchType.Feeder)
            {
                Feeder = customBigCraftable;

                var feederDef = FeederDef.FromFeeder(Feeder);
                Position = new Vector3(
                    Feeder.TileLocation.X * Game1.tileSize,
                    Feeder.TileLocation.Y * Game1.tileSize,
                    feederDef.ZOffset);

                // Center on tile
                Position.X += Game1.tileSize / 2;
                Position.Y += Game1.tileSize / 2;
            }
        }

        public override bool Equals(object obj)
        {
            var perch = obj as Perch;

            if (perch == null) return false;
            else if (perch.Type == PerchType.MapTile && this.Type == PerchType.MapTile)
                return perch.MapTile.Equals(this.MapTile);
            else if (perch.Type == PerchType.Bath && this.Type == PerchType.Bath)
                return perch.Bath.TileLocation.Equals(Bath.TileLocation);
            else if (perch.Type == PerchType.Feeder && this.Type == PerchType.Feeder)
                return perch.Feeder.TileLocation.Equals(Feeder.TileLocation);
            else if (perch.Type == PerchType.Tree && this.Type == PerchType.Tree)
                return perch.Tree.currentTileLocation.Equals(Tree.currentTileLocation);
            return false;
        }

        public override int GetHashCode()
        {
            if (Type == PerchType.MapTile) return MapTile.GetHashCode();
            else if (Type == PerchType.Bath) return Bath.GetHashCode();
            else if (Type == PerchType.Feeder) return Feeder.GetHashCode();
            else if (Type == PerchType.Tree) return Tree.GetHashCode();
            return 0;
        }

        public static PerchType? GetCustomBigCraftablePerchType(CustomBigCraftable customBigCraftable)
        {
            if (ContentManager.BathIds.Contains(customBigCraftable.Id)) return PerchType.Bath;
            else if (ContentManager.FeederIds.Contains(customBigCraftable.Id)) return PerchType.Feeder;
            return null;
        }

        public BetterBirdie GetOccupant(GameLocation location)
        {
            IEnumerable<BetterBirdie> birdies = location.critters.Where(c => c is BetterBirdie && ((BetterBirdie)c).IsPerched).Select(c => (BetterBirdie)c);
            return birdies.FirstOrDefault(birdie => birdie.Perch.Equals(this));
        }

        public static Perch GetRandomAvailableMapPerch(GameLocation location, BetterBirdie birdie, List<Perch> occupiedPerches)
        {
            try
            {
                // Get all map perches
                var allMapPerches = Utilities.Randomize(Game1.currentLocation.getMapProperty("Perches").Split("/")).ToList();

                // Check random map perches until an available one is found or we reach 25 trials
                for (int trial = 0; trial < Math.Min(allMapPerches.Count, 25); trial++)
                {
                    string mapPerch = allMapPerches[trial];

                    var values = mapPerch.Split(" ");
                    var x = int.Parse(values[0]);
                    var y = int.Parse(values[1]);
                    var zOffset = int.Parse(values[2]);
                    // var perchType = int.Parse(values[3]);

                    var tileLocation = new Vector2(x, y);

                    if (birdie != null && !birdie.CheckRelocationDistance(tileLocation)) continue; // Too close/straight

                    var perch = new Perch(tileLocation, zOffset);
                    if (occupiedPerches.Any(occupiedPerch => occupiedPerch.Equals(perch))) continue; // Occupied feeder (more performant than calling Perch.GetOccupant() each time)

                    return perch;
                }
            } catch (Exception e) {
                ModEntry.Instance.Monitor.Log($"Invalid map property Perches: {e.ToString()}", StardewModdingAPI.LogLevel.Error);
            }

            return null;
        }

        public static Perch GetRandomAvailableTreePerch(GameLocation location, BetterBirdie birdie, List<Perch> occupiedPerches)
        {
            // Get all trees
            var allTrees = Utilities.Randomize(location.terrainFeatures.Values.Where(tf => tf is Tree)).ToList();

            // Check random trees until an available one is found or we reach 25 trials
            for (int trial = 0; trial < Math.Min(allTrees.Count, 25); trial++)
            {
                Tree tree = (Tree)allTrees[trial];

                if (birdie != null && !birdie.CheckRelocationDistance(tree.currentTileLocation)) continue; // Too close/straight

                var tileHeight = tree.getRenderBounds(tree.currentTileLocation).Height / Game1.tileSize;
                if (tileHeight < 4) continue; // Small tree
                if (tree.health.Value < Tree.startingHealth) continue; // Damaged tree
                if (tree.tapped.Value) continue; // Tapped tree

                var perch = new Perch(tree);
                if (occupiedPerches.Any(occupiedPerch => occupiedPerch.Equals(perch))) continue; // Occupied tree (more performant than calling Perch.GetOccupant() each time)

                return perch;
            }

            return null;
        }

        public static Perch GetRandomAvailableBirdBathPerch(GameLocation location, BirdieDef birdieDef, BetterBirdie birdie, List<Perch> occupiedPerches)
        {
            // Get all bird baths
            var allBirdBaths = Utilities.Randomize(location.Objects
                .SelectMany(overlaidDict => overlaidDict.Values)
                .Where(obj =>
                    typeof(CustomBigCraftable).IsAssignableFrom(obj.GetType()) &&
                    GetCustomBigCraftablePerchType((CustomBigCraftable)obj) == PerchType.Bath
                )).ToList();

            // Check random bird baths until an available one is found or we reach 25 trials
            for (int trial = 0; trial < Math.Min(allBirdBaths.Count, 25); trial++)
            {
                CustomBigCraftable bath = (CustomBigCraftable)allBirdBaths[trial];

                if (birdie != null && !birdie.CheckRelocationDistance(bath.TileLocation)) continue; // Too close/straight

                var bathDef = BathDef.FromBath(bath);
                if (Game1.currentSeason.Equals("winter") && !bathDef.Heated) continue; // Not heated

                if (Utility.isThereAFarmerOrCharacterWithinDistance(bath.TileLocation, birdieDef.GetContextualCautiousness(), location) != null) continue; // Character nearby

                var perch = new Perch(bath);
                if (occupiedPerches.Any(occupiedPerch => occupiedPerch.Equals(perch))) continue; // Occupied bird bath (more performant than calling Perch.GetOccupant() each time)

                return perch;
            }

            return null;
        }

        public static Perch GetRandomAvailableFeederPerch(GameLocation location, BirdieDef birdieDef, BetterBirdie birdie, List<Perch> occupiedPerches)
        {
            // Get all bird feeders
            var allFeeders = Utilities.Randomize(location.Objects
                .SelectMany(overlaidDict => overlaidDict.Values)
                .Where(obj =>
                    typeof(CustomBigCraftable).IsAssignableFrom(obj.GetType()) &&
                    GetCustomBigCraftablePerchType((CustomBigCraftable)obj) == PerchType.Feeder
                )).ToList();

            // Check random feeders until an available one is found or we reach 25 trials
            for (int trial = 0; trial < Math.Min(allFeeders.Count, 25); trial++)
            {
                CustomBigCraftable feeder = (CustomBigCraftable)allFeeders[trial];

                if (feeder.MinutesUntilReady <= 0) continue; // Empty feeder

                if (birdie != null && !birdie.CheckRelocationDistance(feeder.TileLocation)) continue; // Too close/straight

                var feederDef = FeederDef.FromFeeder(feeder);
                var foodDef = FoodDef.FromFeeder(feeder);

                if (!(birdieDef.CanPerchAt(feederDef) && birdieDef.CanEat(foodDef))) continue; // Incompatible feeder

                if (Utility.isThereAFarmerOrCharacterWithinDistance(feeder.TileLocation, birdieDef.GetContextualCautiousness(), location) != null) continue; // Character nearby

                var perch = new Perch(feeder);
                if (occupiedPerches.Any(occupiedPerch => occupiedPerch.Equals(perch))) continue; // Occupied feeder (more performant than calling Perch.GetOccupant() each time)

                return perch;
            }

            return null;
        }

        public static Perch GetRandomAvailablePerch(GameLocation location, BirdieDef birdieDef = null, BetterBirdie birdie = null)
        {
            // Get all perched birdies
            var occupiedPerches = location.critters.Where(c => c is BetterBirdie && ((BetterBirdie)c).IsPerched).Select(c => ((BetterBirdie)c).Perch).ToList();

            var mapPropertyPerches = location.getMapProperty("Perches");
            var hasMapPropertyPerches = !string.IsNullOrWhiteSpace(mapPropertyPerches);

            // Determine which types of perches we can use
            var perchTypes = new List<PerchType>() { PerchType.Tree };
            if (hasMapPropertyPerches) perchTypes.Add(PerchType.MapTile);
            if (birdieDef != null) perchTypes.Add(PerchType.Feeder);
            if (birdieDef != null && birdieDef.CanBathe) perchTypes.Add(PerchType.Bath);

            // Randomize the perch types
            perchTypes = Utilities.Randomize(perchTypes).ToList();

            // Debug helper
            if (ModEntry.debug_PerchType.HasValue)
            {
                perchTypes = new List<PerchType>() { ModEntry.debug_PerchType.Value };
            }

            Perch perch = null;

            foreach (var perchType in perchTypes)
            {
                if (perchType == PerchType.MapTile)
                {
                    perch = GetRandomAvailableMapPerch(location, birdie, occupiedPerches);
                } else if (perchType == PerchType.Bath)
                {
                    perch = GetRandomAvailableBirdBathPerch(location, birdieDef, birdie, occupiedPerches);
                } else if (perchType == PerchType.Feeder)
                {
                    perch = GetRandomAvailableFeederPerch(location, birdieDef, birdie, occupiedPerches);
                } else if (perchType == PerchType.Tree)
                {
                    perch = GetRandomAvailableTreePerch(location, birdie, occupiedPerches);
                }

                if (perch != null) break;
            }

            return perch;
        }
    }
}
