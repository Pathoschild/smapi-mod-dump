using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarAI;
using StardewModdingAPI;
using StardewValley;
using WindowsInput;

namespace StarAI.PathFindingCore
{
    class PathFindingLogic
    {
        public static TileNode source;
        public static List<TileNode> goals = new List<TileNode>();
        public static List<TileNode> queue = new List<TileNode>();
        public static int totalPathCost;
        public static TileNode currentGoal;
        public static int delay;

       
      //  public static int index = 0;


        public static void pathFindToAllGoals()
        {
            List<TileNode> path = new List<TileNode>();
            List<TileNode> cleanseGoals = new List<TileNode>();
            foreach (var v in goals)
            {
                Commands.pathfind("path to all goals", new string[]{
               "addStart",
               "currentPosition"
            });
                queue = new List<TileNode>();
                currentGoal = v;
                pathFindToSingleGoal(source, v, queue); //v is a goal in my goal list, queue is my queue to work with,and I always set my start to where I am at.
                List<TileNode> removalList = new List<TileNode>();
                foreach(var tile in path)
                {
                    removalList.Add(tile);
                }
                foreach (var tile in removalList)
                {
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(tile);
                    //StardustCore.Utilities.masterRemovalList.Add(v);
                     v.thisLocation.removeObject(v.tileLocation, false);
                    //v.performRemoveAction(v.tileLocation, v.thisLocation);
                }
                cleanseGoals.Add(v);
            }
            foreach(var v in cleanseGoals)
            {
                goals.Remove(v);
            }

            
        }


        public static void pathFindToSingleGoal(TileNode Source, TileNode Goal, List<TileNode> Queue)
        {
         object[] obj = new object[3];
            obj[0] = Source;
            obj[1] = Goal;
            obj[2] = Queue;
            pathFindToSingleGoal(obj);
         }

