using Microsoft.Xna.Framework;
using StarAI.PathFindingCore;
using StarAI.PathFindingCore.WaterLogic;
using StarAI.TaskCore.MapTransitionLogic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore
{
    class TaskList
    {
        public static List<CustomTask> taskList = new List<CustomTask>();
        public static Task executioner = new Task(new Action(runTaskList));

        public static List<CustomTask> removalList = new List<CustomTask>();

        public static bool pathafterLocationChange;
        public static void runTaskList()
        {
           
            //myTask t = new myTask(StarAI.PathFindingCore.CropLogic.CropLogic.harvestSingleCrop);
             
            bool assignNewTask = true;

            if(TaskPrerequisites.BedTimePrerequisite.enoughTimeToDoTaskStatic() == false)
            {
                CustomTask task = WayPoints.pathToWayPointReturnTask("bed");
                if (task == null) ModCore.CoreMonitor.Log("SOMETHING WENT WRONG WHEN TRYING TO GO TO BED", LogLevel.Error);
                ModCore.CoreMonitor.Log("Not enough time remaining in day. Going home.", LogLevel.Alert);
                task.runTask();
                Utilities.tileExceptionList.Clear();
                taskList.Clear();
                removalList.Clear();
                return;
            }

            while(ranAllTasks()==false||TaskPrerequisites.BedTimePrerequisite.enoughTimeToDoTaskStatic()==false)
            {
                Utilities.tileExceptionList.Clear();
                foreach (var task2 in taskList)
            {
                    if (removalList.Contains(task2)) continue;
                    var temp = task2;
                    recalculateTask(ref temp);               
                //task.taskMetaData = new TaskMetaData(task.taskMetaData.name, PathFindingCore.Utilities.calculatePathCost(task.objectParameterDataArray), task.taskMetaData.staminaPrerequisite, task.taskMetaData.toolPrerequisite);
            }
               // ModCore.CoreMonitor.Log("DONE CALCULATING JUNK NOW RUNNING TASK");
            //Some really cool delegate magic that sorts in place by the cost of the action!!!!
            taskList.Sort(delegate (CustomTask t1, CustomTask t2)
            {
                return t1.taskMetaData.cost.CompareTo(t2.taskMetaData.cost);
            });
                CustomTask v = taskList.ElementAt(0);
                int i = 0;
                while (removalList.Contains(v))
                {
                    v = taskList.ElementAt(i);
                    i++;
                }
                //  v.Start();
                bool recalculate= interruptionTasks(v);

                if (recalculate) {
                    recalculateTask(ref v);
                }
                if (v.taskMetaData.verifyAllPrerequisitesHit() == true)
                {
                    v.runTask();
                    Utilities.clearExceptionListWithName("Child");
                    Utilities.clearExceptionListWithName("Navigation");
                    removalList.Add(v);
                }
                else
                {
                    removalList.Add(v);
                }
            }

            Utilities.tileExceptionList.Clear();
            taskList.Clear();
            removalList.Clear();
            
        }

        public static void recalculateTask(ref CustomTask v)
        {
            object[] oArray = (object[])v.objectParameterDataArray;
          //  ModCore.CoreMonitor.Log("RECALCULATING: "+ v.taskMetaData.name);

            if (v.taskMetaData.name.Contains("Path to "))
            {
                Utilities.tileExceptionList.Clear();
               // ModCore.CoreMonitor.Log("POKE DEW VALLEY: " + v.taskMetaData.name);
                string[] s = v.taskMetaData.name.Split(' ');
                ModCore.CoreMonitor.Log(s.ElementAt(s.Length-1));
                List<List<TileNode>> newPaths = new List<List<TileNode>>(); 
                newPaths = TaskCore.MapTransitionLogic.WarpGoal.getWarpChainReturn(Game1.player.currentLocation, s.ElementAt(s.Length-1));
                v.taskMetaData.cost = 0;
                int value = 0;
                foreach (var path in newPaths)
                {
                    value+= (path.Count * TaskMetaDataHeuristics.pathCostMultiplier);
                }
                object[] arr = (object[])v.objectParameterDataArray;
                arr[3] = newPaths;
                v.taskMetaData.cost = value;
                v.taskMetaData.pathsToTake = newPaths;
                //ModCore.CoreMonitor.Log("IDK ANY MORE: " + v.taskMetaData.cost);
                return;
            }
            Utilities.tileExceptionList.Clear();
            try
            {
                Utilities.tileExceptionList.Clear();
                TileNode t = (TileNode)oArray[0];
                Utilities.tileExceptionList.Clear();
                //ModCore.CoreMonitor.Log("Premtive calculate 1");
                //ModCore.CoreMonitor.Log("Valaue before???:" + v.taskMetaData.pathsToTake[0].Count);
                v.taskMetaData.calculateTaskCost(t, false);
                //v.taskMetaData.pathsToTake = new List<List<TileNode>>();
                //v.taskMetaData.pathsToTake.Add(StarAI.PathFindingCore.Utilities.getIdealPath(v));


                object[] objArr = new object[10];
                objArr[0] = (object)t;
                objArr[1] = (object)v.taskMetaData.pathsToTake[0];
                //ModCore.CoreMonitor.Log("HMM SO WHAT'S HAPPENING???:" + v.taskMetaData.pathsToTake[0].Count);
                int malcolm = 0;
                objArr[2] = (object)v.taskMetaData.pathsToTake[0].ElementAt(malcolm); //source of whatever is hit.
                try
                {
                    objArr[3] = oArray[3];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[4] = oArray[4];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[5] = oArray[5];
                }
                catch (Exception err2)
                {

                }
                v.objectParameterDataArray = objArr;
                return;
            }
            catch (Exception err)
            {
               
            }
            
            try
            {
                Utilities.tileExceptionList.Clear();
                List<TileNode> t = (List<TileNode>)oArray[0];
               // ModCore.CoreMonitor.Log("Premtive calculate 2");
                foreach (var s in Utilities.tileExceptionList)
                {
                    ModCore.CoreMonitor.Log(s.actionType);
                }
                v.taskMetaData.calculateTaskCost(t, false);
                object[] objArr = new object[10];
                objArr[0] = (object)t; //List of trees to use for path calculations
                objArr[1] = (object)v.taskMetaData.pathsToTake[0]; //The path itself.
                int malcolm = 0;
               // ModCore.CoreMonitor.Log("THIS IS MALCOLM:" + malcolm);
                objArr[2] = (object)v.taskMetaData.pathsToTake[0].ElementAt(malcolm); //source of whatever is hit.
                try
                {
                    objArr[3] = oArray[3];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[4] = oArray[4];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[5] = oArray[5];
                }
                catch (Exception err2)
                {

                }
                v.objectParameterDataArray = objArr;
                Utilities.tileExceptionList.Clear();
                return;
            }
            catch(Exception err)
            {

            }

            try
            {
                Utilities.tileExceptionList.Clear();
                List<List<TileNode>> t = (List<List<TileNode>>)oArray[3];
               // ModCore.CoreMonitor.Log("Premtive calculate 3");
                foreach (var s in Utilities.tileExceptionList)
                {
                    ModCore.CoreMonitor.Log(s.actionType);
                }
                v.taskMetaData.calculateTaskCost(t, false);
                object[] objArr = new object[10];
                objArr[0] = (object)t; //List of trees to use for path calculations
                objArr[1] = (object)v.taskMetaData.pathsToTake; //The path itself.
                int malcolm = 0;
               // ModCore.CoreMonitor.Log("THIS IS MALCOLM:" + malcolm);
                objArr[2] = (object)v.taskMetaData.pathsToTake[0].ElementAt(malcolm); //source of whatever is hit.
                try
                {
                    objArr[3] = oArray[3];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[4] = oArray[4];
                }
                catch (Exception err2)
                {

                }
                try
                {
                    objArr[5] = oArray[5];
                }
                catch (Exception err2)
                {

                }
                v.objectParameterDataArray = objArr;
                Utilities.tileExceptionList.Clear();
                return;
            }
            catch(Exception err)
            {

            }
        }

        public static bool interruptionTasks(CustomTask v)
        {

            if (v.taskMetaData.bedTimePrerequisite.enoughTimeToDoTask() == false)
            {
                CustomTask task = WayPoints.pathToWayPointReturnTask("bed");
                if (task == null)
                {
                    ModCore.CoreMonitor.Log("SOMETHING WENT WRONG WHEN TRYING TO GO TO BED", LogLevel.Error);
                    return false;
                }
                ModCore.CoreMonitor.Log("Not enough time remaining in day. Going home and removing tasks.", LogLevel.Alert);
                task.runTask();
                return true;
            }

            if (v.taskMetaData.locationPrerequisite.isPlayerAtLocation() == false)
            {
                //Force player to move to that location, but also need the cost again....
               // ModCore.CoreMonitor.Log("PLAYERS LOCATION:"+Game1.player.currentLocation.name);
                Utilities.tileExceptionList.Clear();
               CustomTask task= WarpGoal.getWarpChainReturnTask(Game1.player.currentLocation, v.taskMetaData.locationPrerequisite.location.name);
                if (task == null)
                {
                    ModCore.CoreMonitor.Log("SOMETHING WENT WRONG WHEN TRYING TO GO TO" + v.taskMetaData.locationPrerequisite.location.name, LogLevel.Error);
                    return false;
                }

                task.runTask();
                object[] arr = (object[])v.objectParameterDataArray;
                List<TileNode> path;
                try
                {

                    List<List<TileNode>> okList = (arr[0] as List<List<TileNode>>);
                    List<TileNode> smallList = okList.ElementAt(okList.Count - 1);
                    TileNode tile = smallList.ElementAt(smallList.Count - 1);
                    //arr[0] = WarpGoal.pathToWorldTileReturnTask(Game1.player.currentLocation, v.taskMetaData.locationPrerequisite.location.name,(int) tile.tileLocation.X,(int) tile.tileLocation.Y);
                    TileNode s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    s.fakePlacementAction(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY());

                   path = Utilities.getIdealPath(tile, s);
                }
                catch(Exception err)
                {

                    Utilities.tileExceptionList.Clear();
                    List<TileNode> smallList = (arr[1] as List<TileNode>);
                    TileNode tile = smallList.ElementAt(smallList.Count-1);
                    //ModCore.CoreMonitor.Log("LOC:" + tile.thisLocation + tile.thisLocation);


                    Warp lastWarp = new Warp(-1, -1, "Grahm", -1, -1, false);
                    GameLocation fakeLocation = Game1.getLocationFromName(Game1.player.currentLocation.name);
                    foreach(var ok in fakeLocation.warps)
                    {
                        if (ok.X == Game1.player.getTileX() && ok.Y == Game1.player.getTileY() + 1) lastWarp = ok;
                    }

                    //ModCore.CoreMonitor.Log("MYLOC:" + lastWarp.TargetName + lastWarp.TargetX +" "+lastWarp.TargetY);
                    //arr[0] = WarpGoal.pathToWorldTileReturnTask(Game1.player.currentLocation, v.taskMetaData.locationPrerequisite.location.name,(int) tile.tileLocation.X,(int) tile.tileLocation.Y);
                    TileNode s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    s.fakePlacementAction(Game1.getLocationFromName(lastWarp.TargetName), lastWarp.TargetX, lastWarp.TargetY);

                     path = Utilities.getIdealPath(tile, s);
                     //arr[0] = s;
                }

               // ModCore.CoreMonitor.Log("PATHCOUNT:"+path.Count);
                
                //arr[1] = path;
                //v.objectParameterDataArray = arr;
                PathFindingLogic.calculateMovement(path);
                return false;

            }

            if (v.taskMetaData.name == "Water Crop")
            {
                StardewValley.Tools.WateringCan w = new WateringCan();
                bool found = false;
                foreach (var item in Game1.player.items)
                {
                    if (item == null) continue;
                    if (item.GetType() == typeof(StardewValley.Tools.WateringCan))
                    {
                        w = (WateringCan)item;
                        found = true;
                    }
                }
                if (found == false)
                {
                    removalList.Add(v);
                    return false;
                }
                if (w.WaterLeft == 0)
                {
                    CustomTask waterRefill = WaterLogic.getAllWaterTilesTask(Game1.player.currentLocation);
                    ModCore.CoreMonitor.Log("No water in can. Going to refil");
                    waterRefill.runTask();
                    return true;
                }
                //
            }
            return false;
        }

        public static bool ranAllTasks()
        {
            foreach(CustomTask task in taskList)
            {
                if (removalList.Contains(task)) continue;
                else return false;
            }
            return true;
        }

        public static void printAllTaskMetaData()
        {
            ModCore.CoreMonitor.Log(taskList.Count.ToString());
            foreach (var task in taskList)
            {
                task.taskMetaData.printMetaData();
            }
        }
    }
}
