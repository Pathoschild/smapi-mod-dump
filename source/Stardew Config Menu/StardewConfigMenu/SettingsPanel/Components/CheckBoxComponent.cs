using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewConfigFramework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;

namespace StardewConfigMenu.Panel.Components {
	internal delegate void CheckBoxToggled(bool isOn);

	internal class CheckBoxComponent: OptionComponent {

		internal event CheckBoxToggled CheckBoxToggled;

		ClickableTextureComponent checkbox = new ClickableTextureComponent(new Rectangle(0, 0, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Width, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Height), Game1.mouseCursors, OptionsCheckbox.sourceRectChecked, Game1.pixelZoom);

		public override int Width => checkbox.bounds.Width;
		public override int Height => checkbox.bounds.Height;
		public override int X => checkbox.bounds.X;
		public override int Y => checkbox.bounds.Y;

		//
		// Static Fields
		//

		public virtual bool IsChecked {
			get {
				return _IsChecked;
			}
			protected set {
				_IsChecked = value;
				this.CheckBoxToggled?.Invoke(value);
			}
		}

		internal override bool visible {
			set {
				base.visible = value;
				checkbox.visible = value;
			}
		}

		private bool _IsChecked;


		//
		// Fields
		//

		internal CheckBoxComponent(string label, bool isChecked, int x, int y, bool enabled = true) : base(label, enabled) {
			this.checkbox.bounds.X = x;
			this.checkbox.bounds.Y = y;
			//base.height = checkbox.bounds.Height;
			//base.width = checkbox.bounds.Width;
			//base.xPositionOnScreen = checkbox.bounds.X;
			//base.yPositionOnScreen = checkbox.bounds.Y;

			//this.bounds = new Rectangle(x, y, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Width, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Height);
			this._IsChecked = isChecked;
		}

		internal CheckBoxComponent(string label, bool isChecked, bool enabled = true) : base(label, enabled) {
			//base.height = checkbox.bounds.Height;
			//base.width = checkbox.bounds.Width;

			//this.bounds = new Rectangle(0, 0, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Width, Game1.pixelZoom * OptionsCheckbox.sourceRectChecked.Height);
			this._IsChecked = isChecked;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			base.receiveLeftClick(x, y, playSound);

			bool test = this.IsAvailableForSelection();

			if (this.checkbox.containsPoint(x, y) && enabled && this.IsAvailableForSelection()) {
				IsChecked = !IsChecked;
				if (playSound)
					Game1.playSound("drumkit6");
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {

		}

		public override void draw(SpriteBatch b, int x, int y) {
			this.checkbox.bounds.X = x;
			this.checkbox.bounds.Y = y;
			this.draw(b);
		}

		public override void draw(SpriteBatch b) {
			base.draw(b);

			if (IsChecked)
				checkbox.sourceRect = OptionsCheckbox.sourceRectChecked;
			else
				checkbox.sourceRect = OptionsCheckbox.sourceRectUnchecked;

			checkbox.draw(b, Color.White * ((this.enabled) ? 1f : 0.33f), 0.88f);
			//b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X), (float) (this.bounds.Y)), new Rectangle?((this.IsChecked) ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked), Color.White * ((this.enabled) ? 1f : 0.33f), 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.4f);

			var labelSize = Game1.dialogueFont.MeasureString(this.label);

			Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (this.checkbox.bounds.Right + Game1.pixelZoom * 4), (float) (this.checkbox.bounds.Y + ((this.checkbox.bounds.Height - labelSize.Y) / 2))), this.enabled ? Game1.textColor : (Game1.textColor * 0.33f), 1f, 0.1f, -1, -1, 1f, 3);

		}


	}
}
