using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.Events;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Helpers {
    public class InputHelper {
        private readonly Func<string> _getClipboardRaw;
        
        private Keys? _heldKey;
        private DateTime _keyStart;
        private DateTime _lastRepeat;

        public InputHelper() {
            // Clipboard
            try {
                // Import winforms
                Assembly winformsAssembly = Assembly.Load("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                Type clipboardType = winformsAssembly.GetType("System.Windows.Forms.Clipboard");
                MethodInfo[] clipboardMethods = clipboardType.GetMethods();

                // Clipboard.GetText()
                MethodInfo getClipboardInfo = clipboardMethods.FirstOrDefault(m => m.IsStatic && m.Name == "GetText" && m.GetParameters().Length == 0);
                if (getClipboardInfo != null) {
                    this._getClipboardRaw = (Func<string>) Delegate.CreateDelegate(typeof(Func<string>), getClipboardInfo);
                }
            } catch {
                // TODO: Better logging
                Console.WriteLine("WinForms assembly could not be loaded. Clipboard actions will be disabled. (This is normal for Linux/Mac)", LogLevel.Warn);
            }

            // Keyboard input
            //ControlEvents.KeyPressed += this.KeyPressed;
            //ControlEvents.KeyReleased += this.KeyReleased;
            //GameEvents.UpdateTick += this.UpdateTick;
        }

        public string GetClipboardText() {
            if (this._getClipboardRaw == null)
                return null;

            // Clipboard.GetText() must be called from a STA thread
            string clipboardText = "";
            Thread clipboardThread = new Thread(() => clipboardText = this._getClipboardRaw());
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.Start();
            clipboardThread.Join();
            return clipboardText;
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            // Reset the held 
            this._keyStart = DateTime.UtcNow;
            this._lastRepeat = this._keyStart;
            this._heldKey = e.KeyPressed;
        }

        private void KeyReleased(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed != this._heldKey)
                return;

            this._heldKey = null;
        }

        private void UpdateTick(object sender, EventArgs e) {
            // Check if a key is being held
            if (this._heldKey == null)
                return;

            // Check if it's a modifier key
            Keys heldKey = this._heldKey.Value;
            if (heldKey.IsShift() || heldKey.IsAlt() || heldKey.IsCtrl() || heldKey.IsWin())
                return;

            // Check with hardware to make sure it's still pressed (for sure!)
            // Most significant bit is whether the key is pressed
            if ((InputHelpers.GetKeyState((int) heldKey) & 0x8000) == 0) {
                this._heldKey = null;
                return;
            }

            DateTime now = DateTime.UtcNow;

            // Check if the key has been held long enough
            TimeSpan timeHeld = now - this._keyStart;
            if (timeHeld.TotalSeconds < Constants.KeyRepeatDelay)
                return;

            // Perform the repeat
            TimeSpan delta = now - this._lastRepeat;
            float frequency = timeHeld.TotalSeconds < Constants.KeyRepeatRampTime + Constants.KeyRepeatDelay ? Constants.KeyRepeatMinFrequency : Constants.KeyRepeatMaxFrequency;
            TimeSpan period = TimeSpan.FromSeconds(1 / frequency);
            if (delta >= period) {
                this.OnRepeatedKeystroke(new EventArgsKeyRepeated(heldKey));
                this._lastRepeat = now;
            }
        }

        #region Events
        public event EventHandler<EventArgsKeyRepeated> RepeatedKeystroke;

        #region Invocators
        protected virtual void OnRepeatedKeystroke(EventArgsKeyRepeated e) {
            this.RepeatedKeystroke?.Invoke(this, e);
        }
        #endregion
        #endregion
    }
}
