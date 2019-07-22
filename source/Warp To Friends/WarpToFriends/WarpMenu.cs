using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WarpToFriends.Helpers;
using System.Collections.Generic;
using StardewValley.BellsAndWhistles;

namespace WarpToFriends
{
	public class WarpMenu : IClickableMenu
	{

		private readonly List<Farmer> _farmers;

		private List<PlayerBar> _playerBars;
		private ClickableTextureComponent _optionsButton;

		public WarpMenu(int w = 700, int h = 400)
			: base(Game1.viewport.Width / 2 - w / 2, Game1.viewport.Height / 2 - h / 2, w, h, true)
		{
			_farmers = PlayerHelper.GetAllCreatedFarmers();
			base.width = w;
			base.height = h;
			
			setUpPlayerBars();

			_optionsButton = new ClickableTextureComponent("Options", new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom),
				"", "Configure mod options", Game1.mouseCursors, new Rectangle(162, 440, 16, 16), Game1.pixelZoom);
		}

		public void setUpPlayerBars()
		{

			_playerBars = new List<PlayerBar>();

			for (int i = 0; i < _farmers.Count; i++)
			{
				PlayerBar pb = new PlayerBar(_farmers[i]);

				int xPos = xPositionOnScreen;
				int yPos = yPositionOnScreen + ((height / 4) * i);

				Rectangle sectionBounds = new Rectangle(xPos + 16, yPos + 16, width - 32, height / 4);
				pb.section = new ClickableComponent(sectionBounds, _farmers[i].Name);

				int iconStartX = xPos + 30;
				int iconStartY = yPos + 20;
				pb.icon = new ClickableComponent(new Rectangle(iconStartX, iconStartY, 80, 80), _farmers[i].Name);

				Rectangle buttonBounds = new Rectangle(iconStartX + 530, iconStartY + 16, 85, 50);
				pb.warpButton = new ClickableComponent(buttonBounds, _farmers[i].Name);

				_playerBars.Add(pb);
			}

		}

		public override void draw(SpriteBatch b)
		{
			drawMenuBackground(b);
			drawMenuTitle(b);
			drawPlayerBars(b);
			drawOptionsButton(b);
			base.draw(b);
			drawMouse(b);
		}

		private void drawMenuTitle(SpriteBatch b)
		{
			SpriteText.drawStringWithScrollCenteredAt(b, "Warp to Friends", xPositionOnScreen + width / 2, yPositionOnScreen - 72); 
		}

		private void drawOptionsButton(SpriteBatch b)
		{
			_optionsButton.draw(b);
			_optionsButton.tryHover(Game1.getMouseX(), Game1.getMouseY());
			if(_optionsButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
			{
				IClickableMenu.drawToolTip(b, _optionsButton.hoverText, _optionsButton.name, null);
			}
		}

		private void drawPlayerBars(SpriteBatch b)
		{
			foreach (PlayerBar pb in _playerBars)
			{
				pb.draw(b);
			}
		}

		private void drawMenuBackground(SpriteBatch b)
		{

			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, base.height + 32, Color.White, 4f, true);

		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			foreach(PlayerBar pb in _playerBars)
			{
				if (!pb.online || pb.farmer == Game1.player) continue;
				if(pb.warpButton.containsPoint(x, y))
				{
					PlayerHelper.warpFarmerToPlayer(pb.farmer);
				}
			}
			if(_optionsButton.containsPoint(x, y))
			{
				Game1.activeClickableMenu.exitThisMenuNoSound();
				Game1.activeClickableMenu = new OptionsMenu<ModConfig>(ModEntry.Helper, 500, 400, Game1.player.UniqueMultiplayerID, ModEntry.config, this);
			}

			base.receiveLeftClick(x, y, playSound);
		}
	}
}
