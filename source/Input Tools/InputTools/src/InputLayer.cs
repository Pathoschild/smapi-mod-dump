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
using InputTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using static InputTools.IInputToolsAPI;

namespace InputTools
{
    public class InputLayer : IInputToolsAPI.IInputLayer
    {
        internal bool _isActive = true;
        internal IInputToolsAPI.BlockBehavior _block;

        private InputToolsAPI inputTools;
        internal InputLayer(InputToolsAPI inputTools, object layerKey)
        {
            this.inputTools = inputTools;
            this.layerKey = layerKey;
        }

        internal InputLayer GetLayerBelow(bool stopAtBlock = true)
        {
            if (this == this.inputTools.Global)
            {
                if (stopAtBlock && this.inputTools._Global._block == BlockBehavior.Block)
                    return null;
                if (this.inputTools.stack.layers.Count > 0)
                    return this.inputTools.stack.Get(this.inputTools.stack.layers[this.inputTools.stack.layers.Count - 1]) as InputLayer;
            }
            else if (!stopAtBlock || this._block == BlockBehavior.PassBelow)
            {
                for (int i = this.inputTools.stack.layers.Count - 1; i >= 0; i--)
                {
                    if (this.inputTools.stack.layers[i] == this.layerKey && i > 0)
                        return this.inputTools.stack.Get(this.inputTools.stack.layers[i - 1]) as InputLayer;
                }
            }
            return null;
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

        internal void OnButtonPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnButtonPressed(button);
        }

        internal void OnButtonHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnButtonHeld(button);
        }

