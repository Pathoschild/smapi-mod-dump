using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using XRectangle = xTile.Dimensions.Rectangle;

namespace Igorious.StardewValley.DynamicAPI.Locations
{
    public sealed class SmartLocationProxy
    {
        private static int TileSize => Game1.tileSize;

        private GameLocation Location { get; }

        public SmartLocationProxy(GameLocation location)
        {
            Location = location;
        }

        public bool PerformToolAction(Func<Tool, int, int, bool> baseMethod, Tool tool, int xTile, int yTile)
        {
            var point = new Point(xTile * TileSize + TileSize / 2, yTile * TileSize + TileSize / 2);
            foreach (var o in Location.Objects.Values.Where(o => o is ISmartObject))
            {
                if (!o.getBoundingBox(o.TileLocation).Contains(point) || !o.performToolAction(tool)) continue;

                var player = tool.getLastFarmerToUse();
                var location = player.currentLocation;
                o.performRemoveAction(o.TileLocation, location);
                var playerCenter = player.GetBoundingBox().Center;
                location.debris.Add(new Debris(o.ParentSheetIndex * (o.bigCraftable ? -1 : +1), player.GetToolLocation(), new Vector2(playerCenter.X, playerCenter.Y)));
                location.Objects.Remove(o.TileLocation);
                return true;
            }
            return baseMethod(tool, xTile, yTile);
        }

        public bool IsActionableTile(Func<int, int, Farmer, bool> baseMethod, int xTile, int yTile, Farmer farmer)
        {
            var rectangle = new Rectangle(xTile * TileSize, yTile * TileSize, TileSize, TileSize);
            foreach (var o in Location.Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(rectangle))
                {
                    return o.isActionable(farmer);
                }
            }
            return baseMethod(xTile, yTile, farmer);
        }

        public bool CheckAction(Func<Location, XRectangle, Farmer, bool> baseMethod, Location tileLocation, XRectangle viewport, Farmer farmer)
        {
            var rectangle = new Rectangle(tileLocation.X * TileSize, tileLocation.Y * TileSize, TileSize, TileSize);
            foreach (var o in Location.Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(rectangle))
                {
                    if (o.checkForAction(farmer)) return true;
                }
            }
            return baseMethod(tileLocation, viewport, farmer);
        }

        public bool IsCollidingPosition(Func<Rectangle, XRectangle, bool, int, bool, Character, bool, bool, bool, bool> baseMethod, Rectangle position, XRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (IsCollidingWithSmartObject(position)) return true;
            return baseMethod(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
        }

        public bool IsCollidingPosition(Func<Rectangle, XRectangle, bool, int, bool, Character, bool> baseMethod, Rectangle position, XRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character)
        {
            if (character == null || character.willDestroyObjectsUnderfoot)
            {
                if (IsCollidingWithSmartObject(position)) return true;
            }
            return baseMethod(position, viewport, isFarmer, damagesFarmer, glider, character);
        }

        public bool IsTileOccupied(Func<Vector2, string, bool> baseMethod, Vector2 tileLocation, string characterToIgnore = "")
        {
            var rectangle = new Rectangle((int)tileLocation.X * TileSize + 1, (int)tileLocation.Y * TileSize + 1, TileSize - 2, TileSize - 2);
            if (IsCollidingWithSmartObject(rectangle)) return true;
            return baseMethod(tileLocation, characterToIgnore);
        }

        public bool IsTileLocationTotallyClearAndPlaceable(Func<Vector2, bool> baseMethod, Vector2 v)
        {
            var tiledVector = v * TileSize;
            var point = new Point((int)tiledVector.X + TileSize / 2, (int)tiledVector.Y + TileSize / 2);

            foreach (var o in Location.Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Contains(point)) return false;
            }
            return baseMethod(v);
        }

        private bool IsCollidingWithSmartObject(Rectangle position)
        {
            foreach (var o in Location.Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(position)) return true;
            }
            return false;
        }
    }
}