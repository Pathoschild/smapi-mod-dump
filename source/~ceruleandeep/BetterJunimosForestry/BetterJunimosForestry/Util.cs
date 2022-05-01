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
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace BetterJunimosForestry {
    internal class FakeFarmer : Farmer
    {
        public override Vector2 GetToolLocation(bool ignoreClick = false)
        {
            return new(0, 0);
        }
    }
    
    public static class Util {
        internal static readonly Dictionary<int, int> WildTreeSeeds = new()
        {
            {292, 8}, // mahogany
            {309, 1}, // acorn
            {310, 2}, // maple
            {311, 3}, // pine
            {891, 7}  // mushroom
        };
        
        /// <summary>Get whether a tile is blocked due to something it contains.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="tile">The tile to check.</param>
        /// <remarks>Derived from <see cref="StardewValley.Tools.Hoe.DoFunction"/>.</remarks>
        public static bool IsOccupied(GameLocation location, Vector2 tile)
        {
            // impassable tiles (e.g. water)
            if (!location.isTilePassable(new Location((int)tile.X, (int)tile.Y), Game1.viewport))
                return true;

            // objects & large terrain features
            if (location.objects.ContainsKey(tile) || location.largeTerrainFeatures.Any(p => p.tilePosition.Value == tile))
                return true;
            
            // logs, boulders, etc
            if (location.resourceClumps.Any(resourceClump => resourceClump.occupiesTile((int)tile.X, (int)tile.Y)))
            {
                return true;
            }
            
            // non-dirt terrain features
            if (location.terrainFeatures.TryGetValue(tile, out var feature))
            {
                var dirt = feature as HoeDirt;
                if (dirt is not {crop: null})
                    return true;
            }

            // buildings
            if (location is BuildableGameLocation buildableLocation)
            {
                if (buildableLocation.buildings.Any(building => building.occupiesTile(tile)))
                    return true;
            }

            // buildings from the map
            if (location.getTileIndexAt(Utility.Vector2ToPoint(tile), "Buildings") > -1) return true;
            
            // furniture
            if (location.GetFurnitureAt(tile) != null) return true;
            
            return false;
        }
        
        internal static bool CanBeHoed(GameLocation location, Vector2 tileLocation)
        {
            var XCoord = (int)tileLocation.X;
            var YCoord = (int)tileLocation.Y;

            if (location.terrainFeatures.ContainsKey(tileLocation))
            {
                if (location.terrainFeatures[tileLocation] is HoeDirt hoeDirt)
                {
                    if(hoeDirt.crop != null)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (location.objects.ContainsKey(tileLocation))
            {
                return false;
            }
            
            if(location.doesTileHaveProperty(XCoord, YCoord, "Diggable", "Back") == null)
            {
                return false;
            }

            if (!location.isTilePassable(new Location(XCoord, YCoord), Game1.viewport))
            {
                return false;
            }

            if (location.resourceClumps.Any(resourceClump => resourceClump.occupiesTile((int)tileLocation.X, (int)tileLocation.Y)))
            {
                return false;
            }
            
            var tileLocationRect = new Rectangle((int)tileLocation.X * 64 + 1, (int)tileLocation.Y * 64 + 1, 62, 62);
            if (location.largeTerrainFeatures.Any(largeTerrainFeature => largeTerrainFeature.getBoundingBox().Intersects(tileLocationRect)))
            {
                return false;
            }

            var f = location.GetFurnitureAt(tileLocation);
            if (f != null)
            {
                return false;
            }

            return location is not Farm farm || farm.buildings.All(building => !building.occupiesTile(tileLocation));
        }

        public static bool IsHoed(GameLocation location, Vector2 pos) {
            if (location.terrainFeatures.TryGetValue(pos, out var feature))
            {
                return feature is HoeDirt;
            }

            return false;
        }
        
        public static bool HasCrop(GameLocation location, Vector2 pos) {
            if (location.terrainFeatures.TryGetValue(pos, out var feature))
            {
                return (feature as HoeDirt)?.crop != null;
            }

            return false;
        }
        
        public static bool SpawningTreesForbidden(GameLocation location, Vector2 pos) {
            var noSpawn = location.doesTileHaveProperty((int)pos.X, (int)pos.Y, "NoSpawn", "Back");
            var cantSpawnHere = noSpawn != null && (noSpawn.Equals("Tree") || noSpawn.Equals("All") || noSpawn.Equals("True"));
            return cantSpawnHere;
        }

        public static JunimoHut HutOnTile(Vector2 pos) {
            foreach (var b in Game1.getFarm().buildings) {
                if (b is JunimoHut hut && b.occupiesTile(pos)) {
                    return hut;
                }
            }
            return null;
        }

        public static Guid GetHutIdFromHut(JunimoHut hut) {
            return Game1.getFarm().buildings.GuidOf(hut);
        }

        public static Vector2 GetPosFromHut(JunimoHut hut) {
            return new (hut.tileX.Value, hut.tileY.Value);
        }
        
        public static string GetModeForHut(JunimoHut hut) {
            if (hut is null) return Modes.Normal;
            
            if (ModEntry.HutStates.TryGetValue(GetPosFromHut(hut), out HutState state)) {
                return state.Mode;
            }
            return Modes.Normal;
        }
        
        public static JunimoHut GetHutFromId(Guid id) {
            Farm farm = Game1.getFarm();
            try {
                return farm.buildings[id] as JunimoHut;
            }
            catch (Exception ex) {
                ModEntry.SMonitor.Log($"GetHutFromId: exception while getting position of {id}", LogLevel.Error);
                ModEntry.SMonitor.Log($"{ex.ToString()}", LogLevel.Error);
                return null;
            }
        }

        public static Vector2 GetHutPositionFromId(Guid id) {
            Farm farm = Game1.getFarm();
            try {
                JunimoHut hut = farm.buildings[id] as JunimoHut;
                return new Vector2(hut.tileX.Value, hut.tileY.Value);
            }
            catch {
                ModEntry.SMonitor.Log($"GetHutPositionFromId: exception while getting position of {id}", LogLevel.Error);
                return new Vector2(0, 0);
            }
        }

        public static Vector2 GetHutPositionFromHut(JunimoHut hut) {
            return new Vector2(hut.tileX.Value, hut.tileY.Value);
        }

        public static JunimoHut GetHutFromPosition(Vector2 pos) {
            Farm farm = Game1.getFarm();
            foreach (Building b in farm.buildings) {
                if (b.occupiesTile(pos) && b is JunimoHut hut) {
                    return hut;
                }
            }

            return null;
        }
        
        public static void AddItemToChest(GameLocation farm, Chest chest, SObject item) {
            Item obj = chest.addItem(item);
            if (obj == null) return;
            Vector2 pos = chest.TileLocation;
            for (int index = 0; index < obj.Stack; ++index)
                Game1.createObjectDebris(item.ParentSheetIndex, (int)pos.X + 1, (int)pos.Y + 1, -1, item.Quality, 1f, farm);
        }

        public static void RemoveItemFromChest(Chest chest, Item item) {
            if (ModEntry.Config.InfiniteJunimoInventory) { return; }
            item.Stack--;
            if (item.Stack == 0) {
                chest.items.Remove(item);
            }
        }
        
        public static bool IsWithinRadius(GameLocation location, JunimoHut hut, Vector2 pos)
        {
            if (!location.IsFarm) return true;
            var radius = ModEntry.BJApi.GetJunimoHutMaxRadius();
            var (x, y) = pos;
            var outcome = !(x < hut.tileX.Value + 1 - radius || x >= hut.tileX.Value + 2 + radius);
            if (y < hut.tileY.Value + 1 - radius || y >= hut.tileY.Value + 2 + radius) outcome = false;
            return outcome;
        }
        
        public static bool BlocksDoor(GameLocation location, JunimoHut hut, Vector2 pos) {
            if (!location.IsFarm) return false;
            var (x, y) = pos;
            var blocks = (int)x == hut.tileX.Value + 1 && (int)y == hut.tileY.Value + 2;
            // ModEntry.SMonitor.Log($"BlocksDoor: hut [{hut.tileX.Value} {hut.tileY.Value}], pos [{pos.X} {pos.Y}], blocks: {blocks}", LogLevel.Trace);
            return blocks;
        }
    }
}
