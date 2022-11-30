/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sagittaeri/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace InputTools
{
    public class UpdateControlInput
    {
        // From Game1.cs UpdateControlInput line 16330
        /*
        public void Replace_UpdateControlInput(GameTime time)
        {
            KeyboardState currentKBState = Game1.GetKeyboardState();
            MouseState currentMouseState = Game1.input.GetMouseState();
            GamePadState currentPadState = Game1.input.GetGamePadState();
            if (Game1.ticks < Game1._activatedTick + 2 && Game1.oldKBState.IsKeyDown(Keys.Tab) != currentKBState.IsKeyDown(Keys.Tab))
            {
                List<Keys> keys = Game1.oldKBState.GetPressedKeys().ToList();
                if (currentKBState.IsKeyDown(Keys.Tab))
                {
                    keys.Add(Keys.Tab);
                }
                else
                {
                    keys.Remove(Keys.Tab);
                }
                Game1.oldKBState = new KeyboardState(keys.ToArray());
            }

            Game1.hooks.OnGame1_UpdateControlInput(ref currentKBState, ref currentMouseState, ref currentPadState, delegate
            {
                if (Game1.options.gamepadControls)
                {
                    bool flag = false;
                    if (Math.Abs(currentPadState.ThumbSticks.Right.X) > 0f || Math.Abs(currentPadState.ThumbSticks.Right.Y) > 0f)
                    {
                        Game1.setMousePositionRaw((int)((float)currentMouseState.X + currentPadState.ThumbSticks.Right.X * Game1.thumbstickToMouseModifier), (int)((float)currentMouseState.Y - currentPadState.ThumbSticks.Right.Y * Game1.thumbstickToMouseModifier));
                        flag = true;
                    }
                    if (Game1.IsChatting)
                    {
                        flag = true;
                    }
                    if (((Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY()) && Game1.getMouseX() != 0 && Game1.getMouseY() != 0) || flag)
                    {
                        if (flag)
                        {
                            if (Game1.timerUntilMouseFade <= 0)
                            {
                                Game1.lastMousePositionBeforeFade = new Point(this.localMultiplayerWindow.Width / 2, this.localMultiplayerWindow.Height / 2);
                            }
                        }
                        else
                        {
                            Game1.lastCursorMotionWasMouse = true;
                        }
                        if (Game1.timerUntilMouseFade <= 0 && !Game1.lastCursorMotionWasMouse)
                        {
                            Game1.setMousePositionRaw(Game1.lastMousePositionBeforeFade.X, Game1.lastMousePositionBeforeFade.Y);
                        }
                        Game1.timerUntilMouseFade = 4000;
                    }
                }
                else if (Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY())
                {
                    Game1.lastCursorMotionWasMouse = true;
                }
                bool actionButtonPressed = false;
                bool switchToolButtonPressed = false;
                bool useToolButtonPressed = false;
                bool useToolButtonReleased = false;
                bool addItemToInventoryButtonPressed = false;
                bool cancelButtonPressed = false;
                bool moveUpPressed = false;
                bool moveRightPressed = false;
                bool moveLeftPressed = false;
                bool moveDownPressed = false;
                bool moveUpReleased = false;
                bool moveRightReleased = false;
                bool moveDownReleased = false;
                bool moveLeftReleased = false;
                bool moveUpHeld = false;
                bool moveRightHeld = false;
                bool moveDownHeld = false;
                bool moveLeftHeld = false;
                bool flag2 = false;
                if ((Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.actionButton)) || (currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released))
                {
                    actionButtonPressed = true;
                    Game1.rightClickPolling = 250;
                }
                if ((Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.useToolButton)) || (currentMouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released))
                {
                    useToolButtonPressed = true;
                }
                if ((Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.useToolButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton)) || (currentMouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed))
                {
                    useToolButtonReleased = true;
                }
                if (currentMouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue)
                {
                    switchToolButtonPressed = true;
                }
                if ((Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.cancelButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.cancelButton)) || (currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released))
                {
                    cancelButtonPressed = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton))
                {
                    moveUpPressed = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton))
                {
                    moveRightPressed = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton))
                {
                    moveDownPressed = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton))
                {
                    moveLeftPressed = true;
                }
                if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveUpButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
                {
                    moveUpReleased = true;
                }
                if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveRightButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
                {
                    moveRightReleased = true;
                }
                if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveDownButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
                {
                    moveDownReleased = true;
                }
                if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveLeftButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
                {
                    moveLeftReleased = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton))
                {
                    moveUpHeld = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton))
                {
                    moveRightHeld = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton))
                {
                    moveDownHeld = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton))
                {
                    moveLeftHeld = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    flag2 = true;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) || currentMouseState.RightButton == ButtonState.Pressed)
                {
                    Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
                    if (Game1.rightClickPolling <= 0)
                    {
                        Game1.rightClickPolling = 100;
                        actionButtonPressed = true;
                    }
                }
                if (Game1.options.gamepadControls)
                {
                    if (currentKBState.GetPressedKeys().Length != 0 || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
                    {
                        Game1.timerUntilMouseFade = 4000;
                    }
                    if (currentPadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
                    {
                        actionButtonPressed = true;
                        Game1.lastCursorMotionWasMouse = false;
                        Game1.rightClickPolling = 250;
                    }
                    if (currentPadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
                    {
                        useToolButtonPressed = true;
                        Game1.lastCursorMotionWasMouse = false;
                    }
                    if (!currentPadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
                    {
                        useToolButtonReleased = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.RightTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.RightTrigger))
                    {
                        switchToolButtonPressed = true;
                        Game1.triggerPolling = 300;
                    }
                    else if (currentPadState.IsButtonDown(Buttons.LeftTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.LeftTrigger))
                    {
                        switchToolButtonPressed = true;
                        Game1.triggerPolling = 300;
                    }
                    if (currentPadState.IsButtonDown(Buttons.X))
                    {
                        flag2 = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.A))
                    {
                        Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
                        if (Game1.rightClickPolling <= 0)
                        {
                            Game1.rightClickPolling = 100;
                            actionButtonPressed = true;
                        }
                    }
                    if (currentPadState.IsButtonDown(Buttons.RightTrigger) || currentPadState.IsButtonDown(Buttons.LeftTrigger))
                    {
                        Game1.triggerPolling -= time.ElapsedGameTime.Milliseconds;
                        if (Game1.triggerPolling <= 0)
                        {
                            Game1.triggerPolling = 100;
                            switchToolButtonPressed = true;
                        }
                    }
                    if (currentPadState.IsButtonDown(Buttons.RightShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.RightShoulder))
                    {
                        Game1.player.shiftToolbar(true);
                    }
                    if (currentPadState.IsButtonDown(Buttons.LeftShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.LeftShoulder))
                    {
                        Game1.player.shiftToolbar(false);
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadUp) && !Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
                    {
                        moveUpPressed = true;
                    }
                    else if (!currentPadState.IsButtonDown(Buttons.DPadUp) && Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
                    {
                        moveUpReleased = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadRight) && !Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
                    {
                        moveRightPressed = true;
                    }
                    else if (!currentPadState.IsButtonDown(Buttons.DPadRight) && Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
                    {
                        moveRightReleased = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadDown) && !Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
                    {
                        moveDownPressed = true;
                    }
                    else if (!currentPadState.IsButtonDown(Buttons.DPadDown) && Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
                    {
                        moveDownReleased = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadLeft) && !Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
                    {
                        moveLeftPressed = true;
                    }
                    else if (!currentPadState.IsButtonDown(Buttons.DPadLeft) && Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
                    {
                        moveLeftReleased = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadUp))
                    {
                        moveUpHeld = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadRight))
                    {
                        moveRightHeld = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadDown))
                    {
                        moveDownHeld = true;
                    }
                    if (currentPadState.IsButtonDown(Buttons.DPadLeft))
                    {
                        moveLeftHeld = true;
                    }
                    if ((double)currentPadState.ThumbSticks.Left.X < -0.2)
                    {
                        moveLeftPressed = true;
                        moveLeftHeld = true;
                    }
                    else if ((double)currentPadState.ThumbSticks.Left.X > 0.2)
                    {
                        moveRightPressed = true;
                        moveRightHeld = true;
                    }
                    if ((double)currentPadState.ThumbSticks.Left.Y < -0.2)
                    {
                        moveDownPressed = true;
                        moveDownHeld = true;
                    }
                    else if ((double)currentPadState.ThumbSticks.Left.Y > 0.2)
                    {
                        moveUpPressed = true;
                        moveUpHeld = true;
                    }
                    if ((double)Game1.oldPadState.ThumbSticks.Left.X < -0.2 && !moveLeftHeld)
                    {
                        moveLeftReleased = true;
                    }
                    if ((double)Game1.oldPadState.ThumbSticks.Left.X > 0.2 && !moveRightHeld)
                    {
                        moveRightReleased = true;
                    }
                    if ((double)Game1.oldPadState.ThumbSticks.Left.Y < -0.2 && !moveDownHeld)
                    {
                        moveDownReleased = true;
                    }
                    if ((double)Game1.oldPadState.ThumbSticks.Left.Y > 0.2 && !moveUpHeld)
                    {
                        moveUpReleased = true;
                    }
                    if (this.controllerSlingshotSafeTime > 0f)
                    {
                        if (!currentPadState.IsButtonDown(Buttons.DPadUp) && !currentPadState.IsButtonDown(Buttons.DPadDown) && !currentPadState.IsButtonDown(Buttons.DPadLeft) && !currentPadState.IsButtonDown(Buttons.DPadRight) && (double)Math.Abs(currentPadState.ThumbSticks.Left.X) < 0.04 && (double)Math.Abs(currentPadState.ThumbSticks.Left.Y) < 0.04)
                        {
                            this.controllerSlingshotSafeTime = 0f;
                        }
                        if (this.controllerSlingshotSafeTime <= 0f)
                        {
                            this.controllerSlingshotSafeTime = 0f;
                        }
                        else
                        {
                            this.controllerSlingshotSafeTime -= (float)time.ElapsedGameTime.TotalSeconds;
                            moveUpPressed = false;
                            moveDownPressed = false;
                            moveLeftPressed = false;
                            moveRightPressed = false;
                            moveUpHeld = false;
                            moveDownHeld = false;
                            moveLeftHeld = false;
                            moveRightHeld = false;
                        }
                    }
                }
                else
                {
                    this.controllerSlingshotSafeTime = 0f;
                }
                Game1.ResetFreeCursorDrag();
                if (flag2)
                {
                    Game1.mouseClickPolling += time.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    Game1.mouseClickPolling = 0;
                }
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.toolbarSwap) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.toolbarSwap))
                {
                    Game1.player.shiftToolbar((!currentKBState.IsKeyDown(Keys.LeftControl)) ? true : false);
                }
                if (Game1.mouseClickPolling > 250 && (Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is FishingRod) || (int)Game1.player.CurrentTool.upgradeLevel <= 0))
                {
                    useToolButtonPressed = true;
                    Game1.mouseClickPolling = 100;
                }
                Game1.PushUIMode();
                foreach (IClickableMenu current in Game1.onScreenMenus)
                {
                    if ((Game1.displayHUD || current == Game1.chatBox) && Game1.wasMouseVisibleThisFrame && current.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        current.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
                    }
                }
                Game1.PopUIMode();
                if (Game1.chatBox != null && Game1.chatBox.chatBox.Selected && Game1.oldMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
                {
                    Game1.chatBox.receiveScrollWheelAction(currentMouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
                }
                if (Game1.panMode)
                {
                    this.updatePanModeControls(currentMouseState, currentKBState);
                }
                else
                {
                    if (Game1.inputSimulator != null)
                    {
                        if (currentKBState.IsKeyDown(Keys.Escape))
                        {
                            Game1.inputSimulator = null;
                        }
                        else
                        {
                            Game1.inputSimulator.SimulateInput(ref actionButtonPressed, ref switchToolButtonPressed, ref useToolButtonPressed, ref useToolButtonReleased, ref addItemToInventoryButtonPressed, ref cancelButtonPressed, ref moveUpPressed, ref moveRightPressed, ref moveLeftPressed, ref moveDownPressed, ref moveUpReleased, ref moveRightReleased, ref moveLeftReleased, ref moveDownReleased, ref moveUpHeld, ref moveRightHeld, ref moveLeftHeld, ref moveDownHeld);
                        }
                    }
                    if (useToolButtonReleased && Game1.player.CurrentTool != null && Game1.CurrentEvent == null && Game1.pauseTime <= 0f && Game1.player.CurrentTool.onRelease(Game1.currentLocation, Game1.getMouseX(), Game1.getMouseY(), Game1.player))
                    {
                        Game1.oldMouseState = Game1.input.GetMouseState();
                        Game1.oldKBState = currentKBState;
                        Game1.oldPadState = currentPadState;
                        Game1.player.usingSlingshot = false;
                        Game1.player.canReleaseTool = true;
                        Game1.player.UsingTool = false;
                        Game1.player.CanMove = true;
                    }
                    else
                    {
                        if (((useToolButtonPressed && !Game1.isAnyGamePadButtonBeingPressed()) || (actionButtonPressed && Game1.isAnyGamePadButtonBeingPressed())) && Game1.pauseTime <= 0f && Game1.wasMouseVisibleThisFrame)
                        {
                            Game1.PushUIMode();
                            foreach (IClickableMenu current2 in Game1.onScreenMenus)
                            {
                                if (Game1.displayHUD || current2 == Game1.chatBox)
                                {
                                    if ((!Game1.IsChatting || current2 == Game1.chatBox) && (!(current2 is LevelUpMenu) || (current2 as LevelUpMenu).informationUp) && current2.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
                                    {
                                        current2.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                                        Game1.PopUIMode();
                                        Game1.oldMouseState = Game1.input.GetMouseState();
                                        Game1.oldKBState = currentKBState;
                                        Game1.oldPadState = currentPadState;
                                        return;
                                    }
                                    if (current2 == Game1.chatBox && Game1.options.gamepadControls && Game1.IsChatting)
                                    {
                                        Game1.oldMouseState = Game1.input.GetMouseState();
                                        Game1.oldKBState = currentKBState;
                                        Game1.oldPadState = currentPadState;
                                        Game1.PopUIMode();
                                        return;
                                    }
                                    current2.clickAway();
                                }
                            }
                            Game1.PopUIMode();
                        }
                        if (Game1.IsChatting || Game1.player.freezePause > 0)
                        {
                            if (Game1.IsChatting)
                            {
                                ButtonCollection.ButtonEnumerator enumerator2 = Utility.getPressedButtons(currentPadState, Game1.oldPadState).GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    Buttons current3 = enumerator2.Current;
                                    Game1.chatBox.receiveGamePadButton(current3);
                                }
                            }
                            Game1.oldMouseState = Game1.input.GetMouseState();
                            Game1.oldKBState = currentKBState;
                            Game1.oldPadState = currentPadState;
                        }
                        else
                        {
                            if (Game1.paused || Game1.HostPaused)
                            {
                                if (!Game1.HostPaused || !Game1.IsMasterGame || (!Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) && !currentPadState.IsButtonDown(Buttons.B) && !currentPadState.IsButtonDown(Buttons.Back)))
                                {
                                    Game1.oldMouseState = Game1.input.GetMouseState();
                                    return;
                                }
                                Game1.netWorldState.Value.IsPaused = false;
                                if (Game1.chatBox != null)
                                {
                                    Game1.chatBox.globalInfoMessage("Resumed");
                                }
                            }
                            if (Game1.eventUp)
                            {
                                if (Game1.currentLocation.currentEvent == null && Game1.locationRequest == null)
                                {
                                    Game1.eventUp = false;
                                }
                                else if (actionButtonPressed || useToolButtonPressed)
                                {
                                    Event currentEvent = Game1.CurrentEvent;
                                    if (currentEvent != null)
                                    {
                                        currentEvent.receiveMouseClick(Game1.getMouseX(), Game1.getMouseY());
                                    }
                                }
                            }
                            bool flag3 = Game1.eventUp || Game1.farmEvent != null;
                            if (actionButtonPressed || (Game1.dialogueUp && useToolButtonPressed))
                            {
                                Game1.PushUIMode();
                                foreach (IClickableMenu current4 in Game1.onScreenMenus)
                                {
                                    if (Game1.wasMouseVisibleThisFrame && (Game1.displayHUD || current4 == Game1.chatBox) && current4.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && (!(current4 is LevelUpMenu) || (current4 as LevelUpMenu).informationUp))
                                    {
                                        current4.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                                        Game1.oldMouseState = Game1.input.GetMouseState();
                                        if (!Game1.isAnyGamePadButtonBeingPressed())
                                        {
                                            Game1.PopUIMode();
                                            Game1.oldKBState = currentKBState;
                                            Game1.oldPadState = currentPadState;
                                            return;
                                        }
                                    }
                                }
                                Game1.PopUIMode();
                                if (!Game1.pressActionButton(currentKBState, currentMouseState, currentPadState))
                                {
                                    Game1.oldKBState = currentKBState;
                                    Game1.oldMouseState = Game1.input.GetMouseState();
                                    Game1.oldPadState = currentPadState;
                                    return;
                                }
                            }
                            if (useToolButtonPressed && (!Game1.player.UsingTool || (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon)) && !Game1.player.isEating && !Game1.pickingTool && !Game1.dialogueUp && !Game1.menuUp && Game1.farmEvent == null && (Game1.player.CanMove || (Game1.player.CurrentTool != null && (Game1.player.CurrentTool.Name.Equals("Fishing Rod") || Game1.player.CurrentTool is MeleeWeapon))))
                            {
                                if (Game1.player.CurrentTool != null && (!(Game1.player.CurrentTool is MeleeWeapon) || Game1.didPlayerJustLeftClick(true)))
                                {
                                    Game1.player.FireTool();
                                }
                                if (!Game1.pressUseToolButton() && Game1.player.canReleaseTool && Game1.player.UsingTool)
                                {
                                    Tool currentTool = Game1.player.CurrentTool;
                                }
                                if (Game1.player.UsingTool)
                                {
                                    Game1.oldMouseState = Game1.input.GetMouseState();
                                    Game1.oldKBState = currentKBState;
                                    Game1.oldPadState = currentPadState;
                                    return;
                                }
                            }
                            if (useToolButtonReleased && this._didInitiateItemStow)
                            {
                                this._didInitiateItemStow = false;
                            }
                            if (useToolButtonReleased && Game1.player.canReleaseTool && Game1.player.UsingTool && Game1.player.CurrentTool != null)
                            {
                                Game1.player.EndUsingTool();
                            }
                            if (switchToolButtonPressed && !Game1.player.UsingTool && !Game1.dialogueUp && (Game1.pickingTool || Game1.player.CanMove) && !Game1.player.areAllItemsNull() && !flag3)
                            {
                                Game1.pressSwitchToolButton();
                            }
                            if (cancelButtonPressed)
                            {
                                if (Game1.numberOfSelectedItems != -1)
                                {
                                    Game1.numberOfSelectedItems = -1;
                                    Game1.dialogueUp = false;
                                    Game1.player.CanMove = true;
                                }
                                else if (Game1.nameSelectUp && NameSelect.cancel())
                                {
                                    Game1.nameSelectUp = false;
                                    Game1.playSound("bigDeSelect");
                                }
                            }
                            if (Game1.player.CurrentTool != null && flag2 && Game1.player.canReleaseTool && !flag3 && !Game1.dialogueUp && !Game1.menuUp && Game1.player.Stamina >= 1f && !(Game1.player.CurrentTool is FishingRod))
                            {
                                int num = (Game1.player.CurrentTool.hasEnchantmentOfType<ReachingToolEnchantment>() ? 1 : 0);
                                if (Game1.player.toolHold <= 0 && (int)Game1.player.CurrentTool.upgradeLevel + num > Game1.player.toolPower)
                                {
                                    float num2 = 1f;
                                    if (Game1.player.CurrentTool != null)
                                    {
                                        num2 = Game1.player.CurrentTool.AnimationSpeedModifier;
                                    }
                                    Game1.player.toolHold = (int)(600f * num2);
                                }
                                else if ((int)Game1.player.CurrentTool.upgradeLevel + num > Game1.player.toolPower)
                                {
                                    Game1.player.toolHold -= time.ElapsedGameTime.Milliseconds;
                                    if (Game1.player.toolHold <= 0)
                                    {
                                        Game1.player.toolPowerIncrease();
                                    }
                                }
                            }
                            if (Game1.upPolling >= 650f)
                            {
                                moveUpPressed = true;
                                Game1.upPolling -= 100f;
                            }
                            else if (Game1.downPolling >= 650f)
                            {
                                moveDownPressed = true;
                                Game1.downPolling -= 100f;
                            }
                            else if (Game1.rightPolling >= 650f)
                            {
                                moveRightPressed = true;
                                Game1.rightPolling -= 100f;
                            }
                            else if (Game1.leftPolling >= 650f)
                            {
                                moveLeftPressed = true;
                                Game1.leftPolling -= 100f;
                            }
                            else if (!Game1.nameSelectUp && Game1.pauseTime <= 0f && Game1.locationRequest == null && !Game1.player.UsingTool && (!flag3 || (Game1.CurrentEvent != null && Game1.CurrentEvent.playerControlSequence)))
                            {
                                if (Game1.player.movementDirections.Count < 2)
                                {
                                    int count = Game1.player.movementDirections.Count;
                                    if (moveUpHeld)
                                    {
                                        Game1.player.setMoving(1);
                                    }
                                    if (moveRightHeld)
                                    {
                                        Game1.player.setMoving(2);
                                    }
                                    if (moveDownHeld)
                                    {
                                        Game1.player.setMoving(4);
                                    }
                                    if (moveLeftHeld)
                                    {
                                        Game1.player.setMoving(8);
                                    }
                                }
                                if (moveUpReleased || (Game1.player.movementDirections.Contains(0) && !moveUpHeld))
                                {
                                    Game1.player.setMoving(33);
                                    if (Game1.player.movementDirections.Count == 0)
                                    {
                                        Game1.player.setMoving(64);
                                    }
                                }
                                if (moveRightReleased || (Game1.player.movementDirections.Contains(1) && !moveRightHeld))
                                {
                                    Game1.player.setMoving(34);
                                    if (Game1.player.movementDirections.Count == 0)
                                    {
                                        Game1.player.setMoving(64);
                                    }
                                }
                                if (moveDownReleased || (Game1.player.movementDirections.Contains(2) && !moveDownHeld))
                                {
                                    Game1.player.setMoving(36);
                                    if (Game1.player.movementDirections.Count == 0)
                                    {
                                        Game1.player.setMoving(64);
                                    }
                                }
                                if (moveLeftReleased || (Game1.player.movementDirections.Contains(3) && !moveLeftHeld))
                                {
                                    Game1.player.setMoving(40);
                                    if (Game1.player.movementDirections.Count == 0)
                                    {
                                        Game1.player.setMoving(64);
                                    }
                                }
                                if ((!moveUpHeld && !moveRightHeld && !moveDownHeld && !moveLeftHeld && !Game1.player.UsingTool) || Game1.activeClickableMenu != null)
                                {
                                    Game1.player.Halt();
                                }
                            }
                            else if (Game1.isQuestion)
                            {
                                if (moveUpPressed)
                                {
                                    Game1.currentQuestionChoice = Math.Max(Game1.currentQuestionChoice - 1, 0);
                                    Game1.playSound("toolSwap");
                                }
                                else if (moveDownPressed)
                                {
                                    Game1.currentQuestionChoice = Math.Min(Game1.currentQuestionChoice + 1, Game1.questionChoices.Count - 1);
                                    Game1.playSound("toolSwap");
                                }
                            }
                            else if (Game1.numberOfSelectedItems != -1 && !Game1.dialogueTyping)
                            {
                                int val = 99;
                                if (Game1.selectedItemsType.Equals("Animal Food"))
                                {
                                    val = 999 - Game1.player.Feed;
                                }
                                else if (Game1.selectedItemsType.Equals("calicoJackBet"))
                                {
                                    val = Math.Min(Game1.player.clubCoins, 999);
                                }
                                else if (Game1.selectedItemsType.Equals("flutePitch"))
                                {
                                    val = 26;
                                }
                                else if (Game1.selectedItemsType.Equals("drumTone"))
                                {
                                    val = 6;
                                }
                                else if (Game1.selectedItemsType.Equals("jukebox"))
                                {
                                    val = Game1.player.songsHeard.Count - 1;
                                }
                                else if (Game1.selectedItemsType.Equals("Fuel"))
                                {
                                    val = 100 - ((Lantern)Game1.player.getToolFromName("Lantern")).fuelLeft;
                                }
                                if (moveRightPressed)
                                {
                                    Game1.numberOfSelectedItems = Math.Min(Game1.numberOfSelectedItems + 1, val);
                                    Game1.playItemNumberSelectSound();
                                }
                                else if (moveLeftPressed)
                                {
                                    Game1.numberOfSelectedItems = Math.Max(Game1.numberOfSelectedItems - 1, 0);
                                    Game1.playItemNumberSelectSound();
                                }
                                else if (moveUpPressed)
                                {
                                    Game1.numberOfSelectedItems = Math.Min(Game1.numberOfSelectedItems + 10, val);
                                    Game1.playItemNumberSelectSound();
                                }
                                else if (moveDownPressed)
                                {
                                    Game1.numberOfSelectedItems = Math.Max(Game1.numberOfSelectedItems - 10, 0);
                                    Game1.playItemNumberSelectSound();
                                }
                            }
                            if (moveUpHeld && !Game1.player.CanMove)
                            {
                                Game1.upPolling += time.ElapsedGameTime.Milliseconds;
                            }
                            else if (moveDownHeld && !Game1.player.CanMove)
                            {
                                Game1.downPolling += time.ElapsedGameTime.Milliseconds;
                            }
                            else if (moveRightHeld && !Game1.player.CanMove)
                            {
                                Game1.rightPolling += time.ElapsedGameTime.Milliseconds;
                            }
                            else if (moveLeftHeld && !Game1.player.CanMove)
                            {
                                Game1.leftPolling += time.ElapsedGameTime.Milliseconds;
                            }
                            else if (moveUpReleased)
                            {
                                Game1.upPolling = 0f;
                            }
                            else if (moveDownReleased)
                            {
                                Game1.downPolling = 0f;
                            }
                            else if (moveRightReleased)
                            {
                                Game1.rightPolling = 0f;
                            }
                            else if (moveLeftReleased)
                            {
                                Game1.leftPolling = 0f;
                            }
                            if (Game1.debugMode)
                            {
                                if (currentKBState.IsKeyDown(Keys.Q))
                                {
                                    Game1.oldKBState.IsKeyDown(Keys.Q);
                                }
                                if (currentKBState.IsKeyDown(Keys.P) && !Game1.oldKBState.IsKeyDown(Keys.P))
                                {
                                    Game1.NewDay(0f);
                                }
                                if (currentKBState.IsKeyDown(Keys.M) && !Game1.oldKBState.IsKeyDown(Keys.M))
                                {
                                    Game1.dayOfMonth = 28;
                                    Game1.NewDay(0f);
                                }
                                if (currentKBState.IsKeyDown(Keys.T) && !Game1.oldKBState.IsKeyDown(Keys.T))
                                {
                                    Game1.addHour();
                                }
                                if (currentKBState.IsKeyDown(Keys.Y) && !Game1.oldKBState.IsKeyDown(Keys.Y))
                                {
                                    Game1.addMinute();
                                }
                                if (currentKBState.IsKeyDown(Keys.D1) && !Game1.oldKBState.IsKeyDown(Keys.D1))
                                {
                                    Game1.warpFarmer("Mountain", 15, 35, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D2) && !Game1.oldKBState.IsKeyDown(Keys.D2))
                                {
                                    Game1.warpFarmer("Town", 35, 35, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D3) && !Game1.oldKBState.IsKeyDown(Keys.D3))
                                {
                                    Game1.warpFarmer("Farm", 64, 15, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D4) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                                {
                                    Game1.warpFarmer("Forest", 34, 13, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D5) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                                {
                                    Game1.warpFarmer("Beach", 34, 10, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D6) && !Game1.oldKBState.IsKeyDown(Keys.D6))
                                {
                                    Game1.warpFarmer("Mine", 18, 12, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.D7) && !Game1.oldKBState.IsKeyDown(Keys.D7))
                                {
                                    Game1.warpFarmer("SandyHouse", 16, 3, false);
                                }
                                if (currentKBState.IsKeyDown(Keys.K) && !Game1.oldKBState.IsKeyDown(Keys.K))
                                {
                                    Game1.enterMine(Game1.mine.mineLevel + 1);
                                }
                                if (currentKBState.IsKeyDown(Keys.H) && !Game1.oldKBState.IsKeyDown(Keys.H))
                                {
                                    Game1.player.changeHat(Game1.random.Next(FarmerRenderer.hatsTexture.Height / 80 * 12));
                                }
                                if (currentKBState.IsKeyDown(Keys.I) && !Game1.oldKBState.IsKeyDown(Keys.I))
                                {
                                    Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 * 8));
                                }
                                if (currentKBState.IsKeyDown(Keys.J) && !Game1.oldKBState.IsKeyDown(Keys.J))
                                {
                                    Game1.player.changeShirt(Game1.random.Next(40));
                                    Game1.player.changePants(new Color(Game1.random.Next(255), Game1.random.Next(255), Game1.random.Next(255)));
                                }
                                if (currentKBState.IsKeyDown(Keys.L) && !Game1.oldKBState.IsKeyDown(Keys.L))
                                {
                                    Game1.player.changeShirt(Game1.random.Next(40));
                                    Game1.player.changePants(new Color(Game1.random.Next(255), Game1.random.Next(255), Game1.random.Next(255)));
                                    Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 * 8));
                                    if (Game1.random.NextDouble() < 0.5)
                                    {
                                        Game1.player.changeHat(Game1.random.Next(-1, FarmerRenderer.hatsTexture.Height / 80 * 12));
                                    }
                                    else
                                    {
                                        Game1.player.changeHat(-1);
                                    }
                                    Game1.player.changeHairColor(new Color(Game1.random.Next(255), Game1.random.Next(255), Game1.random.Next(255)));
                                    Game1.player.changeSkinColor(Game1.random.Next(16));
                                }
                                if (currentKBState.IsKeyDown(Keys.U) && !Game1.oldKBState.IsKeyDown(Keys.U))
                                {
                                    (Game1.getLocationFromName("FarmHouse") as FarmHouse).setWallpaper(Game1.random.Next(112), -1, true);
                                    (Game1.getLocationFromName("FarmHouse") as FarmHouse).setFloor(Game1.random.Next(40), -1, true);
                                }
                                if (currentKBState.IsKeyDown(Keys.F2))
                                {
                                    Game1.oldKBState.IsKeyDown(Keys.F2);
                                }
                                if (currentKBState.IsKeyDown(Keys.F5) && !Game1.oldKBState.IsKeyDown(Keys.F5))
                                {
                                    Game1.displayFarmer = !Game1.displayFarmer;
                                }
                                if (currentKBState.IsKeyDown(Keys.F6))
                                {
                                    Game1.oldKBState.IsKeyDown(Keys.F6);
                                }
                                if (currentKBState.IsKeyDown(Keys.F7) && !Game1.oldKBState.IsKeyDown(Keys.F7))
                                {
                                    Game1.drawGrid = !Game1.drawGrid;
                                }
                                if (currentKBState.IsKeyDown(Keys.B) && !Game1.oldKBState.IsKeyDown(Keys.B))
                                {
                                    Game1.player.shiftToolbar(false);
                                }
                                if (currentKBState.IsKeyDown(Keys.N) && !Game1.oldKBState.IsKeyDown(Keys.N))
                                {
                                    Game1.player.shiftToolbar(true);
                                }
                                if (currentKBState.IsKeyDown(Keys.F10) && !Game1.oldKBState.IsKeyDown(Keys.F10) && Game1.server == null)
                                {
                                    Game1.multiplayer.StartServer();
                                }
                            }
                            else if (!Game1.player.UsingTool)
                            {
                                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot1) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot1))
                                {
                                    Game1.player.CurrentToolIndex = 0;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot2) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot2))
                                {
                                    Game1.player.CurrentToolIndex = 1;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot3) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot3))
                                {
                                    Game1.player.CurrentToolIndex = 2;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot4) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot4))
                                {
                                    Game1.player.CurrentToolIndex = 3;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot5) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot5))
                                {
                                    Game1.player.CurrentToolIndex = 4;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot6) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot6))
                                {
                                    Game1.player.CurrentToolIndex = 5;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot7) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot7))
                                {
                                    Game1.player.CurrentToolIndex = 6;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot8) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot8))
                                {
                                    Game1.player.CurrentToolIndex = 7;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot9) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot9))
                                {
                                    Game1.player.CurrentToolIndex = 8;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot10) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot10))
                                {
                                    Game1.player.CurrentToolIndex = 9;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot11) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot11))
                                {
                                    Game1.player.CurrentToolIndex = 10;
                                }
                                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot12) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot12))
                                {
                                    Game1.player.CurrentToolIndex = 11;
                                }
                            }
                            if (((Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime && Game1.activeClickableMenu == null) || (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.emoteButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.emoteButton))) && !Game1.debugMode && Game1.player.CanEmote())
                            {
                                if (Game1.player.CanMove)
                                {
                                    Game1.player.Halt();
                                }
                                Game1.emoteMenu = new EmoteMenu();
                                Game1.emoteMenu.gamepadMode = Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime;
                                Game1.timerUntilMouseFade = 0;
                            }
                            if (!Program.releaseBuild)
                            {
                                if (Game1.IsPressEvent(ref currentKBState, Keys.F3) || Game1.IsPressEvent(ref currentPadState, Buttons.LeftStick))
                                {
                                    Game1.debugMode = !Game1.debugMode;
                                    if (Game1.gameMode == 11)
                                    {
                                        Game1.gameMode = 3;
                                    }
                                }
                                if (Game1.IsPressEvent(ref currentKBState, Keys.F8))
                                {
                                    this.requestDebugInput();
                                }
                            }
                            if (currentKBState.IsKeyDown(Keys.F4) && !Game1.oldKBState.IsKeyDown(Keys.F4))
                            {
                                Game1.displayHUD = !Game1.displayHUD;
                                Game1.playSound("smallSelect");
                                if (!Game1.displayHUD)
                                {
                                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3666"));
                                }
                            }
                            bool flag4 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.menuButton);
                            bool flag5 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.journalButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.journalButton);
                            bool flag6 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.mapButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.mapButton);
                            if (Game1.options.gamepadControls && !flag4)
                            {
                                flag4 = (currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B));
                            }
                            if (Game1.options.gamepadControls && !flag5)
                            {
                                flag5 = currentPadState.IsButtonDown(Buttons.Back) && !Game1.oldPadState.IsButtonDown(Buttons.Back);
                            }
                            if (Game1.options.gamepadControls && !flag6)
                            {
                                flag6 = currentPadState.IsButtonDown(Buttons.Y) && !Game1.oldPadState.IsButtonDown(Buttons.Y);
                            }
                            if (flag4 && Game1.CanShowPauseMenu())
                            {
                                if (Game1.activeClickableMenu == null)
                                {
                                    Game1.PushUIMode();
                                    Game1.activeClickableMenu = new GameMenu();
                                    Game1.PopUIMode();
                                }
                                else if (Game1.activeClickableMenu.readyToClose())
                                {
                                    Game1.exitActiveMenu();
                                }
                            }
                            if (Game1.dayOfMonth > 0 && Game1.player.CanMove && flag5 && !Game1.dialogueUp && !flag3)
                            {
                                if (Game1.activeClickableMenu == null)
                                {
                                    Game1.activeClickableMenu = new QuestLog();
                                }
                            }
                            else if (flag3 && Game1.CurrentEvent != null && flag5 && !Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
                            {
                                Game1.CurrentEvent.skipped = true;
                                Game1.CurrentEvent.skipEvent();
                                Game1.freezeControls = false;
                            }
                            if (Game1.options.gamepadControls && Game1.dayOfMonth > 0 && Game1.player.CanMove && Game1.isAnyGamePadButtonBeingPressed() && flag6 && !Game1.dialogueUp && !flag3)
                            {
                                if (Game1.activeClickableMenu == null)
                                {
                                    Game1.PushUIMode();
                                    Game1.activeClickableMenu = new GameMenu(4);
                                    Game1.PopUIMode();
                                }
                            }
                            else if (Game1.dayOfMonth > 0 && Game1.player.CanMove && flag6 && !Game1.dialogueUp && !flag3 && Game1.activeClickableMenu == null)
                            {
                                Game1.PushUIMode();
                                Game1.activeClickableMenu = new GameMenu(3);
                                Game1.PopUIMode();
                            }
                            Game1.checkForRunButton(currentKBState);
                            Game1.oldKBState = currentKBState;
                            Game1.oldMouseState = Game1.input.GetMouseState();
                            Game1.oldPadState = currentPadState;
                        }
                    }
                }
            });
        }
        */
    }
}
