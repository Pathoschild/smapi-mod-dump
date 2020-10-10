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

namespace ServerBrowser
{
	static class DrawTools
	{
		public static void DrawBox(SpriteBatch spriteBatch, int xPos, int yPos, int boxWidth, int boxHeight)
		{
			if (xPos > 0)
			{
				spriteBatch.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle(306, 320, 16, 16), Color.White);
				spriteBatch.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 20, boxWidth, 24), new Rectangle(275, 313, 1, 6), Color.White);
				spriteBatch.Draw(Game1.mouseCursors, new Rectangle(xPos + 12, yPos + boxHeight, boxWidth - 20, 32), new Rectangle(275, 328, 1, 8), Color.White);
				spriteBatch.Draw(Game1.mouseCursors, new Rectangle(xPos - 32, yPos + 24, 32, boxHeight - 28), new Rectangle(264, 325, 8, 1), Color.White);
				spriteBatch.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 28, boxHeight), new Rectangle(293, 324, 7, 1), Color.White);
				spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 44), (float)(yPos - 28)), new Rectangle(261, 311, 14, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - 8), (float)(yPos - 28)), new Rectangle(291, 311, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - 8), (float)(yPos + boxHeight - 8)), new Rectangle(291, 326, 12, 12), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 44), (float)(yPos + boxHeight - 4)), new Rectangle(261, 327, 14, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			}
		}
	}
}
