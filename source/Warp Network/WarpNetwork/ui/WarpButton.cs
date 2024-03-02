/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System;
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork.ui
{
	class WarpButton : ClickableComponent
	{
		private bool wasHovered = false;
		private Color tint = Color.White;
		public IWarpNetAPI.IDestinationHandler location
		{
			set
			{
				loc = value;
				updateLabel();
			}
			get => loc;
		}
		public int index = 0;
		private IWarpNetAPI.IDestinationHandler loc;
		private static readonly Rectangle bg = new(384, 396, 15, 15);
		private string text = "Unnamed";
		private Vector2 textSize;
		private Texture2D texture;
		private Farmer who;

		public WarpButton(Rectangle bounds, IWarpNetAPI.IDestinationHandler location, int index, Farmer who) : base(bounds, "")
		{
			this.location = location;
			this.index = index;
			texture = loc.Icon;
			this.who = who;
		}
		public void updateLabel()
		{
			text = TokenParser.ParseText(loc.Label, player: who);
			textSize = Game1.dialogueFont.MeasureString(text);
			texture = loc.Icon ?? WarpMenu.defaultIcon;
		}
		public void draw(SpriteBatch b)
		{
			if (loc == null)
				return;
			if (containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
			{
				tint = Color.Wheat;
				if (!wasHovered)
					Game1.playSound("shiny4");
				wasHovered = true;
			}
			else
			{
				tint = Color.White;
				wasHovered = false;
			}
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, bg, bounds.X, bounds.Y, bounds.Width, bounds.Height, tint, scale, false);
			b.Draw(texture, new Rectangle(bounds.X + 12, bounds.Y + 12, bounds.Height - 24, bounds.Height - 24), Color.White);
			Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2(bounds.X + bounds.Height - 9, MathF.Round(bounds.Y - textSize.Y / 2 + bounds.Height / 2 + 6)), Game1.textColor);
		}
	}
}
