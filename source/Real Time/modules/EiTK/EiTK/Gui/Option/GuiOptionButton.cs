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