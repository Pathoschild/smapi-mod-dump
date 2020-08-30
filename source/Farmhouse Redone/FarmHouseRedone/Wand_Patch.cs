using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    class Wand_wandWarpForReal_Patch
    {
        public static bool Prefix(Wand __instance)
        {
            Vector2 frontDoor = FarmState.frontDoorLocation + new Vector2(0, 1);

            Game1.warpFarmer("Farm", (int)frontDoor.X, (int)frontDoor.Y, false);
            if (!Game1.isStartingToGetDarkOut())
                Game1.playMorningSong();
            else
                Game1.changeMusicTrack("none", false, Game1.MusicContext.Default);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;

            IReflectedField<Farmer> lastUser = FarmHouseStates.reflector.GetField<Farmer>(__instance, "lastUser");

            lastUser.GetValue().temporarilyInvincible = false;
            lastUser.GetValue().temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;

            return false;
        }
    }
}
