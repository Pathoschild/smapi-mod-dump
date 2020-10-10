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
