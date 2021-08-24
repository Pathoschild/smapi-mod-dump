/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/RaisedGardenBeds
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaisedGardenBeds
{
	public class NewRecipeMenu : IClickableMenu
	{
		public List<string> VariantKeys;
		public ClickableTextureComponent OkButton;
		public ClickableTextureComponent StarIcon;

		private MouseState _oldMouseState;
		private int _timerBeforeStart;
		private bool _informationUp;
		private bool _isActive;

		private readonly string _titleString;
		private readonly Dictionary<string, string> _itemStrings;
		private readonly Dictionary<string, KeyValuePair<string, int>> _itemSprites;

		public static readonly Vector2 Dimensions = new Vector2(768, 512);
		private const int OkButtonId = 101;


		public NewRecipeMenu(List<string> variantKeys)
			: base(x: 0,  y: 0, width: 0,  height: 0)
		{
			Log.T($"Opened end of night menu: {this.GetType().FullName}");

			Game1.player.team.endOfNightStatus.UpdateState(ModEntry.EndOfNightState);
			this.VariantKeys = variantKeys;
			this.width = (int)Dimensions.X;
			this.height = ((int)Dimensions.Y / 2) + (this.VariantKeys.Count * Game1.smallestTileSize * 3 / 2 * Game1.pixelZoom);
			this.OkButton = new ClickableTextureComponent(
				bounds: Rectangle.Empty,
				texture: Game1.mouseCursors,
				sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
				scale: 1f)
			{
				myID = NewRecipeMenu.OkButtonId
			};
			
			this._isActive = true;
			this._timerBeforeStart = 250;
			Game1.player.completelyStopAnimatingOrDoingAction();
			Game1.player.freezePause = 100;
			this.gameWindowSizeChanged(Rectangle.Empty, Rectangle.Empty);
			this.populateClickableComponentList();

			string craftingString = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
			this._titleString = Translations.GetTranslation("menu.title.new");
			this._itemStrings = 
				this.VariantKeys
				.ToDictionary(
					vk => vk,
					vk => Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe",
						craftingString,
						OutdoorPot.GetDisplayNameFromVariantKey(variantKey: vk)));
			this._itemSprites = 
				this.VariantKeys
				.ToDictionary(
					vk => vk,
					vk => OutdoorPot.GetSpriteFromVariantKey(variantKey: vk));
		}

		protected override void cleanupBeforeExit()
		{
			Log.T("Exiting end of night menu.");
			base.cleanupBeforeExit();
		}

		public override void snapToDefaultClickableComponent()
		{
			this.currentlySnappedComponent = this.OkButton;
			this.snapCursorToCurrentSnappedComponent();
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			this.xPositionOnScreen = (Game1.uiViewport.Width / 2) - (this.width / 2);
			this.yPositionOnScreen = (Game1.uiViewport.Height / 2) - (this.height / 2);
			this.OkButton.bounds.Width = this.OkButton.bounds.Height = 64;
			this.OkButton.bounds.X = Math.Min(
				this.xPositionOnScreen + this.width + 4,
				Game1.uiViewport.Width - this.OkButton.bounds.Width - 4);
			this.OkButton.bounds.Y = Math.Min(
				this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth,
				Game1.uiViewport.Height - this.OkButton.bounds.Height - 4);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		public override void performHoverAction(int x, int y)
		{
		}

		public override void receiveKeyPress(Keys key)
		{
			if (Game1.options.doesInputListContain(Game1.options.cancelButton, key)
				|| Game1.options.doesInputListContain(Game1.options.menuButton, key)
				|| Game1.options.doesInputListContain(Game1.options.journalButton, key))
			{
				this.OkButtonClicked();
			}
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
			if ((b == Buttons.Start || b == Buttons.B) && this._isActive)
			{
				this.OkButtonClicked();
			}
		}

		public override void update(GameTime time)
		{
			if (!this._isActive)
			{
				this.exitThisMenu();
				return;
			}
			
			if (this._timerBeforeStart > 0)
			{
				this._informationUp = true;
				this._timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
				if (this._timerBeforeStart <= 0 && Game1.options.SnappyMenus)
				{
					this.populateClickableComponentList();
					this.snapToDefaultClickableComponent();
				}
				return;
			}

			this._oldMouseState = Game1.input.GetMouseState();
			if (this._isActive && !this._informationUp && this.StarIcon != null)
			{
				if (this.StarIcon.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
				{
					this.StarIcon.sourceRect.X = 294;
				}
				else
				{
					this.StarIcon.sourceRect.X = 310;
				}
			}
			if (this._isActive && this.StarIcon != null && !this._informationUp
				&& (this._oldMouseState.LeftButton == ButtonState.Pressed || (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A)))
				&& this.StarIcon.containsPoint(this._oldMouseState.X, this._oldMouseState.Y))
			{
				this._informationUp = true;
				Game1.player.completelyStopAnimatingOrDoingAction();
				Game1.playSound("bigSelect");
				Game1.player.freezePause = 100;
			}

			if (!this._isActive || !this._informationUp)
				return;
			
			Game1.player.completelyStopAnimatingOrDoingAction();
			if (this.OkButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
			{
				this.OkButton.scale = Math.Min(1.1f, this.OkButton.scale + 0.05f);
				if ((this._oldMouseState.LeftButton == ButtonState.Pressed || (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A))) && readyToClose())
				{
					this.OkButtonClicked();
				}
			}
			else
			{
				this.OkButton.scale = Math.Max(1f, this.OkButton.scale - 0.05f);
			}
			Game1.player.freezePause = 100;
		}

		public override void draw(SpriteBatch b)
		{
			if (this._timerBeforeStart > 0)
				return;

			// Blackout
			b.Draw(
				texture: Game1.fadeToBlackRect,
				destinationRectangle: new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
				color: Color.Black * 0.5f);
			
			if (!this._informationUp && this._isActive && this.StarIcon != null)
			{
				this.StarIcon.draw(b);
			}
			else
			{
				if (!this._informationUp)
					return;
				
				// Draw popup header
				const int wh = 16;
				Vector2 padding = new Vector2(22, -8) * Game1.pixelZoom;
				const float iconScale = 3f;
				Vector2 iconSize = new Vector2(26, 20);
				Vector2 textSize = Game1.dialogueFont.MeasureString(this._titleString);
				textSize = new Vector2(
					textSize.X + padding.X + (iconSize.X * iconScale),
					Math.Max(textSize.Y, iconSize.Y * iconScale) + padding.Y);
				Vector2 positionPadded = new Vector2(
					this.xPositionOnScreen + ((this.width - textSize.X - (wh * Game1.pixelZoom)) / 2),
					this.yPositionOnScreen - textSize.Y - (wh * Game1.pixelZoom / 2) - (2 * Game1.pixelZoom));
				Point sourceOrigin = new Point(260, 310);
				// background
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle(
						(int)(positionPadded.X + (wh * Game1.pixelZoom / 2)),
						(int)(positionPadded.Y + (wh * Game1.pixelZoom / 2)),
						(int)(textSize.X),
						(int)(textSize.Y + (wh * Game1.pixelZoom))),
					sourceRectangle: new Rectangle(360, 437, 1, 8),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// icon
				b.Draw(
					texture: Game1.mouseCursors,
					position: positionPadded + new Vector2(wh * Game1.pixelZoom) + new Vector2(0, -8f * iconScale),
					sourceRectangle: new Rectangle(420, 488, (int)iconSize.X, (int)iconSize.Y),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: iconScale,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// top-left
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X, (int)positionPadded.Y, wh * Game1.pixelZoom, wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X, sourceOrigin.Y, wh, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// top-right
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X + (int)textSize.X, (int)positionPadded.Y, wh * Game1.pixelZoom, wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X + 28, sourceOrigin.Y, wh, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// bottom-left
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X, (int)positionPadded.Y + (int)textSize.Y + (wh * Game1.pixelZoom), wh * Game1.pixelZoom, wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X, sourceOrigin.Y + 16, wh, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// bottom-right
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X + (int)textSize.X, (int)positionPadded.Y + (int)textSize.Y + (wh * Game1.pixelZoom), wh * Game1.pixelZoom, wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X + 28, sourceOrigin.Y + 16, wh, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// top
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)(positionPadded.X + (wh * Game1.pixelZoom)), (int)positionPadded.Y, (int)textSize.X - (wh * Game1.pixelZoom), wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X + wh, sourceOrigin.Y, 1, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// bottom
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)(positionPadded.X + (wh * Game1.pixelZoom)), (int)(positionPadded.Y + textSize.Y + (wh * Game1.pixelZoom)), (int)textSize.X - (wh * Game1.pixelZoom), wh * Game1.pixelZoom),
					sourceRectangle: new Rectangle(sourceOrigin.X + wh, sourceOrigin.Y + 16, 1, wh),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// left
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X, (int)positionPadded.Y + (wh * Game1.pixelZoom), wh * Game1.pixelZoom, (int)textSize.Y),
					sourceRectangle: new Rectangle(sourceOrigin.X, sourceOrigin.Y + wh, wh, 1),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// right
				b.Draw(texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle((int)positionPadded.X + (int)textSize.X, (int)positionPadded.Y + (wh * Game1.pixelZoom), wh * Game1.pixelZoom, (int)textSize.Y),
					sourceRectangle: new Rectangle(sourceOrigin.X + 28, sourceOrigin.Y + wh, wh, 1),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// title text
				b.DrawString(
					spriteFont: Game1.dialogueFont,
					text: this._titleString,
					position: positionPadded + new Vector2(iconSize.X * iconScale, 0) + new Vector2(wh * Game1.pixelZoom) + new Vector2(padding.X / 4, -6f * iconScale),
					color: Game1.textColor);


				// Draw actual popup
				Game1.drawDialogueBox(
					x: this.xPositionOnScreen,
					y: this.yPositionOnScreen,
					width: this.width,
					height: this.height,
					speaker: false,
					drawOnlyBox: true);

				const int paddingY = 3;
				int x = this.xPositionOnScreen + (this.width / 2);
				int y = this.yPositionOnScreen;
				int yOffset = IClickableMenu.spaceToClearTopBorder;
				
				foreach (string variantKey in this.VariantKeys)
				{
					yOffset += (int)Game1.dialogueFont.MeasureString(_itemStrings[variantKey]).Y + (paddingY * Game1.pixelZoom)
						 + ((Game1.smallestTileSize + paddingY) * 2);
					int xOffset = -(int)((Game1.smallFont.MeasureString(_itemStrings[variantKey]).X / 2) - (Game1.smallestTileSize * Game1.pixelZoom));
					b.DrawString(
						spriteFont: Game1.smallFont,
						text: _itemStrings[variantKey],
						position: new Vector2(
							x + xOffset,
							y + yOffset),
						color: Game1.textColor);

					yOffset -= (Game1.smallestTileSize * Game1.pixelZoom);
					b.Draw(
						texture: ModEntry.Sprites[this._itemSprites[variantKey].Key],
						sourceRectangle: OutdoorPot.GetSpriteSourceRectangle(spriteIndex: this._itemSprites[variantKey].Value),
						position: new Vector2(
							x + xOffset - (Game1.smallestTileSize * 1.5f * Game1.pixelZoom),
							y + yOffset),
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: Game1.pixelZoom,
						effects: SpriteEffects.None,
						layerDepth: 1f);
					yOffset += (Game1.smallestTileSize * Game1.pixelZoom);
				}
				this.OkButton.draw(b);
				
				if (!Game1.options.SnappyMenus)
				{
					Game1.mouseCursorTransparency = 1f;
					this.drawMouse(b);
				}
			}
		}

		private void OkButtonClicked()
		{
			this._isActive = false;
			this._informationUp = false;
		}

		public static void Push(List<string> variantKeys)
		{
			if (variantKeys == null)
				return;

			for (int i = variantKeys.Count - 1; i >= 0; --i)
			{
				if (!ModEntry.ItemDefinitions.ContainsKey(variantKeys[i]))
				{
					variantKeys.RemoveAt(i);
				}
			}
			if (variantKeys.Count > 0)
			{
				Game1.endOfNightMenus.Push(new NewRecipeMenu(variantKeys: variantKeys));
			}
		}
	}
}
