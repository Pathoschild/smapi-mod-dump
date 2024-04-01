/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using MarketTown;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;
using lv = StardewModdingAPI.LogLevel;

namespace MarketTown
{
    internal class FarmOutside
    {
        internal static bool NPCinScreen()
        {
            var x = 0;
            var y = 0;
            var farm = Game1.getLocationFromName("Farm");
            foreach (NPC who in Utility.getAllCharacters())
            {
                if (who.IsVillager && who.currentLocation.Name == "Farm" 
                    && who.modData.ContainsKey("hapyke.FoodStore/invited") && who.modData["hapyke.FoodStore/invited"] == "true")
                {
                    x = (int)(who.Position.X / 64);
                    y = (int)(who.Position.Y / 64);
                }
            }


            //return Utility.isOnScreen(who.Position.ToPoint(), 0, farm);
            return Utility.isOnScreen(new Point(x, y), 0, farm);
        }

        internal static void PlayerWarp(object sender, WarpedEventArgs e)
        {

            var isBusStop = e.NewLocation.Name.Contains("BusStop");

            if (isBusStop)
            {
                foreach (NPC who in Game1.getLocationFromName("BusStop").characters)
                {
                    if (who.Name.Contains("MT.Guest_") && who.Tile != who.DefaultPosition / 64)
                    {
                        who.Halt();
                        who.temporaryController = null;
                        who.controller = null;
                        who.ClearSchedule();
                        Game1.warpCharacter(who, who.DefaultMap, who.DefaultPosition / 64);
                    }
                }
            }


            if (!e.Player.IsMainPlayer)
            {
                return;
            }

            var isFarm = e.NewLocation.Name.StartsWith("Farm");
            var isFarmHouse = e.NewLocation.Name.StartsWith("FarmHouse");

            if (!isFarm && !isFarmHouse) //if its neither the farm nor the farmhouse
                return;

            if (isFarmHouse && !ModEntry.Config.EnableVisitInside)      //If not enable visit inside
                return;

            string name = null;
            Point door = new();

            if (isFarm)
            {
                ModEntry.IsOutside = true;

                door = Game1.getFarm().GetMainFarmHouseEntry();
                door.X += 3;
                door.Y += 2; //two more tiles down
                name = e.NewLocation.Name;
            }

            if (isFarmHouse)
            {
                ModEntry.IsOutside = false;

                var home = Utility.getHomeOfFarmer(Game1.player);
                door = home.getEntryLocation();
                door.X += 3;
                door.Y -= 2;
                name = home.Name;
            }

            foreach (NPC visit in Utility.getAllCharacters())
            {
                try
                {
                    if (visit.IsVillager && (visit.currentLocation.Name == "Farm" || visit.currentLocation.Name == "FarmHouse") 
                        && visit.modData.ContainsKey("hapyke.FoodStore/invited") && visit.modData["hapyke.FoodStore/invited"] == "true" 
                        && Game1.timeOfDay > ModEntry.Config.InviteComeTime)
                    {
                        if (visit.controller is not null)
                            visit.Halt();

                        Game1.warpCharacter(visit, name, door);

                        visit.faceDirection(2);

                        door.X--;
                        visit.controller = new PathFindController(visit, Game1.getFarm(), door, 2);
                    }
                }
                catch { }
            }
        }

        internal static void WalkAround(string who)
        {
            var c = Game1.getCharacterFromName(who);
            if (c == null) return;

            var newspot = getRandomOpenPointInFarm(c.currentLocation, Game1.random);

            try
            {
                c.controller = null;
                c.isCharging = true;

                c.controller = new PathFindController(
                    c,
                    c.currentLocation,
                    newspot,
                    Game1.random.Next(0, 4)
                    );
            }
            catch { }
        }


        internal static Point getRandomOpenPointInFarm(GameLocation location, Random r, int tries = 5, int maxDistance = 15)
        {
            foreach (NPC who in Utility.getAllCharacters())
            {
                if (who.IsVillager && (((who.currentLocation.Name == "Farm" || who.currentLocation.Name == "FarmHouse") && who.modData["hapyke.FoodStore/invited"] == "true") || (who.Name.Contains("MT.Guest_") && !who.currentLocation.Name.Contains("BusStop")) ))
                {

                    var map = location.map;

                    Point zero = Point.Zero;
                    bool CanGetHere = false;

                    for (int i = 0; i < tries; i++)
                    {
                        //we get random position using width and height of map
                        zero = new Point(r.Next(1, map.Layers[0].LayerWidth - 1), r.Next(1, map.Layers[0].LayerHeight - 1));

                        bool isFloorValid = location.isTileOnMap(zero.ToVector2()) && location.isTilePassable(new xTile.Dimensions.Location(zero.X, zero.Y), Game1.viewport) && !location.isWaterTile(zero.X, zero.Y);
                        bool IsBehindTree = location.isBehindTree(zero.ToVector2());
                        Warp WarpOrDoor = location.isCollidingWithWarpOrDoor(new Rectangle(zero, new Point(1, 1)));

                        //check that location is clear + not water tile + not behind tree + not a warp
                        CanGetHere = !location.IsTileBlockedBy(new Vector2(zero.X, zero.Y)) && location.CanItemBePlacedHere(new Vector2(zero.X, zero.Y)) 
                            && isFloorValid && !IsBehindTree && WarpOrDoor == null;

                        //if the new point is too far away
                        Point difference = new(Math.Abs(zero.X - (int)who.Position.X), Math.Abs(zero.Y - (int)who.Position.Y));
                        if (difference.X > maxDistance && difference.Y > maxDistance)
                        {
                            CanGetHere = false;
                        }

                        if (CanGetHere)
                        {
                            break;
                        }
                    }
                    return zero;
                }
            }
            return Point.Zero;
        }
    }
}
