using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.Huds
{
    public class MineIcons
    {

        private static Texture2D _iconPickaxe;
        private static Texture2D _iconMonster;
        private static Texture2D _iconLadder;

        public static void Init(IModHelper helper)
        {
            _iconPickaxe = helper.Content.Load<Texture2D>("icon_pickaxe.png");
            _iconMonster = helper.Content.Load<Texture2D>("icon_monster.png");
            _iconLadder = helper.Content.Load<Texture2D>("icon_ladder.png");
        }

        public void Draw(string stoneStr, string monsterStr, string ladderStr)
        {
            SpriteBatch batch = Game1.spriteBatch;

            bool redrawCursor = false;
            string displayText = "";

            Point mousePos = new Point(Game1.getMouseX(), Game1.getMouseY());

            int y = 96;
            int x = 16;
            {
                IClickableMenu.drawTextureBox(batch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - 16, y - 16, 40 + 32, 40 + 32, Color.White);
                batch.Draw(_iconPickaxe, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    displayText = stoneStr;
                    redrawCursor = true;
                }

                x += 80;
            }
            if (monsterStr != null)
            {
                IClickableMenu.drawTextureBox(batch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - 16, y - 16, 40 + 32, 40 + 32, Color.White);
                batch.Draw(_iconMonster, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    displayText = monsterStr;
                    redrawCursor = true;
                }

                x += 80;
            }
            if (ladderStr != null)
            {
                IClickableMenu.drawTextureBox(batch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x-16, y-16, 40 + 32, 40 + 32, Color.White);
                batch.Draw(_iconLadder, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    displayText = ladderStr;
                    redrawCursor = true;
                }
            }

            if (displayText != "")
            {
                Util.DrawSimpleTextbox(batch, displayText, Game1.dialogueFont, this, true, isTips: true);
            }

            if(redrawCursor)
                DrawCursor(batch);
        }

        public static void DrawCursor(SpriteBatch batch)
        {
            if (!Game1.options.hardwareCursor && !Game1.options.gamepadControls)
            {
                batch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }
    }
}
