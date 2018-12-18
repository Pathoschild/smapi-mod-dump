using Microsoft.Xna.Framework;
using StarAI.PathFindingCore;
using StardewModdingAPI;
using StardewValley;
using StardustCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarAI;
using StarAI.PathFindingCore.WaterLogic;
using StarAI.TaskCore.MapTransitionLogic;
using StarAI.TaskCore;
using StarAI.TaskCore.CropLogic;
using StarAI.TaskCore.DebrisLogic;

namespace StarAI
{
    class Commands
    {

        public static void initializeCommands()
        {
            ModCore.CoreHelper.ConsoleCommands.Add("hello", "Ok?", new Action<string, string[]>(hello));
            ModCore.CoreHelper.ConsoleCommands.Add("pathfind", "pathy?", new Action<string, string[]>(Commands.pathfind));
            ModCore.CoreHelper.ConsoleCommands.Add("pathfinding", "pathy?", new Action<string, string[]>(Commands.pathfind));
            ModCore.CoreHelper.ConsoleCommands.Add("Execute", "Run tasks", new Action<string, string[]>(Commands.runTasks));
            //ModCore.CoreHelper.ConsoleCommands.Add("execute", "Run tasks", new Action<string, string[]>(Commands.runTasks));
            ModCore.CoreHelper.ConsoleCommands.Add("runTasks", "Run tasks", new Action<string, string[]>(Commands.runTasks));
            ModCore.CoreHelper.ConsoleCommands.Add("Water", "Water the crops", new Action<string, string[]>(Commands.waterCrops));
            ModCore.CoreHelper.ConsoleCommands.Add("Harvest", "Harvest the crops", new Action<string, string[]>(Commands.harvestCrops));
            ModCore.CoreHelper.ConsoleCommands.Add("getseeds", "Get Seeds From chests.", new Action<string, string[]>(Commands.getSeedsFromChests));

            ModCore.CoreHelper.ConsoleCommands.Add("choptwigs", "Chop twigs.", new Action<string, string[]>(Commands.chopAllTwigs));
            ModCore.CoreHelper.ConsoleCommands.Add("chopsticks", "Chop twigs.", new Action<string, string[]>(Commands.chopAllTwigs));

            ModCore.CoreHelper.ConsoleCommands.Add("choptrees", "Chop trees down.", new Action<string, string[]>(Commands.chopAllTrees));
            ModCore.CoreHelper.ConsoleCommands.Add("cuttrees", "Chop trees down.", new Action<string, string[]>(Commands.chopAllTrees));

            ModCore.CoreHelper.ConsoleCommands.Add("breakstones", "Break small stones with pickaxe.", new Action<string, string[]>(Commands.breakAllStones));

            ModCore.CoreHelper.ConsoleCommands.Add("cutweed", "Cut weeds with a tool.", new Action<string, string[]>(Commands.cutAllWeeds));
            ModCore.CoreHelper.ConsoleCommands.Add("cutweeds", "Cut weeds with a tool", new Action<string, string[]>(Commands.cutAllWeeds));

            ModCore.CoreHelper.ConsoleCommands.Add("watercan", "Fill my watering can.", new Action<string, string[]>(Commands.fillWateringCan));
            ModCore.CoreHelper.ConsoleCommands.Add("fillcan", "Fill my watering can.", new Action<string, string[]>(Commands.fillWateringCan));

            ModCore.CoreHelper.ConsoleCommands.Add("digdirt", "Dig out dirt on the farm.", new Action<string, string[]>(Commands.hoeDirtAmount));
            ModCore.CoreHelper.ConsoleCommands.Add("tillsoil", "Dig out dirt on the farm.", new Action<string, string[]>(Commands.hoeDirtAmount));
            ModCore.CoreHelper.ConsoleCommands.Add("hoedirt", "Dig out dirt on the farm.", new Action<string, string[]>(Commands.hoeDirtAmount));

            ModCore.CoreHelper.ConsoleCommands.Add("plant", "Plant Seeds", new Action<string, string[]>(Commands.plantSeeds));
            ModCore.CoreHelper.ConsoleCommands.Add("plantseeds", "Plant Seeds", new Action<string, string[]>(Commands.plantSeeds));

            ModCore.CoreHelper.ConsoleCommands.Add("shippingbin", "Goto shipping bin", new Action<string, string[]>(Commands.goToShippingBin));
            ModCore.CoreHelper.ConsoleCommands.Add("shipItem", "Ship an Item", new Action<string, string[]>(Commands.shipItem));

            ModCore.CoreHelper.ConsoleCommands.Add("pathto", "Path to the adjacent map", new Action<string, string[]>(Commands.pathToMap));

            ModCore.CoreHelper.ConsoleCommands.Add("goto", "Path to a given waypoint", new Action<string, string[]>(Commands.wayPoints));

            ModCore.CoreHelper.ConsoleCommands.Add("waypoints", "Utilities to deal with waypoints", new Action<string, string[]>(Commands.wayPoints));

            ModCore.CoreHelper.ConsoleCommands.Add("placement", "Toggleplacement", new Action<string, string[]>(Commands.togglePlacement));

            // ModCore.CoreHelper.ConsoleCommands.Add("chopsticks", "Chop twigs.", new Action<string, string[]>(Commands.chopAllTwigs));
            pathfind("Initialize Delay 0", new string[] {
                "setDelay",
                "0"
                });
        }

