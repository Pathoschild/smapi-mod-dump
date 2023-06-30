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
namespace FindObjectMod.Framework.Menus.Components
{
    public class ModOptionsColorPicker : OptionsButton
    {
        public ModOptionsColorPicker(string label_, Color color, Action<Color> setColor) : base(label_, delegate ()
        {
            IClickableMenu menu = Game1.activeClickableMenu;
            Game1.activeClickableMenu = new ColorPickerMenu(color, setColor, delegate ()
            {
                Game1.activeClickableMenu = menu;
                ModMenu i = menu as ModMenu;
                bool flag = i != null;
                if (flag)
                {
                    i.Initialization();
                }
            });
        })
        {
            this.SetColor = setColor;
            this.MColor = color;
        }

        public virtual void draw(SpriteBatch b, int slotX, int slotY)
        {
            base.draw(b, slotX, slotY);
            ModMenu menu = Game1.activeClickableMenu as ModMenu;
            bool flag = menu != null;
            if (flag)
            {
                bool isAndroid = Utilities.IsAndroid;
                if (isAndroid)
                {
                    int pixBox = 8;
                    int x = this.bounds.X + this.bounds.Width + 16;
                    int y = this.bounds.Y;
                    int x2 = x + this.bounds.Height - pixBox;
                    int y2 = y + this.bounds.Height - pixBox;
                    Game1.DrawBox(x + pixBox, y, x2 - x, y2 - y, null);
                    x += pixBox;
                    y = y;
                    x2 += pixBox;
                    for (int i = y; i < y2; i++)
                    {
                        Utility.drawLineWithScreenCoordinates(x, i, x2, i, b, this.MColor, 1f);
                    }
                }
                else
                {
                    int pixBox2 = 8;
                    int x3 = this.bounds.X + this.bounds.Width + slotX + 24;
                    int y3 = this.bounds.Y + slotY + this.bounds.Height / 4;
                    int x4 = x3 + this.bounds.Height / 2 - pixBox2;
                    int y4 = y3 + this.bounds.Height / 2 - pixBox2;
                    Game1.DrawBox(x3 + pixBox2, y3, x4 - x3, y4 - y3, null);
                    x3 += pixBox2;
                    y3 = y3;
                    x4 += pixBox2;
                    for (int j = y3; j < y4; j++)
                    {
                        Utility.drawLineWithScreenCoordinates(x3, j, x4, j, b, this.MColor, 1f);
                    }
                }
            }
        }

        public Action<Color> SetColor;

        public Color MColor;
    }
}
