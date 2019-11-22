using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestNaming
{
    class ChestNamingMenu : NamingMenu
    {
        public ChestNamingMenu(doneNamingBehavior b, string title, string defaultName = null, int maxInput = 32) : base(b, title, defaultName)
        {
            this.textBox.textLimit = maxInput;
            textBox.limitWidth = false;
            int width = textBox.Width;
            textBox.Width = width*4;
            textBox.Text = defaultName ?? "";
            textBox.Width = width;
        }
    }
}
