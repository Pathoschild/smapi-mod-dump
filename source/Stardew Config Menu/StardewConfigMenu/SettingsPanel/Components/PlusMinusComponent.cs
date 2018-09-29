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
	internal delegate void PlusMinusValueChanged(decimal Value);

	internal class PlusMinusComponent: OptionComponent {

		internal event PlusMinusValueChanged PlusMinusValueChanged;

		ClickableTextureComponent minus = new ClickableTextureComponent(new Rectangle(0, 0, OptionsPlusMinus.minusButtonSource.Width * Game1.pixelZoom, OptionsPlusMinus.minusButtonSource.Height * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.minusButtonSource, Game1.pixelZoom);
		ClickableTextureComponent plus = new ClickableTextureComponent(new Rectangle(0, 0, OptionsPlusMinus.plusButtonSource.Width * Game1.pixelZoom, OptionsPlusMinus.plusButtonSource.Height * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.plusButtonSource, Game1.pixelZoom);
		public override int Width => plus.bounds.Right - minus.bounds.X;
		public override int Height => minus.bounds.Height;
		public override int X => minus.bounds.X;
		public override int Y => minus.bounds.Y;

		internal PlusMinusComponent(string labelText, decimal min, decimal max, decimal stepsize, decimal defaultSelection, int x, int y, DisplayType type = DisplayType.NONE, bool enabled = true) : base(labelText, enabled) {
			this._min = Math.Round(min, 3);
			this._max = Math.Round(max, 3);
			this._stepSize = Math.Round(stepsize, 3);
			this._type = type;

			var valid = CheckValidInput(Math.Round(defaultSelection, 3));
			var newVal = (int) ((valid - _min) / _stepSize) * _stepSize + _min;
			this._Value = newVal;

			this.minus.bounds.X = x;
			this.minus.bounds.Y = y;

			var maxRect = Game1.dialogueFont.MeasureString((this._max + this._stepSize % 1).ToString());
			var minRect = Game1.dialogueFont.MeasureString((this._min - this._stepSize % 1).ToString());

			ValueMaxLabelSize = (maxRect.X > minRect.X) ? maxRect : minRect;
		}

		internal PlusMinusComponent(string labelText, decimal min, decimal max, decimal stepsize, decimal defaultSelection, DisplayType type = DisplayType.NONE, bool enabled = true) : this(labelText, min, max, stepsize, defaultSelection, 0, 0, type, enabled) { }

		protected int typeExtraWidth {
			get {
				return (int) Game1.dialogueFont.MeasureString(typeExtraString).X;
			}
		}

		protected string typeExtraString {
			get {
				switch (type) {
				case DisplayType.PERCENT:
					return "%";
				default:
					return "";
				}
			}
		}

		readonly private DisplayType _type;
		public virtual DisplayType type => _type;


		readonly protected Vector2 ValueMaxLabelSize;

		protected Vector2 ValueLabelSize {
			get {
				return Game1.dialogueFont.MeasureString(Value.ToString());
			}
		}

		readonly private decimal _min;
		public virtual decimal min => _min;

		readonly private decimal _max;
		public virtual decimal max => _max;

		readonly private decimal _stepSize;
		public virtual decimal stepSize => _stepSize;

		private decimal _Value;
		public virtual decimal Value {
			get {
				return _Value;
			}
			protected set {
				var valid = CheckValidInput(Math.Round(value, 3));
				var newVal = (int) ((valid - min) / stepSize) * stepSize + min;
				if (newVal != this._Value) {
					this._Value = newVal;
					this.PlusMinusValueChanged?.Invoke(this._Value);
				}
			}
		}

		protected virtual void StepUp() {
			this.Value += this.stepSize;
		}

		protected virtual void StepDown() {
			this.Value -= this.stepSize;
		}

		protected decimal CheckValidInput(decimal input) {
			if (input > _max)
				return _max;

			if (input < _min)
				return _min;

			return input;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {

			if (this.minus.containsPoint(x, y) && enabled && this.IsAvailableForSelection()) {
				var temp = this.Value;
				this.StepDown();
				if (playSound && temp != this.Value)
					Game1.playSound("drumkit6");
			} else if (this.plus.containsPoint(x, y) && enabled && this.IsAvailableForSelection()) {
				var temp = this.Value;
				this.StepUp();
				if (playSound && temp != this.Value)
					Game1.playSound("drumkit6");
			}
		}

		public override void draw(SpriteBatch b, int x, int y) {
			this.minus.bounds.X = x;
			this.minus.bounds.Y = y;
			this.plus.bounds.X = minus.bounds.Right + (int) this.ValueMaxLabelSize.X + typeExtraWidth + 4 * Game1.pixelZoom;
			this.plus.bounds.Y = minus.bounds.Y;
			this.draw(b);
		}

		public override void draw(SpriteBatch b) {
			base.draw(b);

			minus.draw(b, Color.White * ((this.enabled && (Value - stepSize >= min)) ? 1f : 0.33f), 0.88f);
			plus.draw(b, Color.White * ((this.enabled && (Value + stepSize <= max)) ? 1f : 0.33f), 0.88f);
			//b.Draw(Game1.mouseCursors, new Vector2((float) (minus.bounds.X), (float) (minus.bounds.Y)), OptionsPlusMinus.minusButtonSource, Color.White * ((this.enabled) ? 1f : 0.33f), 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.4f);

			b.DrawString(Game1.dialogueFont, Value.ToString() + typeExtraString, new Vector2((float) (minus.bounds.Right + (this.plus.bounds.X - minus.bounds.Right - ValueLabelSize.X - typeExtraWidth) / 2), (float) (minus.bounds.Y + ((minus.bounds.Height - ValueMaxLabelSize.Y) / 2))), this.enabled ? Game1.textColor : (Game1.textColor * 0.33f));
			//Utility.drawBoldText(b, $"{Value.ToString()}", Game1.smallFont, new Vector2((float)(this.bounds.Right + Game1.pixelZoom * 2), (float)(this.bounds.Y + ((this.bounds.Height - valueLabelSize.Y) / 2))), this.enabled ? Game1.textColor : (Game1.textColor * 0.33f));

			//b.Draw(Game1.mouseCursors, new Vector2((float) (this.plusButtonbounds.X), (float) (this.plusButtonbounds.Y)), OptionsPlusMinus.plusButtonSource, Color.White * ((this.enabled) ? 1f : 0.33f), 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.4f);


			var labelSize = Game1.dialogueFont.MeasureString(this.label);

			Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (this.plus.bounds.Right + Game1.pixelZoom * 4), (float) (minus.bounds.Y + ((minus.bounds.Height - labelSize.Y) / 2))), this.enabled ? Game1.textColor : (Game1.textColor * 0.33f), 1f, 0.1f, -1, -1, 1f, 3);

		}
	}
}
