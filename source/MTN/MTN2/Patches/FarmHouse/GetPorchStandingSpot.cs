/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmHousePatches
{
    /// <summary>
    /// REASON FOR PATCHING: Adjust point of reference if Farm house was moved.
    /// 
    /// Patches the method FarmHouse.getPorchStandingSpot to adjust for the 
    /// movement of the Farm House.
    /// </summary>
    public class getPorchStandingSpotPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the custom farms (and the loaded farm).</param>
        public getPorchStandingSpotPatch(ICustomManager customManager) {
            getPorchStandingSpotPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method of FarmHouse.getPorchStandingSpot
        /// is executed.
        /// 
        /// Skips over the original method if a custom farm is loaded.
        /// </summary>
        /// <returns></returns>
        public static bool Prefix() {
            if (!customManager.Canon) return false;
            return true;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method of FarmHouse.getPorchStandingSpot
        /// has executed.
        /// 
        /// Adjusts the results if a custom farm is currently loaded. Otherwise, does nothing.
        /// </summary>
        /// <param name="__instance">The instance of the FarmHouse class</param>
        /// <param name="__result">The returning point instance</param>
        public static void Postfix(FarmHouse __instance, ref Point __result) {
            if (customManager.Canon) return;
            int num = __instance.farmerNumberOfOwner;
            
            if (num == 0 || num == 1) {
                __result = customManager.FarmHousePorch;
                return;
            }
        }
    }
}
