/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using SObject = StardewValley.Object;

namespace MaddUtil
{
    public static class Popup
    {
		public static void Draw(SpriteBatch spriteBatch, Texture2D Sprite, int x, int y)
		{
            Rectangle srcRect = new Rectangle(0, 0, 16, 16);
            float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            spriteBatch.Draw(
                Game1.mouseCursors,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 - 8, (float)(y * 64 - 96 - 16) + yOffset)),
                new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24),
                Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.003f);
            spriteBatch.Draw(
                Sprite,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + yOffset)),
                srcRect,
                Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.004f);
        }
	}
}
