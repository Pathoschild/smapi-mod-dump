using System.Collections.Generic;
using EiTK.Utils;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EiTK.Gui.Option
{
    public class GuiOptionList
    {
        
        public List<GuiOptionsElements> guiOptionsElementses;
        
        private int x;
        private int y;
        private int maxVisible;

        private int index;
        
        public GuiOptionList(int x, int y,int maxVisible)
        {
            this.x = x;
            this.y = y;
            this.maxVisible = maxVisible;
            this.guiOptionsElementses = new List<GuiOptionsElements>();
            this.index = 0;
        }

        public void receiveLeftClick()
        {
            foreach (var VARIABLE in guiOptionsElementses)
            {
                VARIABLE.receiveLeftClick();
            }
        }
        
        
        public void draw(SpriteBatch b)
        {

            if (guiOptionsElementses.Count >= maxVisible)
            {
                for (int i = index, j = 0; j < maxVisible; i++, j++)
                {
                    guiOptionsElementses[i].draw(b,this.x,this.y + j * 60);
                }
            }
            else
            {
                for (int i = index, j = 0; j < guiOptionsElementses.Count; i++, j++)
                {
                    guiOptionsElementses[i].draw(b,this.x,this.y + j * 60);
                }
            }
        }
        
        public void receiveScrollWheelAction(int direction)
        {

            // if(!GuiUtils.isHovered(Game1.getMouseX(),Game1.getMouseY(),this.x,this.y,700,60 * maxVisible))
            //     return;
            
            if (guiOptionsElementses.Count >= maxVisible)
            {
                if (direction > 0 && this.index > 0)
                {
                    this.index--;
                }
                else if (direction < 0 && this.index + maxVisible < guiOptionsElementses.Count)
                {
                    this.index++;
                }
            }
            else
            {
                if (direction > 0 && this.index > 0)
                {
                    this.index--;
                }
                else if (direction < 0 && this.index + guiOptionsElementses.Count < guiOptionsElementses.Count)
                {
                    this.index++;
                }
            }
        }

    }
}