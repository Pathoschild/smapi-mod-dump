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

namespace StarAI.TaskCore.DebrisLogic
{
    class DebrisLogic
    {
        public static List<TileNode> sticksToChop = new List<TileNode>();
        public static List<TileNode> stonesToBreak = new List<TileNode>();
        public static List<TileNode> weedsToCut = new List<TileNode>();
        public static List<TileNode> treesToChop = new List<TileNode>();

        //Stick Logic
        #region
        public static void getAllSticksToChop(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllSticksToChop(arr);
        }

        /// <summary>
        /// DO NOT USE THIS UNLESS YOU WANT LAG UP THE WAZOO. WILL ATTEMPT TO PATH TO ALL STICKS AT THE LOCATION AND CHOP THEM!
        /// </summary>
        /// <param name="obj"></param>
        public static void getAllSticksToChop(object obj)
        {
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];

            //If Twig is next to me just chop it.
            StardewValley.Object twig = StardustCore.Utilities.checkCardinalForObject("Twig");
            if (twig != null)
            {
                StardustCore.Utilities.faceDirectionTowardsSomething(twig.tileLocation);
                foreach (var item in Game1.player.items)
                {
                    if (item is StardewValley.Tools.Axe)
                    {
                        Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                    }
                }
                while ((twig.name == "Twig"))
                {
                    if (!location.isObjectAt((int)twig.tileLocation.X * Game1.tileSize, (int)twig.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
                    if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);
                }
                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
                return;
            }


            foreach (var v in location.objects)
            {
               // ModCore.CoreMonitor.Log(v.Value.name);

                if (v.Value.name == "Twig")
                {
                   // ModCore.CoreMonitor.Log(v.Value.name,LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                    t.fakePlacementAction(Game1.currentLocation, (int)v.Key.X, (int)v.Key.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ChopStick"));
                    sticksToChop.Add(t);
                    twingCount++;
                }
                }


            int ok = 0;

            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in sticksToChop)
            {
                tempList.Add(v);
            }

            object[] objList = new object[10];
            objList[0] = tempList;

                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Axe w = new StardewValley.Tools.Axe();
                ModCore.CoreMonitor.Log("Processing twig:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(chopSingleStick, objList, new ExecutionCore.TaskMetaData("Chop Single Stick",new LocationPrerequisite(location), new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));

            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
                {
                Utilities.tileExceptionList.Clear();
                    Utilities.clearExceptionListWithNames(true);
                    return;
                }

                ModCore.CoreMonitor.Log("TASK COST:" + task.taskMetaData.cost.ToString());

                    ExecutionCore.TaskList.taskList.Add(task);
                ModCore.CoreMonitor.Log("TASK LIST COUNT:"+ExecutionCore.TaskList.taskList.Count.ToString());
                Utilities.clearExceptionListWithName(true, "Child");
                Utilities.clearExceptionListWithName("Child");
                //   waterSingleCrop(v);
            
            sticksToChop.Clear();
        }


        public static void getAllSticksToChopRadius(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllSticksToChopRadius(arr);
        }



