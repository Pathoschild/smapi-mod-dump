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
    class constructor
    {
        public static void Postfix(Farm __instance)
        {
            if (Game1.whichFarm > 4)
            {
                if (Memory.loadedFarm == null) {
                    Memory.loadCustomFarmType(Game1.whichFarm);
                }
                Rectangle newOpenArea = new Rectangle((Memory.loadedFarm.shippingBin.pointOfInteraction.x - 1) * 64, Memory.loadedFarm.shippingBin.pointOfInteraction.y * 64, 256, 192);
                Traverse.Create(__instance).Field("shippingBinLidOpenArea").SetValue(newOpenArea);
            }
        }
    }
}
