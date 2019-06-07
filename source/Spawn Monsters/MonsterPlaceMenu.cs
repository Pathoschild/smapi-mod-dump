using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Reflection;

namespace Spawn_Monsters
{
	/// <summary>
	/// A menu for placing monsters in SDV
	/// </summary>
	class MonsterPlaceMenu : IClickableMenu
	{
		private ClickableTextureComponent ok;

		public Type Monster { get; set; }
		public int Area { get; set; }
		public object[] Args { get; set; }
		public bool ShouldShow { get; set; } = true;
		private Texture2D placementTile;

		public MonsterPlaceMenu(string name, int arealevel = 0)
			: base(0, 0, Game1.viewport.Width, Game1.viewport.Height) {

			Assembly a = Assembly.GetAssembly(new Monster().GetType());

			switch (name) {
				case "Green Slime":
					Monster = Type.GetType("StardewValley.Monsters.GreenSlime, " + a);
					Args = new object[1];
					break;
				case "Bat":
					Monster = Type.GetType("StardewValley.Monsters.Bat, " + a);
					Args = new object[1];
					break;
				case "Bug":
					Monster = Type.GetType("StardewValley.Monsters.Bug, " + a);
					Args = new object[2];
					break;
				case "Duggy":
					Monster = new DuggyFixed(new Vector2(0,0)).GetType();
					Args = new object[1];
					break;
				case "Dust Spirit":
					Monster = Type.GetType("StardewValley.Monsters.DustSpirit, " + a);
					Args = new object[1];
					break;
				case "Fly":
					Monster = Type.GetType("StardewValley.Monsters.Fly, " + a);
					Args = new object[1];
					break;
				case "Ghost":
					Monster = Type.GetType("StardewValley.Monsters.Ghost, " + a);
					Args = new object[1];
					break;
				case "Grub":
					Monster = Type.GetType("StardewValley.Monsters.Grub, " + a);
					Args = new object[1];
					break;
				case "Metal Head":
					Monster = Type.GetType("StardewValley.Monsters.MetalHead, " + a);
					Args = new object[2];
					break;
				case "Mummy":
					Monster = Type.GetType("StardewValley.Monsters.Mummy, " + a);
					Args = new object[1];
					break;
				case "Rock Crab":
					Monster = Type.GetType("StardewValley.Monsters.RockCrab, " + a);
					Args = new object[1];
					break;
				case "Stone Golem":
					Monster = Type.GetType("StardewValley.Monsters.RockGolem, " + a);
					Args = new object[1];
					break;
				case "Serpent":
					Monster = Type.GetType("StardewValley.Monsters.Serpent, " + a);
					Args = new object[1];
					break;
				case "Shadow Brute":
					Monster = Type.GetType("StardewValley.Monsters.ShadowBrute, " + a);
					Args = new object[1];
					break;
				case "Shadow Shaman":
					Monster = Type.GetType("StardewValley.Monsters.ShadowShaman, " + a);
					Args = new object[1];
					break;
				case "Skeleton":
					Monster = Type.GetType("StardewValley.Monsters.Skeleton, " + a);
					Args = new object[1];
					break;
				case "Squid Kid":
					Monster = Type.GetType("StardewValley.Monsters.SquidKid, " + a);
					Args = new object[1];
					break;
			}
			Game1.playSound("bigSelect");
			Area = arealevel;
			ok = new ClickableTextureComponent(new Rectangle(16, 16, 60, 60), Game1.mouseCursors, new Rectangle(128, 256, 63, 63), 1f, false);
			this.placementTile = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (ok.containsPoint(x, y)) {
				Game1.exitActiveMenu();
				Game1.playSound("bigDeSelect");
				return;
			}
			//Build args
			Args[0] = new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y); //Every monster has a position argument at the first arg
			if (Args.Length == 2) {
				Args[1] = Area;
			}

			//spawn monster
			if (IsOkToPlace((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y)) {
				Monster m = (Monster)Activator.CreateInstance(Monster, Args);
				m.currentLocation = Game1.currentLocation;
				m.setTileLocation(new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y));
				Game1.currentLocation.addCharacter(m);

				ShouldShow = false;
				Game1.playSound("axe");
			}
			base.receiveLeftClick(x, y, playSound);
		}

		private bool IsOkToPlace(int tileX, int tileY) {
			if (Monster.Name.Contains("Duggy")) {
				if(Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].TileIndexProperties.ContainsKey("Diggable")) {
					return true;
				} else if(!Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].TileIndexProperties.ContainsKey("Diggable") && Game1.currentLocation.map.GetLayer("Back").Tiles[tileX, tileY].TileIndex == 0) {
					return true;
				}
				return false;
			}
			return true;
		}

		public override void draw(SpriteBatch b) {
			if (ShouldShow) {
				Game1.drawDialogueBox(0, Game1.viewport.Height - 400, 280, 300, false, true, (string)null, false, false);
				b.DrawString(Game1.dialogueFont, $"Click\nanywhere\nto spawn a\n{Monster.Name.Replace("Fixed", "")}", new Vector2(35, Game1.viewport.Height - 300), Color.Black);
			}

			if (IsOkToPlace((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y)) {
				b.Draw(this.placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White);
			} else {
				b.Draw(this.placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(64, 0, 64, 64), Color.White);
			}
			ok.draw(b);
			drawMouse(b);
		}
	}
}
