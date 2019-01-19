using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.ObjectsPatches
{
    /// <summary>
    /// REASON FOR PATCHING: The relocation of Farmhouse.
    /// 
    /// Patches the method Objects.totemWarpForReal to adjusut for the 
    /// relocation of the farm house.
    /// </summary>
    public class totemWarpForRealPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public totemWarpForRealPatch(ICustomManager customManager) {
            totemWarpForRealPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if the farm house is custom. Skips the original method if so.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        public static bool Prefix(Object __instance) {
            if (!customManager.Canon && __instance.parentSheetIndex == 688) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has executed.
        /// 
        /// Adjusts the functionality of the Warp Totem to target the relocated rabbit shrine
        /// on custom farm maps.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="Object"/> that called totemWarpForReal.</param>
        public static void Postfix(Object __instance) {
            if (!customManager.Canon && __instance.parentSheetIndex == 688) {
                Game1.warpFarmer("Farm", customManager.RabbitShrine.X, customManager.RabbitShrine.Y, false);
                Game1.fadeToBlackAlpha = 0.99f;
                Game1.screenGlow = false;
                Game1.player.temporarilyInvincible = false;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.displayFarmer = true;
            }
        }
    }
}
