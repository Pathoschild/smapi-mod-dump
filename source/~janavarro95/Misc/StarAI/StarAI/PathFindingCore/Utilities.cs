using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace StarAI.PathFindingCore
{
    public class Utilities
    {
        

        public static  List<TileExceptionMetaData> tileExceptionList = new List<TileExceptionMetaData>();

        public static List<TileExceptionNode> ignoreCheckTiles = new List<TileExceptionNode>();
       public static string folderForExceptionTiles="ExceptionTilesData";

        public static bool placement;


        public static Vector2 parseCenterFromTile(int tileX, int tileY)
        {
            //int x = (tileX * Game1.tileSize) + Game1.tileSize / 2;
            //int y = (tileY * Game1.tileSize) + Game1.tileSize / 2;
            int x = (tileX * Game1.tileSize);
            int y = (tileY * Game1.tileSize);
            return new Vector2(x, y);
        }

        public static void initializeTileExceptionList()
        {
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\spring_outdoorsTileSheet", 779));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\spring_outdoorsTileSheet", 780));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\spring_outdoorsTileSheet", 781));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\spring_outdoorsTileSheet", 782));

            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\summer_outdoorsTileSheet", 779));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\summer_outdoorsTileSheet", 780));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\summer_outdoorsTileSheet", 781));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\summer_outdoorsTileSheet", 782));

            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\fall_outdoorsTileSheet", 779));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\fall_outdoorsTileSheet", 780));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\fall_outdoorsTileSheet", 781));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\fall_outdoorsTileSheet", 782));

            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\winter_outdoorsTileSheet", 779));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\winter_outdoorsTileSheet", 780));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\winter_outdoorsTileSheet", 781));
            ignoreCheckTiles.Add(new TileExceptionNode("Maps\\winter_outdoorsTileSheet", 782));
        }
        

        public static void cleanExceptionList(TileNode t)
        {
            TileExceptionMetaData err= new TileExceptionMetaData(null,"");
            foreach (var v in tileExceptionList)
            {
                if (v.tile == t) err = v;
            }
            if(err.tile != null)
            {
                tileExceptionList.Remove(err);
            }
        }

        public static TileExceptionMetaData getExceptionFromTile(TileNode t)
        {
            foreach(var v in tileExceptionList)
            {
                if (t.tileLocation == v.tile.tileLocation) return v;
            }
            return null;
        }

        public static void clearExceptionListWithNames(bool removeFromWorld)
        {
            List<TileNode> removalList = new List<TileNode>();
            List<TileExceptionMetaData> removalList2 = new List<TileExceptionMetaData>();
            foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                TileExceptionMetaData exc = getExceptionFromTile((v as TileNode));
                if (exc != null)
                {
                    if (exc.actionType == "Navigation" || exc.actionType == "CostCalculation" || exc.actionType == "Child")
                    {
                        removalList.Add(exc.tile);
                        removalList2.Add(exc);
                    }
                }
                
            }

            foreach(var v in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                if (removeFromWorld) v.thisLocation.objects.Remove(v.tileLocation);

            }
            foreach(var v in removalList2)
            {
                tileExceptionList.Remove(v);
            }
        }

        public static void clearExceptionListWithName(string name)
        {
            List<TileExceptionMetaData> removalList = new List<TileExceptionMetaData>();
            foreach(var v in tileExceptionList)
            {
                //ModCore.CoreMonitor.Log("DING");
                if (v.actionType == name) removalList.Add(v);
            }
            foreach(var v in removalList)
            {
                Utilities.tileExceptionList.Remove(v);
            }

        }

        public static void clearExceptionListWithName(bool removeFromWorld,string name)
        {
            List<TileNode> removalList = new List<TileNode>();
            List<TileExceptionMetaData> removalList2 = new List<TileExceptionMetaData>();
            foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                TileExceptionMetaData exc = getExceptionFromTile((v as TileNode));
                if (exc != null)
                {
                    if (exc.actionType == name)
                    {
                        removalList.Add(exc.tile);
                        removalList2.Add(exc);
                    }
                }

            }

            foreach (var v in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                if (removeFromWorld) v.thisLocation.objects.Remove(v.tileLocation);

            }
            foreach (var v in removalList2)
            {
                tileExceptionList.Remove(v);
            }
        }
        /// <summary>
        /// Used to calculate center of a tile with varience.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="goal"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool isWithinRange(float position, float goal, int tolerance)
        {
            if (position >= goal - tolerance && position <= goal + tolerance) return true;
            return false;
        }


        public static int calculatePathCost(TileNode v, bool Placement = true)
        {
            object[] obj = new object[2];
            obj[0] = v;
            obj[1] = Placement;
            return calculatePathCost(obj);
        }

        public static int calculatePathCost(object obj)
        {
            object[] objArr = (object[])obj;
            TileNode v = (TileNode)objArr[0];
            bool placement;
            try
            {
                 placement = (bool)objArr[1];
            }
            catch(Exception err)
            {
                 placement = true;
            }
            foreach (var q in objArr)
            {
                ModCore.CoreMonitor.Log("OK THIS IS THE RESULT !: " + q, LogLevel.Alert);
            }
            if (v == null) ModCore.CoreMonitor.Log("WTF MARK!!!!!!: ", LogLevel.Alert);
            bool moveOn = false;

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
                    bool f = PathFindingCore.TileNode.checkIfICanPlaceHere(v, pos * Game1.tileSize, v.thisLocation, true,true);
                    ModCore.CoreMonitor.Log("OK THIS IS THE RESULT F: " + f, LogLevel.Alert);
                    if (f == true)
                    {

                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                        if(placement)t.placementAction(v.thisLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize);
                        else t.fakePlacementAction(v.thisLocation, (int)pos.X, (int)pos.Y);
                        //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(t, Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize));
                        miniGoals.Add(t);
                        //Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "CostCalculation"));
                    }
                }
            }
            List<TileNode> removalList = new List<TileNode>();
            foreach (var nav in miniGoals)
            {
                TileNode tempSource = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                if(placement)tempSource.placementAction(Game1.player.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
                else tempSource.fakePlacementAction(v.thisLocation, (int)v.tileLocation.X, (int)v.tileLocation.Y);
               
                List<TileNode> path = PathFindingCore.PathFindingLogic.pathFindToSingleGoalReturnPath(tempSource, nav, new List<TileNode>(),true,true);

                Utilities.clearExceptionListWithName(placement, "Child");

                ModCore.CoreMonitor.Log(tempSource.tileLocation.ToString()+tempSource.tileLocation.ToString());
                ModCore.CoreMonitor.Log(nav.tileLocation.ToString() + nav.tileLocation.ToString());

                if (path.Count != 0)
                {
                    ModCore.CoreMonitor.Log("PATH WAS NOT NULL", LogLevel.Warn);
                    paths.Add(path);
                    foreach (var someTile in path)
                    {
                        if (someTile == nav) removalList.Add(someTile);
                        StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(someTile);
                        if (placement)
                        {
                            try
                            {
                                StardewValley.Object ob = someTile.thisLocation.objects[someTile.tileLocation];
                                ModCore.CoreMonitor.Log(ob.name);
                                if (ob.name == "Twig") ModCore.CoreMonitor.Log("Culperate 2");
                                someTile.thisLocation.objects.Remove(someTile.tileLocation);
                            }
                            catch(Exception err)
                            {

                            }
                        }
                        //someTile.performRemoveAction(someTile.tileLocation, someTile.thisLocation);    
                        //StardustCore.Utilities.masterRemovalList.Add(v);
                    }
                }

            }
            foreach (var q in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(q);
                if (placement)
                {
                    try
                    {
                        StardewValley.Object ob = q.thisLocation.objects[q.tileLocation];
                        ModCore.CoreMonitor.Log(ob.name);
                        if (ob.name == "Twig") ModCore.CoreMonitor.Log("Culperate 1");
                        q.thisLocation.objects.Remove(q.tileLocation);
                    }
                    catch(Exception err)
                    {

                    }
                }
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
                //goodTile.placementAction(goodTile.thisLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize);
                //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(goodTile, Game1.currentLocation, (int)goodTile.tileLocation.X * Game1.tileSize, (int)goodTile.tileLocation.Y * Game1.tileSize));
            }
            //END HERE FOR JUST CALCULATING PATH COST
            if (paths.Count == 0) return Int32.MaxValue;
            return correctPath.Count;
        }

        /// <summary>
        /// This is used to pathfind to a single explicit target.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<TileNode> getIdealPath(TileNode target, TileNode Source=null)
        {
            object[] arr = new object[2];
            arr[0] = target;
            arr[1] = Source;
            return getIdealPath(arr);
        }

        /// <summary>
        /// This is used to pathfind to a single explicit target.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<TileNode> getIdealPath(object obj)
        {
            object[] objArr = (object[])obj;
            TileNode v = (TileNode)objArr[0];
            TileNode s;
            try
            {
                s = (TileNode)objArr[1];
                if (s == null)
                {
                    s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                    s.fakePlacementAction(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY());
                   // ModCore.CoreMonitor.Log("WHUT???????");
                }
            }
            catch(Exception err)
            {
                s = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                s.fakePlacementAction(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY());
                ModCore.CoreMonitor.Log("ICECREAM!!!!!!???????");
            }


            
            bool utility = true;

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
                    bool f = PathFindingCore.TileNode.checkIfICanPlaceHere(v, pos * Game1.tileSize, v.thisLocation, true, utility);
                    if (f == false)
                    {
                       // ModCore.CoreMonitor.Log("FAILED TO PUT DOWN A GOAL????");
                        ModCore.CoreMonitor.Log(v.thisLocation.ToString()+pos.ToString());
                    }
                    // ModCore.CoreMonitor.Log("OK THIS IS THE RESULT F: " + f, LogLevel.Alert);
                    if (f == true)
                    {

                        TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                        if (placement) t.placementAction(v.thisLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize);
                        else t.fakePlacementAction(v.thisLocation, (int)pos.X, (int)pos.Y);
                        //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode( t, Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize));
                        miniGoals.Add(t);
                        Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Navigation"));
                    }
                }
            }
            List<TileNode> removalList = new List<TileNode>();

            Utilities.clearExceptionListWithName("Child");
            Utilities.clearExceptionListWithName("Navigation");
            TileNode tempSource = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
            if (placement) tempSource.placementAction(v.thisLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
            else tempSource.fakePlacementAction(s.thisLocation, (int)s.tileLocation.X, (int)s.tileLocation.Y);

            Utilities.tileExceptionList.Add(new TileExceptionMetaData(tempSource, "Navigation"));
            //StaardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(tempSource, Game1.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize));


            //have this take in a list of goals and see which goal it reaches first
            List<TileNode> path = PathFindingCore.PathFindingLogic.pathFindToSingleGoalReturnPath(tempSource, miniGoals, new List<TileNode>(), placement, utility);
            if (path.Count == 0)
            {
                ModCore.CoreMonitor.Log("NOPE, no path I guess.", LogLevel.Warn);
            }
            else
            {
                ModCore.CoreMonitor.Log("There is a path", LogLevel.Alert);
                ModCore.CoreMonitor.Log("COST OF THE PATH IS: " + path.Count.ToString(), LogLevel.Alert);
            }
            if (path.Count != 0)
            {
                //ModCore.CoreMonitor.Log("PATH WAS NOT NULL", LogLevel.Warn);
                paths.Add(path);
                foreach (var someTile in path)
                {
                    removalList.Add(someTile);
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(someTile);
                    if (placement) someTile.thisLocation.objects.Remove(someTile.tileLocation);

                    //someTile.performRemoveAction(someTile.tileLocation, someTile.thisLocation);
                    //StardustCore.Utilities.masterRemovalList.Add(someTile);
                    //ModCore.CoreMonitor.Log("CAUGHT MY CULPERATE", LogLevel.Warn);
                }
            }
            
           // Console.WriteLine("GOALS COUNT:" + miniGoals.Count);
            foreach (var q in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(q);
                if (placement) q.thisLocation.objects.Remove(q.tileLocation);
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


            return correctPath;
        }



        /// <summary>
        /// This is used to pathfind to any target that statisfies conditions. The first one hit becomes the new goal.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<TileNode> getIdealPath(List<TileNode> t)
        {
            object[] arr = new object[1];
            arr[0] = t;
            return getAnyIdealPath(arr);
        }

        /// <summary>
        /// This is used to pathfind to any target that statisfies conditions. The first one hit becomes the new goal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<TileNode> getAnyIdealPath(object obj)
        {
            object[] objArr = (object[])obj;
          List<TileNode> vList = (List<TileNode>)objArr[0];

            bool utility = true;

            int xMin = -1;
            int yMin = -1;
            int xMax = 1;
            int yMax = 1;
            List<TileNode> miniGoals = new List<TileNode>();
            List<List<TileNode>> paths = new List<List<TileNode>>();
            //try to set children to tiles where children haven't been before
            foreach (var v in vList)
            {
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
                        bool f = PathFindingCore.TileNode.checkIfICanPlaceHere(v, pos * Game1.tileSize, v.thisLocation, true, utility);
                        // ModCore.CoreMonitor.Log("OK THIS IS THE RESULT F: " + f, LogLevel.Alert);
                        if (f == true)
                        {

                            TileNode t = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
                            if (placement) t.placementAction(v.thisLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize);
                            else t.fakePlacementAction(v.thisLocation, (int)pos.X, (int)pos.Y);
                            //StardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode( t, Game1.currentLocation, (int)pos.X * Game1.tileSize, (int)pos.Y * Game1.tileSize));
                            miniGoals.Add(t);
                            Utilities.tileExceptionList.Add(new TileExceptionMetaData(t, "Navigation"));
                        }
                    }
                }
            }
            List<TileNode> removalList = new List<TileNode>();

            Utilities.clearExceptionListWithName("Child");
            Utilities.clearExceptionListWithName("Navigation");
            TileNode tempSource = new TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.RosyBrown));
            if (placement) tempSource.placementAction(Game1.player.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
            else tempSource.fakePlacementAction(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY());

            Utilities.tileExceptionList.Add(new TileExceptionMetaData(tempSource, "Navigation"));
            //StaardustCore.Utilities.masterAdditionList.Add(new StardustCore.DataNodes.PlacementNode(tempSource, Game1.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize));


            //have this take in a list of goals and see which goal it reaches first
            List<TileNode> path = PathFindingCore.PathFindingLogic.pathFindToSingleGoalReturnPath(tempSource, miniGoals, new List<TileNode>(), placement, utility);

            ModCore.CoreMonitor.Log("OK MY PATH IS:" + path.Count);

            Utilities.clearExceptionListWithName("Child");
            Utilities.clearExceptionListWithName("Navigation");
            if (path.Count == 0)
            {
                ModCore.CoreMonitor.Log("NOPE, no path I guess.", LogLevel.Warn);
            }
            else
            {
                ModCore.CoreMonitor.Log("There is a path", LogLevel.Alert);
                ModCore.CoreMonitor.Log("COST OF THE PATH IS: " + path.Count.ToString(), LogLevel.Alert);
            }
            if (path.Count != 0)
            {
                //ModCore.CoreMonitor.Log("PATH WAS NOT NULL", LogLevel.Warn);
                paths.Add(path);
                foreach (var someTile in path)
                {
                    removalList.Add(someTile);
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(someTile);
                    if (placement) someTile.thisLocation.objects.Remove(someTile.tileLocation);

                    //someTile.performRemoveAction(someTile.tileLocation, someTile.thisLocation);
                    //StardustCore.Utilities.masterRemovalList.Add(someTile);
                    //ModCore.CoreMonitor.Log("CAUGHT MY CULPERATE", LogLevel.Warn);
                }
            }

            Console.WriteLine("GOALS COUNT:" + miniGoals.Count);
            foreach (var q in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(q);
                if (placement) q.thisLocation.objects.Remove(q.tileLocation);
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


            return correctPath;
        }


     
    
    }
}
