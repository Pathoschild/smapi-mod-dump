using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MTN.Patches.ObjectsPatch
{
    class totemWarpForRealPatch
    {
        public static bool Prefix(Object __instance)
        {
            if (Memory.isRabbitRelocated && __instance.parentSheetIndex == 688)
            {
                return false;
            }
            return true;
        }

        public static void Postfix(Object __instance)
        {
            if (Memory.isRabbitRelocated && __instance.parentSheetIndex == 688)
            {
                Game1.warpFarmer("Farm", Memory.loadedFarm.rabbitStatue.pointOfInteraction.x, Memory.loadedFarm.rabbitStatue.pointOfInteraction.y, false);
                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;
            }
        }
    }
}
