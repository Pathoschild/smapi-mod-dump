using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using MapUtilities.Parallax;

namespace MapUtilities
{
    class Background_draw_Patch
    {
        public static bool Prefix(SpriteBatch b, Background __instance)
        {
            if(__instance is ParallaxBackground)
            {
                ParallaxBackground bg = (__instance as ParallaxBackground);
                bg.draw(b);
                return false;
            }
            return true;
        }
    }

    class Background_update_Patch
    {
        public static bool Prefix(xTile.Dimensions.Rectangle viewport, Background __instance)
        {
            if (__instance is ParallaxBackground)
            {
                ParallaxBackground bg = (__instance as ParallaxBackground);
                bg.update(viewport);
                return false;
            }
            return true;
        }
    }
}
