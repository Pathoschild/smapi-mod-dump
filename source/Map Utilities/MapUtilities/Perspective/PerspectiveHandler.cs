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
