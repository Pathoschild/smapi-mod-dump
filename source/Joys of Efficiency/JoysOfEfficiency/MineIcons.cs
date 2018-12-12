using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace JoysOfEfficiency
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

            Point mousePos = new Point(Game1.getMouseX(), Game1.getMouseY());

            int y = Game1.options.zoomButtons ? 350 : 320;
            int x = GetWidthInPlayArea() - 84;
            {
                batch.Draw(_iconPickaxe, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    Util.DrawSimpleTextbox(batch, stoneStr, Game1.dialogueFont, true);
                    redrawCursor = true;
                }

                x -= 48;
            }
            if (monsterStr != null)
            {
                batch.Draw(_iconMonster, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    Util.DrawSimpleTextbox(batch, monsterStr, Game1.dialogueFont, true);
                    redrawCursor = true;
                }

                x -= 48;
            }
            if (ladderStr != null)
            {
                batch.Draw(_iconLadder, new Vector2(x, y), null, Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);
                Rectangle rect = new Rectangle(x, y, 40, 40);
                if (rect.Contains(mousePos))
                {
                    Util.DrawSimpleTextbox(batch, ladderStr, Game1.dialogueFont, true);
                    redrawCursor = true;
                }
            }

            if(redrawCursor)
                DrawCursor(batch);
        }

        public static void DrawCursor(SpriteBatch batch)
        {
            if (!Game1.options.hardwareCursor)
            {
                batch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }

        public static int GetWidthInPlayArea()
        {
            int result;

            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                int right = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
                int totalWidth = Game1.currentLocation.map.Layers[0].LayerWidth * Game1.tileSize;
                int someOtherWidth = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - totalWidth;

                result = right - someOtherWidth / 2;
            }
            else
                result = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;

            return result;
        }
    }
}
