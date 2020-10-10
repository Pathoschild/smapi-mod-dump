/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using xTile.ObjectModel;

namespace RightClickMoveMode
{
    public class ModConfig
    {
        public bool ExtendedModeDefault { get; set; } = true;
        public bool RightClickMoveModeDefault { get; set; } = true;
        public String RightClickMoveModeToggleButton { get; set; } = "G";

        public bool HoldingMoveOnly { get; set; } = false;
        public int HoldTickCount { get; set; } = 10;
        public bool StopAfterHold { get; set; } = true;
        //public bool PerformUseAtMouseReleasePlace { get; set; } = false;

        public bool WeaponsSpecticalInteraction { get; set; } = true;
        public int WeaponsSpecticalInteractionType { get; set; } = 1;
    }

    public class ModEntry : Mod
    {
        private ModConfig config;

        public const float hitboxRadius = 64f * 2;

        public static bool isRightClickMoveModeOn = true;
        public static bool isExtendedModeOn = true;
        public static bool isWeaponsSpecticalInteraction = true;
        public static int WeaponsSpecticalInteractionType = 0;
        public static bool isHoldingMoveOnly = false;
        public static int HoldTick = 10;
        public static bool isStopAfterHold = true;
        //public static bool isPerformUseAtMouseReleasePlace = true;

        public static bool isMovingAutomaticaly = false;
        public static bool isHoldingRightMouse;
        public static bool isBeingAutoCommand = false;
        public static bool isMouseOutsiteHitBox = false;

        public static bool isBeingControl = false;

        public static bool isHoldingMove = false;

        public static bool isHoldingLeftCtrl = false;
        public static bool isHoldingRunButton = false;
        public static bool isHoldingRightCtrl = false;
        public static bool isHoldingRightAlt = false;
        public static bool isWheeling = false;

        public static int isDone = 0;

        //public static KeyValuePair<Vector2, TerrainFeature> pointedTerrainFeatures = new KeyValuePair<Vector2, TerrainFeature>(new Vector2(0f,0f), null);
        //public static LargeTerrainFeature pointedLargeTerrainFeature = null;
        public static StardewValley.NPC pointedNPC = null;

        private String RightClickMoveModeOpenButton;

        private static Vector2 vector_PlayerToDestination;
        private static Vector2 vector_PlayerToMouse;
        private static Vector2 vector_AutoMove;

        private static Vector2 position_MouseOnScreen;
        private static Vector2 position_Source;
        private static Vector2 position_Destination;

        private static Vector2 grabTile;

        private static int tickCount = 15;
        private static int holdCount = 15;

        private static int currentToolIndex = 1;

        public static bool isDebugMode = false;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed += this.InputEvents_ButtonPressed;
            Helper.Events.Input.CursorMoved += Input_CursorMoved;
            Helper.Events.Input.MouseWheelScrolled += Input_MouseWheelScrolled;
            Helper.Events.Input.ButtonReleased += this.InputEvents_ButtonReleased;
            Helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
            Helper.Events.Player.Warped += this.PlayerEvents_Warped;

            StartPatching();

            this.config = this.Helper.ReadConfig<ModConfig>();

            isRightClickMoveModeOn = this.config.RightClickMoveModeDefault;

            RightClickMoveModeOpenButton = this.config.RightClickMoveModeToggleButton.ToUpper();

            if (this.config.WeaponsSpecticalInteraction) ;
            WeaponsSpecticalInteractionType = this.config.WeaponsSpecticalInteractionType;

            isHoldingMoveOnly = this.config.HoldingMoveOnly;
            HoldTick = this.config.HoldTickCount;

            isStopAfterHold = this.config.StopAfterHold;
            //isPerformUseAtMouseReleasePlace = this.config.PerformUseAtMouseReleasePlace;

            position_MouseOnScreen = new Vector2(0f, 0f);
            position_Source = new Vector2(0f, 0f);
            vector_PlayerToDestination = new Vector2(0f, 0f);
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            bool flag = Context.IsWorldReady;
            if (isExtendedModeOn)
            {
                if (flag)
                    if ((isHoldingLeftCtrl || isHoldingRightCtrl) && isWheeling)
                    {
                        Game1.player.CurrentToolIndex = currentToolIndex;
                    }
            }

