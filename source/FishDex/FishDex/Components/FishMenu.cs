using FishDex.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using static FishDex.DataParser;
using SObject = StardewValley.Object;

namespace FishDex.Components
{
	/// <summary>A UI which shows information about fishes.</summary>
	internal class FishMenu : IClickableMenu
	{
		/*********
        ** Fields
        *********/

		private readonly IEnumerable<FishInfo> Fishes;

		/// <summary>Encapsulates logging and monitoring.</summary>
		private readonly IMonitor Monitor;

		/// <summary>The aspect ratio of the page background.</summary>
		private readonly Vector2 AspectRatio = new Vector2(Sprites.Letter.Sprite.Width, Sprites.Letter.Sprite.Height);

		/// <summary>Simplifies access to private game code.</summary>
		private readonly IReflectionHelper Reflection;

		/// <summary>The amount to scroll long content on each up/down scroll.</summary>
		private readonly int ScrollAmount;

		/// <summary>Whether to show all the fishes.</summary>
		private readonly bool ShowAll;

		/// <summary>The clickable 'scroll to top' icon.</summary>
		private readonly ClickableTextureComponent ScrollToTopButton;

		/// <summary>The clickable 'scroll to bottom' icon.</summary>
		private readonly ClickableTextureComponent ScrollToBottomButton;

		/// <summary>The spacing around the scroll buttons.</summary>
		private readonly int ScrollButtonGutter = 15;

		/// <summary>The maximum pixels to scroll.</summary>
		private int MaxScroll;

		/// <summary>The number of pixels to scroll.</summary>
		private int CurrentScroll;

		private List<ClickableTextureComponent> ClickableFishTextures = new List<ClickableTextureComponent>();

		private String hoverText = "";


		/*********
        ** Public methods
        *********/
		/****
        ** Constructors
        ****/
		/// <summary>Construct an instance.</summary>
		/// <param name="parser">Provides access to parsed fish data.</param>
		/// <param name="monitor">Encapsulates logging and monitoring.</param>
		/// <param name="reflectionHelper">Simplifies access to private game code.</param>
		/// <param name="scroll">The amount to scroll long content on each up/down scroll.</param>
		/// <param name="showAll">Whether to show all the fishes.</param>
		public FishMenu(DataParser parser, IMonitor monitor, IReflectionHelper reflectionHelper, int scroll, bool showAll)
		{
			// save data
			this.Fishes = parser.GetFishData().OrderBy(s => s.Name);
			this.Monitor = monitor;
			this.Reflection = reflectionHelper;
			this.ScrollAmount = scroll;
			this.ShowAll = showAll;

			// add scroll buttons
			this.ScrollToTopButton = new ClickableTextureComponent(Rectangle.Empty, Sprites.Icons.Sheet, Sprites.Icons.UpArrow, 1);
			this.ScrollToBottomButton = new ClickableTextureComponent(Rectangle.Empty, Sprites.Icons.Sheet, Sprites.Icons.DownArrow, 1);

			// update layout
			this.UpdateLayout();
		}

		/****
        ** Events
        ****/
		/// <summary>The method invoked when the player left-clicks on the lookup UI.</summary>
		/// <param name="x">The X-position of the cursor.</param>
		/// <param name="y">The Y-position of the cursor.</param>
		/// <param name="playSound">Whether to enable sound.</param>
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			this.HandleLeftClick(x, y);
		}

		/// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
		/// <param name="x">The X-position of the cursor.</param>
		/// <param name="y">The Y-position of the cursor.</param>
		/// <param name="playSound">Whether to enable sound.</param>
		public override void receiveRightClick(int x, int y, bool playSound = true) { }

		/// <summary>The method invoked when the player scrolls the mouse wheel on the lookup UI.</summary>
		/// <param name="direction">The scroll direction.</param>
		public override void receiveScrollWheelAction(int direction)
		{
			if (direction > 0)    // positive number scrolls content up
				this.ScrollUp();
			else
				this.ScrollDown();
		}

