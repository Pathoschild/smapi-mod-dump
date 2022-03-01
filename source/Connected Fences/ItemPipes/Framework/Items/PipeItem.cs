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
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Items.Objects;



namespace ItemPipes.Framework
{
	public abstract class PipeItem : CustomObjectItem
	{
		[XmlIgnore]
		public Texture2D SpriteTexture { get; set; }
		[XmlIgnore]
		public Texture2D DefaultSprite { get; set; }
		[XmlIgnore]
		public Texture2D ConnectingSprite { get; set; }
		[XmlIgnore]
		public Texture2D ItemMovingSprite { get; set; }
		[XmlIgnore]
		public Dictionary<string, int> DrawGuide { get; set; }

        public PipeItem() : base()
		{
			DrawGuide = new Dictionary<string, int>();
			PopulateDrawGuide();
		}

		public PipeItem(Vector2 position) : base(position)
		{
			Passable = false;
			DrawGuide = new Dictionary<string, int>();
			PopulateDrawGuide();
		}

		public void Save()
        {
			modData.Add("State", State);
        }

		public virtual void Init()
		{
            try
            {
				ItemTexture = ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{IDName}/{IDName}_Item.png");
				DefaultSprite = ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{IDName}/{IDName}_default_Sprite.png");
				ConnectingSprite = ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{IDName}/{IDName}_connecting_Sprite.png");
				ItemMovingSprite = ModEntry.helper.Content.Load<Texture2D>($"assets/Pipes/{IDName}/{IDName}_item_Sprite.png");
			}
			catch(Exception e)
            {
				Printer.Info("Can't load pipe sprites!");
            }

			SpriteTexture = DefaultSprite;
		}

		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (justCheckingForActivity)
			{
				return true;
			
			}
			/*
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true) && Name.Equals("FilterPipe"))
			{
				List<Node> nodes;
				if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
				{
					FilterPipe pipe = (FilterPipe)nodes.Find(n => n.Position.Equals(__instance.TileLocation));
					pipe.Chest.ShowMenu();
				}
				return false;
			}
			else if (DataAccess.IOPipeNames.Contains(__instance.Name))
			{
				List<Node> nodes;
				if (DataAccess.LocationNodes.TryGetValue(Game1.currentLocation, out nodes))
				{
					IOPipe pipe = (IOPipe)nodes.Find(n => n.Position.Equals(__instance.TileLocation));
					//Add state display
				}
				return false;
			}
			*/
			if (!justCheckingForActivity && who != null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
			{
				this.performToolAction(null, who.currentLocation);
			}
			return true;
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			Rectangle srcRect = new Rectangle(0, 0, 16, 16);
			spriteBatch.Draw(ItemTexture, objectPosition, srcRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 3) / 10000f));
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1)
				|| drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && this.Stack != int.MaxValue;
			Rectangle srcRect = new Rectangle(0, 0, 16, 16);
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
			Rectangle srcRect = new Rectangle(16 * 2, 0, 16, 16);
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
					if (pipe.PassingItem)
					{
						SpriteTexture = ItemMovingSprite;
						spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64)),
							new Rectangle(sourceRectPosition * 16 % SpriteTexture.Bounds.Width,
							sourceRectPosition * 16 / SpriteTexture.Bounds.Width * 16,
							16, 16), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
						drawItem(pipe, spriteBatch, x, y);
					}
					else if (State == "connecting")
					{
						SpriteTexture = ConnectingSprite;
						spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64)),
							new Rectangle(sourceRectPosition * 16 % SpriteTexture.Bounds.Width,
							sourceRectPosition * 16 / SpriteTexture.Bounds.Width * 16,
							16, 16), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
					}
					else
					{
						SpriteTexture = DefaultSprite;
						spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64)),
							new Rectangle(sourceRectPosition * 16 % SpriteTexture.Bounds.Width,
							sourceRectPosition * 16 / SpriteTexture.Bounds.Width * 16,
							16, 16), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
					}
					/*
					spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)),
						new Rectangle(sourceRectPosition * Fence.fencePieceWidth % SpriteTexture.Bounds.Width,
						sourceRectPosition * Fence.fencePieceWidth / SpriteTexture.Bounds.Width * Fence.fencePieceHeight,
						Fence.fencePieceWidth, Fence.fencePieceHeight), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
					*/
				}
			}
		}

		public void drawItem(PipeNode pipe, SpriteBatch spriteBatch, int x, int y)
		{
			Item item = pipe.StoredItem;
			Texture2D SpriteSheet;
			Rectangle srcRect;
			Vector2 originalPosition;
			Vector2 position;
			//How to handle drawing custom mod items
			if (item is PipeItem)
			{
				PipeItem pipeItem = (PipeItem)item;
				SpriteSheet = pipeItem.ItemTexture;
				srcRect = new Rectangle(0, 0, 16, 16);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 16, originalPosition.Y + 64 + 16);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
			else if (item is SObject && (item as SObject).bigCraftable.Value)
			{
				SpriteSheet = Game1.bigCraftableSpriteSheet;
				srcRect = SObject.getSourceRectForBigCraftable(item.ParentSheetIndex);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 23, originalPosition.Y + 64 + 10);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
			else if (item is Tool)
			{
				Tool tool = (Tool)item;
				if (item is MeleeWeapon || item is Slingshot || item is Sword)
				{
					SpriteSheet = Tool.weaponsTexture;
					srcRect = Game1.getSquareSourceRectForNonStandardTileSheet(SpriteSheet, 16, 16, tool.IndexOfMenuItemView);
					originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
					position = new Vector2(originalPosition.X + 19, originalPosition.Y + 64 + 19);
					spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 1.7f, SpriteEffects.None,
						((float)(y * 64 + 32) / 10000f) + 0.002f);
				}
				else
				{
					SpriteSheet = Game1.toolSpriteSheet;
					srcRect = Game1.getSquareSourceRectForNonStandardTileSheet(SpriteSheet, 16, 16, tool.IndexOfMenuItemView);
					originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
					position = new Vector2(originalPosition.X + 19, originalPosition.Y + 64 + 18);
					spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 1.7f, SpriteEffects.None,
						((float)(y * 64 + 32) / 10000f) + 0.002f);
				}
			}
			//Boots = standard
			else if (item is Boots)
			{
				Boots boot = (Boots)item;
				SpriteSheet = Game1.objectSpriteSheet;
				srcRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, boot.indexInTileSheet.Value, 16, 16);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 18, originalPosition.Y + 64 + 16);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
			//rings = standard
			else if (item is Ring)
			{
				SpriteSheet = Game1.objectSpriteSheet;
				srcRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, 16, 16);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 10, originalPosition.Y + 64 + 14);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
			else if (item is Hat)
			{
				Hat hat = (Hat)item;
				SpriteSheet = FarmerRenderer.hatsTexture;
				srcRect = new Rectangle((int)hat.which * 20 % FarmerRenderer.hatsTexture.Width, (int)hat.which * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 12, originalPosition.Y + 64 + 18);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
			else if (item is Clothing)
			{
				Clothing cloth = (Clothing)item;
				Color clothes_color = cloth.clothesColor;
				if (cloth.isPrismatic.Value)
				{
					clothes_color = Utility.GetPrismaticColor();
				}
				if (cloth.clothesType.Value == 0)
				{
					SpriteSheet = FarmerRenderer.shirtsTexture;
					srcRect = new Rectangle(cloth.indexInTileSheetMale.Value * 8 % 128, cloth.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8);
					originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
					position = new Vector2(originalPosition.X + 20, originalPosition.Y + 64 + 20);
					spriteBatch.Draw(SpriteSheet, position, srcRect, clothes_color, 0f, Vector2.Zero, 3f, SpriteEffects.None,
						((float)(y * 64 + 32) / 10000f) + 0.002f);
				}
				else if (cloth.clothesType.Value == 1)
				{
					SpriteSheet = FarmerRenderer.pantsTexture;
					srcRect = new Rectangle(192 * (cloth.indexInTileSheetMale.Value % (FarmerRenderer.pantsTexture.Width / 192)), 688 * (cloth.indexInTileSheetMale.Value / (FarmerRenderer.pantsTexture.Width / 192)) + 672, 16, 16);
					originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
					position = new Vector2(originalPosition.X + 8, originalPosition.Y + 64 + 10);
					spriteBatch.Draw(SpriteSheet, position, srcRect, clothes_color, 0f, Vector2.Zero, 3f, SpriteEffects.None,
						((float)(y * 64 + 32) / 10000f) + 0.002f);
				}
			}
			else
			{
				SpriteSheet = Game1.objectSpriteSheet;
				srcRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, 16, 16);
				originalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
				position = new Vector2(originalPosition.X + 17, originalPosition.Y + 64 + 17);
				spriteBatch.Draw(SpriteSheet, position, srcRect, Color.White, 0f, Vector2.Zero, 1.9f, SpriteEffects.None,
					((float)(y * 64 + 32) / 10000f) + 0.002f);
			}
		}

		public virtual bool countsForDrawing(SObject adj)
		{
			if (adj is PipeItem)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		/*
		public int getDrawSum(GameLocation location)
		{
			DataAccess DataAccess = DataAccess.GetDataAccess();
			bool CN = false;
			bool CS = false;
			bool CW = false;
			bool CE = false;
			int drawSum = 0;
			Vector2 surroundingLocations = this.TileLocation;
			surroundingLocations.X += 1f;
			//0 = 6
			//West = 100 = 11
			if (location.objects.ContainsKey(surroundingLocations) && (location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this)
				|| location.objects[surroundingLocations] is PPMItem))
			{
				drawSum += 100;
			}
			else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
			{
				CW = true;
			}
			//East = 10 = 10
			//W + E = 110 = 8
			surroundingLocations.X -= 2f;
			if (location.objects.ContainsKey(surroundingLocations) && (location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this)
				|| location.objects[surroundingLocations] is PPMItem))
			{
				drawSum += 10;
			}
			else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
			{
				CE = true;
			}
			//South = 500 = 6
			//S + E = 600 = 1
			//S + W = 510 = 3
			//S + E + W = 610
			surroundingLocations.X += 1f;
			surroundingLocations.Y += 1f;
			if (location.objects.ContainsKey(surroundingLocations) && (location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this)
				|| location.objects[surroundingLocations] is PPMItem))
			{
				drawSum += 500;
			}
			else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
			{
				CS = true;
			}
			surroundingLocations.Y -= 2f;
			//North = 1000 = 4
			//N + E = 1100 = 7
			//N + W = 1010 = 9
			//N + S = 1500 = 4
			//N + E + W = 1110 = 8
			//N + E + S = 1600 = 1
			//N + S + W = 1510 = 3
			//N + E + W  + S = 1610 = 5
			if (location.objects.ContainsKey(surroundingLocations) && (location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this)
				|| location.objects[surroundingLocations] is PPMItem))
			{
				drawSum += 1000;
			}
			else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
			{
				CN = true;
			}
			if (DataAccess.IOPipeNames.Contains(this.Name))
			{
				if (CN || CS || CW || CE)
				{
					drawSum = GetAdjChestsSum(drawSum, CN, CS, CW, CE);
				}
			}
			if (this.Name.Equals("ConnectorPipe"))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				Node node = nodes.Find(n => n.Position.Equals(this.TileLocation));
				if (node is ConnectorPipeNode)
				{
					ConnectorPipeNode connector = (ConnectorPipeNode)node;
					if (connector.PassingItem)
					{
						drawSum += 5;
					}

				}
			}
			return drawSum;
		}
		*/
		public virtual string GetSpriteKey(GameLocation location)
		{
			string key = "";
			Vector2 position = this.TileLocation;
			position.Y -= 1f;
			if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
				|| location.objects[position] is PPMItem))
			{
				key += "N";
			}
			position = this.TileLocation;
			position.Y += 1f;
			if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
				|| location.objects[position] is PPMItem))
			{
				key += "S";
			}
			position = this.TileLocation;
			position.X += 1f;
			if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
				|| location.objects[position] is PPMItem))
			{
				key += "W";
			}
			position = this.TileLocation;
			position.X -= 1f;
			if (location.objects.ContainsKey(position) && (location.objects[position] is PipeItem && ((PipeItem)location.objects[position]).countsForDrawing(this)
				|| location.objects[position] is PPMItem))
			{
				key += "E";
			}
			return key;
		}




		/*
		private static int GetAdjChestsSum(int drawSum, bool CN, bool CS, bool CW, bool CE)
		{
			switch (drawSum)
			{
				case 0:
					if (CN) { drawSum = 500; }
					else if (CS) { drawSum = 1000; }
					else if (CW) { drawSum = 10; }
					else if (CE) { drawSum = 100; }
					break;
				case 1000:
					if (CS) { drawSum += 2; }
					else if (CW) { drawSum += 3; }
					else if (CE) { drawSum += 4; }
					break;
				case 500:
					if (CN) { drawSum += 1; }
					else if (CW) { drawSum += 3; }
					else if (CE) { drawSum += 4; }
					break;
				case 100:
					if (CN) { drawSum += 1; }
					else if (CS) { drawSum += 2; }
					else if (CE) { drawSum += 4; }
					break;
				case 10:
					if (CN) { drawSum += 1; }
					else if (CS) { drawSum += 2; }
					else if (CW) { drawSum += 3; }
					break;
				case 1500:
					if (CW) { drawSum += 3; }
					else if (CE) { drawSum += 4; }
					break;
				case 110:
					if (CN) { drawSum += 1; }
					else if (CS) { drawSum += 2; }
					break;
				case 1100:
					if (CS) { drawSum += 2; }
					else if (CE) { drawSum += 4; }
					break;
				case 1010:
					if (CS) { drawSum += 2; }
					else if (CW) { drawSum += 3; }
					break;
				case 600:
					if (CN) { drawSum += 1; }
					else if (CE) { drawSum += 4; }
					break;
				case 510:
					if (CN) { drawSum += 1; }
					else if (CW) { drawSum += 3; }
					break;
			}
			return drawSum;
		}
		*/
		/*
		public virtual Dictionary<int, int> GetNewDrawGuide()
		{
			Dictionary<int, int> DrawGuide = new Dictionary<int, int>();
			DataAccess DataAccess = DataAccess.GetDataAccess();
			DrawGuide.Add(0, 0);
			DrawGuide.Add(1000, 1);
			DrawGuide.Add(500, 2); ;
			DrawGuide.Add(100, 3);
			DrawGuide.Add(10, 4);
			DrawGuide.Add(1500, 5);
			DrawGuide.Add(1100, 6);
			DrawGuide.Add(1010, 7);
			DrawGuide.Add(600, 8);
			DrawGuide.Add(510, 9);
			DrawGuide.Add(110, 10);
			DrawGuide.Add(1600, 11);
			DrawGuide.Add(1510, 12);
			DrawGuide.Add(1110, 13);
			DrawGuide.Add(610, 14);
			DrawGuide.Add(1610, 15);
			DrawGuide.Add(5, 0);
			DrawGuide.Add(1005, 16);
			DrawGuide.Add(505, 17); ;
			DrawGuide.Add(105, 18);
			DrawGuide.Add(15, 19);
			DrawGuide.Add(1505, 20);
			DrawGuide.Add(1105, 21);
			DrawGuide.Add(1015, 22);
			DrawGuide.Add(605, 23);
			DrawGuide.Add(515, 24);
			DrawGuide.Add(115, 25);
			DrawGuide.Add(1605, 26);
			DrawGuide.Add(1515, 27);
			DrawGuide.Add(1115, 28);
			DrawGuide.Add(615, 29);
			DrawGuide.Add(1615, 30);
			return DrawGuide;
		}
		*/
		public virtual void PopulateDrawGuide()
		{
			DrawGuide.Clear();
			DrawGuide.Add("", 0);
			DrawGuide.Add("N", 1);
			DrawGuide.Add("S", 2);
			DrawGuide.Add("W", 3);
			DrawGuide.Add("E", 4);
			DrawGuide.Add("NS", 5);
			DrawGuide.Add("NW", 6);
			DrawGuide.Add("NE", 7);
			DrawGuide.Add("SW", 8);
			DrawGuide.Add("SE", 9);
			DrawGuide.Add("WE", 10);
			DrawGuide.Add("NSW", 11);
			DrawGuide.Add("NSE", 12);
			DrawGuide.Add("NWE", 13);
			DrawGuide.Add("SWE", 14);
			DrawGuide.Add("NSWE", 15);
		}
	}
}