        public static void getAllSticksToChopRadius(object obj)
        {
            int radius = 1;
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            if (StardustCore.Utilities.doesLocationContainObject(location, "Twig")) {
                StardewValley.Object twig =StardustCore.Utilities.checkRadiusForObject(radius, "Twig");
                while (twig == null)
                {
                    radius++;
                    twig =StardustCore.Utilities.checkRadiusForObject(radius, "Twig");
                }


                ModCore.CoreMonitor.Log(twig.name);

                if (twig.name == "Twig")
                {
                    ModCore.CoreMonitor.Log(twig.name, LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                    t.fakePlacementAction(Game1.currentLocation, (int)twig.tileLocation.X, (int)twig.tileLocation.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ChopStick"));
                    sticksToChop.Add(t);
                    twingCount++;
                }
            }
            else
            {
                ModCore.CoreMonitor.Log("Twig is not found at location.");
            }
        


            int ok = 0;
            foreach (var v in sticksToChop)
            {

                object[] objList = new object[2];
                objList[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Axe w = new StardewValley.Tools.Axe();
                ModCore.CoreMonitor.Log("Processing twig:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(chopSingleStick, objList, new ExecutionCore.TaskMetaData("Chop Single Stick",new LocationPrerequisite(v.thisLocation) ,new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
                objList[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = objList;


                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                  //  System.Threading.Thread.Sleep(1000);
                    Utilities.clearExceptionListWithNames(true);
                    continue;
                }


                ExecutionCore.TaskList.taskList.Add(task);
                Utilities.clearExceptionListWithName("Child");
            }
            sticksToChop.Clear();
        }


        public static void chopSingleStick(TileNode v,List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            chopSingleStick(obj);
        }



        public static void chopSingleStick(object obj)
        {
            
            object[] objArray=(object[])obj;
           
            TileNode v = (TileNode)objArray[2];
            //List<TileNode> correctPath = Utilities.pathStuff(v);//(List<TileNode>)objArray[1];
            List<TileNode> correctPath =(List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);

            }
            PathFindingLogic.calculateMovement(correctPath);



            Vector2 tileLocation = v.tileLocation;
            ModCore.CoreMonitor.Log(tileLocation.ToString());
            StardewValley.Object sObject = new StardewValley.Object();
            try
            {
                ModCore.CoreMonitor.Log("once");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].name == "Twig")
                {
                    ModCore.CoreMonitor.Log("onceGod");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)];
                    Game1.player.faceDirection(3);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("twice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].name == "Twig")
                {
                    ModCore.CoreMonitor.Log("twiceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)];
                    Game1.player.faceDirection(1);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("thrice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)].name == "Twig")
                {
                    ModCore.CoreMonitor.Log("thriceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)];
                    Game1.player.faceDirection(0);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("fourth");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].name == "Twig")
                {
                    ModCore.CoreMonitor.Log("fourthGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)];
                    Game1.player.faceDirection(2);
                }
            }
            catch (Exception err)
            {

            }
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.Axe)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            bool move = false;
           // StardewValley.Object twig = v.thisLocation.objects[v.tileLocation];
            while ((sObject.name=="Twig"))
            {
                if (!v.thisLocation.isObjectAt((int)sObject.tileLocation.X * Game1.tileSize, (int)sObject.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
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
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
        }
        #endregion

        //Stone logic
        #region


        public static void getAllStonesToBreak(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllStonesToBreak(arr);
        }

        /// <summary>
        /// DO NOT USE THIS UNLESS YOU WANT LAG UP THE WAZOO. WILL ATTEMPT TO PATH TO ALL STICKS AT THE LOCATION AND CHOP THEM!
        /// </summary>
        /// <param name="obj"></param>
        public static void getAllStonesToBreak(object obj)
        {
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            string targetName = "Stone";

            StardewValley.Object twig = StardustCore.Utilities.checkCardinalForObject(targetName);
            if (twig != null)
            {
                StardustCore.Utilities.faceDirectionTowardsSomething(twig.tileLocation);
                foreach (var item in Game1.player.items)
                {
                    if (item is StardewValley.Tools.Pickaxe)
                    {
                        Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                    }
                }
                while ((twig.name == targetName))
                {
                    if (!location.isObjectAt((int)twig.tileLocation.X * Game1.tileSize, (int)twig.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
                    if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);
                }
                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
                return;
            }



            foreach (var v in location.objects)
            {
               // ModCore.CoreMonitor.Log(v.Value.name);

                if (v.Value.name == targetName)
                {
                    //ModCore.CoreMonitor.Log(v.Value.name, LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                    t.fakePlacementAction(Game1.currentLocation, (int)v.Key.X, (int)v.Key.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Break Stone"));
                    stonesToBreak.Add(t);
                    twingCount++;
                }
            }


            int ok = 0;
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in stonesToBreak)
            {
                tempList.Add(v);
            }

            object[] objList = new object[10];
                objList[0] = tempList;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Pickaxe w = new StardewValley.Tools.Pickaxe();
                ModCore.CoreMonitor.Log("Processing :"+ targetName+" : "+  ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(breakSingleStone, objList, new ExecutionCore.TaskMetaData("Break Single Rock",  new LocationPrerequisite(location), new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;
            if (task.taskMetaData.cost == Int32.MaxValue)
                {
                Utilities.tileExceptionList.Clear();
                    Utilities.clearExceptionListWithNames(true);
                    return;
                }
                ExecutionCore.TaskList.taskList.Add(task);
             //   Utilities.clearExceptionListWithName(true, "Child");
                Utilities.clearExceptionListWithName("Child");
                //   waterSingleCrop(v);
            
            stonesToBreak.Clear();
        }




        public static void getAllStonestoBreakRadius(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllStonesToBreakRadius(arr);
        }



        public static void getAllStonesToBreakRadius(object obj)
        {
            int radius = 1;
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            string targetName = "Stone";
            if (StardustCore.Utilities.doesLocationContainObject(location, targetName))
            {
                StardewValley.Object twig = StardustCore.Utilities.checkRadiusForObject(radius, targetName);
                while (twig == null)
                {
                    radius++;
                    twig = StardustCore.Utilities.checkRadiusForObject(radius, targetName);
                }


                ModCore.CoreMonitor.Log(twig.name);

                if (twig.name == targetName)
                {
                    ModCore.CoreMonitor.Log(twig.name, LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                    t.fakePlacementAction(Game1.currentLocation, (int)twig.tileLocation.X, (int)twig.tileLocation.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "BreakStone"));
                    stonesToBreak.Add(t);
                    twingCount++;
                }
            }
            else
            {
                ModCore.CoreMonitor.Log(targetName+" is not found at location.");
            }



            int ok = 0;
            foreach (var v in stonesToBreak)
            {

                object[] objList = new object[2];
                objList[0] = v;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Pickaxe w = new StardewValley.Tools.Pickaxe();
                ModCore.CoreMonitor.Log("Processing stone:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(breakSingleStone, objList, new ExecutionCore.TaskMetaData("Break Single Stone", new LocationPrerequisite(v.thisLocation), new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
                objList[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = objList;


                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    //  System.Threading.Thread.Sleep(1000);
                    Utilities.clearExceptionListWithNames(true);
                    continue;
                }


                ExecutionCore.TaskList.taskList.Add(task);
                Utilities.clearExceptionListWithName("Child");
            }
            stonesToBreak.Clear();
        }


        public static void breakSingleStone(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[2] = v;
            obj[1] = path;
            breakSingleStone(obj);
        }



        public static void breakSingleStone(object obj)
        {

            object[] objArray = (object[])obj;

            TileNode v = (TileNode)objArray[2];
            //List<TileNode> correctPath = Utilities.pathStuff(v);//(List<TileNode>)objArray[1];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);

            }
            PathFindingLogic.calculateMovement(correctPath);


            Vector2 tileLocation = v.tileLocation;
            ModCore.CoreMonitor.Log(tileLocation.ToString());
            StardewValley.Object sObject = new StardewValley.Object();
            try
            {
                ModCore.CoreMonitor.Log("once");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].Name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].Name == "Stone")
                {
                    ModCore.CoreMonitor.Log("onceGod");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)];
                    Game1.player.faceDirection(3);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("twice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].Name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].Name == "Stone")
                {
                    ModCore.CoreMonitor.Log("twiceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)];
                    Game1.player.faceDirection(1);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("thrice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)].Name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)].Name == "Stone")
                {
                    ModCore.CoreMonitor.Log("thriceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y - 1)];
                    Game1.player.faceDirection(0);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("fourth");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].Name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].Name == "Stone")
                {
                    ModCore.CoreMonitor.Log("fourthGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)];
                    Game1.player.faceDirection(2);
                }
            }
            catch (Exception err)
            {

            }
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.Pickaxe)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            bool move = false;
            //StardewValley.Object twig = v.thisLocation.objects[v.tileLocation];
            ModCore.CoreMonitor.Log("NAME"+sObject.name);
            while ((sObject.name == "Stone"))
            {
                if (!v.thisLocation.isObjectAt((int)sObject.tileLocation.X * Game1.tileSize, (int)sObject.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
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
                ModCore.CoreMonitor.Log("DOESNT Pickaxe LIKE YOU THINK IT SHOULD");
                ModCore.CoreMonitor.Log("player pos: " + Game1.player.position.ToString(), LogLevel.Warn);
                ModCore.CoreMonitor.Log("TilePos: " + v.position.ToString(), LogLevel.Error);
            }
            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
                //goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
        }

        #endregion

        //Weed Logic
        #region

        public static void getAllWeedsToCut(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllWeedsToCut(arr);
        }

        /// <summary>
        /// DO NOT USE THIS UNLESS YOU WANT LAG UP THE WAZOO. WILL ATTEMPT TO PATH TO ALL STICKS AT THE LOCATION AND CHOP THEM!
        /// </summary>
        /// <param name="obj"></param>
        public static void getAllWeedsToCut(object obj)
        {
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            string targetName = "Weeds";
            StardewValley.Object twig = StardustCore.Utilities.checkCardinalForObject(targetName);
            if (twig != null)
            {
                StardustCore.Utilities.faceDirectionTowardsSomething(twig.tileLocation);
                foreach (var item in Game1.player.items)
                {
                    if (item is StardewValley.Tools.Pickaxe)
                    {
                        Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                    }
                }
                while ((twig.name == targetName))
                {
                    if (!location.isObjectAt((int)twig.tileLocation.X * Game1.tileSize, (int)twig.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
                    if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);
                }
                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
                return;
            }
            foreach (var v in location.objects)
            {
               // ModCore.CoreMonitor.Log(v.Value.name);

                if (v.Value.name == "Weeds")
                {
               //     ModCore.CoreMonitor.Log(v.Value.name, LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    //t.placementAction(Game1.currentLocation, (int)v.Key.X * Game1.tileSize, (int)v.Key.Y * Game1.tileSize);
                    t.fakePlacementAction(Game1.currentLocation, (int)v.Key.X, (int)v.Key.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "CutWeeds"));
                    weedsToCut.Add(t);
                    twingCount++;
                }
            }


            int ok = 0;


            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in weedsToCut)
            {
                tempList.Add(v);
            }

            object[] objList = new object[10];
            objList[0] = tempList;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.MeleeWeapon w = new StardewValley.Tools.MeleeWeapon();
                ModCore.CoreMonitor.Log("Processing weed:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(cutSingleWeed, objList, new ExecutionCore.TaskMetaData("Cut Single Weed", new LocationPrerequisite(location), null, new ToolPrerequisite(true, w.GetType(), 1)));
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;
            if (task.taskMetaData.cost == Int32.MaxValue)
                {
                Utilities.tileExceptionList.Clear();
                    Utilities.clearExceptionListWithNames(true);
                    return;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                Utilities.clearExceptionListWithName("Child");
                //   waterSingleCrop(v);
            
            weedsToCut.Clear();
        }


        public static void getAllWeedsToCutRadius(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllWeedsToCutRadius(arr);
        }



        public static void getAllWeedsToCutRadius(object obj)
        {
            int radius = 1;
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            string targetName = "Weeds";
            if (StardustCore.Utilities.doesLocationContainObject(location, targetName))
            {
                StardewValley.Object twig = StardustCore.Utilities.checkRadiusForObject(radius, targetName);
                while (twig == null)
                {
                    radius++;
                    twig = StardustCore.Utilities.checkRadiusForObject(radius, targetName);
                }

                if (twig.name == targetName)
                {
                    ModCore.CoreMonitor.Log(twig.name, LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));             
                    t.fakePlacementAction(Game1.currentLocation, (int)twig.tileLocation.X, (int)twig.tileLocation.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "CutWeeds"));
                    weedsToCut.Add(t);
                    twingCount++;
                }
            }
            else
            {
                ModCore.CoreMonitor.Log("Weeds is not found at location.");
            }

            int ok = 0;
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in treesToChop)
            {
                tempList.Add(v);
            }

                object[] objList = new object[10];
                objList[0] = tempList;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.MeleeWeapon w = new StardewValley.Tools.MeleeWeapon();
                ModCore.CoreMonitor.Log("Processing weeds:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(cutSingleWeed, objList, new ExecutionCore.TaskMetaData("Cut Single Weed", new LocationPrerequisite(location), new StaminaPrerequisite(true, 3), new ToolPrerequisite(true, w.GetType(), 1)));
                objList[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = objList;

                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    Utilities.clearExceptionListWithNames(true);
                    return;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                Utilities.clearExceptionListWithName("Child");
            
            weedsToCut.Clear();
        }


        public static void cutSingleWeed(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            cutSingleWeed(obj);
        }



        public static void cutSingleWeed(object obj)
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
            StardewValley.Object sObject = new StardewValley.Object();
            try
            {
                ModCore.CoreMonitor.Log("once");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)].name=="Weeds")
                {
                    ModCore.CoreMonitor.Log("onceGod");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X - 1, tileLocation.Y)];
                    Game1.player.faceDirection(3);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("twice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)].name=="Weeds")
                {
                    ModCore.CoreMonitor.Log("twiceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X + 1, tileLocation.Y)];
                    Game1.player.faceDirection(1);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("thrice");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y-1)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y-1)].name == "Weeds")
                {
                    ModCore.CoreMonitor.Log("thriceGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y-1)];
                    Game1.player.faceDirection(0);
                }
            }
            catch (Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("fourth");
                ModCore.CoreMonitor.Log(v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].name);
                if (v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y + 1)].name == "Weeds")
                {
                    ModCore.CoreMonitor.Log("fourthGood");
                    sObject = v.thisLocation.objects[new Vector2(tileLocation.X, tileLocation.Y +1)];
                    Game1.player.faceDirection(2);
                }
            }
            catch (Exception err)
            {

            }
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.MeleeWeapon)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            
            while ((sObject.name == "Weeds"))
            {
                if (!v.thisLocation.isObjectAt((int)sObject.tileLocation.X * Game1.tileSize, (int)sObject.tileLocation.Y * Game1.tileSize)) break; //CHANNGE THIS LINE
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
                ModCore.CoreMonitor.Log("DOESNT Slice LIKE YOU THINK IT SHOULD");
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

        #endregion


        //Tree Logic
        #region
        public static void getAllTreesToChopRadius(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllTreesToChopRadius(arr);
        }



        public static void getAllTreesToChopRadius(object obj)
        {
            int radius = 1;
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            // string targetName = "Weeds";
            Type terrainType = typeof(StardewValley.TerrainFeatures.Tree);


            if (StardustCore.Utilities.doesLocationContainTerrainFeature(location,terrainType))
            {
               
                KeyValuePair<Vector2, StardewValley.TerrainFeatures.TerrainFeature> pair = StardustCore.Utilities.checkRadiusForTerrainFeature(radius, terrainType);
                Vector2 pos = pair.Key;
                StardewValley.TerrainFeatures.TerrainFeature terrain = pair.Value;
                while (terrain == null)
                {
                    ModCore.CoreMonitor.Log(radius.ToString());
                    radius++;
                    pair=  StardustCore.Utilities.checkRadiusForTerrainFeature(radius, terrainType);
                    pos = pair.Key;
                    terrain = pair.Value;
                }
                
                if (terrain.GetType() == terrainType)
                {
                    ModCore.CoreMonitor.Log(terrain.GetType().ToString(), LogLevel.Warn);
                    ModCore.CoreMonitor.Log(pos.ToString(), LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, (int)pos.X, (int)pos.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ChopTree"));
                    treesToChop.Add(t);
                    twingCount++;
                }
            }
            else
            {
                ModCore.CoreMonitor.Log("Trees not found at location.");
            }

            int ok = 0;
           
                object[] objList = new object[2];
                objList[0] = treesToChop;
                // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
                StardewValley.Tools.Axe w = new StardewValley.Tools.Axe();
                ModCore.CoreMonitor.Log("Processing Trees:" + ok.ToString() + " / " + twingCount.ToString());
                ok++;
                int numberOfUses = 15;
                ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(chopSingleTree, objList, new ExecutionCore.TaskMetaData("ChopSingleTree", new LocationPrerequisite(location), new StaminaPrerequisite(true, 2*numberOfUses), new ToolPrerequisite(true, w.GetType(), numberOfUses)));
                objList[1] = task.taskMetaData.pathsToTake[0];
                task.objectParameterDataArray = objList;

                if (task.taskMetaData.cost == Int32.MaxValue)
                {
                    Utilities.clearExceptionListWithNames(true);
                    return;
                }
                ExecutionCore.TaskList.taskList.Add(task);
                Utilities.clearExceptionListWithName("Child");
            
                treesToChop.Clear();
        }

        public static void getAllTreesToChop(GameLocation location)
        {
            object[] arr = new object[1];
            arr[0] = location;
            getAllTreesToChop(arr);
        }



        public static void getAllTreesToChop(object obj)
        {
            int radius = 1;
            int twingCount = 0;
            object[] objArr = (object[])obj;
            GameLocation location = (GameLocation)objArr[0];
            // string targetName = "Weeds";
            Type terrainType = typeof(StardewValley.TerrainFeatures.Tree);
            if ((StardustCore.Utilities.doesLocationContainTerrainFeature(location, terrainType) == false))
            {
                ModCore.CoreMonitor.Log("Trees not found at location.");
                return;
            }


            KeyValuePair<Vector2, StardewValley.TerrainFeatures.TerrainFeature> pair2 = StardustCore.Utilities.checkCardinalForTerrainFeature(terrainType);
            if (pair2.Value != null)
            {
                while (location.terrainFeatures.ContainsValue(pair2.Value))
                {
                    //  if (!v.thisLocation.isTerrainFeatureAt((int)v.tileLocation.X, (int)v.tileLocation.Y)) break;
                    if (WindowsInput.InputSimulator.IsKeyDown(WindowsInput.VirtualKeyCode.VK_C) == false) WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.VK_C);
                }
                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.VK_C);
                return;
            }

            foreach (var v in location.terrainFeatures)
            {
                if (v.Value.GetType() != terrainType) continue; 
                //KeyValuePair<Vector2, StardewValley.TerrainFeatures.TerrainFeature> pair = StardustCore.Utilities.checkRadiusForTerrainFeature(radius, terrainType);
                Vector2 pos = v.Key;
                StardewValley.TerrainFeatures.TerrainFeature terrain = v.Value;


                if (terrain.GetType() == terrainType)
                {
                    //ModCore.CoreMonitor.Log(terrain.GetType().ToString(), LogLevel.Warn);
                    //ModCore.CoreMonitor.Log(pos.ToString(), LogLevel.Warn);
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, (int)pos.X, (int)pos.Y);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ChopTree"));
                    treesToChop.Add(t);
                    twingCount++;
                }
            }
          
           
            int ok = 0;

            object[] objList = new object[10];
            List<TileNode> tempTreesToChop = new List<TileNode>();
            foreach(var v in treesToChop)
            {
                tempTreesToChop.Add(v);
            }
            objList[0] = tempTreesToChop;
            
            // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
            StardewValley.Tools.Axe w = new StardewValley.Tools.Axe();
            ModCore.CoreMonitor.Log("Processing Trees:" + treesToChop.Count.ToString() + " / " + twingCount.ToString());
            ok++;
            int numberOfUses = 15;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(chopSingleTree, objList, new ExecutionCore.TaskMetaData("ChopSingleTree", new LocationPrerequisite(location), new StaminaPrerequisite(true, 2 * numberOfUses), new ToolPrerequisite(true, w.GetType(), numberOfUses)));
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");

            treesToChop.Clear();
            Utilities.tileExceptionList.Clear();
        }


        public static void chopSingleTree(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            chopSingleTree(obj);
        }



        public static void chopSingleTree(object obj)
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
            StardewValley.TerrainFeatures.TerrainFeature tree=new StardewValley.TerrainFeatures.Tree();
            //if(v.thisLocation.isTerrainFeatureAt)
            try
            {
                ModCore.CoreMonitor.Log("once");
                ModCore.CoreMonitor.Log(v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)].GetType().ToString());
                if (v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
                {
                    ModCore.CoreMonitor.Log("onceGod");
                    tree = v.thisLocation.terrainFeatures[new Vector2(tileLocation.X - 1, tileLocation.Y)];
                    Game1.player.faceDirection(3);
                }
            }
            catch(Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("twice");
                ModCore.CoreMonitor.Log(v.thisLocation.terrainFeatures[new Vector2(tileLocation.X +1, tileLocation.Y)].GetType().ToString());
                if (v.thisLocation.terrainFeatures[new Vector2(tileLocation.X + 1, tileLocation.Y)] is StardewValley.TerrainFeatures.Tree)
                {
                    ModCore.CoreMonitor.Log("twiceGood");
                    tree = v.thisLocation.terrainFeatures[new Vector2(tileLocation.X + 1, tileLocation.Y)];
                    Game1.player.faceDirection(1);
                }
            }
            catch(Exception err)
            {

            }
            try
            {
                ModCore.CoreMonitor.Log("thrice");
                ModCore.CoreMonitor.Log(v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y-1)].GetType().ToString());
                if (v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y - 1)] is StardewValley.TerrainFeatures.Tree)
                {
                    ModCore.CoreMonitor.Log("thriceGood");
                    tree = v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y - 1)];
                    Game1.player.faceDirection(0);
                }
            }
            catch (Exception err) {

            }
            try
            {
                ModCore.CoreMonitor.Log("quad");
                ModCore.CoreMonitor.Log(v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y+1)].GetType().ToString());
                if (v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y + 1)] is StardewValley.TerrainFeatures.Tree)
                {
                    ModCore.CoreMonitor.Log("quadGood");
                    tree = v.thisLocation.terrainFeatures[new Vector2(tileLocation.X, tileLocation.Y + 1)];
                    Game1.player.faceDirection(2);
                }
            }
            catch(Exception err)
            {

            }
            foreach (var item in Game1.player.items)
            {
                if (item is StardewValley.Tools.Axe)
                {
                    Game1.player.CurrentToolIndex = Game1.player.getIndexOfInventoryItem(item);
                }
            }
            
            while (v.thisLocation.terrainFeatures.ContainsValue(tree))
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
        #endregion

    }
}
