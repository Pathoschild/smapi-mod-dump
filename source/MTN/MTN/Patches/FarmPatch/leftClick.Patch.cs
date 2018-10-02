using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.FarmPatch
{
    //[HarmonyPatch(typeof(Farm))]
    //[HarmonyPatch("leftClick")]
    class leftClickPatch
    {

        public static bool Prefix()
        {
            if (Memory.isCustomFarmLoaded)
            {
                return false;
            }
            return true;
        }

        public static void Postfix(Farm __instance, ref bool __result, int x, int y, Farmer who)
        {
            if (!Memory.isCustomFarmLoaded || __instance.Name != "Farm") return;

            int binX = Memory.loadedFarm.shippingBin.pointOfInteraction.x;
            int binY = Memory.loadedFarm.shippingBin.pointOfInteraction.y;

            if (who.ActiveObject != null && x / 64 >= binX && x / 64 <= binX + 1 && y / 64 >= binY && y / 64 <= binY+1 && who.ActiveObject.canBeShipped() && Vector2.Distance(who.getTileLocation(), new Vector2(71.5f, 14f)) <= 2f)
            {
                __instance.shippingBin.Add(who.ActiveObject);
                __instance.lastItemShipped = who.ActiveObject;
                who.showNotCarrying();
                __instance.showShipment(who.ActiveObject, true);
                who.ActiveObject = null;
                __result = true;
            }
        }
    }
}
