/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OrnithologistsGuild.Game.Critters;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;
using OrnithologistsGuild.Models;

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
        public Object Bath;
        public Object Feeder;
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

            var height = tree.getRenderBounds().Height;
            Position = new Vector3(
                tree.Tile.X * Game1.tileSize,
                tree.Tile.Y * Game1.tileSize,
                -System.MathF.Ceiling((float)(height) / 1.65f));

            // Center on tile
            Position.X += Game1.tileSize / 2;
            Position.Y += Game1.tileSize / 2;
        }
        public Perch(Object bigCraftable)
        {
            if (bigCraftable.IsBath())
            {
                Bath = bigCraftable;

                var bathFields = bigCraftable.GetBathFields();
                Position = new Vector3(
                    Bath.TileLocation.X * Game1.tileSize,
                    Bath.TileLocation.Y * Game1.tileSize,
                    bathFields.ZOffset);

                // Center on tile
                Position.X += Game1.tileSize / 2;
                Position.Y += Game1.tileSize / 2;
            } else if (bigCraftable.IsFeeder())
            {
                Feeder = bigCraftable;

                var feederFields = bigCraftable.GetFeederFields();
                Position = new Vector3(
                    Feeder.TileLocation.X * Game1.tileSize,
                    Feeder.TileLocation.Y * Game1.tileSize,
                    feederFields.ZOffset);

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
                return perch.Tree.Tile.Equals(Tree.Tile);
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

        public BetterBirdie GetOccupant(GameLocation location)
        {
            if (location.critters == null) return null;

            IEnumerable<BetterBirdie> birdies = location.critters.Where(c => c is BetterBirdie && ((BetterBirdie)c).IsPerched).Select(c => (BetterBirdie)c);
            return birdies.FirstOrDefault(birdie => birdie.Perch.Equals(this));
        }

        public static IEnumerable<Perch> GetAllMapPerches(GameLocation location)
        {
            var mapPropertyPerches = location.getMapProperty("Perches");
            if (string.IsNullOrWhiteSpace(mapPropertyPerches)) return Enumerable.Empty<Perch>();

            try
            {
                // Get all map perches
                return mapPropertyPerches.Split("/")
                    .Select(mapPerch =>
                    {
                        var values = mapPerch.Split(" ");
                        var x = int.Parse(values[0]);
                        var y = int.Parse(values[1]);
                        var zOffset = int.Parse(values[2]);
                        // var perchType = int.Parse(values[3]);

                        var tileLocation = new Vector2(x, y);

                        return new Perch(tileLocation, zOffset);
                    });
            } catch (System.Exception e) {
                ModEntry.Instance.Monitor.Log($"Invalid map property Perches: {e.ToString()}", StardewModdingAPI.LogLevel.Error);
            }

            return Enumerable.Empty<Perch>();
        }

        public static IEnumerable<Perch> GetAllTreePerches(GameLocation location)
        {
            return location.terrainFeatures.Values.Where(tf => tf is Tree)
                .Select(tree => (Tree)tree)
                .Where(tree =>
                {
                    var tileHeight = tree.getRenderBounds().Height / Game1.tileSize;
                    if (tileHeight < 4) return false; // Small tree
                    if (tree.health.Value < Tree.startingHealth) return false; // Damaged tree
                    if (tree.tapped.Value) return false; // Tapped tree

                    return true;
                })
                .Select(tree => new Perch(tree));
        }

        public static IEnumerable<Perch> GetAllBirdBathPerches(GameLocation location)
        {
            return location.Objects
                .SelectMany(overlaidDict => overlaidDict.Values)
                .Where(obj => obj.IsBath())
                .Where(bath => !Game1.currentSeason.Equals("winter") || bath.GetBathFields().Heated)
                .Select(bath => new Perch(bath));
        }

        public static IEnumerable<Perch> GetAllFeederPerches(GameLocation location)
        {
            // Get all bird feeders
            return location.Objects
                .SelectMany(overlaidDict => overlaidDict.Values)
                .Where(obj => obj.IsFeeder())
                .Where(feeder => feeder.MinutesUntilReady > 0) // No empty feeders
                .Select(feeder => new Perch(feeder));
        }

        public static IEnumerable<Perch> GetAllAvailablePerches(GameLocation location, bool mapTile, bool tree, bool feeder, bool bath)
        {
            // Get all perched birdies
            var occupiedPerches = location.critters == null ?
                Enumerable.Empty<Perch>() :
                location.critters.Where(c => c is BetterBirdie && ((BetterBirdie)c).IsPerched).Select(c => ((BetterBirdie)c).Perch);

            return Enumerable.Empty<Perch>()
                .Concat(mapTile ? GetAllMapPerches(location)      : Enumerable.Empty<Perch>())
                .Concat(tree    ? GetAllTreePerches(location)     : Enumerable.Empty<Perch>())
                .Concat(feeder  ? GetAllFeederPerches(location)   : Enumerable.Empty<Perch>())
                .Concat(bath    ? GetAllBirdBathPerches(location) : Enumerable.Empty<Perch>())
                .Except(occupiedPerches);
        }
    }
}
