using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.EventPatches
{
    /// <summary>
    /// REASON FOR PATCHING: To readjust the X, Y coordinates of the
    /// warp(s) to the farmhouse after festivals/events, for the farmhouse
    /// may have moved due to custom farm maps.
    /// 
    /// Patches the method Event.setExitLocation to readjust the warp for
    /// custom farm maps.
    /// </summary>
    public class setExitLocationPatch
    {
        private static CustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public setExitLocationPatch(CustomManager customManager) {
            setExitLocationPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix method. Occurs before the original method is executed.
        /// 
        /// Checks to see if a cusstom farm map is loaded, and if the exit location to be the farm
        /// map. Readjusts the warp point if so.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="location"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Prefix(Event __instance, string location, ref int x, ref int y) {
            if (!customManager.Canon && location == "Farm") {
                x = customManager.FarmHousePorch.X + 2 - Utility.getFarmerNumberFromFarmer(Game1.player);
                y = customManager.FarmHousePorch.Y;
            }
        }
    }
}
