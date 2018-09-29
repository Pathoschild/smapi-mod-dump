using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using StarAI.PathFindingCore;
using StarAI.ExecutionCore;

namespace StarAI.TaskCore.MapTransitionLogic
{
    class TransitionLogic
    {

        /// <summary>
        /// Will transition to the next map by using warp goals if the map is adjacent to the one I am at and I can path to it.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="targetName"></param>
        public static void transitionToAdjacentMap(GameLocation location,string targetName)
        {
            List<TileNode> warpGoals = new List<TileNode>();
            foreach(var v in location.warps)
            {
                if (v.TargetName == targetName)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, v.X, v.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));
                    warpGoals.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[4];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in warpGoals)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToAdjacentWarpGoal, objList, new ExecutionCore.TaskMetaData("GoTo adj map", null, null, null, null, null));

            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            objList[3] = targetName;
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();

            warpGoals.Clear();
        }

        public static CustomTask transitionToAdjacentMapReturnTask(GameLocation location, string targetName)
        {
            List<TileNode> warpGoals = new List<TileNode>();
            foreach (var v in location.warps)
            {
                if (v.TargetName == targetName)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, v.X, v.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));
                    warpGoals.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[4];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in warpGoals)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToAdjacentWarpGoal, objList, new ExecutionCore.TaskMetaData("GoTo adj map", null, null, null, null, null));

            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return null;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            objList[3] = targetName;
    
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();

            warpGoals.Clear();
            return task;
        }


        public static List<TileNode> transitionToAdjacentMapReturn(GameLocation location, string targetName)
        {
            List<TileNode> warpGoals = new List<TileNode>();
            foreach (var v in location.warps)
            {
                if (v.TargetName == targetName)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, v.X, v.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));
                    warpGoals.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[4];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in warpGoals)
            {
                tempList.Add(v);
            }
            return tempList;
            objList[0] = tempList;
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToAdjacentWarpGoal, objList, new ExecutionCore.TaskMetaData("Path to adj map", null, null, null, null, null));

            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return null;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            objList[3] = targetName;
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();

            warpGoals.Clear();
        }


        public static void goToAdjacentWarpGoal(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            goToAdjacentWarpGoal(obj);
        }

        public static void goToAdjacentWarpGoal(object obj)
        {
            object[] objArray = (object[])obj;

            TileNode v = (TileNode)objArray[2];
            string locationName = (string)objArray[3];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            PathFindingLogic.calculateMovement(correctPath);
            Vector2 tileLocation = v.tileLocation;

            for(int i = -1; i <= 1; i++)
            {

                for (int j = -1; j <= 1; j++)
                {
                    foreach (var warp in v.thisLocation.warps)
                    {
                        if (warp.X == Game1.player.getTileX()+i && warp.Y == Game1.player.getTileY()+j)
                        {
                            Game1.warpFarmer(warp.TargetName, warp.TargetX, warp.TargetY, false);
                        }
                    }
                }
            }
                
                /*
                if (Game1.player.facingDirection == 2)
                {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_S) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_S);
                }
                if (Game1.player.facingDirection == 1)
                {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_D) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_D);
                }
                if (Game1.player.facingDirection == 0)
                {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_W) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W);

                }
                if (Game1.player.facingDirection == 3)
                {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_A) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_A);
                }
                
            
            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_A);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_D);
            */
            //ModCore.CoreMonitor.Log(tileLocation.ToString());
            //if(v.thisLocation.isTerrainFeatureAt)

            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
            }

        }



        public static void transitionToAdjacentMap(GameLocation location, string targetName,int tileX, int tileY)
        {
            List<TileNode> warpGoals = new List<TileNode>();
            foreach (var v in location.warps)
            {
                if (v.TargetName == targetName)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, v.X, v.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));
                    warpGoals.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[10];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in warpGoals)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToAdjacentWarpGoalTile, objList, new ExecutionCore.TaskMetaData("GoTo adj map tile", null, null, null, null, null));

           

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            objList[3] = targetName;
            objList[4] = new Vector2(tileX, tileY);
            task.objectParameterDataArray = objList;

            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();

            warpGoals.Clear();
        }

        public static CustomTask transitionToAdjacentMapReturnTask(GameLocation location, string targetName, int tileX, int tileY)
        {
            List<TileNode> warpGoals = new List<TileNode>();
            foreach (var v in location.warps)
            {
                if (v.TargetName == targetName)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, v.X, v.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));
                    warpGoals.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[10];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in warpGoals)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToAdjacentWarpGoalTile, objList, new ExecutionCore.TaskMetaData("GoTo adj map tile", null, null, null, null, null));



            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return null;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            objList[3] = targetName;
            objList[4] = new Vector2(tileX, tileY);
            task.objectParameterDataArray = objList;

            //ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();

            warpGoals.Clear();
            return task;
        }



        public static void goToAdjacentWarpGoalTile(TileNode v, List<TileNode> path,string mapName ,Vector2 position)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            obj[3] = mapName;
            obj[4] = position;
            goToAdjacentWarpGoalTile(obj);
        }

        public static void goToAdjacentWarpGoalTile(object obj)
        {
            object[] objArray = (object[])obj;

            List<TileNode> tileList = (List<TileNode>)objArray[0];
            string locationName = (string)objArray[3];
            Vector2 position = (Vector2)objArray[4];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            PathFindingLogic.calculateMovement(correctPath);
            Warp lastWarp = new Warp(-1, -1, "Claire", -1, -1, false);
            foreach (var v in tileList)
            {
                Vector2 tileLocation = v.tileLocation;
          
                for (int i = -1; i <= 1; i++)
                {

                    for (int j = -1; j <= 1; j++)
                    {
                        foreach (var warp in v.thisLocation.warps)
                        {
                            if (warp.X == Game1.player.getTileX() + i && warp.Y == Game1.player.getTileY() + j)
                            {
                                Game1.warpFarmer(warp.TargetName, warp.TargetX, warp.TargetY, false);
                                lastWarp = warp;
                            }
                        }
                    }
                }

                Utilities.cleanExceptionList(v);
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                foreach (var goodTile in correctPath)
                {
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                    goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
                }
            }

            ModCore.CoreMonitor.Log("Going here I guess???" + locationName + " : " + position);
            ModCore.CoreMonitor.Log("From Here???" + Game1.getLocationFromName(lastWarp.TargetName) + " " + lastWarp.TargetX + "  " + lastWarp.TargetY);
            TileNode s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
            s.fakePlacementAction(Game1.getLocationFromName(lastWarp.TargetName), lastWarp.TargetX, lastWarp.TargetY);
            Utilities.tileExceptionList.Add(new TileExceptionMetaData(s, "WarpGoal"));

            TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
            t.fakePlacementAction(Game1.getLocationFromName(locationName), (int)position.X, (int)position.Y);
            Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WarpGoal"));

            PathFindingLogic.calculateMovement(Utilities.getIdealPath(t, s));
        }


    }
}
