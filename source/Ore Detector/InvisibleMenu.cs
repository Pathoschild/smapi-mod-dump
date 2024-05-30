/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Ore-Detector
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreDetector
{
    public class InvisibleMenu : IClickableMenu
    {
        Texture2D rectangle;

        private int rectangleWidth = 400;

        private int rectangleHeight = 200;
        public InvisibleMenu() 
        {
            int screenWidth = Game1.viewport.Width;
            int screenHeight = Game1.viewport.Height;
            if (!ModEntry.Config.showOreName)
                rectangleWidth = 300;
            base.initialize(0, 0, screenWidth, screenHeight);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            if (rectangle == null)
            {
                rectangle = new Texture2D(b.GraphicsDevice, 1, 1);
                rectangle.SetData(new[] { Color.White });
            }
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            b.DrawString(Game1.dialogueFont, I18n.OreDetector_SetPosition(), new Vector2(mouseX - rectangleWidth / 2 - 75, mouseY - rectangleHeight / 2 - Game1.dialogueFont.LineSpacing), Color.White);
            b.Draw(rectangle, new Rectangle(mouseX - rectangleWidth / 2 - 75, mouseY - rectangleHeight / 2, rectangleWidth, rectangleHeight), Color.Red * 0.5f);
            drawMouse(b);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            ModEntry.Config.customPosition = new Vector2(x - rectangleWidth / 2, y - rectangleHeight / 2);
            ModEntry.instance.Helper.WriteConfig(ModEntry.Config);
            exitThisMenu();
        }
    }
}
