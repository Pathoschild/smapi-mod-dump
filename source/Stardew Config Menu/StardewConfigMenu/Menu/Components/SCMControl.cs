/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StardewConfigMenu.Components {

	abstract class SCMControl {
		static protected SCMControl SelectedComponent;

		internal bool IsActiveComponent => SelectedComponent == this;
		protected bool IsAvailableForSelection => (IsActiveComponent || SelectedComponent == null) && Visible;

		public virtual int Height { get; set; }
		public virtual int Width { get; set; }
		public virtual int X { get; set; }
		public virtual int Y { get; set; }

		private bool _visible = false;
		internal protected bool _enabled;
		internal protected string _label;

		virtual internal bool Visible {
			set {
				_visible = value;
				UnregisterAsActiveComponent();
			}
			get => _visible;
		}

		public virtual bool Enabled => _enabled;

		public virtual string Label => _label;

		public SCMControl(string label, bool enabled = true) {
			_label = label;
			_enabled = enabled;
		}

		protected void RegisterAsActiveComponent() {
			SelectedComponent = this;
		}

		protected void UnregisterAsActiveComponent() {
			if (IsActiveComponent) {
				SelectedComponent = null;
			}
		}

		// For moving the component
		public virtual void Draw(SpriteBatch b, int x, int y) {
			X = x;
			Y = y;
			Draw(b);
		}
		public virtual void Draw(SpriteBatch b) { }

		public virtual void ReceiveLeftClick(int x, int y, bool playSound = true) { }
		public virtual void ReceiveRightClick(int x, int y, bool playSound = true) { }
		public virtual void ReceiveKeyPress(Keys key) { }
		public virtual void LeftClickHeld(int x, int y) { }
		public virtual void ReleaseLeftClick(int x, int y) { }
		public virtual void ReceiveScrollWheelAction(int direction) { }

	}
}
