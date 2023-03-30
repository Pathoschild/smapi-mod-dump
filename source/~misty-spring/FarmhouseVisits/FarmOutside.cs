/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;
using lv = StardewModdingAPI.LogLevel;

namespace FarmVisitors
{
    internal class FarmOutside
    {
        internal static bool NPCinScreen()
        {
            var who = Game1.getCharacterFromName(ModEntry.VisitorName);
            var farm = Game1.getLocationFromName("Farm");

            var x = ((int)(who.Position.X / 64));
            var y = ((int)(who.Position.Y / 64));

            if (ModEntry.Debug)
            {
                ModEntry.Log($"farm name = {farm.Name}, visitor position = ({x}, {y})", lv.Info);
            }

            //return Utility.isOnScreen(who.Position.ToPoint(), 0, farm);
            return Utility.isOnScreen(new Point(x,y), 0, farm);
        }

        /* NOTE:
         * There is a NPC barrier surrounding the door. Characters warped to that tile won't be able to move.
         * For this reason, the NPC must be warped 2 tiles below the door.
         * 
         * This could be fixed by editing map properties- but it'd only be compatible with vanilla maps (and might have side effects). This is the best workaround currently.
         */
        internal static void PlayerWarp(object sender, WarpedEventArgs e)
        {
            if(ModEntry.ForcedSchedule)
            {
                return;
            }

            var isFarm = e.NewLocation.IsFarm;
            var isFarmHouse = e.NewLocation.Name.StartsWith("FarmHouse");

            if (!isFarm && !isFarmHouse) //if its neither the farm nor the farmhouse
                return;

            if (!ModEntry.CanFollow || string.IsNullOrWhiteSpace(ModEntry.VisitorName))
                return; //if npcs can't follow or there's no visit

            if (ModEntry.Debug)
            {
                ModEntry.Log($"Leaving {e.OldLocation.Name}...Warped to {e.NewLocation.Name}. isFarm = {e.NewLocation.IsFarm} , CanFollow = {ModEntry.CanFollow}, VisitorName = {ModEntry.VisitorName}", lv.Info);
            }

            string name = null;
            Point door = new();

            if (isFarm)
            {
                ModEntry.IsOutside = true;

                door = Game1.getFarm().GetMainFarmHouseEntry();
                //door.X--; //-1, moves npc one tile to the left
                door.Y += 2; //two more tiles down

                name = e.NewLocation.Name;
            }

            if (isFarmHouse)
            {
                ModEntry.IsOutside = false;

                var home = Utility.getHomeOfFarmer(Game1.player);
                door = home.getEntryLocation();
                name = home.Name;
            }

            var visit = Game1.getCharacterFromName(ModEntry.VisitorName);

            if (visit.controller is not null)
                visit.Halt();

            Game1.warpCharacter(visit, name, door);

            visit.faceDirection(2);

            door.X--;
            visit.controller = new PathFindController(visit, Game1.getFarm(), door, 2); //(this was made as test, but will stay commented-out just in case.) */
        }

