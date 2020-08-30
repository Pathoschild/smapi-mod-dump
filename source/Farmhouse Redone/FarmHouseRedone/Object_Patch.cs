using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone
{
    class Object_totemWarpForReal_Patch
    {
        public static bool Prefix(StardewValley.Object __instance)
        {
            if(__instance.parentSheetIndex == 688 && FarmState.hasFarmWarpChanged())
            {
                Vector2 farmWarp = FarmState.farmWarpLocation;
                Game1.warpFarmer("Farm", (int)farmWarp.X, (int)farmWarp.Y, false);

                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;

                return false;
            }
            return true;
        }
    }
}
