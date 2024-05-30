/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace ichortower.ui
{
    public abstract class Widget
    {
        protected IClickableMenu parent;
        public Rectangle Bounds;
        public string Name = "";
        public string HoverText;
        public bool InHoverState = false;
        public bool InActiveState = false;

        public Widget(IClickableMenu parent)
        {
            this.parent = parent;
        }

        public Widget(IClickableMenu parent, Rectangle bounds)
        {
            this.parent = parent;
            this.Bounds = bounds;
        }

        public Widget(IClickableMenu parent, Rectangle bounds, string name)
        {
            this.parent = parent;
            this.Bounds = bounds;
            this.Name = name;
        }

        public virtual void draw(SpriteBatch b)
        {
        }

        public virtual void click(int x, int y, bool playSound = true)
        {
        }

        public virtual void clickHold(int x, int y)
        {
        }

        public virtual void keyPress(Keys key)
        {
        }

        public virtual void scrollWheel(int direction)
        {
        }

        public static Rectangle[] nineslice(Rectangle source, int cornerX, int cornerY)
        {
            int[] xval = {
                source.X,
                source.X + cornerX,
                source.X + source.Width - cornerX
            };
            int[] yval = {
                source.Y,
                source.Y + cornerY,
                source.Y + source.Height - cornerY
            };
            int[] wval = {
                cornerX,
                source.Width - 2 * cornerX,
                cornerX
            };
            int[] hval = {
                cornerY,
                source.Height - 2 * cornerY,
                cornerY
            };
            var ret = new List<Rectangle>();
            for (int c = 0; c < 3; ++c) {
                for (int r = 0; r < 3; ++r) {
                    ret.Add(new Rectangle(xval[r], yval[c], wval[r], hval[c]));
                }
            }
            return ret.ToArray();
        }
    }
}
