using Microsoft.Xna.Framework;
using StarAI.ExecutionCore.TaskPrerequisites;
using StarAI.PathFindingCore;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.TaskCore.CropLogic
{
    class ShippingLogic
    {
        public static void goToShippingBinSetUp()
        {
            List<TileNode> shippingTiles = new List<TileNode>();
            if (Game1.player.currentLocation.name == "Farm")
            {
                //CHEATING AND STUPID WAY BUT WILL PATH TO SHIPPING BIN.
                for (int i = 0; i <= 1; i++)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, 71+i, 14);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ShippingBin"));
                    shippingTiles.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[3];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in shippingTiles)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;

            // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
            StardewValley.Tools.WateringCan w = new StardewValley.Tools.WateringCan();
            //ModCore.CoreMonitor.Log("Processing water tiles:" + shippingTiles.Count.ToString() + " / " + twingCount.ToString());
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToShippingBin, objList, new ExecutionCore.TaskMetaData("GoToShippingBin", null, null,null,null,null));
           
            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();


        }


        public static void goToShippingBin(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            goToShippingBin(obj);
        }



        public static void goToShippingBin(object obj)
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
            //ModCore.CoreMonitor.Log(tileLocation.ToString());
            //if(v.thisLocation.isTerrainFeatureAt)

            //DO SOME LOGIC HERE IF I WANT TO SHIP???

            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
            }
        
        }



        public static void goToShippingBinShipItem(Item I)
        {
            List<TileNode> shippingTiles = new List<TileNode>();
            ModCore.CoreMonitor.Log(I.Name);
            
            if (I==null) ModCore.CoreMonitor.Log("DIE");
            if (Game1.player.currentLocation.name == "Farm")
            {
                //CHEATING AND STUPID WAY BUT WILL PATH TO SHIPPING BIN.
                for (int i = 0; i <= 1; i++)
                {
                    TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Brown));
                    t.fakePlacementAction(Game1.currentLocation, 71 + i, 14);
                    Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "ShippingBin"));
                    shippingTiles.Add(t);
                }
            }
            int ok = 0;

            object[] objList = new object[4];
            List<TileNode> tempList = new List<TileNode>();
            foreach (var v in shippingTiles)
            {
                tempList.Add(v);
            }
            objList[0] = tempList;
            objList[3] = I;
            // ExecutionCore.TaskList.taskList.Add(new Task(new Action<object>(waterSingleCrop), obj));
            //ModCore.CoreMonitor.Log("Processing water tiles:" + shippingTiles.Count.ToString() + " / " + twingCount.ToString());
            ok++;
            int numberOfUses = 1;
            ExecutionCore.CustomTask task = new ExecutionCore.CustomTask(goToShippingBinShipItem, objList, new ExecutionCore.TaskMetaData("GoToShippingBin",new LocationPrerequisite(Game1.getLocationFromName("Farm")), null, null, null, null,new ItemPrerequisite(I,I.Stack)));

            task.objectParameterDataArray = objList;

            if (task.taskMetaData.cost == Int32.MaxValue)
            {
                Utilities.clearExceptionListWithNames(true);
                return;
            }
            objList[1] = task.taskMetaData.pathsToTake[0];
            objList[2] = task.taskMetaData.pathsToTake[0].ElementAt(0);
            ExecutionCore.TaskList.taskList.Add(task);
            Utilities.clearExceptionListWithName("Child");
            Utilities.tileExceptionList.Clear();


        }


        public static void goToShippingBinShipItem(TileNode v, List<TileNode> path)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = path;
            goToShippingBinShipItem(obj);
        }



        public static void goToShippingBinShipItem(object obj)
        {
            object[] objArray = (object[])obj;
            Item I= (Item)objArray[3];

            TileNode v = (TileNode)objArray[2];
            List<TileNode> correctPath = (List<TileNode>)objArray[1];
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(goodTile);
                goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
            }
            PathFindingLogic.calculateMovement(correctPath);
            Vector2 tileLocation = v.tileLocation;
            //ModCore.CoreMonitor.Log(tileLocation.ToString());
            //if(v.thisLocation.isTerrainFeatureAt)

            //DO SOME LOGIC HERE IF I WANT TO SHIP???
            int amount = I.Stack;

            Item ok= StardustCore.Utilities.getItemFromInventory(I.Name);
            Item cool = new StardewValley.Object(I.parentSheetIndex, amount);
          //Game1.player.removeItemsFromInventory(StardewValley.Game1.player.getIndexOfInventoryItem(ok), 1);
           Game1.shippingBin.Add((StardewValley.Object)cool);

            int value= ok.Stack - amount;
            ModCore.CoreMonitor.Log("AMOUNT:" + amount);
            if (value <= 0) {
                Game1.player.items.Remove(ok);
                ok = null;
            }
            else ok.Stack = value;
           //Game1.shipObject((StardewValley.Object)I);

            ModCore.throwUpShippingMenu = true;
   

            Utilities.cleanExceptionList(v);
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
            foreach (var goodTile in correctPath)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(goodTile);
                goodTile.performRemoveAction(goodTile.tileLocation, goodTile.thisLocation);
            }

        }


    }
}
