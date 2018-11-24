using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGeodes
{
    public class AdornMenu_PickTool : IClickableMenu
    {
        public AdornMenu_PickTool()
        : base(Game1.viewport.Width / 2 - 320, Game1.viewport.Height / 2 - 240, 640, 480, true)
        {
            // todo
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            justClicked = true;
        }

        public bool justClicked = false;
        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
            StardewValley.BellsAndWhistles.SpriteText.drawStringHorizontallyCenteredAt(b, "Adornments", xPositionOnScreen + 320, yPositionOnScreen + IClickableMenu.borderWidth);
            //b.DrawString(Game1.dialogueFont, "Adornments", new Vector2(xPositionOnScreen + IClickableMenu.borderWidth, yPositionOnScreen + IClickableMenu.borderWidth), Color.Black);

            if (drawHover(b, "Weapon", xPositionOnScreen + IClickableMenu.borderWidth * 5, yPositionOnScreen + IClickableMenu.borderWidth * 3) && justClicked)
            {
                Game1.activeClickableMenu = new AdornMenu(ToolType.Weapon);
            }
            if (drawHover(b, "Pickaxe", xPositionOnScreen + IClickableMenu.borderWidth * 5, yPositionOnScreen + IClickableMenu.borderWidth * 4) && justClicked)
            {
                Game1.activeClickableMenu = new AdornMenu(ToolType.Pickaxe);
            }
            if (drawHover(b, "Axe", xPositionOnScreen + IClickableMenu.borderWidth * 5, yPositionOnScreen + IClickableMenu.borderWidth * 5) && justClicked)
            {
                Game1.activeClickableMenu = new AdornMenu(ToolType.Axe);
            }
            if (drawHover(b, "Watering Can", xPositionOnScreen + IClickableMenu.borderWidth * 5, yPositionOnScreen + IClickableMenu.borderWidth * 6) && justClicked)
            {
                Game1.activeClickableMenu = new AdornMenu(ToolType.WateringCan);
            }
            if (drawHover(b, "Hoe", xPositionOnScreen + IClickableMenu.borderWidth * 5, yPositionOnScreen + IClickableMenu.borderWidth * 7) && justClicked)
            {
                Game1.activeClickableMenu = new AdornMenu(ToolType.Hoe);
            }

            base.drawMouse(b);

            justClicked = false;
        }

        private bool drawHover(SpriteBatch b, string str, int x, int y)
        {
            Color col = Color.Black;
            var strSize = Game1.dialogueFont.MeasureString(str);

            bool ret = false;
            if ( new Rectangle(x, y, (int) strSize.X, (int) strSize.Y).Contains(Game1.getMouseX(), Game1.getMouseY()) )
            {
                col = new Color(0.4f, 0.4f, 0.4f);
                ret = true;
            }

            b.DrawString(Game1.dialogueFont, str, new Vector2(x, y), col);
            return ret;
        }
    }
}
