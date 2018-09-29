using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework;
using StardewConfigMenu.Panel.Components.ModOptions;
using StardewConfigMenu.Panel.Components;
using Microsoft.Xna.Framework.Input;


namespace StardewConfigMenu.Panel {
	internal class ModSheet: IClickableMenu {
		private List<OptionComponent> Options = new List<OptionComponent>();

		private ClickableTextureComponent upArrow;
		private ClickableTextureComponent downArrow;
		private ClickableTextureComponent scrollBar;
		private Rectangle scrollBarRunner;

		internal bool visible {
			set {
				if (!value)
					Options.ForEach(x => {
						x.visible = value;
					});
				else
					setVisibleOptions();

				_visible = value;
			}
			get {
				return _visible;
			}
		}

		private bool _visible = false;

		private int startingOption = 0;

		internal ModSheet(ModOptions modOptions, int x, int y, int width, int height) : base(x, y, width, height) {
			for (int i = 0; i < modOptions.List.Count; i++) {
				// check type of option
				Type t = modOptions.List[i].GetType();
				if (t.Equals(typeof(ModOptionCategoryLabel)))
					Options.Add(new ModCategoryLabelComponent(modOptions.List[i] as ModOptionCategoryLabel));
				else if (t.Equals(typeof(ModOptionSelection))) {
					int minWidth = 350;
					var option = (modOptions.List[i] as ModOptionSelection);
					option.Choices.Labels.ForEach(choice => { minWidth = Math.Max((int) Game1.smallFont.MeasureString(choice + "     ").X, minWidth); });

					Options.Add(new ModDropDownComponent(option, minWidth));
				} else if (t.Equals(typeof(ModOptionToggle)))
					Options.Add(new ModCheckBoxComponent(modOptions.List[i] as ModOptionToggle));
				else if (t.Equals(typeof(ModOptionTrigger)))
					Options.Add(new ModButtonComponent(modOptions.List[i] as ModOptionTrigger));
				else if (t.Equals(typeof(ModOptionStepper)))
					Options.Add(new ModPlusMinusComponent(modOptions.List[i] as ModOptionStepper));
				else if (t.Equals(typeof(ModOptionRange)))
					//Options.Add(new SliderComponent("Hey", 0, 10, 1, 5, true));
					Options.Add(new ModSliderComponent(modOptions.List[i] as ModOptionRange));
				//break;


				// create proper component
				// add to Options
			}
			this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + Game1.tileSize / 4, this.yPositionOnScreen, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), (float) Game1.pixelZoom, false);
			this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + Game1.tileSize / 4, this.yPositionOnScreen + height - 12 * Game1.pixelZoom, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), (float) Game1.pixelZoom, false);
			this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + Game1.pixelZoom * 3, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), (float) Game1.pixelZoom, false);
			this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, this.scrollBar.bounds.Width, this.downArrow.bounds.Y - this.upArrow.bounds.Y - this.upArrow.bounds.Height - Game1.pixelZoom * 3);
			AddListeners();
		}

		public void AddListeners() {
			RemoveListeners();
			ControlEvents.KeyPressed += KeyPressed;
		}

		public void RemoveListeners(bool children = false) {
			if (children) {
				Options.ForEach(x => { x.RemoveListeners(); });
			}

			ControlEvents.KeyPressed -= KeyPressed;
			this.scrolling = false;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {

			if (GameMenu.forcePreventClose) { return; }

			Options.ForEach(z => {
				if (z.visible)
					z.receiveRightClick(x, y, playSound);
			});
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (GameMenu.forcePreventClose || !this.visible) { return; }

			Options.ForEach(z => {
				if (z.visible)
					z.receiveLeftClick(x, y, playSound);
			});

			if (this.downArrow.containsPoint(x, y) && this.startingOption < Math.Max(0, this.Options.Count - 6)) {
				if (playSound && this.Options.Count > 6 && this.startingOption != this.Options.Count - 6)
					Game1.playSound("shwip");
				this.downArrowPressed();
			} else if (this.upArrow.containsPoint(x, y) && this.startingOption > 0) {
				if (playSound && this.Options.Count > 6 && this.startingOption != 0)
					Game1.playSound("shwip");

				this.upArrowPressed();

			} else if (this.scrollBar.containsPoint(x, y) && Options.Count > 6) {
				this.scrolling = true;
			}
			// failsafe
			this.startingOption = Math.Max(0, Math.Min(this.Options.Count - 6, this.startingOption));
		}

		private bool scrolling = false;

		public override void leftClickHeld(int x, int y) {
			if (GameMenu.forcePreventClose) { return; }

			Options.ForEach(z => {
				if (z.visible)
					z.leftClickHeld(x, y);
			});

			if (this.scrolling) {
				int oldPosition = this.startingOption;

				if (y < this.scrollBarRunner.Y) {
					this.startingOption = 0;
					this.setScrollBarToCurrentIndex();
				} else if (y > this.scrollBarRunner.Bottom) {
					this.startingOption = this.Options.Count - 6;
					this.setScrollBarToCurrentIndex();
				} else {
					float num = (float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height;
					this.startingOption = (int) Math.Round(Math.Max(0, num * (Options.Count - 6)));
				}

				this.setScrollBarToCurrentIndex();
				if (oldPosition != this.startingOption) {
					Game1.playSound("shiny4");
				}
			}
		}

		public override void releaseLeftClick(int x, int y) {
			Options.ForEach(z => {
				if (z.visible)
					z.releaseLeftClick(x, y);
			});

			this.scrolling = false;
		}

		public override void receiveScrollWheelAction(int direction) {
			Options.ForEach(z => {
				if (z.visible)
					z.receiveScrollWheelAction(direction);
			});

			if (direction > 0) {
				if (this.Options.Count > 6 && this.startingOption != 0)
					Game1.playSound("shiny4");

				this.upArrowPressed();

			} else if (direction < 0) {
				if (this.Options.Count > 6 && this.startingOption != this.Options.Count - 6)
					Game1.playSound("shiny4");
				this.downArrowPressed();
			}
		}

		private void KeyPressed(object sender, EventArgsKeyPressed e) {
			Options.ForEach(z => {
				if (z.visible)
					z.receiveKeyPress(e.KeyPressed);
			});

			if (e.KeyPressed == Keys.Up)
				upArrowPressed();
			if (e.KeyPressed == Keys.Down)
				downArrowPressed();
		}

		public void upArrowPressed() {
			this.upArrow.scale = this.upArrow.baseScale;
			if (startingOption > 0)
				this.startingOption--;
			this.setScrollBarToCurrentIndex();
			//this.setVisibleOptions();
		}

		public void downArrowPressed() {
			this.downArrow.scale = this.downArrow.baseScale;
			if (startingOption < Options.Count - 6)
				this.startingOption++;
			this.setScrollBarToCurrentIndex();
			//this.setVisibleOptions();
		}

		public void setVisibleOptions() {
			for (int i = 0; i < Options.Count; i++) {
				if (i >= startingOption && i < startingOption + 6) {
					Options[i].visible = true;
				} else {
					Options[i].visible = false;
				}
			}
		}

		public void setScrollBarToCurrentIndex() {
			setVisibleOptions();
			if (this.Options.Count > 0) {
				this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.Options.Count - 6 + 1) * this.startingOption + this.upArrow.bounds.Bottom + (Game1.pixelZoom);
				if (this.startingOption == this.Options.Count - 6) {
					this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - Game1.pixelZoom * 2;
				}
			}
		}

		public override void draw(SpriteBatch b) {
			base.draw(b);

			//drawTextureBox();
			if (this.Options.Count > 6) {
				this.upArrow.draw(b);
				this.downArrow.draw(b);
				IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, (float) Game1.pixelZoom, false);
				this.scrollBar.draw(b);
			}

			//b.DrawString(Game1.dialogueFont, this.Options.modManifest.Name, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen), Color.White);

			for (int i = startingOption; i < Options.Count; i++) {
				if (!(Options[i] is ModDropDownComponent) && Options[i].visible)
					Options[i].draw(b, this.xPositionOnScreen, ((this.height / 6) * (i - startingOption)) + this.yPositionOnScreen + ((this.height / 6) - Options[i].Height) / 2);
			}

			// Draw Dropdowns last, they must be on top; must draw from bottom to top
			for (int i = Math.Min(startingOption + 5, Options.Count - 1); i >= startingOption; i--) {
				if (Options[i] is ModDropDownComponent && Options[i].visible)
					Options[i].draw(b, this.xPositionOnScreen, ((this.height / 6) * (i - startingOption)) + this.yPositionOnScreen + ((this.height / 6) - Options[i].Height) / 2);
			}

		}
	}
}
