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


namespace ItemPipes.Framework
{
	public class PipeItem : SObject
	{
        public string IDName { get; set; }
		public string Description { get; set; }
		[XmlIgnore]
		public Texture2D SpriteTexture { get; set; }
		public string SpriteTexturePath { get; set; }
		[XmlIgnore]
		public Texture2D ItemTexture { get; set; }
		public string ItemTexturePath { get; set; }
		[XmlIgnore]
		public Dictionary<int, int> DrawGuide { get; set; }
		public string State { get; set; }
		public bool Passable { get; set; }
		public PipeItem()
		{
			Name = "DEFAULT NAME";
			Description = "DEFAULT DESCRIPTION";
			State = "default";
			type.Value = "Crafting";
			canBeSetDown.Value = true;
		}

		public PipeItem(Vector2 position) : this()
		{
			Name = "DEFAULT NAME";
			Description = "DEFAULT DESCRIPTION";
			State = "default";
			type.Value = "Crafting";
			TileLocation = position;
			base.boundingBox.Value = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
			Passable = false;
			canBeSetDown.Value = true;
		}

		public virtual void LoadTextures()
		{
			ItemTexturePath = $"assets/Pipes/{IDName}/{IDName}_Item.png";
			ItemTexture = ModEntry.helper.Content.Load<Texture2D>(ItemTexturePath);
			SpriteTexturePath = $"assets/Pipes/{IDName}/{IDName}_Sprite.png";
			SpriteTexture = ModEntry.helper.Content.Load<Texture2D>(SpriteTexturePath);
		}

		public override string getDescription()
		{
			return Description;
		}

		protected override int getDescriptionWidth()
		{
			int minimum_size = 272;
			if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
			{
				minimum_size = 384;
			}
			return Math.Max(minimum_size, (int)Game1.dialogueFont.MeasureString((Name == null) ? "" : Name).X);
		}

		public override string getCategoryName()
		{
			return "Item Pipes";
		}

		public override Color getCategoryColor()
		{
			return Color.Black;
		}

		protected override string loadDisplayName()
		{
			return Name;
		}

		public override bool isPassable()
		{
			return Passable;
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

		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
		{
			Vector2 placementTile = new Vector2(x / 64, y / 64);
			if (location.objects.ContainsKey(placementTile))
			{
				return false;
			}
			SObject obj = ItemFactory.CreateObject(placementTile, this);
			if (obj != null)
			{
				location.objects.Add(placementTile, obj);
				location.playSound("axe");
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool performToolAction(Tool t, GameLocation location)
		{
			if (t is Pickaxe)
			{
				var who = t.getLastFarmerToUse();
				this.performRemoveAction(this.TileLocation, location);
				Debris deb = new Debris(getOne(), who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
				Game1.currentLocation.debris.Add(deb);
				Game1.currentLocation.objects.Remove(this.TileLocation);
				return false;
			}
			return false;
		}

		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			return base.performObjectDropInAction(dropIn, probe, who);
		}

		public override Item getOne()
		{
			return ItemFactory.CreateItem(this);
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
			DataAccess DataAccess = DataAccess.GetDataAccess();
			int sourceRectPosition = 1;
			int drawSum = this.getDrawSum(Game1.currentLocation);
			sourceRectPosition = GetNewDrawGuide()[drawSum];
			SpriteTexture = Helper.GetHelper().Content.Load<Texture2D>($"assets/Pipes/{IDName}/{IDName}_Sprite.png");
			spriteBatch.Draw(SpriteTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), new Rectangle(sourceRectPosition * Fence.fencePieceWidth % SpriteTexture.Bounds.Width, sourceRectPosition * Fence.fencePieceWidth / SpriteTexture.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.001f);
		}

		public bool countsForDrawing(SObject adj)
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
			if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this))
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
			if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this))
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
			if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this))
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
			if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is PipeItem && ((PipeItem)location.objects[surroundingLocations]).countsForDrawing(this))
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
				if (node is ConnectorNode)
				{
					ConnectorNode connector = (ConnectorNode)node;
					if (connector.PassingItem)
					{
						drawSum += 5;
					}

				}
			}
			return drawSum;
		}

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
	}
}