		/// <summary>The method called when the game window changes size.</summary>
		/// <param name="oldBounds">The former viewport.</param>
		/// <param name="newBounds">The new viewport.</param>
		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			this.UpdateLayout();
		}

		/// <summary>The method called when the player presses a controller button.</summary>
		/// <param name="button">The controller button pressed.</param>
		public override void receiveGamePadButton(Buttons button)
		{
			switch (button)
			{
				// left click
				case Buttons.A:
					Point p = Game1.getMousePosition();
					this.HandleLeftClick(p.X, p.Y);
					break;

				// exit
				case Buttons.B:
					this.exitThisMenu();
					break;

				// scroll up
				case Buttons.RightThumbstickUp:
					this.ScrollUp();
					break;

				// scroll down
				case Buttons.RightThumbstickDown:
					this.ScrollDown();
					break;
			}
		}

		/****
        ** Methods
        ****/
		/// <summary>Scroll up the menu content by the specified amount (if possible).</summary>
		public void ScrollUp()
		{
			this.CurrentScroll -= this.ScrollAmount;
		}

		/// <summary>Scroll down the menu content by the specified amount (if possible).</summary>
		public void ScrollDown()
		{
			this.CurrentScroll += this.ScrollAmount;
		}

		/// <summary>Handle a left-click from the player's mouse or controller.</summary>
		/// <param name="x">The x-position of the cursor.</param>
		/// <param name="y">The y-position of the cursor.</param>
		public void HandleLeftClick(int x, int y)
		{
			// close menu when clicked outside
			if (!this.isWithinBounds(x, y))
				this.exitThisMenu();

			// scroll up or down
			else if (this.ScrollToTopButton.containsPoint(x, y))
				this.ScrollToTop();
			else if (this.ScrollToBottomButton.containsPoint(x, y))
				this.ScrollToBottom();
		}

		private void ScrollToBottom()
		{
			this.CurrentScroll = int.MaxValue;
		}

		private void ScrollToTop()
		{
			this.CurrentScroll = 0;
		}

		public override void performHoverAction(int x, int y)
		{
			this.hoverText = "";
			foreach (ClickableTextureComponent textureComponent in this.ClickableFishTextures)
			{
				if (textureComponent.containsPoint(x, y) && this.CurrentScroll == 0)
				{
					textureComponent.scale = Math.Min(textureComponent.scale + 0.02f, textureComponent.baseScale + 0.1f);
					this.hoverText = CreateHoverText(textureComponent.name);
				}
				else
					textureComponent.scale = Math.Max(textureComponent.scale - 0.02f, textureComponent.baseScale);
			}
		}

		/// <summary>Render the UI.</summary>
		/// <param name="spriteBatch">The sprite batch being drawn.</param>
		public override void draw(SpriteBatch spriteBatch)
		{
			this.Monitor.InterceptErrors("drawing the lookup info", () =>
			{
				// calculate dimensions
				int x = this.xPositionOnScreen;
				int y = this.yPositionOnScreen;
				const int gutter = 15;
				float leftOffset = gutter;
				float topOffset = gutter;
				float contentWidth = this.width - gutter * 2;
				float contentHeight = this.height - gutter * 2;
				int tableBorderWidth = 1;

				// get font
				SpriteFont font = Game1.smallFont;
				float lineHeight = font.MeasureString("ABC").Y;
				float spaceWidth = DrawHelper.GetSpaceWidth(font);

				// draw background
				// (This uses a separate sprite batch because it needs to be drawn before the
				// foreground batch, and we can't use the foreground batch because the background is
				// outside the clipping area.)
				using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
				{
					backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
					backgroundBatch.DrawSprite(Sprites.Letter.Sheet, Sprites.Letter.Sprite, x, y, scale: this.width / (float)Sprites.Letter.Sprite.Width);
					backgroundBatch.End();
				}

				// draw foreground
				// (This uses a separate sprite batch to set a clipping area for scrolling.)
				using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
				{
					GraphicsDevice device = Game1.graphics.GraphicsDevice;
					Rectangle prevScissorRectangle = device.ScissorRectangle;
					try
					{
						// begin draw
						device.ScissorRectangle = new Rectangle(x + gutter, y + gutter, (int)contentWidth, (int)contentHeight);
						contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

						// scroll view
						this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
						this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
						topOffset -= this.CurrentScroll; // scrolled down == move text up

						leftOffset += 36;
						float wrapWidth = this.width - leftOffset - gutter;

						float caughtTextSize = 0;
						topOffset += lineHeight;
						{
							Vector2 caughtLabelSize = contentBatch.DrawTextBlock(font, $"Caught : ", new Vector2(x + leftOffset, y + topOffset), wrapWidth);
							Vector2 caughtValueSize = contentBatch.DrawTextBlock(font, $"{this.Fishes.Count(fish => fish.Caught)}/{this.Fishes.Count()}", new Vector2(x + leftOffset + caughtLabelSize.X, y + topOffset), wrapWidth, bold: Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh);
							caughtTextSize = caughtLabelSize.X + caughtValueSize.X;
						}

						{
							int caught = 0, total = 0;

							foreach(var fish in this.Fishes)
							{
								if ((fish.Id == 159 || fish.Id == 160 || fish.Id == 163 || fish.Id == 775 || fish.Id == 682))
								{
									if (fish.Caught)
										caught++;
									total++;
								}
							}

							Vector2 legendariesCaughtLabelSize = contentBatch.DrawTextBlock(font, $"Legendaries : ", new Vector2(x + leftOffset, y + topOffset + lineHeight), wrapWidth);
							Vector2 legendariesCaughtValueSize = contentBatch.DrawTextBlock(font, $"{caught}/{total}", new Vector2(x + leftOffset + legendariesCaughtLabelSize.X, y + topOffset + lineHeight), wrapWidth, bold: Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh);
							caughtTextSize = legendariesCaughtLabelSize.X + legendariesCaughtValueSize.X;
							topOffset += lineHeight;
						}

						{
							Vector2 catchableLabelSize = contentBatch.DrawTextBlock(font, $"Catchable now : ", new Vector2(x + leftOffset + caughtTextSize + leftOffset, y + topOffset - lineHeight), wrapWidth);

							float rowWidth = wrapWidth - catchableLabelSize.X - caughtTextSize - leftOffset;
							int fishPerRow = (int) (rowWidth / ((Game1.tileSize / 2) + 8));

							float innerOffset = (Game1.tileSize / 2 ) + 8;
							float innerTopOffset = 0;

							int count = 0;

							foreach (var fish in GetFishesCurrentlyCatchable())
							{
								int xPos = (int) (x + leftOffset + caughtTextSize + leftOffset + catchableLabelSize.X + (innerOffset * count));
								int yPos = (int) (y + topOffset - lineHeight + innerTopOffset);
								ClickableTextureComponent textureComponent = new ClickableTextureComponent(fish.Name + "*" + fish.GetTod() + "*" + fish.GetLocation(), new Rectangle(xPos, yPos, Game1.tileSize/2, Game1.tileSize/2), (string)null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fish.Id, 16, 16), 2f, true);
								this.ClickableFishTextures.Add(textureComponent);
								textureComponent.draw(contentBatch, Color.White, 0.86f);

								count++;
								if (count == fishPerRow)
								{
									count = 0;
									innerTopOffset += (Game1.tileSize / 2) + 8;
								}
							}
							topOffset += innerTopOffset + (lineHeight * 2);
						}

						float column1RowHeight = 0, column2Offset = 0, rowOffset = topOffset;

						// draw fish info
						foreach (FishInfo fish in this.Fishes)
						{
							// draw sprite
							{
								Item item = new SObject(fish.Id, 1);
								item.drawInMenu(contentBatch, new Vector2(x + leftOffset + column2Offset, y + topOffset), 1f, 1f, 1f, StackDrawType.Hide, this.ShowAll || fish.Caught ? Color.White : Color.Black * 0.2f, false);
								topOffset += Game1.tileSize / 2 + spaceWidth;
							}

							// draw name
							{
								Vector2 nameSize = contentBatch.DrawTextBlock(font, $"{(this.ShowAll || fish.Caught ? fish.Name : "???")}", new Vector2(x + leftOffset + column2Offset + Game1.tileSize + spaceWidth, y + topOffset), wrapWidth, bold: Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh);

								// draw caught status for legendary fishes
								{
									if ((fish.Caught && (fish.Id == 159 || fish.Id == 160 || fish.Id == 163 || fish.Id == 682 || fish.Id == 775)))
									{
										contentBatch.DrawTextBlock(font, "(X)", new Vector2(x + leftOffset + column2Offset + Game1.tileSize + spaceWidth + nameSize.X + spaceWidth, y + topOffset), wrapWidth, bold: Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.zh);
									}
								}

								topOffset += Game1.tileSize / 2 + spaceWidth;
							}

							// draw table
							foreach (string key in fish.Data.Keys)
							{
								float cellPadding = 3;
								float labelWidth = fish.Data.Keys.Max(p => font.MeasureString(p).X);
								float valueWidth = wrapWidth / 2.2F - labelWidth - cellPadding * 4 - tableBorderWidth;

								// draw label & value
								Vector2 labelSize = contentBatch.DrawTextBlock(font, key, new Vector2(x + leftOffset + cellPadding + column2Offset, y + topOffset + cellPadding), wrapWidth);
								Vector2 valuePosition = new Vector2(x + leftOffset + labelWidth + cellPadding * 3 + column2Offset, y + topOffset + cellPadding);
								Vector2 valueSize = contentBatch.DrawTextBlock(font, this.ShowAll || fish.Caught ? fish.Data[key] : "???", valuePosition, valueWidth);
								Vector2 rowSize = new Vector2(labelWidth + valueWidth + cellPadding * 4, Math.Max(labelSize.Y, valueSize.Y));

								// draw table row
								Color lineColor = Color.Gray;
								contentBatch.DrawLine(x + leftOffset + column2Offset, y + topOffset, new Vector2(rowSize.X, tableBorderWidth), lineColor); // top
								contentBatch.DrawLine(x + leftOffset + column2Offset, y + topOffset + rowSize.Y, new Vector2(rowSize.X, tableBorderWidth), lineColor); // bottom
								contentBatch.DrawLine(x + leftOffset + column2Offset, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // left
								contentBatch.DrawLine(x + leftOffset + column2Offset + labelWidth + cellPadding * 2, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // middle
								contentBatch.DrawLine(x + leftOffset + column2Offset + rowSize.X, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // right

								// update offset
								topOffset += Math.Max(labelSize.Y, valueSize.Y);
							}

							if (column2Offset == 0)
							{
								column2Offset += (wrapWidth / 2.2F - tableBorderWidth) + leftOffset / 2;
								column1RowHeight = topOffset + lineHeight;
								topOffset = rowOffset; // Reset topOffset
							}
							else
							{
								column2Offset = 0;

								// draw spacer
								topOffset += lineHeight;

								// Take max of column1 and column2 heights
								rowOffset = Math.Max(column1RowHeight, topOffset);
								// Move to next row
								topOffset = rowOffset;
							}
						}

						// update max scroll
						this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));

						// draw scroll icons
						if (this.MaxScroll > 0 && this.CurrentScroll > 0)
							this.ScrollToTopButton.draw(spriteBatch);
						if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
							this.ScrollToBottomButton.draw(spriteBatch);

						// end draw
						contentBatch.End();
					}
					finally
					{
						device.ScissorRectangle = prevScissorRectangle;
					}
				}

				// draw cursor
				this.drawMouse(Game1.spriteBatch);

				if (!this.hoverText.Equals(""))
				{
					IClickableMenu.drawHoverText(spriteBatch, this.hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
				}
			}, this.OnDrawError);
		}

		/*********
        ** Private methods
        *********/

		private IEnumerable<FishInfo> GetFishesCurrentlyCatchable()
		{
			String season = Game1.currentSeason;
			String weather = Game1.isRaining ? "rainy" : "sunny";
			int currentTime = Game1.timeOfDay;

			string[] delimiter = new string[] { " | " };

			foreach (var fish in this.Fishes)
			{
				if (fish.Caught || this.ShowAll)
				{
					// Exclude legendaries if already caught
					if (fish.Caught && (fish.Id == 159 || fish.Id == 160 || fish.Id == 163 || 
						fish.Id == 775 || fish.Id == 682))
						continue;

					// Exclude fishes that cannot be caught in the current season and/or weather
					String[] seasons = fish.GetSeason().Split(delimiter, StringSplitOptions.None);
					if (!seasons.Contains("all") && !seasons.Contains(season))
						continue;

					if (!fish.GetWeather().Contains("both") && !fish.GetWeather().Contains(weather))
						continue;

					String[] tod = fish.GetTod().Split(delimiter, StringSplitOptions.None);
					foreach (var time in tod)
					{
						String[] timeStrings = time.Split('-');
						int t1 = Int32.Parse(timeStrings[0]);
						int t2 = Int32.Parse(timeStrings[1]);

						if (currentTime >= t1 && currentTime < t2)
							yield return fish;
					}
				}
			}
		}

		private string CreateHoverText(string name)
		{
			string[] fishInfo = name.Split('*');
			string str = fishInfo[0] + Environment.NewLine + Environment.NewLine;

			str += "Time : " + fishInfo[1] + Environment.NewLine + "Location : " + fishInfo[2];
			return str;
		}

		/// <summary>Update the layout dimensions based on the current game scale.</summary>
		private void UpdateLayout()
		{
			// update size
			this.width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
			this.height = Math.Min((int)(this.AspectRatio.Y / this.AspectRatio.X * this.width), Game1.viewport.Height);

			// update position
			Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
			this.xPositionOnScreen = (int)origin.X;
			this.yPositionOnScreen = (int)origin.Y;

			// update up/down buttons
			int x = this.xPositionOnScreen;
			int y = this.yPositionOnScreen;
			int gutter = this.ScrollButtonGutter;
			float contentHeight = this.height - gutter * 2;
			this.ScrollToTopButton.bounds = new Rectangle(x + width - (gutter * 4), (int)(y + contentHeight - Sprites.Icons.UpArrow.Height - gutter - Sprites.Icons.DownArrow.Height), Sprites.Icons.UpArrow.Height, Sprites.Icons.UpArrow.Width);
			this.ScrollToBottomButton.bounds = new Rectangle(x + width - (gutter * 4), (int)(y + contentHeight - Sprites.Icons.DownArrow.Height), Sprites.Icons.DownArrow.Height, Sprites.Icons.DownArrow.Width);
		}

		/// <summary>The method invoked when an unhandled exception is intercepted.</summary>
		/// <param name="ex">The intercepted exception.</param>
		private void OnDrawError(Exception ex)
		{
			this.Monitor.InterceptErrors("handling an error in the lookup code", () => this.exitThisMenu());
		}
	}
}
