
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultiplayerEmotes.Framework.Constants;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerEmotes.Menus {

	public class EmotesMenuButton : IClickableMenu {

		private readonly string HoverText = "Emotes";
		public ClickableTextureComponent EmoteMenuButtonComponent;
		public EmotesMenuBox EmotesMenuBoxComponent;
		public bool IsHovering;

		private Texture2D EmotesTexture;

		public bool AnimatedEmoteIcon { get; set; }
		public bool ShowTooltipOnHover { get; set; }

		public bool AnimationOnHover;
		public int AnimationCooldownTime = 10000;

		private TemporaryAnimatedSprite AnimatedSpriteIcon;
		private int animationTimer;
		public bool playAnimation;

		private Vector2 iconPositionOffset;

		public bool IsBeingDragged;

		private bool WasRightDown;

		private readonly IModHelper helper;
		private readonly ModData modData;
		private readonly ModConfig modConfig;

		public EmotesMenuButton(IModHelper helper, ModConfig modConfig, ModData modData) {

			this.helper = helper;
			this.modConfig = modConfig;
			this.modData = modData;

			Rectangle sourceRect = Sprites.MenuButton.SourceRectangle;
			Rectangle targetRect = new Rectangle(modData.MenuPosition.X, modData.MenuPosition.Y, sourceRect.Width * Game1.pixelZoom, sourceRect.Height * Game1.pixelZoom);

			this.xPositionOnScreen = targetRect.X;
			this.yPositionOnScreen = targetRect.Y;

			this.width = targetRect.Width;
			this.height = targetRect.Height;

			this.EmotesTexture = Sprites.Emotes.Texture;
			this.EmoteMenuButtonComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), Sprites.MenuButton.Texture, sourceRect, 4f, false);

			//Texture2D chatBoxTexture = helper.Content.Load<Texture2D>(Sprites.ChatBox.AssetName, ContentSource.GameContent);
			this.EmotesMenuBoxComponent = new EmotesMenuBox(helper, this, EmotesTexture);

			IsBeingDragged = false;

			animationTimer = AnimationCooldownTime;
			AnimatedEmoteIcon = modConfig.AnimateEmoteButtonIcon;
			//AnimationOnHover = true; // Not in use
			AnimatedSpriteIcon = new TemporaryAnimatedSprite(Sprites.Emotes.AssetName, new Rectangle(0, 0, 16, 16), 250f, 4, 0, new Vector2(this.EmoteMenuButtonComponent.bounds.X + 15, this.EmoteMenuButtonComponent.bounds.Y + 15), false, false, 0.9f, 0f, Color.White, 2.0f, 0f, 0f, 0f, true);

			ShowTooltipOnHover = modConfig.ShowTooltipOnHover;

			initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false);
			UpdatePosition();

			SubscribeEvents();

		}

		private void SubscribeEvents() {
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
			helper.Events.Display.RenderedHud += this.OnRenderedHud;
			helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		private void UnsubscribeEvents() {
			helper.Events.Display.MenuChanged -= this.OnMenuChanged;
			helper.Events.Display.RenderedHud -= this.OnRenderedHud;
			helper.Events.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;
			helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
		}

		/// <summary>Raised after the player scrolls the mouse wheel.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e) {
			var cursor = helper.Input.GetCursorPosition().ScreenPixels;
			if (this.isWithinBounds((int)cursor.X, (int)cursor.Y) && this.EmotesMenuBoxComponent.IsOpen) {
				MouseState mouseState = Game1.oldMouseState;
				Game1.oldMouseState = new MouseState(mouseState.X, mouseState.Y, e.NewValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
				receiveScrollWheelAction(e.Delta);
			}
		}

		/// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
			if(e.NewMenu is TitleMenu) {
				UnsubscribeEvents();
				SaveData();
			}
		}

		private void SaveData() {
			modData.MenuPosition = new Point(this.xPositionOnScreen, this.yPositionOnScreen);
			helper.Data.WriteJsonFile("data.json", modData);
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
			if (this.IsBeingDragged && e.Button == SButton.MouseRight) {
				helper.Input.Suppress(e.Button);
			}
		}

		private void UpdatePosition() {

			if(this.IsBeingDragged) {
				this.xPositionOnScreen = Game1.getMouseX() - (int)iconPositionOffset.X;
				this.yPositionOnScreen = Game1.getMouseY() - (int)iconPositionOffset.Y;
			}

			Utility.makeSafe(ref xPositionOnScreen, ref yPositionOnScreen, this.width, this.height);

			if(EmotesMenuBoxComponent != null) {
				// Change the position of the emotes box relative to the button. The box will chage of side when not enough space left to fit the menu.
				this.EmotesMenuBoxComponent.UpdatePosition(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
			}

			this.EmoteMenuButtonComponent.bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
			if(playAnimation && ShouldPlayAnimation()) {
				this.AnimatedSpriteIcon.position = new Vector2(this.EmoteMenuButtonComponent.bounds.X + 15, this.EmoteMenuButtonComponent.bounds.Y + 15);
			}

		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {

			bool wasRightDown = this.WasRightDown;
			bool isRightDown = helper.Input.IsDown(SButton.MouseRight);
			this.WasRightDown = isRightDown;

			if(!ShouldDragIcon(wasRightDown, isRightDown)) {

				if(this.EmoteMenuButtonComponent.containsPoint(x, y)) {
					EmotesMenuBoxComponent.Toggle();
				} else if(this.EmotesMenuBoxComponent.IsOpen) {
					this.EmotesMenuBoxComponent.receiveLeftClick(x, y, playSound);
				}

				this.EmoteMenuButtonComponent.scale = 4f;

				//if(this.EmoteMenuButtonComponent.containsPoint(x, y)) {
				//	if(EmotesMenuBoxComponent.IsOpen) {
				//		EmotesMenuBoxComponent.Close();
				//	} else {
				//		EmotesMenuBoxComponent.Open();
				//	}
				//	this.EmoteMenuButtonComponent.scale = 4f;
				//} else if(this.EmotesMenuBoxComponent.IsOpen && this.EmotesMenuBoxComponent.isWithinBounds(x, y)) {
				//	this.EmotesMenuBoxComponent.leftClick(x, y);
				//} else {
				//	if(this.EmotesMenuBoxComponent.IsOpen) {
				//		this.EmotesMenuBoxComponent.IsOpen = false;
				//		this.EmoteMenuButtonComponent.scale = 4f;
				//	}
				//	if(this.isWithinBounds(x, y)) {
				//		EmotesMenuBoxComponent.IsOpen = true;
				//	}
				//}

			}

		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			this.UpdatePosition();
		}

		public override void clickAway() {
			base.clickAway();
			this.EmotesMenuBoxComponent.clickAway();
		}

		public override bool isWithinBounds(int x, int y) {
			UpdatePosition();
			Rectangle component = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

			//#if DEBUG
			//			ModEntry.ModMonitor.Log($"(x: {x}, y: {y}) (xPositionOnScreen: {xPositionOnScreen}, yPositionOnScreen: {yPositionOnScreen}), (width: {width}, height: {height})");
			//#endif
			if(component.Contains(x, y)) {
				return true;
			} else if(EmotesMenuBoxComponent.IsOpen) {
				return EmotesMenuBoxComponent.isWithinBounds(x, y);
			} else {
				return false;
			}

			/*
			int d1x = x - (xPositionOnScreen + width);	// d1x = b->min.x - a->max.x;
			int d1y = y - (yPositionOnScreen + height); // d1y = b->min.y - a->max.y;
			int d2x = xPositionOnScreen - x;			// d2x = a->min.x - b->max.x;
			int d2y = yPositionOnScreen - y;			// d2y = a->min.y - b->max.y;

			ModEntry.ModMonitor.Log($"(d1x: {d1x}, d1y: {d1y}) (d2x: {d2x}, d2y: {d2y})");

			if((d1x > 0 || d1y > 0) || (d2x > 0 || d2y > 0)) {
				return emoteMenu.isWithinBounds(x, y);
			} else {
				return true;
			}
			*/
		}

		public override void receiveScrollWheelAction(int direction) {
			if(this.EmotesMenuBoxComponent.IsOpen) {
				this.EmotesMenuBoxComponent.receiveScrollWheelAction(direction);
			}
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
			this.UpdatePosition();
		}

		public override void update(GameTime time) {

			this.UpdateAnimationTimer(time);
			//this.UpdateHoldToDragTimer(time);

			bool wasRightDown = this.WasRightDown;
			bool isRightDown = helper.Input.IsDown(SButton.MouseRight);
			this.WasRightDown = isRightDown;

			if (ShouldDragIcon(wasRightDown, isRightDown)) {

				IsBeingDragged = true;

				this.xPositionOnScreen = Game1.getMouseX() - (int)iconPositionOffset.X;
				this.yPositionOnScreen = Game1.getMouseY() - (int)iconPositionOffset.Y;


				iconPositionOffset = new Vector2(Game1.getMouseX() - this.EmoteMenuButtonComponent.bounds.X, Game1.getMouseY() - this.EmoteMenuButtonComponent.bounds.Y);

				//Dont let the mouse grab the air, and set to grab at least the border
				//Horizontal position
				if(iconPositionOffset.X < 0) {
					iconPositionOffset.X = 0;
				} else if(iconPositionOffset.X >= EmoteMenuButtonComponent.bounds.Width) {
					iconPositionOffset.X = EmoteMenuButtonComponent.bounds.Width - 1;
				}

				//Vertical position
				if(iconPositionOffset.Y < 0) {
					iconPositionOffset.Y = 0;
				} else if(iconPositionOffset.Y >= EmoteMenuButtonComponent.bounds.Height) {
					iconPositionOffset.Y = EmoteMenuButtonComponent.bounds.Height - 1;
				}

			} else {
				IsBeingDragged = false;
			}

			if((playAnimation && AnimatedEmoteIcon) /*|| (isHovering && AnimationOnHover)*/) {
				if(IsPlayingAnimation()) {
					AnimatedSpriteIcon.update(time);
				} else {
					playAnimation = false;
					AnimatedSpriteIcon.reset();
				}
			}

		}

		private bool ShouldDragIcon(bool wasDown, bool isDown)
		{
			return wasDown && isDown && this.EmoteMenuButtonComponent.containsPoint(Game1.getMouseX(), Game1.getMouseY());
		}

		public bool ShouldPlayAnimation() {
			return this.AnimatedSpriteIcon != null && AnimatedEmoteIcon && !IsBeingDragged;
		}

		public bool IsPlayingAnimation() {
			return this.AnimatedSpriteIcon != null && AnimatedSpriteIcon.currentParentTileIndex < AnimatedSpriteIcon.animationLength - 1;
		}

		/*
		public void UpdateHoldToDragTimer(GameTime time) {
			if(this.holdToDragTimer <= HoldToDragTime && ShouldDragIcon()) {
				this.holdToDragTimer += time.ElapsedGameTime.Milliseconds;
			} else if(this.holdToDragTimer > HoldToDragTime && MouseStateMonitor.MouseHolded()) {

				iconPositionOffset = new Vector2(Game1.getMouseX() - this.emoteMenuIcon.bounds.X, Game1.getMouseY() - this.emoteMenuIcon.bounds.Y);

				//Dont let the mouse grab the air, and set to grab at least the border
				//Horizontal position
				if(iconPositionOffset.X < 0) {
					iconPositionOffset.X = 0;
				} else if(iconPositionOffset.X >= emoteMenuIcon.bounds.Width) {
					iconPositionOffset.X = emoteMenuIcon.bounds.Width - 1;
				}

				//Vertical position
				if(iconPositionOffset.Y < 0) {
					iconPositionOffset.Y = 0;
				} else if(iconPositionOffset.Y >= emoteMenuIcon.bounds.Height) {
					iconPositionOffset.Y = emoteMenuIcon.bounds.Height - 1;
				}

				IsBeingDragged = true;

			} else {
				this.holdToDragTimer = 0;
				IsBeingDragged = false;
			}
		}
		*/

		public void UpdateAnimationTimer(GameTime time) {

			if(ShouldPlayAnimation() && !playAnimation) {
				// If there is no animation playing start countdown for next animation
				if(this.animationTimer > 0) {
					playAnimation = false;
					this.animationTimer -= time.ElapsedGameTime.Milliseconds;
				} else { // If timer is less or equal to 0
					playAnimation = true;
					this.animationTimer = AnimationCooldownTime;
				}
			}

		}

		private void DrawAnimatedIcon(SpriteBatch b) {

			this.EmoteMenuButtonComponent.tryHover(Game1.getMouseX(), Game1.getMouseY(), 0.4f);
			this.EmoteMenuButtonComponent.tryHover(Game1.getMouseX(), Game1.getMouseY(), 0.4f);

			this.EmoteMenuButtonComponent.draw(b);

			if(playAnimation && ShouldPlayAnimation()) {
				this.AnimatedSpriteIcon.draw(b, true, 0, 0, 1f);
			} else {
				b.Draw(EmotesTexture, new Vector2(this.EmoteMenuButtonComponent.bounds.X + 15, this.EmoteMenuButtonComponent.bounds.Y + 15), new Rectangle?(new Rectangle(48, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.9f);
			}

		}

		private void DrawTooltipText(SpriteBatch b) {
			this.IsHovering = isWithinBounds(Game1.getMouseX(), Game1.getMouseY());
			if(IsHovering && !EmotesMenuBoxComponent.IsOpen && !IsBeingDragged && ShowTooltipOnHover) {
				drawHoverText(b, this.HoverText, Game1.dialogueFont);
			}
		}

		private void DrawEmoteMenu(SpriteBatch b) {
			if(this.EmotesMenuBoxComponent.IsOpen) {
				this.EmotesMenuBoxComponent.draw(b);
			}
		}

		/// <summary>Raised after drawing the HUD (item toolbar, clock, etc) to the sprite batch, but before it's rendered to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnRenderedHud(object sender, RenderedHudEventArgs e) {
			this.Draw(e.SpriteBatch);
		}

		public void Draw(SpriteBatch b) {
			if(Context.IsPlayerFree && Game1.activeClickableMenu == null) {
				this.UpdatePosition();
				this.DrawAnimatedIcon(b);
				this.DrawTooltipText(b);
				this.DrawEmoteMenu(b);
			}
		}

	}

}