        public static void hoeDirtAmount(string s, string[] args)
        {
            if (args.Length != 3)
            {
                ModCore.CoreMonitor.Log("Error, need to specify 3 paramaters:<Game location name>, <Number of tiles to dig.>, <Radius to search around farmer>");
                return;
            }
            GameLocation loc = Game1.getLocationFromName(args[0]);
            if (loc == null)
            {
                ModCore.CoreMonitor.Log("Error location " + args[0] + "is not valid");
                return;
            }
            SeedLogic.makeAsMuchDirtAsSpecifiedAroundFarmer(loc, Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
        }

        public static void plantSeeds(string s, string[] args)
        {
            SeedLogic.plantSeeds(Game1.player.currentLocation);
        }

        public static void taskListCommands(string s, string[] args)
        {
            if (args.Length == 0)
            {
                ModCore.CoreMonitor.Log("Error: Need more parameters. Possible command paramaters are...");
                ModCore.CoreMonitor.Log("(Pop/pop): pop off the first task and remove it");
                ModCore.CoreMonitor.Log("(Clear/clear/Flush/flush):Remove all tasks from the task list");
                return;
            }
            if (args[0] == "Pop" || args[0] == "pop")
            {
                ExecutionCore.TaskList.taskList.Remove(ExecutionCore.TaskList.taskList.ElementAt(0));
                ModCore.CoreMonitor.Log("Removed top task from tasklist.");
                return;
            }
            if (args[0] == "Clear" || args[0] == "clear")
            {
                ExecutionCore.TaskList.taskList.Clear();
                ModCore.CoreMonitor.Log("Cleared out the task list");
                return;
            }
            if (args[0] == "Flush" || args[0] == "flush")
            {
                ExecutionCore.TaskList.taskList.Clear();
                ModCore.CoreMonitor.Log("Cleared out the task list");
                return;
            }
        }

        public static void wayPoints(string s, string[] args)
        {
            if (args.Length == 0)
            {
                ModCore.CoreMonitor.Log("Invalid arguments. Possible arguments are:");
                ModCore.CoreMonitor.Log("Print: Print all waypoints");
                ModCore.CoreMonitor.Log("print: Print all waypoints");
                ModCore.CoreMonitor.Log("goto <waypointName>: Go to a specified waypoint in the world.");
                return;
            }

            if (s == "goto")
            {
                if (args.Length == 0)
                {
                    ModCore.CoreMonitor.Log("Please specify a waypoint name. They can be fetched with the command line \"waypoints print\"");
                    return;
                }
                WayPoints.pathToWayPoint(args[0]);
                return;
            }

            if (args[0] == "Print" || args[0] == "print")
            {
                WayPoints.printWayPoints();
            }
            if (args[0] == "goto" || args[0] == "GoTo" || args[0] == "goTo")
            {
                if (args.Length == 1)
                {
                    ModCore.CoreMonitor.Log("Please specify a waypoint name. They can be fetched with the command line \"waypoints print\"");
                    return;
                }
                WayPoints.pathToWayPoint(args[1]);
                return;
            }
        }

        public static void pathToMap(string s, string[] args)
        {
            if (args.Length == 0)
            {
                ModCore.CoreMonitor.Log("Need 1 parameter. MapName");
                ModCore.CoreMonitor.Log("OR need 3 parameters. MapName, xTile, yTile");
                return;
            }
            else
            {
                if (args.Length == 1)
                {
                    //path to the map location.
                    WarpGoal.getWarpChain(Game1.player.currentLocation, args[0]);
                }

                if (args.Length >= 3)
                {
                    //path to the map location.
                    WarpGoal.pathToWorldTile(Game1.player.currentLocation, args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                }
            }
        }

        public static void getSeedsFromChests(string s, string[] args)
        {
            ChestLogic.getAllSeasonalSeedsFromAllChestsAtLocation(Game1.player.currentLocation);
        }

        public static void shipItem(string s, string[] args)
        {
            if (args.Length < 2)
            {
                ModCore.CoreMonitor.Log("NOT ENOUGH PARAMETERS. NEED 2 ARGS. ItemIndex,Amount");
                return;
            }
            StardewValley.Object ok = new StardewValley.Object(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));

            if (ok == null) {
                ModCore.CoreMonitor.Log("ITEM IS NULL????");
                return;
            }
            ExecutionCore.TaskPrerequisites.ItemPrerequisite pre = new ExecutionCore.TaskPrerequisites.ItemPrerequisite(ok, ok.stack);
            if (pre.doesPlayerHaveEnoughOfMe())
            {
                ShippingLogic.goToShippingBinShipItem(ok);
            }
            else
            {
                ModCore.CoreMonitor.Log("Player does not have: " + ok.name + ": amount: " + ok.stack.ToString());
            }
        }

        public static void goToShippingBin(string s, string[] args)
        {
            ShippingLogic.goToShippingBinSetUp();
        }

        public static void fillWateringCan(string s, string[] args)
        {
            WaterLogic.getAllWaterTiles(Game1.player.currentLocation);
        }

        public static void chopAllTrees(string s, string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "All" || args[0] == "all")
                {
                    ModCore.CoreMonitor.Log("CHOP ALL TREES");
                    DebrisLogic.getAllTreesToChop(Game1.player.currentLocation);
                    return;

                }
            }
            DebrisLogic.getAllTreesToChop(Game1.player.currentLocation);
        }

