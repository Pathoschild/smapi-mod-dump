
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerEmotes.Framework.Constants;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MultiplayerEmotes.Menus {

	public class EmotesMenuBox : IClickableMenu {

		public bool IsOpen { get; set; }

		private List<ClickableComponent> emoteSelectionButtons;
		private Texture2D EmotesMenuBoxTexture;
		private Texture2D EmotesTexture;
		private Texture2D EmotesMenuArrowsTexture;
		private int firstComponentIndex;
		private ClickableComponent upArrow;
		private ClickableComponent downArrow;
		private readonly EmotesMenuButton EmotesMenuButton;
		public int totalEmotes;

		public int emoteSize = 16;
		public int animationFrames = 4;
		public int maxRowComponents = 3;
		public int maxColComponents = 3;

		private Rectangle EmotesMenuBoxArrow;
		private Rectangle EmotesMenuBoxArrowPosition;

		public EmotesMenuBox(IModHelper helper, EmotesMenuButton emotesMenuButton, Texture2D emotesTexture) {

			this.EmotesMenuButton = emotesMenuButton;
			this.EmotesTexture = emotesTexture;
			this.EmotesMenuBoxTexture = Sprites.MenuBox.Texture;
			this.EmotesMenuArrowsTexture = Sprites.MenuArrow.Texture;

			this.width = Sprites.MenuBox.Width;
			this.height = Sprites.MenuBox.Height;

			this.EmotesMenuBoxArrow = Sprites.MenuBox.LeftArrow.SourceRectangle;
			this.EmotesMenuBoxArrowPosition.Width = (this.width * EmotesMenuBoxArrow.Width) / Sprites.MenuBox.Width;
			this.EmotesMenuBoxArrowPosition.Height = (this.height * EmotesMenuBoxArrow.Height) / Sprites.MenuBox.Height;

			this.emoteSelectionButtons = new List<ClickableComponent>();
			int componentSize = emoteSize * Game1.pixelZoom;

			this.emoteSelectionButtons = GetClickableComponentList(26, 16, componentSize, componentSize, maxRowComponents, maxColComponents, 10, 10);

			//TODO: Refactor upArrow and downArrow 
			this.upArrow = new ClickableComponent(new Rectangle(256 + 4, 20 + 16, 32, 20), nameof(Sprites.MenuArrow.UpSourceRectangle));
			this.downArrow = new ClickableComponent(new Rectangle(256 + 4, 200 + 16, 32, 20), nameof(Sprites.MenuArrow.DownSourceRectangle));

			totalEmotes = (emotesTexture.Width / (animationFrames * emoteSize)) * ((emotesTexture.Height - emoteSize) / emoteSize);

			this.exitFunction += this.OnExit;
			IsOpen = false;

		}

		public List<ClickableComponent> GetClickableComponentList(int x, int y, int width, int height, int rows, int cols, int horizontalSpacing, int verticalSpacing) {

			List<ClickableComponent> componentList = new List<ClickableComponent>();

			for(int i = 0; i < rows; ++i) {
				for(int j = 0; j < cols; j++) {

					Rectangle bounds = new Rectangle(x + j * width, y + i * height, width, height);

					if(j > 0) {
						bounds.X += j * horizontalSpacing;
					}

					if(i > 0) {
						bounds.Y += i * verticalSpacing;
					}

					componentList.Add(new ClickableComponent(bounds, string.Concat(j + (i * cols) + 1)));
				}
			}
			return componentList;
		}

		public override bool isWithinBounds(int x, int y) {

			// Check if is within the box arrow
			if(EmotesMenuBoxArrowPosition.Contains(x, y)) {
				return true;
			}

			// Check if X coord is within bounds
			if((this.xPositionOnScreen > x) || (x >= (this.xPositionOnScreen + this.width))) {
				return false;
			}

			// Check if Y coord is within bounds
			if((this.yPositionOnScreen > y) || (y >= (this.yPositionOnScreen + this.height))) {
				return false;
			}

			// X and Y coords are within bounds
			return true;
		}

		private int GetMaxAmmountComponentShowing() {
			return maxColComponents * maxRowComponents;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {

			if(isWithinBounds(x, y)) {

				int x1 = x - this.xPositionOnScreen;
				int y1 = y - this.yPositionOnScreen;

				if(this.upArrow.containsPoint(x1, y1)) {
					this.upArrowPressed(GetMaxAmmountComponentShowing());
				} else if(this.downArrow.containsPoint(x1, y1)) {
					this.downArrowPressed(GetMaxAmmountComponentShowing());
				}

				foreach(ClickableComponent emoteSelectionButton in this.emoteSelectionButtons) {
					if(emoteSelectionButton.containsPoint(x1, y1)) {
						Game1.player.IsEmoting = false;
						int emote = this.firstComponentIndex + int.Parse(emoteSelectionButton.name);
						Game1.player.doEmote(emote * 4);
						Game1.playSound("coin");
						break;
					}
				}

			}

		}

		private void upArrowPressed(int amountToScroll) {
			if(this.firstComponentIndex != 0) {
				Game1.playSound("Cowboy_Footstep");
			}
			this.firstComponentIndex = Math.Max(0, this.firstComponentIndex - amountToScroll);
			this.upArrow.scale = 0.75f;
		}

		private void downArrowPressed(int amountToScroll) {
			if(this.firstComponentIndex != totalEmotes - GetMaxAmmountComponentShowing()) {
				Game1.playSound("Cowboy_Footstep");
			}
			this.firstComponentIndex = Math.Min(totalEmotes - GetMaxAmmountComponentShowing(), this.firstComponentIndex + amountToScroll);
			this.downArrow.scale = 0.75f;
		}

		public override void receiveScrollWheelAction(int direction) {
			if(direction < 0) {
				this.downArrowPressed(maxRowComponents);
			} else if(direction > 0) {
				this.upArrowPressed(maxRowComponents);
			}
		}

		public override bool readyToClose() {
			return true;
		}

		public void Toggle() {
			if(IsOpen) {
				Close();
			} else {
				Open();
			}
		}

		public void Open(bool playSound = true) {
			IsOpen = true;
			if(playSound) {
				Game1.playSound("shwip");
			}
		}

		public void Close(bool playSound = true) {
			IsOpen = false;
			if(playSound) {
				Game1.playSound("shwip");
			}
		}

		private void OnExit() {
		}

		public override void clickAway() {
			if(this.IsOpen && !this.isWithinBounds(Game1.getMouseX(), Game1.getMouseY())) {
				Close();
			}
		}

		public void UpdatePosition(int buttonXPosition, int buttonYPosition, int buttonWidth, int buttonHeight) {

			// Emotes menu box arrow dimensions
			this.EmotesMenuBoxArrowPosition.Width = (this.width * EmotesMenuBoxArrow.Width) / Sprites.MenuBox.Width;
			this.EmotesMenuBoxArrowPosition.Height = (this.height * EmotesMenuBoxArrow.Height) / Sprites.MenuBox.Height;

			// Emotes menu box arrow position
			this.EmotesMenuBoxArrowPosition.X = buttonXPosition + buttonWidth;
			this.EmotesMenuBoxArrowPosition.Y = buttonYPosition + (buttonHeight / 2) - (EmotesMenuBoxArrowPosition.Height / 2);

			// Emotes menu box position
			this.xPositionOnScreen = buttonXPosition + buttonWidth + EmotesMenuBoxArrowPosition.Width - 8;
			this.yPositionOnScreen = buttonYPosition + (buttonHeight / 2) - (this.height / 2);

			// Emotes menu box reposition inside game screen
			if(this.xPositionOnScreen < 0) {
				this.xPositionOnScreen = 0;
			} else if((this.xPositionOnScreen + this.width) >= Game1.viewport.Width) {
				this.EmotesMenuBoxArrow = Sprites.MenuBox.RightArrow.SourceRectangle;
				this.EmotesMenuBoxArrowPosition.X = buttonXPosition - EmotesMenuBoxArrowPosition.Width;
				this.xPositionOnScreen = buttonXPosition - this.width - EmotesMenuBoxArrowPosition.Width + 8;
			} else {
				this.EmotesMenuBoxArrow = Sprites.MenuBox.LeftArrow.SourceRectangle;
			}

			if(this.yPositionOnScreen < 0) {
				this.yPositionOnScreen = 0;
			} else if((this.yPositionOnScreen + this.height) >= Game1.viewport.Height) {
				this.yPositionOnScreen = Game1.viewport.Height - this.height;
			}

			// Up arrow position
			// X = boxWidth  -  arrowWidth	 -  boxRightBorderPixels  -  rigthSpacing
			// Y = 0         -  arrowHeight	 +  boxTopBorderPixels	  +  topSpacing
			this.upArrow.bounds.X = this.width - this.upArrow.bounds.Width - 8 - 16;
			this.upArrow.bounds.Y = this.upArrow.bounds.Height + 8 - 10;

			// Down arrow position
			// X = boxWidth	  -  arrowWidth   -  boxBottomBorderPixels  -  rigthSpacing
			// Y = boxHeight  -  arrowHeight  -  boxRightBorderPixels   -  bottomSpacing
			this.downArrow.bounds.X = this.width - this.downArrow.bounds.Width - 8 - 16;
			this.downArrow.bounds.Y = this.height - this.downArrow.bounds.Height - 8 - 10;

		}

		public override void draw(SpriteBatch b) {

			b.Draw(this.EmotesMenuBoxTexture, new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), new Rectangle?(Sprites.MenuBox.SourceRectangle), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			b.Draw(this.EmotesMenuBoxTexture, EmotesMenuBoxArrowPosition, new Rectangle?(EmotesMenuBoxArrow), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

			for(int index = 0; index < this.emoteSelectionButtons.Count; ++index) {
				b.Draw(this.EmotesTexture, new Vector2(this.emoteSelectionButtons[index].bounds.X + this.xPositionOnScreen, this.emoteSelectionButtons[index].bounds.Y + this.yPositionOnScreen), new Rectangle?(new Rectangle(emoteSize * (animationFrames - 1), (this.firstComponentIndex + index + 1) * emoteSize, emoteSize, emoteSize)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
			}

			if((double)this.upArrow.scale < 1.0) {
				this.upArrow.scale += 0.05f;
			}

			if((double)this.downArrow.scale < 1.0) {
				this.downArrow.scale += 0.05f;
			}

			b.Draw(this.EmotesMenuArrowsTexture, new Vector2(this.upArrow.bounds.X + this.xPositionOnScreen, this.upArrow.bounds.Y + this.yPositionOnScreen), new Rectangle?(Sprites.MenuArrow.UpSourceRectangle), Color.White * (this.firstComponentIndex == 0 ? 0.25f : 1f), 0.0f, Vector2.Zero, this.upArrow.scale, SpriteEffects.None, 0.9f);
			b.Draw(this.EmotesMenuArrowsTexture, new Vector2(this.downArrow.bounds.X + this.xPositionOnScreen, this.downArrow.bounds.Y + this.yPositionOnScreen), new Rectangle?(Sprites.MenuArrow.DownSourceRectangle), Color.White * (this.firstComponentIndex == this.totalEmotes - GetMaxAmmountComponentShowing() ? 0.25f : 1f), 0.0f, Vector2.Zero, this.downArrow.scale, SpriteEffects.None, 0.9f);

			//TODO: Change cursor to indicate clicable element. Investigate further
			if(this.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && !Game1.options.hardwareCursor) {
				//ModEntry.ModMonitor.Log($"Cursor: {Game1.mouseCursor}");
				//Game1.mouseCursor = 44;
				//Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 16, 16, 16);
				//Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Rectangle?(new Rectangle(Game1.mouseCursor, 16, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, (float)(4.0 + Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
				//b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (double)Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
			}

		}

	}

}
