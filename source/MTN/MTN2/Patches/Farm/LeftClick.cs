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
    /// REASON FOR PATCHING: To adjust the logic pertaining to the shipping bin,
    /// as the original method uses hardcoded coordinates.
    /// 
    /// 
    /// Patches the method Farm.leftClick to adjust for the movement of the
    /// starting shipping bin (the bin that is not classified as a building).
    /// </summary>
    public class leftClickPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="farmManager">The class controlling information pertaining to the custom farms (and the loaded farm).</param>
        public leftClickPatch(ICustomManager customManager) {
            leftClickPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix method. Occurs before the original method is executed.
        /// 
        /// Checks if 
        /// </summary>
        /// <returns></returns>
        public static bool Prefix() {
            if (!customManager.Canon) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="who"></param>
        public static void Postfix(Farm __instance, ref bool __result, int x, int y, Farmer who) {
            if (customManager.Canon || __instance.Name != "Farm") return;

            int binX = customManager.ShippingBin.X;
            int binY = customManager.ShippingBin.Y;

            if ((who.ActiveObject != null) &&
                (x / 64 >= binX) && (x / 64 <= binX + 1) &&
                (y / 64 >= binY) && (y / 64 <= binY + 1) &&
                (who.ActiveObject.canBeShipped()) && Vector2.Distance(who.getTileLocation(), new Vector2(71.5f, 14f)) <= 2f) {
                //Code segment starts here.
                __instance.getShippingBin(who).Add(who.ActiveObject);
                __instance.lastItemShipped = who.ActiveObject;
                who.showNotCarrying();
                __instance.showShipment(who.ActiveObject, true);
                who.ActiveObject = null;
                __result = true;
            }
        } 
    }
}