        /*
        public void goPickUpDebris()
        {
                public static void removeSquareDebrisFromTile(int tileX, int tileY)
        {
            for (int index = Game1.currentLocation.debris.Count - 1; index >= 0; --index)
            {
                if (Game1.currentLocation.debris[index].debrisType == Debris.DebrisType.SQUARES && (int)((double)Game1.currentLocation.debris[index].Chunks[0].position.X / (double)Game1.tileSize) == tileX && Game1.currentLocation.debris[index].chunkFinalYLevel / Game1.tileSize == tileY)
                    Game1.currentLocation.debris.RemoveAt(index);
            }
        }
        */


        public static void chopAllTwigs(string s, string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "All" || args[0] == "all")
                {
                    DebrisLogic.getAllSticksToChop(Game1.player.currentLocation);
                    return;
                }
            }
            DebrisLogic.getAllSticksToChop(Game1.player.currentLocation);
        }


        public static void goToMap(string s, string[] args)
        {
            if (args.Length < 1)
            {
                ModCore.CoreMonitor.Log("Need args length of 1. Param: Name of location to go to.");
                return;
            }
            TaskCore.MapTransitionLogic.TransitionLogic.transitionToAdjacentMap(Game1.player.currentLocation, args[0]);
        }

        public static void breakAllStones(string s, string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "All" || args[0] == "all")
                {
                    DebrisLogic.getAllStonesToBreak(Game1.player.currentLocation);
                    return;
                }
            }
            DebrisLogic.getAllStonesToBreak(Game1.player.currentLocation);
        }

        public static void cutAllWeeds(string s, string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "All" || args[0] == "all")
                {
                    DebrisLogic.getAllWeedsToCut(Game1.player.currentLocation);
                    return;
                }
            }
            DebrisLogic.getAllWeedsToCut(Game1.player.currentLocation);
        }

        public static void runTasks(string s, string[] args)
        {
            ExecutionCore.TaskList.runTaskList();

        }

        public static void waterCrops(string s, string[] args)
        {
            CropLogic.getAllCropsNeededToBeWatered();
        }

        public static void harvestCrops(string s, string[] args)
        {
            CropLogic.getAllCropsNeededToBeHarvested();
        }

        public static void togglePlacement(string s, string[] args)
        {
            if (PathFindingCore.Utilities.placement == true)
            {
                PathFindingLogic.delay = 0;
                PathFindingCore.Utilities.placement = false;
                ModCore.CoreMonitor.Log("Placement disabled");
                return;
            }
            if (PathFindingCore.Utilities.placement == false)
            {
                PathFindingLogic.delay = 100;
                PathFindingCore.Utilities.placement = true;
                ModCore.CoreMonitor.Log("Placement enabled");
                return;
            }

        }

        /// <summary>
        /// 1.Set start position
        /// 2.set goal
        /// 3.queue up the task
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        public static void pathfind(string s, string[] args)
        {
            if (PathFindingCore.PathFindingLogic.source != null)
            {
                ModCore.CoreMonitor.Log("THIS IS OUR SOURCE POINT: as " + s + " : " + PathFindingCore.PathFindingLogic.source.tileLocation.ToString());
            }
            if (args.Length < 1)
            {
                ModCore.CoreMonitor.Log("No args passed into path finding function", LogLevel.Error);
            }

            //Set delay code
            #region
            if (args[0]=="setDelay"|| args[0]=="delay" || args[0]=="setdelay"|| args[0] == "SetDelay")
            {
                PathFindingLogic.delay = Convert.ToInt32(args[1]);
                ModCore.CoreMonitor.Log("Pathfinding node delay set to: " + Convert.ToString(PathFindingLogic.delay) + " milliseconds.");
            }
            #endregion

            //PathTo Code
            #region
            if (args[0]=="pathTo"|| args[0]=="pathto"|| args[0]=="PathTo"|| args[0] == "Pathto")
            {
                if (PathFindingLogic.currentGoal == null)
                {
                    pathfind(s, new string[]{

                    "setStart",
                    "currentPosition"
                });
                }
                else
                {
                    pathfind(s, new string[]{

                    "setStart",
                    PathFindingLogic.currentGoal.tileLocation.X.ToString(),
                    PathFindingLogic.currentGoal.tileLocation.Y.ToString(),
                });
                }
              

                int currentX;
                int currentY;
                if (PathFindingLogic.currentGoal == null)
                {
                     currentX = Game1.player.getTileX();
                     currentY = Game1.player.getTileY();
                }
                else
                {
                     currentX = (int)PathFindingLogic.currentGoal.tileLocation.X;
                     currentY = (int)PathFindingLogic.currentGoal.tileLocation.Y;
                }

                int xOffset = Convert.ToInt32(args[1]);
                int yOffset = Convert.ToInt32(args[2]);
                int destX = currentX + xOffset;
                int destY = currentY + yOffset;
                pathfind(s, new string[]
                {
                    "addGoal",
                    destX.ToString(),
                    destY.ToString()

                });

                pathfind("pathfind pathto", new string[]
                {
                        "queue"
                });
                //PathFindingLogic.currentGoal = null;
                //PathFindingLogic.source = null;
            }
            #endregion

            //Add goal code
            #region
            if (args[0] == "addGoal" || args[0] == "setGoal")
            {

                TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Green));
                Vector2 pos = new Vector2((int)(Convert.ToInt32(args[1]) * Game1.tileSize), Convert.ToInt32(args[2]) * Game1.tileSize);
                bool ok =TileNode.checkIfICanPlaceHere(t, new Vector2(pos.X, pos.Y), Game1.player.currentLocation);
                if (ok == false)
                {
                    ModCore.CoreMonitor.Log("Can't place a goal point here!!!", LogLevel.Error);
                    return;
                }
                t.placementAction(Game1.currentLocation, (int)pos.X, (int)pos.Y);
                ModCore.CoreMonitor.Log("Placing start at: " + pos.ToString(), LogLevel.Warn);

                if (PathFindingLogic.currentGoal != null)
                {
                    PathFindingLogic.source = PathFindingLogic.currentGoal;
                }
                PathFindingLogic.currentGoal = t;
                PathFindingLogic.goals.Add(t);
            }
            #endregion
            
            //Add start
            #region
            if (args[0] == "addStart" || args[0] == "setStart")
            {

                TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Magenta));
                Vector2 pos;
                if (args[1] == "currentPosition")
                {
                    pos = new Vector2((int)(Game1.player.getTileX() * Game1.tileSize), Game1.player.getTileY() * Game1.tileSize);
                }
                else
                {
                    pos = new Vector2((int)(Convert.ToInt32(args[1]) * Game1.tileSize), Convert.ToInt32(args[2]) * Game1.tileSize);
                }
              
                bool ok = TileNode.checkIfICanPlaceHere(t, new Vector2(pos.X, pos.Y), Game1.player.currentLocation);
                bool cry = false;
                if (t.thisLocation == null)
                {
                    cry = true;
                    t.thisLocation=Game1.player.currentLocation;
                }

                if (t.thisLocation.isObjectAt((int)pos.X, (int)pos.Y))
                {
                    StardewValley.Object maybe = t.thisLocation.getObjectAt((int)pos.X, (int)pos.Y);
                    if (maybe is TileNode)
                    {
                        
                            PathFindingLogic.source = (TileNode)maybe;
                            ModCore.CoreMonitor.Log("Changed the source point!!!!:"+PathFindingLogic.source.tileLocation, LogLevel.Warn);
                           // ok = true;
                        
                    }
                }
                if (ok == false)
                {
                    ModCore.CoreMonitor.Log("Can't place a start point here!!!", LogLevel.Error);
                    return;
                }
                t.placementAction(Game1.currentLocation, (int)pos.X, (int)pos.Y);
                ModCore.CoreMonitor.Log("Placing start at: "+pos.ToString(), LogLevel.Warn);
                PathFindingLogic.source = t;
            }
            #endregion
            
            //Restart Code
            #region
            if (args[0] == "restart")
            {
                List<CoreObject> removalList = new List<CoreObject>();
                foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
                {
                    removalList.Add(v);
                }
                foreach (var v in removalList)
                {
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                    Game1.player.currentLocation.objects.Remove(v.TileLocation);
                    pathfind("pathfind restart", new string[]
                    {
                        "addGoal",
                        PathFindingLogic.currentGoal.tileLocation.X.ToString(),
                        PathFindingLogic.currentGoal.tileLocation.Y.ToString(),
                    }
                    );
                }
                removalList.Clear();
                pathfind("pathfind restart", new string[]
                {
                        "addStart",
                        PathFindingLogic.source.tileLocation.X.ToString(),
                        PathFindingLogic.source.tileLocation.Y.ToString(),
                }
                );
                pathfind("pathfind restart", new string[]
               {
                        "start"
               }
               );

            }
            #endregion
            
            //start code
            #region
            if (args[0] == "start")
            {
                if (Game1.player == null) return;
                if (Game1.hasLoadedGame == false) return;
                // ModCore.CoreMonitor.Log(Game1.player.currentLocation.isTileLocationOpen(new xTile.Dimensions.Location((int)(Game1.player.getTileX() + 1)*Game1.tileSize, (int)(Game1.player.getTileY())*Game1.tileSize)).ToString());
                //CoreMonitor.Log(Convert.ToString(warpGoals.Count));
                if (PathFindingCore.PathFindingLogic.currentGoal == null)
                {
                    ModCore.CoreMonitor.Log("NO VALID GOAL SET FOR PATH FINDING!", LogLevel.Error);
                }
                if (PathFindingCore.PathFindingLogic.source == null)
                {
                    ModCore.CoreMonitor.Log("NO VALID START SET FOR PATH FINDING!", LogLevel.Error);
                }

                PathFindingLogic.pathFindToAllGoals();

            }
            #endregion

            //Queue Code
            #region
            if (args[0]=="queue" || args[0] == "Queue")
            {
                if (Game1.player == null) return;
                if (Game1.hasLoadedGame == false) return;
                // ModCore.CoreMonitor.Log(Game1.player.currentLocation.isTileLocationOpen(new xTile.Dimensions.Location((int)(Game1.player.getTileX() + 1)*Game1.tileSize, (int)(Game1.player.getTileY())*Game1.tileSize)).ToString());
                //CoreMonitor.Log(Convert.ToString(warpGoals.Count));
                if (PathFindingCore.PathFindingLogic.currentGoal == null)
                {
                    ModCore.CoreMonitor.Log("NO VALID GOAL SET FOR PATH FINDING!", LogLevel.Error);
                    return;
                }
                if (PathFindingCore.PathFindingLogic.source == null)
                {
                    ModCore.CoreMonitor.Log("NO VALID START SET FOR PATH FINDING!", LogLevel.Error);
                    return;
                }
                object[] obj = new object[3];
                obj[0] = PathFindingLogic.source;
                obj[1] = PathFindingLogic.currentGoal;
                PathFindingLogic.queue = new List<TileNode>();
                obj[2] = PathFindingLogic.queue;
               // ExecutionCore.TaskList.taskList.Add(new ExecutionCore.CustomTask(PathFindingLogic.pathFindToSingleGoal, obj,new ExecutionCore.TaskMetaData("Pathfind Command",PathFindingCore.Utilities.calculatePathCost(PathFindingLogic.source,false))));
                //ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(PathFindingLogic.pathFindToSingleGoal),obj));
            }
            #endregion


            //AddTask one liner
            #region
            if (args[0] == "addTask" || args[0] == "addtask" || args[0] == "AddTask" || args[0] == "Addtask")
            {
                pathfind("add a task",new string[]{
                    "addStart",
                    args[1],
                    args[2],
                });
                pathfind("add a task", new string[] {
                    "addGoal",
                    args[3],
                    args[4]

                });
                pathfind("add a task", new string[]
                {
                    "queue"
                });
            }
            #endregion
        }

        /// <summary>
        /// A test function.
        /// </summary>
        /// <param name="s">This is the command's name</param>
        /// <param name="sarray">This is the parameters that follow.</param>
        public static void hello(string s, string[] sarray)
        {
            ModCore.CoreMonitor.Log(s, LogLevel.Info);

            foreach (var word in sarray)
            {
                ModCore.CoreMonitor.Log(word, LogLevel.Info);
            }
            ModCore.CoreMonitor.Log("FUDGE");
            // Game1.player.tryToMoveInDirection(2, true, 0, false);

        }
    }

}

