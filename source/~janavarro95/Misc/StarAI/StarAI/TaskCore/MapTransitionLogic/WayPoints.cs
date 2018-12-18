using Microsoft.Xna.Framework;
using StarAI.ExecutionCore;
using StarAI.PathFindingCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.TaskCore.MapTransitionLogic
{
    public class WayPoints
    {
        public static Dictionary<string, KeyValuePair<string, Vector2>> wayPoints = new Dictionary<string, KeyValuePair<string, Vector2>>();

        //Positions listed here will always put the player exactly 1 tile away from what is listed here.
        public static void initializeWaypoints()
        {
            wayPoints.Add("seeds", new KeyValuePair<string, Vector2>("SeedShop", new Vector2(5, 18))); //This waypoint will position the player at the General store 1 tile below the counter.
            wayPoints.Add("bed", new KeyValuePair<string, Vector2>("FarmHouse", new Vector2(-1, -1))); //to be initialized after load.
            wayPoints.Add("porch", new KeyValuePair<string, Vector2>("Farm", new Vector2(64, 15))); //to be initialized after load.

            ModCore.CoreMonitor.Log("Star AI WayPoints: Done initializing: " + wayPoints.Count + " waypoints.");
        }


        public static void pathToWayPoint(string wayPointName)
        {
            KeyValuePair<string, Vector2> outValue;
            bool isAvailable = wayPoints.TryGetValue(wayPointName, out outValue);
            if (isAvailable == true)
            {
                MapTransitionLogic.WarpGoal.pathToWorldTile(Game1.player.currentLocation, outValue.Key, (int)outValue.Value.X, (int)outValue.Value.Y);
            }
        }

        public static CustomTask pathToWayPointReturnTask(string wayPointName)
        {
            KeyValuePair<string, Vector2> outValue;
            bool isAvailable = wayPoints.TryGetValue(wayPointName, out outValue);
            if (isAvailable == true)
            {
             return MapTransitionLogic.WarpGoal.pathToWorldTileReturnTask(Game1.player.currentLocation, outValue.Key, (int)outValue.Value.X, (int)outValue.Value.Y);
            }
            return null;
        }


        public static void printWayPoints()
        {
            foreach(var v in wayPoints)
            {
                ModCore.CoreMonitor.Log("Waypoint Name:" + v.Key);
                ModCore.CoreMonitor.Log("Waypoint Position:" + v.Value.Key+" "+v.Value.Value);
            }
        }


        /// <summary>
        /// To be called after load and after save.
        /// </summary>
        public static void setUpBedWaypoint()
        {
            Vector2 vec = Game1.player.mostRecentBed / Game1.tileSize;
            int x = (int)Math.Floor(vec.X);
            x += 2;
            int y = (int)Math.Floor(vec.Y);
            vec = new Vector2(x, y);
            wayPoints["bed"] = new KeyValuePair<string, Vector2>("FarmHouse",vec);
        }

        public static void verifyWayPoints()
        {
            List<string> removalList = new List<string>();
            int i = 0;
            foreach(var waypoint in wayPoints)
            {
                i++;
                ModCore.CoreMonitor.Log("Validating waypoints " + i + " / " + wayPoints.Count);
                TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                t.fakePlacementAction(Game1.getLocationFromName(waypoint.Value.Key),(int)waypoint.Value.Value.X,(int)waypoint.Value.Value.Y);

                // bool canWaypointBeHere=TileNode.checkIfICanPlaceHere(t, waypoint.Value.Value * Game1.tileSize, Game1.getLocationFromName(waypoint.Value.Key), true, false);
                bool canPathHere = false;
                foreach(Warp w in Game1.getLocationFromName(waypoint.Value.Key).warps)
                {
                    TileNode s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    s.fakePlacementAction(Game1.getLocationFromName(waypoint.Value.Key), w.X, w.Y);

                    var path=Utilities.getIdealPath(t, s);
                    //If I can't find the goal at first keep trying.
                    if (path.Count == 0)
                    {
                        continue;
                    }
                    else
                    { //I found the goal so this is a valid waypoint.
                        canPathHere = true;
                        break;
                    }
                }
                //Valid waypoint don't remove.
                if (canPathHere == true)
                {
                    ModCore.CoreMonitor.Log("Waypoint: " + waypoint.Key + " has been validated as a valid waypoint position at:" + waypoint.Value.Key + " " + waypoint.Value.Value,StardewModdingAPI.LogLevel.Alert);
                    continue;
                }
                else
                {
                    //Couldn't path to this location. Guess I'll remove it.
                    ModCore.CoreMonitor.Log("Removing waypoint: " + waypoint.Key, StardewModdingAPI.LogLevel.Alert);
                    ModCore.CoreMonitor.Log("Can't find path at the location to: " + waypoint.Value.Key + " " + waypoint.Value.Value, StardewModdingAPI.LogLevel.Alert);
                    removalList.Add(waypoint.Key);
                }
            }

            foreach(var wayPointName in removalList)
            {
                wayPoints.Remove(wayPointName);
            }

            Utilities.tileExceptionList.Clear();
        }

    }
}
