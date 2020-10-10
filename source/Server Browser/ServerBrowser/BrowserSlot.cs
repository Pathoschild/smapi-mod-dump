/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ServerBrowser
{
	class BrowserSlot
	{
		Texture2D emojiTexture;

		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public const int height = 100;
		public Rectangle Bounds => new Rectangle(X, Y, Width, height);

		public string FarmName { get; set; }
		public string ServerDescription { get; set; }
		
		public int PlayersOnline { get; set; }
		public int PlayerSlots { get; set; }
		private string PlayerCountText => $"{PlayersOnline}/{PlayerSlots}";

		public string CabinCountText { get; set; }

		public bool ShowPasswordLockIcon { get; set; } = false;
		
		private int hoverX = -1;
		private int hoverY = -1;

		public Action CallBack { get; set; } = null;

		public BrowserSlot(int x, int y, int width, int playersOnline, int playerSlots, string cabinCountText, string farmName, string description)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;

			this.PlayersOnline = playersOnline;
			this.PlayerSlots = playerSlots;
			this.CabinCountText = cabinCountText;

			this.FarmName = farmName;
			this.ServerDescription = description;
			
			this.emojiTexture = emojiTexture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
		}
		
		public void Draw(SpriteBatch spriteBatch)
		{
			IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), X, Y, Width, height, Bounds.Contains(hoverX, hoverY) ? Color.Wheat : Color.White, 4f, false);
			
			spriteBatch.DrawString(Game1.smallFont, FarmName, new Vector2(X + 22, Y + 16), Color.Black);

			string trimmedServerDescription = TrimDescription(ServerDescription);
			spriteBatch.DrawString(Game1.smallFont, trimmedServerDescription, new Vector2(X + 22, Y + 50), Color.Chocolate);
			
			DrawEmoji(spriteBatch, X + Width - 170, Y + 14, 128);//0 = Smile face. 128 = gamepad
			DrawEmoji(spriteBatch, X + Width - 170, Y + 51, 107);//107 = House/Cabin

			spriteBatch.DrawString(Game1.smallFont, PlayerCountText, new Vector2(X + Width - 131, Y + 16), Color.Black);
			spriteBatch.DrawString(Game1.smallFont, CabinCountText, new Vector2(X + Width - 131, Y + 56), Color.Black);
			
			//Rusty key - for password protected servers
			if (ShowPasswordLockIcon)
				spriteBatch.Draw(Game1.mouseCursors, new Vector2(X + Width - 245, Y + 14), new Rectangle(145, 320, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);

			#region Hover
			if (Game1.smallFont.GetBounds(trimmedServerDescription, X + 22, Y + 50).Contains(hoverX, hoverY))
			{
				IClickableMenu.drawHoverText(Game1.spriteBatch, ServerDescription, Game1.smallFont, 0, 0, boldTitleText: "Server message");
			}
			else if (ShowPasswordLockIcon && new Rectangle(X + Width - 245, Y + 14, 16*4, 16*4).Contains(hoverX, hoverY))
			{
				IClickableMenu.drawHoverText(Game1.spriteBatch, $"This server is password protected", Game1.smallFont, 0, 0);

			}
			else
			{
				int largestWidth = (int)Math.Max(Game1.smallFont.MeasureString(PlayerCountText).X, Game1.smallFont.MeasureString(CabinCountText).X);
				var playerCountBounds = new Rectangle(X + Width - 170, Y + 14, largestWidth + 35, 73);

				if (playerCountBounds.Contains(hoverX, hoverY))
				{
					IClickableMenu.drawHoverText(Game1.spriteBatch, $"Players online: {PlayersOnline}\nPlayer slots: {PlayerSlots}\nEmpty cabins: {CabinCountText}", Game1.smallFont, 0, 0);
				}
			}

			hoverX = -1;
			hoverY = -1;
			#endregion
		}

		public void TryHover(int x, int y)
		{
			hoverX = x;
			hoverY = y;
		}

		public void Clicked(int x, int y)
		{
			Console.WriteLine("Slot clicked");
			CallBack?.Invoke();
		}
		
		private void DrawEmoji(SpriteBatch spriteBatch, int x, int y, int emojiIndex)
		{
			spriteBatch.Draw(emojiTexture, new Vector2(x, y), new Rectangle(emojiIndex * 9 % emojiTexture.Width, emojiIndex * 9 / emojiTexture.Width * 9, 9, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
		}

		private string TrimDescription(string description)
		{
			string trimmed = description;
			int spaceForServerDesc = Width - 22 - 230 + (ShowPasswordLockIcon ? -75 : 0);

			bool hasTrimmedAnything = false;

			if (trimmed.Contains("\n"))
			{
				int index = trimmed.IndexOf("\n");
				trimmed = trimmed.Remove(index, trimmed.Length - index);
				hasTrimmedAnything = true;
			}

			int measured = 0;
			do
			{
				measured = (int)Game1.smallFont.MeasureString(trimmed).X;
				if (measured > spaceForServerDesc)
				{
					trimmed = trimmed.Remove(trimmed.Length - 1);
					hasTrimmedAnything = true;
				}
			} while (measured > spaceForServerDesc);

			if (hasTrimmedAnything)
			{
				trimmed += "...";
			}

			return trimmed;
		}
	}
}
