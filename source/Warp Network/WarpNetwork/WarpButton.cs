/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using WarpNetwork.models;

namespace WarpNetwork
{
    class WarpButton : ClickableComponent
    {
        private bool wasHovered = false;
        private Color tint = Color.White;
        public WarpLocation location
        {
            set
            {
                loc = value;
                updateLabel();
            }
            get
            {
                return loc;
            }
        }
        public int index = 0;
        private WarpLocation loc;
        private static readonly Rectangle bg = new(384, 396, 15, 15);
        private string text = "Unnamed";
        private Vector2 textSize;

        public WarpButton(Rectangle bounds, WarpLocation location, int index) : base(bounds, "")
        {
            this.location = location;
            this.index = index;
        }
        public void updateLabel()
        {
            text = loc.Label;
            textSize = Game1.dialogueFont.MeasureString(text);
        }
        public void draw(SpriteBatch b)
        {
            if (loc == null)
                return;
            if (containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                tint = Color.Wheat;
                if (!wasHovered)
                    Game1.playSound("shiny4");
                wasHovered = true;
            }
            else
            {
                tint = Color.White;
                wasHovered = false;
            }
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, bg, bounds.X, bounds.Y, bounds.Width, bounds.Height, tint, scale, false);
            b.Draw(loc.IconTex, new Rectangle(bounds.X + 12, bounds.Y + 12, bounds.Height - 24, bounds.Height - 24), Color.White);
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2(bounds.X + bounds.Height - 9, MathF.Round(bounds.Y - textSize.Y / 2 + bounds.Height / 2 + 6)), Game1.textColor);
        }
    }
}
