using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmPatches
{
    /// <summary>
    /// Patches the method Farm.getFrontDoorPositionForFarmer 
    /// to adjust for the movement of the Farm House.
    /// </summary>
    public class getFrontDoorPositionForFarmerPatch
    {
        private static CustomFarmManager farmManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="farmManager">The class controlling information pertaining to the custom farms (and the loaded farm).</param>
        public getFrontDoorPositionForFarmerPatch(CustomFarmManager farmManager) {
            getFrontDoorPositionForFarmerPatch.farmManager = farmManager;
        }

        /// <summary>
        /// Postfix method. Occurs after the original method of Farm.getFrontDoorPositionForFarmer 
        /// is executed.
        /// 
        /// Adjusts the returning value if the farm is a custom farm.
        /// </summary>
        /// <param name="__instance">The instance of the Farm class.</param>
        /// <param name="__result">The returning Point value.</param>
        public static void Postfix(Farm __instance, ref Point __result) {
            if (!farmManager.Canon) {
                __result = farmManager.FarmHousePorch;
            }
        }
    }
}
