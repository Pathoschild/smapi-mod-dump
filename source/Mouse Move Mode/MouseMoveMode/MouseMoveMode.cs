/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace MouseMoveMode
{
    public class ModConfig
    {
        public bool RightClickMoveModeDefault { get; set; } = true;
        public KeybindList RightClickMoveModeToggleButton { get; set; } = KeybindList.Parse("F6");
        public KeybindList ForceMoveButton { get; set; } = KeybindList.Parse("Space");
        public int HoldTickCount { get; set; } = 15;
        public bool HoldingMoveOnly { get; set; } = false;
        public int WeaponsSpecticalInteractionType { get; set; } = 2;
        public bool ExtendedModeDefault { get; set; } = true;
        public float MouseWhellingMaxZoom = Options.maxZoom;
        public float MouseWhellingMinZoom = Options.minZoom;
        public KeybindList FullScreenKeybindShortcut { get; set; } = KeybindList.Parse("RightAlt + Enter");
    }

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static ModConfig config;
        public static float hitboxRadius = 64f*2;
        public static float baseHitboxRadius = 64f*2;

        public static bool isMovingAutomaticaly = false;
        public static bool isBeingAutoCommand = false;
        public static bool isMouseOutsiteHitBox = false;
        public static bool isBeingControl = false;
        public static bool isHoldingMove = false;
        public static int isTryToDoActionAtClickedTitle = 0;

        public static bool isHoldingRunButton = false;

        private static Vector2 grabTile;
        public static NPC pointedNPC = null;

        private static Vector2 vector_PlayerToDestination;
        private static Vector2 vector_PlayerToMouse;
        private static Vector2 vector_AutoMove;

        private static Vector2 position_MouseOnScreen;
        private static Vector2 position_Destination;


        private static int tickCount = 15;
        private static int holdCount = 15;

        private static int currentToolIndex = 1;
        public static bool isDebugMode = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed+=this.MouseMoveMode_InputEvents_ButtonPressed;
            Helper.Events.Input.ButtonPressed+=this.ExtendedMode_InputEvents_ButtonPressed;
            Helper.Events.Input.CursorMoved+=Input_CursorMoved;
            Helper.Events.Input.MouseWheelScrolled+=Input_MouseWheelScrolled;
            Helper.Events.Input.ButtonReleased+=this.InputEvents_ButtonReleased;
            Helper.Events.GameLoop.UpdateTicked+=this.GameEvents_UpdateTick;
            Helper.Events.Player.Warped+=this.PlayerEvents_Warped;

            StartPatching();

            ModEntry.config=this.Helper.ReadConfig<ModConfig>();
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            bool flag = Context.IsWorldReady;

            if (!config.RightClickMoveModeDefault)
                return;
            if (!Context.IsWorldReady)
                return;

            hitboxRadius=baseHitboxRadius;

            if (Game1.player.ActiveObject!=null)
            {
                if (Game1.player.ActiveObject.isPlaceable())
                {
                    hitboxRadius=baseHitboxRadius*1.5f;
                }
            }
            vector_PlayerToMouse.X=position_MouseOnScreen.X+Game1.viewport.X-Game1.player.GetBoundingBox().Center.X;
            vector_PlayerToMouse.Y=position_MouseOnScreen.Y+Game1.viewport.Y-Game1.player.GetBoundingBox().Center.Y;

            if (!Context.IsPlayerFree)
                return;

            MouseState mouseState = Mouse.GetState();
            switch (mouseState.RightButton)
            {
                case ButtonState.Pressed:
                    if (holdCount<config.HoldTickCount)
                    {
                        isHoldingMove=false;
                        holdCount++;
                    } else
                    {
                        isHoldingMove=true;
                        isTryToDoActionAtClickedTitle=0;
                    }
                    break;
                default:
                    if (holdCount>=config.HoldTickCount)
                    {
                        isHoldingMove=false;
                        isMovingAutomaticaly=false;
                    }
                    holdCount=0;
                    break;
            }
            if (isHoldingMove)
            {
                isMovingAutomaticaly=true;

                if (isBeingControl)
                {
                    if (tickCount==0)
                    {
                        isBeingControl=false;
                        tickCount=15;
                    } else
                        tickCount--;
                }
            } else
            {
                if (isTryToDoActionAtClickedTitle==2)
                {
                    position_Destination=pointedNPC.Position;
                }
                vector_PlayerToDestination.X=position_Destination.X-Game1.player.GetBoundingBox().Center.X;
                vector_PlayerToDestination.Y=position_Destination.Y-Game1.player.GetBoundingBox().Center.Y;
            }

            if (Game1.player.ActiveObject!=null)
            {
                if (isMovingAutomaticaly&&(Game1.player.ActiveObject is StardewValley.Objects.Furniture))
                {
                    isMovingAutomaticaly=false;
                    Game1.player.Halt();
                }
            }
        }

        private void PlayerEvents_Warped(object sender, WarpedEventArgs e)
        {
            isMovingAutomaticaly=false;
            // There are location that player's new position (after warp) is too close to new warp
            // This prevent warp back to back
            if (e.OldLocation is StardewValley.Locations.Town&&e.NewLocation is StardewValley.Locations.Mountain)
            {
                Game1.player.Position+=new Vector2(0f, -10f);
            }
            if (e.OldLocation is StardewValley.Farm&&e.NewLocation.Name=="Backwoods")
            {
                Game1.player.Position+=new Vector2(0f, -10f);
            }
        }

        private int SpecialCooldown(MeleeWeapon currentWeapon)
        {
            switch (currentWeapon.type)
            {
                case MeleeWeapon.defenseSword:
                    return MeleeWeapon.defenseCooldown;
                case MeleeWeapon.dagger:
                    return MeleeWeapon.daggerCooldown;
                case MeleeWeapon.club:
                    return MeleeWeapon.clubCooldown;
                case MeleeWeapon.stabbingSword:
                    return MeleeWeapon.attackSwordCooldown;
                default:
                    return 0;
            }
        }

        private void ExtendedMode_InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!config.ExtendedModeDefault)
                return;

            if (config.FullScreenKeybindShortcut.JustPressed())
            {
                if (Game1.options.isCurrentlyWindowedBorderless()||Game1.options.isCurrentlyFullscreen())
                    Game1.options.setWindowedOption("Windowed");
                else
                {
                    Game1.options.setWindowedOption("Windowed Borderless");
                }
                Game1.exitActiveMenu();
            }
        }

        private void MouseMoveMode_InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            string button = e.Button.ToString();

            if (!Context.IsWorldReady)
                return;

            if (config.RightClickMoveModeToggleButton.JustPressed())
            {
                config.RightClickMoveModeDefault=!config.RightClickMoveModeDefault;
            }

            if (!config.RightClickMoveModeDefault)
                return;

            if (e.Button==Game1.options.runButton[0].ToSButton())
            {
                isHoldingRunButton=true;
            }

            bool mouseRightIsDown = button=="MouseRight"&&Context.IsPlayerFree;
            bool isMouseOutsiteHitBox = vector_PlayerToMouse.Length().CompareTo(hitboxRadius)>0;

            if (Game1.player.ActiveObject!=null)
            {
                if (Game1.player.ActiveObject is Furniture)
                {
                    mouseRightIsDown=false;
                }
            }
            if (Game1.player.CurrentTool!=null&&Game1.player.CurrentTool is MeleeWeapon weapon&&!Game1.player.CurrentTool.Name.Contains("Scythe")&&SpecialCooldown(weapon)<=0)
            {
                if (config.WeaponsSpecticalInteractionType==1)
                {
                    mouseRightIsDown=false;
                    if (isMouseOutsiteHitBox&&button=="MouseRight"&&!Game1.player.isRidingHorse())
                    {
                        weapon.animateSpecialMove(Game1.player);
                        Helper.Input.Suppress(e.Button);
                    }
                } else if (config.WeaponsSpecticalInteractionType==2)
                {
                    if (button=="MouseRight")
                    {
                        Helper.Input.Suppress(e.Button);
                        isMouseOutsiteHitBox=true;
                    }
                    if ((button=="MouseMiddle"||button=="MouseX1")&&!Game1.player.isRidingHorse())
                    {
                        weapon.animateSpecialMove(Game1.player);
                        Helper.Input.Suppress(e.Button);
                    }
                    if (button=="MouseLeft"&&vector_PlayerToMouse.Length().CompareTo(hitboxRadius)<0&&!Game1.player.isRidingHorse())
                    {
                        if (vector_PlayerToMouse.Y<32f)
                        {
                            weapon.animateSpecialMove(Game1.player);
                            Helper.Input.Suppress(e.Button);
                        }
                    }
                }
            }


            if (mouseRightIsDown)
            {
                if (!config.HoldingMoveOnly)
                {
                    position_Destination.X=position_MouseOnScreen.X+Game1.viewport.X;
                    position_Destination.Y=position_MouseOnScreen.Y+Game1.viewport.Y;

                    vector_PlayerToDestination.X=position_Destination.X-Game1.player.GetBoundingBox().Center.X;
                    vector_PlayerToDestination.Y=position_Destination.Y-Game1.player.GetBoundingBox().Center.Y;
                    grabTile=new Vector2((float)(position_MouseOnScreen.X+Game1.viewport.X), (float)(position_MouseOnScreen.Y+Game1.viewport.Y))/64f;

                    isMovingAutomaticaly=true;
                    isBeingControl=false;
                }

                if (config.ForceMoveButton.IsDown())
                {
                    Helper.Input.Suppress(e.Button);
                } else if (isMouseOutsiteHitBox)
                {
                    Helper.Input.Suppress(e.Button);

                    isTryToDoActionAtClickedTitle=GetActionType(ref grabTile);
                } else if (!isMouseOutsiteHitBox)
                {
                    isTryToDoActionAtClickedTitle=0;
                }
            } else
            {
                if (e.Button.IsUseToolButton())
                {
                    tickCount=15;
                } else
                    tickCount=0;
                if (!config.ForceMoveButton.IsDown())
                    isBeingControl=true;
            }
        }

        private int GetActionType(ref Vector2 grabTile)
        {

            // There is 5 type:
            // 1 is for Object is 1x1 tile size but with 2x1 hit box (Chess, ...)
            // 2 is for NPC
            // 3 to handle Fence, Seed, ... thaat placeable
            // 4 to handle terrainFeatures (some has hitbox that unreachable and have to change)
            // 5 is Unknown, try to grap at pointed place 
            StardewValley.Object pointedObject = Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);

            if (pointedObject==null&&Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y+1)!=null)
            {
                grabTile.Y+=1;
                pointedObject=Game1.player.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
            }

            if (pointedObject!=null&&pointedObject.Type!=null&&(pointedObject.IsSpawnedObject||(pointedObject.Type.Equals("Crafting")
                &&pointedObject.Type.Equals("interactive"))))
            {
                return 1;
            }

            pointedNPC=Game1.player.currentLocation.isCharacterAtTile(grabTile);
            if (pointedNPC==null)
                pointedNPC=Game1.player.currentLocation.isCharacterAtTile(grabTile+new Vector2(0f, 1f));
            if (pointedNPC!=null&&!pointedNPC.IsMonster)
            {
                currentToolIndex=Game1.player.CurrentToolIndex;
                return 2;
            }

            if (Game1.player.ActiveObject!=null&&Game1.player.ActiveObject.isPlaceable())
            {
                currentToolIndex=Game1.player.CurrentToolIndex;
                return 3;
            }

            foreach (var v in Game1.player.currentLocation.terrainFeatures.Pairs)
            {
                if (v.Value.getBoundingBox(v.Key).Intersects(new Rectangle((int)grabTile.X*64, (int)grabTile.Y*64, 64, 64)))
                {
                    if ((v.Value is Grass)||(v.Value is HoeDirt dirt&&!dirt.readyForHarvest()))
                    { } else
                        return 4;
                }
            }

            if (Game1.player.currentLocation.largeTerrainFeatures!=null)
            {
                foreach (var f in Game1.player.currentLocation.largeTerrainFeatures)
                {
                    if (f.getBoundingBox().Intersects(new Rectangle((int)grabTile.X*64, (int)grabTile.Y*64, 64, 64)))
                    {
                        return 4;
                    }
                }
            }

            if (Game1.isActionAtCurrentCursorTile||Game1.isInspectionAtCurrentCursorTile)
            {
                if (!Game1.currentLocation.isActionableTile((int)grabTile.X, (int)grabTile.Y, Game1.player))
                    if (Game1.currentLocation.isActionableTile((int)grabTile.X, (int)grabTile.Y+1, Game1.player))
                        grabTile.Y+=1;
                return 1;
            }

            return 5;
        }


        private void Input_MouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            bool flag = Context.IsWorldReady;

            if (!config.ExtendedModeDefault)
                return;
            if (!Context.IsWorldReady)
                return;

            // MouseState mouseState = Mouse.GetState();
            // mouseState.w

            if (this.Helper.Input.IsDown(SButton.LeftControl)||this.Helper.Input.IsDown(SButton.RightControl))
            {
                if (e.OldValue<e.NewValue)
                {
                    if (Game1.options.zoomLevel<=config.MouseWhellingMaxZoom)
                        Game1.options.desiredBaseZoomLevel+=0.05f;
                    Game1.exitActiveMenu();
                    if (!(Game1.player.UsingTool&&(Game1.player.CurrentTool==null||!(Game1.player.CurrentTool is FishingRod fishingRod)||(!fishingRod.isReeling&&!fishingRod.pullingOutOfWater))))
                    {
                        Game1.player.CurrentToolIndex+=1;
                    }
                } else if (e.OldValue>e.NewValue)
                {
                    if (Game1.options.zoomLevel>=config.MouseWhellingMinZoom)
                        Game1.options.desiredBaseZoomLevel-=0.05f;
                    Game1.exitActiveMenu();
                    if (!(Game1.player.UsingTool&&(Game1.player.CurrentTool==null||!(Game1.player.CurrentTool is FishingRod fishingRod)||(!fishingRod.isReeling&&!fishingRod.pullingOutOfWater))))
                    {
                        Game1.player.CurrentToolIndex-=1;
                    }
                }
            }
        }

        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            bool flag = Context.IsWorldReady;

            if (config.RightClickMoveModeDefault)
                if (flag)
                {
                    position_MouseOnScreen.X=Game1.getMousePosition(Game1.uiMode).X;
                    position_MouseOnScreen.Y=Game1.getMousePosition(Game1.uiMode).Y;
                }
        }

        private void InputEvents_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            string button = e.Button.ToString();

            if (config.RightClickMoveModeDefault)
            {
                if (e.Button==Game1.options.runButton[0].ToSButton())
                {
                    isHoldingRunButton=false;
                }
            }
        }

        public static void TryToCheckGrapTile()
        {
            if (isTryToDoActionAtClickedTitle==0)
                return;

            if (Game1.player.isRidingHorse())
            {
                if ((isTryToDoActionAtClickedTitle==2)&&Utility.tileWithinRadiusOfPlayer(pointedNPC.getTileX(), pointedNPC.getTileY(), 2, Game1.player))
                    Game1.player.mount.dismount();
                else if (isTryToDoActionAtClickedTitle!=0&&isTryToDoActionAtClickedTitle!=5&&Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 2, Game1.player))
                    Game1.player.mount.dismount();
            }

            if (isTryToDoActionAtClickedTitle==1&&Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player)&&Game1.tryToCheckAt(grabTile, Game1.player))
            {
                isTryToDoActionAtClickedTitle=0;
                isMovingAutomaticaly=false;
            }

            if ((isTryToDoActionAtClickedTitle==2||isTryToDoActionAtClickedTitle==3)&&(Game1.player.CurrentToolIndex!=currentToolIndex))
            {
                isTryToDoActionAtClickedTitle=0;
            }

            if (isTryToDoActionAtClickedTitle==3&&(Game1.player.ActiveObject==null))
            {
                isTryToDoActionAtClickedTitle=0;
            }

            if (isTryToDoActionAtClickedTitle==2&&Utility.tileWithinRadiusOfPlayer(pointedNPC.getTileX(), pointedNPC.getTileY(), 1, Game1.player)&&Game1.tryToCheckAt(pointedNPC.getTileLocation(), Game1.player))
            {
                isTryToDoActionAtClickedTitle=0;
                isMovingAutomaticaly=false;
            }

            if (isTryToDoActionAtClickedTitle==3&&Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                int stack = Game1.player.ActiveObject.Stack;
                Utility.tryToPlaceItem(Game1.player.currentLocation, Game1.player.ActiveObject, (int)grabTile.X*64+32, (int)grabTile.Y*64+32);
                if (Game1.player.ActiveObject==null||Game1.player.ActiveObject.Stack<stack||Game1.player.ActiveObject.isPlaceable())
                {
                    isTryToDoActionAtClickedTitle=0;
                    // isMovingAutomaticaly=false;
                }
            }

            if (isTryToDoActionAtClickedTitle==4&&Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                Game1.tryToCheckAt(grabTile, Game1.player);
                isTryToDoActionAtClickedTitle=0;
            }

            if (isTryToDoActionAtClickedTitle==5&&Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
            {
                if (!Game1.player.isRidingHorse())
                    Game1.tryToCheckAt(grabTile, Game1.player);
                isTryToDoActionAtClickedTitle=0;
            }
        }

        public static void MoveVectorToCommand()
        {
            bool flag = isMovingAutomaticaly;

            if (flag)
            {
                if (isHoldingMove)
                {
                    vector_AutoMove.X=vector_PlayerToMouse.X;
                    vector_AutoMove.Y=vector_PlayerToMouse.Y;
                } else
                {
                    vector_AutoMove.X=vector_PlayerToDestination.X;
                    vector_AutoMove.Y=vector_PlayerToDestination.Y;
                }

                TryToCheckGrapTile();

                bool flag2 = false;
                bool flag3 = false;

                Game1.player.movementDirections.Clear();
                if (vector_AutoMove.X<=5&&vector_AutoMove.X>=-5)
                {
                    vector_AutoMove.X=0;
                    flag2=true;
                } else if (vector_AutoMove.X>=5)
                    Game1.player.SetMovingRight(true);
                else if (vector_AutoMove.X<=-5)
                    Game1.player.SetMovingLeft(true);

                if (vector_AutoMove.Y<=5&&vector_AutoMove.Y>=-5)
                {
                    vector_AutoMove.Y=0;
                    flag3=true;
                } else if (vector_AutoMove.Y>=5)
                    Game1.player.SetMovingDown(true);
                else if (vector_AutoMove.Y<=-5)
                    Game1.player.SetMovingUp(true);

                vector_AutoMove.Normalize();

                if (flag2&&flag3)
                {
                    isMovingAutomaticaly=false;
                    isTryToDoActionAtClickedTitle=0;
                }
            }
        }

        public static void StartPatching()
        {
            var newHarmony = new Harmony("ylsama.RightClickMoveMode");

            var farmer_Halt_Info = AccessTools.Method(typeof(Farmer), "Halt");
            var farmer_Halt_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_Farmer_HaltPatch");
            newHarmony.Patch(farmer_Halt_Info, new HarmonyMethod(farmer_Halt_PrefixPatch));

            var farmer_getMovementSpeed_Info = AccessTools.Method(typeof(Farmer), "getMovementSpeed");
            var farmer_getMovementSpeed_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_Farmer_getMovementSpeedPatch");
            newHarmony.Patch(farmer_getMovementSpeed_Info, new HarmonyMethod(farmer_getMovementSpeed_PrefixPatch));

            var farmer_MovePosition_Info = AccessTools.Method(typeof(Farmer), "MovePosition", new Type[] { typeof(GameTime), typeof(xTile.Dimensions.Rectangle), typeof(GameLocation) });
            var farmer_MovePosition_PrefixPatch = AccessTools.Method(typeof(ModEntry), "PrefixMethod_Farmer_MovePositionPatch");
            newHarmony.Patch(farmer_MovePosition_Info, new HarmonyMethod(farmer_MovePosition_PrefixPatch));

            var game1_UpdateControlInput_Info = AccessTools.Method(typeof(Game1), "UpdateControlInput", new Type[] { typeof(GameTime) });
            var game1_UpdateControlInput_PostfixPatch = AccessTools.Method(typeof(ModEntry), "PostfixMethod_Game1_UpdateControlInputPatch");
            newHarmony.Patch(game1_UpdateControlInput_Info, null, new HarmonyMethod(game1_UpdateControlInput_PostfixPatch));
        }

        public static bool PrefixMethod_Farmer_HaltPatch()
        {
            // Prefix Method return will control the base method execution
            // true mean base method will exec, false mean the opposite
            if (config.RightClickMoveModeDefault)
            {
                return !isMovingAutomaticaly||isBeingAutoCommand;
            }
            return true;
        }

        public static bool PrefixMethod_Farmer_MovePositionPatch()
        {
            if (config.RightClickMoveModeDefault)
            {
                if (!isBeingControl&&isMovingAutomaticaly&&Context.IsPlayerFree&&Game1.player.CanMove)
                {
                    MovePosition(Game1.currentGameTime, Game1.viewport, Game1.player.currentLocation);
                    return false;
                }
            }
            return true;
        }

        public static void PostfixMethod_Game1_UpdateControlInputPatch()
        {
            if (config.RightClickMoveModeDefault)
            {
                if (!isBeingControl&&Context.IsPlayerFree&&Game1.player.CanMove)
                {
                    isBeingAutoCommand=true;
                    MoveVectorToCommand();

                    if (isHoldingRunButton&&!Game1.player.canOnlyWalk)
                    {
                        Game1.player.setRunning(!Game1.options.autoRun, false);
                        Game1.player.setMoving(Game1.player.running ? (byte)16 : (byte)48);
                    } else if (!isHoldingRunButton&&!Game1.player.canOnlyWalk)
                    {
                        Game1.player.setRunning(Game1.options.autoRun, false);
                        Game1.player.setMoving(Game1.player.running ? (byte)16 : (byte)48);
                    }

                    isBeingAutoCommand=false;
                } else
                    isBeingAutoCommand=false;
            }
        }

        public static bool PrefixMethod_Farmer_getMovementSpeedPatch(ref float __result)
        {
            if (config.RightClickMoveModeDefault)
            {
                if (!isBeingControl&&Context.IsPlayerFree)
                {

                    float movementSpeed;
                    if (Game1.CurrentEvent==null||Game1.CurrentEvent.playerControlSequence)
                    {
                        Game1.player.movementMultiplier=0.066f;
                        movementSpeed=Math.Max(1f, ((float)Game1.player.speed+(Game1.eventUp ? 0f : ((float)Game1.player.addedSpeed+(Game1.player.isRidingHorse() ? 4.6f : Game1.player.temporarySpeedBuff))))*Game1.player.movementMultiplier*(float)Game1.currentGameTime.ElapsedGameTime.Milliseconds);
                    } else
                    {
                        movementSpeed=Math.Max(1f, (float)Game1.player.speed+(Game1.eventUp ? ((float)Math.Max(0, Game1.CurrentEvent.farmerAddedSpeed-2)) : ((float)Game1.player.addedSpeed+(Game1.player.isRidingHorse() ? 5f : Game1.player.temporarySpeedBuff))));
                    }
                    __result=movementSpeed;
                    return false;
                }
            }
            return true;
        }

        public static void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (!Game1.player.IsSitting())
            {
                TimeSpan elapsedGameTime;
                if (Game1.CurrentEvent==null||Game1.CurrentEvent.playerControlSequence)
                {
                    if (Game1.shouldTimePass(false)&&Game1.player.temporarilyInvincible)
                    {
                        if (Game1.player.temporaryInvincibilityTimer<0)
                        {
                            Game1.player.currentTemporaryInvincibilityDuration=1200;
                        }
                        int num = Game1.player.temporaryInvincibilityTimer;
                        elapsedGameTime=time.ElapsedGameTime;
                        Game1.player.temporaryInvincibilityTimer=num+elapsedGameTime.Milliseconds;
                        if (Game1.player.temporaryInvincibilityTimer>Game1.player.currentTemporaryInvincibilityDuration)
                        {
                            Game1.player.temporarilyInvincible=false;
                            Game1.player.temporaryInvincibilityTimer=0;
                        }
                    }
                } else if (Game1.player.temporarilyInvincible)
                {
                    Game1.player.temporarilyInvincible=false;
                    Game1.player.temporaryInvincibilityTimer=0;
                }
                if (Game1.activeClickableMenu!=null)
                {
                    if (Game1.CurrentEvent==null)
                    {
                        return;
                    }
                    if (Game1.CurrentEvent.playerControlSequence)
                    {
                        return;
                    }
                }
                if (Game1.player.isRafting)
                {
                    Game1.player.moveRaft(currentLocation, time);
                } else
                {
                    if (Game1.player.xVelocity!=0f||Game1.player.yVelocity!=0f)
                    {
                        if (double.IsNaN((double)Game1.player.xVelocity)||double.IsNaN((double)Game1.player.yVelocity))
                        {
                            Game1.player.xVelocity=0f;
                            Game1.player.yVelocity=0f;
                        }
                        Rectangle nextPositionFloor = Game1.player.GetBoundingBox();
                        nextPositionFloor.X+=(int)Math.Floor((double)Game1.player.xVelocity);
                        nextPositionFloor.Y-=(int)Math.Floor((double)Game1.player.yVelocity);
                        Rectangle nextPositionCeil = Game1.player.GetBoundingBox();
                        nextPositionCeil.X+=(int)Math.Ceiling((double)Game1.player.xVelocity);
                        nextPositionCeil.Y-=(int)Math.Ceiling((double)Game1.player.yVelocity);
                        Rectangle nextPosition = Rectangle.Union(nextPositionFloor, nextPositionCeil);
                        if (!currentLocation.isCollidingPosition(nextPosition, viewport, true, -1, false, Game1.player))
                        {
                            Game1.player.position.X+=Game1.player.xVelocity;
                            Game1.player.position.Y-=Game1.player.yVelocity;
                            Game1.player.xVelocity-=Game1.player.xVelocity/16f;
                            Game1.player.yVelocity-=Game1.player.yVelocity/16f;
                            if (Math.Abs(Game1.player.xVelocity)<=0.05f)
                            {
                                Game1.player.xVelocity=0f;
                            }
                            if (Math.Abs(Game1.player.yVelocity)<=0.05f)
                            {
                                Game1.player.yVelocity=0f;
                            }
                        } else
                        {
                            Game1.player.xVelocity-=Game1.player.xVelocity/16f;
                            Game1.player.yVelocity-=Game1.player.yVelocity/16f;
                            if (Math.Abs(Game1.player.xVelocity)<=0.05f)
                            {
                                Game1.player.xVelocity=0f;
                            }
                            if (Math.Abs(Game1.player.yVelocity)<=0.05f)
                            {
                                Game1.player.yVelocity=0f;
                            }
                        }
                    }

                    if (Game1.player.CanMove||Game1.eventUp||Game1.player.controller!=null)
                    {
                        Game1.player.TemporaryPassableTiles.ClearNonIntersecting(Game1.player.GetBoundingBox());

                        Game1.player.temporarySpeedBuff=0f;

                        if (Game1.player.movementDirections.Contains(0))
                            TryMoveDrection(time, viewport, currentLocation, 0);

                        if (Game1.player.movementDirections.Contains(2))
                            TryMoveDrection(time, viewport, currentLocation, 2);

                        if (Game1.player.movementDirections.Contains(1))
                            TryMoveDrection(time, viewport, currentLocation, 1);

                        if (Game1.player.movementDirections.Contains(3))
                            TryMoveDrection(time, viewport, currentLocation, 3);

                        if (Game1.player.movementDirections.Count==2)
                        {
                            if (Math.Abs(vector_AutoMove.Y/vector_AutoMove.X).CompareTo(0.45f)<0)
                            {
                                Game1.player.SetMovingDown(false);
                                Game1.player.SetMovingUp(false);
                            } else if (Math.Abs(vector_AutoMove.Y)>Math.Sin(Math.PI/3))
                            {
                                Game1.player.SetMovingRight(false);
                                Game1.player.SetMovingLeft(false);
                            }
                        }
                    }

                    if (Game1.player.movementDirections.Count>0&&!Game1.player.UsingTool)
                    {
                        Game1.player.FarmerSprite.intervalModifier=1f-(Game1.player.running ? 0.03f : 0.025f)*(Math.Max(1f, ((float)Game1.player.speed+(Game1.eventUp ? 0f : ((float)Game1.player.addedSpeed+(Game1.player.isRidingHorse() ? 4.6f : 0f))))*Game1.player.movementMultiplier*(float)Game1.currentGameTime.ElapsedGameTime.Milliseconds)*1.25f);
                    } else
                    {
                        Game1.player.FarmerSprite.intervalModifier=1f;
                    }
                    if (Game1.player.temporarilyInvincible)
                    {
                        Game1.player.temporaryInvincibilityTimer+=time.ElapsedGameTime.Milliseconds;
                        if (Game1.player.temporaryInvincibilityTimer>1200)
                        {
                            Game1.player.temporarilyInvincible=false;
                            Game1.player.temporaryInvincibilityTimer=0;
                        }
                    }
                    if (currentLocation!=null&&currentLocation.isFarmerCollidingWithAnyCharacter())
                    {
                        Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int)Game1.player.getTileLocation().X*64, (int)Game1.player.getTileLocation().Y*64, 64, 64));
                    }
                }
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
            Warp warp = Game1.currentLocation.isCollidingWithWarp(Game1.player.nextPosition(faceDirection), Game1.player);
            if (warp!=null&&Game1.player.IsLocalPlayer)
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
                    if (faceDirection==0||faceDirection==2)
                        Game1.player.position.Y+=movementSpeed*vector_AutoMove.Y;
                    else
                        Game1.player.position.X+=movementSpeed*vector_AutoMove.X;

                    Game1.player.behaviorOnMovement(faceDirection);
                } else
                {
                    nextPos=Game1.player.nextPositionHalf(faceDirection);

                    if (!currentLocation.isCollidingPosition(nextPos, viewport, true, 0, false, Game1.player))
                    {

                        if (faceDirection==0||faceDirection==2)
                            Game1.player.position.Y+=movementSpeed*vector_AutoMove.Y/2f;
                        else
                            Game1.player.position.X+=movementSpeed*vector_AutoMove.X/2f;

                        Game1.player.behaviorOnMovement(faceDirection);
                    } else if (Game1.player.movementDirections.Count==1)
                    {
                        Rectangle tmp = Game1.player.nextPosition(faceDirection);
                        tmp.Width/=4;
                        bool leftCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                        tmp.X+=tmp.Width*3;
                        bool rightCorner = currentLocation.isCollidingPosition(tmp, viewport, true, 0, false, Game1.player);
                        if (leftCorner&&!rightCorner&&!currentLocation.isCollidingPosition(Game1.player.nextPosition(LeftDirection(faceDirection)), viewport, true, 0, false, Game1.player))
                        {
                            if (faceDirection==0||faceDirection==2)
                                Game1.player.position.X+=(float)Game1.player.speed*((float)time.ElapsedGameTime.Milliseconds/64f);
                            else
                                Game1.player.position.Y+=(float)Game1.player.speed*((float)time.ElapsedGameTime.Milliseconds/64f);
                        } else if (rightCorner&&!leftCorner&&!currentLocation.isCollidingPosition(Game1.player.nextPosition(RightDirection(faceDirection)), viewport, true, 0, false, Game1.player))
                        {
                            if (faceDirection==0||faceDirection==2)
                                Game1.player.position.X-=(float)Game1.player.speed*((float)time.ElapsedGameTime.Milliseconds/64f);
                            else
                                Game1.player.position.Y-=(float)Game1.player.speed*((float)time.ElapsedGameTime.Milliseconds/64f);
                        }
                    }
                }
            }
        }
    }
}
