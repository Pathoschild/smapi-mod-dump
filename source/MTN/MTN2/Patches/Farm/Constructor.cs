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
    /// REASON FOR PATCHING: To adjust the field containing the location
    /// of the lid for the starting ship bin, for a custom farm map may
    /// have relocated the shipping bin.
    /// 
    /// 
    /// Patches the constructor for the Farm class to adjust the starting
    /// shipping bin's location. The patch handles the shipping bin's lid
    /// location.
    /// </summary>
    public class ConstructorFarmPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public ConstructorFarmPatch(ICustomManager customManager) {
            ConstructorFarmPatch.customManager = customManager;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original constructor has executed.
        /// 
        /// Readjusts the location of the shipping bin's lid if a custom farm is loaded. Otherwise,
        /// nothing is done.
        /// </summary>
        /// <param name="__instance">The instance of Farm that was created.</param>
        public static void Postfix(Farm __instance) {
            if (!customManager.Canon) {
                if (customManager.LoadedFarm == null) {
                    customManager.LoadCustomFarmByMtnData();
                }
                Rectangle newOpenArea = new Rectangle((customManager.ShippingBin.X - 1) * 64, customManager.ShippingBin.Y * 64, 256, 192);
                Traverse.Create(__instance).Field("shippingBinLidOpenArea").SetValue(newOpenArea);
            }
        }
    }
}
