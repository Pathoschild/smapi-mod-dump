/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

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
    /// REASON FOR PATCHING: Starting Shipping Bin
    /// 
    /// Patches the method Farm.resetLocalState to adjust for the movement
    /// of the starting shipping bin (the bin that is not classified as a building)
    /// </summary>
    public class resetLocalStatePatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public resetLocalStatePatch(ICustomManager customManager) {
            resetLocalStatePatch.customManager = customManager;
        }

        /// <summary>
        /// Postfix method. Occurs after the original method of Farm.resetLocalState is executed.
        /// 
        /// Adjusts the starting shipping bin's lid when the user is using a custom farm.
        /// </summary>
        /// <param name="__instance">The instance of the Farm class.</param>
        public static void Postfix(Farm __instance) {
            if (customManager.Canon || __instance == null || __instance.Name != "Farm") return;

            int binX = customManager.ShippingBin.X;
            int binY = customManager.ShippingBin.Y;

            TemporaryAnimatedSprite actualBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(134, 226, 30, 25), new Vector2(binX, binY) * 64f + new Vector2(2f, -7f) * 4f, false, 0f, Color.White) {
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
