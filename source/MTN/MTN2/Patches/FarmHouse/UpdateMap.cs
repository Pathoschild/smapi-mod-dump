/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmHousePatches
{
    /// <summary>
    /// REASON FOR PATCHING: Adjust exit warp if Farm House was moved.
    /// 
    /// Patches the method FarmHouse.updateMap to accomidate for custom
    /// farm maps with the farm house relocated. Resets the exit warp,
    /// the one players use when leaving the farm housue.
    /// </summary>
    public class updateMapPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public updateMapPatch(ICustomManager customManager) {
            updateMapPatch.customManager = customManager;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has executed.
        /// 
        /// Resets the warps when the Farm House is upgraded to accomidate for custom farm maps, 
        /// assuming the farm house has been relocated.
        /// </summary>
        /// <param name="__instance">The instance of FarmHouse that called updateMap.</param>
        public static void Postfix(FarmHouse __instance) {
            // TO DO: Refactor for custom FarmHouse maps.
            if (customManager.Canon) return;

            if (__instance is Cabin) {
                // TO DO
            } else {
                int X = 0;
                int Y = 0;
                __instance.warps.Clear();
                switch (Game1.MasterPlayer.houseUpgradeLevel) {
                    case 0:
                        X = 3;
                        Y = 12;
                        break;
                    case 1:
                        X = 9;
                        Y = 12;
                        break;
                    case 2:
                    case 3:
                        X = 12;
                        Y = 21;
                        __instance.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
                        __instance.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
                        break;
                }
                __instance.warps.Add(new Warp(X, Y, "Farm", customManager.FarmHousePorch.X, customManager.FarmHousePorch.Y, false));
            }
        }
    }
}
