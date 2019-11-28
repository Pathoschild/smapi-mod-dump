using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities
{
    class GameLocation_draw_Patch
    {
        public static bool Prefix(SpriteBatch b, GameLocation __instance)
        {
            Particles.ParticleHandler.draw(b);
            return true;
        }
    }
}
