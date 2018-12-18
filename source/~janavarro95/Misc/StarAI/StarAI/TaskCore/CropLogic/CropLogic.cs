using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardustCore;
using StardewValley;
using Microsoft.Xna.Framework;
using System.IO;
using StarAI.ExecutionCore.TaskPrerequisites;
using StarAI.PathFindingCore;

namespace StarAI.TaskCore.CropLogic
{
    

    class CropLogic
    {
        public static List<TileNode> cropsToWater = new List<TileNode>();
        public static List<TileNode> cropsToHarvest = new List<TileNode>();

        public static void getAllCropsNeededToBeWatered()
        {
            foreach (var v in Game1.player.currentLocation.terrainFeatures)
            {

                if (v.Value is StardewValley.TerrainFeatures.HoeDirt)
                {
                    if ((v.Value as StardewValley.TerrainFeatures.HoeDirt).crop != null)
                    {
                        //cropsToWater.Add(v.Key);
                        //If my dirt needs to be watered and the crop isn't fully grown.
                        if ((v.Value as StardewValley.TerrainFeatures.HoeDirt).state==0 && isCropFullGrown((v.Value as StardewValley.TerrainFeatures.HoeDirt).crop) == false)
                        {
                            TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightSkyBlue));
                            t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                            //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize));
                            PathFindingCore.Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Water"));
                            cropsToWater.Add(t);
                        }
                    }
                }
            }
            
