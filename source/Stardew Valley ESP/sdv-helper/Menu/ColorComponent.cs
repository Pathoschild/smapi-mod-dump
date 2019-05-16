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
    class ColorComponent : IClickableMenu
    {
        private DiscreteColorPicker colorPicker;
        private ClickableComponent toggleVisibility;
        private Settings settings;
        private string name;

        public ColorComponent(string name, int color, Settings settings)
        {
            // literally does not even use third argument to set starting color
            colorPicker = new DiscreteColorPicker(1, 1)
            {
                visible = false,
                colorSelection = color
            };
            this.settings = settings;
            this.name = name;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            int oldColor = colorPicker.colorSelection;
            colorPicker.receiveLeftClick(x, y, playSound);
            if (oldColor != colorPicker.colorSelection)
            {
                settings.SetColorFor(name, colorPicker.colorSelection);
            }
            colorPicker.visible = false;

            if (toggleVisibility != null && toggleVisibility.containsPoint(x, y))
            {
                colorPicker.visible = !colorPicker.visible;
            }
        }

        public void drawAt(SpriteBatch b, int x, int y)
        {
            colorPicker.xPositionOnScreen = x;
            colorPicker.yPositionOnScreen = y;
            draw(b);
        }

        public override void draw(SpriteBatch b)
        {
            // preview button thing
            Rectangle pickedPositionRect = new Rectangle(colorPicker.xPositionOnScreen + borderWidth / 2 - 28 - 36, colorPicker.yPositionOnScreen + borderWidth / 2, 28, 28);
            if (colorPicker.colorSelection == 0)
                b.Draw(Game1.mouseCursors, new Vector2(pickedPositionRect.X, pickedPositionRect.Y), new Rectangle?(new Rectangle(295, 503, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            else
                // should make this rely on the color manager maybe because there are two locations to get the color from
                // even though they do the same thing for now, but if more colors are ever added...
                b.Draw(Game1.staminaRect, pickedPositionRect, colorPicker.getColorFromSelection(colorPicker.colorSelection));
            toggleVisibility = new ClickableComponent(pickedPositionRect, "");

            // color palette
            colorPicker.draw(b);
        }

    }
}
