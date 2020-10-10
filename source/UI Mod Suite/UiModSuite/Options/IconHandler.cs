/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/UiModSuite
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiModSuite.Options {
    class IconHandler {

        // Handles icon display x offset
        public static int amountOfVisibleIcons = 0;

        /// <summary>
        /// This class is used to draw icons in a cascading fashion
        /// </summary>
        /// <returns>The correct offset for the new icon</returns>
        internal static int getIconXPosition() {
            int iconX = (int) DemiacleUtility.getWidthInPlayArea() - 134 - ( 46 * amountOfVisibleIcons );
            amountOfVisibleIcons++;
            return iconX;
        }

        internal static void reset( object sender, EventArgs e ) {
            amountOfVisibleIcons = 0;
        }

    }
}