        public static void pathFindToSingleGoal(object data)
        {
            int index = 0;
            List<TileNode> path = new List<TileNode>();
            //path.Clear();
            //ModCore.CoreMonitor.Log("LET'S GO!!!!", LogLevel.Error);
            object[] obj = (object[])data;

            TileNode Source =(TileNode) obj[0];

            if (Source.parent != null)
            {
                Source.parent = null;
            }

           // ModCore.CoreMonitor.Log("PATH FROM SOURCE: "+Source.tileLocation, LogLevel.Error);
           
            TileNode Goal = (TileNode)obj[1];
           // ModCore.CoreMonitor.Log("PATH To GOAL: " + Goal.tileLocation, LogLevel.Error);
            List<TileNode> Queue = (List<TileNode>)obj[2];
            totalPathCost = 0;
            TileNode currentNode = Source;
            queue.Add(currentNode);
            index++;
            bool goalFound = false;
            while (currentNode.tileLocation != Goal.tileLocation && queue.Count != 0)
            {
               // ModCore.CoreMonitor.Log("LET'S GO SINGLE GOAL!!!!", LogLevel.Error);
                //Add children to current node
                int xMin = -1;
                int yMin = -1;
                int xMax = 1;
                int yMax = 1;

                //try to set children to tiles where children haven't been before
                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        //Include these 4 checks for just left right up down movement. Remove them to enable 8 direction path finding
                        if (x == -1 && y == -1) continue; //upper left
                        if (x == -1 && y ==  1) continue; //bottom left
                        if (x == 1 && y == -1) continue; //upper right
                        if (x == 1 && y == 1) continue; //bottom right
                                                        //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                
                        TileNode.setSingleTileAsChild(currentNode, (int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y,false);
                        Vector2 check = new Vector2((int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y);
                        if(check.X==Goal.tileLocation.X && check.Y == Goal.tileLocation.Y)
                        {
                            Goal.parent = currentNode;
                            currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                            currentNode = Goal;
                           // ModCore.CoreMonitor.Log("SNAGED THE GOAL!!!!!!");
                            //System.Threading.Thread.Sleep(2000);
                            goalFound = true;
                        }
                    }
                }
                if (goalFound == true)
                {
                    currentNode = Goal;
                    //ModCore.CoreMonitor.Log("FOUND YOU!!!");
                    //System.Threading.Thread.Sleep(2000);
                    break;
                }
                List<TileNode> adjList = new List<TileNode>();
                foreach (var node in currentNode.children) {
                    //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                    //TileNode.setSingleTileAsChild(source, (int)source.tileLocation.X + x, (int)source.tileLocation.Y + y);
                    if (node.parent == null)
                    {
                        ModCore.CoreMonitor.Log("I DONT UNDERSTAND!");
                        System.Threading.Thread.Sleep(delay);
                    }
                    //ModCore.CoreMonitor.Log("ok checking adj:" + node.tileLocation.ToString());


                    if (node.seenState == 0)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightPink); //Seen
                        adjList.Add(node);
                    }
                    if (node.seenState == 1)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue);
                    }
                    if (node.seenState == 2)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange);
                    }
                }




                foreach (var v in adjList)
                {
                    if (queue.Contains(v)) continue;
                    else queue.Add(v);
                }
                currentNode.seenState = 2;
                
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange); //Finished
                try
                {
                    currentNode = queue.ElementAt(index);
                }
                catch(Exception err)
                {
                    break;
                }
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue); //Working
                index++;
          
            }

            if (currentNode.tileLocation != Goal.tileLocation)
            {
               // ModCore.CoreMonitor.Log("NO PATH FOUND", LogLevel.Error);
                return;
            }

            if (currentNode.tileLocation == Goal.tileLocation)
            {
               // ModCore.CoreMonitor.Log("SWEET BEANS!!!!!!", LogLevel.Error);
                queue.Clear();
                index = 0;
                //ModCore.CoreMonitor.Log(currentNode.parent.ToString(), LogLevel.Error);
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                //currentGoal.drawColor=
            }

            while (currentNode.parent != null)
            {
                currentNode.drawColor= StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                path.Add(currentNode);
                if (currentNode.parent.tileLocation.X<currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Right", 0);
                }
                if (currentNode.parent.tileLocation.X > currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Left", 0);
                }
                if (currentNode.parent.tileLocation.Y < currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Down", 0);
                }
                if (currentNode.parent.tileLocation.Y > currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Up", 0);
                }
                currentNode.parent.animationManager.enableAnimation();
                currentNode = currentNode.parent;
                System.Threading.Thread.Sleep(delay);
                if (currentNode.parent == null)
                {
                    currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                    path.Add(currentNode);
                }
            }
            List<TileNode> removalList = new List<TileNode>();
            foreach(var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                if(v is TileNode)
                {
                    
                    foreach(var exc in Utilities.tileExceptionList)
                    {
                        if (exc.tile == (v as TileNode)) continue;
                    }

                    if (path.Contains(v) || goals.Contains(v)|| v.drawColor==StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red))
                    {
                        continue;
                    }
                    else
                    {
                        removalList.Add((TileNode)v);
                    }
                }
            }
            foreach(var v in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                 v.thisLocation.removeObject(v.tileLocation, false);
                //v.performRemoveAction(v.tileLocation, v.thisLocation);    
            //StardustCore.Utilities.masterRemovalList.Add(v);
            }

            calculateMovement(path);
            // goals.Remove(Goal);
            //goals.Remove(Goal);
            
        }

        /// <summary>
        /// Used to return the path to the destination without moving towards the goal.
        /// </summary>
        /// <param name="Source">The starting point to path from.</param>
        /// <param name="Goal">The goal to path to.</param>
        /// <param name="Queue">Depreciated for further builds.</param>
        /// <param name="Placement">Whether or not tiles are actually going to be placed</param>
        /// <returns></returns>
        public static List<TileNode> pathFindToSingleGoalReturnPath(TileNode Source, TileNode Goal, List<TileNode> Queue,bool Placement,bool CheckForUtility)
        {
            object[] obj = new object[5];
            obj[0] = Source;
            obj[1] = Goal;
            obj[2] = Queue;
            obj[3] = Placement;
            obj[4] = CheckForUtility;
           return pathFindToSingleGoalReturnPath(obj);
        }

        public static List<TileNode> pathFindToSingleGoalReturnPath(TileNode Source, List<TileNode> Goal, List<TileNode> Queue, bool Placement, bool CheckForUtility)
        {
            object[] obj = new object[5];
            obj[0] = Source;
            obj[1] = Goal;
            obj[2] = Queue;
            obj[3] = Placement;
            obj[4] = CheckForUtility;
            return pathFindToSingleGoalReturnPathList(obj);
        }

        public static List<TileNode> pathFindToSingleGoalReturnPath(object data)
        {
            int index = 0;
            List<TileNode> path = new List<TileNode>();
            //path.Clear();
            //ModCore.CoreMonitor.Log("LET'S GO!!!!", LogLevel.Error);
            object[] obj = (object[])data;

            TileNode Source = (TileNode)obj[0];

            if (Source.parent != null)
            {
                Source.parent = null;
            }

            //

            TileNode Goal = (TileNode)obj[1];
            //
            List<TileNode> Queue = (List<TileNode>)obj[2];
            totalPathCost = 0;
            TileNode currentNode = Source;

            bool placement = (bool)obj[3];
            bool checkForUtility = (bool)obj[4];
            queue.Add(currentNode);
            index++;
            bool goalFound = false;
            Utilities.clearExceptionListWithName("Child");
            Utilities.clearExceptionListWithName("Navigation");
            while (currentNode.tileLocation != Goal.tileLocation && queue.Count != 0)
            {
                // ModCore.CoreMonitor.Log("LET'S GO PATH!!!!", LogLevel.Error);
                //ModCore.CoreMonitor.Log("PATH FROM SOURCE: " + currentNode.tileLocation, LogLevel.Error);
                //ModCore.CoreMonitor.Log("PATH To GOAL: " + Goal.tileLocation, LogLevel.Error);
                //Console.WriteLine("OK WTF IS GOING ON????");
                //Add children to current node
                int xMin = -1;
                int yMin = -1;
                int xMax = 1;
                int yMax = 1;

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
                                                        //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                                                        //ModCore.CoreMonitor.Log("HERE1", LogLevel.Error);

                        TileNode.setSingleTileAsChild(currentNode, (int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y, checkForUtility,placement);
                        //ModCore.CoreMonitor.Log("OR NO?", LogLevel.Error);
                        Vector2 check = new Vector2((int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y);
                        if (check.X == Goal.tileLocation.X && check.Y == Goal.tileLocation.Y)
                        {
                            Goal.parent = currentNode;
                            currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                            currentNode = Goal;
                            // ModCore.CoreMonitor.Log("SNAGED THE GOAL!!!!!!");
                            //System.Threading.Thread.Sleep(2000);
                            goalFound = true;
                        }
                    }
                }
                if (goalFound == true)
                {
                    currentNode = Goal;
                    //ModCore.CoreMonitor.Log("FOUND YOU!!!");
                    //System.Threading.Thread.Sleep(2000);
                    break;
                }
                List<TileNode> adjList = new List<TileNode>();
                foreach (var node in currentNode.children)
                {
                   // ModCore.CoreMonitor.Log("MAYBE HERE",LogLevel.Warn);
                    //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                    //TileNode.setSingleTileAsChild(source, (int)source.tileLocation.X + x, (int)source.tileLocation.Y + y);
                    if (node.parent == null)
                    {
                        //ModCore.CoreMonitor.Log("I DONT UNDERSTAND!");
                        System.Threading.Thread.Sleep(delay);
                    }
                    //ModCore.CoreMonitor.Log("ok checking adj:" + node.tileLocation.ToString());


                    if (node.seenState == 0)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightPink); //Seen
                        adjList.Add(node);
                    }
                    if (node.seenState == 1)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue);
                    }
                    if (node.seenState == 2)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange);
                    }
                }




                foreach (var v in adjList)
                {
                    if (queue.Contains(v)) continue;
                    else queue.Add(v);
                }
                currentNode.seenState = 2;

                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange); //Finished
                try
                {
                    currentNode = queue.ElementAt(index);
                }
                catch (Exception err)
                {
                    break;
                }
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue); //Working
                index++;

            }

            if (currentNode.tileLocation != Goal.tileLocation)
            {
               // ModCore.CoreMonitor.Log("NO PATH FOUND", LogLevel.Error);
                return new List<TileNode>();
            }

            if (currentNode.tileLocation == Goal.tileLocation)
            {
               // ModCore.CoreMonitor.Log("SWEET BEANS!!!!!!", LogLevel.Error);
                queue.Clear();
                index = 0;
                //ModCore.CoreMonitor.Log(currentNode.parent.ToString(), LogLevel.Error);
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                //currentGoal.drawColor=
            }

            while (currentNode.parent != null)
            {
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                path.Add(currentNode);
                if (currentNode.parent.tileLocation.X < currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Right", 0);
                }
                if (currentNode.parent.tileLocation.X > currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Left", 0);
                }
                if (currentNode.parent.tileLocation.Y < currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Down", 0);
                }
                if (currentNode.parent.tileLocation.Y > currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Up", 0);
                }
                if (currentNode.parent.tileLocation == currentNode.tileLocation)
                {
                    currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Black); //Working
                }
                currentNode.parent.animationManager.enableAnimation();
                currentNode = currentNode.parent;
                System.Threading.Thread.Sleep(delay);
                if (currentNode.parent == null)
                {
                    currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                    path.Add(currentNode);
                }
            }
            List<TileNode> removalList = new List<TileNode>();
            List<TileNode> ignoreList = new List<TileNode>();
            foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                if (v is TileNode)
                {
                     removalList.Add((TileNode)v);
                }
            }
            foreach (var v in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                //v.performRemoveAction(v.tileLocation, v.thisLocation);

                try
                {
                    StardewValley.Object ob = v.thisLocation.objects[v.tileLocation];

                    //ModCore.CoreMonitor.Log(ob.name);
                    if (v.name != "Generic Colored Tile") continue;// ModCore.CoreMonitor.Log("Culperate 3");
                    if (placement) v.thisLocation.removeObject(v.tileLocation, false);
                }
                catch(Exception err)
                {

                }
                 
                
                //StardustCore.Utilities.masterRemovalList.Add(v);
            }
            return path;
            //calculateMovement(path);
            // goals.Remove(Goal);

        }


        public static KeyValuePair<bool, TileNode> doesNodeEqualGoal(TileNode currentNode, List<TileNode> goal)
        {
            foreach(var v in goal)
            {
                if (v.tileLocation == currentNode.tileLocation) return new KeyValuePair<bool, TileNode>(true, v);
            }
            return new KeyValuePair<bool, TileNode>(false, null);
        }

        public static KeyValuePair<bool,TileNode> doesNodeEqualGoal(Vector2 currentNode, List<TileNode> goal)
        {
            foreach (var v in goal)
            {
                if (v.tileLocation.X == currentNode.X && v.tileLocation.Y == currentNode.Y) return new KeyValuePair<bool, TileNode> (true,v);
            }
            return new KeyValuePair<bool, TileNode>(false,null);
        }
        public static List<TileNode> pathFindToSingleGoalReturnPathList(object data)
        {
            int index = 0;
            List<TileNode> path = new List<TileNode>();
            //path.Clear();
            //ModCore.CoreMonitor.Log("LET'S GO 2222!!!!", LogLevel.Error);
            object[] obj = (object[])data;

            TileNode Source = (TileNode)obj[0];

            if (Source.parent != null)
            {
                Source.parent = null;
            }

            //

            List<TileNode> Goals = (List<TileNode>)obj[1];
            List<TileNode> Queue = (List<TileNode>)obj[2];
            totalPathCost = 0;
            TileNode currentNode = Source;

            bool placement = (bool)obj[3];
            bool checkForUtility = (bool)obj[4];
            queue.Add(currentNode);
           // index++;
            bool goalFound = false;
            while (doesNodeEqualGoal(currentNode,Goals).Key==false && queue.Count != 0)
            {
               //  ModCore.CoreMonitor.Log("LET'S GO PATH!!!!", LogLevel.Error);
               // ModCore.CoreMonitor.Log("PATH FROM Node: " + currentNode.tileLocation, LogLevel.Error);
               // ModCore.CoreMonitor.Log("PATH FROM Source: " + Source.tileLocation, LogLevel.Error);
               // ModCore.CoreMonitor.Log("GOALS COUNT " + Goals.Count, LogLevel.Error);
             
                // ModCore.CoreMonitor.Log("THIS IS MY MISTAKE!!!!!!! " + Goals.Count, LogLevel.Error);

                //Console.WriteLine("OK WTF IS GOING ON????");
                //Add children to current node
                int xMin = -1;
                int yMin = -1;
                int xMax = 1;
                int yMax = 1;

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
                                                        //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                                                        //ModCore.CoreMonitor.Log("HERE1", LogLevel.Error);

                        TileNode.setSingleTileAsChild(currentNode, (int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y, checkForUtility, placement);
                       // ModCore.CoreMonitor.Log("OR NO?", LogLevel.Error);
                        Vector2 check = new Vector2((int)currentNode.tileLocation.X + x, (int)currentNode.tileLocation.Y + y);
                        if (doesNodeEqualGoal(check,Goals).Key==true)
                        {
                            doesNodeEqualGoal(check, Goals).Value.parent = currentNode;
                           // Goal = doesNodeEqualGoal(check, Goals).Value;
                            currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                            currentNode = doesNodeEqualGoal(check, Goals).Value;
                            // ModCore.CoreMonitor.Log("SNAGED THE GOAL!!!!!!");
                            //System.Threading.Thread.Sleep(2000);
                            goalFound = true;
                        }
                    }
                }
                if (goalFound == true)
                {
                    currentNode = doesNodeEqualGoal(currentNode, Goals).Value;
                   // ModCore.CoreMonitor.Log("FOUND YOU!!!");

                  //  path.Add(currentNode);
                    //System.Threading.Thread.Sleep(2000);
                    break;
                }
                List<TileNode> adjList = new List<TileNode>();
                foreach (var node in currentNode.children)
                {
                    // ModCore.CoreMonitor.Log("MAYBE HERE",LogLevel.Warn);
                    //TileNode t = new TileNode(1, Vector2.Zero, Souce.texturePath,source.dataPath, source.drawColor);
                    //TileNode.setSingleTileAsChild(source, (int)source.tileLocation.X + x, (int)source.tileLocation.Y + y);
                    if (node.parent == null)
                    {
                        //ModCore.CoreMonitor.Log("I DONT UNDERSTAND!");
                        System.Threading.Thread.Sleep(delay);
                    }
                    //ModCore.CoreMonitor.Log("ok checking adj:" + node.tileLocation.ToString());


                    if (node.seenState == 0)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightPink); //Seen
                       
                    }
                    if (node.seenState == 1)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue);
                    }
                    if (node.seenState == 2)
                    {
                        node.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange);
                    }
                    adjList.Add(node);
                }
               



                foreach (var v in adjList)
                {
                    if (queue.Contains(v)) continue;
                    else queue.Add(v);
                }
                currentNode.seenState = 2;

                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.DarkOrange); //Finished
                try
                {
                    currentNode = queue.ElementAt(index);
                }
                catch (Exception err)
                {
                   
                    //ModCore.CoreMonitor.Log("INDEX ERROR:"+index, LogLevel.Error);
                    break;
                }
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Blue); //Working
                index++;

            }

            if (doesNodeEqualGoal(currentNode,Goals).Key==false)
            {
               // ModCore.CoreMonitor.Log("NO PATH FOUND", LogLevel.Error);
                return new List<TileNode>();
            }

            if (doesNodeEqualGoal(currentNode, Goals).Key == true)
            {
                // ModCore.CoreMonitor.Log("SWEET BEANS!!!!!!", LogLevel.Error);
                queue.Clear();
                index = 0;
                //ModCore.CoreMonitor.Log(currentNode.parent.ToString(), LogLevel.Error);
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.LightGreen);
                //currentGoal.drawColor=
            }

            while (currentNode.parent != null)
            {
                currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                path.Add(currentNode);
                if (currentNode.parent.tileLocation.X < currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Right", 0);
                }
                if (currentNode.parent.tileLocation.X > currentNode.tileLocation.X)
                {
                    currentNode.parent.animationManager.setAnimation("Left", 0);
                }
                if (currentNode.parent.tileLocation.Y < currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Down", 0);
                }
                if (currentNode.parent.tileLocation.Y > currentNode.tileLocation.Y)
                {
                    currentNode.parent.animationManager.setAnimation("Up", 0);
                }
                if (currentNode.parent.tileLocation == currentNode.tileLocation)
                {
                    currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Black); //Working
                }
                currentNode.parent.animationManager.enableAnimation();
                currentNode = currentNode.parent;
                System.Threading.Thread.Sleep(delay);
                if (currentNode.parent == null)
                {
                    currentNode.drawColor = StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Red); //Working
                    path.Add(currentNode);
                }
               // ModCore.CoreMonitor.Log("??????");
            }
            List<TileNode> removalList = new List<TileNode>();
            List<TileNode> ignoreList = new List<TileNode>();
            foreach (var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                if (v is TileNode)
                {
                   // ModCore.CoreMonitor.Log("Removing item: " + why + " / " + StardustCore.ModCore.SerializationManager.trackedObjectList.Count);
                    removalList.Add((TileNode)v);                  
                 }
             }
            
            foreach (var v in removalList)
            {
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                //v.performRemoveAction(v.tileLocation, v.thisLocation);

                try
                {
                    StardewValley.Object ob = v.thisLocation.objects[v.tileLocation];

                    //ModCore.CoreMonitor.Log(ob.name);
                    if (v.name != "Generic Colored Tile") continue;// ModCore.CoreMonitor.Log("Culperate 3");
                    if (placement) v.thisLocation.removeObject(v.tileLocation, false);
                }
                catch (Exception err)
                {

                }


                //StardustCore.Utilities.masterRemovalList.Add(v);
            }

            if (path.Count==0&& goalFound==true)
            {
                path.Add(Source);
            }
            return path;
            //calculateMovement(path);
            // goals.Remove(Goal);

        }




        public static void calculateMovement(List<TileNode> path)
        {
            path.Reverse();

           // ModCore.CoreMonitor.Log("PATH COUNT TIME!!!!: " + path.Count);
            bool xTargetReached = false;
            bool yTargetReached = false;
            List<TileNode> removalList = new List<TileNode>();
            if (path.Count == 0) return;
            while (path.Count > 0)
            {
                TileNode w = path[0];
              
               // ModCore.CoreMonitor.Log("Here: " +Game1.player.position.ToString());
              //  ModCore.CoreMonitor.Log("LOC: " + Game1.player.currentLocation);
                Vector2 center2 = Utilities.parseCenterFromTile((int)w.tileLocation.X, (int)w.tileLocation.Y);
               // ModCore.CoreMonitor.Log("Goto: " + center2);
                //ModCore.CoreMonitor.Log("My position now: " + Game1.player.getTileLocation());
                //ModCore.CoreMonitor.Log("My Point position now: " + Game1.player.getTileLocationPoint());

                if (Game1.player.getTileX() == w.tileLocation.X && Game1.player.getTileY() == w.tileLocation.Y)
                {
                   
                        
                    removalList.Add(w);
                    
                    path.Remove(w);
                    xTargetReached = false;
                    yTargetReached = false;
                   
                    //ModCore.CoreMonitor.Log("LOOOP", LogLevel.Debug);
                    // return;
                    continue;
                }
                else
                {
                    Vector2 center = Utilities.parseCenterFromTile((int)w.tileLocation.X, (int)w.tileLocation.Y);
                  
                    while (Game1.player.position.X > center.X && xTargetReached == false)
                    {
                        if (Utilities.isWithinRange(Game1.player.position.X, center.X, 6) == true)
                        {
                            //ModCore.CoreMonitor.Log("XXXXXXXtargetReached");
                            xTargetReached = true;
                            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_A);
                            //break;
                            continue;
                        }
                        if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_A) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_A);
                    }
                   

                    while (Game1.player.position.X < center.X && xTargetReached == false)
                    {
                        //ModCore.CoreMonitor.Log("Player x: " + Game1.player.position.X);
                        //ModCore.CoreMonitor.Log("center x: " + center.X);
                        if (Utilities.isWithinRange(Game1.player.position.X, center.X, 6) == true)
                        {
                            xTargetReached = true;
                            //ModCore.CoreMonitor.Log("XXXXXXXtargetReached");
                            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_D);
                            continue;
                           }
                        if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_D) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_D);
                    }


                  

                    while (Game1.player.position.Y < center.Y && yTargetReached == false)
                    {
                        //ModCore.CoreMonitor.Log("banana");
                        if (Utilities.isWithinRange(Game1.player.position.Y, center.Y, 6) == true)
                        {
                            yTargetReached = true;
                            //ModCore.CoreMonitor.Log("YYYYYYYYYtargetReached");
                            
                            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                            continue;
                        }
                      if(InputSimulator.IsKeyDown(VirtualKeyCode.VK_S)==false)  InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_S);

                    }

                

                    while (Game1.player.position.Y > center.Y && yTargetReached == false)
                    {
                        //ModCore.CoreMonitor.Log("potato");
                        if (Utilities.isWithinRange(Game1.player.position.Y, center.Y, 6) == true)
                        {
                            yTargetReached = true;
                           // ModCore.CoreMonitor.Log("YYYYYYYYYtargetReached");
                            InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                            continue;
                        }
                        if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_W) == false) InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W);
                    }


                    if (xTargetReached == true && yTargetReached == true)
                    {
                        
                           

                        removalList.Add(w);
                        
                        path.Remove(w);
                        xTargetReached = false;
                        yTargetReached = false;
          

                        InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_A);
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_D);
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                        //return;
                        //ModCore.CoreMonitor.Log("Reached goal!", LogLevel.Error);
                        //Game1.player.position = new Vector2(center.X, center.Y);
                        continue;
                    }


                    ModCore.CoreMonitor.Log("UNCAUGHT EXCEPTION", LogLevel.Error);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_A);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_D);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                }
            }
            foreach(var v in removalList)
            {
                //v.thisLocation.objects.Remove(v.tileLocation);
                //v.thisLocation.removeObject(v.tileLocation, false);
                //v.performRemoveAction(v.tileLocation, v.thisLocation);
                //StardustCore.Utilities.masterRemovalList.Add(v);
                v.thisLocation.objects.Remove(v.tileLocation);
                //ModCore.CoreMonitor.Log("WHUT???"+v.tileLocation);
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                //var ok = v;
                //ok = null;
            }
            //goals.Clear();
        }


    }
}
