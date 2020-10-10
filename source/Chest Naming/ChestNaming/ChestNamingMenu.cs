/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/ChestNaming
**
*************************************************/

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
