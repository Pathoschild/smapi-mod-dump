using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starbot
{
    public static class Core
    {
        public static bool WantsToStop = false;

        private static bool MovingDown = false;
        private static bool MovingLeft = false;
        private static bool MovingRight = false;
        private static bool MovingUp = false;

        public static bool ShouldBeMoving { get { return MovingDown || MovingLeft || MovingRight || MovingUp; } }
        private static bool IsStuck = false;
        private static int LastTileX = -3, LastTileY = -3;
        private static string LastLocationName = null;
        private static int UpdatesSinceCoordinateChange = 0;
        private static int LastGameDay = -3;

        public static bool IsSleeping = false;

        //Objectives
        private static bool IsBored = true;
        private static Objective Objective = null;
        private static List<string> ObjectivesCompletedToday = new List<string>();
        public static List<Objective> ObjectivePool = new List<Objective>();

        private static void FindNewObjective()
        {
            if(ObjectivePool.Count == 0)
            {
                //sleep time
                Mod.instance.Monitor.Log("Bot has no remaining objectives for today. Time for bed!", LogLevel.Alert);
                Objective = new Objectives.ObjectiveSleep();
                Mod.instance.Monitor.Log("New objective: " + Objective.AnnounceMessage, LogLevel.Info);
            } else
            {
                int randomObjective = Mod.RNG.Next(ObjectivePool.Count);
                Objective = ObjectivePool[randomObjective];
                ObjectivePool.RemoveAt(randomObjective);
                Mod.instance.Monitor.Log("New objective: " + Objective.AnnounceMessage, LogLevel.Info);
                if (Game1.IsMultiplayer && !Objective.Cooperative)
                {
                    Mod.instance.Helper.Multiplayer.SendMessage<string>(Objective.UniquePoolId, "taskAssigned");
                }
            }
        }

        private static void ResetObjectivePool()
        {
            ObjectivePool.Clear();
            ObjectivePool.Add(new Objectives.ObjectiveForage("BusStop"));
            ObjectivePool.Add(new Objectives.ObjectiveForage("Beach"));
            ObjectivePool.Add(new Objectives.ObjectiveForage("Forest"));
            ObjectivePool.Add(new Objectives.ObjectiveForage("Backwoods"));
            ObjectivePool.Add(new Objectives.ObjectiveForage("Mountain"));
            ObjectivePool.Add(new Objectives.ObjectiveForage("Town"));
            ObjectivePool.Add(new Objectives.ObjectiveClearDebris("Farm"));
        }

        public static void FailObjective()
        {
            if(Objective != null)
            {
                Mod.instance.Monitor.Log("Objective failed: " + Objective.AnnounceMessage, LogLevel.Info);
                Objective.Fail();
                if (Objective.FailureCount < 3) ObjectivePool.Add(Objective);
                else
                {
                    Mod.instance.Monitor.Log("Skipping objective for today (too many failures): " + Objective.AnnounceMessage, LogLevel.Info);
                }
                Objective = null;
                IsRouting = false;
                IsPathfinding = false;
                StopMovingDown();
                StopMovingLeft();
                StopMovingRight();
                StopMovingUp();
            }
        }

        //Routing
        public static bool IsRouting = false;
        private static bool IsCriticalRoute = false;
        private static int RoutingDestinationX = -3, RoutingDestinationY = -3;
        private static bool HasRoutingDestination { get { return RoutingDestinationX != -3 && RoutingDestinationY != -3; } }
        private static List<string> Route = null;

        public static bool RouteTo(string targetMap, int targetX = -3, int targetY = -3, bool critical = false, int localCutoff = -1)
        {
            if (Game1.player.currentLocation.NameOrUniqueName != targetMap)
            {
                //Mod.instance.Monitor.Log("Routing to: " + targetMap + (targetY == -1 ? targetX + ", " + targetY : ""), LogLevel.Trace);
                //calculate a route to the destination
                var route = Routing.GetRoute(targetMap);
                if (route == null || route.Count < 2)
                {
                    if (critical)
                    {
                        Mod.instance.Monitor.Log("Routing failed: no route!", LogLevel.Warn);
                        FailObjective();
                    }
                    return false;
                } else
                {
                    //debug, print route:
                    //string routeInfo = "Route: ";
                    //foreach (string s in route) routeInfo += s + ", ";
                    //Mod.instance.Monitor.Log(routeInfo.Substring(0, routeInfo.Length - 2), LogLevel.Trace);
                }

                //set the bot's route
                IsRouting = true;
                IsCriticalRoute = critical;
                RoutingDestinationX = targetX;
                RoutingDestinationY = targetY;
                Route = route;
                AdvanceRoute();
                return true;
            } else if(targetX != -3 && targetY != -3)
            {
                RoutingDestinationX = targetX;
                RoutingDestinationY = targetY;
                return PathfindTo(targetX, targetY, critical, false, localCutoff);
            }
            return false;
        }

        private static void ClearRoutingDestination()
        {
            RoutingDestinationX = -3;
            RoutingDestinationY = -3;
        }

        //call on location change
        private static void AdvanceRoute()
        {
            if (!IsRouting) return;
            //Mod.instance.Monitor.Log("Advancing route...", LogLevel.Trace);
            Route.RemoveAt(0); //remove the current map from the list
            if (Route.Count == 0)
            {
                //route complete
                IsRouting = false;
                Route = null;
                if(HasRoutingDestination)
                {
                    //pathfind to final destination coordinates
                    PathfindTo(RoutingDestinationX, RoutingDestinationY, IsCriticalRoute);
                }
            } else
            {
                //pathfind to the next map
                foreach (var w in Game1.player.currentLocation.warps)
                {
                    if (w.TargetName == Route[0])
                    {
                        PathfindTo(w.X, w.Y, IsCriticalRoute);
                        return;
                    }
                }
                foreach (var w in Game1.player.currentLocation.doors.Keys)
                {
                    if (Game1.player.currentLocation.doors[w] == Route[0])
                    {
                        PathfindTo(w.X, w.Y + 1, IsCriticalRoute, true);
                        return;
                    }
                }
                if (Game1.player.currentLocation is StardewValley.Locations.BuildableGameLocation)
                {
                    StardewValley.Locations.BuildableGameLocation bl = Game1.player.currentLocation as StardewValley.Locations.BuildableGameLocation;
                    foreach (var b in bl.buildings)
                    {
                        if(b.indoors.Value.NameOrUniqueName == Route[0])
                        {
                            PathfindTo(b.getPointForHumanDoor().X, b.getPointForHumanDoor().Y + 1, IsCriticalRoute, true);
                            return;
                        }
                    }
                }

            }
        }

        //Pathfinding
        private static bool IsPathfinding = false;
        private static int PathfindingDestinationX = -3, PathfindingDestinationY = -3;
        private static bool PathfindingOpenDoor = false;
        private static List<Tuple<int,int>> Path = null;

        private static bool PathfindTo(int x, int y, bool critical = false, bool openDoor = false, int cutoff = -1)
        {
            //Mod.instance.Monitor.Log("Pathfinding to: " + x + ", " + y, LogLevel.Trace);
            var path = Pathfinder.Pathfinder.FindPath(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY(), x, y, cutoff);
            if (path == null)
            {
                if (critical)
                {
                    Mod.instance.Monitor.Log("Pathfinding failed: no path!", LogLevel.Alert);
                    FailObjective();
                }
                return false;
            }

            //set the bot's path
            IsPathfinding = true;
            PathfindingDestinationX = x;
            PathfindingDestinationY = y;
            PathfindingOpenDoor = openDoor;
            Path = path;
            return true;
        }

        private static void ClearPathfindingDestination()
        {
            PathfindingDestinationX = -3;
            PathfindingDestinationY = -3;
        }

        private static void AdvancePath()
        {
            if (!IsPathfinding) return;
            if (Path.Count == 0)
            {
                Path = null;
                IsPathfinding = false;
                //ClearMoveTarget();
                ClearPathfindingDestination();
                //Mod.instance.Monitor.Log("Pathfinding complete.", LogLevel.Alert);
                if (PathfindingOpenDoor)
                {
                    DoOpenDoor();
                }
                return;
            }
            var next = Path[0];
            Path.RemoveAt(0);
            MoveTo(next.Item1, next.Item2);
        }

        //Movement
        private static bool HasMoveTarget { get { return MoveTargetX != -3 && MoveTargetY != -3; } }
        private static int MoveTargetX = -3;
        private static int MoveTargetY = -3;

        public static void MoveTo(int x, int y)
        {
            MoveTargetX = x;
            MoveTargetY = y;
        }

        private static void ClearMoveTarget()
        {
            MoveTargetX = -3;
            MoveTargetY = -3;
        }

        private static int StopX = 0; //a delay on releasing keys so the animation isnt janky from spamming the button on and off
        private static int StopY = 0;
        private static readonly int StopDelay = 16;
        private static void AdvanceMove()
        {
            float px = Game1.player.Position.X;
            float py = Game1.player.Position.Y;

            float tx = ((float)MoveTargetX * Game1.tileSize);// + (Game1.tileSize / 2);
            float ty = ((float)MoveTargetY * Game1.tileSize) + (Game1.tileSize / 3);

            float epsilon = Game1.tileSize * 0.1f;

            if (tx - px > epsilon) StartMovingRight();
            else if (px - tx > epsilon) StartMovingLeft();
            else
            {
                if (StopX > StopDelay)
                {
                    StopX = 0;
                    StopMovingRight();
                    StopMovingLeft();
                }
                else StopX++;
            }

            if (ty - py > epsilon) StartMovingDown();
            else if (py - ty > epsilon) StartMovingUp();
            else
            {
                if (StopY > StopDelay)
                {
                    StopY = 0;
                    StopMovingDown();
                    StopMovingUp();
                }
                else StopY++;
            }

            if(Math.Abs(px - tx) < epsilon && Math.Abs(py - ty) < epsilon)
            {
                ClearMoveTarget();
            }
        }

        public static void Reset()
        {
            ReleaseKeys();
            Routing.Reset();
            ClearMoveTarget();
            ClearPathfindingDestination();
            IsPathfinding = false;
            PathfindingOpenDoor = false;
            Path = null;
            IsRouting = false;
            ClearRoutingDestination();
            Route = null;
            LastTileX = -3;
            LastTileY = -3;
            LastLocationName = null;

            IsStuck = false;
            UpdatesSinceCoordinateChange = 0;
            WantsToStop = false;

            MovingDown = false;
            MovingLeft = false;
            MovingRight = false;
            MovingUp = false;

            Objective = null;
            ObjectivesCompletedToday.Clear();
            ObjectivePool.Clear();
            IsBored = false;
            LastGameDay = -3;
        }

        public static void Update()
        {
            if (!Routing.Ready) return;

            Mod.Input.Update();

            //cutscenes break it anyway
            if (Game1.eventUp)
            {
                WantsToStop = true;
            }

            //are we waiting on action button
            if (ActionButtonTimer > 0)
            {
                ActionButtonTimer--;
                StopMovingDown();
                StopMovingLeft();
                StopMovingRight();
                StopMovingUp();
                if (ActionButtonTimer == 0)
                {
                    StopActionButton();
                }
                return;
            }

            //are we waiting on a tool swing
            if (SwingToolTimer > 0)
            {
                SwingToolTimer--;
                StopMovingDown();
                StopMovingLeft();
                StopMovingRight();
                StopMovingUp();
                if (SwingToolTimer == 0)
                {
                    StopUseTool();
                }
                return;
            }

            //only update navigation while navigation is possible
            if (Context.CanPlayerMove)
            {
                //new day
                if (LastGameDay != Game1.dayOfMonth)
                {
                    //new day
                    IsSleeping = false;
                    ObjectivesCompletedToday.Clear();
                    Objective = null;
                    ResetObjectivePool();
                    LastGameDay = Game1.dayOfMonth;
                }

                //shh don't wake the bot
                if (IsSleeping) return;

                //cache player position
                int px = Game1.player.getTileX();
                int py = Game1.player.getTileY();

                //for now, if stuck let's just shut it down
                if (IsStuck)
                {
                    WantsToStop = true;
                    return;
                }

                //on logical location change
                if(Game1.currentLocation.NameOrUniqueName != LastLocationName)
                {
                    if (OpeningDoor)
                    {
                        StopActionButton();
                        OpeningDoor = false;
                    }
                    LastLocationName = Game1.currentLocation.NameOrUniqueName;
                    ClearMoveTarget();
                    ReleaseKeys();
                    if(IsRouting && Route[0] == Game1.currentLocation.NameOrUniqueName) AdvanceRoute();
                }

                if (OpeningDoor)
                {
                    return; //let's not interfere
                }

                //if we don't have a move target, check the path for one
                if (!HasMoveTarget && IsPathfinding)
                {
                    //is pathfinding complete?
                    if(px == PathfindingDestinationX && py == PathfindingDestinationY)
                    {
                        //move to the next node in the path
                        AdvancePath();

                        //check for route destination and announce
                        if (HasRoutingDestination && px == RoutingDestinationX && py == RoutingDestinationY)
                        {
                            //Mod.instance.Monitor.Log("Routing complete.", LogLevel.Alert);
                            ClearMoveTarget();
                            ClearPathfindingDestination();
                            ClearRoutingDestination();
                            ReleaseKeys();
                        }
                    } else
                    {
                        //move to the next node in the path
                        AdvancePath();
                    }
                }

                if (HasMoveTarget)
                {
                    AdvanceMove();
                }


                //stuck detection
                if (ShouldBeMoving)
                {
                    //on logical tile change
                    if (px != LastTileX || py != LastTileY)
                    {
                        LastTileX = px;
                        LastTileY = py;
                        UpdatesSinceCoordinateChange = 0;
                    }
                    UpdatesSinceCoordinateChange++;
                    if (!IsStuck && UpdatesSinceCoordinateChange > 60 * 5) OnStuck();
                }



                //bored?
                if(!HasMoveTarget && !IsPathfinding && !IsRouting)
                {
                    IsBored = true;
                }

                if (IsBored)
                {
                    if(Objective != null)
                    {
                        if (Objective.IsComplete)
                        {
                            string objName = Objective.GetType().Name;
                            Mod.instance.Monitor.Log("Objective completed: " + Objective.AnnounceMessage, LogLevel.Info);
                            ObjectivesCompletedToday.Add(objName);
                            Objective = null;
                        } else
                        {
                            IsBored = false;
                            Objective.Step();
                        }
                    } else
                    {
                        FindNewObjective();
                    }
                }
            } else
            {
                if(Objective != null && !Objective.IsComplete)
                {
                    Objective.CantMoveUpdate();
                }
            }
        }

        public static bool EquipToolIfOnHotbar(string name)
        {
            var t = Game1.player.getToolFromName(name);
            if (t == null)
            {
                Mod.instance.Monitor.Log("Could not equip tool: " + name + " (not found in inventory)", LogLevel.Info);
                return false;
            }
            for (int index = 0; index < 12; ++index)
            {
                if (Game1.player.items.Count > index && Game1.player.items.ElementAt<Item>(index) != null)
                {
                    if(Game1.player.items[index] == t)
                    {
                        //found it
                        if(Game1.player.CurrentToolIndex != index)
                        {
                            Mod.instance.Monitor.Log("Equipping tool: " + name, LogLevel.Info);
                            Game1.player.CurrentToolIndex = index;
                        }
                        return true;
                    }
                }
            }
            Mod.instance.Monitor.Log("Could not equip tool: " + name + " (not found on hotbar)", LogLevel.Info);
            return false;
        }

        public static bool OpeningDoor = false;
        private static void DoOpenDoor()
        {
            OpeningDoor = true;
            //StartMovingUp();
            FaceTile(Game1.player.getTileX(), Game1.player.getTileY() - 1);
            StartActionButton();
        }

        private static void OnStuck()
        {
            IsStuck = true;
            Mod.instance.Monitor.Log("Bot is stuck.", LogLevel.Info);
            UpdatesSinceCoordinateChange = 0;
            ReleaseKeys();
            if(Objective != null)
            {
                FailObjective();
                IsStuck = false;
            }
        }

        private static readonly int ActionButtonTime = 30;
        private static int ActionButtonTimer = 0;
        public static void DoActionButton(bool stop = true)
        {
            if (stop) { 
                StopMovingDown();
                StopMovingLeft();
                StopMovingRight();
                StopMovingUp();
            }
            StartActionButton();
            ActionButtonTimer = ActionButtonTime;
        }

        private static void StartActionButton()
        {
            Mod.Input.StartActionButton();
        }

        private static void StopActionButton()
        {
            Mod.Input.StopActionButton();
        }

        private static readonly int SwingToolTime = 30;
        private static int SwingToolTimer = 0;
        public static void SwingTool()
        {
            StopMovingDown();
            StopMovingLeft();
            StopMovingRight();
            StopMovingUp();
            StartUseTool();
            SwingToolTimer = SwingToolTime;
        }

        private static void StartUseTool()
        {
            Mod.Input.StartUseTool();
        }

        private static void StopUseTool()
        {
            Mod.Input.StopUseTool();
        }

        public static void FaceTile(int x, int y)
        {
            var v = new Vector2(((float)x * Game1.tileSize) + (Game1.tileSize / 2f), ((float)y * Game1.tileSize) + (Game1.tileSize / 2f));
            Game1.player.faceGeneralDirection(v);
            Game1.setMousePosition(Utility.Vector2ToPoint(Game1.GlobalToLocal(v)));
        }

        private static void StartMovingDown()
        {
            if (MovingUp) StopMovingUp();
            Mod.Input.StartMoveDown();
            MovingDown = true;
        }

        private static void StopMovingDown()
        {
            Mod.Input.StopMoveDown();
            MovingDown = false;
        }

        private static void StartMovingLeft()
        {
            if (MovingRight) StopMovingRight();
            Mod.Input.StartMoveLeft();
            MovingLeft = true;
        }

        private static void StopMovingLeft()
        {
            Mod.Input.StopMoveLeft();
            MovingLeft = false;
        }

        private static void StartMovingUp()
        {
            if (MovingDown) StopMovingDown();
            Mod.Input.StartMoveUp();
            MovingUp = true;
        }

        private static void StopMovingUp()
        {
            Mod.Input.StopMoveUp();
            MovingUp = false;
        }

        private static void StartMovingRight()
        {
            if (MovingLeft) StopMovingLeft();
            Mod.Input.StartMoveRight();
            MovingRight = true;
        }

        private static void StopMovingRight()
        {
            Mod.Input.StopMoveRight();
            MovingRight = false;
        }

        public static void ReleaseKeys()
        {
            StopMovingDown();
            StopMovingLeft();
            StopMovingRight();
            StopMovingUp();
        }

        public static void AnswerGameLocationDialogue(int selection)
        {
            if(Game1.activeClickableMenu is StardewValley.Menus.DialogueBox)
            {
                var db = Game1.activeClickableMenu as StardewValley.Menus.DialogueBox;
                var responses = (List < Response > )typeof(StardewValley.Menus.DialogueBox).GetField("responses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                Mod.instance.Monitor.Log("Responding to dialogue with selection: " + responses[selection].responseKey);
                Game1.currentLocation.answerDialogue(responses[selection]);
                Game1.dialogueUp = false;
                if (!Game1.IsMultiplayer)
                {
                    Game1.activeClickableMenu = null;
                    Game1.playSound("dialogueCharacterClose");
                    Game1.currentDialogueCharacterIndex = 0;
                }
            }
        }
    }
}
