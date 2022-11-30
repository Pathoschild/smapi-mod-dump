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
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace InputTools
{
    /// <summary>
    /// The API which lets other mods interact with InputTools.
    ///
    /// Some basic concepts:
    ///     - A single button interaction is usually referred to as "Button" here
    ///     - A double button interaction (e.g. Ctrl+Space) is usually referred to as "Button Pair" here
    ///     - An Action refers to custom-defined actions that can be triggered by inputs such as "Jump" or "Navigate Left"
    ///
    /// More complex concepts:
    ///     - An Input Layer (sometimes just Layer) refers an object where all the input listeners (ButtonPressed, CursorMoved, etc)
    ///       resides. Multiple input layers are arranged in a "Stack", where each of layer which can be turned off and on
    ///       at will for more complex input setups
    ///     - "Global" is a special Input Layer which always sits above all the other layers, and is generally considered the
    ///       default (and only) Input Layer for most use-cases
    /// </summary>
    public interface IInputToolsAPI
    {
        /// <summary>Enum for input devices.</summary>
        public enum InputDevice
        {
            None,
            Mouse,
            Keyboard,
            Controller
        }

        /// <summary>Enum to specify whether or not a layer blocks inputs from going to the layer below.</summary>
        public enum BlockBehavior
        {
            None,
            Block,
            PassBelow
        }

        /// <summary>Event fired when the keybinding config changes</summary>
        public event EventHandler KeybindingConfigChanged;
        /// <summary>Event fired when the last input device changes e.g. from keyboard to mouse or controller</summary>
        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;

        /// <summary>Gets the most-recently active input device</summary>
        /// <returns>The input device enum</returns>
        public IInputToolsAPI.InputDevice GetCurrentInputDevice();

        /// <summary>Checks if keybinding has changed in SDV config. Known issue: Reset button doesn't register.</summary>
        /// <returns>If keybinging has changed in config</returns>
        public bool IsKeybindingConfigChanged();

        /// <summary>List the other mods that have also loaded InputTools.</summary>
        /// <returns>Returns a list of unique mod ID.</returns>
        public List<string> GetListOfModIDs();

        /// <summary>Gets the input device of a specific button.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public IInputToolsAPI.InputDevice GetInputDevice(SButton button);

        /// <summary>Checks if a button is the Confirm action. SDV keybindings and Controller A buttons are assigned to Confirm.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsConfirmButton(SButton button);

        /// <summary>Checks if a button is the Cancel action. SDV keybindings and Controller B buttons are assigned to Cancel.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsCancelButton(SButton button);

        /// <summary>Checks if a button is the Alt action. SDV keybindings and Controller X buttons are assigned to Alt.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsAltButton(SButton button);

        /// <summary>Checks if a button is the Menu action. SDV keybindings and Controller Y buttons are assigned to Menu.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsMenuButton(SButton button);

        /// <summary>Checks if a button is the Move-Right action. SDV keybindings and Controller DPad/Left-Thumbstick Right buttons are assigned to Move-Right.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsMoveRightButton(SButton button);

        /// <summary>Checks if a button is the Move-Down action. SDV keybindings and Controller DPad/Left-Thumbstick Down buttons are assigned to Move-Down.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsMoveDownButton(SButton button);

        /// <summary>Checks if a button is the Move-Left action. SDV keybindings and Controller DPad/Left-Thumbstick Left buttons are assigned to Move-Left.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>True if yes.</returns>
        public bool IsMoveLeftButton(SButton button);

        /// <summary>Checks if a button is the Move-Up action. SDV keybindings and Controller DPad/Left-Thumbstick Up buttons are assigned to Move-Up.</summary>
        /// <param name="button">The button enum.</param>
        /// <returns>Returns the input device the button is from if a valid button, and InputDevice.None is invalid.</returns>
        public bool IsMoveUpButton(SButton button);

        /// <summary>Shows a virtual keyboard to get text from user</summary>
        /// <param name="finishedCallback">Callback with the text when keyboard is closed. Text is null if user cancelled instead of confirmed.</param>
        /// <param name="updateCallback">Callback with the text at every letter update by user.</param>
        /// <param name="textboxWidth">Width of the input text box above the keyboard.</param>
        /// <param name="initialText">Initial text inside the input text box.</param>
        public void GetTextFromVirtualKeyboard(Action<string> finishedCallback, Action<string> updateCallback = null, int? textboxWidth = 300, string initialText = "");

        /// <summary>Force-closes the virtual keyboard before the user finishes their text entry.</summary>
        public void CloseVirtualKeyboard();

        /// <summary>Listens for the next keystroke (can be two keys). Useful to get keybinding from user.</summary>
        /// <param name="keyBindingCallback">Callback function with argument Tuple<SButton, SButton> when keybinding is done. Null is sent if user pressed Cancel instead.</param>
        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback);

        /// <summary>Stops listening for keybinding before user chooses one.</summary>
        public void StopListeningForKeybinding();

        /// <summary>Registers a new input action (e.g. Jump, NavigateLeft, etc) or append an existing one</summary>
        /// <param name="actionID">The ID of the new input action.</param>
        /// <param name="keyTriggers">List of buttons that will trigger this action</param>
        public void RegisterAction(string actionID, params SButton[] keyTriggers);

        /// <summary>Registers a new input action (e.g. Jump, NavigateLeft, etc) or append an existing one</summary>
        /// <param name="actionID">The ID of the new input action.</param>
        /// <param name="keyTriggers">List of button pairs (e.g. Ctrl+Space) that will trigger this action</param>
        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers);

        /// <summary>Unregisters a previously registered input action.</summary>
        /// <param name="actionID">The ID of the input action to delete.</param>
        public void UnregisterAction(string actionID);

        /// <summary>Gets a list of actions that will be triggered by a button</summary>
        /// <param name="key">The button that will trigger the actions</param>
        /// <returns>The list of actions</returns>
        public List<string> GetActionsFromKey(SButton key);

        /// <summary>Gets a list of actions that will be triggered by a button pair</summary>
        /// <param name="key">The button pair that will trigger the actions</param>
        /// <returns>The list of actions</returns>
        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair);

        /// <summary>Gets a list of buttons pairs that an action is triggered by</summary>
        /// <param name="actionID">The action ID</param>
        /// <returns>The list of button pairs</returns>
        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID);

        /// <summary>Creates a new custom Input Layer (i.e. object where all input listeners and methods are) at the top of the stack</summary>
        /// <param name="layerKey">Layer ID, can be any object</param>
        /// <param name="startActive">Whether or not input events are processed</param>
        /// <param name="block">Whether or not input events are sent down to the layer below</param>
        /// <returns>The created Input Layer</returns>
        public IInputToolsAPI.IInputLayer CreateLayer(object layerKey, bool startActive = true, IInputToolsAPI.BlockBehavior block = IInputToolsAPI.BlockBehavior.Block);

        /// <summary>Removes Input Layer at the top of the stack. This will disconnect any listeners as well.</summary>
        /// <returns>The popped Input Layer. If the stack is empty it returns null.</returns>
        public IInputToolsAPI.IInputLayer PopLayer();

        /// <summary>Removes a custom Input Layer. This will disconnect any listeners as well.</summary>
        /// <param name="layerKey">Layer ID, can be any object</param>
        public void RemoveLayer(object layerKey);

        /// <summary>Gets Input Layer at the top of the stack.</summary>
        /// <returns>The peeked Input Layer. If the stack is empty it returns null.</returns>
        public IInputToolsAPI.IInputLayer PeekLayer();

        /// <summary>Gets an Input Layer</summary>
        /// <param name="layerKey">Layer ID, can be any object</param>
        /// <returns>The Input Layer from your stack if layerKey is null. If layerKey is null, returns the Global Input Layer</returns>
        public IInputToolsAPI.IInputLayer GetLayer(object layerKey);

        /// <summary>The "default" Input Layer that sits above all the custom layers</summary>
        public IInputLayer Global { get; }

        /// <summary>
        /// An Input Layer (sometimes just Layer) refers an object where all the input listeners (ButtonPressed, CursorMoved, etc)
        /// resides. Multiple input layers are arranged in a "Stack", where each of layer which can be turned off and on at will
        /// for more complex input setups
        ///
        /// Note: if an Input Layer is disabled, it doesn't fire ANY events
        /// </summary>
        public interface IInputLayer
        {
            /// <summary>Event fired when a single button is pressed this tick</summary>
            public event EventHandler<SButton> ButtonPressed;
            /// <summary>Event fired when a single button is held since last tick</summary>
            public event EventHandler<SButton> ButtonHeld;
            /// <summary>Event fired when a single button is released this tick</summary>
            public event EventHandler<SButton> ButtonReleased;

            /// <summary>Event fired when a button pair is pressed this tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairPressed;
            /// <summary>Event fired when a button pair is held since last tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairHeld;
            /// <summary>Event fired when a button pair (either button) is released this tick. Always fired before the single button event</summary>
            public event EventHandler<Tuple<SButton, SButton>> ButtonPairReleased;

            /// <summary>Event fired when the Confirm action is pressed this tick</summary>
            public event EventHandler<SButton> ConfirmPressed;
            /// <summary>Event fired when the Confirm action is held since last tick</summary>
            public event EventHandler<SButton> ConfirmHeld;
            /// <summary>Event fired when the Confirm action is released this tick</summary>
            public event EventHandler<SButton> ConfirmReleased;

            /// <summary>Event fired when the Cancel action is pressed this tick</summary>
            public event EventHandler<SButton> CancelPressed;
            /// <summary>Event fired when the Cancel action is held since last tick</summary>
            public event EventHandler<SButton> CancelHeld;
            /// <summary>Event fired when the Cancel action is released this tick</summary>
            public event EventHandler<SButton> CancelReleased;

            /// <summary>Event fired when the Alt action is pressed this tick</summary>
            public event EventHandler<SButton> AltPressed;
            /// <summary>Event fired when the Alt action is held since last tick</summary>
            public event EventHandler<SButton> AltHeld;
            /// <summary>Event fired when the Alt action is released this tick</summary>
            public event EventHandler<SButton> AltReleased;

            /// <summary>Event fired when the Menu action is pressed this tick</summary>
            public event EventHandler<SButton> MenuPressed;
            /// <summary>Event fired when the Menu action is held since last tick</summary>
            public event EventHandler<SButton> MenuHeld;
            /// <summary>Event fired when the Menu action is released this tick</summary>
            public event EventHandler<SButton> MenuReleased;

            /// <summary>Event fired when the MoveRight action is pressed this tick</summary>
            public event EventHandler<SButton> MoveRightPressed;
            /// <summary>Event fired when the MoveRight action is held since last tick</summary>
            public event EventHandler<SButton> MoveRightHeld;
            /// <summary>Event fired when the MoveRight action is released this tick</summary>
            public event EventHandler<SButton> MoveRightReleased;

            /// <summary>Event fired when the MoveDown action is pressed this tick</summary>
            public event EventHandler<SButton> MoveDownPressed;
            /// <summary>Event fired when the MoveDown action is held since last tick</summary>
            public event EventHandler<SButton> MoveDownHeld;
            /// <summary>Event fired when the MoveDown action is released this tick</summary>
            public event EventHandler<SButton> MoveDownReleased;

            /// <summary>Event fired when the MoveLeft action is pressed this tick</summary>
            public event EventHandler<SButton> MoveLeftPressed;
            /// <summary>Event fired when the MoveLeft action is held since last tick</summary>
            public event EventHandler<SButton> MoveLeftHeld;
            /// <summary>Event fired when the MoveLeft action is released this tick</summary>
            public event EventHandler<SButton> MoveLeftReleased;

            /// <summary>Event fired when the MoveUp action is pressed this tick</summary>
            public event EventHandler<SButton> MoveUpPressed;
            /// <summary>Event fired when the MoveUp action is held since last tick</summary>
            public event EventHandler<SButton> MoveUpHeld;
            /// <summary>Event fired when the MoveUp action is released this tick</summary>
            public event EventHandler<SButton> MoveUpReleased;

            /// <summary>Event fired when buttons that can trigger the action is pressed this tick</summary>
            public event EventHandler<string> ActionPressed;
            /// <summary>Event fired when buttons that can trigger the action is held since last tick</summary>
            public event EventHandler<string> ActionHeld;
            /// <summary>Event fired when buttons that can trigger the action is released this tick</summary>
            public event EventHandler<string> ActionReleased;

            /// <summary>Event fired when buttons that moves the player is pressed this tick</summary>
            public event EventHandler<Vector2> MoveAxisPressed;
            /// <summary>Event fired when buttons that moves the player is held since last tick</summary>
            public event EventHandler<Vector2> MoveAxisHeld;
            /// <summary>Event fired when buttons that moves the player is released this tick</summary>
            public event EventHandler<Vector2> MoveAxisReleased;

            /// <summary>Event fired when the mouse wheel is moved this tick</summary>
            public event EventHandler<Vector2> MouseWheelMoved;
            /// <summary>Event fired when the mouse cursor is moved this tick</summary>
            public event EventHandler<IInputToolsAPI.InputDevice> CursorMoved;
            /// <summary>Event fired when the placement tile has changed this tick</summary>
            public event EventHandler<Vector2> PlacementTileChanged;
            /// <summary>Event fired when the held item changed this tick</summary>
            public event EventHandler<Item> PlacementItemChanged;

            /// <summary>Event fired at every tick</summary>
            public event EventHandler<UpdateTickedEventArgs> LayerUpdateTicked;

            /// <summary>Reference to the layer ID</summary>
            public object layerKey { get; }
            /// <summary>Is the active i.e. firing input events</summary>
            public bool isActive { get; }
            /// <summary>Block behaviour of layer i.e. is it stopping inputs from continuing down the stack</summary>
            public IInputToolsAPI.BlockBehavior block { get; }

            /// <summary>Gets the layer directly below this one.</summary>
            /// <param name="stopAtBlock">If false ignore the block settings and always get the layer below</param>
            /// <returns>The layer below if valid, null if not</returns>
            public IInputToolsAPI.IInputLayer GetBelow(bool stopAtBlock = true);


            /// <summary>Checks if a button is pressed this tick. Always false if layer is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's pressed this tick</returns>
            public bool IsButtonPressed(SButton button);

            /// <summary>Checks if a button is held since last tick. Always false if layer is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's being held since last tick</returns>
            public bool IsButtonHeld(SButton button);

            /// <summary>Checks if a button is released this tick. Always false if layer is inactive.</summary>
            /// <param name="button">The button to check</param>
            /// <returns>true if it's released this tick</returns>
            public bool IsButtonReleased(SButton button);

            /// <summary>Checks if a button pair is pressed this tick. Always false if layer is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's pressed this tick</returns>
            public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if a button pair is held since last tick. Always false if layer is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's being held since last tick</returns>
            public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if a button pair is released this tick. Always false if layer is inactive</summary>
            /// <param name="buttonPair">The button pair</param>
            /// <returns>true if it's released this tick</returns>
            public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair);

            /// <summary>Checks if the mouse wheel is moved this tick. Always false if layer is inactive</summary>
            /// <returns>true if mouse wheel moved this tick</returns>
            public bool IsMouseWheelMoved();

            /// <summary>Checks if the cursor is moved this tick. Always None if layer is inactive</summary>
            /// <returns>The device movement was from this tick. If both, Controller is returned. If not moved, None.</returns>
            public IInputToolsAPI.InputDevice IsCursorMoved();

            /// <summary>Checks if the currently held item is a bomb. Used for placement tile calculattion.</summary>
            /// <returns>true is a bomb</returns>
            public bool IsHeldItemBomb();

            /// <summary>Checks if the last calculated placement was from the mouse cursor</summary>
            /// <returns>true is cursor, false if it's the farmer i.e. controller movement</returns>
            public bool IsPlacementTileFromCursor();

            /// <summary>Checks if placement tile had changed this tick. Always false if layer is inactive.</summary>
            /// <returns></returns>
            public bool IsPlacementTileChanged();

            /// <summary>Checks if the Confirm action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from.If not pressed, None.</returns>
            public SButton IsConfirmPressed();

            /// <summary>Checks if the Confirm action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. If not held, None.</returns>
            public SButton IsConfirmHeld();

            /// <summary>Checks if the Confirm action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. If not released, None.</returns>
            public SButton IsConfirmReleased();

            /// <summary>Checks if the Cancel action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. If not pressed, None.</returns>
            public SButton IsCancelPressed();

            /// <summary>Checks if the Cancel action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. If not held, None.</returns>
            public SButton IsCancelHeld();

            /// <summary>Checks if the Cancel action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. If not released, None.</returns>
            public SButton IsCancelReleased();

            /// <summary>Checks if the Alt action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. If not pressed, None.</returns>
            public SButton IsAltPressed();

            /// <summary>Checks if the Alt action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. If not held, None.</returns>
            public SButton IsAltHeld();

            /// <summary>Checks if the Alt action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. If not released, None.</returns>
            public SButton IsAltReleased();

            /// <summary>Checks if the Menu action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. If not pressed, None.</returns>
            public SButton IsMenuPressed();

            /// <summary>Checks if the Menu action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. If not held, None.</returns>
            public SButton IsMenuHeld();

            /// <summary>Checks if the Menu action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. If not released, None.</returns>
            public SButton IsMenuReleased();

            /// <summary>Checks if any Move action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. None is not pressed.</returns>
            public SButton IsMoveButtonPressed();

            /// <summary>Checks if any Move action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. None is not held.</returns>
            public SButton IsMoveButtonHeld();

            /// <summary>Checks if any Move action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. None is not released.</returns>
            public SButton IsMoveButtonReleased();

            /// <summary>Checks if Move-Right action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. None is not pressed.</returns>
            public SButton IsMoveRightPressed();

            /// <summary>Checks if Move-Right action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. None is not held.</returns>
            public SButton IsMoveRightHeld();

            /// <summary>Checks if Move-Right action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. None is not released.</returns>
            public SButton IsMoveRightReleased();

            /// <summary>Checks if Move-Down action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. None is not pressed.</returns>
            public SButton IsMoveDownPressed();

            /// <summary>Checks if Move-Down action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. None is not held.</returns>
            public SButton IsMoveDownHeld();

            /// <summary>Checks if Move-Down action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. None is not released.</returns>
            public SButton IsMoveDownReleased();

            /// <summary>Checks if Move-Left action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. None is not pressed.</returns>
            public SButton IsMoveLeftPressed();

            /// <summary>Checks if Move-Left action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. None is not held.</returns>
            public SButton IsMoveLeftHeld();

            /// <summary>Checks if Move-Left action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. None is not released.</returns>
            public SButton IsMoveLeftReleased();

            /// <summary>Checks if Move-Up action was pressed this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was pressed from. None is not pressed.</returns>
            public SButton IsMoveUpPressed();

            /// <summary>Checks if Move-Up action was held since last tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was held from. None is not held.</returns>
            public SButton IsMoveUpHeld();

            /// <summary>Checks if Move-Up action was released this tick. Always None if layer is inactive.</summary>
            /// <returns>The button it was released from. None is not released.</returns>
            public SButton IsMoveUpReleased();

            /// <summary>Checks if a custom action was pressed this tick. Always false if layer is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair it was pressed from. Null if not pressed.</returns>
            public Tuple<SButton, SButton> IsActionPressed(string actionID);

            /// <summary>Checks if a custom action was held since last tick. Always false if layer is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair it was held with. Null if not held.</returns>
            public Tuple<SButton, SButton> IsActionHeld(string actionID);

            /// <summary>Checks if a custom action was released this tick. Always false if layer is inactive.</summary>
            /// <param name="actionID">ID of the registered action</param>
            /// <returns>The button pair it was released from. Null if not released.</returns>
            public Tuple<SButton, SButton> IsActionReleased(string actionID);

            /// <summary>Gets the Move action axis values. Note that it's sensitive to thumbstick distance/velocity.</summary>
            /// <returns>X and Y are either -1 (left, up), 0 (center), or +1 (right, down).</returns>
            public Vector2 GetMoveAxis();

            /// <summary>Gets the unscaled screen pixels position of the cursor</summary>
            /// <returns>Unscaled screen pixels position</returns>
            public Vector2 GetCursorScreenPos();

            /// <summary>Gets the tile position under the cursor</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetCursorTilePos();

            /// <summary>Gets the mouse wheel position</summary>
            /// <returns>X is the horizontal scroll and Y the vertical scroll in integer since game start</returns>
            public Vector2 GetMouseWheelPos();

            /// <summary>Gets the tile position of where a held item will be placed. Automatically corrected if controller is used.</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetPlacementTile();

            /// <summary>Gets the tile position of where a held item will be placed assuming a controller is used.</summary>
            /// <returns>X and Y in integers</returns>
            public Vector2 GetPlacementTileWithController();

            /// <summary>Sets this Input Layer active to receive inputs, or inactive to ignore inputs.</summary>
            /// <param name="active">True if active</param>
            public void SetActive(bool active);

            /// <summary>Sets this Input Stack's block behaviour to determine whether to pass inputs to the layer below.</summary>
            /// <param name="block">Block stops inputs from continuing, PassBelow allows inputs to continue downwards.</param>
            public void SetBlock(IInputToolsAPI.BlockBehavior block);

            /// <summary>Moves this Input Layer to the top of the stack (but still below the Global layer) so that it can process inputs before the other custom layers.</summary>
            public void MoveToTop();

            /// <summary>Checks if this Input Layer is reachable or blocked by a layer above. Also automatically false if inactive.</summary>
            /// <returns>True if layer is reachable and active i.e. processing inputs.</returns>
            public bool IsReachableByInput();

            /// <summary>Deletes this Input Layer.</summary>
            public void RemoveSelf();
        }
    }
}
