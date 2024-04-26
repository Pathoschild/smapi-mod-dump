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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace LetsMoveIt
{
    internal partial class ModEntry
    {
        /// <summary>Pickup Object</summary>
        /// /// <param name="location">The current location.</param>
        private void PickupObject(GameLocation location)
        {
            if (Config.ModKey != SButton.None && !this.Helper.Input.IsDown(Config.ModKey))
            {
                if (MovingObject is not null)
                {
                    this.Helper.Input.Suppress(Config.MoveKey);
                    this.Helper.Input.Suppress(Config.OverwriteKey);
                    bool overwriteTile = this.Helper.Input.IsDown(Config.OverwriteKey);
                    PlaceObject(location, overwriteTile);
                }
                return;
            }
            Vector2 cursorTile = Game1.currentCursorTile;
            var mp = Game1.getMousePosition() + new Point(Game1.viewport.Location.X, Game1.viewport.Location.Y);
            //SMonitor.Log("pickupMain | " + location.isBuildable(cursorTile) + " | " + location.IsBuildableLocation() + " | " + location.IsTileOccupiedBy(cursorTile, CollisionMask.Buildings) + " | " + location.isTilePassable(cursorTile) + " | " + location.isTileOnMap(cursorTile), LogLevel.Info); // <<< debug >>>
            foreach (var c in location.characters)
            {
                var bb = c.GetBoundingBox();
                if(c is NPC)
                    bb = new Rectangle(bb.Location - new Point(0, 64), new Point(64, 128));
                if (bb.Contains(mp))
                {
                    this.Pickup(c, cursorTile, c.currentLocation);
                    return;
                }
            }
            if (location is Farm)
            {
                foreach (var a in (location as Farm).animals.Values)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        this.Pickup(a, cursorTile, a.currentLocation);
                        return;
                    }
                }
            }
            if (location is AnimalHouse)
            {
                foreach (var a in (location as AnimalHouse).animals.Values)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        this.Pickup(a, cursorTile, a.currentLocation);
                        return;
                    }
                }
            }
            if (location is Forest)
            {
                foreach (var a in (location as Forest).marniesLivestock)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        this.Pickup(a, cursorTile, a.currentLocation);
                        return;
                    }
                }
            }
            if (location.objects.TryGetValue(cursorTile, out var obj))
            {
                this.Pickup(obj, cursorTile, obj.Location);
                return;
            }
            foreach (var rc in location.resourceClumps)
            {
                if (rc.occupiesTile((int)cursorTile.X, (int)cursorTile.Y))
                {
                    //SMonitor.Log("pickup | " + Game1.currentLocation.resourceClumps.IndexOf(rc), LogLevel.Info); // <<< debug >>>
                    this.Pickup(rc, cursorTile, rc.Location);
                    return;
                }
            }
            if (location.isCropAtTile((int)cursorTile.X, (int)cursorTile.Y) && Config.MoveCropWithoutTile)
            {
                var cp = (location.terrainFeatures[cursorTile] as HoeDirt).crop;
                //SMonitor.Log("pickup | " + cp, LogLevel.Info); // <<< debug >>>
                this.Pickup(cp, cursorTile, cp.currentLocation);
                return;
            }
            if (location.largeTerrainFeatures is not null)
            {
                foreach (var ltf in location.largeTerrainFeatures)
                {
                    if (ltf.getBoundingBox().Contains((int)cursorTile.X * 64, (int)cursorTile.Y * 64))
                    {
                        this.Pickup(ltf, cursorTile, ltf.Location);
                        return;
                    }
                }
            }
            if (location.terrainFeatures.TryGetValue(cursorTile, out var tf))
            {
                this.Pickup(tf, cursorTile, tf.Location);
                return;

            }
            if (location.IsTileOccupiedBy(cursorTile, CollisionMask.Buildings) && Config.MoveBuilding)
            {
                //SMonitor.Log("pickupBuilding | " + location.isBuildable(cursorTile), LogLevel.Info); // <<< debug >>>
                var building = location.buildings.FirstOrDefault(b => b.intersects(new Rectangle(Utility.Vector2ToPoint(cursorTile * 64 - new Vector2(32, 32)), new Point(64, 64))));
                if (building != null)
                {
                    Vector2 mousePos = GetGridPosition();
                    Vector2 viewport = new(Game1.viewport.X, Game1.viewport.Y);
                    Vector2 buildingPos = new Vector2(building.tileX.Value, building.tileY.Value) * 64 - viewport;
                    this.Pickup(building, cursorTile, mousePos - buildingPos, location);
                    return;
                }
            }
        }

        /// <summary>Place Object</summary>
        /// <param name="location">The current location.</param>
        /// <param name="overwriteTile">To Overwrite existing Object.</param>
        public static void PlaceObject(GameLocation location, bool overwriteTile)
        {
            if (!Config.ModEnabled)
            {
                MovingObject = null;
                return;
            }
            if (MovingObject is null)
                return;
            Vector2 cursorTile = Game1.currentCursorTile;
            if (!overwriteTile)
            {
                if (!location.isTilePassable(cursorTile) || !location.isTileOnMap(cursorTile) || location.isTileHoeDirt(cursorTile) || location.isCropAtTile((int)cursorTile.X, (int)cursorTile.Y) || location.IsTileBlockedBy(cursorTile, ignorePassables: CollisionMask.All))
                {
                    //SMonitor.Log("Impassable " + Game1.tileSize, LogLevel.Info); // <<< debug >>>
                    if (MovingObject is not Crop && !location.isTileHoeDirt(cursorTile))
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                }
                if (BoundingBoxTile.Count is not 0)
                {
                    bool occupied = false;
                    BoundingBoxTile.ToList().ForEach(b => {
                        if (!location.isTilePassable(b) || !location.isTileOnMap(b) || location.isTileHoeDirt(b) || location.isCropAtTile((int)b.X, (int)b.Y) || location.IsTileBlockedBy(b, ignorePassables: CollisionMask.All))
                        {
                            occupied = true;
                        }
                    });
                    if (occupied)
                    {
                        Game1.playSound("cancel");
                        return;
                    }

                }
            }
            if (MovingObject is Character)
            {
                (MovingObject as Character).Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                MovingObject = null;
            }
            else if (MovingObject is Object)
            {
                //SMonitor.Log("Object", LogLevel.Info); // <<< debug >>>
                var obj = MovingLocation.objects[MovingTile];
                MovingLocation.objects.Remove(MovingTile);
                if (location.objects.ContainsKey(cursorTile))
                {
                    location.objects.Remove(cursorTile);
                }
                location.objects.Add(cursorTile, obj);
                MovingObject = null;
            }
            else if (MovingObject is FarmAnimal)
            {
                (MovingObject as FarmAnimal).Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                MovingObject = null;
            }
            else if (MovingObject is ResourceClump)
            {
                //SMonitor.Log("RC", LogLevel.Info); // <<< debug >>>
                int index = MovingLocation.resourceClumps.IndexOf(MovingObject as ResourceClump);
                if (index >= 0)
                {
                    if (location == MovingLocation)
                    {
                        location.resourceClumps[index].netTile.Value = cursorTile;
                        MovingObject = null;
                    }
                    else
                    {
                        MovingLocation.resourceClumps.Remove(MovingObject as ResourceClump);
                        location.resourceClumps.Add(MovingObject as ResourceClump);
                        int newIndex = location.resourceClumps.IndexOf(MovingObject as ResourceClump);
                        location.resourceClumps[newIndex].netTile.Value = Game1.lastCursorTile;
                        MovingObject = null;
                    }
                }
            }
            else if (MovingObject is TerrainFeature)
            {
                if (MovingObject is LargeTerrainFeature && MovingLocation.largeTerrainFeatures.Contains(MovingObject as LargeTerrainFeature))
                {
                    int index = MovingLocation.largeTerrainFeatures.IndexOf(MovingObject as LargeTerrainFeature);
                    //SMonitor.Log("LTF: " + index, LogLevel.Info); // <<< debug >>>
                    if (index >= 0)
                    {
                        if (location == MovingLocation)
                        {
                            location.largeTerrainFeatures[index].netTilePosition.Value = cursorTile;
                            MovingObject = null;
                        }
                        else
                        {
                            MovingLocation.largeTerrainFeatures.Remove(MovingObject as LargeTerrainFeature);
                            location.largeTerrainFeatures.Add(MovingObject as LargeTerrainFeature);
                            int newIndex = location.largeTerrainFeatures.IndexOf(MovingObject as LargeTerrainFeature);
                            location.largeTerrainFeatures[newIndex].netTilePosition.Value = cursorTile;
                            MovingObject = null;
                        }
                    }
                }
                else if (MovingLocation.terrainFeatures.ContainsKey(MovingTile))
                {
                    var tf = MovingLocation.terrainFeatures[MovingTile];
                    //SMonitor.Log("TF: " + tf, LogLevel.Info); // <<< debug >>>
                    MovingLocation.terrainFeatures.Remove(MovingTile);
                    if(location.terrainFeatures.ContainsKey(cursorTile))
                    {
                        location.terrainFeatures.Remove(cursorTile);
                    }
                    location.terrainFeatures.Add(cursorTile, tf);
                    HashSet<Vector2> neighbors = new() { cursorTile + new Vector2(0, 1), cursorTile + new Vector2(1, 0), cursorTile + new Vector2(0, -1), cursorTile + new Vector2(-1, 0) };
                    //SMonitor.Log("cursor:" + cursorTile, LogLevel.Info); // <<< debug >>>
                    foreach (Vector2 ct in neighbors)
                    {
                        //SMonitor.Log("neighbors:" + ct, LogLevel.Info); // <<< debug >>>
                        if (location.terrainFeatures.ContainsKey(ct))
                        {
                            if (location.terrainFeatures[ct] is HoeDirt)
                            {
                                (location.terrainFeatures[ct] as HoeDirt).updateNeighbors();
                            }
                        }
                    }
                    if (location.terrainFeatures[cursorTile] is HoeDirt)
                    {
                        (location.terrainFeatures[cursorTile] as HoeDirt).updateNeighbors();
                        (location.terrainFeatures[cursorTile] as HoeDirt).crop?.updateDrawMath(cursorTile);
                    }
                    MovingObject = null;
                }
            }
            else if (MovingObject is Crop crop)
            {
                if (location.isCropAtTile((int)cursorTile.X, (int)cursorTile.Y) || !location.isTileHoeDirt(cursorTile))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (location.isTileHoeDirt(cursorTile))
                {
                    (MovingLocation.terrainFeatures[MovingTile] as HoeDirt).crop = null;
                    (location.terrainFeatures[cursorTile] as HoeDirt).crop = crop;
                    (location.terrainFeatures[cursorTile] as HoeDirt).crop.updateDrawMath(cursorTile);
                    MovingObject = null;
                }
            }
            else if (MovingObject is Building building)
            {
                if (location.IsBuildableLocation())
                {
                    //SMonitor.Log("Building | " + new Vector2((int)Math.Round(cursorTile.X - MovingOffset.X / 64), (int)Math.Round(cursorTile.Y - MovingOffset.Y / 64)) + " | " + cursorTile, LogLevel.Info); // <<< debug >>>
                    if (location.buildStructure(building, new Vector2(cursorTile.X - MovingOffset.X / 64, cursorTile.Y - MovingOffset.Y / 64), Game1.player, overwriteTile))
                    {
                        if (MovingObject is ShippingBin)
                        {
                            (MovingObject as ShippingBin).initLid();
                        }
                        if (MovingObject is GreenhouseBuilding)
                        {
                            Game1.getFarm().greenhouseMoved.Value = true;
                        }
                        (MovingObject as Building).performActionOnBuildingPlacement();
                        MovingObject = null;
                    }
                    else
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                }
                else
                {
                    Game1.playSound("cancel");
                    return;
                }
            }
            if (MovingObject is null)
            {
                PlaySound();
            }
        }

        private static void PlaySound()
        {
            if(!string.IsNullOrEmpty(Config.Sound))
                Game1.playSound(Config.Sound);
        }

        private void Pickup(object obj, Vector2 cursorTile, GameLocation lastLocation)
        {
            this.Pickup(obj, cursorTile, GetGridPosition(), lastLocation);
        }

        private void Pickup(object obj, Vector2 cursorTile, Vector2 offset, GameLocation lastLocation)
        {
            MovingObject = obj;
            MovingTile = cursorTile;
            MovingLocation = lastLocation;
            MovingOffset = offset;
            //SMonitor.Log($"Picked up {name}"); // <<< debug >>>
            this.Helper.Input.Suppress(Config.MoveKey);
            PlaySound();
        }
    }
}
