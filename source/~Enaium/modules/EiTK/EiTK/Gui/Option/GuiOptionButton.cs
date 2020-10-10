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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace EiTK.Gui.Option
{
    public class GuiOptionButton : GuiOptionsElements
    {
        private string buttonText;
        private int width;
        private Action action;

        private GuiButton button;

        public GuiOptionButton(string label, string buttonText,int width, Action action) : base(label)
        {
            this.buttonText = buttonText;
            this.width = width;
            this.action = action;
        }

        public override void receiveLeftClick()
        {
            base.receiveLeftClick();
            this.button.receiveLeftClick();
        }


        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            base.draw(b, slotX, slotY);
            this.button = new GuiButton(this.buttonText,slotX + this.width,slotY,SpriteText.getWidthOfString(this.buttonText) + 50,50,this.action);
            this.button.draw(b);
        }
    }
}