using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.WandPatch
{
    class wandWarpForRealPatch
    {
        public static bool Prefix()
        {
            return (Memory.isFarmHouseRelocated) ? false : true;
        }

        public static void Postfix(Wand __instance)
        {
            if (!Memory.isFarmHouseRelocated) return;

            Game1.warpFarmer("Farm", Memory.loadedFarm.farmHousePorchX(), Memory.loadedFarm.farmHousePorchY(), false);
            if (!Game1.isStartingToGetDarkOut())
                Game1.playMorningSong();
            else
                Game1.changeMusicTrack("none");
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            //__instance.lastUser.temporarilyInvincible = false;
            Traverse.Create(__instance).Field("lastUser").Field("temporarilyInvincible").SetValue(false);
            //__instance.lastUser.temporaryInvincibilityTimer = 0;
            Traverse.Create(__instance).Field("lastUser").Field("temporaryInvincibilityTimer").SetValue(0);
            Game1.displayFarmer = true;
        }
    }
}
