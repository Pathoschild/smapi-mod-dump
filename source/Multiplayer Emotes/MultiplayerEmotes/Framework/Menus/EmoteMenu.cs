
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerEmotes.Framework.Constants;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MultiplayerEmotes.Menus {

	public class EmoteMenu : IClickableMenu {

		private List<ClickableComponent> emoteSelectionButtons;
		private Texture2D emoteMenuTexture;
		private Texture2D emoteTexture;
		private Texture2D chatBoxTexture;
		private int pageStartIndex;
		private ClickableComponent upArrow;
		private ClickableComponent downArrow;
		public bool IsOpen { get; set; }
		private readonly EmoteMenuButton emoteMenuButton;
		public int totalEmotes;

		public int emoteSize = 16;
		public int animationFrames = 4;
		public int maxRowComponents = 3;
		public int maxColComponents = 3;
		// Static arrow test
		//private Texture2D menuTexture;

		public EmoteMenu(IModHelper helper, EmoteMenuButton emoteMenuButton, Texture2D emoteMenuTexture, Texture2D chatBoxTexture, Texture2D emoteTexture, Vector2 position) {

			// Static arrow test
			//this.menuTexture = helper.Content.Load<Texture2D>("assets\\emoteBoxPrototype.png", ContentSource.ModFolder);
			this.emoteMenuButton = emoteMenuButton;
			this.emoteMenuTexture = emoteMenuTexture;
			this.chatBoxTexture = chatBoxTexture;
			this.emoteTexture = emoteTexture;

			this.width = 300;
			this.height = 250;

			this.emoteSelectionButtons = new List<ClickableComponent>();
			int spriteSize = emoteSize * Game1.pixelZoom + 8;
			for(int i = 0; i < maxRowComponents; ++i) {
				for(int j = 0; j < maxColComponents; j++) {
					this.emoteSelectionButtons.Add(new ClickableComponent(new Rectangle(j * spriteSize + 36, i * spriteSize + 16, spriteSize, spriteSize), string.Concat(j + (i * maxColComponents) + 1)));
				}
			}

			this.upArrow = new ClickableComponent(Sprites.ChatBox.UpArrow, nameof(Sprites.ChatBox.UpArrow));
			this.downArrow = new ClickableComponent(Sprites.ChatBox.DownArrow, nameof(Sprites.ChatBox.DownArrow));

			totalEmotes = (emoteTexture.Width / (animationFrames * emoteSize)) * ((emoteTexture.Height - emoteSize) / emoteSize);

			this.exitFunction += this.OnExit;
			IsOpen = false;

		}

		public override bool isWithinBounds(int x, int y) {
			Rectangle component = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
			return component.Contains(x, y);
		}

		private int getAmmountToScroll() {
			return maxColComponents * maxRowComponents;
		}

		public void leftClick(int x, int y, EmoteMenuButton emb) {

			if(isWithinBounds(x, y)) {

				int x1 = x - this.xPositionOnScreen;
				int y1 = y - this.yPositionOnScreen;

				if(this.upArrow.containsPoint(x1, y1)) {
					this.upArrowPressed(getAmmountToScroll());
				} else if(this.downArrow.containsPoint(x1, y1)) {
					this.downArrowPressed(getAmmountToScroll());
				}

				foreach(ClickableComponent emoteSelectionButton in this.emoteSelectionButtons) {
					if(emoteSelectionButton.containsPoint(x1, y1)) {
						Game1.player.IsEmoting = false;
						int emote = this.pageStartIndex + int.Parse(emoteSelectionButton.name);
						Game1.player.doEmote(emote * 4);
						Game1.playSound("coin");
						break;
					}
				}

			}

		}

		private void upArrowPressed(int amountToScroll) {
			if(this.pageStartIndex != 0) {
				Game1.playSound("Cowboy_Footstep");
			}
			this.pageStartIndex = Math.Max(0, this.pageStartIndex - amountToScroll);
			this.upArrow.scale = 0.75f;
		}

		private void downArrowPressed(int amountToScroll) {
			if(this.pageStartIndex != totalEmotes - getAmmountToScroll()) {
				Game1.playSound("Cowboy_Footstep");
			}
			this.pageStartIndex = Math.Min(totalEmotes - getAmmountToScroll(), this.pageStartIndex + amountToScroll);
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

		public void Open() {
			IsOpen = true;
			Game1.playSound("shwip");
		}

		public void Close() {
			IsOpen = false;
			Game1.playSound("shwip");
		}

		private void OnExit() {
		}

		public override void draw(SpriteBatch b) {
			/*// Static arrow test
			int xPosition = this.emoteMenuButton.xPositionOnScreen + this.emoteMenuButton.emoteMenuIcon.bounds.Width;
			int yPosition = this.yPositionOnScreen + (this.emoteMenuButton.yPositionOnScreen + (this.emoteMenuButton.height / 2)) - (this.yPositionOnScreen - 2 + (Sprites.Menu.LeftArrow.Height / 2));//this.emoteMenuIcon.bounds.Y - 248;
			int arrowWidth = Sprites.Menu.LeftArrow.Width;
			int arrowHeight = Sprites.Menu.LeftArrow.Height;

			b.Draw(menuTexture, new Rectangle(xPosition + arrowWidth - 6, yPositionOnScreen, this.width - arrowWidth, this.height), new Rectangle?(Sprites.Menu.EmoteBox), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			b.Draw(menuTexture, new Rectangle(xPosition, yPosition, Sprites.Menu.LeftArrow.Width, Sprites.Menu.LeftArrow.Height), new Rectangle?(Sprites.Menu.LeftArrow), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
			*/
			b.Draw(this.emoteMenuTexture, new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), new Rectangle?(new Rectangle(0, 0, 244, 300)), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			
			for(int index = 0; index < this.emoteSelectionButtons.Count; ++index) {
				b.Draw(this.emoteTexture, new Vector2((float)(this.emoteSelectionButtons[index].bounds.X + this.xPositionOnScreen + 4), (this.emoteSelectionButtons[index].bounds.Y + this.yPositionOnScreen + 4)), new Rectangle?(new Rectangle((emoteSize * (animationFrames - 1)), (this.pageStartIndex + index + 1) * emoteSize, emoteSize, emoteSize)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
			}

			if((double)this.upArrow.scale < 1.0) {
				this.upArrow.scale += 0.05f;
			}

			if((double)this.downArrow.scale < 1.0) {
				this.downArrow.scale += 0.05f;
			}

			b.Draw(this.chatBoxTexture, new Vector2((this.upArrow.bounds.X + this.xPositionOnScreen + 16), (float)(this.upArrow.bounds.Y + this.yPositionOnScreen + 16)), new Rectangle?(new Rectangle(156, 300, 32, 20)), Color.White * (this.pageStartIndex == 0 ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.upArrow.scale, SpriteEffects.None, 0.9f);
			b.Draw(this.chatBoxTexture, new Vector2((this.downArrow.bounds.X + this.xPositionOnScreen + 16), (float)(this.downArrow.bounds.Y + this.yPositionOnScreen + 16)), new Rectangle?(new Rectangle(192, 304, 32, 20)), Color.White * (this.pageStartIndex == this.totalEmotes - getAmmountToScroll() ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.downArrow.scale, SpriteEffects.None, 0.9f);

			//TODO: Change cursor to indicate clicable element
			if(isWithinBounds(Game1.getMouseX(), Game1.getMouseY())) {

			}

		}

	}

}
