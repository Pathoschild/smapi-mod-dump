using Microsoft.Xna.Framework;
using StarAI.ExecutionCore.TaskPrerequisites;
using StarAI.PathFindingCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace StarAI.TaskCore.CropLogic
{
    class SeedLogic
    {

        public static void makeAsMuchDirtAsSpecifiedAroundFarmer(GameLocation location, int amount, int radius)
        {
            List<TileNode> hoeDirtThings = new List<TileNode>();
            for(int i = -radius; i <= radius;i++)
            {
                for (int j = -radius; j <= radius;j++)
                {
                    Vector2 position = new Vector2(Game1.player.getTileX() + i, Game1.player.getTileY() + j);
                    //if (hoeDirtThings.Count >= amount) continue;
                    if(canBeHoeDirt(location, position))
                    {
                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightSkyBlue));
                        //t.placementAction(Game1.currentLocation, (int)i* Game1.tileSize, (int)j * Game1.tileSize);
                        t.fakePlacementAction(location, (int)position.X,(int)position.Y);
                        //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize));
                        PathFindingCore.Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "HoeDirt"));
                        hoeDirtThings.Add(t);
                    }
                }
            }
            int taskAmount = 0;
            foreach (var v in hoeDirtThings)
            {
                if (taskAmount >= amount) break;
                object[] obj = new object[2];
                obj[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Hoe w = new StardewValley.Tools.Hoe();
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(hoeSingleTileOfDirt, obj, new ExecutionCore.TaskMetaData("Dig Dirt", new LocationPrerequisite(v.thisLocation), new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    StarAI.PathFindingCore.Utilities.clearExceptionListWithNames(true);
                    Utilities.tileExceptionList.Clear();
                    continue;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                taskAmount++;
                obj[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = obj;
                //   waterSingleCrop(v);
                StarAI.PathFindingCore.Utilities.clearExceptionListWithName("Child");
            }
            hoeDirtThings.Clear();


        }

        public static void hoeSingleTileOfDirt(object obj)
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
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.Hoe)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            bool move = false;
          
            
            while (Game1.player.currentLocation.isTileHoeDirt(v.tileLocation)==false)
            {
                if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);

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


                //Game1.setMousePosition((int)v.tileLocation.X*Game1.tileSize/2,(int)v.tileLocation.Y*Game1.tileSize/2);
                ModCore.CoreMonitor.Log("DOESNT Dig dirt LIKE YOU THINK IT SHOULD");
                ModCore.CoreMonitor.Log("player pos: " + Game1.player.position.ToString(), LogLevel.Warn);
                ModCore.CoreMonitor.Log("TilePos: " + v.position.ToString(), LogLevel.Error);
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

        public static bool canBeHoeDirt(GameLocation location, Vector2 tileLocation)
        {
            if (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") == null || location.isTileOccupied(tileLocation, "") || !location.isTilePassable(new Location((int)tileLocation.X, (int)tileLocation.Y), Game1.viewport))
                return false;
            else return true;
            //this.terrainFeatures.Add(tileLocation, (TerrainFeature)new HoeDirt(!Game1.isRaining || !this.isOutdoors ? 0 : 1));
        }

        public static Crop parseCropFromSeedIndex(int index)
        {
            return new Crop(index, 0, 0);
        }

        public static KeyValuePair<int, Crop> getSeedCropPair(int index) {

            return new KeyValuePair<int, Crop>(index, parseCropFromSeedIndex(index));
        }

        public static void buySeeds()
        {
            var retList = UtilityCore.SeedCropUtility.sortSeedListByUtility(ShopCore.ShopLogic.getGeneralStoreSeedStock(true));
           var item = retList.ElementAt(0);
            item.Stack++;
            while (Game1.player.money >= item.salePrice())
            {
                item.Stack++;
                Game1.player.money -= item.salePrice();
            }
            Game1.player.addItemToInventoryBool(item);

        }



        public static void plantSeeds(GameLocation location2)
        {
            List<TileNode> seedsToPlant = new List<TileNode>();
            GameLocation location = Game1.getLocationFromName("Farm");
            string name = "";
            foreach (var seed in Game1.player.items)
            {
                if (seed == null) continue;
                if (seed.getCategoryName() == "Seed")
                {
                    if (parseCropFromSeedIndex(seed.parentSheetIndex).seasonsToGrowIn.Contains(Game1.currentSeason))
                    {
                        name = seed.Name;
                    }
                }
            }
            if (name == "")
            {
                ModCore.CoreMonitor.Log("Error: No valid seeds to plant found in inventory. Try to go buy some");
                Utilities.tileExceptionList.Clear();
                return;
            }

            foreach (var terrain in location.terrainFeatures)
            {
                if (terrain.Value is StardewValley.TerrainFeatures.HoeDirt)
                {
                    //Vector2 position = new Vector2(Game1.player.getTileX() + i, Game1.player.getTileY() + j);
                    //if (hoeDirtThings.Count >= amount) continue;
                    if ((terrain.Value as StardewValley.TerrainFeatures.HoeDirt).crop != null) continue;
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightSkyBlue));
                    //t.placementAction(Game1.currentLocation, (int)i* Game1.tileSize, (int)j * Game1.tileSize);
                    t.fakePlacementAction(location, (int)terrain.Key.X, (int)terrain.Key.Y);
                    //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize));
                    PathFindingCore.Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "PlantSeeds"));
                    seedsToPlant.Add(t);
                }
            }
            int taskAmount = 0;
            foreach (var v in seedsToPlant)
            {
                object[] obj = new object[2];
                obj[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));

                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(plantSingleSeedPacket, obj, new ExecutionCore.TaskMetaData("Plant "+name, new LocationPrerequisite(v.thisLocation), null, null));
                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    StarAI.PathFindingCore.Utilities.clearExceptionListWithNames(true);
                    Utilities.tileExceptionList.Clear();
                    continue;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                taskAmount++;
                obj[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = obj;
                //   waterSingleCrop(v);
                StarAI.PathFindingCore.Utilities.clearExceptionListWithName("Child");
                Utilities.tileExceptionList.Clear();
            }
            seedsToPlant.Clear();


        }

        public static void plantSingleSeedPacket(object obj)
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
            foreach (var item in Game1.player.items)
            {
                if (item == null) continue;
                if (item.getCategoryName()=="Seed")
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            bool move = false;


            while ((v.thisLocation.terrainFeatures[v.tileLocation] as StardewValley.TerrainFeatures.HoeDirt).crop==null)
            {
                if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_X) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_X);

                Vector2 center = new Vector2();
                if (Game1.player.facingDirection == 2)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X + 1, (int)v.tileLocation.Y);
                    //continue;
                }
                if (Game1.player.facingDirection == 1)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X - 1, (int)v.tileLocation.Y);
                   // continue;
                }
                if (Game1.player.facingDirection == 0)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y + 1);
                    //continue;

                }
                if (Game1.player.facingDirection == 3)
                {
                    center = StarAI.PathFindingCore.Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y - 1);
                   // continue;
                }
                //Game1.player.position = center;

                Crop c= parseCropFromSeedIndex(Game1.player.ActiveObject.parentSheetIndex);
                (v.thisLocation.terrainFeatures[v.tileLocation] as StardewValley.TerrainFeatures.HoeDirt).crop = c;

                if (Game1.player.ActiveObject.stack > 1)
                {
                    Game1.player.reduceActiveItemByOne();
                   // Game1.player.ActiveObject.stack--;
                }
                else
                {
                    Game1.player.items.Remove(Game1.player.ActiveObject);
                    Game1.player.ActiveObject = null;
                }

                //Game1.setMousePosition((int)v.tileLocation.X*Game1.tileSize/2,(int)v.tileLocation.Y*Game1.tileSize/2);
                ModCore.CoreMonitor.Log("DOESNT Plant Seeds LIKE YOU THINK IT SHOULD");
                ModCore.CoreMonitor.Log("player pos: " + Game1.player.position.ToString(), LogLevel.Warn);
                ModCore.CoreMonitor.Log("TilePos: " + v.position.ToString(), LogLevel.Error);
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
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_X);
        }


    }
}
