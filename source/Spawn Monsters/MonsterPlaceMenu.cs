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
		private Texture2D placementTile;
		public Type Monster { get; set; }
		public object[] Args { get; set; }

		public MonsterPlaceMenu(string name, object arg)
			: base(0, 0, Game1.viewport.Width, Game1.viewport.Height) {

			Assembly a = Assembly.GetAssembly(new Monster().GetType());

			//prepare arguments, we will have 2 args if we passed an additional argument
			if (arg != null) {
				Args = new object[2];
				Args[1] = arg;
			} else Args = new object[1];

			//Determine type of monster to spawn
			switch (name) {
				case "Green Slime":
					Monster = Type.GetType("StardewValley.Monsters.GreenSlime, " + a);
					break;

				case "Bat":
				case "Frost Bat":
				case "Lava Bat":
				case "Iridium Bat":
					Monster = Type.GetType("StardewValley.Monsters.Bat, " + a);	
					break;

				case "Bug":
				case "Armored Bug":
					Monster = Type.GetType("StardewValley.Monsters.Bug, " + a);
					if((int)arg == 121) Game1.addHUDMessage(new HUDMessage("Be aware that armored bugs are unkillable.",2));
					break;

				case "Duggy":
					Monster = new DuggyFixed(new Vector2(0,0)).GetType();
					Game1.addHUDMessage(new HUDMessage("Duggies can only be spawned on diggable tiles.", 2));
					break;

				case "Dust Spirit":
					Monster = Type.GetType("StardewValley.Monsters.DustSpirit, " + a);
					break;

				case "Fly":
					Monster = Type.GetType("StardewValley.Monsters.Fly, " + a);
					break;

				case "Ghost":
				case "Carbon Ghost":
					Monster = Type.GetType("StardewValley.Monsters.Ghost, " + a);
					break;

				case "Grub":
					Monster = Type.GetType("StardewValley.Monsters.Grub, " + a);
					break;

				case "Lava Crab":
					Monster = Type.GetType("StardewValley.Monsters.LavaCrab, " + a);
					break;

				case "Metal Head":
					Monster = Type.GetType("StardewValley.Monsters.MetalHead, " + a);
					Args = new object[2];
					break;

				case "Mummy":
					Monster = Type.GetType("StardewValley.Monsters.Mummy, " + a);
					break;

				case "Rock Crab":
				case "Iridium Crab":
					Monster = Type.GetType("StardewValley.Monsters.RockCrab, " + a);
					break;

				case "Stone Golem":
				case "Wilderness Golem":
					Monster = Type.GetType("StardewValley.Monsters.RockGolem, " + a);
					break;

				case "Serpent":
					Monster = Type.GetType("StardewValley.Monsters.Serpent, " + a);
					break;

				case "Shadow Brute":
					Monster = Type.GetType("StardewValley.Monsters.ShadowBrute, " + a);
					break;

				case "Shadow Shaman":
					Monster = Type.GetType("StardewValley.Monsters.ShadowShaman, " + a);
					break;

				case "Skeleton":
					Monster = Type.GetType("StardewValley.Monsters.Skeleton, " + a);
					break;

				case "Squid Kid":
					Monster = Type.GetType("StardewValley.Monsters.SquidKid, " + a);
					break;
			}
			

			Game1.playSound("bigSelect");
			ok = new ClickableTextureComponent(new Rectangle(16, 16, 60, 60), Game1.mouseCursors, new Rectangle(128, 256, 63, 63), 1f, false);
			placementTile = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");
			Game1.addHUDMessage(new HUDMessage($"Click anywhere to spawn a {name.Replace("Green ", "")}", null));
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (ok.containsPoint(x, y)) {
				Game1.exitActiveMenu();
				Game1.playSound("bigDeSelect");
				return;
			}

			Args[0] = new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y); //Every monster has a position argument at the first arg

			//spawn monster
			if (IsOkToPlace((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y)) {
				Monster m = (Monster)Activator.CreateInstance(Monster, Args);
				m.currentLocation = Game1.currentLocation;
				m.setTileLocation(new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y));
				Game1.currentLocation.addCharacter(m);
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
