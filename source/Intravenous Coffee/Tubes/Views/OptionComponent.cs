/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tubes
{
    abstract class OptionComponent
    {
        static protected OptionComponent selectedComponent;

        virtual internal bool visible {
            set { _visible = value; }
            get {
                return _visible;
            }
        }

        public virtual bool enabled {
            get {
                return _enabled;
            }
            protected set {
                _enabled = value;
            }
        }

        public virtual string label {
            get {
                return _label;
            }
            protected set {
                _label = value;
            }
        }

        private bool _visible = false;
        internal protected bool _enabled;
        internal protected string _label;

        public virtual int Height { get; }
        public virtual int Width { get; }
        public virtual int X { get; }
        public virtual int Y { get; }

        public OptionComponent(string label, bool enabled = true)
        {
            this._label = label;
            this._enabled = enabled;
            this.AddListeners();
        }

        public OptionComponent(string label, int x, int y, bool enabled = true)
        {
            this._label = label;
            this._enabled = enabled;
            this.AddListeners();
        }

        protected bool IsAvailableForSelection()
        {
            if ((IsActiveComponent() || OptionComponent.selectedComponent == null) && visible) {
                return true;
            } else {
                return false;
            }
        }

        protected void RegisterAsActiveComponent()
        {
            OptionComponent.selectedComponent = this;
        }

        protected void UnregisterAsActiveComponent()
        {
            if (this.IsActiveComponent()) {
                OptionComponent.selectedComponent = null;
            }
        }

        internal bool IsActiveComponent()
        {
            return OptionComponent.selectedComponent == this;
        }

        // For moving the component
        public virtual void draw(SpriteBatch b, int x, int y) { }

        public virtual void draw(SpriteBatch b) { }

        internal void AddListeners()
        {
            RemoveListeners();
        }

        internal void RemoveListeners()
        {
            this.UnregisterAsActiveComponent();
            this.visible = false;
        }

        public virtual void receiveLeftClick(int x, int y, bool playSound = true) { }
        public virtual void receiveRightClick(int x, int y, bool playSound = true) { }
        public virtual void receiveKeyPress(Keys key) { }
        public virtual void leftClickHeld(int x, int y) { }
        public virtual void releaseLeftClick(int x, int y) { }
        public virtual void receiveScrollWheelAction(int direction) { }

    }
}