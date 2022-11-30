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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using static InputTools.IInputToolsAPI;

namespace InputTools
{
    public class InputToolsAPI : IInputToolsAPI
    {
        internal ModEntry modEntry;
        internal IModHelper Helper;
        internal IMonitor Monitor;

        internal InputLayer _Global;
        public Actions actions;
        public ControlStack stack;

        internal InputToolsAPI(ModEntry modEntry)
        {
            this.modEntry = modEntry;
            this.Helper = modEntry.Helper;
            this.Monitor = modEntry.Monitor;

            this.actions = new Actions(this);
            this.stack = new ControlStack(this);
            this._Global = new InputLayer(this, null) { _block = BlockBehavior.PassBelow };
        }

        /*********
        ** Private methods
        *********/
        internal void OnInputDeviceChanged(IInputToolsAPI.InputDevice inputDevice)
        {
            InputDeviceChanged?.Invoke(this, inputDevice);
        }

        internal void OnKeybindingConfigChanged()
        {
            KeybindingConfigChanged?.Invoke(this, null);
        }

        private Action<Tuple<SButton, SButton>> keyBindingCallback;
        private Tuple<SButton, SButton> keyBindingCandidate;
        private bool? savedGlobalActive;
        private IInputToolsAPI.BlockBehavior? savedGlobalBlock;
        private void KeyBindingSinglePressed(object? sender, SButton val)
        {
            if (this.IsCancelButton(val))
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(null);
                this.keyBindingCandidate = null;
                return;
            }
            if (this.keyBindingCandidate == null)
                this.keyBindingCandidate = new Tuple<SButton, SButton>(val, SButton.None);
        }
        private void KeyBindingSingleReleased(object? sender, SButton val)
        {
            if (this.keyBindingCandidate != null && this.keyBindingCandidate.Item1 == val && this.keyBindingCandidate.Item2 == SButton.None)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(new Tuple<SButton, SButton>(val, SButton.None));
                this.keyBindingCandidate = null;
            }
        }
        private void KeyBindingPairPressed(object? sender, Tuple<SButton, SButton> val)
        {
            this.keyBindingCandidate = val;
        }
        private void KeyBindingPairReleased(object? sender, Tuple<SButton, SButton> val)
        {
            if (this.keyBindingCandidate == val)
            {
                this.StopListeningForKeybinding();
                this.keyBindingCallback?.Invoke(val);
            }
        }

        public List<string> GetListOfModIDs()
        {
            return this.modEntry.GetListOfModIDs();
        }

        public event EventHandler<IInputToolsAPI.InputDevice> InputDeviceChanged;
        public event EventHandler KeybindingConfigChanged;

        public IInputToolsAPI.InputDevice GetCurrentInputDevice()
        {
            return this.modEntry.GetCurrentInputDevice();
        }

        public bool IsKeybindingConfigChanged()
        {
            return this.modEntry.IsKeybindingConfigChanged();
        }

        public IInputToolsAPI.InputDevice GetInputDevice(SButton button)
        {
            return this.modEntry.GetInputDevice(button);
        }

        public bool IsConfirmButton(SButton button)
        {
            return this.modEntry.IsConfirmButton(button);
        }

        public bool IsCancelButton(SButton button)
        {
            return this.modEntry.IsCancelButton(button);
        }

        public bool IsAltButton(SButton button)
        {
            return this.modEntry.IsAltButton(button);
        }

        public bool IsMenuButton(SButton button)
        {
            return this.modEntry.IsMenuButton(button);
        }

        public bool IsMoveRightButton(SButton button)
        {
            return this.modEntry.IsMoveRightButton(button);
        }

        public bool IsMoveDownButton(SButton button)
        {
            return this.modEntry.IsMoveDownButton(button);
        }

        public bool IsMoveLeftButton(SButton button)
        {
            return this.modEntry.IsMoveLeftButton(button);
        }

        public bool IsMoveUpButton(SButton button)
        {
            return this.modEntry.IsMoveUpButton(button);
        }

        public void GetTextFromVirtualKeyboard(Action<string> finishedCallback, Action<string> updateCallback = null, int? textboxWidth = 300, string initialText = "")
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputLayer tempLayer = this.CreateLayer(this);
                this.savedGlobalActive = this._Global._isActive;
                this.savedGlobalBlock = this._Global._block;
                this.Global.SetActive(false);
                this.Global.SetBlock(BlockBehavior.PassBelow);
                TextBox textbox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textbox"), null, Game1.smallFont, Game1.textColor);
                textbox.TitleText = "Enter input";
                textbox.Text = initialText;
                if (textboxWidth.HasValue)
                    textbox.Width = textboxWidth.Value;
                textbox.SelectMe();
                Game1.showTextEntry(textbox);

                string textboxTextLastTick = initialText;
                tempLayer.LayerUpdateTicked += new EventHandler<UpdateTickedEventArgs>((s, e) =>
                {
                    if (Game1.textEntry != null)
                    {
                        if (textboxTextLastTick != textbox.Text)
                            updateCallback?.Invoke(textbox.Text);
                        textboxTextLastTick = textbox.Text;
                        textbox.SelectMe();
                    }
                    else
                    {
                        this.CloseVirtualKeyboard();
                        finishedCallback.Invoke(null);
                    }
                });
                textbox.OnEnterPressed += new TextBoxEvent(target =>
                {
                    this.CloseVirtualKeyboard();
                    finishedCallback.Invoke(textbox.Text);
                });
            }), 1);
        }

        public void CloseVirtualKeyboard()
        {
            IInputLayer tempLayer = this.GetLayer(this);
            if (tempLayer == null)
                return;
            this.RemoveLayer(this);
            if (this.savedGlobalActive != null)
                this.Global.SetActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetBlock(this.savedGlobalBlock.Value);
            this.savedGlobalActive = null;
            this.savedGlobalBlock = null;
        }

        public void ListenForKeybinding(Action<Tuple<SButton, SButton>> keyBindingCallback)
        {
            DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(() =>
            {
                this.StopListeningForKeybinding();
                IInputLayer tempLayer = this.CreateLayer(this);
                this.savedGlobalActive = this._Global._isActive;
                this.savedGlobalBlock = this._Global._block;
                this.Global.SetActive(false);
                this.Global.SetBlock(BlockBehavior.PassBelow);
                tempLayer.ButtonPressed += this.KeyBindingSinglePressed;
                tempLayer.ButtonReleased += this.KeyBindingSingleReleased;
                tempLayer.ButtonPairPressed += this.KeyBindingPairPressed;
                tempLayer.ButtonPairReleased += this.KeyBindingPairReleased;
                this.keyBindingCandidate = null;
                this.keyBindingCallback = keyBindingCallback;
            }), 1);
        }

        public void StopListeningForKeybinding()
        {
            IInputLayer tempLayer = this.GetLayer(this);
            if (tempLayer == null)
                return;
            tempLayer.ButtonPressed -= this.KeyBindingSinglePressed;
            tempLayer.ButtonReleased -= this.KeyBindingSingleReleased;
            tempLayer.ButtonPairPressed -= this.KeyBindingPairPressed;
            tempLayer.ButtonPairReleased -= this.KeyBindingPairReleased;
            this.RemoveLayer(this);
            if (this.savedGlobalActive != null)
                this.Global.SetActive(this.savedGlobalActive.Value);
            if (this.savedGlobalBlock != null)
                this.Global.SetBlock(this.savedGlobalBlock.Value);
            this.savedGlobalActive = null;
            this.savedGlobalBlock = null;
        }

        public void RegisterAction(string actionID, params SButton[] keyTriggers)
        {
            this.actions.RegisterAction(actionID, keyTriggers);
        }

        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers)
        {
            this.actions.RegisterAction(actionID, keyTriggers);
        }

        public void UnregisterAction(string actionID)
        {
            this.actions.UnregisterAction(actionID);
        }

        public List<string> GetActionsFromKey(SButton key)
        {
            return this.actions.GetActionsFromKey(key);
        }

        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair)
        {
            return this.actions.GetActionsFromKeyPair(keyPair);
        }

        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID)
        {
            return this.actions.GetKeyPairsFromActions(actionID);
        }

        public IInputToolsAPI.IInputLayer CreateLayer(object layerKey, bool startActive = true, IInputToolsAPI.BlockBehavior block = IInputToolsAPI.BlockBehavior.Block)
        {
            return this.stack.Create(layerKey, startActive, block);
        }

        public IInputToolsAPI.IInputLayer PopLayer()
        {
            return this.stack.Pop();
        }

        public void RemoveLayer(object layerKey)
        {
            this.stack.Remove(layerKey);
        }

        public IInputToolsAPI.IInputLayer PeekLayer()
        {
            return this.stack.Peek();
        }

        public IInputToolsAPI.IInputLayer GetLayer(object layerKey)
        {
            return this.stack.Get(layerKey);
        }

        public IInputLayer Global { get { return this._Global; } }
    }
}
