using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevator
{
	class ElevatorMenu : IClickableMenu
	{
		private readonly int numberOfPlayersPerColumn;
		private readonly int numberOfColumns;
		private int currentIndex = 0;

		readonly List<Farmer> players = new List<Farmer>();
		readonly List<ElevatorButton> elevatorButtons = new List<ElevatorButton>();

		//UI stuff
		public int w = Game1.viewport.Width - 200;
		public int h = Game1.viewport.Height - 140;
		private readonly Rectangle textureSegment = new Rectangle(384, 373, 18, 18);

		private ClickableTextureComponent forwardButton;
		private bool ForwardButtonActive => currentIndex + numberOfColumns * numberOfPlayersPerColumn < players.Count;

		private ClickableTextureComponent backwardButton;
		private bool BackwardButtonActive => currentIndex != 0;

		public ElevatorMenu() : base(1, 1, Game1.viewport.Width - 200, Game1.viewport.Height - 140, true)
		{
			numberOfPlayersPerColumn = Game1.viewport.Height / 100;
			numberOfColumns = Game1.viewport.Width / 230;
			
			UpdatePosition();

			//If you want the elevator menu to show all cabins (not just the ones off the map), use this instead: players.AddRange(Game1.getAllFarmhands().Where(x => x.Name.Length > 0));
			players.AddRange(Game1.getAllFarmhands().Where(x => x.Name.Length > 0 && CabinHelper.FindCabinOutside(x)?.tileX.Value <= -10000));
		
			players = players.OrderBy(x => x != Game1.player).ThenBy(x => x.Name).ToList();//Sort alphabetically, and put the local player first

			if (players.Contains(Game1.player)) players.Insert(0, null); // Mailbox button

			UpdateButtonList();

			forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);
			backwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false);
		}

		public override void draw(SpriteBatch spriteBatch)
		{
			UpdatePosition();

			drawTextureBox(spriteBatch, Game1.mouseCursors, textureSegment,
				base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f, false);


			foreach (var button in elevatorButtons)
				button.Draw(spriteBatch);
			
			if (ForwardButtonActive)
				forwardButton.draw(spriteBatch);
			if (BackwardButtonActive)
				backwardButton.draw(spriteBatch);
			
			base.draw(spriteBatch);//Only draws the upper right close button
			drawMouse(spriteBatch);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			foreach (var button in elevatorButtons)
				if (button.containsPoint(x, y))
					button.OnClicked();

			if (ForwardButtonActive && forwardButton.containsPoint(x, y))
			{
				currentIndex += numberOfColumns * numberOfPlayersPerColumn;
				UpdateButtonList();
			}
			else if (BackwardButtonActive && backwardButton.containsPoint(x, y))
			{
				currentIndex -= numberOfColumns * numberOfPlayersPerColumn;
				UpdateButtonList();
			}

			base.receiveLeftClick(x, y, playSound);
		}

		public override void performHoverAction(int x, int y)
		{
			foreach (var button in elevatorButtons)
				button.TryHover(x, y);

			forwardButton.tryHover(x, y, 0.2f);
			backwardButton.tryHover(x, y, 0.2f);

			base.performHoverAction(x, y);
		}

		//also make it run when screen resizes
		private void UpdateButtonList()
		{
			elevatorButtons.Clear();

			var playersToDraw = new List<Farmer>();
			if (currentIndex < players.Count)
			{
				int count = Math.Min(players.Count - currentIndex, numberOfPlayersPerColumn * numberOfColumns);
				playersToDraw = players.GetRange(currentIndex, count);
			}

			int startY = yPositionOnScreen + 35;
			int startX = xPositionOnScreen + 30;
			int x = startX;
			int y = startY;
			int i = 0;
			foreach (Farmer player in playersToDraw)
			{
				ElevatorButton button = new ElevatorButton(x, y, player, player == null)
				{
					myID = 1000 + i//Unused
				};
				
				elevatorButtons.Add(button);
				y += 75;

				i++;
				if (i == numberOfColumns * numberOfPlayersPerColumn)
					break;

				if (i % numberOfPlayersPerColumn == 0)
				{
					x += 200;
					y = startY;
				}
			}
		}

		private void UpdatePosition()
		{
			xPositionOnScreen = Game1.viewport.Width / 2 - w / 2;
			yPositionOnScreen = Game1.viewport.Height / 2 - h / 2;
			
			//Utility.makeSafe(ref position, 300, 284) ???
		}
	}
}
