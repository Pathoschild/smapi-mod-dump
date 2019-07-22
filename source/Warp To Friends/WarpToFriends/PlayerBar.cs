using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Linq;

namespace WarpToFriends
{
	class PlayerBar
	{
		public Farmer farmer;
		public long PlayerID { get; set; }
		public ClickableComponent icon;
		public ClickableComponent warpButton;
		public ClickableComponent section;
		public bool online;

		public PlayerBar(Farmer f)
		{
			farmer = f;
			checkOnline();
		}

		private void checkOnline()
		{
			online = Game1.getOnlineFarmers().Any(o => o == farmer);
		}

		public void draw(SpriteBatch b)
		{
			checkOnline();

			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15),
				section.bounds.X, section.bounds.Y, section.bounds.Width, section.bounds.Height, Color.White, 4f, false);

			farmer.FarmerRenderer.drawMiniPortrat(b, new Vector2(icon.bounds.X, icon.bounds.Y), 0.5f, 5, 1, farmer);

			string nameT = $"Player:\n{farmer.Name}";
			string locaT = "Location:\n" + ((online) ? farmer.currentLocation.Name : "Offline");

			Utility.drawTextWithShadow(b, nameT, Game1.smallFont, new Vector2(section.bounds.X + 120, section.bounds.Y + 22), Game1.textColor);
			Utility.drawTextWithShadow(b, locaT, Game1.smallFont, new Vector2(section.bounds.X + 300, section.bounds.Y + 22), Game1.textColor);

			if (Game1.player == farmer) return;
			Color color = (warpButton.containsPoint(Game1.getMouseX(), Game1.getMouseY())) ? Color.Wheat : Color.White;
			UtilityPlus.drawButtonWithText(b, warpButton.bounds, (online)? color : Color.Gray, "Warp", Game1.smallFont, Game1.textColor);

		}

	}
}
