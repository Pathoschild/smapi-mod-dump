using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScryingOrb
{
	internal class DatePicker : IClickableMenu
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		
		private readonly WorldDate initialDate;
		private WorldDate date;

		private readonly string promptMessage;
		private readonly Action<WorldDate> onConfirm;

		private readonly Texture2D calendarTile;
		private readonly Texture2D dayButtonTiles;
		
		private string hoverText;
		private int hoverButton = -999;
		private int selectedDay = -1;
		private List<TemporaryAnimatedSprite> daySparkles =
			new List<TemporaryAnimatedSprite> ();
		private List<int> seasonSpriteHits = new List<int> { 0, 0, 0, 0 };
		private List<WeatherDebris> seasonDebris = new List<WeatherDebris> ();

		private ClickableComponent promptLabel;
		private ClickableComponent dateLabel;
		private List<ClickableComponent> weekLabels;
		private ClickableTextureComponent calendar;
		private List<ClickableTextureComponent> dayButtons;
		private List<ClickableTextureComponent> seasonSprites;
		private ClickableTextureComponent prevButton;
		private ClickableTextureComponent nextButton;
		private ClickableTextureComponent scryButton;
		private List<ClickableTextureComponent> otherButtons;

		private static readonly bool Mobile =
			Constants.TargetPlatform == GamePlatform.Android;

		private static readonly SpriteFont DateFont =
			Mobile ? Game1.smallFont : Game1.dialogueFont;

		private static readonly int CalendarSize = 500;
		private static readonly float CalendarScale =
			Mobile ? 0.8f : 1f;

		private static int Width =>
			CalendarSize +
			borderWidth * 2 +
			spaceToClearSideBorder * 4;

		private static int Height =>
			Game1.smallFont.LineSpacing +
			DateFont.LineSpacing +
			(int) (CalendarSize * CalendarScale) +
			Game1.tileSize +
			borderWidth * 2 +
			spaceToClearTopBorder - (Mobile ? 40 : 0) +
			spaceToClearSideBorder * (Mobile ? 3 : 6);

		private static int X => (Game1.viewport.Width - Width) / 2;

		private static int Y => (Game1.viewport.Height - Height -
			(Mobile ? 60 : 0)) / 2;

		private struct SeasonDatum
		{
			public readonly Color mainColor;
			public readonly Rectangle spriteBounds;
			public readonly string spriteAsset;
			public readonly Rectangle spriteSource;
			public readonly int spriteTextColor;

			public SeasonDatum (Color mainColor, Rectangle spriteBounds,
				string spriteAsset, Rectangle spriteSource, int spriteTextColor)
			{
				this.mainColor = mainColor;
				this.spriteBounds = spriteBounds;
				this.spriteAsset = spriteAsset;
				this.spriteSource = spriteSource;
				this.spriteTextColor = spriteTextColor;
			}
		}

		private static readonly List<SeasonDatum> SeasonData = new List<SeasonDatum>
		{
			new SeasonDatum (new Color ( 54, 179,  67), new Rectangle ( 191, -242, 48, 54), "TileSheets\\crops", new Rectangle (112, 522, 48, 54), SpriteText.color_Green),
			new SeasonDatum (new Color (143,  63, 204), new Rectangle ( 191,  194, 48, 54), "TileSheets\\crops", new Rectangle (160, 518, 48, 54), SpriteText.color_Purple),
			new SeasonDatum (new Color (212,  50,   0), new Rectangle (-239,  194, 48, 54), "TileSheets\\crops", new Rectangle (208, 518, 48, 54), SpriteText.color_Red),
			new SeasonDatum (new Color ( 12, 130, 181), new Rectangle (-239, -239, 48, 48), "Maps\\winter_town", new Rectangle (288, 384, 48, 48), SpriteText.color_Blue),
		};

		public DatePicker (WorldDate initialDate, string promptMessage,
			Action<WorldDate> onConfirm)
			: base (X, Y, Width, Height)
		{
			this.initialDate = initialDate;
			date = initialDate;

			int initialYearStart = (this.initialDate.Year - 1) * 112;
			selectedDay = this.initialDate.TotalDays - initialYearStart;

			this.promptMessage = promptMessage;
			this.onConfirm = onConfirm;

			calendarTile = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "calendar.png"));
			dayButtonTiles = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "dayButton.png"));

			arrangeInterface ();
		}
		
		public override void gameWindowSizeChanged (Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged (oldBounds, newBounds);
			xPositionOnScreen = X;
			yPositionOnScreen = Y;
			arrangeInterface ();
		}

		private void arrangeInterface ()
		{
			int xOff = xPositionOnScreen + borderWidth + spaceToClearSideBorder * 2;
			int yOff = yPositionOnScreen + borderWidth + spaceToClearTopBorder -
				(Mobile ? 40 : 0);

			promptLabel = new ClickableComponent (
				new Rectangle (xPositionOnScreen, yOff, width, Game1.smallFont.LineSpacing),
				"PromptLabel");
			yOff += Game1.smallFont.LineSpacing + spaceToClearSideBorder;

			dateLabel = new ClickableComponent (
				new Rectangle (xOff, yOff, CalendarSize, DateFont.LineSpacing),
				"DateLabel");
			yOff += DateFont.LineSpacing +
				spaceToClearSideBorder * (Mobile ? 1 : 2);

			int calendarScaled = (int) (CalendarSize * CalendarScale);
			calendar = new ClickableTextureComponent ("Calendar",
				new Rectangle (xOff + (CalendarSize - calendarScaled) / 2, yOff,
					calendarScaled, calendarScaled),
				null, null, calendarTile, new Rectangle (), CalendarScale, true);
			int xCenter = xOff + CalendarSize / 2;
			int yCenter = yOff + calendarScaled / 2;
			yOff += calendarScaled +
				spaceToClearSideBorder * (Mobile ? 1 : 2);

			weekLabels = new List<ClickableComponent> ();
			double weekRadius = 164.5 * CalendarScale;
			for (int i = 0; i < 16; ++i)
			{
				double angle = 2 * Math.PI * (i + 0.5) / 16.0;
				int x = (int) (xCenter + weekRadius * Math.Sin (angle) +
					((i % 4 == 0) ? 0.0 : 4.0) +
					(Mobile ? -4.0 : 0.0));
				int y = (int) (yCenter - weekRadius * Math.Cos (angle) +
					(Mobile ? -28.0 : -24.0));

				weekLabels.Add (new ClickableComponent (new Rectangle (x, y, 0, 0),
					$"Week{i}"));
			}

			dayButtons = new List<ClickableTextureComponent> ();
			double dayRadius = 220.0 * CalendarScale;
			for (int i = 0; i < 112; ++i)
			{
				WorldDate date = dayToWorldDate (i);

				double angle = 2 * Math.PI * (i + 0.5) / 112.0;
				int x = (int) (xCenter + dayRadius * Math.Sin (angle));
				int y = (int) (yCenter - dayRadius * Math.Cos (angle));

				dayButtons.Add (new ClickableTextureComponent ($"Day{i}",
					new Rectangle (x, y, 12, 52), null, date.Localize (),
					dayButtonTiles, new Rectangle (36 * (i / 28), 0, 12, 52),
					CalendarScale));
			}

			seasonSprites = new List<ClickableTextureComponent> ();
			for (int i = 0; i < 4; ++i)
			{
				Texture2D texture = Helper.Content.Load<Texture2D>
					(SeasonData[i].spriteAsset, ContentSource.GameContent);
				Rectangle sb = SeasonData[i].spriteBounds;
				Rectangle bounds = new Rectangle
					((int) (sb.X + xCenter), // not scaling
					(int) (sb.Y * CalendarScale + yCenter),
					(int) (sb.Width * CalendarScale),
					(int) (sb.Height * CalendarScale));
				seasonSprites.Add (new ClickableTextureComponent ($"SeasonSprite{i}",
					bounds, null, null, texture, SeasonData[i].spriteSource,
					1f)); // not scaling despite reduced bounds
			}

			xOff -= spaceToClearSideBorder;

			prevButton = new ClickableTextureComponent ("PrevButton",
				new Rectangle (xOff, yOff, Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.prevLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 44),
				1f);

			nextButton = new ClickableTextureComponent ("NextButton",
				new Rectangle (xOff + Game1.tileSize + spaceToClearSideBorder, yOff,
					Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.nextLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 33),
				1f);

			scryButton = new ClickableTextureComponent ("ScryButton",
				new Rectangle (xOff + CalendarSize + spaceToClearSideBorder * 2
					- Game1.tileSize, yOff, Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.scryLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 46),
				1f);

			otherButtons = new List<ClickableTextureComponent>
				{ prevButton, nextButton, scryButton };
		}

		public override void receiveLeftClick (int x, int y, bool playSound = true)
		{
			base.receiveLeftClick (x, y, playSound);

			int day = getDayAtPoint (x, y);
			if (day > -1)
			{
				if (playSound) Game1.playSound ("newArtifact");
				selectDay (day);
				return;
			}

			if (!Mobile)
			{
				for (int i = 0; i < seasonSprites.Count; ++i)
				{
					if (seasonSprites[i].containsPoint (x, y))
					{
						if (playSound) Game1.playSound ("leafrustle");
						hitSeasonSprite (i);
						return;
					}
				}
			}

			if (prevButton.containsPoint (x, y))
			{
				prevButton.scale = 1f;
				if (playSound) Game1.playSound ("newArtifact");
				selectPrev ();
				return;
			}

			if (nextButton.containsPoint (x, y))
			{
				nextButton.scale = 1f;
				if (playSound) Game1.playSound ("newArtifact");
				selectNext ();
				return;
			}

			if (scryButton.containsPoint (x, y))
			{
				scryButton.scale = 1f;
				if (playSound) Game1.playSound ("select");
				confirm ();
				return;
			}
		}

		public override void receiveKeyPress (Keys key)
		{
			base.receiveKeyPress (key);

			switch (key)
			{
			case Keys.Left:
				Game1.playSound ("newArtifact");
				selectPrev ();
				break;
			case Keys.Right:
				Game1.playSound ("newArtifact");
				selectNext ();
				break;
			case Keys.Enter:
				Game1.playSound ("select");
				confirm ();
				break;
			}
		}

		private void selectDay (int day)
		{
			selectedDay = day;
			date = dayToWorldDate (day);
			Rectangle bounds = new Rectangle (
				dayButtons[day].bounds.X - 30,
				dayButtons[day].bounds.Y - 10, 20, 20);
			daySparkles = Utility.sparkleWithinArea (bounds, 2,
				SeasonData[day / 28].mainColor, 50);
		}

		private WorldDate dayToWorldDate (int day)
		{
			int initialYearStart = (initialDate.Year - 1) * 112;
			int initialDay = initialDate.TotalDays - initialYearStart;
			int offset = (day < initialDay) ? 112 : 0;
			return Utilities.TotalDaysToWorldDate (initialYearStart + day + offset);
		}

		private void selectPrev ()
		{
			selectDay ((selectedDay == 0) ? 111 : selectedDay - 1);
		}

		private void selectNext ()
		{
			selectDay ((selectedDay == 111) ? 0 : selectedDay + 1);
		}

		private void confirm ()
		{
			Game1.exitActiveMenu ();
			onConfirm (date);
		}

		private void hitSeasonSprite (int seasonIndex)
		{
			if (++seasonSpriteHits[seasonIndex] < 4)
				return;
			seasonSpriteHits[seasonIndex] = 0;

			Random rng = new Random ();
			Rectangle spriteBounds = seasonSprites[seasonIndex].bounds;

			seasonDebris.Clear ();
			int debrisCount = ((seasonIndex == 3) ? 3 : 1) * (5 + rng.Next (0, 5));
			for (int i = 0; i < debrisCount; ++i)
			{
				Vector2 position = new Vector2 (spriteBounds.X + rng.Next (0, 48),
					spriteBounds.Y + rng.Next (0, 48));
				seasonDebris.Add (new WeatherDebris (position, seasonIndex,
					rng.Next (15) / 500f,
					rng.Next (-10, 10) / 10f + ((seasonIndex > 1) ? 1f : -1f),
					rng.Next (-10, 10) / 10f + ((seasonIndex % 3 == 0) ? 0.75f : -2f)));
			}
		}

		public override void performHoverAction (int x, int y)
		{
			base.performHoverAction (x, y);
			if (Mobile)
				return;

			hoverText = null;
			int oldHoverButton = hoverButton;
			hoverButton = -999;

			int day = getDayAtPoint (x, y);
			if (day > -1)
			{
				hoverButton = day;
				if (oldHoverButton != hoverButton)
					Game1.playSound ("Cowboy_gunshot");
				hoverText = dayButtons[day].hoverText;
			}

			for (int i = 0; i < otherButtons.Count; ++i)
			{
				ClickableTextureComponent button = otherButtons[i];
				if (button.containsPoint (x, y))
				{
					hoverButton = -1 - i;
					if (oldHoverButton != hoverButton)
						Game1.playSound ("Cowboy_Footstep");
					hoverText = button.hoverText;
				}
				button.scale = button.containsPoint (x, y)
					? Math.Min (button.scale + 0.02f, button.baseScale + 0.1f)
					: Math.Max (button.scale - 0.02f, button.baseScale);
			}
		}

		private int getDayAtPoint (int x, int y)
		{
			x -= calendar.bounds.Center.X;
			y -= calendar.bounds.Center.Y;
			double radius = Math.Sqrt (Math.Pow (x, 2) + Math.Pow (y, 2));
			if (radius < 192.0 * CalendarScale || radius > 248.0 * CalendarScale)
				return -1;
			double pct = Math.Atan2 (y, x) / (2 * Math.PI);
			return (int) Math.Floor (140.0 + 112.0 * pct) % 112;
		}

		public override void update (GameTime time)
		{
			for (int i = daySparkles.Count - 1; i >= 0; --i)
			{
				if (daySparkles[i].update (time))
					daySparkles.RemoveAt (i);
			}

			foreach (WeatherDebris debris in seasonDebris)
				debris.update ();
		}

		public override void draw (SpriteBatch b)
		{
			// dialog background
			Game1.drawDialogueBox (xPositionOnScreen, yPositionOnScreen, width,
				height, false, true);

			// PromptLabel
			float promptWidth = Game1.smallFont.MeasureString (promptMessage).X;
			float promptOffset = (promptLabel.bounds.Width - promptWidth) / 2;
			Utility.drawTextWithShadow (b, promptMessage, Game1.smallFont,
				new Vector2 (promptLabel.bounds.X + promptOffset, promptLabel.bounds.Y),
				Game1.textColor);

			// DateLabel
			string dateText = date.Localize ();
			float dateWidth = DateFont.MeasureString (dateText).X;
			float dateOffset = (dateLabel.bounds.Width - dateWidth) / 2;
			Utility.drawTextWithShadow (b, dateText, DateFont,
				new Vector2 (dateLabel.bounds.X + dateOffset, dateLabel.bounds.Y),
				Game1.textColor);

			// Calendar
			calendar.draw (b);

			// WeekLabels
			for (int i = 0; i < weekLabels.Count; ++i)
			{
				ClickableComponent label = weekLabels[i];
				string text = (i % 4 + 1).ToString ();
				SpriteText.drawStringHorizontallyCenteredAt (b, text,
					label.bounds.X, label.bounds.Y, junimoText: true);
			}

			// DayButtons
			for (int i = 0; i < dayButtons.Count; ++i)
			{
				ClickableTextureComponent button = dayButtons[i];
				Vector2 position = new Vector2 (button.bounds.X, button.bounds.Y);
				Rectangle sourceRect = new Rectangle (button.sourceRect.X +
					((i == selectedDay) ? 24 : (i == hoverButton) ? 12 : 0),
					button.sourceRect.Y, button.sourceRect.Width,
					button.sourceRect.Height);
				Vector2 origin = new Vector2 (button.sourceRect.Width / 2,
					button.sourceRect.Height / 2);
				double angle = 2 * Math.PI * (i + 0.5) / 112.0;
				b.Draw (button.texture, position, sourceRect, Color.White,
					(float) angle, origin, CalendarScale, SpriteEffects.None,
					0.86f + (float) button.bounds.Y / 20000f);
			}

			// SeasonSprites
			foreach (ClickableTextureComponent sprite in seasonSprites)
				sprite.draw (b);

			// PrevButton, NextButton, ScryButton
			foreach (ClickableTextureComponent button in otherButtons)
				button.draw (b);

			// SeasonDebris
			foreach (WeatherDebris debris in seasonDebris)
				debris.draw (b);

			// DaySparkles
			foreach (TemporaryAnimatedSprite sparkle in daySparkles)
				sparkle.draw (b, true);

			// hover text
			if (hoverText != null)
				drawHoverText (b, hoverText, Game1.smallFont);

			// mouse cursor
			if (!Game1.options.hardwareCursor)
				drawMouse (b);
		}
	}
}