            if (isRightClickMoveModeOn)
                if (flag)
                {
                    vector_PlayerToMouse.X = position_MouseOnScreen.X + Game1.viewport.X - Game1.player.GetBoundingBox().Center.X;
                    vector_PlayerToMouse.Y = position_MouseOnScreen.Y + Game1.viewport.Y - Game1.player.GetBoundingBox().Center.Y;

                    if (Context.IsPlayerFree)
                    {
                        if (isHoldingRightMouse)
                        {
                            if (holdCount < HoldTick)
                            {
                                isHoldingMove = false;
                                holdCount++;
                            }
                            else
                            {
                                isHoldingMove = true;
                                isDone = 0;
                            }
                        }

                        if (isHoldingMove)
                        {
                            isMovingAutomaticaly = true;

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
                            if (isDone == 2)
                            {
                                position_Destination = pointedNPC.Position;
                            }
                            vector_PlayerToDestination.X = position_Destination.X - Game1.player.GetBoundingBox().Center.X;
                            vector_PlayerToDestination.Y = position_Destination.Y - Game1.player.GetBoundingBox().Center.Y;
                        }

                        if (Game1.player.ActiveObject != null)
                        {
                            if (isMovingAutomaticaly && (Game1.player.ActiveObject is Furniture))
                            {
                                isMovingAutomaticaly = false;
                                Game1.player.Halt();
                            }
                        }
                    }
                }
        }

        private void PlayerEvents_Warped(object sender, WarpedEventArgs e)
        {
            isMovingAutomaticaly = false;

            if (e.OldLocation is StardewValley.Locations.Town && e.NewLocation is StardewValley.Locations.Mountain)
            {
                Game1.player.Position += new Vector2(0f, -10f);
            }
            if (e.OldLocation is StardewValley.Farm && e.NewLocation.Name == "Backwoods")
            {
                Game1.player.Position += new Vector2(0f, -10f);
            }
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

        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            bool flag = Context.IsWorldReady;
            string button = e.Button.ToString();

            if (button == RightClickMoveModeOpenButton)
            {
                isRightClickMoveModeOn = !isRightClickMoveModeOn;
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

            if (isRightClickMoveModeOn)
            {
                if (e.Button == Game1.options.runButton[0].ToSButton())
                {
                    isHoldingRunButton = true;
                }

                if (flag)
                {
                    if (button == "MouseRight")
                    {
                        isHoldingRightMouse = true;
                        holdCount = 0;
                    }

                    bool flag2 = button == "MouseRight" && Context.IsPlayerFree;
                    bool flag3 = false;

                    isMouseOutsiteHitBox = vector_PlayerToMouse.Length().CompareTo(hitboxRadius) > 0;

                    if (Game1.player.ActiveObject != null)
                    {
                        if (Game1.player.ActiveObject is Furniture)
                        {
                            flag2 = false;
                        }
                    }
                    if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && !Game1.player.CurrentTool.Name.Contains("Scythe") && SpecialCooldown((MeleeWeapon)Game1.player.CurrentTool) <= 0)
                    {
                        if (WeaponsSpecticalInteractionType == 1)
                        {
                            flag2 = false;
                            if (isMouseOutsiteHitBox && button == "MouseRight" && !Game1.player.isRidingHorse())
                            {
                                ((MeleeWeapon)Game1.player.CurrentTool).animateSpecialMove(Game1.player);
                                Helper.Input.Suppress(e.Button);
                            }
                        }
                        else if (WeaponsSpecticalInteractionType == 2)
                        {
                            if (button == "MouseRight")
                            {
                                Helper.Input.Suppress(e.Button);
                                //isMouseOutsiteHitBox = vector_PlayerToMouse.Length().CompareTo(hitboxRadius/4) > 0;
                                isMouseOutsiteHitBox = true;
                            }
                        }
                    }


                    if (flag2)
                    {
                        if (!isHoldingMoveOnly)
                        {
                            position_Destination.X = (float)e.Cursor.ScreenPixels.X + Game1.viewport.X;
                            position_Destination.Y = (float)e.Cursor.ScreenPixels.Y + Game1.viewport.Y;

                            vector_PlayerToDestination.X = position_Destination.X - Game1.player.GetBoundingBox().Center.X;
                            vector_PlayerToDestination.Y = position_Destination.Y - Game1.player.GetBoundingBox().Center.Y;
                            grabTile = new Vector2((float)(position_MouseOnScreen.X + Game1.viewport.X), (float)(position_MouseOnScreen.Y + Game1.viewport.Y)) / 64f;

                            isMovingAutomaticaly = true;
                            isBeingControl = false;
                        }

                        flag3 = flag3 || isMouseOutsiteHitBox;

                        if (isHoldingRunButton && Game1.player.isRidingHorse())
                        {
                            Helper.Input.Suppress(e.Button);
                        }
                        else if (flag3)
                        {
                            Helper.Input.Suppress(e.Button);

                            isDone = getActionType(ref grabTile);
                        }
                        else if (!flag3)
                        {
                            isDone = 0;
                        }
                    }
                    else
                    {
                        if (e.Button.IsUseToolButton())
                        {
                            tickCount = 15;
                        }
                        else
                            tickCount = 0;
                        isBeingControl = true;
                    }
                }
            }
        }

