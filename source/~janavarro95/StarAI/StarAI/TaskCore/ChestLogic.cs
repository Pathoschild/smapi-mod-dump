using Microsoft.Xna.Framework;
using StarAI.PathFindingCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.TaskCore
{
    class ChestLogic
    {
        public static List<TileNode> chestsAtThisLocation = new List<TileNode>();

        public static void getAllSeasonalSeedsFromAllChestsAtLocation(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllSeasonalSeedsFromAllChestsAtLocation(arr);
        }

        public static void getAllSeasonalSeedsFromAllChestsAtLocation(object obj)
        {
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            foreach (var v in location.objects)
            {
                ModCore.CoreMonitor.Log(v.Value.name);
                if (v.Value is StardewValley.Objects.Chest)
                {
                    //if contains seeds that can be planted this season.
                    foreach(var item in (v.Value as StardewValley.Objects.Chest).items)
                    {
                        if (item.getCategoryName() == "Seed")
                        {
                           
                            StardewValley.Crop c = new Crop(item.parentSheetIndex, 0, 0);
                            if (c.seasonsToGrowIn.Contains(Game1.currentSeason))
                            {
                                TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                                 //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                                t.tileLocation=new Vector2((int)v.Key.X, (int)v.Key.Y);
                                t.position = new Vector2(v.Key.X*Game1.tileSize, v.Key.Y*Game1.tileSize);
                                t.thisLocation = location;
                                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize));
                                Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Chest"));
                                chestsAtThisLocation.Add(t);
                            }
                        }
                    }

                }

            }

            foreach (var v in chestsAtThisLocation)
            {
                object[] objList = new object[2];
                objList[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(pathToSingleChest, objList, new ExecutionCore.TaskMetaData("GetSeasonalSeedsFromChest",new ExecutionCore.TaskPrerequisites.LocationPrerequisite(v.thisLocation), null, null, new ExecutionCore.TaskPrerequisites.InventoryFullPrerequisite(true)));
                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    Utilities.clearExceptionListWithNames(true);
                    continue;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                //Utilities.clearExceptionListWithName(true, "Child");
                if (task.taskMetaData.pathsToTake == null)
                {
                    ModCore.CoreMonitor.Log("WTF???");
                }
                ModCore.CoreMonitor.Log("COST:"+task.taskMetaData.cost);
                objList[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = objList;
                Utilities.clearExceptionListWithName("Child");
                Utilities.clearExceptionListWithName("Navigation");

                //   waterSingleCrop(v);
            }
            chestsAtThisLocation.Clear();
        }

        public static void pathToSingleChest(object obj)
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

            bool move = false;
            Chest chest =(Chest) v.thisLocation.objects[v.tileLocation];
            List<Item> removalListSeeds = new List<Item>();
            //Try to grab all the seeds I can from the chest.
            while (Game1.player.isInventoryFull()==false&&chest.items.Count>0)
            {

                if (chest.giftbox)
                {
                    ModCore.CoreMonitor.Log("GIFT BOX", LogLevel.Warn);
                    v.thisLocation.objects.Remove(v.tileLocation);
                }
                foreach (var item in chest.items)
                {
                    if (item.getCategoryName() == "Seed")
                    {
                        int seedIndex = item.parentSheetIndex;
                        
                        if (seedIndex == 770)
                        {
                            seedIndex = Crop.getRandomLowGradeCropForThisSeason(Game1.currentSeason);
                            if (seedIndex == 473)
                                --seedIndex;
                        }

                        StardewValley.Crop c = new Crop(seedIndex, 0, 0);

                        if (c.seasonsToGrowIn.Contains(Game1.currentSeason))
                        {
                            Game1.player.addItemByMenuIfNecessary(item);
                            removalListSeeds.Add(item);
                            break;
                        }
                    }
                }


                foreach(var remove in removalListSeeds)
                {
                    chest.items.Remove(remove);
                }
                // if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);



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

            }
            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            // StardustCore.Utilities.masterRemovalList.Add(v);
            //v.performRemoveAction(v.tileLocation, v.thisLocation);
           // v.thisLocation.objects.Remove(v.tileLocation);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                //StardustCore.Utilities.masterRemovalList.Add(v);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
                //goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            //WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
        }


    }
}
