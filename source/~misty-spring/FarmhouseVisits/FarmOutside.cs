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

            if (ModEntry.Config.Debug)
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

            if (!ModEntry.Config.WalkOnFarm || string.IsNullOrWhiteSpace(ModEntry.VisitorName))
                return; //if npcs can't follow or there's no visit

            if (ModEntry.Config.Debug)
            {
                ModEntry.Log($"Leaving {e.OldLocation.Name}...Warped to {e.NewLocation.Name}. isFarm = {e.NewLocation.IsFarm} , CanFollow = {ModEntry.Config.WalkOnFarm}, VisitorName = {ModEntry.VisitorName}", lv.Info);
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
            var newspot = getRandomOpenPointInFarm(gameLocation, Game1.random);

            try
            {
                c.PathToOnFarm(newspot);
                
                if(ModEntry.Config.Debug)
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

        internal static Point getRandomOpenPointInFarm(GameLocation location,Random r, int tries = 30, int maxDistance = 10)
        {
            NPC who = Game1.getCharacterFromName(ModEntry.VisitorName);

            var map = location.map;

            Point zero = Point.Zero;
            bool CanGetHere = false;

            for (int i = 0; i < tries; i++)
            {
                //we get random position using width and height of map
                zero = new Point(r.Next(map.Layers[0].LayerWidth), r.Next(map.Layers[0].LayerHeight));

                bool isFloorValid = location.isTileOnMap(zero.ToVector2()) && location.isTilePassable(new xTile.Dimensions.Location(zero.X, zero.Y), Game1.viewport) && !location.isWaterTile(zero.X, zero.Y);
                bool IsBehindTree = location.isBehindTree(zero.ToVector2());
                Warp WarpOrDoor = location.isCollidingWithWarpOrDoor(new Rectangle(zero, new Point(1, 1)));

                //check that location is clear + not water tile + not behind tree + not a warp
                CanGetHere = location.isTileLocationTotallyClearAndPlaceable(zero.X, zero.Y) && isFloorValid && !IsBehindTree && WarpOrDoor == null;

                //if the new point is too far away
                Point difference = new (Math.Abs(zero.X - (int)who.Position.X),Math.Abs(zero.Y - (int)who.Position.Y));
                if(difference.X > maxDistance && difference.Y > maxDistance)
                {
                    CanGetHere = false;
                }

                if (CanGetHere)
                {
                    break;
                }
            }

            if (ModEntry.Config.Debug)
            {
                ModEntry.Log($"New position for {ModEntry.VisitorName}: {zero.X},{zero.Y}", lv.Debug);
            }
           
            return zero;
        }
    }
}
