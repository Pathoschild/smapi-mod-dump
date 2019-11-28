using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using xTile.Tiles;
using xTile.ObjectModel;

namespace MapUtilities.Pseudo3D
{
    class GameLocation_isCollidingPosition_Patch
    {
        //public static void Postfix(bool __result, GameLocation __instance, Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        //{
        //    //Fix for handling NPCs as well
        //    if (!MapHandler.isPseudo3DLocation(__instance) || MapHandler.currentLayerID.Equals("Base"))
        //        return;
        //    //Logger.log("Running patched collision detection...");
        //    if (!glider && (!Game1.eventUp || character != null && !isFarmer && (!pathfinding || !character.willDestroyObjectsUnderfoot)))
        //    {
        //        bool solid = isTileSolid(position, viewport, __instance, character, position.Width > 64, glider, isFarmer, false);
        //        __result = solid;
        //        Logger.log("Result: " + __result.ToString() + " (" + solid.ToString() + ")");
        //    }
        //}

        public static bool Prefix(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, GameLocation __instance, ref bool __result)
        {
            string currentLevel = LevelHandler.getLevelForCharacter(character);
            //Fix for handling NPCs as well
            if (!MapHandler.isPseudo3DLocation(__instance) || currentLevel.Equals("Base") || !MapHandler.hasLevel(__instance, currentLevel))
                return true;
            //Logger.log("Running patched collision detection...");
            bool solid = isTileSolid(position, viewport, __instance, character, position.Width > 64, glider, isFarmer, false);
            //Logger.log("Result: " + solid.ToString());
            __result = solid;
            return false;
        }

