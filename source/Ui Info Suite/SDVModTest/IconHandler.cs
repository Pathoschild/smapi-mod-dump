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
using StardewModdingAPI.Utilities;
using System;

namespace UIInfoSuite
{
    internal class IconHandler
    {
        public static IconHandler Handler { get; private set; }

        static IconHandler()
        {
            if (Handler == null)
                Handler = new IconHandler();
        }

        private readonly PerScreen<int> _amountOfVisibleIcons = new PerScreen<int>();

        private IconHandler()
        {

        }

        public Point GetNewIconPosition()
        {

            var yPos = Game1.options.zoomButtons ? 290 : 260;
            var xPosition = Tools.GetWidthInPlayArea() - 70 - 48 * _amountOfVisibleIcons.Value;
            if (Game1.player.visibleQuestCount > 0)
            {
                xPosition -= 65;
            }
            ++_amountOfVisibleIcons.Value;
            return new Point(xPosition, yPos);
        }

        public void Reset(object sender, EventArgs e)
        {
            _amountOfVisibleIcons.Value = 0;
        }


    }
}