            //Instead of just running this function I should add it to my execution queue.
            foreach(var v in cropsToWater)
            {
                object[] obj = new object[2];
                obj[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(waterSingleCrop, obj, new ExecutionCore.TaskMetaData("Water Crop", new LocationPrerequisite(v.thisLocation),new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                   StarAI.PathFindingCore.Utilities.clearExceptionListWithNames(true);
                    continue;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                obj[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = obj;
                //   waterSingleCrop(v);
                StarAI.PathFindingCore.Utilities.clearExceptionListWithName("Child");
            }
            cropsToWater.Clear();
        }

        public static void waterSingleCrop(TileNode v,List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            waterSingleCrop(obj);
        }


        public static void waterSingleCrop(object obj) {


            object[] objArray = (object[])obj;

            TileNode v = (TileNode)objArray[0];
            //List<TileNode> correctPath = Utilities.pathStuff(v);//(List<TileNode>)objArray[1];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);

            }
            PathFindingLogic.calculateMovement(correctPath);

            if (v.tileLocation.X < Game1.player.getTileX())
                {
                    Game1.player.faceDirection(3);
                }
                else if (v.tileLocation.X > Game1.player.getTileX())
                {
                    Game1.player.faceDirection(1);
                }
                else if (v.tileLocation.Y < Game1.player.getTileY())
                {
                    Game1.player.faceDirection(0);
                }
                else if (v.tileLocation.Y > Game1.player.getTileY())
                {
                    Game1.player.faceDirection(2);
                }
                foreach (var item in Game1.player.items)
                {
                    if(item is StardewValley.Tools.WateringCan)
                    {
                        Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                    }
                }
                bool move = false;
                while ((v.thisLocation.terrainFeatures[v.tileLocation] as StardewValley.TerrainFeatures.HoeDirt).state==0)
                {
                  if(WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C)==false)  WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);

                    Vector2 center=new Vector2();
                    if (Game1.player.facingDirection == 2)
                    {
                         center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X+1, (int)v.tileLocation.Y);
                        continue;
                    }
                    if (Game1.player.facingDirection == 1)
                    {
                        center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X-1, (int)v.tileLocation.Y);
                        continue;
                    }
                    if (Game1.player.facingDirection == 0)
                    {
                        center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y+1);
                        continue;

                    }
                    if (Game1.player.facingDirection == 3)
                    {
                        center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y-1);
                        continue;
                    }
                    Game1.player.position = center;
                  
                 
                    //Game1.setMousePosition((int)v.tileLocation.X*Game1.tileSize/2,(int)v.tileLocation.Y*Game1.tileSize/2);
                    ModCore.CoreMonitor.Log("DOESNT WATER LIKE YOU THINK IT SHOULD");
                    ModCore.CoreMonitor.Log("player pos: "+Game1.player.position.ToString(),LogLevel.Warn);
                    ModCore.CoreMonitor.Log("TilePos: "+v.position.ToString(), LogLevel.Error);
                }
            StarAI.PathFindingCore.Utilities.cleanExceptionList(v);
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            // StardustCore.Utilities.masterRemovalList.Add(v);
            //v.performRemoveAction(v.tileLocation, v.thisLocation);
            v.thisLocation.objects.Remove(v.tileLocation);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                //StardustCore.Utilities.masterRemovalList.Add(v);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);    
            //goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
            }





        public static void getAllCropsNeededToBeHarvested()
        {
            foreach (var v in Game1.player.currentLocation.terrainFeatures)
            {

                if (v.Value is StardewValley.TerrainFeatures.HoeDirt)
                {
                    if ((v.Value as StardewValley.TerrainFeatures.HoeDirt).crop != null)
                    {
                        

                        //If my dirt needs to be watered and the crop isn't fully grown.

                        if (isCropFullGrown((v.Value as StardewValley.TerrainFeatures.HoeDirt).crop))
                        {
                            ModCore.CoreMonitor.Log("OK!!!!");
                            TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LimeGreen));
                            t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                            //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize));
                            StarAI.PathFindingCore.Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Harvest"));
                            cropsToHarvest.Add(t);
                        }
                    }
                }
            }

            //Instead of just running this function I should add it to my execution queue.
            foreach (var v in cropsToHarvest)
            {
                object[] obj = new object[2];
                obj[0] = v;
                //ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(harvestSingleCrop), obj));

                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(harvestSingleCrop, obj, new ExecutionCore.TaskMetaData("HarvestSingleCrop",new LocationPrerequisite(v.thisLocation) ,null, null, new ExecutionCore.TaskPrerequisites.InventoryFullPrerequisite(true)));
                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    StarAI.PathFindingCore.Utilities.clearExceptionListWithNames(true);
                    continue;
                }

                ExecutionCore.TaskList.taskList.Add(task);
                obj[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = obj;
                StarAI.PathFindingCore.Utilities.clearExceptionListWithName("Child");
                //   waterSingleCrop(v);
            }
            cropsToHarvest.Clear();
        }

        public static void harvestSingleCrop(TileNode v,List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            harvestSingleCrop(obj);
        }



        public static void harvestSingleCrop(object obj)
        {
            object[] objArray = (object[])obj;

            TileNode v = (TileNode)objArray[0];
            //List<TileNode> correctPath = Utilities.pathStuff(v);//(List<TileNode>)objArray[1];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);

            }
            PathFindingLogic.calculateMovement(correctPath);
            if (v.tileLocation.X < Game1.player.getTileX())
            {
                Game1.player.faceDirection(3);
            }
            else if (v.tileLocation.X > Game1.player.getTileX())
            {
                Game1.player.faceDirection(1);
            }
            else if (v.tileLocation.Y < Game1.player.getTileY())
            {
                Game1.player.faceDirection(0);
            }
            else if (v.tileLocation.Y > Game1.player.getTileY())
            {
                Game1.player.faceDirection(2);
            }
            /*
            foreach (var item in Game1.player.items)
            {
                //if (item is StardewValley.Tools.WateringCan)
                //{
                   // Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                //}
            }
            */
            bool move = false;
            while ((v.thisLocation.terrainFeatures[v.tileLocation] as StardewValley.TerrainFeatures.HoeDirt).crop !=null)
            {
                if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_X) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_X);

                Vector2 center = new Vector2();
                if (Game1.player.facingDirection == 2)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X + 1, (int)v.tileLocation.Y);
                    continue;
                }
                if (Game1.player.facingDirection == 1)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X - 1, (int)v.tileLocation.Y);
                    continue;
                }
                if (Game1.player.facingDirection == 0)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y + 1);
                    continue;

                }
                if (Game1.player.facingDirection == 3)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y - 1);
                    continue;
                }
                Game1.player.position = center;
            }
            StarAI.PathFindingCore.Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            v.thisLocation.objects.Remove(v.tileLocation);
            //v.performRemoveAction(v.tileLocation, v.thisLocation);
            //StardustCore.Utilities.masterRemovalList.Add(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_X);
        }


        public static bool isCropFullGrown(Crop c)
        {

            if (c.currentPhase >= c.phaseDays.Count - 1)
            {
                c.currentPhase = c.phaseDays.Count - 1;
                c.dayOfCurrentPhase = 0;
                return true;
            }
            return false;
        }


    }
}
