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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using InputTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace InputTools
{
    public class ModConfig
    {
        public bool HideCursorInstantlyWhenControllerUsed { get; set; } = true;
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public Dictionary<string, InputToolsAPI> inputTools = new Dictionary<string, InputToolsAPI>();
        public ModConfig Config;

        internal List<SButton> confirmButtons = new List<SButton>();
        internal List<SButton> cancelButtons = new List<SButton>();
        internal List<SButton> altButtons = new List<SButton>();
        internal List<SButton> menuButtons = new List<SButton>();
        internal List<SButton> moveRightButtons = new List<SButton>();
        internal List<SButton> moveDownButtons = new List<SButton>();
        internal List<SButton> moveLeftButtons = new List<SButton>();
        internal List<SButton> moveUpButtons = new List<SButton>();

        internal List<SButton> buttonsPressing = new List<SButton>();
        internal List<Tuple<SButton, SButton>> buttonPairsPressing = new List<Tuple<SButton, SButton>>();
        internal List<Tuple<SButton, SButton>> buttonPairsReleased = new List<Tuple<SButton, SButton>>();
        internal int tickButtonPairsReleased;
        internal IInputToolsAPI.InputDevice lastInputDevice;
        internal Vector2 lastCursorScreenPixels;
        internal Vector2 lastTileHighlightPos;
        internal Item lastItemHeld;
        internal bool isLastPlacementTileFromCursor;
        internal bool isFarmerMovedLastTick;
        internal bool isCursorMovedLastTick;
        internal bool isPlacementTileMovedLastTick;
        internal bool isItemChangedLastTick;
        internal Vector2 moveAxisLastTick;
        internal int lastRebindTick = 0;

        internal IInputToolsAPI.InputDevice lastInputDeviceUsed = IInputToolsAPI.InputDevice.None;
        internal bool lastConfigChanged = false;
        internal int lastTickInputDeviceUpdated;
        internal int lastTickConfigUpdated;
        internal Vector2 lastMousePos;
        internal int lastScrollWheelPos;
        internal int lastHorizontalScrollWheelPos;
        internal bool mouseMovedLastTick;
        internal bool mouseWheelMovedLastTick;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            this.Helper.Events.GameLoop.GameLaunched += new EventHandler<GameLaunchedEventArgs>((s, e) =>
            {
                // get Generic Mod Config Menu's API (if it's installed)
                try
                {
                    GenericModConfigMenu.IGenericModConfigMenuApi configMenu = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                    if (configMenu is null)
                        return;

                    // register mod
                    configMenu.Register(
                        mod: this.ModManifest,
                        reset: () => this.Config = new ModConfig(),
                        save: () => this.Helper.WriteConfig(this.Config)
                    );

                    // add some config options
                    configMenu.AddBoolOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.HideCursorInstantlyWhenControllerUsed.name"),
                        tooltip: () => this.Helper.Translation.Get("config.HideCursorInstantlyWhenControllerUsed.tooltip"),
                        getValue: () => this.Config.HideCursorInstantlyWhenControllerUsed,
                        setValue: value => this.Config.HideCursorInstantlyWhenControllerUsed = value
                    );
                }
                catch (Exception exception)
                {
                    this.Monitor.Log($"Failed to load spacechase0.GenericModConfigMenu. Reason: {exception.Message}", LogLevel.Error);
                    this.Config = new ModConfig();
                }

                this.ReloadKeybindings();
            });
            this.Helper.Events.GameLoop.SaveLoaded += new EventHandler<SaveLoadedEventArgs>((s, e) =>
            {
                this.ReloadKeybindings();
            });

            this.Config = this.Helper.ReadConfig<ModConfig>();

        }

        public override object GetApi(IModInfo mod)
        {
            this.inputTools[mod.Manifest.UniqueID] = new InputToolsAPI(this);
            return this.inputTools[mod.Manifest.UniqueID];
        }

        public List<string> GetListOfModIDs()
        {
            return new List<string>(this.inputTools.Keys);
        }

        public InputToolsAPI GetInstanceFromAnotherMod(string uniqueModID)
        {
            if (this.inputTools.ContainsKey(uniqueModID))
               return this.inputTools[uniqueModID];
            return null;
        }

        public IInputToolsAPI.InputDevice GetCurrentInputDevice()
        {
            if (this.lastTickInputDeviceUpdated == Game1.ticks)
                return this.lastInputDeviceUsed;
            this.lastTickInputDeviceUpdated = Game1.ticks;

            IInputToolsAPI.InputDevice newInputDevice = this.lastInputDeviceUsed;
            if (Game1.isAnyGamePadButtonBeingPressed() || Game1.isAnyGamePadButtonBeingHeld() || Game1.isGamePadThumbstickInMotion())
                newInputDevice = IInputToolsAPI.InputDevice.Controller;
            else if (Game1.GetKeyboardState().GetPressedKeyCount() > 0)
                newInputDevice = IInputToolsAPI.InputDevice.Keyboard;
            else if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed
                || Game1.input.GetMouseState().MiddleButton == ButtonState.Pressed
                || Game1.input.GetMouseState().RightButton == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton1 == ButtonState.Pressed
                || Game1.input.GetMouseState().XButton2 == ButtonState.Pressed
                || (Game1.input.GetMouseState().Position.ToVector2() != this.lastMousePos && Game1.lastCursorMotionWasMouse)
                || Game1.input.GetMouseState().ScrollWheelValue != this.lastScrollWheelPos
                || Game1.input.GetMouseState().HorizontalScrollWheelValue != this.lastHorizontalScrollWheelPos
                )
                newInputDevice = IInputToolsAPI.InputDevice.Mouse;

            this.mouseMovedLastTick = this.lastMousePos != Game1.input.GetMouseState().Position.ToVector2();
            this.mouseWheelMovedLastTick = (this.lastScrollWheelPos != Game1.input.GetMouseState().ScrollWheelValue) || (this.lastHorizontalScrollWheelPos != Game1.input.GetMouseState().HorizontalScrollWheelValue);

            this.lastMousePos = Game1.input.GetMouseState().Position.ToVector2();
            this.lastScrollWheelPos = Game1.input.GetMouseState().ScrollWheelValue;
            this.lastHorizontalScrollWheelPos = Game1.input.GetMouseState().HorizontalScrollWheelValue;
            if (this.lastInputDeviceUsed != newInputDevice)
            {
                this.lastInputDeviceUsed = newInputDevice;
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api.OnInputDeviceChanged(newInputDevice);
            }
            else
                this.lastInputDeviceUsed = newInputDevice;
            if (this.mouseMovedLastTick)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnCursorMoved(newInputDevice);
            }
            if (this.mouseWheelMovedLastTick)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMouseWheelMoved(this.GetMouseWheelPos());
            }
            return this.lastInputDeviceUsed;
        }

        public bool IsKeybindingConfigChanged()
        {
            if (this.lastTickConfigUpdated == Game1.ticks)
                return this.lastConfigChanged;
            this.lastTickConfigUpdated = Game1.ticks;

            this.lastConfigChanged = false;
            OptionsPage page = (Game1.activeClickableMenu as GameMenu)?.GetCurrentPage() as OptionsPage;
            if (page != null && this.lastRebindTick != page.lastRebindTick)
            {
                this.lastConfigChanged = true;
                this.lastRebindTick = page.lastRebindTick;
                this.ReloadKeybindings();
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api.OnKeybindingConfigChanged();
            }
            return this.lastConfigChanged;
        }

        internal void ReloadOneKeybinding(ref List<SButton> outButtons, string id, InputButton[] sdvButtons, params SButton[] controllerButtons)
        {
            if (outButtons == null)
                outButtons = new List<SButton>();
            bool hasController = false;
            outButtons.Clear();
            foreach (InputButton b in sdvButtons)
            {
                outButtons.Add(b.ToSButton());
                if (this.GetInputDevice(outButtons[outButtons.Count - 1]) == IInputToolsAPI.InputDevice.Controller)
                    hasController = true;
            }
            if (!hasController && controllerButtons != null && controllerButtons.Length > 0)
                outButtons.AddRange(controllerButtons);

            this.Monitor.Log($"{id}: {string.Join('/', outButtons)}", LogLevel.Debug);
        }

        internal void ReloadKeybindings()
        {
            this.ReloadOneKeybinding(ref this.confirmButtons, "Confirm", Game1.options.actionButton, SButton.ControllerA);
            this.ReloadOneKeybinding(ref this.cancelButtons, "Cancel", Game1.options.menuButton, SButton.ControllerB);
            this.ReloadOneKeybinding(ref this.altButtons, "Alt", Game1.options.useToolButton, SButton.ControllerX);
            this.ReloadOneKeybinding(ref this.menuButtons, "Menu", Game1.options.menuButton, SButton.ControllerY);
            this.ReloadOneKeybinding(ref this.moveRightButtons, "MoveRight", Game1.options.moveRightButton, SButton.DPadRight, SButton.LeftThumbstickRight);
            this.ReloadOneKeybinding(ref this.moveDownButtons, "MoveDown", Game1.options.moveDownButton, SButton.DPadDown, SButton.LeftThumbstickDown);
            this.ReloadOneKeybinding(ref this.moveLeftButtons, "MoveLeft", Game1.options.moveLeftButton, SButton.DPadLeft, SButton.LeftThumbstickLeft);
            this.ReloadOneKeybinding(ref this.moveUpButtons, "MoveUp", Game1.options.moveUpButton, SButton.DPadUp, SButton.LeftThumbstickUp);
        }

        public IInputToolsAPI.InputDevice GetInputDevice(SButton button)
        {
            if (button == SButton.None)
                return IInputToolsAPI.InputDevice.None;
            if (SButtonExtensions.TryGetController(button, out _))
                return IInputToolsAPI.InputDevice.Controller;
            if (SButtonExtensions.TryGetKeyboard(button, out _))
                return IInputToolsAPI.InputDevice.Keyboard;
            return IInputToolsAPI.InputDevice.Mouse;
        }

        public bool IsConfirmButton(SButton button)
        {
            return this.confirmButtons.Contains(button);
        }

        public bool IsCancelButton(SButton button)
        {
            return this.cancelButtons.Contains(button);
        }

        public bool IsAltButton(SButton button)
        {
            return this.altButtons.Contains(button);
        }

        public bool IsMenuButton(SButton button)
        {
            return this.menuButtons.Contains(button);
        }

        public bool IsMoveRightButton(SButton button)
        {
            return this.moveRightButtons.Contains(button);
        }

        public bool IsMoveDownButton(SButton button)
        {
            return this.moveDownButtons.Contains(button);
        }

        public bool IsMoveLeftButton(SButton button)
        {
            return this.moveLeftButtons.Contains(button);
        }

        public bool IsMoveUpButton(SButton button)
        {
            return this.moveUpButtons.Contains(button);
        }

        public bool IsButtonPressed(SButton button)
        {
            return this.Helper.Input.GetState(button) == SButtonState.Pressed;
        }

        public bool IsButtonHeld(SButton button)
        {
            return this.Helper.Input.GetState(button) == SButtonState.Held;
        }

        public bool IsButtonReleased(SButton button)
        {
            return this.Helper.Input.GetState(button) == SButtonState.Released;
        }

        internal SButton IsAnyOfTheButtonsPressed(List<SButton> buttons)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (this.IsButtonPressed(buttons[i]))
                    return buttons[i];
            }
            return SButton.None;
        }

        internal SButton IsAnyOfTheButtonsHeld(List<SButton> buttons)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (this.IsButtonHeld(buttons[i]))
                    return buttons[i];
            }
            return SButton.None;
        }

        internal SButton IsAnyOfTheButtonsReleased(List<SButton> buttons)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (this.IsButtonReleased(buttons[i]))
                    return buttons[i];
            }
            return SButton.None;
        }

        public SButton IsMoveButtonHeld()
        {
            SButton button = this.IsMoveRightHeld();
            if (button != SButton.None)
                return button;
            button = this.IsMoveDownHeld();
            if (button != SButton.None)
                return button;
            button = this.IsMoveLeftHeld();
            if (button != SButton.None)
                return button;
            button = this.IsMoveUpHeld();
            if (button != SButton.None)
                return button;
            return SButton.None;
        }

        public SButton IsMoveRightPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.moveRightButtons);
        }

        public SButton IsMoveRightHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.moveRightButtons);
        }

        public SButton IsMoveRightReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.moveRightButtons);
        }

        public SButton IsMoveDownPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.moveDownButtons);
        }

        public SButton IsMoveDownHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.moveDownButtons);
        }

        public SButton IsMoveDownReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.moveDownButtons);
        }

        public SButton IsMoveLeftPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.moveLeftButtons);
        }

        public SButton IsMoveLeftHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.moveLeftButtons);
        }

        public SButton IsMoveLeftReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.moveLeftButtons);
        }

        public SButton IsMoveUpPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.moveUpButtons);
        }

        public SButton IsMoveUpHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.moveUpButtons);
        }

        public SButton IsMoveUpReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.moveUpButtons);
        }

        public Vector2 GetMouseWheelPos()
        {
            return new Vector2(Game1.input.GetMouseState().HorizontalScrollWheelValue, Game1.input.GetMouseState().ScrollWheelValue);
        }

        public Vector2 GetMoveAxis()
        {
            SButton moveRight = this.IsMoveRightHeld();
            if (moveRight == SButton.LeftThumbstickRight)
                return Game1.input.GetGamePadState().ThumbSticks.Left;
            if (moveRight == SButton.RightThumbstickRight)
                return Game1.input.GetGamePadState().ThumbSticks.Right;
            SButton moveDown = this.IsMoveDownHeld();
            if (moveDown == SButton.LeftThumbstickDown)
                return Game1.input.GetGamePadState().ThumbSticks.Left;
            if (moveDown == SButton.RightThumbstickDown)
                return Game1.input.GetGamePadState().ThumbSticks.Right;
            SButton moveLeft = this.IsMoveLeftHeld();
            if (moveLeft == SButton.LeftThumbstickLeft)
                return Game1.input.GetGamePadState().ThumbSticks.Left;
            if (moveLeft == SButton.RightThumbstickLeft)
                return Game1.input.GetGamePadState().ThumbSticks.Right;
            SButton moveUp = this.IsMoveUpHeld();
            if (moveUp == SButton.LeftThumbstickUp)
                return Game1.input.GetGamePadState().ThumbSticks.Left;
            if (moveUp == SButton.RightThumbstickUp)
                return Game1.input.GetGamePadState().ThumbSticks.Right;

            return new Vector2(moveRight != SButton.None ? 1 : (moveLeft != SButton.None ? -1 : 0),
                moveDown != SButton.None ? 1 : (moveUp != SButton.None ? -1 : 0));
        }

        public bool IsHeldItemBomb()
        {
            Item item = Game1.player.CurrentItem;
            if (item == null)
                return false;
            int itemID = item.ParentSheetIndex;
            return Utility.IsNormalObjectAtParentSheetIndex(item, itemID) && (itemID == 286 || itemID == 287 || itemID == 288);
        }

        public Vector2 GetPlacementTileWithController()
        {
            Vector2 grabTile = Game1.player.GetGrabTile();
            bool previousValue = Game1.isCheckingNonMousePlacement;
            Game1.isCheckingNonMousePlacement = true;
            Vector2 placementTile = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, Game1.player.CurrentItem, (int)grabTile.X * 64 + 32, (int)grabTile.Y * 64 + 32) / 64;
            Game1.isCheckingNonMousePlacement = previousValue;
            return placementTile;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.lastInputDevice = this.GetInputDevice(e.Button);

            if (this.buttonsPressing.Count > 0)
            {
                foreach (SButton heldButton in this.buttonsPressing)
                {
                    Tuple<SButton, SButton> buttonPair = new Tuple<SButton, SButton>(heldButton, e.Button);
                    if (!this.buttonPairsPressing.Contains(buttonPair))
                        this.buttonPairsPressing.Add(buttonPair);
                    foreach (InputToolsAPI api in this.inputTools.Values)
                    {
                        foreach (string groupID in api.actions.GetActionsFromKeyPair(buttonPair))
                            api._Global.OnActionPressed(groupID);
                        api._Global.OnButtonPairPressed(buttonPair);
                    }
                    //this.Monitor.Log($"{Game1.ticks} ButtonPairPressed {buttonPair}", LogLevel.Debug);
                }
            }
            if (!this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Add(e.Button);

            foreach (InputToolsAPI api in this.inputTools.Values)
            {
                api._Global.OnButtonPressed(e.Button);
                foreach (string groupID in api.actions.GetActionsFromKey(e.Button))
                    api._Global.OnActionPressed(groupID);
            }

            if (this.IsConfirmButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnConfirmPressed(e.Button);
            }
            if (this.IsCancelButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnCancelPressed(e.Button);
            }
            if (this.IsAltButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnAltPressed(e.Button);
            }
            if (this.IsMenuButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMenuPressed(e.Button);
            }

            if (this.IsMoveRightButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveRightPressed(e.Button);
            }
            if (this.IsMoveDownButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveDownPressed(e.Button);
            }
            if (this.IsMoveLeftButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveLeftPressed(e.Button);
            }
            if (this.IsMoveUpButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveUpPressed(e.Button);
            }

            Vector2 moveAxis = this.GetMoveAxis();
            if (this.moveAxisLastTick == Vector2.Zero && moveAxis != Vector2.Zero)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveAxisPressed(moveAxis);
            }
            this.moveAxisLastTick = moveAxis;

            //this.Monitor.Log($"{Game1.ticks} ButtonPressed {e.Button}", LogLevel.Debug);
        }

        internal void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            if (this.buttonPairsPressing.Count > 0)
            {
                this.buttonPairsReleased.Clear();
                this.tickButtonPairsReleased = Game1.ticks;
                foreach (Tuple<SButton, SButton> buttonPair in new List<Tuple<SButton, SButton>>(this.buttonPairsPressing))
                {
                    if (buttonPair.Item1 == e.Button || buttonPair.Item2 == e.Button)
                    {
                        this.buttonPairsReleased.Add(buttonPair);
                        this.buttonPairsPressing.Remove(buttonPair);
                        foreach (InputToolsAPI api in this.inputTools.Values)
                        {
                            api._Global.OnButtonPairReleased(buttonPair);
                            //this.Monitor.Log($"{Game1.ticks} ButtonPairRemoved {buttonPair}", LogLevel.Debug);
                            foreach (string groupID in api.actions.GetActionsFromKeyPair(buttonPair))
                                api._Global.OnActionReleased(groupID);
                        }
                    }
                }
            }

            if (this.buttonsPressing.Contains(e.Button))
                this.buttonsPressing.Remove(e.Button);
            foreach (InputToolsAPI api in this.inputTools.Values)
            {
                api._Global.OnButtonReleased(e.Button);
                foreach (string groupID in api.actions.GetActionsFromKey(e.Button))
                    api._Global.OnActionReleased(groupID);
            }

            if (this.IsConfirmButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnConfirmReleased(e.Button);
            }
            if (this.IsCancelButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnCancelReleased(e.Button);
            }
            if (this.IsAltButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnAltReleased(e.Button);
            }
            if (this.IsMenuButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMenuReleased(e.Button);
            }

            if (this.IsMoveRightButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveRightReleased(e.Button);
            }
            if (this.IsMoveDownButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveDownReleased(e.Button);
            }
            if (this.IsMoveLeftButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveLeftReleased(e.Button);
            }
            if (this.IsMoveUpButton(e.Button))
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveUpReleased(e.Button);
            }

            Vector2 moveAxis = this.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero && moveAxis == Vector2.Zero)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveAxisReleased(moveAxis);
            }
            this.moveAxisLastTick = Vector2.Zero;

            //this.Monitor.Log($"{Game1.ticks} ButtonReleased {e.Button}", LogLevel.Debug);
        }

        internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            //if (!Context.IsWorldReady)
            //    return;

            this.IsKeybindingConfigChanged();
            this.GetCurrentInputDevice();

            this.isFarmerMovedLastTick = Game1.player.lastPosition != Game1.player.Position;
            this.isCursorMovedLastTick = this.lastCursorScreenPixels != this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.lastCursorScreenPixels = this.Helper.Input.GetCursorPosition().ScreenPixels;
            this.isPlacementTileMovedLastTick = false;
            this.isItemChangedLastTick = this.lastItemHeld != Game1.player.CurrentItem;
            this.lastItemHeld = Game1.player.CurrentItem;
            if (this.isItemChangedLastTick)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnPlacementItemChanged(Game1.player.CurrentItem);
            }

            SButton moveButtonHeld = this.IsMoveButtonHeld();
            bool isKeyboardMoveButtonHeld = this.GetInputDevice(moveButtonHeld) == IInputToolsAPI.InputDevice.Keyboard;
            bool isControllerMoveButtonHeld = this.GetInputDevice(moveButtonHeld) == IInputToolsAPI.InputDevice.Controller;
            if ((this.Config.HideCursorInstantlyWhenControllerUsed || !Game1.wasMouseVisibleThisFrame) && (
                (!Game1.wasMouseVisibleThisFrame && this.isItemChangedLastTick) || (this.isFarmerMovedLastTick && !isKeyboardMoveButtonHeld)))
            {
                // If controller last used, placement tile is the grab tile i.e. tile in front of player
                Vector2 placementTile = this.GetPlacementTileWithController();
                if (this.Config.HideCursorInstantlyWhenControllerUsed)
                    Game1.timerUntilMouseFade = 0;
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != placementTile;
                if (this.isPlacementTileMovedLastTick)
                {
                    this.lastTileHighlightPos = placementTile;
                    this.isLastPlacementTileFromCursor = false;
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnPlacementTileChanged(this.lastTileHighlightPos);
                }
            }
            else if ((this.isLastPlacementTileFromCursor && this.isItemChangedLastTick) || this.isCursorMovedLastTick || this.isFarmerMovedLastTick)
            {
                // Otherwise placement tile is the tile under the cursor
                this.isPlacementTileMovedLastTick = this.lastTileHighlightPos != this.Helper.Input.GetCursorPosition().Tile;
                if (this.isPlacementTileMovedLastTick)
                {
                    this.lastTileHighlightPos = this.Helper.Input.GetCursorPosition().Tile;
                    this.isLastPlacementTileFromCursor = true;
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnPlacementTileChanged(this.lastTileHighlightPos);
                }
            }

            foreach (SButton button in this.buttonsPressing)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                {
                    api._Global.OnButtonHeld(button);
                    foreach (string groupID in api.actions.GetActionsFromKey(button))
                        api._Global.OnActionHeld(groupID);
                }

                if (this.IsConfirmButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnConfirmHeld(button);
                }
                if (this.IsCancelButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnCancelHeld(button);
                }
                if (this.IsAltButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnAltHeld(button);
                }
                if (this.IsMenuButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnMenuHeld(button);
                }

                if (this.IsMoveRightButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnMoveRightHeld(button);
                }
                if (this.IsMoveDownButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnMoveDownHeld(button);
                }
                if (this.IsMoveLeftButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnMoveLeftHeld(button);
                }
                if (this.IsMoveUpButton(button))
                {
                    foreach (InputToolsAPI api in this.inputTools.Values)
                        api._Global.OnMoveUpHeld(button);
                }
            }
            foreach (Tuple<SButton, SButton> buttonPair in this.buttonPairsPressing)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                {
                    api._Global.OnButtonPairHeld(buttonPair);
                    foreach (string groupID in api.actions.GetActionsFromKeyPair(buttonPair))
                        api._Global.OnActionHeld(groupID);
                }
            }

            this.moveAxisLastTick = this.GetMoveAxis();
            if (this.moveAxisLastTick != Vector2.Zero)
            {
                foreach (InputToolsAPI api in this.inputTools.Values)
                    api._Global.OnMoveAxisHeld(this.moveAxisLastTick);
            }

            foreach (InputToolsAPI api in this.inputTools.Values)
                api._Global.OnLayerUpdateTicked(e);
        }
    }
}
