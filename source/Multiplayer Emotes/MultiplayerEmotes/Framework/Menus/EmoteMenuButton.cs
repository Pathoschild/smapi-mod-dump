
using System;
using System.IO;
using System.Reflection;
using MultiplayerEmotes.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultiplayerEmotes.Events;
using MultiplayerEmotes.Framework.Constants;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerEmotes.Menus {

	public class EmoteMenuButton : IClickableMenu {

		private string hoverText;
		public ClickableTextureComponent emoteMenuIcon;
		public EmoteMenu emoteMenu;
		public bool isHovering;

		private Texture2D emoteMenuTexture;
		private Texture2D emoteTexture;
		private Rectangle targetRect;
		private Rectangle sourceRect;

		public bool AnimatedEmoteIcon { get; set; }
		public bool ShowTooltipOnHover { get; set; }

		public bool AnimationOnHover { get; set; }
		public int AnimationCooldownTime { get; set; }

		private TemporaryAnimatedSprite iconAnimation;
		private int animationTimer;
		public bool playAnimation;

		private Vector2 iconPositionOffset;

		public bool IsBeingDragged { get; private set; }

		MouseState currentMouseState;
		MouseState previousMouseState;

		private readonly IModHelper helper;
		private readonly ModData modData;
		private readonly ModConfig modConfig;

		public EmoteMenuButton(IModHelper helper, ModConfig modConfig, ModData modData) {

			this.helper = helper;
			this.modConfig = modConfig;
			this.modData = modData;

			this.sourceRect = new Rectangle(301, 288, 15, 15);
			this.targetRect = new Rectangle((int)modData.MenuPosition.X, (int)modData.MenuPosition.Y, sourceRect.Width * Game1.pixelZoom, sourceRect.Height * Game1.pixelZoom);

			this.xPositionOnScreen = targetRect.X;
			this.yPositionOnScreen = targetRect.Y;

			this.width = targetRect.Width;
			this.height = targetRect.Height;

			this.emoteMenuTexture = helper.Content.Load<Texture2D>(Sprites.Menu.AssetName, ContentSource.ModFolder);
			this.emoteTexture = Sprites.Emotes.Texture;
			this.emoteMenuIcon = new ClickableTextureComponent(new Rectangle(this.targetRect.X, this.targetRect.Y, this.width, this.height), Game1.mouseCursors, sourceRect, 4f, false);

			//Texture2D chatBoxTexture = helper.Content.Load<Texture2D>(Sprites.ChatBox.AssetName, ContentSource.GameContent);
			this.emoteMenu = new EmoteMenu(helper, this, emoteMenuTexture, Sprites.ChatBox.Texture, emoteTexture, new Vector2(this.targetRect.X, this.targetRect.Y));

			IsBeingDragged = false;

			AnimationCooldownTime = 10000;
			animationTimer = AnimationCooldownTime;
			AnimatedEmoteIcon = modConfig.AnimateEmoteButtonIcon;
			AnimationOnHover = true; // Not in use
			iconAnimation = new TemporaryAnimatedSprite(Sprites.Emotes.AssetName, new Rectangle(0, 0, 16, 16), 250f, 4, 0, new Vector2(this.emoteMenuIcon.bounds.X + 15, this.emoteMenuIcon.bounds.Y + 15), false, false, 0.9f, 0f, Color.White, 2.0f, 0f, 0f, 0f, true);

			ShowTooltipOnHover = modConfig.ShowTooltipOnHover;
			hoverText = "Emotes";
			isHovering = false;

			emoteMenu.IsOpen = false;

			initialize(this.targetRect.X, this.targetRect.Y, this.width, this.height, false);
			updatePosition();

			currentMouseState = Mouse.GetState();
			previousMouseState = currentMouseState;

			SubscribeEvents();

		}

		private void SubscribeEvents() {
			MenuEvents.MenuChanged += this.OnMenuChanged;
			GraphicsEvents.OnPostRenderHudEvent += this.OnPostRenderHudEvent;
			ControlEvents.MouseChanged += this.OnMouseChanged;
			InputEvents.ButtonPressed += this.OnButtonPressed;
		}

		private void UnsubscribeEvents() {
			MenuEvents.MenuChanged -= this.OnMenuChanged;
			GraphicsEvents.OnPostRenderHudEvent -= this.OnPostRenderHudEvent;
			ControlEvents.MouseChanged -= this.OnMouseChanged;
			InputEvents.ButtonPressed -= this.OnButtonPressed;
		}

		private void OnMouseChanged(object sender, EventArgsMouseStateChanged e) {
			MouseStateMonitor.UpdateMouseState();
			if(MouseStateMonitor.ScrollChanged() && this.isWithinBounds(MouseStateMonitor.CurrentMouseState.X, MouseStateMonitor.CurrentMouseState.Y) && this.emoteMenu.IsOpen) {
				MouseState mouseState = Game1.oldMouseState;
				Game1.oldMouseState = new MouseState(mouseState.X, mouseState.Y, MouseStateMonitor.CurrentMouseState.ScrollWheelValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
				receiveScrollWheelAction(MouseStateMonitor.ScrollValueDifference());
			}
		}

		private void OnMenuChanged(object sender, EventArgsClickableMenuChanged e) {
			if(e.NewMenu is TitleMenu) {
				UnsubscribeEvents();
				SaveData();
			}
		}

		private void SaveData() {
			modData.MenuPosition = new Vector2(this.xPositionOnScreen, this.yPositionOnScreen);
			helper.WriteJsonFile("data.json", modData);
		}

		private void OnButtonPressed(object sender, EventArgsInput e) {
			if(this.IsBeingDragged && e.Button == SButton.MouseRight) {
				e.SuppressButton();
			}
		}

		private void updatePosition() {

			if(this.IsBeingDragged) {
				this.targetRect.X = Game1.getMouseX() - (int)iconPositionOffset.X;
				this.targetRect.Y = Game1.getMouseY() - (int)iconPositionOffset.Y;
			}

			Utility.makeSafe(ref this.targetRect.X, ref this.targetRect.Y, this.targetRect.Width, this.targetRect.Height);

			if(emoteMenu != null) {
				this.emoteMenu.xPositionOnScreen = this.xPositionOnScreen + this.emoteMenuIcon.bounds.Width;
				this.emoteMenu.yPositionOnScreen += (this.yPositionOnScreen + (this.height / 2)) - (this.emoteMenu.yPositionOnScreen - 2 + (emoteMenu.height / 2));//this.emoteMenuIcon.bounds.Y - 248;
				Utility.makeSafe(ref this.emoteMenu.xPositionOnScreen, ref this.emoteMenu.yPositionOnScreen, this.emoteMenu.width, this.emoteMenu.height);
			}

			this.xPositionOnScreen = targetRect.X;
			this.yPositionOnScreen = targetRect.Y;

			this.width = targetRect.Width;
			this.height = targetRect.Height;

			this.emoteMenuIcon.bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
			if(playAnimation && ShouldPlayAnimation()) {
				this.iconAnimation.position = new Vector2(this.emoteMenuIcon.bounds.X + 15, this.emoteMenuIcon.bounds.Y + 15);
			}

		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {

			if(!ShouldDragIcon()) {

				if(this.emoteMenuIcon.containsPoint(x, y)) {
					if(emoteMenu.IsOpen) {
						emoteMenu.Close();
					} else {
						emoteMenu.Open();
					}
					this.emoteMenuIcon.scale = 4f;
				} else if(this.emoteMenu.IsOpen && this.emoteMenu.isWithinBounds(x, y)) {
					this.emoteMenu.leftClick(x, y, this);
				} else {
					if(this.emoteMenu.IsOpen) {
						this.emoteMenu.IsOpen = false;
						this.emoteMenuIcon.scale = 4f;
					}
					if(this.isWithinBounds(x, y)) {
						emoteMenu.IsOpen = true;
					}
				}

			}

		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			this.updatePosition();
		}

		public override void clickAway() {
			base.clickAway();
			if(!this.emoteMenu.IsOpen || !this.emoteMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY())) {
				emoteMenu.IsOpen = false;
			}
		}

		public bool isWithinBounds(Vector2 position) {
			return isWithinBounds((int)position.X, (int)position.Y);
		}

		public override bool isWithinBounds(int x, int y) {
			updatePosition();
			Rectangle component = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

//#if DEBUG
//			ModEntry.ModMonitor.Log($"(x: {x}, y: {y}) (xPositionOnScreen: {xPositionOnScreen}, yPositionOnScreen: {yPositionOnScreen}), (width: {width}, height: {height})");
//#endif
			if(component.Contains(x, y)) {
				return true;
			} else if(emoteMenu.IsOpen) {
				return emoteMenu.isWithinBounds(x, y);
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
			if(this.emoteMenu.IsOpen) {
				this.emoteMenu.receiveScrollWheelAction(direction);
			}
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
			this.updatePosition();
		}

		public override void update(GameTime time) {

			this.UpdateAnimationTimer(time);
			//this.UpdateHoldToDragTimer(time);

			if(ShouldDragIcon()) {

				IsBeingDragged = true;

				this.targetRect.X = Game1.getMouseX() - (int)iconPositionOffset.X;
				this.targetRect.Y = Game1.getMouseY() - (int)iconPositionOffset.Y;


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

			} else {
				IsBeingDragged = false;
			}

			if((playAnimation && AnimatedEmoteIcon) /*|| (isHovering && AnimationOnHover)*/) {
				if(IsPlayingAnimation()) {
					iconAnimation.update(time);
				} else {
					playAnimation = false;
					iconAnimation.reset();
				}
			}

		}

		private bool ShouldDragIcon() {
			return MouseStateMonitor.MouseHolded() && !MouseStateMonitor.MouseReleased() && this.emoteMenuIcon.containsPoint(Game1.getMouseX(), Game1.getMouseY());
		}

		public bool ShouldPlayAnimation() {
			return this.iconAnimation != null && AnimatedEmoteIcon && !IsBeingDragged;
		}

		public bool IsPlayingAnimation() {
			return this.iconAnimation != null && iconAnimation.currentParentTileIndex < iconAnimation.animationLength - 1;
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

			this.emoteMenuIcon.tryHover(Game1.getMouseX(), Game1.getMouseY(), 0.4f);
			this.emoteMenuIcon.tryHover(Game1.getMouseX(), Game1.getMouseY(), 0.4f);

			this.emoteMenuIcon.draw(b);

			if(playAnimation && ShouldPlayAnimation()) {
				this.iconAnimation.draw(b, true, 0, 0, 1f);
			} else {
				b.Draw(emoteTexture, new Vector2(this.emoteMenuIcon.bounds.X + 15, this.emoteMenuIcon.bounds.Y + 15), new Rectangle?(new Rectangle(48, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.9f);
			}

		}

		private void DrawTooltipText(SpriteBatch b) {
			this.isHovering = isWithinBounds(Game1.getMouseX(), Game1.getMouseY());
			if(isHovering && !emoteMenu.IsOpen && !IsBeingDragged && ShowTooltipOnHover) {
				drawHoverText(b, this.hoverText, Game1.dialogueFont);
			}
		}

		private void DrawEmoteMenu(SpriteBatch b) {
			if(this.emoteMenu.IsOpen) {
				this.emoteMenu.draw(b);
			}
		}

		private void OnPostRenderHudEvent(object sender, EventArgs e) {
			this.Draw(Game1.spriteBatch);
		}

		public void Draw(SpriteBatch b) {
			if(Context.IsPlayerFree && Game1.activeClickableMenu == null) {
				this.updatePosition();
				this.DrawAnimatedIcon(b);
				this.DrawTooltipText(b);
				this.DrawEmoteMenu(b);
			}
		}

	}

}