        public static bool isTileSolid(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, GameLocation location, Character character, bool flag, bool glider, bool isFarmer, bool projectile)
        {
            string back = MapHandler.getEquivalentLayerIfPresent(location.map, character, "Back");
            string buildings = MapHandler.getEquivalentLayerIfPresent(location.map, character, "Buildings");

            if (back != null && (location.isFarm || location.name.Value.StartsWith("UndergroundMine")) && (character != null && !character.Name.Contains("NPC")) && (!character.eventActor && !glider))
            {
                //Logger.log("Checking Back layer...");
                PropertyValue propertyValue = null;
                Tile tile1 = location.map.GetLayer(back).PickTile(new Location(position.Right, position.Top), viewport.Size);
                if (tile1 != null)
                    tile1.Properties.TryGetValue("NPCBarrier", out propertyValue);
                if (propertyValue != null)
                    return true;
                Tile tile2 = location.map.GetLayer(back).PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                if (tile2 != null)
                    tile2.Properties.TryGetValue("NPCBarrier", out propertyValue);
                if (propertyValue != null)
                    return true;
                Tile tile3 = location.map.GetLayer(back).PickTile(new Location(position.Left, position.Top), viewport.Size);
                if (tile3 != null)
                    tile3.Properties.TryGetValue("NPCBarrier", out propertyValue);
                if (propertyValue != null)
                    return true;
                Tile tile4 = location.map.GetLayer(back).PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                if (tile4 != null)
                    tile4.Properties.TryGetValue("NPCBarrier", out propertyValue);
                if (propertyValue != null)
                    return true;
                if (glider && !projectile)
                    return false;
            }
            if (!isFarmer || !Game1.player.isRafting)
            {
                //Logger.log("Checking Back layer...");
                PropertyValue propertyValue = (PropertyValue)null;
                if (back != null)
                {
                    Tile tile1 = location.map.GetLayer(back).PickTile(new Location(position.Right, position.Top), viewport.Size);
                    if (tile1 != null)
                        tile1.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                    if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Right, position.Top)))
                        return true;
                    Tile tile2 = location.map.GetLayer(back).PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                    if (tile2 != null)
                        tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                    if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Right, position.Bottom)))
                        return true;
                    Tile tile3 = location.map.GetLayer(back).PickTile(new Location(position.Left, position.Top), viewport.Size);
                    if (tile3 != null)
                        tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                    if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Left, position.Top)))
                        return true;
                    Tile tile4 = location.map.GetLayer(back).PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                    if (tile4 != null)
                        tile4.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                    if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Left, position.Bottom)))
                        return true;
                    if (flag)
                    {
                        Tile tile5 = location.map.GetLayer(back).PickTile(new Location(position.Center.X, position.Bottom), viewport.Size);
                        if (tile5 != null)
                            tile5.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Center.X, position.Bottom)))
                            return true;
                        Tile tile6 = location.map.GetLayer(back).PickTile(new Location(position.Center.X, position.Top), viewport.Size);
                        if (tile6 != null)
                            tile6.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Center.X, position.Top)))
                            return true;
                    }
                }
                if (buildings != null)
                {
                    //Logger.log("Checking Buildings layer...");
                    Tile tile7 = location.map.GetLayer(buildings).PickTile(new Location(position.Right, position.Top), viewport.Size);
                    if (tile7 != null)
                    {
                        tile7.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                        if (propertyValue == null)
                            tile7.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue == null && !isFarmer)
                            tile7.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                        if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                            tile7.Properties.TryGetValue("Action", out propertyValue);
                        if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Right, position.Top)))
                        {
                            //Logger.log("Tile " + tile7.Id.ToString() + " was solid.  (" + tile7.Layer.Id + ")");
                            if (character == null)
                                return true;
                            return character.shouldCollideWithBuildingLayer(location);
                        }
                    }
                    Tile tile8 = location.map.GetLayer(buildings).PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                    if (tile8 != null && propertyValue == null | isFarmer)
                    {
                        tile8.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                        if (propertyValue == null)
                            tile8.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue == null && !isFarmer)
                            tile8.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                        if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                            tile8.Properties.TryGetValue("Action", out propertyValue);
                        if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Right, position.Bottom)))
                        {
                            //Logger.log("Tile " + tile8.Id.ToString() + " was solid.  (" + tile8.Layer.Id + ")");
                            if (character == null)
                                return true;
                            return character.shouldCollideWithBuildingLayer(location);
                        }
                    }
                    Tile tile9 = location.map.GetLayer(buildings).PickTile(new Location(position.Left, position.Top), viewport.Size);
                    if (tile9 != null && propertyValue == null | isFarmer)
                    {
                        tile9.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                        if (propertyValue == null)
                            tile9.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue == null && !isFarmer)
                            tile9.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                        if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                            tile9.Properties.TryGetValue("Action", out propertyValue);
                        if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Left, position.Top)))
                        {
                            //Logger.log("Tile " + tile9.Id.ToString() + " was solid.  (" + tile9.Layer.Id + ")");
                            if (character == null)
                                return true;
                            return character.shouldCollideWithBuildingLayer(location);
                        }
                    }
                    Tile tile10 = location.map.GetLayer(buildings).PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                    if (tile10 != null && propertyValue == null | isFarmer)
                    {
                        tile10.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                        if (propertyValue == null)
                            tile10.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                        if (propertyValue == null && !isFarmer)
                            tile10.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                        if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                            tile10.Properties.TryGetValue("Action", out propertyValue);
                        if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Left, position.Bottom)))
                        {
                            //Logger.log("Tile " + tile10.Id.ToString() + " was solid.  (" + tile10.Layer.Id + ")");
                            if (character == null)
                                return true;
                            return character.shouldCollideWithBuildingLayer(location);
                        }
                    }
                    if (flag)
                    {
                        Tile tile5 = location.map.GetLayer(buildings).PickTile(new Location(position.Center.X, position.Top), viewport.Size);
                        if (tile5 != null && propertyValue == null | isFarmer)
                        {
                            tile5.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                            if (propertyValue == null)
                                tile5.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                            if (propertyValue == null && !isFarmer)
                                tile5.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                            if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                                tile5.Properties.TryGetValue("Action", out propertyValue);
                            if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Center.X, position.Top)))
                            {
                                //Logger.log("Tile " + tile5.Id.ToString() + " was solid.  (" + tile5.Layer.Id + ")");
                                if (character == null)
                                    return true;
                                return character.shouldCollideWithBuildingLayer(location);
                            }
                        }
                        Tile tile6 = location.map.GetLayer(buildings).PickTile(new Location(position.Center.X, position.Bottom), viewport.Size);
                        if (tile6 != null && propertyValue == null | isFarmer)
                        {
                            tile6.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
                            if (propertyValue == null)
                                tile6.TileIndexProperties.TryGetValue("Passable", out propertyValue);
                            if (propertyValue == null && !isFarmer)
                                tile6.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue);
                            if (propertyValue == null && !isFarmer && (character != null && character.canPassThroughActionTiles()))
                                tile6.Properties.TryGetValue("Action", out propertyValue);
                            if ((propertyValue == null || propertyValue.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(position.Center.X, position.Bottom)))
                            {
                                //Logger.log("Tile " + tile6.Id.ToString() + " was solid.  (" + tile6.Layer.Id + ")");
                                if (character == null)
                                    return true;
                                return character.shouldCollideWithBuildingLayer(location);
                            }
                        }
                    }
                }
            }
            //Logger.log("Found no solid tiles.");
            return false;
        }
    }
}
