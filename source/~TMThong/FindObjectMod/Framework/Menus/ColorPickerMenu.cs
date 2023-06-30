/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace FindObjectMod.Framework.Menus
{
    public class ColorPickerMenu : IClickableMenu
    {
        public ColorPickerMenu(Color color, Action<Color> setColor, Action callBack)
        {
            this.MColor = color;
            this.action = setColor;
            this.callback = callBack;
        }

        private void init()
        {
            this.width = 256;
            this.height = this.width / 2;
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
            bool isAndroid = Utilities.IsAndroid;
            if (isAndroid)
            {
                this.width = Game1.viewport.Width / 2;
                this.height = Game1.viewport.Height / 2;
                this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
            }
            this.colorPicker = new ColorPicker("Color Picker", this.xPositionOnScreen, this.yPositionOnScreen);
            this.colorPicker.hueBar.bounds = new Rectangle(0, 0, this.width, this.height / 3);
            this.colorPicker.saturationBar.bounds = new Rectangle(0, this.height / 3, this.width, this.height / 3);
            this.colorPicker.valueBar.bounds = new Rectangle(0, this.height / 3 * 2, this.width, this.height / 3);
            this.colorPicker.SetPrivateFieldValue("bounds", new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
            this.colorPicker.setColor(this.MColor);
            this.rectangle = new Rectangle(Game1.viewport.Width / 2 - 32, this.yPositionOnScreen - 64 - 48, 64, 64);
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width + 24, this.yPositionOnScreen + this.height - 64, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
        }

        public override void draw(SpriteBatch b)
        {
            bool flag = !GameMenu.forcePreventClose;
            if (flag)
            {
                Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, null);
                Game1.DrawBox(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height, null);
                int x = this.rectangle.X;
                int y = this.rectangle.Y;
                int x2 = this.rectangle.X + this.rectangle.Width;
                int y2 = this.rectangle.Y + this.rectangle.Height;
                for (int i = y; i < y2; i++)
                {
                    Utility.drawLineWithScreenCoordinates(x, i, x2, i, b, this.MColor, 1f);
                }
                bool flag2 = this.colorPicker == null;
                if (flag2)
                {
                    this.init();
                }
                else
                {
                    this.colorPicker.draw(b);
                }
                ClickableTextureComponent clickableTextureComponent = this.okButton;
                if (clickableTextureComponent != null)
                {
                    clickableTextureComponent.draw(b);
                }
                base.drawMouse(b);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool flag = this.okButton.bounds.Contains(x, y);
            if (flag)
            {
                Action action = this.callback;
                if (action != null)
                {
                    action();
                }
            }
            else
            {
                ColorPicker colorPicker = this.colorPicker;
                Color? old = (colorPicker != null) ? new Color?(colorPicker.getSelectedColor()) : null;
                ColorPicker colorPicker2 = this.colorPicker;
                if (colorPicker2 != null)
                {
                    colorPicker2.click(x, y);
                }
                Color? color = old;
                ColorPicker colorPicker3 = this.colorPicker;
                bool flag2 = color != ((colorPicker3 != null) ? new Color?(colorPicker3.getSelectedColor()) : null);
                if (flag2)
                {
                    Action<Color> action2 = this.action;
                    if (action2 != null)
                    {
                        action2(this.colorPicker.getSelectedColor());
                    }
                    this.MColor = this.colorPicker.getSelectedColor();
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            ColorPicker colorPicker = this.colorPicker;
            Color? old = (colorPicker != null) ? new Color?(colorPicker.getSelectedColor()) : null;
            ColorPicker colorPicker2 = this.colorPicker;
            if (colorPicker2 != null)
            {
                colorPicker2.clickHeld(x, y);
            }
            Color? color = old;
            ColorPicker colorPicker3 = this.colorPicker;
            bool flag = color != ((colorPicker3 != null) ? new Color?(colorPicker3.getSelectedColor()) : null);
            if (flag)
            {
                Action<Color> action = this.action;
                if (action != null)
                {
                    action(this.colorPicker.getSelectedColor());
                }
                this.MColor = this.colorPicker.getSelectedColor();
            }
        }

        public Color MColor;

        private const int pixBox = 8;

        public Action<Color> action;

        public Action callback;

        public ColorPicker colorPicker;

        public Rectangle rectangle = new Rectangle(0, 0, 0, 0);

        public ClickableTextureComponent okButton;
    }
}
