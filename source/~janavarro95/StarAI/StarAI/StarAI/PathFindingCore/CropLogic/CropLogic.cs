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

namespace StarAI.PathFindingCore.CropLogic
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
                            Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Water"));
                            cropsToWater.Add(t);
                        }
                    }
                }
            }
            
            //Instead of just running this function I should add it to my execution queue.
            foreach(var v in cropsToWater)
            {
                object[] obj = new object[1];
                obj[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
                ExecutionCore.TaskList.taskList.Add(new ExecutionCore.CustomTask(waterSingleCrop, obj,new ExecutionCore.TaskMetaData("Water Crop", new StaminaPrerequisite(true,3),new ToolPrerequisite(true,w.GetType(),1)))); 
            //   waterSingleCrop(v);
            }
        }

        public static void waterSingleCrop(TileNode v)
        {
            object[] obj = new object[1];
            obj[0] = v;
            waterSingleCrop(obj);
        }


        public static void waterSingleCrop(object obj) {
            object[] objArr =(object[]) obj;
            TileNode v =(TileNode) objArr[0];
            bool moveOn = false;
            foreach (var q in Utilities.tileExceptionList)
            {
                if(q.tile==v && q.actionType=="Water")
                {
                    moveOn = true;
                }
            }
            if (moveOn == false) return;

                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
                int xMin = -1;
                int yMin = -1;
                int xMax = 1;
                int yMax = 1;
                List<TileNode> miniGoals = new List<TileNode>();
                List<List<TileNode>> paths = new List<List<TileNode>>();
                //try to set children to tiles where children haven't been before
                for (int x = xMin; x <= xMax; x++)
                {                    for (int y = yMin; y <= yMax; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        //Include these 4 checks for just left right up down movement. Remove them to enable 8 direction path finding
                        if (x == -1 && y == -1) continue; //upper left
                        if (x == -1 && y == 1) continue; //bottom left
                        if (x == 1 && y == -1) continue; //upper right
                        if (x == 1 && y == 1) continue; //bottom right

                        Vector2 pos = new Vector2(v.tileLocation.X + x, v.tileLocation.Y + y);
                        //ModCore.CoreMonitor.Log("AHHHHHHH POSITION: " + pos.ToString(), LogLevel.Alert);
                        bool f=  PathFindingCore.TileNode.checkIfICanPlaceHere(v, pos*Game1.tileSize, v.thisLocation,true);
                       // ModCore.CoreMonitor.Log("OK THIS IS THE RESULT F: " + f, LogLevel.Alert);
                        if (f == true)
                        {
                         
                            TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                        t.placementAction(Game1.currentLocation,(int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize);
                        //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode( t, Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize));
                        miniGoals.Add(t);
                            Utilities.tileExceptionList.Add(new TileExceptionMetaData(t,"CropNavigation"));
                        }
                    }
                }
            List<TileNode> removalList = new List<TileNode>();
                foreach(var nav in miniGoals)
                {
                    TileNode tempSource= new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                tempSource.placementAction(Game1.player.currentLocation, Game1.player.getTileX()*Game1.tileSize, Game1.player.getTileY()*Game1.tileSize);
                //StaardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(tempSource, Game1.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize));
                List<TileNode> path=  PathFindingCore.PathFindingLogic.pathFindToSingleGoalReturnPath(tempSource,nav,new List<TileNode>(),true);

                if (path.Count!=0)
                    {
                        //ModCore.CoreMonitor.Log("PATH WAS NOT NULL", LogLevel.Warn);
                        paths.Add(path);
                        foreach(var someTile in path)
                        {
                        if (someTile == nav) removalList.Add(someTile);
                            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(someTile);
                        someTile.thisLocation.objects.Remove(someTile.tileLocation);
                        //someTile.performRemoveAction(someTile.tileLocation, someTile.thisLocation);
                        //StardustCore.Utilities.masterRemovalList.Add(someTile);
                        //ModCore.CoreMonitor.Log("CAUGHT MY CULPERATE", LogLevel.Warn);
                    }
                    }
                    
                }
            Console.WriteLine("GOALS COUNT:" + miniGoals.Count);
                foreach(var q in removalList) {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(q);
                q.thisLocation.objects.Remove(q.tileLocation);
            }
            removalList.Clear();
                int pathCost = 999999999;
                List<TileNode> correctPath = new List<TileNode>();
                foreach(var potentialPath in paths)
                {
                if (potentialPath.Count == 0) continue;
                    if (potentialPath.Count < pathCost)
                    {
                        
                        pathCost = potentialPath.Count;
                        correctPath = potentialPath;
                    }
                }

                foreach (var goodTile in correctPath) {
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation,(int) goodTile.tileLocation.X*Game1.tileSize, (int)goodTile.tileLocation.Y*Game1.tileSize);

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
                         center = Utilities.parseCenterFromTile((int)v.tileLocation.X+1, (int)v.tileLocation.Y);
                        continue;
                    }
                    if (Game1.player.facingDirection == 1)
                    {
                        center = Utilities.parseCenterFromTile((int)v.tileLocation.X-1, (int)v.tileLocation.Y);
                        continue;
                    }
                    if (Game1.player.facingDirection == 0)
                    {
                        center = Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y+1);
                        continue;

                    }
                    if (Game1.player.facingDirection == 3)
                    {
                        center = Utilities.parseCenterFromTile((int)v.tileLocation.X, (int)v.tileLocation.Y-1);
                        continue;
                    }
                    Game1.player.position = center;
                  
                 
                    //Game1.setMousePosition((int)v.tileLocation.X*Game1.tileSize/2,(int)v.tileLocation.Y*Game1.tileSize/2);
                    ModCore.CoreMonitor.Log("DOESNT WATER LIKE YOU THINK IT SHOULD");
                    ModCore.CoreMonitor.Log("player pos: "+Game1.player.position.ToString(),LogLevel.Warn);
                    ModCore.CoreMonitor.Log("TilePos: "+v.position.ToString(), LogLevel.Error);
                }
                Utilities.cleanExceptionList(v);
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
                            Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Harvest"));
                            cropsToHarvest.Add(t);
                        }
                    }
                }
            }

            //Instead of just running this function I should add it to my execution queue.
            foreach (var v in cropsToHarvest)
            {
                object[] obj = new object[1];
                obj[0] = v;
                //ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(harvestSingleCrop), obj));
                ExecutionCore.TaskList.taskList.Add(new ExecutionCore.CustomTask(harvestSingleCrop, obj,new ExecutionCore.TaskMetaData("HarvestSingleCrop",null,null,new ExecutionCore.TaskPrerequisites.InventoryFullPrerequisite(true))));    
                
                //   waterSingleCrop(v);
            }
        }

        public static void harvestSingleCrop(TileNode v)
        {
            object[] obj = new object[1];
            obj[0] = v;
            harvestSingleCrop(obj);
        }



        public static void harvestSingleCrop(object obj)
        {
            object[] objArr = (object[])obj;
            TileNode v = (TileNode)objArr[0];
            foreach (var q in objArr){
                ModCore.CoreMonitor.Log("OK THIS IS THE RESULT !: " + q, LogLevel.Alert);
            }
            if(v==null) ModCore.CoreMonitor.Log("WTF MARK!!!!!!: ", LogLevel.Alert);
            bool moveOn = false;
            foreach (var q in Utilities.tileExceptionList)
            {
                if (q.tile == v && q.actionType == "Harvest")
                {
                    moveOn = true;
                }
            }
            if (moveOn == false) return;

            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_X);
            int xMin = -1;
            int yMin = -1;
            int xMax = 1;
            int yMax = 1;
            List<TileNode> miniGoals = new List<TileNode>();
            List<List<TileNode>> paths = new List<List<TileNode>>();
            //try to set children to tiles where children haven't been before
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (x == 0 && y == 0) continue;

                    //Include these 4 checks for just left right up down movement. Remove them to enable 8 direction path finding
                    if (x == -1 && y == -1) continue; //upper left
                    if (x == -1 && y == 1) continue; //bottom left
                    if (x == 1 && y == -1) continue; //upper right
                    if (x == 1 && y == 1) continue; //bottom right

                    Vector2 pos = new Vector2(v.tileLocation.X + x, v.tileLocation.Y + y);
                    //ModCore.CoreMonitor.Log("AHHHHHHH POSITION: " + pos.ToString(), LogLevel.Alert);
                    bool f = PathFindingCore.TileNode.checkIfICanPlaceHere(v, pos * Game1.tileSize, v.thisLocation, true);
                    ModCore.CoreMonitor.Log("OK THIS IS THE RESULT F: " + f, LogLevel.Alert);
                    if (f == true)
                    {

                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                        t.placementAction(Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize);
                        //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize));
                        miniGoals.Add(t);
                        Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "CropNavigation"));
                    }
                }
            }
            List<TileNode> removalList = new List<TileNode>();
            foreach (var nav in miniGoals)
            {
                TileNode tempSource = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                tempSource.placementAction(Game1.player.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
                List<TileNode> path = PathFindingCore.PathFindingLogic.pathFindToSingleGoalReturnPath(tempSource, nav, new List<TileNode>(),true);
                if (path.Count!=0)
                {
                    ModCore.CoreMonitor.Log("PATH WAS NOT NULL", LogLevel.Warn);
                    paths.Add(path);
                    foreach (var someTile in path)
                    {
                        if (someTile == nav) removalList.Add(someTile);
                        StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(someTile);
                        someTile.thisLocation.objects.Remove(someTile.tileLocation);
                        //someTile.performRemoveAction(someTile.tileLocation, someTile.thisLocation);    
                    //StardustCore.Utilities.masterRemovalList.Add(v);
                    }
                }

            }
            foreach (var q in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(q);
                q.thisLocation.objects.Remove(q.tileLocation);
            }
            removalList.Clear();
            int pathCost = 999999999;
            List<TileNode> correctPath = new List<TileNode>();
            foreach (var potentialPath in paths)
            {
                if (potentialPath.Count == 0) continue;
                if (potentialPath.Count < pathCost)
                {
                    pathCost = potentialPath.Count;
                    correctPath = potentialPath;
                }
            }

            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
            }
            //END HERE FOR JUST CALCULATING PATH COST
            
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
