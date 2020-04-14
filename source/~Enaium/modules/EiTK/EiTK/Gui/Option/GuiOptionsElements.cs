using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace EiTK.Gui.Option
{
    public class GuiOptionsElements
    {
        private string label;
        
        public GuiOptionsElements(string label)
        {
            this.label = label;
        }
        public virtual void receiveLeftClick()
        {
        }

        public virtual void draw(SpriteBatch b, int slotX, int slotY)
        {
            GuiHelper.drawString(b, this.label, slotX, slotY);
        }
    }
}