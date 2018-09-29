using System;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Harmony;
using Microsoft.Xna.Framework;
using System.Reflection;
using StardewValley.Objects;
using StardewValley.Tools;

namespace RightClickMoveMode
{
    public class ModConfig
    {
        public String RightClickMoveModeDefault { get; set; } = "On";
        public String WeaponsSpecticalInteraction { get; set; } = "Enable";
        public String WeaponsSpecticalInteractionType { get; set; } = "Disable";
        public String HoldingMoveOnly { get; set; } = "Disable";
        public String ExtendedModeDefault { get; set; } = "On";
        public String RightClickMoveModeOpenButton { get; set; } = "G";
        public String ExtendedModeOpenButton { get; set; } = "H";
    }

    public class ModEntry : Mod
    {
        private ModConfig config;

        public const float hitboxRadius = 64f * 2;

        public static bool isRightClickMoveModeOn = true;
        public static bool isExtendedModeOn = true;
        public static bool isWeaponsSpecticalInteraction = true;
        public static bool isHoldingMoveOnly = true;
        
        public static bool isMovingAutomaticaly = false;  
        public static bool isBeingAutoCommand = false; 
        public static bool isMouseOutsiteHitBox = false;

        public static bool isBeingControl = false;

        public static bool isHoldingMove = false;

        public static bool isHoldingLeftCtrl = false;
        public static bool isHoldingRightCtrl = false;
        public static bool isHoldingRightAlt = false;
        public static bool isWheeling = false;

        public static bool isDone = true;

        public static StardewValley.Object pointedObject = null;
        public static StardewValley.Object pointedObjectDebug = null;

        private String RightClickMoveModeOpenButton;

        private String ExtendedModeOpenButton;

        private static Vector2 vector_PlayerToDestination;  
        private static Vector2 vector_PlayerToMouse;
        private static Vector2 vector_AutoMove; 

        private static Vector2 position_MouseOnScreen;
        private static Vector2 position_Source;
        private static Vector2 position_Destination;

        private static Vector2 grabTile;

        private static int tickCount = 15;
        private static int stuckCount = 30;

        private static int currentToolIndex = 1;
        
        public static bool isDebugMode = false;

        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            InputEvents.ButtonReleased += this.InputEvents_ButtonReleased;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            PlayerEvents.Warped += this.PlayerEvents_Warped;

            StartPatching();

            this.config = this.Helper.ReadConfig<ModConfig>();

            RightClickMoveModeOpenButton = this.config.RightClickMoveModeOpenButton.ToUpper();
            ExtendedModeOpenButton = this.config.ExtendedModeOpenButton.ToUpper();

            isRightClickMoveModeOn = this.config.RightClickMoveModeDefault.ToUpper() == "ON";
            isExtendedModeOn = this.config.ExtendedModeDefault.ToUpper() == "ON";
            isWeaponsSpecticalInteraction = !(this.config.WeaponsSpecticalInteraction.ToUpper() == "DISABLE");
            isHoldingMoveOnly = this.config.HoldingMoveOnly.ToUpper() == "ENABLE";

