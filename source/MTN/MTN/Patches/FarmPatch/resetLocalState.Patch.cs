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
    //[HarmonyPatch("resetLocalState")]
    class resetLocalStatePatch
    {
        public static void Postfix(Farm __instance)
        {
            if (__instance == null || __instance.Name != "Farm") return;
            if (Game1.whichFarm < 5) return;
            int binX = Memory.loadedFarm.shippingBin.pointOfInteraction.x;
            int binY = Memory.loadedFarm.shippingBin.pointOfInteraction.y;
            TemporaryAnimatedSprite actualBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(134, 226, 30, 25), new Vector2(binX, binY) * 64f + new Vector2(2f, -7f) * 4f, false, 0f, Color.White)
            {
                holdLastFrame = true,
                destroyable = false,
                interval = 20f,
                animationLength = 13,
                paused = true,
                scale = 4f,
                layerDepth = 0.1961f,
                pingPong = true,
                pingPongMotion = 0
            };
            Traverse.Create(__instance).Field("shippingBinLid").SetValue(actualBinLid);
        }
    }
}
