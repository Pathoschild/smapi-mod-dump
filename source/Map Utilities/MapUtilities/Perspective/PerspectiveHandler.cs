/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/Map-Utilities
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Perspective
{
    public static class PerspectiveHandler
    {
        public static int angle;

        public static void update(GameLocation location = null)
        {
            if (location == null)
                location = Game1.currentLocation;


        }
    }
}
