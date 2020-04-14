using System;
using System.Collections.Generic;
using EiTK.Gui.Option;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace EiTK.Gui
{
    public class GuiMenu : IClickableMenu
    {

        public List<GuiButton> buttons;

        public List<GuiOptionList> optionLists;


        public GuiMenu()
        {
            buttons = new List<GuiButton>();
            optionLists = new List<GuiOptionList>();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            try
            {
                foreach (var VARIABLE in buttons)
                {
                    VARIABLE.receiveLeftClick();
                }
            
                foreach (var VARIABLE in optionLists)
                {
                    VARIABLE.receiveLeftClick();
                }
            }
            catch (NullReferenceException e)
            {
                ;
            }


        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            try
            {
                foreach (var VARIABLE in buttons)
                {
                    VARIABLE.draw(b);
                }
            
                foreach (var VARIABLE in optionLists)
                {
                    VARIABLE.draw(b);
                }
                
            }
            catch (NullReferenceException e)
            {
                ;
            }
            

            
        }


        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            
            foreach (var VARIABLE in optionLists)
            {
                VARIABLE.receiveScrollWheelAction(direction);
            }
            
        }
    }
}