        internal static void WalkAroundFarm(string who)
        {
            var c = Game1.getCharacterFromName(who);

            var gameLocation = Game1.getFarm();
            //var newspot = getRandomOpenPointInFarm(gameLocation, Game1.random);
            var newspot = getRandomFreeTile(gameLocation);

            try
            {
                c.PathToOnFarm(newspot);
                
                if(ModEntry.Debug)
                {
                    ModEntry.Log($"is the controller empty?: {c.controller == null}", lv.Debug);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Log($"Something went wrong (PathToOnFarm): {ex}", lv.Error);
            }

            if (Game1.random.Next(0, 11) <= 5)
            {
                var anyCrops = ModEntry.Crops.Any();

                if (Game1.currentSeason == "winter")
                {
                    c.setNewDialogue(
                        Values.GetDialogueType(
                            c,
                            DialogueType.Winter),
                        true,
                        false);
                }
                else if ((Game1.random.Next(0, 2) <= 0 || !anyCrops) && ModEntry.Animals.Any())
                {
                    c.setNewDialogue(
                        string.Format(
                            Values.GetDialogueType(
                                c,
                                DialogueType.Animal),
                            Values.GetRandomObj(
                                ItemType.Animal)),
                        true,
                        false);
                }
                else if (anyCrops)
                {
                    c.setNewDialogue(
                        string.Format(
                            Values.GetDialogueType(
                                c, 
                                DialogueType.Crop), 
                            Values.GetRandomObj(
                                ItemType.Crop)), 
                        true, 
                        false);
                }
                else
                {
                    c.setNewDialogue(
                        Values.GetDialogueType(
                            c,
                            DialogueType.NoneYet),
                        true,
                        false);
                }
            }
        }

        internal static Point getRandomOpenPointInFarm(GameLocation location,Random r, int buffer = 0, int tries = 30)
        {
            var map = location.map;

            Point zero = Point.Zero;
            for (int i = 0; i < tries; i++)
            {
                zero = new Point(r.Next(map.Layers[0].LayerWidth), r.Next(map.Layers[0].LayerHeight));

                Rectangle rectangle = new Rectangle(zero.X - buffer, zero.Y - buffer, 1 + buffer * 2, 1 + buffer * 2);

                bool flag = false;

                for (int j = rectangle.X; j < rectangle.Right; j++)
                {
                    for (int k = rectangle.Y; k < rectangle.Bottom; k++)
                    {
                        flag = (location.getTileIndexAt(j, k, "Back") == -1 || !location.isTileLocationTotallyClearAndPlaceable(j, k) || location.isWaterTile(j, k));
                        /*
                        if (location.isTilePlaceable(new Vector2(j,k)) && location.getTileSheetIDAt(j, k, "Back") == "untitled tile sheet")
                        {
                            flag = true;
                        }*/

                        if (flag)
                        {
                            break;
                        }
                    }

                    if (flag)
                    {
                        break;
                    }
                }

                if (!flag)
                {

                    if (ModEntry.Debug)
                    {
                        ModEntry.Log($"New position for {ModEntry.VisitorName}: {zero.X},{zero.Y}", lv.Debug);
                    }

                    return zero;
                }
            }
            
            return Point.Zero;
        }

        internal static Point getRandomFreeTile(GameLocation location, int distance = 10, int tries = 30)
        {
            var map = location.map;
            Point zero = Point.Zero;
            NPC who = Game1.getCharacterFromName(ModEntry.VisitorName);

            var width = distance;
            var height = distance;

            /*if (map.DisplayWidth > 109) //if the map is bigger than 109 tiles(width), set new max. (to 10% of the map's width / height, respectively)
            {
                width = map.DisplayHeight / 10;
                height = map.DisplayWidth / 10;
            }*/

            for (int i = 0; i < tries; i++)
            {
                zero = location.getRandomTile().ToPoint();
                var vector = location.getRandomTile();

                bool iswater = location.isWaterTile(zero.X, zero.Y);
                bool behindtree = location.isBehindTree(vector);
                Warp warpOrDoor = location.isCollidingWithWarpOrDoor(new Rectangle(zero,new Point(1,1)));
                bool nearScreen = false;

                var x = Math.Abs((who.Position.X / 64) - vector.X);
                var y = Math.Abs((who.Position.Y / 64) - vector.Y);

                if (x <= width && y <= height) //if the difference isn't greater than 10
                {
                    nearScreen = true;
                }

                if (!iswater && !behindtree && warpOrDoor == null && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(vector) && nearScreen)
                {
                    if (ModEntry.Debug)
                    {
                        ModEntry.Log($"New position for {ModEntry.VisitorName}: {zero.X},{zero.Y}", lv.Debug);
                    }

                    return zero;
                }
            }

            return Point.Zero;
        }
    }
}
