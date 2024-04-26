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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Threading;
using xTile.Tiles;

namespace Spawn_Monsters
{
    /// <summary>
    /// Represents a menu for placing monsters in SDV.
    /// </summary>
    internal class MonsterPlaceMenu : IClickableMenu
    {
        private readonly IModHelper modHelper;

        private readonly ClickableTextureComponent ok;
        private readonly Texture2D placementTile;
        private readonly MonsterData.Monster monster;
        private readonly MonsterData monsterData;

        private readonly AnimatedSprite monsterTexture;

        public MonsterPlaceMenu(IModHelper modHelper, MonsterData.Monster monster, AnimatedSprite texture)
            : base(0, 0, Game1.viewport.Width, Game1.viewport.Height) {

            this.modHelper = modHelper;

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
            Game1.addHUDMessage(new HUDMessage($"Click anywhere to spawn a {monsterData.Displayname}"));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (ok.containsPoint(x, y)) {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
                return;
            }

            var cursorPosition = modHelper.Input.GetCursorPosition();
            Vector2 spawningLocation = cursorPosition.AbsolutePixels;

            if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem || monster == MonsterData.Monster.MagmaDuggy) {
                spawningLocation = cursorPosition.Tile;
            }

            if (Spawner.GetInstance().SpawnMonster(monster, spawningLocation)) {
                Game1.playSound("axe");
            }
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b) {

            var mousePosition = Utility.PointToVector2(Game1.getMousePosition());

            if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem || monster == MonsterData.Monster.MagmaDuggy) {
                mousePosition = Utility.clampToTile(mousePosition);
            }

            var canPlaceHere = Spawner.IsOkToPlace(monster, Game1.currentCursorTile);


            //var placementTileSourceRect =  ? new Rectangle(0, 0, 64, 64) : new Rectangle(64, 0, 64, 64);

            if(monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem || monster == MonsterData.Monster.MagmaDuggy) {
                b.Draw(Game1.mouseCursors, mousePosition, new Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
            }

            //b.Draw(placementTile, mousePosition, placementTileSourceRect, Color.White);
            if (monster == MonsterData.Monster.CursedDoll) {
                b.Draw(Game1.objectSpriteSheet, new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 16 * 4, 16 * 4), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 103, 16, 16)), new Color(255, 50, 50));
            } else {
                monsterTexture.draw(b, mousePosition + new Vector2(monsterData.Texturewidth / 16f, -monsterData.Textureheight * 2), 1, 0, 0, (monsterData.TextureColor == default ? Color.White : monsterData.TextureColor) * 0.7f, false, 4);
            }

            ok.draw(b);
            drawMouse(b);
        }
    }
}
