/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spawn_Monsters.Monsters;
using Spawn_Monsters.MonsterSpawning;
using StardewValley;
using StardewValley.Menus;

namespace Spawn_Monsters
{
    /// <summary>
    /// Represents a menu for placing monsters in SDV.
    /// </summary>
    internal class MonsterPlaceMenu : IClickableMenu
    {
        private readonly ClickableTextureComponent ok;
        private readonly Texture2D placementTile;
        private readonly MonsterData.Monster monster;
        private readonly MonsterData monsterData;

        private readonly AnimatedSprite monsterTexture;

        public MonsterPlaceMenu(MonsterData.Monster monster, AnimatedSprite texture)
            : base(0, 0, Game1.viewport.Width, Game1.viewport.Height) {

            if (monster != MonsterData.Monster.CursedDoll) {
                monsterTexture = texture;
                monsterTexture.CurrentFrame = 0;
            }

            this.monster = monster;
            monsterData = MonsterData.GetMonsterData(monster);
            ok = new ClickableTextureComponent(new Rectangle(16, 16, 60, 60), Game1.mouseCursors, new Rectangle(128, 256, 63, 63), 1f, false);
            placementTile = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            //Warn users
            if (monster == MonsterData.Monster.ArmoredBug) {
                Game1.addHUDMessage(new HUDMessage("Be aware that armored bugs are unkillable.", 2));
            } else if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.MagmaDuggy) {
                monsterTexture.CurrentFrame = 5;
                Game1.addHUDMessage(new HUDMessage("Duggies can only be spawned on diggable tiles.", 2));
            }

            Game1.playSound("bigSelect");
            Game1.addHUDMessage(new HUDMessage($"Click anywhere to spawn a {monsterData.Displayname}", null));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (ok.containsPoint(x, y)) {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
                return;
            }
            if (Spawner.GetInstance().SpawnMonster(monster, WhereToPlace())) {
                Game1.playSound("axe");
            }
            base.receiveLeftClick(x, y, playSound);
        }


        private Vector2 WhereToPlace() {
            if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem || monster == MonsterData.Monster.MagmaDuggy) {
                return Game1.currentCursorTile;
            } else {
                return new Vector2(Game1.getMouseX() + Game1.viewport.X - monsterData.Texturewidth, Game1.getMouseY() + Game1.viewport.Y - monsterData.Textureheight);
            }
        }


        private Vector2 WhereToDraw() {
            if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem || monster == MonsterData.Monster.MagmaDuggy) {
                return new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y - Game1.tileSize / 2);
            } else {
                return new Vector2(Game1.getMouseX() - monsterData.Texturewidth, Game1.getMouseY() - monsterData.Textureheight * 2.2f);
            }
        }

        public override void draw(SpriteBatch b) {
            if (Spawner.IsOkToPlace(monster, Game1.currentCursorTile)) {
                b.Draw(placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(0, 0, 64, 64), Color.White);
            } else {
                b.Draw(placementTile, new Vector2((Game1.currentCursorTile.X * Game1.tileSize) - Game1.viewport.X, (Game1.currentCursorTile.Y * Game1.tileSize) - Game1.viewport.Y), new Rectangle(64, 0, 64, 64), Color.White);
            }
            if (monster == MonsterData.Monster.CursedDoll) {
                Vector2 vector2 = WhereToDraw();
                b.Draw(Game1.objectSpriteSheet, new Rectangle((int)vector2.X, (int)vector2.Y, 16 * 4, 16 * 4), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 103, 16, 16)), new Color(255, 50, 50));
            } else {
                monsterTexture.draw(b, WhereToDraw(), 1, 0, 0, (monsterData.TextureColor == default ? Color.White : monsterData.TextureColor) * 0.7f, false, 4);
            }

            ok.draw(b);
            drawMouse(b);
        }
    }
}
