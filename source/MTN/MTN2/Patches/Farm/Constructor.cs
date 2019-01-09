using Harmony;
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
    /// Patches the constructor for the Farm class to adjust the starting
    /// shipping bin's location. The patch handles the shipping bin's lid
    /// location.
    /// </summary>
    public class ConstructorFarmPatch
    {
        private static CustomFarmManager farmManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="farmManager">The class controlling information pertaining to the custom farms (and the loaded farm).</param>
        public ConstructorFarmPatch(CustomFarmManager farmManager) {
            ConstructorFarmPatch.farmManager = farmManager;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original constructor has executed.
        /// 
        /// Readjusts the location of the shipping bin's lid if a custom farm is loaded. Otherwise,
        /// nothing is done.
        /// </summary>
        /// <param name="__instance">The instance of Farm that was created.</param>
        public static void Postfix(Farm __instance) {
            if (!farmManager.Canon) {
                if (farmManager.LoadedFarm == null) {
                    farmManager.LoadCustomFarm(Game1.whichFarm);
                }
                Rectangle newOpenArea = new Rectangle((farmManager.ShippingBinPoints.X - 1) * 64, farmManager.ShippingBinPoints.Y * 64, 256, 192);
                Traverse.Create(__instance).Field("shippingBinLidOpenArea").SetValue(newOpenArea);
            }
        }
    }
}
