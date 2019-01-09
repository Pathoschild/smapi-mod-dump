using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.ObjectsPatches
{
    public class totemWarpForRealPatch
    {
        private static CustomFarmManager farmManager;

        public totemWarpForRealPatch(CustomFarmManager farmManager) {
            totemWarpForRealPatch.farmManager = farmManager;
        }

        public static bool Prefix(Object __instance) {
            if (!farmManager.Canon && __instance.parentSheetIndex == 688) {
                return false;
            }
            return true;
        }

        public static void Postfix(Object __instance) {
            if (!farmManager.Canon && __instance.parentSheetIndex == 688) {
                Game1.warpFarmer("Farm", farmManager.RabbitShrine.X, farmManager.RabbitShrine.Y, false);
                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;
            }
        }
    }
}
