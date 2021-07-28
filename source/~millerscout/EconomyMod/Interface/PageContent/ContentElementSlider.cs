/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface.PageContent
{
    public class ContentElementSlider : OptionsElement, IContentElement
    {
        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            //base.draw(b, slotX, slotY);
            Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(slotX + bounds.X + bounds.Width + 8 + (int)labelOffset.X, slotY + bounds.Y + (int)labelOffset.Y), greyedOut ? (Game1.textColor * 0.33f) : Game1.textColor, 1f, 0.1f);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, sliderBGSource, slotX + bounds.X, slotY + bounds.Y, bounds.Width, bounds.Height, Color.White, 4f, drawShadow: false);
            b.Draw(Game1.mouseCursors, new Vector2((float)(slotX + bounds.X) + (float)(bounds.Width - 40) * ((float)value / 100f), slotY + bounds.Y), sliderButtonRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
        }
        public const int pixelsWide = 48;

        public const int pixelsHigh = 6;

        public const int sliderButtonWidth = 10;

        public const int sliderMaxValue = 100;

        public int value;

        public static Rectangle sliderBGSource = new Rectangle(403, 383, 6, 6);

        public static Rectangle sliderButtonRect = new Rectangle(420, 441, 10, 6);
        private Action<object> setAction;
        private Func<int> getValue;
        public Rectangle clickArea;


        public ContentElementSlider(string label, Func<int> GetValue, Action<object> setAction, int x = -1, int y = -1)
            : base(label, x, y, 192, 24, 0)
        {
            this.setAction = setAction;
            this.getValue = GetValue;
            labelOffset = new Vector2(0, -8);///i don't like how the text wasn't aligned to the slider.
            value = GetValue();
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!greyedOut)
            {
                base.leftClickHeld(x, y);
                if (x < bounds.X)
                {
                    value = 0;
                }
                else if (x > bounds.Right - 40)
                {
                    value = 100;
                }
                else
                {
                    value = (int)((float)(x - bounds.X) / (float)(bounds.Width - 40) * 100f);
                }
                setAction(value);
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!greyedOut)
            {
                base.receiveLeftClick(x, y);
                leftClickHeld(x, y);
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (Game1.options.snappyMenus && Game1.options.gamepadControls && !greyedOut)
            {
                if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                {
                    value = Math.Min(value + 10, 100);
                    setAction(value);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                {
                    value = Math.Max(value - 10, 0);
                    setAction(value);
                }
            }
        }


    }
}
