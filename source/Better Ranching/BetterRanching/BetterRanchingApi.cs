/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/urbanyeti/stardew-better-ranching
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace BetterRanching
{
    public class BetterRanchingApi
	{
		private readonly ModConfig config;
		internal BetterRanchingApi(ModConfig config)
        {
			this.config = config;
        }

		public void DrawHeartBubble(SpriteBatch spriteBatch, Character character, Func<bool> displayHeart)
		{
			DrawHeartBubble(spriteBatch, character.Position.X, character.Position.Y, character.Sprite.getWidth(), displayHeart);
		}

		public void DrawHeartBubble(SpriteBatch spriteBatch, float xPosition, float yPosition, int spriteWidth, Func<bool> displayHeart)
		{
			if (config.DisplayHearts && displayHeart())
			{
				Rectangle? sourceRectangle = new Rectangle(218, 428, 7, 6);
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2)) - (Game1.tileSize * 1 / 2);

				// Thought bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(xPosition + (spriteWidth / 2),
					yPosition - (Game1.tileSize * 4 / 3) + num)),
					new Rectangle(141, 465, 20, 24),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				// Big heart icon
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xPosition + (spriteWidth / 2) + (Game1.tileSize * 1.1)),
				   yPosition - (Game1.tileSize * 1 / 10) + num)),
					sourceRectangle,
					Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom * 5 / 3, SpriteEffects.None,
					1);
			}
		}

		public void DrawItemBubble(SpriteBatch spriteBatch, FarmAnimal animal, bool ranchingInProgress)
        {
			DrawItemBubble(
				spriteBatch,
				animal.Position.X,
				animal.Position.Y,
				animal.Sprite.getWidth(),
				animal.isCoopDweller() && !animal.isBaby(),
				animal.currentProduce.Value,
				() => !ranchingInProgress && (animal.CanBeRanched(GameConstants.Tools.MilkPail) || animal.CanBeRanched(GameConstants.Tools.Shears)),
				() => !animal.wasPet.Value
			);
		}

		public void DrawItemBubble(SpriteBatch spriteBatch, float xPosition, float yPosition, int spriteWidth, bool isShortTarget, int produceIcon, Func<bool> displayItem, Func<bool> displayHeart)
		{
			bool showItem = displayItem() && config.DisplayProduce;
			bool showHeart = displayHeart() && config.DisplayHearts;

			Rectangle? sourceRectangle = new Rectangle(218, 428, 7, 6);

			if (showItem || showHeart)
			{
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2));
				if (isShortTarget) { num -= Game1.tileSize * 1 / 2; }

				// Thought bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(xPosition + (spriteWidth / 2),
					yPosition - (Game1.tileSize * 4 / 3) + num)),
					new Rectangle(141, 465, 20, 24),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				if (showHeart)
				{
					if (showItem)
					{
						// Small heart icon
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xPosition + (spriteWidth / 2) + (Game1.tileSize * .65)),
						   yPosition - (Game1.tileSize * 4 / 10) + num)),
							sourceRectangle,
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), Game1.pixelZoom, SpriteEffects.None,
							1);
					}
					else
					{
						// Big heart icon
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xPosition + (spriteWidth / 2) + (Game1.tileSize * 1.1)),
						   yPosition - (Game1.tileSize * 1 / 10) + num)),
							sourceRectangle,
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom * 5 / 3, SpriteEffects.None,
							1);
					}
				}

				if (showItem)
				{
					if (showHeart)
					{
						// Small item icon
						spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xPosition + (spriteWidth / 2) + (Game1.tileSize * .85)),
						   yPosition - (Game1.tileSize * 7 / 10) + num)),
							Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, produceIcon, 16, 16),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)(Game1.pixelZoom * .60), SpriteEffects.None,
							1);
					}
					else
					{
						// Big item icon
						spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xPosition + (spriteWidth / 2) + (Game1.tileSize * .625)),
						   yPosition - (Game1.tileSize * 7 / 10) + num)),
							Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, produceIcon, 16, 16),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), Game1.pixelZoom, SpriteEffects.None,
							1);
					}
				}
			}
		}
	}
}
