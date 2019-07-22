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
	/// Represents a menu for placing monsters in SDV.
	/// </summary>
	class MonsterPlaceMenu : IClickableMenu
	{
		private ClickableTextureComponent ok;
		private Texture2D placementTile;
		private AnimatedSprite monsterTexture;
		public Type Monster { get; set; }
		public object[] Args { get; set; }
		public Color monsterTextureColor;

		public MonsterPlaceMenu(string name, object arg, AnimatedSprite texture, Color color = default(Color))
			: base(0, 0, Game1.viewport.Width, Game1.viewport.Height) {

			//prepare arguments, we will have 2 args if we passed an additional argument
			if (arg != null) {
				Args = new object[2];
				Args[1] = arg;
			} else Args = new object[1];

			monsterTexture = texture;
			monsterTextureColor = color != default(Color) ? color : Color.White;
			monsterTexture.CurrentFrame = 0;

			//Determine type of monster to spawn
			switch (name) {
				case "Green Slime":
					Monster = typeof(GreenSlime);
					break;

				case "Bat":
				case "Frost Bat":
				case "Lava Bat":
				case "Iridium Bat":
					Monster = typeof(Bat);
					break;

				case "Bug":
				case "Armored Bug":
					Monster = typeof(Bug);
					if((int)arg == 121) Game1.addHUDMessage(new HUDMessage("Be aware that armored bugs are unkillable.",2));
					break;

				case "Duggy":
					Monster = typeof(DuggyFixed);
					monsterTexture.CurrentFrame = 5;
					Game1.addHUDMessage(new HUDMessage("Duggies can only be spawned on diggable tiles.", 2));
					break;

				case "Dust Spirit":
					Monster = typeof(DustSpirit);
					break;

				case "Fly":
					Monster = typeof(Fly);
					break;

				case "Ghost":
				case "Carbon Ghost":
					Monster = typeof(Ghost);
					break;

				case "Grub":
					Monster = typeof(Grub);
					break;

				case "Lava Crab":
					Monster = typeof(LavaCrab);
					break;

				case "Metal Head":
					Monster = typeof(MetalHead);
					Args = new object[2]; //Second argument has to be 80
					Args[1] = 80;
					break;

				case "Mummy":
					Monster = typeof(Mummy);
					break;

				case "Rock Crab":
				case "Iridium Crab":
					Monster = typeof(RockCrab);
					break;

				case "Stone Golem":
				case "Wilderness Golem":
					Monster = typeof(RockGolem);
					break;

				case "Serpent":
					Monster = typeof(Serpent);
					break;

				case "Shadow Brute":
					Monster = typeof(ShadowBrute);
					break;

				case "Shadow Shaman":
					Monster = typeof(ShadowShaman);
					break;

				case "Skeleton":
					Monster = typeof(Skeleton);
					break;

				case "Squid Kid":
					Monster = typeof(SquidKid);
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

			//spawn monster
			if (IsOkToPlace(Game1.currentCursorTile)) {
				Args[0] = WhereToPlace(); //Every monster has a position argument at the first arg
				Monster m = (Monster)Activator.CreateInstance(Monster, Args);
				m.currentLocation = Game1.currentLocation;
				if (WhereToPlace().Equals(Game1.currentCursorTile)) m.setTileLocation(new Vector2(Game1.currentCursorTile.X, Game1.currentCursorTile.Y));
				Game1.currentLocation.addCharacter(m);
				Game1.playSound("axe");
			}
			base.receiveLeftClick(x, y, playSound);
		}

		private bool IsOkToPlace(Vector2 tile) {
			if (Monster.Name.Contains("Duggy")) {
				if(Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y] != null) {
					if (Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndexProperties.ContainsKey("Diggable")) {
						return true;
					} else if (!Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndexProperties.ContainsKey("Diggable") && Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndex == 0) {
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private Vector2 WhereToPlace() {
			if (Monster.Name.Contains("Duggy") || Monster.Name.Contains("Wilderness Golem")) {
				return Game1.currentCursorTile;
			} else {
				return new Vector2(Game1.getMouseX() + Game1.viewport.X -monsterTexture.SpriteWidth, Game1.getMouseY() + Game1.viewport.Y-monsterTexture.SpriteHeight);
			}
		}

		private Vector2 WhereToDraw() {
			if (Monster.Name.Contains("Duggy") || Monster.Name.Contains("Wilderness Golem")) {
				return new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y - Game1.tileSize/2);
			} else {
				return new Vector2(Game1.getMouseX() - monsterTexture.SpriteWidth, Game1.getMouseY() - monsterTexture.SpriteHeight*2.2f);
			}
		}

		public override void draw(SpriteBatch b) {
			if (IsOkToPlace(Game1.currentCursorTile)) {
				b.Draw(this.placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White);
			} else {
				b.Draw(this.placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(64, 0, 64, 64), Color.White);
			}
			monsterTexture.draw(b, WhereToDraw(), 1, 0, 0, monsterTextureColor * 0.7f, false, 4);
			ok.draw(b);
			drawMouse(b);
		}
	}
}
