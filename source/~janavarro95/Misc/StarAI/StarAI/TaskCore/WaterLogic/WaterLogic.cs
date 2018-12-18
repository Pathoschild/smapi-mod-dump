using Microsoft.Xna.Framework;
using StarAI.ExecutionCore;
using StarAI.ExecutionCore.TaskPrerequisites;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;

namespace StarAI.PathFindingCore.WaterLogic
{
    public class WaterLogic
    {

        public static List<TileNode> waterTilesAvailable = new List<TileNode>();

        public static void getAllWaterTiles(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllWaterTiles(arr);
        }



        public static void getAllWaterTiles(object obj)
        {
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            // string targetName = "Weeds";

            Layer layer = location.map.GetLayer("Back");
            for(int i=0; i <= layer.LayerSize.Width; i++)
            {
                for (int j = 0; j <= layer.LayerSize.Height; j++)
                {
                    if (location.isOpenWater(i, j))
                    {
                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                        t.fakePlacementAction(location, i,j);
                        Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ChopTree"));
                        waterTilesAvailable.Add(t);
                        twingCount++;
                    }
                }
            }



            int ok = 0;

            object[] objList = new object[3];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in waterTilesAvailable)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;

            // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
            StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
            ModCore.CoreMonitor.Log("Processing water tiles:" + waterTilesAvailable.Count.ToString() + " / " + twingCount.ToString());
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToSingleWaterTile, objList, new ExecutionCore.TaskMetaData("GoToWaterTile", new LocationPrerequisite(location), new StaminaPrerequisite(true, 2 * numberOfUses), new ToolPrerequisite(true, w.GetType(), numberOfUses)));


            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");

            waterTilesAvailable.Clear();
            Utilities.tileExceptionList.Clear();
        }


        public static CustomTask getAllWaterTilesTask(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            return getAllWaterTilesTask(arr);
        }

        public static CustomTask getAllWaterTilesTask(object obj)
        {
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            // string targetName = "Weeds";

            Layer layer = location.map.GetLayer("Back");
            for (int i = 0; i <= layer.LayerSize.Width; i++)
            {
                for (int j = 0; j <= layer.LayerSize.Height; j++)
                {
                    if (location.isOpenWater(i, j))
                    {
                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                        t.fakePlacementAction(location, i, j);
                        Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "WaterTile"));
                        waterTilesAvailable.Add(t);
                        twingCount++;
                    }
                }
            }



            int ok = 0;

            object[] objList = new object[3];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in waterTilesAvailable)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;

            // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
            StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
            ModCore.CoreMonitor.Log("Processing water tiles:" + waterTilesAvailable.Count.ToString() + " / " + twingCount.ToString());
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToSingleWaterTile, objList, new ExecutionCore.TaskMetaData("GoToWaterTile", new LocationPrerequisite(location), new StaminaPrerequisite(true, 2 * numberOfUses), new ToolPrerequisite(true, w.GetType(), numberOfUses)));


            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return null;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;
            // ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");

            waterTilesAvailable.Clear();
            Utilities.tileExceptionList.Clear();
            return task;
        }


        public static void goToSingleWaterTile(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            goToSingleWaterTile(obj);
        }



        public static void goToSingleWaterTile(object obj)
        {
            object[] objArray = (object[])obj;

            TileNode v = (TileNode)objArray[2];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            PathFindingLogic.calculateMovement(correctPath);
            Vector2 tileLocation = v.tileLocation;
            ModCore.CoreMonitor.Log(tileLocation.ToString());
            //if(v.thisLocation.isTerrainFeatureAt)
           
           if (v.thisLocation.isOpenWater((int)v.tileLocation.X-1,(int)v.tileLocation.Y)) //v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
           {
                ModCore.CoreMonitor.Log("1Good");
                Game1.player.faceDirection(3);
           }
            if (v.thisLocation.isOpenWater((int)v.tileLocation.X + 1, (int)v.tileLocation.Y)) //v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
            {
                ModCore.CoreMonitor.Log("2Good");
                Game1.player.faceDirection(1);
            }
            if (v.thisLocation.isOpenWater((int)v.tileLocation.X, (int)v.tileLocation.Y-1)) //v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
            {
                ModCore.CoreMonitor.Log("3Good");
                Game1.player.faceDirection(0);
            }
            if (v.thisLocation.isOpenWater((int)v.tileLocation.X, (int)v.tileLocation.Y+1)) //v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
            {
                ModCore.CoreMonitor.Log("4Good");
                Game1.player.faceDirection(2);
            }


            
            StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.WateringCan)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                    w = (WateringCan)item;
                }
            }

            while (w.WaterLeft!=w.waterCanMax)
            {
                //  if (!v.thisLocation.isTerrainFeatureAt((int)v.tileLocation.X, (int)v.tileLocation.Y)) break;
                if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);

                Vector2 center = new Vector2();
                if (Game1.player.facingDirection == 2)
                {
                    center = Utilities.parseCenterFromTile((int)v.tileLocation.X + 1, (int)v.tileLocation.Y);
                    continue;
                }
                if (Game1.player.facingDirection == 1)
                {
                    center = Utilities.parseCenterFromTile((int)v.tileLocation.X - 1, (int)v.tileLocation.Y);
                    continue;
                }
                if (Game1.player.facingDirection == 0)
                {
                    center = Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y + 1);
                    continue;

                }
                if (Game1.player.facingDirection == 3)
                {
                    center = Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y - 1);
                    continue;
                }
                Game1.player.position = center;
                //Game1.setMousePosition((int)v.tileLocation.X*Game1.tileSize/2,(int)v.tileLocation.Y*Game1.tileSize/2);
                ModCore.CoreMonitor.Log("DOESNT Axe LIKE YOU THINK IT SHOULD");
                ModCore.CoreMonitor.Log("player pos: " + Game1.player.position.ToString(), LogLevel.Warn);
                ModCore.CoreMonitor.Log("TilePos: " + v.position.ToString(), LogLevel.Error);
            }
            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
            }
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
        }




    }
}
