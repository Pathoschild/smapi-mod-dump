/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System;
using EiTK.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace EiTK.Gui
{
    public class GuiButton
    {
        private string text;
        private int x;
        private int y;
        private int width;
        private int height;
        private Action action;

        private bool hovered;


        public GuiButton(string text, int x, int y, int width, int height, Action action)
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.action = action;
        }

        public void receiveLeftClick()
        {
            if (!this.hovered)
                return;
            Game1.playSound("drumkit6");
            this.action();
            this.hovered = false;
        }

        public void draw(SpriteBatch b)
        {
            this.hovered = GuiUtils.isHovered(Game1.getMouseX(), Game1.getMouseY(), this.x, this.y, this.width,
                this.height);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), this.x, this.y,
                this.width, this.height, this.hovered ? Color.Wheat : Color.White, 4f, true);
            Utility.drawTextWithShadow(b, this.text, Game1.dialogueFont,
                new Vector2((this.x + this.width / 2) - (SpriteText.getWidthOfString(this.text) / 2),
                    (this.y + this.height / 2) - (SpriteText.getHeightOfString(this.text) / 2)), Game1.textColor, 1f, -1f,
                -1, -1, 0.0f, 3);
        }
    }
}