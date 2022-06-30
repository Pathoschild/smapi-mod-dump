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


namespace ItemPipes.Framework.Items.Objects
{

	public class PIPOItem : CustomBigCraftableItem
	{
        public bool ToolCall { get; set; }
		public Texture2D OnTextureR { get; set; }
		public Texture2D OnTextureL { get; set; }
		public Texture2D OnTextureC { get; set; }
		public Texture2D OffTextureR { get; set; }
		public Texture2D OffTextureL { get; set; }
		public Texture2D OffTextureC { get; set; }

		public PIPOItem() : base()
		{
			State = "off";
			DataAccess DataAccess = DataAccess.GetDataAccess();
			OnTextureR = DataAccess.Sprites[IDName + "_onR"];
			OnTextureL = DataAccess.Sprites[IDName + "_onL"];
			OnTextureC = DataAccess.Sprites[IDName + "_onC"];
			OffTextureR = DataAccess.Sprites[IDName + "_offR"];
			OffTextureL = DataAccess.Sprites[IDName + "_offL"];
			OffTextureC = DataAccess.Sprites[IDName + "_offC"];
			ItemTexture = OffTextureC;
			ToolCall = false;
		}

		public PIPOItem(Vector2 position) : base(position)
		{
			State = "off";
			DataAccess DataAccess = DataAccess.GetDataAccess();
			OnTextureR = DataAccess.Sprites[IDName + "_onR"];
			OnTextureL = DataAccess.Sprites[IDName + "_onL"];
			OnTextureC = DataAccess.Sprites[IDName + "_onC"];
			OffTextureR = DataAccess.Sprites[IDName + "_offR"];
			OffTextureL = DataAccess.Sprites[IDName + "_offL"];
			OffTextureC = DataAccess.Sprites[IDName + "_offC"];
			ItemTexture = OffTextureC;
			ToolCall = false;
		}

		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			bool result = false;
			if (justCheckingForActivity)
			{
				result = true;
			}
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
			{
				//When right clicking using a tool
				//it calls checkforaction 2 times, dont know why.
				if(!ToolCall)
                {
					List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
					Node node = nodes.Find(n => n.Position.Equals(TileLocation));
					if (node != null && node is PIPONode)
					{
						PIPONode invis = (PIPONode)node;
						if (invis.ChangeState())
						{
							Passable = true;
						}
						else
						{
							Passable = false;
						}
						result = false;
					}
				}
				if (who.CurrentTool != null && !ToolCall)
				{
					ToolCall = true;
					result = true;
				}

			}
			return result;
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
			spriteBatch.Draw(ItemTexture, location + new Vector2((int)(32f * scaleSize), (int)(16f * scaleSize)), srcRect, color * transparency, 0f,
				new Vector2(8f, 8f) * scaleSize, 2f * scaleSize, SpriteEffects.None, layerDepth);

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
			ToolCall = false;

			DataAccess DataAccess = DataAccess.GetDataAccess();
			List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
			Node node = nodes.Find(n => n.Position.Equals(TileLocation));
			if (node != null && node is PIPONode)
			{
				PIPONode invis = (PIPONode)node;
				State = invis.State;
				float transparency = 1f;
				//Printer.Info($"PIPO: {TileLocation.X} |  Player: {Game1.player.Position.X} ");

				if (State.Equals("on"))
                {
					//Look right
					if (TileLocation.X < Game1.player.getTileLocation().X)
					{
						ItemTexture = OnTextureR;
					}
					//look left
					else if (TileLocation.X > Game1.player.getTileLocation().X)
					{
						ItemTexture = OnTextureL;
					}
					//center
					else if (TileLocation.X == Game1.player.getTileLocation().X)
					{
						ItemTexture = OnTextureC;
					}
					transparency = 0.5f;
					Rectangle srcRect = new Rectangle(0, 0, 16, 32);
					spriteBatch.Draw(ItemTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), srcRect, Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
				}
				else if(State.Equals("off"))
                {
					//Look right
					if (TileLocation.X < Game1.player.getTileLocation().X)
					{
						ItemTexture = OffTextureR;
					}
					//look left
					else if (TileLocation.X > Game1.player.getTileLocation().X)
					{
						ItemTexture = OffTextureL;
					}
					//center
					else if (TileLocation.X == Game1.player.getTileLocation().X)
					{
						ItemTexture = OffTextureC;
					}
					transparency = 1f;
					Rectangle srcRect = new Rectangle(0, 0, 16, 32);
					spriteBatch.Draw(ItemTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), srcRect, Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
				}
			}
		}

	}
}
