/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewModdingAPI;
using Harmony;

namespace FlowerDanceFix
{
    class ImportantMethodsCopiedForReference
    {
        /*
        //StardewValley.Event.answerDialogueQuestion
       
        public void answerDialogueQuestion(NPC who, string answerKey)
        {
            if (!isFestival)
            {
                return;
            }
            switch (answerKey)
            {
                case "yes":
                    {
                        if (festivalData["file"].Equals("fall16"))
                        {
                            initiateGrangeJudging();
                            if (Game1.IsServer)
                            {
                                Game1.multiplayer.sendServerToClientsMessage("festivalEvent");
                            }
                            break;
                        }
                        string[] newCommands = (eventCommands = GetFestivalDataForYear("mainEvent").Split('/'));
                        CurrentCommand = 0;
                        eventSwitched = true;
                        playerControlSequence = false;
                        setUpFestivalMainEvent();
                        if (Game1.IsServer)
                        {
                            Game1.multiplayer.sendServerToClientsMessage("festivalEvent");
                        }
                        break;
                    }
                case "danceAsk":
                    if (Game1.player.spouse != null && who.Name.Equals(Game1.player.spouse))
                    {
                        Game1.player.dancePartner.Value = who;
                        switch (Game1.player.spouse)
                        {
                            case "Abigail":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1613"));
                                break;
                            case "Penny":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1615"));
                                break;
                            case "Maru":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1617"));
                                break;
                            case "Leah":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1619"));
                                break;
                            case "Haley":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1621"));
                                break;
                            case "Sebastian":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1623"));
                                break;
                            case "Sam":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1625"));
                                break;
                            case "Harvey":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1627"));
                                break;
                            case "Elliott":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1629"));
                                break;
                            case "Alex":
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1631"));
                                break;
                            default:
                                who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1632"));
                                break;
                        }
                        foreach (NPC j in actors)
                        {
                            if (j.CurrentDialogue != null && j.CurrentDialogue.Count > 0 && j.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                            {
                                j.CurrentDialogue.Clear();
                            }
                        }
                    }
                    else if (!who.HasPartnerForDance && Game1.player.getFriendshipLevelForNPC(who.Name) >= 1000 && !who.isMarried())
                    {
                        string accept = "";
                        switch (who.Gender)
                        {
                            case 0:
                                accept = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1633");
                                break;
                            case 1:
                                accept = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1634");
                                break;
                        }
                        try
                        {
                            Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.Name));
                        }
                        catch (Exception)
                        {
                        }
                        Game1.player.dancePartner.Value = who;
                        who.setNewDialogue(accept);
                        foreach (NPC i in actors)
                        {
                            if (i.CurrentDialogue != null && i.CurrentDialogue.Count > 0 && i.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                            {
                                i.CurrentDialogue.Clear();
                            }
                        }
                    }
                    else if (who.HasPartnerForDance)
                    {
                        who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1635"));
                    }
                    else
                    {
                        try
                        {
                            who.setNewDialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + who.Name)["danceRejection"]);
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                    Game1.drawDialogue(who);
                    who.immediateSpeak = true;
                    who.facePlayer(Game1.player);
                    who.Halt();
                    break;
                case "no":
                    break;
            }
        }

        //StardewValley.Dialogue.chooseResponse

        public bool chooseResponse(Response response)
        {
            for (int i = 0; i < playerResponses.Count; i++)
            {
                if (playerResponses[i].responseKey == null || response.responseKey == null || !playerResponses[i].responseKey.Equals(response.responseKey))
                {
                    continue;
                }
                if (answerQuestionBehavior != null)
                {
                    if (answerQuestionBehavior(i))
                    {
                        Game1.currentSpeaker = null;
                    }
                    isLastDialogueInteractive = false;
                    finishedLastDialogue = true;
                    answerQuestionBehavior = null;
                    return true;
                }
                if (quickResponse)
                {
                    isLastDialogueInteractive = false;
                    finishedLastDialogue = true;
                    isCurrentStringContinuedOnNextScreen = true;
                    speaker.setNewDialogue(quickResponses[i]);
                    Game1.drawDialogue(speaker);
                    speaker.faceTowardFarmerForPeriod(4000, 3, faceAway: false, farmer);
                    return true;
                }
                if (Game1.isFestival())
                {
                    Game1.currentLocation.currentEvent.answerDialogueQuestion(speaker, playerResponses[i].responseKey);
                    isLastDialogueInteractive = false;
                    finishedLastDialogue = true;
                    return false;
                }
                farmer.changeFriendship(playerResponses[i].friendshipChange, speaker);
                if (playerResponses[i].id != -1)
                {
                    farmer.addSeenResponse(playerResponses[i].id);
                }
                isLastDialogueInteractive = false;
                finishedLastDialogue = false;
                parseDialogueString(speaker.Dialogue[playerResponses[i].responseKey]);
                isCurrentStringContinuedOnNextScreen = true;
                return false;
            }
            return false;
        }

        //StardewValley.Menus.DialogueBox.recieveLeftClick

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (transitioning)
            {
                return;
            }
            if (characterIndexInDialogue < getCurrentString().Length - 1)
            {
                characterIndexInDialogue = getCurrentString().Length - 1;
            }
            else
            {
                if (safetyTimer > 0)
                {
                    return;
                }
                if (isQuestion)
                {
                    if (selectedResponse == -1)
                    {
                        return;
                    }
                    questionFinishPauseTimer = (Game1.eventUp ? 600 : 200);
                    transitioning = true;
                    transitionInitialized = false;
                    transitioningBigger = true;
                    if (characterDialogue == null)
                    {
                        Game1.dialogueUp = false;
                        if (Game1.eventUp && Game1.currentLocation.afterQuestion == null)
                        {
                            Game1.playSound("smallSelect");
                            Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, selectedResponse);
                            selectedResponse = -1;
                            tryOutro();
                            return;
                        }
                        if (Game1.currentLocation.answerDialogue(responses[selectedResponse]))
                        {
                            Game1.playSound("smallSelect");
                        }
                        selectedResponse = -1;
                        tryOutro();
                        return;
                    }
                    characterDialoguesBrokenUp.Pop();
                    characterDialogue.chooseResponse(responses[selectedResponse]);
                    characterDialoguesBrokenUp.Push("");
                    Game1.playSound("smallSelect");
                }
                else if (characterDialogue == null)
                {
                    dialogues.RemoveAt(0);
                    if (dialogues.Count == 0)
                    {
                        closeDialogue();
                    }
                    else
                    {
                        width = Math.Min(1200, SpriteText.getWidthOfString(dialogues[0]) + 64);
                        height = SpriteText.getHeightOfString(dialogues[0], width - 16);
                        this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(width, height).X;
                        this.y = Game1.uiViewport.Height - height - 64;
                        xPositionOnScreen = x;
                        yPositionOnScreen = y;
                        setUpIcons();
                    }
                }
                characterIndexInDialogue = 0;
                if (characterDialogue != null)
                {
                    int oldPortrait = characterDialogue.getPortraitIndex();
                    if (characterDialoguesBrokenUp.Count == 0)
                    {
                        beginOutro();
                        return;
                    }
                    characterDialoguesBrokenUp.Pop();
                    if (characterDialoguesBrokenUp.Count == 0)
                    {
                        if (!characterDialogue.isCurrentStringContinuedOnNextScreen)
                        {
                            beginOutro();
                        }
                        characterDialogue.exitCurrentDialogue();
                    }
                    if (!characterDialogue.isDialogueFinished() && characterDialogue.getCurrentDialogue().Length > 0 && characterDialoguesBrokenUp.Count == 0)
                    {
                        characterDialoguesBrokenUp.Push(characterDialogue.getCurrentDialogue());
                    }
                    checkDialogue(characterDialogue);
                    if (characterDialogue.getPortraitIndex() != oldPortrait)
                    {
                        newPortaitShakeTimer = ((characterDialogue.getPortraitIndex() == 1) ? 250 : 50);
                    }
                }
                if (!transitioning)
                {
                    Game1.playSound("smallSelect");
                }
                setUpIcons();
                safetyTimer = 750;
                if (getCurrentString() != null && getCurrentString().Length <= 20)
                {
                    safetyTimer -= 200;
                }
            }
        }

        //StardewValley.Game1.updateActiveMenu

        public static void updateActiveMenu(GameTime gameTime)
        {
            IClickableMenu active_menu = activeClickableMenu;
            while (active_menu.GetChildMenu() != null)
            {
                active_menu = active_menu.GetChildMenu();
            }
            if (!Program.gamePtr.IsActiveNoOverlay && Program.releaseBuild)
            {
                if (active_menu != null && active_menu.IsActive())
                {
                    active_menu.update(gameTime);
                }
                return;
            }
            MouseState mouseState = input.GetMouseState();
            KeyboardState keyState = GetKeyboardState();
            GamePadState padState = input.GetGamePadState();
            if (CurrentEvent != null)
            {
                if ((mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) || (options.gamepadControls && padState.IsButtonDown(Buttons.A) && oldPadState.IsButtonUp(Buttons.A)))
                {
                    CurrentEvent.receiveMouseClick(getMouseX(), getMouseY());
                }
                else if (options.gamepadControls && padState.IsButtonDown(Buttons.Back) && oldPadState.IsButtonUp(Buttons.Back) && !CurrentEvent.skipped && CurrentEvent.skippable)
                {
                    CurrentEvent.skipped = true;
                    CurrentEvent.skipEvent();
                    freezeControls = false;
                }
                if (CurrentEvent != null && CurrentEvent.skipped)
                {
                    oldMouseState = input.GetMouseState();
                    oldKBState = keyState;
                    oldPadState = padState;
                    return;
                }
            }
            if (options.gamepadControls && active_menu != null && active_menu.IsActive())
            {
                if (isGamePadThumbstickInMotion() && (!options.snappyMenus || active_menu.overrideSnappyMenuCursorMovementBan()))
                {
                    setMousePositionRaw((int)((float)mouseState.X + padState.ThumbSticks.Left.X * thumbstickToMouseModifier), (int)((float)mouseState.Y - padState.ThumbSticks.Left.Y * thumbstickToMouseModifier));
                }
                if (active_menu != null && active_menu.IsActive() && (chatBox == null || !chatBox.isActive()))
                {
                    ButtonCollection.ButtonEnumerator enumerator = Utility.getPressedButtons(padState, oldPadState).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Buttons b2 = enumerator.Current;
                        active_menu.receiveGamePadButton(b2);
                        if (active_menu == null || !active_menu.IsActive())
                        {
                            break;
                        }
                    }
                    enumerator = Utility.getHeldButtons(padState).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Buttons b3 = enumerator.Current;
                        if (active_menu != null && active_menu.IsActive())
                        {
                            active_menu.gamePadButtonHeld(b3);
                        }
                        if (active_menu == null || !active_menu.IsActive())
                        {
                            break;
                        }
                    }
                }
            }
            if ((getMouseX() != getOldMouseX() || getMouseY() != getOldMouseY()) && !isGamePadThumbstickInMotion() && !isDPadPressed())
            {
                lastCursorMotionWasMouse = true;
            }
            ResetFreeCursorDrag();
            if (active_menu != null && active_menu.IsActive())
            {
                active_menu.performHoverAction(getMouseX(), getMouseY());
            }
            if (active_menu != null && active_menu.IsActive())
            {
                active_menu.update(gameTime);
            }
            if (active_menu != null && active_menu.IsActive() && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                if (chatBox != null && chatBox.isActive() && chatBox.isWithinBounds(getMouseX(), getMouseY()))
                {
                    chatBox.receiveLeftClick(getMouseX(), getMouseY());
                }
                else
                {
                    active_menu.receiveLeftClick(getMouseX(), getMouseY());
                }
            }
            else if (active_menu != null && active_menu.IsActive() && mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && (oldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released || ((float)mouseClickPolling > 650f && !(active_menu is DialogueBox))))
            {
                active_menu.receiveRightClick(getMouseX(), getMouseY());
                if ((float)mouseClickPolling > 650f)
                {
                    mouseClickPolling = 600;
                }
                if ((active_menu == null || !active_menu.IsActive()) && activeClickableMenu == null)
                {
                    rightClickPolling = 500;
                    mouseClickPolling = 0;
                }
            }
            if (mouseState.ScrollWheelValue != oldMouseState.ScrollWheelValue && active_menu != null && active_menu.IsActive())
            {
                if (chatBox != null && chatBox.choosingEmoji && chatBox.emojiMenu.isWithinBounds(getOldMouseX(), getOldMouseY()))
                {
                    chatBox.receiveScrollWheelAction(mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue);
                }
                else
                {
                    active_menu.receiveScrollWheelAction(mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue);
                }
            }
            if (options.gamepadControls && active_menu != null && active_menu.IsActive())
            {
                thumbstickPollingTimer -= currentGameTime.ElapsedGameTime.Milliseconds;
                if (thumbstickPollingTimer <= 0)
                {
                    if (padState.ThumbSticks.Right.Y > 0.2f)
                    {
                        active_menu.receiveScrollWheelAction(1);
                    }
                    else if (padState.ThumbSticks.Right.Y < -0.2f)
                    {
                        active_menu.receiveScrollWheelAction(-1);
                    }
                }
                if (thumbstickPollingTimer <= 0)
                {
                    thumbstickPollingTimer = 220 - (int)(Math.Abs(padState.ThumbSticks.Right.Y) * 170f);
                }
                if (Math.Abs(padState.ThumbSticks.Right.Y) < 0.2f)
                {
                    thumbstickPollingTimer = 0;
                }
            }
            if (active_menu != null && active_menu.IsActive() && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                active_menu.releaseLeftClick(getMouseX(), getMouseY());
            }
            else if (active_menu != null && active_menu.IsActive() && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                active_menu.leftClickHeld(getMouseX(), getMouseY());
            }
            Microsoft.Xna.Framework.Input.Keys[] pressedKeys = keyState.GetPressedKeys();
            foreach (Microsoft.Xna.Framework.Input.Keys i in pressedKeys)
            {
                if (active_menu != null && active_menu.IsActive() && !oldKBState.GetPressedKeys().Contains(i))
                {
                    active_menu.receiveKeyPress(i);
                }
            }
            if (chatBox == null || !chatBox.isActive())
            {
                if (isOneOfTheseKeysDown(oldKBState, options.moveUpButton) || (options.snappyMenus && options.gamepadControls && (Math.Abs(padState.ThumbSticks.Left.X) < padState.ThumbSticks.Left.Y || padState.IsButtonDown(Buttons.DPadUp))))
                {
                    directionKeyPolling[0] -= currentGameTime.ElapsedGameTime.Milliseconds;
                }
                else if (isOneOfTheseKeysDown(oldKBState, options.moveRightButton) || (options.snappyMenus && options.gamepadControls && (padState.ThumbSticks.Left.X > Math.Abs(padState.ThumbSticks.Left.Y) || padState.IsButtonDown(Buttons.DPadRight))))
                {
                    directionKeyPolling[1] -= currentGameTime.ElapsedGameTime.Milliseconds;
                }
                else if (isOneOfTheseKeysDown(oldKBState, options.moveDownButton) || (options.snappyMenus && options.gamepadControls && (Math.Abs(padState.ThumbSticks.Left.X) < Math.Abs(padState.ThumbSticks.Left.Y) || padState.IsButtonDown(Buttons.DPadDown))))
                {
                    directionKeyPolling[2] -= currentGameTime.ElapsedGameTime.Milliseconds;
                }
                else if (isOneOfTheseKeysDown(oldKBState, options.moveLeftButton) || (options.snappyMenus && options.gamepadControls && (Math.Abs(padState.ThumbSticks.Left.X) > Math.Abs(padState.ThumbSticks.Left.Y) || padState.IsButtonDown(Buttons.DPadLeft))))
                {
                    directionKeyPolling[3] -= currentGameTime.ElapsedGameTime.Milliseconds;
                }
                if (areAllOfTheseKeysUp(oldKBState, options.moveUpButton) && (!options.snappyMenus || !options.gamepadControls || ((double)padState.ThumbSticks.Left.Y < 0.1 && padState.IsButtonUp(Buttons.DPadUp))))
                {
                    directionKeyPolling[0] = 250;
                }
                if (areAllOfTheseKeysUp(oldKBState, options.moveRightButton) && (!options.snappyMenus || !options.gamepadControls || ((double)padState.ThumbSticks.Left.X < 0.1 && padState.IsButtonUp(Buttons.DPadRight))))
                {
                    directionKeyPolling[1] = 250;
                }
                if (areAllOfTheseKeysUp(oldKBState, options.moveDownButton) && (!options.snappyMenus || !options.gamepadControls || ((double)padState.ThumbSticks.Left.Y > -0.1 && padState.IsButtonUp(Buttons.DPadDown))))
                {
                    directionKeyPolling[2] = 250;
                }
                if (areAllOfTheseKeysUp(oldKBState, options.moveLeftButton) && (!options.snappyMenus || !options.gamepadControls || ((double)padState.ThumbSticks.Left.X > -0.1 && padState.IsButtonUp(Buttons.DPadLeft))))
                {
                    directionKeyPolling[3] = 250;
                }
                if (directionKeyPolling[0] <= 0 && active_menu != null && active_menu.IsActive())
                {
                    active_menu.receiveKeyPress(options.getFirstKeyboardKeyFromInputButtonList(options.moveUpButton));
                    directionKeyPolling[0] = 70;
                }
                if (directionKeyPolling[1] <= 0 && active_menu != null && active_menu.IsActive())
                {
                    active_menu.receiveKeyPress(options.getFirstKeyboardKeyFromInputButtonList(options.moveRightButton));
                    directionKeyPolling[1] = 70;
                }
                if (directionKeyPolling[2] <= 0 && active_menu != null && active_menu.IsActive())
                {
                    active_menu.receiveKeyPress(options.getFirstKeyboardKeyFromInputButtonList(options.moveDownButton));
                    directionKeyPolling[2] = 70;
                }
                if (directionKeyPolling[3] <= 0 && active_menu != null && active_menu.IsActive())
                {
                    active_menu.receiveKeyPress(options.getFirstKeyboardKeyFromInputButtonList(options.moveLeftButton));
                    directionKeyPolling[3] = 70;
                }
                if (options.gamepadControls && active_menu != null && active_menu.IsActive())
                {
                    if (!active_menu.areGamePadControlsImplemented() && padState.IsButtonDown(Buttons.A) && (!oldPadState.IsButtonDown(Buttons.A) || ((float)gamePadAButtonPolling > 650f && !(active_menu is DialogueBox))))
                    {
                        active_menu.receiveLeftClick(getMousePosition().X, getMousePosition().Y);
                        if ((float)gamePadAButtonPolling > 650f)
                        {
                            gamePadAButtonPolling = 600;
                        }
                    }
                    else if (!active_menu.areGamePadControlsImplemented() && !padState.IsButtonDown(Buttons.A) && oldPadState.IsButtonDown(Buttons.A))
                    {
                        active_menu.releaseLeftClick(getMousePosition().X, getMousePosition().Y);
                    }
                    else if (!active_menu.areGamePadControlsImplemented() && padState.IsButtonDown(Buttons.X) && (!oldPadState.IsButtonDown(Buttons.X) || ((float)gamePadXButtonPolling > 650f && !(active_menu is DialogueBox))))
                    {
                        active_menu.receiveRightClick(getMousePosition().X, getMousePosition().Y);
                        if ((float)gamePadXButtonPolling > 650f)
                        {
                            gamePadXButtonPolling = 600;
                        }
                    }
                    ButtonCollection.ButtonEnumerator enumerator = Utility.getPressedButtons(padState, oldPadState).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Buttons b = enumerator.Current;
                        if (active_menu == null || !active_menu.IsActive())
                        {
                            break;
                        }
                        Microsoft.Xna.Framework.Input.Keys key = Utility.mapGamePadButtonToKey(b);
                        if (!(active_menu is FarmhandMenu) || game1.IsMainInstance || !options.doesInputListContain(options.menuButton, key))
                        {
                            active_menu.receiveKeyPress(key);
                        }
                    }
                    if (active_menu != null && active_menu.IsActive() && !active_menu.areGamePadControlsImplemented() && padState.IsButtonDown(Buttons.A) && oldPadState.IsButtonDown(Buttons.A))
                    {
                        active_menu.leftClickHeld(getMousePosition().X, getMousePosition().Y);
                    }
                    if (padState.IsButtonDown(Buttons.X))
                    {
                        gamePadXButtonPolling += gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        gamePadXButtonPolling = 0;
                    }
                    if (padState.IsButtonDown(Buttons.A))
                    {
                        gamePadAButtonPolling += gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        gamePadAButtonPolling = 0;
                    }
                    if (!active_menu.IsActive() && activeClickableMenu == null)
                    {
                        rightClickPolling = 500;
                        gamePadXButtonPolling = 0;
                        gamePadAButtonPolling = 0;
                    }
                }
            }
            else
            {
                _ = options.SnappyMenus;
            }
            if (mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                mouseClickPolling += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                mouseClickPolling = 0;
            }
            oldMouseState = input.GetMouseState();
            oldKBState = keyState;
            oldPadState = padState;
        }

        //StardewValley.Game1.Update
        protected override void Update(GameTime gameTime)
        {
            GameTime time = gameTime;
            DebugTools.BeforeGameUpdate(this, ref time);
            input.UpdateStates();
            if (input.GetGamePadState().IsButtonDown(Buttons.RightStick))
            {
                rightStickHoldTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            GameMenu.bundleItemHovered = false;
            _update(time);
            if (IsMultiplayer && player != null)
            {
                player.requestingTimePause.Value = !shouldTimePass(LocalMultiplayer.IsLocalMultiplayer(is_local_only: true));
                if (IsMasterGame)
                {
                    bool should_time_pause = false;
                    if (LocalMultiplayer.IsLocalMultiplayer(is_local_only: true))
                    {
                        should_time_pause = true;
                        foreach (Farmer onlineFarmer in getOnlineFarmers())
                        {
                            if (!onlineFarmer.requestingTimePause.Value)
                            {
                                should_time_pause = false;
                                break;
                            }
                        }
                    }
                    netWorldState.Value.IsTimePaused = should_time_pause;
                }
            }
            Rumble.update(gameTime.ElapsedGameTime.Milliseconds);
            if (options.gamepadControls && thumbstickMotionMargin > 0)
            {
                thumbstickMotionMargin -= gameTime.ElapsedGameTime.Milliseconds;
            }
            if (!input.GetGamePadState().IsButtonDown(Buttons.RightStick))
            {
                rightStickHoldTime = 0;
            }
            base.Update(gameTime);
        }

        //StardewValley.Game1._update

        private void _update(GameTime gameTime)
        {
            if (graphics.GraphicsDevice == null)
            {
                return;
            }
            bool zoom_dirty = false;
            if (options != null && !takingMapScreenshot)
            {
                if (options.baseUIScale != options.desiredUIScale)
                {
                    if (options.desiredUIScale < 0f)
                    {
                        options.desiredUIScale = options.desiredBaseZoomLevel;
                    }
                    options.baseUIScale = options.desiredUIScale;
                    zoom_dirty = true;
                }
                if (options.desiredBaseZoomLevel != options.baseZoomLevel)
                {
                    options.baseZoomLevel = options.desiredBaseZoomLevel;
                    forceSnapOnNextViewportUpdate = true;
                    zoom_dirty = true;
                }
            }
            if (zoom_dirty)
            {
                refreshWindowSettings();
            }
            CheckGamepadMode();
            FarmAnimal.NumPathfindingThisTick = 0;
            options.reApplySetOptions();
            if (toggleFullScreen)
            {
                toggleFullscreen();
                toggleFullScreen = false;
            }
            input.Update();
            if (frameByFrame)
            {
                if (GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && oldKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    frameByFrame = false;
                }
                bool advanceFrame = false;
                if (GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G) && oldKBState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.G))
                {
                    advanceFrame = true;
                }
                if (!advanceFrame)
                {
                    oldKBState = GetKeyboardState();
                    return;
                }
            }
            if (client != null && client.timedOut)
            {
                multiplayer.clientRemotelyDisconnected(client.pendingDisconnect);
            }
            if (_newDayTask != null)
            {
                if (_newDayTask.Status == TaskStatus.Created)
                {
                    _newDayTask.Start();
                }
                if (_newDayTask.Status >= TaskStatus.RanToCompletion)
                {
                    if (_newDayTask.IsFaulted)
                    {
                        Exception e = _newDayTask.Exception.GetBaseException();
                        Console.WriteLine("_newDayTask failed with an exception");
                        Console.WriteLine(e);
                        throw new Exception("Error on new day: \n---------------\n" + e.Message + "\n" + e.StackTrace + "\n---------------\n");
                    }
                    _newDayTask = null;
                    Utility.CollectGarbage();
                }
                UpdateChatBox();
                return;
            }
            if (isLocalMultiplayerNewDayActive)
            {
                UpdateChatBox();
                return;
            }
            if (IsSaving)
            {
                PushUIMode();
                activeClickableMenu?.update(gameTime);
                if (overlayMenu != null)
                {
                    overlayMenu.update(gameTime);
                    if (overlayMenu == null)
                    {
                        PopUIMode();
                        return;
                    }
                }
                PopUIMode();
                UpdateChatBox();
                return;
            }
            if (exitToTitle)
            {
                exitToTitle = false;
                CleanupReturningToTitle();
                Utility.CollectGarbage();
                if (postExitToTitleCallback != null)
                {
                    postExitToTitleCallback();
                }
            }
            SetFreeCursorElapsed((float)gameTime.ElapsedGameTime.TotalSeconds);
            Program.sdk.Update();
            if (game1.IsMainInstance)
            {
                keyboardFocusInstance = game1;
                foreach (Game1 instance in GameRunner.instance.gameInstances)
                {
                    if (instance.instanceKeyboardDispatcher.Subscriber != null && instance.instanceTextEntry != null)
                    {
                        keyboardFocusInstance = instance;
                        break;
                    }
                }
            }
            if (HasKeyboardFocus())
            {
                keyboardDispatcher.Poll();
            }
            else
            {
                keyboardDispatcher.Discard();
            }
            if (gameMode == 6)
            {
                multiplayer.UpdateLoading();
            }
            if (gameMode == 3)
            {
                multiplayer.UpdateEarly();
                if (player != null && player.team != null)
                {
                    player.team.Update();
                }
            }
            if ((paused || (!IsActiveNoOverlay && Program.releaseBuild)) && (options == null || options.pauseWhenOutOfFocus || paused) && multiplayerMode == 0)
            {
                UpdateChatBox();
                return;
            }
            if (quit)
            {
                Exit();
            }
            currentGameTime = gameTime;
            if (gameMode == 11)
            {
                return;
            }
            ticks++;
            if (IsActiveNoOverlay)
            {
                checkForEscapeKeys();
            }
            updateMusic();
            updateRaindropPosition();
            if (bloom != null)
            {
                bloom.tick(gameTime);
            }
            if (globalFade)
            {
                screenFade.UpdateGlobalFade();
            }
            else if (pauseThenDoFunctionTimer > 0)
            {
                freezeControls = true;
                pauseThenDoFunctionTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (pauseThenDoFunctionTimer <= 0)
                {
                    freezeControls = false;
                    if (afterPause != null)
                    {
                        afterPause();
                    }
                }
            }
            bool should_clamp_cursor = false;
            if (options.gamepadControls && activeClickableMenu != null && activeClickableMenu.shouldClampGamePadCursor())
            {
                should_clamp_cursor = true;
            }
            if (should_clamp_cursor)
            {
                Microsoft.Xna.Framework.Point pos = getMousePositionRaw();
                Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(0, 0, localMultiplayerWindow.Width, localMultiplayerWindow.Height);
                if (pos.X < rect.X)
                {
                    pos.X = rect.X;
                }
                else if (pos.X > rect.Right)
                {
                    pos.X = rect.Right;
                }
                if (pos.Y < rect.Y)
                {
                    pos.Y = rect.Y;
                }
                else if (pos.Y > rect.Bottom)
                {
                    pos.Y = rect.Bottom;
                }
                setMousePositionRaw(pos.X, pos.Y);
            }
            if (gameMode == 3 || gameMode == 2)
            {
                if (!warpingForForcedRemoteEvent && !eventUp && remoteEventQueue.Count > 0 && player != null && player.isCustomized.Value && (!fadeIn || !(fadeToBlackAlpha > 0f)))
                {
                    if (activeClickableMenu != null)
                    {
                        activeClickableMenu.emergencyShutDown();
                        exitActiveMenu();
                    }
                    else if (currentMinigame != null && currentMinigame.forceQuit())
                    {
                        currentMinigame = null;
                    }
                    if (activeClickableMenu == null && currentMinigame == null && player.freezePause <= 0)
                    {
                        Action action = remoteEventQueue[0];
                        remoteEventQueue.RemoveAt(0);
                        action();
                    }
                }
                player.millisecondsPlayed += (uint)gameTime.ElapsedGameTime.Milliseconds;
                bool doMainGameUpdates = true;
                if (currentMinigame != null && !HostPaused)
                {
                    if (pauseTime > 0f)
                    {
                        updatePause(gameTime);
                    }
                    if (fadeToBlack)
                    {
                        screenFade.UpdateFadeAlpha(gameTime);
                        if (fadeToBlackAlpha >= 1f)
                        {
                            fadeToBlack = false;
                        }
                    }
                    else
                    {
                        if (thumbstickMotionMargin > 0)
                        {
                            thumbstickMotionMargin -= gameTime.ElapsedGameTime.Milliseconds;
                        }
                        KeyboardState currentKBState = default(KeyboardState);
                        MouseState currentMouseState = default(MouseState);
                        GamePadState currentPadState = default(GamePadState);
                        if (base.IsActive)
                        {
                            currentKBState = GetKeyboardState();
                            currentMouseState = input.GetMouseState();
                            currentPadState = input.GetGamePadState();
                            bool ignore_controls = false;
                            if (chatBox != null && chatBox.isActive())
                            {
                                ignore_controls = true;
                            }
                            else if (textEntry != null)
                            {
                                ignore_controls = true;
                            }
                            if (ignore_controls)
                            {
                                currentKBState = default(KeyboardState);
                                currentPadState = default(GamePadState);
                            }
                            else
                            {
                                Microsoft.Xna.Framework.Input.Keys[] pressedKeys = currentKBState.GetPressedKeys();
                                foreach (Microsoft.Xna.Framework.Input.Keys i in pressedKeys)
                                {
                                    if (!oldKBState.IsKeyDown(i) && currentMinigame != null)
                                    {
                                        currentMinigame.receiveKeyPress(i);
                                    }
                                }
                                if (options.gamepadControls)
                                {
                                    if (currentMinigame == null)
                                    {
                                        oldMouseState = currentMouseState;
                                        oldKBState = currentKBState;
                                        oldPadState = currentPadState;
                                        UpdateChatBox();
                                        return;
                                    }
                                    ButtonCollection.ButtonEnumerator enumerator2 = Utility.getPressedButtons(currentPadState, oldPadState).GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        Buttons b2 = enumerator2.Current;
                                        if (currentMinigame != null)
                                        {
                                            currentMinigame.receiveKeyPress(Utility.mapGamePadButtonToKey(b2));
                                        }
                                    }
                                    if (currentMinigame == null)
                                    {
                                        oldMouseState = currentMouseState;
                                        oldKBState = currentKBState;
                                        oldPadState = currentPadState;
                                        UpdateChatBox();
                                        return;
                                    }
                                    if (currentPadState.ThumbSticks.Right.Y < -0.2f && oldPadState.ThumbSticks.Right.Y >= -0.2f)
                                    {
                                        currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Down);
                                    }
                                    if (currentPadState.ThumbSticks.Right.Y > 0.2f && oldPadState.ThumbSticks.Right.Y <= 0.2f)
                                    {
                                        currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Up);
                                    }
                                    if (currentPadState.ThumbSticks.Right.X < -0.2f && oldPadState.ThumbSticks.Right.X >= -0.2f)
                                    {
                                        currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Left);
                                    }
                                    if (currentPadState.ThumbSticks.Right.X > 0.2f && oldPadState.ThumbSticks.Right.X <= 0.2f)
                                    {
                                        currentMinigame.receiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Right);
                                    }
                                    if (oldPadState.ThumbSticks.Right.Y < -0.2f && currentPadState.ThumbSticks.Right.Y >= -0.2f)
                                    {
                                        currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Down);
                                    }
                                    if (oldPadState.ThumbSticks.Right.Y > 0.2f && currentPadState.ThumbSticks.Right.Y <= 0.2f)
                                    {
                                        currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Up);
                                    }
                                    if (oldPadState.ThumbSticks.Right.X < -0.2f && currentPadState.ThumbSticks.Right.X >= -0.2f)
                                    {
                                        currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Left);
                                    }
                                    if (oldPadState.ThumbSticks.Right.X > 0.2f && currentPadState.ThumbSticks.Right.X <= 0.2f)
                                    {
                                        currentMinigame.receiveKeyRelease(Microsoft.Xna.Framework.Input.Keys.Right);
                                    }
                                    if (isGamePadThumbstickInMotion() && currentMinigame != null && !currentMinigame.overrideFreeMouseMovement())
                                    {
                                        setMousePosition(getMouseX() + (int)(currentPadState.ThumbSticks.Left.X * thumbstickToMouseModifier), getMouseY() - (int)(currentPadState.ThumbSticks.Left.Y * thumbstickToMouseModifier));
                                    }
                                    else if (getMouseX() != getOldMouseX() || getMouseY() != getOldMouseY())
                                    {
                                        lastCursorMotionWasMouse = true;
                                    }
                                }
                                pressedKeys = oldKBState.GetPressedKeys();
                                foreach (Microsoft.Xna.Framework.Input.Keys j in pressedKeys)
                                {
                                    if (!currentKBState.IsKeyDown(j) && currentMinigame != null)
                                    {
                                        currentMinigame.receiveKeyRelease(j);
                                    }
                                }
                                if (options.gamepadControls)
                                {
                                    if (currentMinigame == null)
                                    {
                                        oldMouseState = currentMouseState;
                                        oldKBState = currentKBState;
                                        oldPadState = currentPadState;
                                        UpdateChatBox();
                                        return;
                                    }
                                    if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.X) && !oldPadState.IsButtonDown(Buttons.X))
                                    {
                                        currentMinigame.receiveRightClick(getMouseX(), getMouseY());
                                    }
                                    else if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.A) && !oldPadState.IsButtonDown(Buttons.A))
                                    {
                                        currentMinigame.receiveLeftClick(getMouseX(), getMouseY());
                                    }
                                    else if (currentPadState.IsConnected && !currentPadState.IsButtonDown(Buttons.X) && oldPadState.IsButtonDown(Buttons.X))
                                    {
                                        currentMinigame.releaseRightClick(getMouseX(), getMouseY());
                                    }
                                    else if (currentPadState.IsConnected && !currentPadState.IsButtonDown(Buttons.A) && oldPadState.IsButtonDown(Buttons.A))
                                    {
                                        currentMinigame.releaseLeftClick(getMouseX(), getMouseY());
                                    }
                                    ButtonCollection.ButtonEnumerator enumerator2 = Utility.getPressedButtons(oldPadState, currentPadState).GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        Buttons b = enumerator2.Current;
                                        if (currentMinigame != null)
                                        {
                                            currentMinigame.receiveKeyRelease(Utility.mapGamePadButtonToKey(b));
                                        }
                                    }
                                    if (currentPadState.IsConnected && currentPadState.IsButtonDown(Buttons.A) && currentMinigame != null)
                                    {
                                        currentMinigame.leftClickHeld(0, 0);
                                    }
                                }
                                if (currentMinigame == null)
                                {
                                    oldMouseState = currentMouseState;
                                    oldKBState = currentKBState;
                                    oldPadState = currentPadState;
                                    UpdateChatBox();
                                    return;
                                }
                                if (currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                {
                                    currentMinigame.receiveLeftClick(getMouseX(), getMouseY());
                                }
                                if (currentMinigame != null && currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                {
                                    currentMinigame.receiveRightClick(getMouseX(), getMouseY());
                                }
                                if (currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                {
                                    currentMinigame.releaseLeftClick(getMouseX(), getMouseY());
                                }
                                if (currentMinigame != null && currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && oldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                {
                                    currentMinigame.releaseLeftClick(getMouseX(), getMouseY());
                                }
                                if (currentMinigame != null && currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                                {
                                    currentMinigame.leftClickHeld(getMouseX(), getMouseY());
                                }
                            }
                        }
                        if (currentMinigame != null && currentMinigame.tick(gameTime))
                        {
                            oldMouseState = currentMouseState;
                            oldKBState = currentKBState;
                            oldPadState = currentPadState;
                            if (currentMinigame != null)
                            {
                                currentMinigame.unload();
                            }
                            currentMinigame = null;
                            fadeIn = true;
                            fadeToBlackAlpha = 1f;
                            UpdateChatBox();
                            return;
                        }
                        if (currentMinigame == null && IsMusicContextActive(MusicContext.MiniGame))
                        {
                            stopMusicTrack(MusicContext.MiniGame);
                        }
                        oldMouseState = currentMouseState;
                        oldKBState = currentKBState;
                        oldPadState = currentPadState;
                    }
                    doMainGameUpdates = IsMultiplayer || currentMinigame == null || currentMinigame.doMainGameUpdates();
                }
                else if (farmEvent != null && !HostPaused && farmEvent.tickUpdate(gameTime))
                {
                    farmEvent.makeChangesToLocation();
                    timeOfDay = 600;
                    UpdateOther(gameTime);
                    displayHUD = true;
                    farmEvent = null;
                    netWorldState.Value.WriteToGame1();
                    currentLocation = player.currentLocation;
                    FarmHouse farmHouse = currentLocation as FarmHouse;
                    if (farmHouse != null)
                    {
                        player.Position = Utility.PointToVector2(farmHouse.GetPlayerBedSpot()) * 64f;
                        BedFurniture.ShiftPositionForBed(player);
                    }
                    else
                    {
                        BedFurniture.ApplyWakeUpPosition(player);
                    }
                    changeMusicTrack("none");
                    currentLocation.resetForPlayerEntry();
                    if (player.IsSitting())
                    {
                        player.StopSitting(animate: false);
                    }
                    player.forceCanMove();
                    freezeControls = false;
                    displayFarmer = true;
                    outdoorLight = Microsoft.Xna.Framework.Color.White;
                    viewportFreeze = false;
                    fadeToBlackAlpha = 0f;
                    fadeToBlack = false;
                    globalFadeToClear();
                    RemoveDeliveredMailForTomorrow();
                    handlePostFarmEventActions();
                    showEndOfNightStuff();
                }
                if (doMainGameUpdates)
                {
                    if (endOfNightMenus.Count > 0 && activeClickableMenu == null)
                    {
                        activeClickableMenu = endOfNightMenus.Pop();
                        if (activeClickableMenu != null && options.SnappyMenus)
                        {
                            activeClickableMenu.snapToDefaultClickableComponent();
                        }
                    }
                    if (specialCurrencyDisplay != null)
                    {
                        specialCurrencyDisplay.Update(gameTime);
                    }
                    if (currentLocation != null && currentMinigame == null)
                    {
                        if (emoteMenu != null)
                        {
                            emoteMenu.update(gameTime);
                            if (emoteMenu != null)
                            {
                                PushUIMode();
                                emoteMenu.performHoverAction(getMouseX(), getMouseY());
                                KeyboardState currentState = GetKeyboardState();
                                if (input.GetMouseState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                                {
                                    emoteMenu.receiveLeftClick(getMouseX(), getMouseY());
                                }
                                else if (input.GetMouseState().RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                                {
                                    emoteMenu.receiveRightClick(getMouseX(), getMouseY());
                                }
                                else if (isOneOfTheseKeysDown(currentState, options.menuButton) || (isOneOfTheseKeysDown(currentState, options.emoteButton) && areAllOfTheseKeysUp(oldKBState, options.emoteButton)))
                                {
                                    emoteMenu.exitThisMenu(playSound: false);
                                }
                                PopUIMode();
                                oldKBState = currentState;
                                oldMouseState = input.GetMouseState();
                            }
                        }
                        else if (textEntry != null)
                        {
                            PushUIMode();
                            updateTextEntry(gameTime);
                            PopUIMode();
                        }
                        else if (activeClickableMenu != null)
                        {
                            PushUIMode();
                            updateActiveMenu(gameTime);
                            PopUIMode();
                        }
                        else
                        {
                            if (pauseTime > 0f)
                            {
                                updatePause(gameTime);
                            }
                            if (!globalFade && !freezeControls && activeClickableMenu == null && (IsActiveNoOverlay || inputSimulator != null))
                            {
                                UpdateControlInput(gameTime);
                            }
                        }
                    }
                    if (showingEndOfNightStuff && endOfNightMenus.Count == 0 && activeClickableMenu == null)
                    {
                        if (newDaySync != null)
                        {
                            newDaySync = null;
                        }
                        player.team.endOfNightStatus.WithdrawState();
                        showingEndOfNightStuff = false;
                        Action afterAction = _afterNewDayAction;
                        if (afterAction != null)
                        {
                            _afterNewDayAction = null;
                            afterAction();
                        }
                        player.ReequipEnchantments();
                        globalFadeToClear(doMorningStuff);
                    }
                    if (currentLocation != null)
                    {
                        if (!HostPaused && !showingEndOfNightStuff)
                        {
                            if (IsMultiplayer || (activeClickableMenu == null && currentMinigame == null))
                            {
                                UpdateGameClock(gameTime);
                            }
                            UpdateCharacters(gameTime);
                            UpdateLocations(gameTime);
                            if (currentMinigame == null)
                            {
                                UpdateViewPort(overrideFreeze: false, getViewportCenter());
                            }
                            else
                            {
                                previousViewportPosition.X = viewport.X;
                                previousViewportPosition.Y = viewport.Y;
                            }
                            UpdateOther(gameTime);
                        }
                        if (messagePause)
                        {
                            KeyboardState tmp = GetKeyboardState();
                            MouseState tmp2 = input.GetMouseState();
                            GamePadState tmp3 = input.GetGamePadState();
                            if (isOneOfTheseKeysDown(tmp, options.actionButton) && !isOneOfTheseKeysDown(oldKBState, options.actionButton))
                            {
                                pressActionButton(tmp, tmp2, tmp3);
                            }
                            oldKBState = tmp;
                            oldPadState = tmp3;
                        }
                    }
                }
                else if (textEntry != null)
                {
                    PushUIMode();
                    updateTextEntry(gameTime);
                    PopUIMode();
                }
            }
            else
            {
                UpdateTitleScreen(gameTime);
                if (textEntry != null)
                {
                    PushUIMode();
                    updateTextEntry(gameTime);
                    PopUIMode();
                }
                else if (activeClickableMenu != null)
                {
                    PushUIMode();
                    updateActiveMenu(gameTime);
                    PopUIMode();
                }
                if (gameMode == 10)
                {
                    UpdateOther(gameTime);
                }
            }
            if (audioEngine != null)
            {
                audioEngine.Update();
            }
            UpdateChatBox();
            if (gameMode != 6)
            {
                multiplayer.UpdateLate();
            }
        }
        */
    }
}