            position_MouseOnScreen = new Vector2(0f, 0f);
            position_Source = new Vector2(0f, 0f);
            vector_PlayerToDestination = new Vector2(0f, 0f);
        }
        


        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            bool flag = Context.IsWorldReady;
            if (flag)
            {
                if (isExtendedModeOn)
                {
                    if ((isHoldingLeftCtrl || isHoldingRightCtrl) && isWheeling)
                    {
                        Game1.player.CurrentToolIndex = currentToolIndex;
                    }
                }

                if (isRightClickMoveModeOn)
                {
                    if (Context.IsPlayerFree)
                    {
                        if (Game1.player.ActiveObject != null)
                        {
                            if (isMovingAutomaticaly && (Game1.player.ActiveObject is Furniture))
                            {
                                isMovingAutomaticaly = false;
                                Game1.player.Halt();
                            }
                        }
                        if (isHoldingMove)
                        {
                            isMovingAutomaticaly = true;

                            grabTile = Game1.currentCursorTile;
                            if (Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y) != null)
                                pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
                            else
                                pointedObject = null;

                            if (isBeingControl)
                            {
                                if (tickCount == 0)
                                {
                                    isBeingControl = false;
                                    tickCount = 15;
                                }
                                else
                                    tickCount--;
                            }
                        }
                        else
                        {
                            if (isHoldingMoveOnly)
                            {
                                isMovingAutomaticaly = false;
                            }
                            else
                            {
                                vector_PlayerToDestination.X = position_Destination.X - Game1.player.Position.X - 32f;
                                vector_PlayerToDestination.Y = position_Destination.Y - Game1.player.Position.Y - 10f;
                            }
                        }

                        vector_PlayerToMouse.X = position_MouseOnScreen.X + Game1.viewport.X - Game1.player.Position.X - 32f;
                        vector_PlayerToMouse.Y = position_MouseOnScreen.Y + Game1.viewport.Y - Game1.player.Position.Y - 10f;
                        
                    }

                    if (pointedObjectDebug != pointedObject)
                    {
                        pointedObjectDebug = pointedObject;
                        if (pointedObject  == null)
                            base.Monitor.Log(String.Format("pointedObject = null"));
                        else
                            base.Monitor.Log(String.Format("pointedObject = {0}", pointedObject.DisplayName));
                    }
                }
            }
        }


        private void PlayerEvents_Warped(object sender, EventArgsPlayerWarped e)
        {
            isMovingAutomaticaly = false;
        }

        private int SpecialCooldown(MeleeWeapon currentWeapon)
        {
            if (currentWeapon.type == 3)
            {
                return MeleeWeapon.defenseCooldown;
            }
            if (currentWeapon.type == 1)
            {
                return MeleeWeapon.daggerCooldown;
            }
            if (currentWeapon.type == 2)
            {
                return MeleeWeapon.clubCooldown;
            }
            if (currentWeapon.type == 0)
            {
                return MeleeWeapon.attackSwordCooldown;
            }
            return 0;
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            bool flag = Context.IsWorldReady;
            string button = e.Button.ToString();
            
            if (button == RightClickMoveModeOpenButton)
            {
                isRightClickMoveModeOn = !isRightClickMoveModeOn;
            }
            if (button == ExtendedModeOpenButton)
            {
                isExtendedModeOn = !isExtendedModeOn;
            }

            if (isExtendedModeOn)
            {
                if (button == "RightControl")
                {
                    isHoldingRightCtrl = true;
                }
                if (button == "LeftControl")
                {
                    isHoldingLeftCtrl = true;
                }
                if (button == "RightAlt")
                {
                    isHoldingRightAlt = true;
                }

                if (button == "Enter" && isHoldingRightAlt)
                {
                    if (Game1.options.isCurrentlyWindowedBorderless() || Game1.options.isCurrentlyFullscreen())
                        Game1.options.setWindowedOption("Windowed");
                    else
                    {
                        Game1.options.setWindowedOption("Windowed Borderless");
                    }
                    Game1.exitActiveMenu();
                }
            }

            if (flag)
            {
                bool flag2 = button == "MouseRight" && isRightClickMoveModeOn && Context.IsPlayerFree;

                if (Game1.player.ActiveObject != null)
                {
                    if (Game1.player.ActiveObject.getCategoryName() == "Furniture")
                    {
                        flag2 = false;
                    }
                }

                if (!isWeaponsSpecticalInteraction)
                {
                    if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && SpecialCooldown((MeleeWeapon)Game1.player.CurrentTool) <= 0)
                        flag2 = false;
                }

                if (flag2)
                {
                    ModEntry.isMovingAutomaticaly = true;
                    isHoldingMove = true;
                    isBeingControl = false;
                    isMouseOutsiteHitBox = vector_PlayerToMouse.Length().CompareTo(hitboxRadius) > 0;

                    grabTile = new Vector2((float)(position_MouseOnScreen.X + Game1.viewport.X), (float)(position_MouseOnScreen.Y + Game1.viewport.Y)) / 64f;

                    if (Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y) != null)
                        pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
                    else
                        pointedObject = null;

                    bool flag3 = false;
                    flag3 = flag3 || isMouseOutsiteHitBox;
                    
                    if (flag3)
                    {
                        e.SuppressButton();
                    }
                }
                else
                {
                    if (e.IsUseToolButton)
                    {
                        tickCount = 15;
                    }
                    else
                        tickCount = 0;
                    isBeingControl = true;
                }
            }
        }

        private void ControlEvents_MouseChanged(object sender, StardewModdingAPI.Events.EventArgsMouseStateChanged e)
        {
            bool flag = Context.IsWorldReady;

            if (flag)
            {
                if (isRightClickMoveModeOn)
                {
                    if (Context.IsPlayerFree)
                    {
                        position_MouseOnScreen.X = (float)e.NewPosition.X;
                        position_MouseOnScreen.Y = (float)e.NewPosition.Y;
                    }
                }

                if (isExtendedModeOn)
                {
                    if (e.PriorState.ScrollWheelValue != e.NewState.ScrollWheelValue)
                        isWheeling = true;
                    else
                        isWheeling = false;

                    if (isHoldingLeftCtrl || isHoldingRightCtrl)
                    {
                        if (e.PriorState.ScrollWheelValue < e.NewState.ScrollWheelValue)
                        {
                            currentToolIndex = Game1.player.CurrentToolIndex;
                            if (Game1.options.zoomLevel <= Options.maxZoom)
                                Game1.options.zoomLevel += 0.05f;
                            Game1.exitActiveMenu();
                        }
                        else if (e.PriorState.ScrollWheelValue > e.NewState.ScrollWheelValue)
                        {
                            currentToolIndex = Game1.player.CurrentToolIndex;
                            if (Game1.options.zoomLevel >= Options.minZoom)
                                Game1.options.zoomLevel -= 0.05f;
                            Game1.exitActiveMenu();
                        }
                    }
                }
            }
        }

        private void InputEvents_ButtonReleased(object sender, EventArgsInput e)
        {
            bool flag = Context.IsWorldReady;

            string button = e.Button.ToString();

            if (isExtendedModeOn)
            {
                if (button == "RightControl")
                {
                    isHoldingRightCtrl = false;
                }

                if (button == "LeftControl")
                {
                    isHoldingLeftCtrl = false;
                }

                if (button == "RightAlt")
                {
                    isHoldingRightAlt = false;
                }
            }

            if (flag)
            {
                if (isRightClickMoveModeOn)
                {
                    bool flag2 = button == "MouseRight" && isHoldingMove;

                    if (flag2)
                    {
                        isHoldingMove = false;

                        position_Destination.X = (float)e.Cursor.ScreenPixels.X + Game1.viewport.X;
                        position_Destination.Y = (float)e.Cursor.ScreenPixels.Y + Game1.viewport.Y;

                        position_Source.X = Game1.player.Position.X + 32f;
                        position_Source.Y = Game1.player.Position.Y + 10f;

                        vector_PlayerToDestination.X = position_Destination.X - Game1.player.Position.X - 32f;
                        vector_PlayerToDestination.Y = position_Destination.Y - Game1.player.Position.Y - 10f;

                        grabTile = new Vector2((float)(position_MouseOnScreen.X + Game1.viewport.X), (float)(position_MouseOnScreen.Y + Game1.viewport.Y)) / 64f;

                        if (Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y) != null)
                            pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
                        else
                            pointedObject = null;

                        if (isMouseOutsiteHitBox)
                        {
                            isDone = false;
                        }
                    }
                }
            }
        }
        
        public static void MoveVectorToCommand()
        {
            bool flag = ModEntry.isMovingAutomaticaly;
            bool flag2 = false;
            bool flag3 = false;

            if (flag)
            {

                if (isHoldingMove)
                {
                    vector_AutoMove.X = vector_PlayerToMouse.X;
                    vector_AutoMove.Y = vector_PlayerToMouse.Y;
                }
                else
                {
                    vector_AutoMove.X = vector_PlayerToDestination.X;
                    vector_AutoMove.Y = vector_PlayerToDestination.Y;
                }


                if (Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    if (!isDone && !(Game1.player.ActiveObject == null) && !(Game1.player.ActiveObject is Furniture) && Game1.player.ActiveObject.isPlaceable())
                    {
                        isDone = true;
                        int stack = Game1.player.ActiveObject.Stack;
                        Utility.tryToPlaceItem(Game1.currentLocation, Game1.player.ActiveObject, (int)grabTile.X * 64 + 32, (int)grabTile.Y * 64 + 32);
                        if (Game1.player.ActiveObject == null || stack < Game1.player.ActiveObject.Stack)
                        {
                            ModEntry.isMovingAutomaticaly = false;
                            if (isHoldingMove)
                            {
                                isBeingControl = true;
                                tickCount = 15;
                            }
                        }
                    }
                    if (!isDone)
                    {
                        isDone = Game1.tryToCheckAt(grabTile, Game1.player);
                        if (!isDone)
                        {
                            grabTile.Y += 1f;
                            isDone = Game1.tryToCheckAt(grabTile, Game1.player);
                        }
                    }
                }

                Game1.player.movementDirections.Clear();
                if (vector_AutoMove.X <= 5 && vector_AutoMove.X >= -5)
                {
                    vector_AutoMove.X = 0;
                    flag2 = true;
                }
                else if (vector_AutoMove.X >= 5)
                    Game1.player.SetMovingRight(true);
                else if (vector_AutoMove.X <= -5)
                    Game1.player.SetMovingLeft(true);

                if (vector_AutoMove.Y <= 5 && vector_AutoMove.Y >= -5)
                {
                    vector_AutoMove.Y = 0;
                    flag3 = true;
                }
                else if (vector_AutoMove.Y >= 5)
                    Game1.player.SetMovingDown(true);
                else if (vector_AutoMove.Y <= -5)
                    Game1.player.SetMovingUp(true);

                vector_AutoMove.Normalize();

                //if (pointedObject != null && !isDone)
                //{
                //    if (pointedObject.isActionable(Game1.player))
                //    {
                //        isDone = true;
                //        pointedObject.checkForAction(Game1.player);
                //        //public static bool canGrabSomethingFromHere(int x, int y, Farmer who)
                //    }
                //}
                if (flag2 && flag3)
                {
                    if (!isDone)
                    {
                        Game1.tryToCheckAt(Game1.player.getTileLocation(), Game1.player);
                    }
                    ModEntry.isMovingAutomaticaly = false;
                }
            }
        }

        public static void StartPatching()
        {
            HarmonyInstance newHarmony = HarmonyInstance.Create("ylsama.RightClickMoveMode");

            MethodInfo farmeHaltrInfo = AccessTools.Method(typeof(Farmer), "Halt");

            MethodInfo farmerMovePositionInfo = AccessTools.Method(typeof(Farmer), "MovePosition", new Type[] { typeof(GameTime), typeof(xTile.Dimensions.Rectangle), typeof(GameLocation) });

            MethodInfo farmerHaltPrefix = AccessTools.Method(typeof(ModEntry), "PrefixMethod_FarmerPatch");

            MethodInfo game1Info = AccessTools.Method(typeof(Game1), "UpdateControlInput", new Type[] { typeof(GameTime) });

            MethodInfo game1HaltPostfix = AccessTools.Method(typeof(ModEntry), "PostfixMethod_Game1Patch");

            MethodInfo farmerMovePositionPrefix = AccessTools.Method(typeof(ModEntry), "PrefixMethod_FarmerMovePositionPatch");

            newHarmony.Patch(farmeHaltrInfo, new HarmonyMethod(farmerHaltPrefix));
            newHarmony.Patch(game1Info, null, new HarmonyMethod(game1HaltPostfix));
            newHarmony.Patch(farmerMovePositionInfo, new HarmonyMethod(farmerMovePositionPrefix));
        }

        public static bool PrefixMethod_FarmerPatch(Game1 __instance)
        {
            if (isRightClickMoveModeOn)
            {
                if (!isMovingAutomaticaly || isBeingAutoCommand)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public static bool PrefixMethod_FarmerMovePositionPatch(Game1 __instance)
        {
            if (isRightClickMoveModeOn)
            {
                if (!isBeingControl && isMovingAutomaticaly && Context.IsPlayerFree)
                {
                    MovePosition(Game1.currentGameTime, Game1.viewport, Game1.currentLocation);
                    return false;
                }
            }
            return true;
        }

        public static void PostfixMethod_Game1Patch(Game1 __instance)
        {
            if (isRightClickMoveModeOn)
            {
                if (!isBeingControl && Context.IsPlayerFree)
                {
                    isBeingAutoCommand = true;
                    MoveVectorToCommand();
                    Game1.player.running = true;
                    isBeingAutoCommand = false;
                }
                else
                    isBeingAutoCommand = false;
            }
        }

        public static void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (Game1.player.xVelocity != 0f || Game1.player.yVelocity != 0f)
            {
                if (double.IsNaN((double)Game1.player.xVelocity) || double.IsNaN((double)Game1.player.yVelocity))
                {
                    Game1.player.xVelocity = 0f;
                    Game1.player.yVelocity = 0f;
                }
                Rectangle nextPositionFloor = Game1.player.GetBoundingBox();
                nextPositionFloor.X += (int)Math.Floor((double)Game1.player.xVelocity);
                nextPositionFloor.Y -= (int)Math.Floor((double)Game1.player.yVelocity);
                Rectangle nextPositionCeil = Game1.player.GetBoundingBox();
                nextPositionCeil.X += (int)Math.Ceiling((double)Game1.player.xVelocity);
                nextPositionCeil.Y -= (int)Math.Ceiling((double)Game1.player.yVelocity);
                Rectangle nextPosition = Rectangle.Union(nextPositionFloor, nextPositionCeil);
                if (!currentLocation.isCollidingPosition(nextPosition, viewport, true, -1, false, Game1.player))
                {
                    Game1.player.position.X += Game1.player.xVelocity;
                    Game1.player.position.Y -= Game1.player.yVelocity;
                    Game1.player.xVelocity -= Game1.player.xVelocity / 16f;
                    Game1.player.yVelocity -= Game1.player.yVelocity / 16f;
                    if (Math.Abs(Game1.player.xVelocity) <= 0.05f)
                    {
                        Game1.player.xVelocity = 0f;
                    }
                    if (Math.Abs(Game1.player.yVelocity) <= 0.05f)
                    {
                        Game1.player.yVelocity = 0f;
                    }
                }
                else
                {
                    Game1.player.xVelocity -= Game1.player.xVelocity / 16f;
                    Game1.player.yVelocity -= Game1.player.yVelocity / 16f;
                    if (Math.Abs(Game1.player.xVelocity) <= 0.05f)
                    {
                        Game1.player.xVelocity = 0f;
                    }
                    if (Math.Abs(Game1.player.yVelocity) <= 0.05f)
                    {
                        Game1.player.yVelocity = 0f;
                    }
                }
            }

            if (Game1.player.CanMove || Game1.eventUp || Game1.player.controller != null)
            {
                if (!Game1.player.temporaryImpassableTile.Intersects(Game1.player.GetBoundingBox()))
                {
                    Game1.player.temporaryImpassableTile = Rectangle.Empty;
                }
                
                float movementSpeed = Game1.player.getMovementSpeed();
                Game1.player.temporarySpeedBuff = 0f;

                if (Game1.player.movementDirections.Count > 1)
                {
                    if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
                    {
                        movementSpeed /= 0.7f;
                    }
                    else
                    {
                        movementSpeed = (float)Math.Max(1, movementSpeed);
                    }
                }
                
                if (Game1.player.movementDirections.Contains(0))
                {
                    Warp warp = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(0));
                    if (warp != null && Game1.player.IsLocalPlayer)
                    {
                        Game1.player.warpFarmer(warp);
                        return;
                    }
                    if (!currentLocation.isCollidingPosition(Game1.player.nextPosition(0), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.Y -= movementSpeed * Math.Abs(vector_AutoMove.Y);
                        Game1.player.behaviorOnMovement(0);
                    }
                    else if (!currentLocation.isCollidingPosition(Game1.player.nextPositionHalf(0), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.Y -= movementSpeed * Math.Abs(vector_AutoMove.Y) / 2f;
                        Game1.player.behaviorOnMovement(0);
                    }
                    else
                    {
                        Game1.player.position.Y -= movementSpeed * Math.Min (0.7f , Math.Abs(vector_AutoMove.Y));
                    }
                    //else if (Game1.player.movementDirections.Count == 1)
                    //{
                    //    Rectangle tmp = Game1.player.nextPosition(0);
                    //    tmp.Width /= 4;
                    //    bool leftCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                    //    tmp.X += tmp.Width * 3;
                    //    bool rightCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                    //    if (leftCorner && !rightCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(1), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.X += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //    else if (rightCorner && !leftCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(3), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.X -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //}
                }
                if (Game1.player.movementDirections.Contains(2))
                {
                    Warp warp2 = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(2));
                    if (warp2 != null && Game1.player.IsLocalPlayer)
                    {
                        Game1.player.warpFarmer(warp2);
                        return;
                    }
                    if (!currentLocation.isCollidingPosition(Game1.player.nextPosition(2), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.Y += movementSpeed * Math.Abs(vector_AutoMove.Y);
                        Game1.player.behaviorOnMovement(2);
                    }
                    else if (!currentLocation.isCollidingPosition(Game1.player.nextPositionHalf(2), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.Y += movementSpeed * Math.Abs(vector_AutoMove.Y) / 2f;
                        Game1.player.behaviorOnMovement(2);
                    }
                    else
                    {
                        Game1.player.position.Y += movementSpeed * Math.Min(0.7f, Math.Abs(vector_AutoMove.Y));
                    }
                    //else if (Game1.player.movementDirections.Count == 1)
                    //{
                    //    Rectangle tmp2 = Game1.player.nextPosition(2);
                    //    tmp2.Width /= 4;
                    //    bool leftCorner2 = currentLocation.isCollidingPosition(tmp2, viewport, true, 0, false, Game1.player);
                    //    tmp2.X += tmp2.Width * 3;
                    //    bool rightCorner2 = currentLocation.isCollidingPosition(tmp2, viewport, true, 0, false, Game1.player);
                    //    if (leftCorner2 && !rightCorner2 && !currentLocation.isCollidingPosition(Game1.player.nextPosition(1), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.X += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //    else if (rightCorner2 && !leftCorner2 && !currentLocation.isCollidingPosition(Game1.player.nextPosition(3), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.X -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //}
                }
                if (Game1.player.movementDirections.Contains(1))
                {
                    Warp warp3 = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(1));
                    if (warp3 != null && Game1.player.IsLocalPlayer)
                    {
                        Game1.player.warpFarmer(warp3);
                        return;
                    }
                    if (!currentLocation.isCollidingPosition(Game1.player.nextPosition(1), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.X += movementSpeed * Math.Abs(vector_AutoMove.X);
                        Game1.player.behaviorOnMovement(1);
                    }
                    else if (!currentLocation.isCollidingPosition(Game1.player.nextPositionHalf(1), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.X += movementSpeed * Math.Abs(vector_AutoMove.X) / 2f;
                        Game1.player.behaviorOnMovement(1);
                    }
                    else
                    {
                        Game1.player.position.X += movementSpeed * Math.Min(0.7f, Math.Abs(vector_AutoMove.X));
                    }
                    //else if (Game1.player.movementDirections.Count == 1)
                    //{
                    //    Rectangle tmp3 = Game1.player.nextPosition(1);
                    //    tmp3.Height /= 4;
                    //    bool topCorner = currentLocation.isCollidingPosition(tmp3, viewport, true, 0, false, Game1.player);
                    //    tmp3.Y += tmp3.Height * 3;
                    //    bool bottomCorner = currentLocation.isCollidingPosition(tmp3, viewport, true, 0, false, Game1.player);
                    //    if (topCorner && !bottomCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(2), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.Y += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //    else if (bottomCorner && !topCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(0), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.Y -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //}
                }
                if (Game1.player.movementDirections.Contains(3))
                {
                    Warp warp4 = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(3));
                    if (warp4 != null && Game1.player.IsLocalPlayer)
                    {
                        Game1.player.warpFarmer(warp4);
                        return;
                    }
                    if (!currentLocation.isCollidingPosition(Game1.player.nextPosition(3), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.X -= movementSpeed * Math.Abs(vector_AutoMove.X);
                        Game1.player.behaviorOnMovement(3);
                    }
                    else if (!currentLocation.isCollidingPosition(Game1.player.nextPositionHalf(3), viewport, true, 0, false, Game1.player))
                    {
                        Game1.player.position.X -= movementSpeed * Math.Abs(vector_AutoMove.X) / 2f;
                        Game1.player.behaviorOnMovement(3);
                    }
                    else
                    {
                        Game1.player.position.X -= movementSpeed * Math.Min(0.7f, Math.Abs(vector_AutoMove.X));
                    }
                    //else if (Game1.player.movementDirections.Count == 1)
                    //{
                    //    Rectangle tmp4 = Game1.player.nextPosition(3);
                    //    tmp4.Height /= 4;
                    //    bool topCorner2 = currentLocation.isCollidingPosition(tmp4, viewport, true, 0, false, Game1.player);
                    //    tmp4.Y += tmp4.Height * 3;
                    //    bool bottomCorner2 = currentLocation.isCollidingPosition(tmp4, viewport, true, 0, false, Game1.player);
                    //    if (topCorner2 && !bottomCorner2 && !currentLocation.isCollidingPosition(Game1.player.nextPosition(2), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.Y += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //    else if (bottomCorner2 && !topCorner2 && !currentLocation.isCollidingPosition(Game1.player.nextPosition(0), viewport, true, 0, false, Game1.player))
                    //    {
                    //        Game1.player.position.Y -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                    //    }
                    //}
                }

                if (Game1.player.movementDirections.Count == 2)
                {
                    if (Math.Abs(vector_AutoMove.Y / vector_AutoMove.X).CompareTo(0.45f) < 0)
                    {
                        Game1.player.SetMovingDown(false);
                        Game1.player.SetMovingUp(false);
                    }
                    else if (Math.Abs(vector_AutoMove.Y) > Math.Sin(Math.PI / 3))
                    {
                        Game1.player.SetMovingRight(false);
                        Game1.player.SetMovingLeft(false);
                    }
                }
            }

            if (Game1.player.movementDirections.Count > 0 && !Game1.player.UsingTool)
            {
                Game1.player.FarmerSprite.intervalModifier = 1f - (Game1.player.running ? 0.03f : 0.025f) * (Math.Max(1f, ((float)Game1.player.speed + (Game1.eventUp ? 0f : ((float)Game1.player.addedSpeed + (Game1.player.isRidingHorse() ? 4.6f : 0f)))) * Game1.player.movementMultiplier * (float)Game1.currentGameTime.ElapsedGameTime.Milliseconds) * 1.25f);
            }
            else
            {
                Game1.player.FarmerSprite.intervalModifier = 1f;
            }
            if (Game1.player.temporarilyInvincible)
            {
                Game1.player.temporaryInvincibilityTimer += time.ElapsedGameTime.Milliseconds;
                if (Game1.player.temporaryInvincibilityTimer > 1200)
                {
                    Game1.player.temporarilyInvincible = false;
                    Game1.player.temporaryInvincibilityTimer = 0;
                }
            }
            if (currentLocation != null && currentLocation.isFarmerCollidingWithAnyCharacter())
            {
                Game1.player.temporaryImpassableTile = new Rectangle((int)Game1.player.getTileLocation().X * 64, (int)Game1.player.getTileLocation().Y * 64, 64, 64);
            }
        }

    }
}

