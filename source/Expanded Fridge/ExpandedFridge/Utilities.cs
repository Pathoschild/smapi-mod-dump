using System;
using System.Collections.Generic;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley.Locations;

using Microsoft.Xna.Framework;


namespace ExpandedFridge
{
    /// <summary>
    /// Collections of static methods and variables.
    /// </summary>
    class Utilities
    {
        /// Sheet index for mini fridges.
        public const int MiniFridgeSheetIndex = 216;

        /// Start tile for placement out of bounds.
        public const int OutOfBoundsTileY = -300;

        /// Wrapper for getting players current location.
        public static GameLocation CurrentLocation { get { return Game1.player.currentLocation; } }
        
        /// Checks if a location has a fridge.
        public static bool IsFridgeInLocation(GameLocation location)
        {
            return ((location is FarmHouse) && (location as FarmHouse).upgradeLevel > 0);
        }

        /// Is a given tile within the map bounds of the given location.
        public static bool IsPointInsideMapBounds(Point point, GameLocation location)
        {
            return location.isTileOnMap(point.X, point.Y);
        }

        /// Wrapper for checking point inside map bounds.
        public static bool IsPointInsideMapBounds(Vector2 point, GameLocation location)
        {
            return location.isTileOnMap(point);
        }

        /// Is given object a mini fridge.
        public static bool IsObjectMiniFridge(StardewValley.Object obj)
        {
            return (obj != null && obj.bigCraftable.Value && (obj is Chest && obj.ParentSheetIndex == MiniFridgeSheetIndex));
        }

        /// Get an array of all locations that have fridges.
        /// Note:   If not on Master Game it could miss locations with fridges.
        ///         Must use request locations or other way to ensure all locations on remote players.
        public static FarmHouse[] GetAllFridgeHouses()
        {
            List<FarmHouse> fridgeLocations = new List<FarmHouse>();
            
            foreach (var location in Game1.locations)
            {
                if (IsFridgeInLocation(location))
                    fridgeLocations.Add(location as FarmHouse);
                else if (location is Farm)
                {
                    foreach (var building in (location as Farm).buildings)
                    {
                        if (building.isCabin && building.daysOfConstructionLeft.Value <= 0 && (building.indoors.Value as FarmHouse).upgradeLevel > 0)
                        {
                            fridgeLocations.Add(building.indoors.Value as FarmHouse);
                        }
                    }
                }
            }

            return fridgeLocations.ToArray();
        }
        
        /// Get an array of mini fridge chests that exists in given location. They are sorted by their tile coordinates with Y as higher priority.
        public static Chest[] GetAllMiniFridgesInLocation(GameLocation location)
        {
            List<Chest> miniFridges = new List<Chest>();
            SortedDictionary<Vector2, Chest> miniFridgeDictionary = new SortedDictionary<Vector2, Chest>(
                Comparer<Vector2>.Create((v1, v2) => v1 == v2 ? 0 : v1.Y > v2.Y ? 1 : (v1.Y == v2.Y && v1.X > v2.X) ? 1 : -1));

            // find all chests in location of mini fridge index
            foreach (var p in location.objects.Pairs)
                if (IsObjectMiniFridge(p.Value))
                    miniFridgeDictionary.Add(p.Key, p.Value as Chest);

            foreach (var c in miniFridgeDictionary.Values)
                miniFridges.Add(c);

            return miniFridges.ToArray();
        }
        
        /// Get a free tile for chest placement in a location.
        /// NOTE: This can return a value outside the map bounds.
        public static Point GetFreeTileInLocation(GameLocation location)
        {
            for (int h = 0; h <= location.map.Layers[0].LayerHeight; h++)//(int h = location.map.Layers[0].LayerHeight; h >= 0; h--)
                for (int w = 0; w <= location.map.Layers[0].LayerWidth; w++)
                    // check if tile in width and height is placeable and not on wall
                    if (location.isTileLocationTotallyClearAndPlaceable(w, h) && (!(location is DecoratableLocation) || !(location as DecoratableLocation).isTileOnWall(w, h)))
                        return new Point(w, h);

            int y = 0;
            int x = 0;

            // move in y direction untill no other potential offmap objects are there
            while (location.isObjectAtTile(x, y))
                y++;

            ModEntry.DebugLog("Warning, object might become placed out of bounds at tile x:" + x + ", y:" + y + " in location: " + location.Name, StardewModdingAPI.LogLevel.Warn);

            // return that position
            return new Point(x, y);
        }

        /// As GetFreeTileInLocation but returns Vector2 instead.
        public static Vector2 GetFreeTileVectorInLocation(GameLocation location)
        {
            var p = GetFreeTileInLocation(location);
            return new Vector2(p.X, p.Y);
        }

        /// Creates a new inventory menu from a chest with option for showing the color picker.
        public static ItemGrabMenu GetNewItemGrabMenuFromChest(Chest chest, bool showColorPicker)
        {
            return new ItemGrabMenu((IList<Item>)chest.items, false, true, new
                    InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                    new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string)null,
                    new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromChest), false, true, true, true, true, 1,
                    !showColorPicker ? (Item)null : (Item)chest, !showColorPicker ? -1 : 1, (object)chest);
        }

        /// Moves all mini fridges in all farmhouses out of the map bounds.
        public static void MoveMiniFridgesOutOfMapBounds()
        {
            foreach (var h in GetAllFridgeHouses())
            {
                var fridgeChests = GetAllMiniFridgesInLocation(h);
                List<Vector2> miniFridgePositions = new List<Vector2>();

                foreach (var c in fridgeChests)
                {
                    foreach (var p in h.objects.Pairs)
                    {
                        if (c == p.Value)
                        {
                            if (IsPointInsideMapBounds(p.Key, h))
                            {
                                miniFridgePositions.Add(p.Key);
                                break;
                            }
                        }
                    }
                }

                foreach (var v in miniFridgePositions)
                {
                    int x = 0;
                    Vector2 oldPosition = new Vector2(v.X, v.Y);
                    Vector2 newPosition = new Vector2(x, OutOfBoundsTileY);

                    while (h.objects.ContainsKey(newPosition))
                        newPosition.X = ++x;
                    
                    StardewValley.Object obj = h.objects[oldPosition];
                    obj.tileLocation.Value = newPosition;
                    h.objects.Remove(oldPosition);
                    h.objects.Add(newPosition, obj);
                }
            }
        }

        /// Moves all mini fridges in all farmhouses into map bounds.
        public static void MoveMiniFridgesIntoMapBounds()
        {
            foreach (var h in GetAllFridgeHouses())
            {
                var fridgeChests = GetAllMiniFridgesInLocation(h);
                List<Vector2> miniFridgePositions = new List<Vector2>();

                foreach (var c in fridgeChests)
                {
                    foreach (var p in h.objects.Pairs)
                    {
                        if (c == p.Value)
                        {
                            if (!IsPointInsideMapBounds(p.Key, h))
                            {
                                miniFridgePositions.Add(p.Key);
                                break;
                            }
                        }
                    }
                }

                foreach (var v in miniFridgePositions)
                {
                    Vector2 oldPosition = new Vector2(v.X, v.Y);
                    Vector2 newPosition = GetFreeTileVectorInLocation(h);
                    
                    StardewValley.Object obj = h.objects[oldPosition];
                    obj.tileLocation.Value = newPosition;
                    h.objects.Remove(oldPosition);
                    h.objects.Add(newPosition, obj);
                }
            }
        }
    }
}