        private int getActionType(ref Vector2 grabTile)
        {

            // There is 5 type:
            // 1 is for Object is 1x1 tile size but with 2x1 hit box (Chess, ...)
            // 2 is for NPC
            // 3 to handle Fence, Seed, ... thaat placeable
            // 4 to handle terrainFeatures (some has hitbox that unreachable and have to change)
            // 5 is Unknown, try to grap at pointed place 
            StardewValley.Object pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);

            if (pointedObject == null && Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y + 1) != null)
            {
                grabTile.Y += 1;
                pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
            }

            if (pointedObject != null && pointedObject.Type != null && (pointedObject.IsSpawnedObject || (pointedObject.Type.Equals("Crafting")
                && pointedObject.Type.Equals("interactive"))))
            {
                return 1;
            }

            pointedNPC = Game1.player.currentLocation.isCharacterAtTile(grabTile);
            if (pointedNPC == null)
                pointedNPC = Game1.player.currentLocation.isCharacterAtTile(grabTile + new Vector2(0f, 1f));
            if (pointedNPC != null && !pointedNPC.IsMonster)
            {
                currentToolIndex = Game1.player.CurrentToolIndex;
                return 2;
            }

            if (Game1.player.ActiveObject != null && Game1.player.ActiveObject.isPlaceable())
            {
                currentToolIndex = Game1.player.CurrentToolIndex;
                return 3;
            }

            foreach (KeyValuePair<Vector2, TerrainFeature> v in Game1.player.currentLocation.terrainFeatures.Pairs)
            {
                if (v.Value.getBoundingBox(v.Key).Intersects(new Rectangle((int)grabTile.X * 64, (int)grabTile.Y * 64, 64, 64)))
                {
                    //pointedTerrainFeatures = v;
                    if ((v.Value is Grass) || (v.Value is HoeDirt && !((HoeDirt)v.Value).readyForHarvest())) ;
                    else
                        return 4;
                }
            }

            if (Game1.player.currentLocation.largeTerrainFeatures != null)
            {
                foreach (LargeTerrainFeature f in Game1.player.currentLocation.largeTerrainFeatures)
                {
                    if (f.getBoundingBox().Intersects(new Rectangle((int)grabTile.X * 64, (int)grabTile.Y * 64, 64, 64)))
                    {
                        return 4;
                    }
                }
            }

            if (Game1.isActionAtCurrentCursorTile || Game1.isInspectionAtCurrentCursorTile)
            {
                if (!Game1.currentLocation.isActionableTile((int)grabTile.X, (int)grabTile.Y, Game1.player))
                    if (Game1.currentLocation.isActionableTile((int)grabTile.X, (int)grabTile.Y + 1, Game1.player))
                        grabTile.Y += 1;
                return 1;
            }

            return 5;
        }


        private void Input_MouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            bool flag = Context.IsWorldReady;

            if (isExtendedModeOn)
            {
                if (e.OldValue != e.NewValue)
                    isWheeling = true;
                else
                    isWheeling = false;

                if (flag)
                {
                    if (isHoldingLeftCtrl || isHoldingRightCtrl)
                    {
                        if (e.OldValue < e.NewValue)
                        {
                            currentToolIndex = Game1.player.CurrentToolIndex;
                            if (Game1.options.zoomLevel <= Options.maxZoom)
                                Game1.options.zoomLevel += 0.05f;
                            Game1.exitActiveMenu();
                        }
                        else if (e.OldValue > e.NewValue)
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

        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            bool flag = Context.IsWorldReady;

            if (isRightClickMoveModeOn)
                if (flag)
                {
                    if (Context.IsPlayerFree)
                    {
                        position_MouseOnScreen.X = (float)e.NewPosition.ScreenPixels.X;
                        position_MouseOnScreen.Y = (float)e.NewPosition.ScreenPixels.Y;
                    }
                }
        }

        private void InputEvents_ButtonReleased(object sender, ButtonReleasedEventArgs e)
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

            if (isRightClickMoveModeOn)
            {
                if (e.Button == Game1.options.runButton[0].ToSButton())
                {
                    isHoldingRunButton = false;
                }

                if (flag)
                {
                    if (button == "MouseRight")
                    {
                        isHoldingRightMouse = false;
                        if (isHoldingMove)
                        {
                            isDone = 0;
                            holdCount = 0;
                            isHoldingMove = false;
                            if (!isStopAfterHold)
                            {
                                position_Destination.X = (float)e.Cursor.ScreenPixels.X + Game1.viewport.X;
                                position_Destination.Y = (float)e.Cursor.ScreenPixels.Y + Game1.viewport.Y;

                                vector_PlayerToDestination.X = position_Destination.X - Game1.player.GetBoundingBox().Center.X;
                                vector_PlayerToDestination.Y = position_Destination.Y - Game1.player.GetBoundingBox().Center.Y;
                            }
                            else
                            {
                                isMovingAutomaticaly = false;
                            }
                            //if (isPerformUseAtMouseReleasePlace && !isStopAfterHold && (holdCount > HoldTick))
                            //{
                            //    grabTile = new Vector2((float)(position_MouseOnScreen.X + Game1.viewport.X), (float)(position_MouseOnScreen.Y + Game1.viewport.Y)) / 64f;
                            //    getActionType(ref grabTile);
                            //}
                        }
                    }
                }
            }
        }

        public static void TryToCheckGrapTile()
        {
            if (isDone == 0) return;

            if (Game1.player.isRidingHorse())
            {
                if ((isDone == 2) && Utility.tileWithinRadiusOfPlayer(pointedNPC.getTileX(), pointedNPC.getTileY(), 2, Game1.player))
                    Game1.player.mount.dismount();
                else if (isDone != 0 && isDone != 5 && Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 2, Game1.player))
                    Game1.player.mount.dismount();
            }

            if (isDone == 1 && Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player) && Game1.tryToCheckAt(grabTile, Game1.player))
            {
                isDone = 0;
                isMovingAutomaticaly = false;
            }

            if ((isDone == 2 || isDone == 3) && (Game1.player.CurrentToolIndex != currentToolIndex))
            {
                isDone = 0;
            }

            if (isDone == 3 && (Game1.player.ActiveObject == null))
            {
                isDone = 0;
            }

            if (isDone == 2 && Utility.tileWithinRadiusOfPlayer(pointedNPC.getTileX(), pointedNPC.getTileY(), 1, Game1.player) && Game1.tryToCheckAt(pointedNPC.getTileLocation(), Game1.player))
            {
                isDone = 0;
                isMovingAutomaticaly = false;
            }

            if (isDone == 3 && Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                int stack = Game1.player.ActiveObject.Stack;
                Utility.tryToPlaceItem(Game1.player.currentLocation, Game1.player.ActiveObject, (int)grabTile.X * 64 + 32, (int)grabTile.Y * 64 + 32);
                if (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Stack < stack || Game1.player.ActiveObject.isPlaceable())
                {
                    isDone = 0;
                    isMovingAutomaticaly = false;
                }
            }

            if (isDone == 4 && Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                Game1.tryToCheckAt(grabTile, Game1.player);
                isDone = 0;
            }

            if (isDone == 5 && Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                if (!Game1.player.isRidingHorse())
                    Game1.tryToCheckAt(grabTile, Game1.player);
                isDone = 0;
            }
        }

        public static void MoveVectorToCommand()
        {
            bool flag = ModEntry.isMovingAutomaticaly;

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

                TryToCheckGrapTile();

                bool flag2 = false;
                bool flag3 = false;

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

                if (flag2 && flag3)
                {
                    ModEntry.isMovingAutomaticaly = false;
                    isDone = 0;
                }
            }
        }

        public static void StartPatching()
        {
            HarmonyInstance newHarmony = HarmonyInstance.Create("ylsama.RightClickMoveMode");

            MethodInfo farmer_Halt_Info = AccessTools.Method(typeof(Farmer), "Halt");
            MethodInfo farmer_Halt_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_FarmerPatch");
            newHarmony.Patch(farmer_Halt_Info, new HarmonyMethod(farmer_Halt_PrefixPatch));


            MethodInfo farmer_getMovementSpeed_Info = AccessTools.Method(typeof(Farmer), "getMovementSpeed");
            MethodInfo farmer_getMovementSpeed_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_Farmer_getMovementSpeedPatch");
            newHarmony.Patch(farmer_getMovementSpeed_Info, new HarmonyMethod(farmer_getMovementSpeed_PrefixPatch));

            MethodInfo farmer_MovePosition_Info = AccessTools.Method(typeof(Farmer), "MovePosition", new Type[] { typeof(GameTime), typeof(xTile.Dimensions.Rectangle), typeof(GameLocation) });
            MethodInfo farmer_MovePosition_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_FarmerMovePositionPatch");
            newHarmony.Patch(farmer_MovePosition_Info, new HarmonyMethod(farmer_MovePosition_PrefixPatch));

            MethodInfo game1_UpdateControlInput_Info = AccessTools.Method(typeof(Game1), "UpdateControlInput", new Type[] { typeof(GameTime) });
            MethodInfo game1_UpdateControlInput_PostfixPatch = AccessTools.Method(typeof(ModEntry), "PostfixMethod_Game1Patch");
            newHarmony.Patch(game1_UpdateControlInput_Info, null, new HarmonyMethod(game1_UpdateControlInput_PostfixPatch));
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
                if (!isBeingControl && isMovingAutomaticaly && Context.IsPlayerFree && Game1.player.CanMove)
                {
                    MovePosition(Game1.currentGameTime, Game1.viewport, Game1.player.currentLocation);
                    return false;
                }
            }
            return true;
        }

        public static void PostfixMethod_Game1Patch(Game1 __instance)
        {
            if (isRightClickMoveModeOn)
            {
                if (!isBeingControl && Context.IsPlayerFree && Game1.player.CanMove)
                {
                    isBeingAutoCommand = true;
                    MoveVectorToCommand();

                    if (isHoldingRunButton && !Game1.player.canOnlyWalk)
                    {
                        Game1.player.setRunning(!Game1.options.autoRun, false);
                        Game1.player.setMoving(Game1.player.running ? (byte)16 : (byte)48);
                    }
                    else if (!isHoldingRunButton && !Game1.player.canOnlyWalk)
                    {
                        Game1.player.setRunning(Game1.options.autoRun, false);
                        Game1.player.setMoving(Game1.player.running ? (byte)16 : (byte)48);
                    }

                    isBeingAutoCommand = false;
                }
                else
                    isBeingAutoCommand = false;
            }
        }

        public static bool PrefixMethod_Farmer_getMovementSpeedPatch(Farmer __instance, ref float __result)
        {
            if (isRightClickMoveModeOn)
            {
                if (!isBeingControl && Context.IsPlayerFree)
                {

                    float movementSpeed;
                    if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
                    {
                        Game1.player.movementMultiplier = 0.066f;
                        movementSpeed = Math.Max(1f, ((float)Game1.player.speed + (Game1.eventUp ? 0f : ((float)Game1.player.addedSpeed + (Game1.player.isRidingHorse() ? 4.6f : Game1.player.temporarySpeedBuff)))) * Game1.player.movementMultiplier * (float)Game1.currentGameTime.ElapsedGameTime.Milliseconds);
                    }
                    else
                    {
                        movementSpeed = Math.Max(1f, (float)Game1.player.speed + (Game1.eventUp ? ((float)Math.Max(0, Game1.CurrentEvent.farmerAddedSpeed - 2)) : ((float)Game1.player.addedSpeed + (Game1.player.isRidingHorse() ? 5f : Game1.player.temporarySpeedBuff))));
                    }
                    __result = movementSpeed;
                    return false;
                }
            }
            return true;
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

                if (Game1.player.movementDirections.Contains(0))
                    TryMoveDrection(time, viewport, currentLocation, 0);

                if (Game1.player.movementDirections.Contains(2))
                    TryMoveDrection(time, viewport, currentLocation, 2);

                if (Game1.player.movementDirections.Contains(1))
                    TryMoveDrection(time, viewport, currentLocation, 1);

                if (Game1.player.movementDirections.Contains(3))
                    TryMoveDrection(time, viewport, currentLocation, 3);

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

        public static int RightDirection(int faceDirection)
        {
            switch (faceDirection)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 0;
                default:
                    return -1;
            }
        }

        public static int LeftDirection(int faceDirection)
        {
            switch (faceDirection)
            {
                case 0:
                    return 3;
                case 1:
                    return 0;
                case 2:
                    return 1;
                case 3:
                    return 2;
                default:
                    return -1;
            }
        }

        public static void TryMoveDrection(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation, int faceDirection)
        {
            Warp warp = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(faceDirection));
            if (warp != null && Game1.player.IsLocalPlayer)
            {
                Game1.player.warpFarmer(warp);
                return;
            }
            float movementSpeed = Game1.player.getMovementSpeed();
            if (Game1.player.movementDirections.Contains(faceDirection))
            {
                Rectangle nextPos = Game1.player.nextPosition(faceDirection);

                if (!currentLocation.isCollidingPosition(nextPos, viewport, true, 0, false, Game1.player))
                {
                    if (faceDirection == 0 || faceDirection == 2)
                        Game1.player.position.Y += movementSpeed * vector_AutoMove.Y;
                    else
                        Game1.player.position.X += movementSpeed * vector_AutoMove.X;

                    Game1.player.behaviorOnMovement(faceDirection);
                }
                else
                {
                    nextPos = Game1.player.nextPositionHalf(faceDirection);

                    if (!currentLocation.isCollidingPosition(nextPos, viewport, true, 0, false, Game1.player))
                    {

                        if (faceDirection == 0 || faceDirection == 2)
                            Game1.player.position.Y += movementSpeed * vector_AutoMove.Y / 2f;
                        else
                            Game1.player.position.X += movementSpeed * vector_AutoMove.X / 2f;

                        Game1.player.behaviorOnMovement(faceDirection);
                    }
                    else if (Game1.player.movementDirections.Count == 1)
                    {
                        Rectangle tmp = Game1.player.nextPosition(faceDirection);
                        tmp.Width /= 4;
                        bool leftCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                        tmp.X += tmp.Width * 3;
                        bool rightCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                        if (leftCorner && !rightCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(LeftDirection(faceDirection)), viewport, true, 0, false, Game1.player))
                        {
                            if (faceDirection == 0 || faceDirection == 2)
                                Game1.player.position.X += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                            else
                                Game1.player.position.Y += (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                        }
                        else if (rightCorner && !leftCorner && !currentLocation.isCollidingPosition(Game1.player.nextPosition(RightDirection(faceDirection)), viewport, true, 0, false, Game1.player))
                        {
                            if (faceDirection == 0 || faceDirection == 2)
                                Game1.player.position.X -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                            else
                                Game1.player.position.Y -= (float)Game1.player.speed * ((float)time.ElapsedGameTime.Milliseconds / 64f);
                        }
                    }
                }
            }
        }
    }
}

