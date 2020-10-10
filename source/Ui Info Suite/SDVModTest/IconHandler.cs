/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIInfoSuite
{
    class IconHandler
    {
        public static IconHandler Handler { get; private set; }

        static IconHandler()
        {
            if (Handler == null)
                Handler = new IconHandler();
        }

        private int _amountOfVisibleIcons;

        private IconHandler()
        {

        }

        public Point GetNewIconPosition()
        {
            int yPos = Game1.options.zoomButtons ? 290 : 260;
            int xPosition = (int)Tools.GetWidthInPlayArea() - 70 - 48 * _amountOfVisibleIcons;
            if (Game1.player.questLog.Any())
            {
                x -= 65;
	        }
            ++_amountOfVisibleIcons;
            return new Point(xPosition, yPos);
        }

        public void Reset(object sender, EventArgs e)
        {
            _amountOfVisibleIcons = 0;
        }


    }
}
