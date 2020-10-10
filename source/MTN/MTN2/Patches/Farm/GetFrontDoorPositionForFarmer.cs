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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmPatches
{
    /// <summary>
    /// REASON FOR PATCHING: A custom farm map with a farm house that is relocated
    /// from its canon position requires this method to readjusts, as it uses hardcoded
    /// values to point to the porch of the farmhouse.
    /// 
    /// 
    /// Patches the method Farm.getFrontDoorPositionForFarmer 
    /// to adjust for the movement of the Farm House.
    /// </summary>
    public class getFrontDoorPositionForFarmerPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public getFrontDoorPositionForFarmerPatch(ICustomManager customManager) {
            getFrontDoorPositionForFarmerPatch.customManager = customManager;
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
            if (!customManager.Canon) {
                __result = customManager.FarmHousePorch;
            }
        }
    }
}
