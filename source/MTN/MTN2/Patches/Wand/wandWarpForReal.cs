using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.WandPatches
{
    /// <summary>
    /// REASON FOR PATCHING: The relocation of Farm House
    /// 
    /// Patches the method Wand.wandWarpForReal to accomidate for the relocation
    /// of the farm house on custom maps.
    /// </summary>
    public class wandWarpForRealPatch
    {
        private static CustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public wandWarpForRealPatch(CustomManager customManager) {
            wandWarpForRealPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if a custom farm map is loaded.
        /// </summary>
        /// <returns><c>true</c> if a custom farm map is loaded, otherwise <c>false</c>.</returns>
        public static bool Prefix() {
            return (customManager.Canon) ? true : false;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method is executed.
        /// 
        /// If a custom farm map is loaded, the functionality of the wand is adjusted to
        /// warp the player to the correct coordinates on the farm map.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="Wand"/> that called wandWarpForReal.</param>
        public static void Postfix(Wand __instance) {
            if (customManager.Canon) return;

            Game1.warpFarmer("Farm", customManager.FarmHousePorch.X, customManager.FarmHousePorch.Y, false);
            if (!Game1.isStartingToGetDarkOut()) {
                Game1.playMorningSong();
            } else {
                Game1.changeMusicTrack("none");
            }
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Traverse.Create(__instance).Field("lastUser").Field("temporarilyInvincible").SetValue(false);
            Traverse.Create(__instance).Field("lastUser").Field("temporaryInvincibilityTimer").SetValue(0);
            Game1.displayFarmer = true;
        }
    }
}