        internal void OnButtonReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnButtonReleased(button);
        }

        internal void OnButtonPairPressed(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonPairPressed?.Invoke(this.layerKey, buttons);
            this.GetLayerBelow()?.OnButtonPairPressed(buttons);
        }

        internal void OnButtonPairHeld(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonPairHeld?.Invoke(this.layerKey, buttons);
            this.GetLayerBelow()?.OnButtonPairHeld(buttons);
        }

        internal void OnButtonPairReleased(Tuple<SButton, SButton> buttons)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ButtonPairReleased?.Invoke(this.layerKey, buttons);
            this.GetLayerBelow()?.OnButtonPairReleased(buttons);
        }

        internal void OnConfirmPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ConfirmPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnConfirmPressed(button);
        }

        internal void OnConfirmHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ConfirmHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnConfirmHeld(button);
        }

        internal void OnConfirmReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ConfirmReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnConfirmReleased(button);
        }

        internal void OnCancelPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                CancelPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnCancelPressed(button);
        }

        internal void OnCancelHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                CancelHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnCancelHeld(button);
        }

        internal void OnCancelReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                CancelReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnCancelReleased(button);
        }

        internal void OnAltPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                AltPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnAltPressed(button);
        }

        internal void OnAltHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                AltHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnAltHeld(button);
        }

        internal void OnAltReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                AltReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnAltReleased(button);
        }

        internal void OnMenuPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MenuPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMenuPressed(button);
        }

        internal void OnMenuHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MenuHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMenuHeld(button);
        }

        internal void OnMenuReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MenuReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMenuReleased(button);
        }

        internal void OnMoveRightPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveRightPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveRightPressed(button);
        }

        internal void OnMoveRightHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveRightHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveRightHeld(button);
        }

        internal void OnMoveRightReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveRightReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveRightReleased(button);
        }

        internal void OnMoveDownPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveDownPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveDownPressed(button);
        }

        internal void OnMoveDownHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveDownHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveDownHeld(button);
        }

        internal void OnMoveDownReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveDownReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveDownReleased(button);
        }

        internal void OnMoveLeftPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveLeftPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveLeftPressed(button);
        }

        internal void OnMoveLeftHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveLeftHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveLeftHeld(button);
        }

        internal void OnMoveLeftReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveLeftReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveLeftReleased(button);
        }

        internal void OnMoveUpPressed(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveUpPressed?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveUpPressed(button);
        }

        internal void OnMoveUpHeld(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveUpHeld?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveUpHeld(button);
        }

        internal void OnMoveUpReleased(SButton button)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveUpReleased?.Invoke(this.layerKey, button);
            this.GetLayerBelow()?.OnMoveUpReleased(button);
        }

        internal void OnActionPressed(string actionID)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ActionPressed?.Invoke(this.layerKey, actionID);
            this.GetLayerBelow()?.OnActionPressed(actionID);
        }

        internal void OnActionHeld(string actionID)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ActionHeld?.Invoke(this.layerKey, actionID);
            this.GetLayerBelow()?.OnActionHeld(actionID);
        }

        internal void OnActionReleased(string actionID)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                ActionReleased?.Invoke(this.layerKey, actionID);
            this.GetLayerBelow()?.OnActionReleased(actionID);
        }

        internal void OnMoveAxisPressed(Vector2 val)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveAxisPressed?.Invoke(this.layerKey, val);
            this.GetLayerBelow()?.OnMoveAxisPressed(val);
        }

        internal void OnMoveAxisHeld(Vector2 val)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveAxisHeld?.Invoke(this.layerKey, val);
            this.GetLayerBelow()?.OnMoveAxisHeld(val);
        }

        internal void OnMoveAxisReleased(Vector2 val)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MoveAxisReleased?.Invoke(this.layerKey, val);
            this.GetLayerBelow()?.OnMoveAxisReleased(val);
        }

        internal void OnCursorMoved(InputDevice val)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                CursorMoved?.Invoke(this.layerKey, val);
            this.GetLayerBelow()?.OnCursorMoved(val);
        }

        internal void OnMouseWheelMoved(Vector2 val)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                MouseWheelMoved?.Invoke(this.layerKey, val);
            this.GetLayerBelow()?.OnMouseWheelMoved(val);
        }

        internal void OnPlacementTileChanged(Vector2 placement)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                PlacementTileChanged?.Invoke(this.layerKey, placement);
            this.GetLayerBelow()?.OnPlacementTileChanged(placement);
        }

        internal void OnPlacementItemChanged(Item item)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                PlacementItemChanged?.Invoke(this.layerKey, item);
            this.GetLayerBelow()?.OnPlacementItemChanged(item);
        }

        internal void OnLayerUpdateTicked(UpdateTickedEventArgs e)
        {
            if (this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                LayerUpdateTicked?.Invoke(this.layerKey, e);
            this.GetLayerBelow()?.OnLayerUpdateTicked(e);
        }

        public event EventHandler<SButton> ButtonPressed;
        public event EventHandler<SButton> ButtonHeld;
        public event EventHandler<SButton> ButtonReleased;

        public event EventHandler<Tuple<SButton, SButton>> ButtonPairPressed;
        public event EventHandler<Tuple<SButton, SButton>> ButtonPairHeld;
        public event EventHandler<Tuple<SButton, SButton>> ButtonPairReleased;

        public event EventHandler<SButton> ConfirmPressed;
        public event EventHandler<SButton> ConfirmHeld;
        public event EventHandler<SButton> ConfirmReleased;

        public event EventHandler<SButton> CancelPressed;
        public event EventHandler<SButton> CancelHeld;
        public event EventHandler<SButton> CancelReleased;

        public event EventHandler<SButton> AltPressed;
        public event EventHandler<SButton> AltHeld;
        public event EventHandler<SButton> AltReleased;

        public event EventHandler<SButton> MenuPressed;
        public event EventHandler<SButton> MenuHeld;
        public event EventHandler<SButton> MenuReleased;

        public event EventHandler<SButton> MoveRightPressed;
        public event EventHandler<SButton> MoveRightHeld;
        public event EventHandler<SButton> MoveRightReleased;

        public event EventHandler<SButton> MoveDownPressed;
        public event EventHandler<SButton> MoveDownHeld;
        public event EventHandler<SButton> MoveDownReleased;

        public event EventHandler<SButton> MoveLeftPressed;
        public event EventHandler<SButton> MoveLeftHeld;
        public event EventHandler<SButton> MoveLeftReleased;

        public event EventHandler<SButton> MoveUpPressed;
        public event EventHandler<SButton> MoveUpHeld;
        public event EventHandler<SButton> MoveUpReleased;

        public event EventHandler<string> ActionPressed;
        public event EventHandler<string> ActionHeld;
        public event EventHandler<string> ActionReleased;

        public event EventHandler<Vector2> MoveAxisPressed;
        public event EventHandler<Vector2> MoveAxisHeld;
        public event EventHandler<Vector2> MoveAxisReleased;

        public event EventHandler<Vector2> MouseWheelMoved;
        public event EventHandler<IInputToolsAPI.InputDevice> CursorMoved;
        public event EventHandler<Vector2> PlacementTileChanged;
        public event EventHandler<Item> PlacementItemChanged;

        public event EventHandler<UpdateTickedEventArgs> LayerUpdateTicked;

        public object layerKey { get; }
        public bool isActive { get { return this._isActive; } }
        public IInputToolsAPI.BlockBehavior block { get { return this._block; } }

        public IInputToolsAPI.IInputLayer GetBelow(bool stopAtBlock = true)
        {
            return this.GetLayerBelow(stopAtBlock);
        }

        public bool IsButtonPressed(SButton button)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.IsButtonPressed(button);
        }

        public bool IsButtonHeld(SButton button)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.IsButtonHeld(button);
        }

        public bool IsButtonReleased(SButton button)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.IsButtonReleased(button);
        }

        public bool IsButtonPairPressed(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.IsButtonHeld(buttonPair.Item1) && this.IsButtonPressed(buttonPair.Item2);
        }

        public bool IsButtonPairHeld(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.buttonPairsPressing.Contains(buttonPair)
                && (this.IsButtonHeld(buttonPair.Item1) && this.IsButtonHeld(buttonPair.Item2));
        }

        public bool IsButtonPairReleased(Tuple<SButton, SButton> buttonPair)
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return Game1.ticks - this.inputTools.modEntry.tickButtonPairsReleased <= 1 && this.inputTools.modEntry.buttonPairsPressing.Contains(buttonPair)
                && (this.IsButtonReleased(buttonPair.Item1) || this.IsButtonReleased(buttonPair.Item2));
        }

        public bool IsMouseWheelMoved()
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.mouseWheelMovedLastTick;
        }

        public IInputToolsAPI.InputDevice IsCursorMoved()
        {
            this.inputTools.GetCurrentInputDevice();
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return IInputToolsAPI.InputDevice.None;
            if (this.inputTools.modEntry.mouseMovedLastTick &&
                (this.IsButtonHeld(SButton.RightThumbstickUp)
                || this.IsButtonHeld(SButton.RightThumbstickRight)
                || this.IsButtonHeld(SButton.RightThumbstickDown)
                || this.IsButtonHeld(SButton.RightThumbstickLeft)))
                return IInputToolsAPI.InputDevice.Controller;
            if (this.inputTools.modEntry.mouseMovedLastTick)
                return IInputToolsAPI.InputDevice.Mouse;
            return IInputToolsAPI.InputDevice.None;
        }

        public bool IsHeldItemBomb()
        {
            return this.inputTools.modEntry.IsHeldItemBomb();
        }

        public bool IsPlacementTileFromCursor()
        {
            return this.inputTools.modEntry.isLastPlacementTileFromCursor;
        }

        public bool IsPlacementTileChanged()
        {
            if (!this.inputTools.stack.IsLayerReachableByInput(this.layerKey))
                return false;
            return this.inputTools.modEntry.isPlacementTileMovedLastTick;
        }

        public SButton IsConfirmPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.confirmButtons);
        }

        public SButton IsConfirmHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.confirmButtons);
        }

        public SButton IsConfirmReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.confirmButtons);
        }

        public SButton IsCancelPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.cancelButtons);
        }

        public SButton IsCancelHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.cancelButtons);
        }

        public SButton IsCancelReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.cancelButtons);
        }

        public SButton IsAltPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.altButtons);
        }

        public SButton IsAltHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.altButtons);
        }

        public SButton IsAltReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.altButtons);
        }

        public SButton IsMenuPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.menuButtons);
        }

        public SButton IsMenuHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.menuButtons);
        }

        public SButton IsMenuReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.menuButtons);
        }

        public SButton IsMoveButtonPressed()
        {
            SButton button = this.IsMoveRightPressed();
            if (button != SButton.None)
                return button;
            button = this.IsMoveDownPressed();
            if (button != SButton.None)
                return button;
            button = this.IsMoveLeftPressed();
            if (button != SButton.None)
                return button;
            button = this.IsMoveUpPressed();
            if (button != SButton.None)
                return button;
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

        public SButton IsMoveButtonReleased()
        {
            SButton button = this.IsMoveRightReleased();
            if (button != SButton.None)
                return button;
            button = this.IsMoveDownReleased();
            if (button != SButton.None)
                return button;
            button = this.IsMoveLeftReleased();
            if (button != SButton.None)
                return button;
            button = this.IsMoveUpReleased();
            if (button != SButton.None)
                return button;
            return SButton.None;
        }

        public SButton IsMoveRightPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.moveRightButtons);
        }

        public SButton IsMoveRightHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.moveRightButtons);
        }

        public SButton IsMoveRightReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.moveRightButtons);
        }

        public SButton IsMoveDownPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.moveDownButtons);
        }

        public SButton IsMoveDownHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.moveDownButtons);
        }

        public SButton IsMoveDownReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.moveDownButtons);
        }

        public SButton IsMoveLeftPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.moveLeftButtons);
        }

        public SButton IsMoveLeftHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.moveLeftButtons);
        }

        public SButton IsMoveLeftReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.moveLeftButtons);
        }

        public SButton IsMoveUpPressed()
        {
            return this.IsAnyOfTheButtonsPressed(this.inputTools.modEntry.moveUpButtons);
        }

        public SButton IsMoveUpHeld()
        {
            return this.IsAnyOfTheButtonsHeld(this.inputTools.modEntry.moveUpButtons);
        }

        public SButton IsMoveUpReleased()
        {
            return this.IsAnyOfTheButtonsReleased(this.inputTools.modEntry.moveUpButtons);
        }

        public Tuple<SButton, SButton> IsActionPressed(string actionID)
        {
            if (string.IsNullOrWhiteSpace(actionID))
                return null;
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
            {
                if (this.IsButtonPairPressed(keyPair))
                    return keyPair;
            }
            return null;
        }

        public Tuple<SButton, SButton> IsActionHeld(string actionID)
        {
            if (string.IsNullOrWhiteSpace(actionID))
                return null;
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
            {
                if (this.IsButtonPairHeld(keyPair))
                    return keyPair;
            }
            return null;
        }

        public Tuple<SButton, SButton> IsActionReleased(string actionID)
        {
            if (string.IsNullOrWhiteSpace(actionID))
                return null;
            foreach (Tuple<SButton, SButton> keyPair in this.inputTools.actions.GetKeyPairsFromActions(actionID))
            {
                if (this.IsButtonPairReleased(keyPair))
                    return keyPair;
            }
            return null;
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

        public Vector2 GetCursorScreenPos()
        {
            return this.inputTools.Helper.Input.GetCursorPosition().ScreenPixels;
        }

        public Vector2 GetCursorTilePos()
        {
            return this.inputTools.Helper.Input.GetCursorPosition().Tile;
        }

        public Vector2 GetMouseWheelPos()
        {
            return this.inputTools.modEntry.GetMouseWheelPos();
        }

        public Vector2 GetPlacementTile()
        {
            return this.inputTools.modEntry.lastTileHighlightPos;
        }

        public Vector2 GetPlacementTileWithController()
        {
            return this.inputTools.modEntry.GetPlacementTileWithController();
        }

        public void SetActive(bool active)
        {
            this._isActive = active;
        }

        public void SetBlock(IInputToolsAPI.BlockBehavior block)
        {
            this._block = block;
        }

        public void MoveToTop()
        {
            if (this.layerKey == null || this == this.inputTools.Global)
                return;
            this.inputTools.stack.MoveToTop(this.layerKey);
        }

        public bool IsReachableByInput()
        {
            if (this == this.inputTools.Global)
                return true;
            if (this.layerKey == null)
                return false;
            return this.inputTools.stack.IsLayerReachableByInput(this.layerKey);
        }

        public void RemoveSelf()
        {
            if (this.layerKey == null || this == this.inputTools.Global)
                return;
            this.inputTools.stack.Remove(this.layerKey);
        }
    }
}
