using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.WandPatches
{
    public class wandWarpForRealPatch
    {
        private static CustomFarmManager farmManager;

        public wandWarpForRealPatch(CustomFarmManager farmManager) {
            wandWarpForRealPatch.farmManager = farmManager;
        }

        public static bool Prefix() {
            return (farmManager.Canon) ? true : false;
        }

        public static void Postfix(Wand __instance) {
            if (farmManager.Canon) return;

            Game1.warpFarmer("Farm", farmManager.FarmHousePorch.X, farmManager.FarmHousePorch.Y, false);
            if (!Game1.isStartingToGetDarkOut()) {
                Game1.playMorningSong();
            } else {
                Game1.changeMusicTrack("none");
            }
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Traverse.Create(__instance).Field("lastUser").Field("temporarilyInvincible").SetValue(false);
            Traverse.Create(__instance).Field("lastUser").Field("temporaryInvincibilityTimer").SetValue(0);
            Game1.displayFarmer = true;
        }
    }
}
