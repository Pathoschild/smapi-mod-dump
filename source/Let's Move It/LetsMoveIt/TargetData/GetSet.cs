/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        /// <summary>Get target.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="tile">The current tile position.</param>
        /// <param name="map">The current map position.</param>
        public static void Get(GameLocation location, Vector2 tile, Point map)
        {
            if (Config.EnableMoveEntity)
            {
                foreach (var c in location.characters)
                {
                    //var bb = c.GetBoundingBox();
                    //bb = new Rectangle(bb.Location - new Point(0, 64), new Point(c.Sprite.getWidth() * 4, c.Sprite.getHeight() * 4));
                    if (c.GetBoundingBox().Contains(map))
                    {
                        Set(c, c.currentLocation, tile);
                        return;
                    }
                }
                if (location is Farm farm)
                {
                    foreach (var a in farm.animals.Values)
                    {
                        if (a.GetBoundingBox().Contains(map))
                        {
                            Set(a, a.currentLocation, tile);
                            return;
                        }
                    }
                }
                if (location is AnimalHouse animalHouse)
                {
                    foreach (var a in animalHouse.animals.Values)
                    {
                        if (a.GetBoundingBox().Contains(map))
                        {
                            Set(a, a.currentLocation, tile);
                            return;
                        }
                    }
                }
                if (location is Forest forest)
                {
                    foreach (var a in forest.marniesLivestock)
                    {
                        if (a.GetBoundingBox().Contains(map))
                        {
                            Set(a, a.currentLocation, tile);
                            return;
                        }
                    }
                }
                if (Game1.player.GetBoundingBox().Contains(map))
                {
                    Set(Game1.player, location, tile);
                    return;
                }
            }
            if (location.objects.TryGetValue(tile, out var obj))
            {
                if ((obj is IndoorPot pot) && Config.MoveCropWithoutIndoorPot)
                {
                    //pot.NetFields.GetFields().ToList().ForEach(l =>
                    //    Monitor.Log(l.Name + ": " + l, LogLevel.Debug) // <<< List NetFields >>> <<< debug >>>
                    //);
                    if (pot.bush.Value is not null && Config.EnableMoveBush)
                    {
                        var b = pot.bush.Value;
                        Set(b, b.Location, b.Tile);
                        return;
                    }
                    if (pot.hoeDirt.Value.crop is not null && Config.EnableMoveCrop)
                    {
                        var cp = pot.hoeDirt.Value.crop;
                        Set(cp, cp.currentLocation, cp.tilePosition);
                        return;
                    }
                }

                if (!Config.EnableMoveObject)
                    goto SkipObjects;
                if (obj.isPlaceable() && !Config.EnableMovePlaceableObject)
                    goto SkipObjects;
                if (obj.IsSpawnedObject && !Config.EnableMoveCollectibleObject)
                    goto SkipObjects;
                if (!obj.isPlaceable() && !obj.IsSpawnedObject && !Config.EnableMoveGeneratedObject)
                    goto SkipObjects;

                Set(obj, obj.Location, obj.TileLocation);
                return;
            }
            SkipObjects:
            foreach (var rc in location.resourceClumps)
            {
                if (rc.occupiesTile((int)tile.X, (int)tile.Y) && Config.EnableMoveResourceClump)
                {
                    int rcIndex = rc.parentSheetIndex.Value;
                    if ((rc is GiantCrop) && !Config.EnableMoveGiantCrop)
                        goto SkipResourceClumps;
                    if ((rcIndex is ResourceClump.stumpIndex) && !Config.EnableMoveStump)
                        goto SkipResourceClumps;
                    if ((rcIndex is ResourceClump.hollowLogIndex) && !Config.EnableMoveHollowLog)
                        goto SkipResourceClumps;
                    if ((rcIndex is ResourceClump.boulderIndex or ResourceClump.quarryBoulderIndex or ResourceClump.mineRock1Index or ResourceClump.mineRock2Index or ResourceClump.mineRock3Index or ResourceClump.mineRock4Index) && !Config.EnableMoveBoulder)
                        goto SkipResourceClumps;
                    if ((rcIndex is ResourceClump.meteoriteIndex) && !Config.EnableMoveMeteorite)
                        goto SkipResourceClumps;

                    Set(rc, rc.Location, rc.Tile);
                    return;
                }
            }
            SkipResourceClumps:
            if (location.isCropAtTile((int)tile.X, (int)tile.Y) && Config.MoveCropWithoutTile && Config.EnableMoveCrop)
            {
                var cp = ((HoeDirt)location.terrainFeatures[tile]).crop;
                Set(cp, cp.currentLocation, cp.tilePosition);
                return;
            }
            if (location.largeTerrainFeatures is not null && Config.EnableMoveTerrainFeature)
            {
                foreach (var ltf in location.largeTerrainFeatures)
                {
                    if (ltf.getBoundingBox().Contains((int)tile.X * 64, (int)tile.Y * 64))
                    {
                        if ((ltf is Bush) && !Config.EnableMoveBush)
                            goto SkipLargeTerrainFeatures;

                        Set(ltf, ltf.Location, ltf.Tile);
                        return;
                    }
                }
            }
            SkipLargeTerrainFeatures:
            if (location.terrainFeatures.TryGetValue(tile, out var tf) && Config.EnableMoveTerrainFeature)
            {
                if ((tf is Flooring) && !Config.EnableMoveFlooring)
                    goto SkipTerrainFeatures;
                if ((tf is Tree) && !Config.EnableMoveTree)
                    goto SkipTerrainFeatures;
                if ((tf is FruitTree) && !Config.EnableMoveFruitTree)
                    goto SkipTerrainFeatures;
                if ((tf is Grass) && !Config.EnableMoveGrass)
                    goto SkipTerrainFeatures;
                if ((tf is HoeDirt) && !Config.EnableMoveFarmland)
                    goto SkipTerrainFeatures;
                if ((tf is Bush) && !Config.EnableMoveBush) // Tea Bush
                    goto SkipTerrainFeatures;

                Set(tf, tf.Location, tf.Tile);
                return;
            }
            SkipTerrainFeatures:
            if (location.IsTileOccupiedBy(tile, CollisionMask.Buildings) && Config.EnableMoveBuilding)
            {
                var building = location.getBuildingAt(tile);
                if (building != null)
                {
                    Vector2 buildingTile = new(building.tileX.Value, building.tileY.Value);
                    Set(building.buildingType.Value, building, location, tile, tile - buildingTile);
                    return;
                }
            }
        }

        /// <summary>Set target</summary>
        private static void Set(object obj, GameLocation lastLocation, Vector2 cursorTile, Vector2? offset = null)
        {
            Set(null, obj, lastLocation, cursorTile, offset ?? Vector2.Zero);
        }
        //private static void SetTarget(string? name, object obj, GameLocation lastLocation, Vector2 cursorTile)
        //{
        //    SetTarget(name, null, obj, lastLocation, cursorTile, Vector2.Zero);
        //}
        //private static void SetTarget(Guid guid, object obj, GameLocation lastLocation, Vector2 cursorTile)
        //{
        //    SetTarget(null, guid, obj, lastLocation, cursorTile, Vector2.Zero);
        //}
        private static void Set(string? name, object obj, GameLocation lastLocation, Vector2 cursorTile, Vector2 offset)
        {
            Name = name;
            TargetObject = obj;
            TargetLocation = lastLocation;
            TilePosition = cursorTile;
            TileOffset = offset;
            Helper.Input.Suppress(Config.MoveKey);
            PlaySound();
        }
    }
}
