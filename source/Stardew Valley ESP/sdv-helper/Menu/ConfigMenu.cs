using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sdv_helper.Config;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdv_helper.Menu
{
    class ConfigMenu : IClickableMenu
    {
        private static readonly int bWidth = 800;
        private static readonly int bHeight = 600;
        private static readonly int bStartX = Game1.viewport.Width / 2 - (bWidth + borderWidth * 2) / 2;
        private static readonly int bStartY = Game1.viewport.Height / 2 - (bHeight + borderWidth * 2) / 2;
        private static readonly string menuText = "Stardew Valley ESP";
        private static readonly int paddingX = 30;
        private static readonly int paddingY = 20;
        private static readonly Rectangle titleRect = new Rectangle(0, 256, 60, 60);
        private static Dictionary<string, ColorComponent> colorPickers = new Dictionary<string, ColorComponent>();
        private static int entriesPerPage = 7; // (int)Math.Floor(bHeight / Game1.dialogueFont.MeasureString("A").Y);
        private static int currentEntry = 0;
        private Settings settings;

        public ConfigMenu(Settings settings)
            : base(bStartX, bStartY, bWidth + borderWidth * 2, bHeight + borderWidth * 2)
        {
            this.settings = settings;
            resetColorPickers();
        }

        private void resetColorPickers()
        {
            // for every entry in a category do this
            List<string> keys = settings.dSettings.Keys.ToList();
            keys.Sort();

            // possible to do a binary insertion or something?
            colorPickers.Clear();
            foreach (string key in keys)
            {
                ColorComponent c = new ColorComponent(key, settings.dSettings[key], settings);
                colorPickers.Add(key, c);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var c in colorPickers)
            {
                c.Value.receiveLeftClick(x, y, playSound);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            // base.receiveScrollWheelAction(direction);

            // < 0 is down other is up
            if (direction < 0) currentEntry = Math.Min(currentEntry + 1, colorPickers.Count - entriesPerPage);
            else currentEntry = Math.Max(currentEntry - 1, 0);

        }

        public override void draw(SpriteBatch b)
        {
            if (settings.dSettings.Count != colorPickers.Count)
                resetColorPickers();

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, bWidth, bHeight, false, true);
            Vector2 size = Game1.dialogueFont.MeasureString(menuText);
            drawTextureBox(b, Game1.menuTexture, titleRect, bStartX - paddingX / 2, bStartY - paddingY / 2, (int)size.X + paddingX, (int)size.Y + paddingY, Color.White);
            Utility.drawTextWithShadow(b, menuText, Game1.dialogueFont, new Vector2(bStartX, bStartY), Game1.textColor);

            for (int i = currentEntry; i < currentEntry + entriesPerPage; i++)
            {
                int yCoord = bStartY + borderWidth * 2 + 20 + (i - currentEntry) * (28 + 36 + 5);
                Utility.drawTextWithShadow(b, colorPickers.ElementAt(i).Key, Game1.dialogueFont, new Vector2(bStartX + borderWidth, yCoord), Game1.textColor);
                colorPickers.ElementAt(i).Value.drawAt(b, bStartX + bWidth / 2 + borderWidth, yCoord);
            }

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero,
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }
    }
}
