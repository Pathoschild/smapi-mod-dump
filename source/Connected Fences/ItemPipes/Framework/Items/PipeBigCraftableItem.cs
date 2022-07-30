/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Xml.Serialization;
using StardewValley.Network;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Nodes.ObjectNodes;
namespace ItemPipes.Framework.Items
{
    public class PipeBigCraftableItem : PipeItem
    {
		public PipeBigCraftableItem() : base()
		{
			bigCraftable.Value = true;
			setOutdoors.Value = true;
			setIndoors.Value = true;

		}

		public PipeBigCraftableItem(Vector2 position) : base(position)
		{
			bigCraftable.Value = true;
			setOutdoors.Value = true;
			setIndoors.Value = true;
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			Rectangle srcRect = new Rectangle(0, 0, 16, 32);
			spriteBatch.Draw(ItemTexture, objectPosition, srcRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 3) / 10000f));
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1)
				|| drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && this.Stack != int.MaxValue;
			Rectangle srcRect = new Rectangle(0, 0, 16, 32);
			spriteBatch.Draw(ItemTexture, location + new Vector2((int)(32f * scaleSize), (int)(32f * scaleSize)), srcRect, color * transparency, 0f,
				new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);

			if (shouldDrawStackNumber)
			{
				var loc = location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f);
				Utility.drawTinyDigits(this.Stack, spriteBatch, loc, 3f * scaleSize, 1f, color);

			}
		}

		public override void drawAsProp(SpriteBatch b)
		{
			int x = (int)this.TileLocation.X;
			int y = (int)this.TileLocation.Y;

			Vector2 scaleFactor = Vector2.One;
			scaleFactor *= 4f;
			Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
			Rectangle srcRect = new Rectangle(16 * 2, 0, 16, 32);
			b.Draw(destinationRectangle: new Rectangle((int)(position.X - scaleFactor.X / 2f), (int)(position.Y - scaleFactor.Y / 2f),
				(int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f)),
				texture: ItemTexture,
				sourceRectangle: srcRect,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				effects: SpriteEffects.None,
				layerDepth: Math.Max(0f, (float)((y + 1) * 64 - 1) / 10000f));
		}

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
		{
			base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
		}

		public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
		{
			string drawSum = this.GetSpriteKey(Game1.currentLocation);
			int sourceRectPosition = DrawGuide[drawSum];
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.LocationNodes.ContainsKey(Game1.currentLocation))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				Node node = nodes.Find(n => n.Position.Equals(TileLocation));

				if (node != null && node is PipeNode)
				{
					PipeNode pipe = (PipeNode)node;
					float transparency = 1f;
					if (pipe.Passable)
					{
						Passable = true;
						transparency = 0.5f;
					}
					else
					{
						Passable = false;
						transparency = 1f;
					}
					if (pipe.ReadyForAnimation())
					{
						if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds >= pipe.StartTimeQ.Peek()
							&& Game1.currentGameTime.TotalGameTime.TotalMilliseconds < pipe.EndTimeQ.Peek())
						{
							if (pipe.ItemQ.Count > 0)
							{
								pipe.PassingItem = true;
								pipe.StoredItem = pipe.ItemQ.Peek();
								SpriteTexture = ItemMovingSprite;
							}
							else if (pipe.Connecting)
							{
								SpriteTexture = ConnectingSprite;
							}
						}
						else if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds >= pipe.EndTimeQ.Peek())
						{
							pipe.EndAnimation();
							if (pipe.PassingItem)
							{
								pipe.PassingItem = false;
							}
							else if (pipe.Connecting)
							{
								pipe.Connecting = false;
							}
							SpriteTexture = DefaultSprite;
						}
						else
						{
							SpriteTexture = DefaultSprite;
						}
					}
					else
					{
						SpriteTexture = DefaultSprite;
					}
					Rectangle srcRect = new Rectangle(
						sourceRectPosition * 16 % SpriteTexture.Bounds.Width,
						sourceRectPosition * 16 / SpriteTexture.Bounds.Width * 32, 16, 32);
					spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)),
						srcRect, Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
					if (pipe.PassingItem)
					{
						drawItem(pipe, spriteBatch, x, y, transparency);
					}
				}
			}
		}
	}
}
