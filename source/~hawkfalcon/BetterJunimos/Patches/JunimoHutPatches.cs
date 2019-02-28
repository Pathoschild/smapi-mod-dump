using System;
using System.Linq;
using BetterJunimos.Abilities;
using BetterJunimos.Utils;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;

namespace BetterJunimos.Patches {
    /* areThereMatureCropsWithinRadius **OVERWRITES PREFIX**
     *
     * Search for actionable tiles
     * Completely rewrite original function.
    */
    internal class PatchSearchAroundHut {
        public static bool Prefix(JunimoHut __instance, ref bool __result) {
            // Prevent unnecessary searching when unpaid
            if (Util.Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday) {
                __instance.lastKnownCropLocation = Point.Zero;
                return false;
            }

            __result = searchAroundHut(__instance);
            return false;
        }

        // search for crops + open plantable spots
        internal static bool searchAroundHut(JunimoHut hut) {
            Guid id = Util.GetHutIdFromHut(hut);
            int radius = Util.MaxRadius;
            for (int x = hut.tileX.Value + 1 - radius; x < hut.tileX.Value + 2 + radius; ++x) {
                for (int y = hut.tileY.Value + 1 - radius; y < hut.tileY.Value + 2 + radius; ++y) {
                    // skip if we find the same lastKnownCropLocation twice
                    if (x == hut.lastKnownCropLocation.X && y == hut.lastKnownCropLocation.Y) continue;
                    Vector2 pos = new Vector2((float)x, (float)y);
                    IJunimoAbility ability = Util.Abilities.IdentifyJunimoAbility(pos, id);
                    if (ability != null) {
                        hut.lastKnownCropLocation = new Point(x, y);
                        return true;
                    }
                }
            }
            hut.lastKnownCropLocation = Point.Zero;
            return false;
        }
    }

    /* Update
     * 
     * To allow more junimos, allow working in rain
     */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoHutUpdate {
        // This is to prevent the update function from running, other than base.Update()
        // Capture sendOutTimer and use to stop execution
        public static void Prefix(JunimoHut __instance, ref int ___junimoSendOutTimer, ref int __state) {
            int timer = ___junimoSendOutTimer;
            __state = timer;
            ___junimoSendOutTimer = 0;
        }

        public static void Postfix(JunimoHut __instance, GameTime time, ref int ___junimoSendOutTimer, int __state) {
            int sendOutTimer = __state;
            if (sendOutTimer <= 0)
                return;

            ___junimoSendOutTimer = sendOutTimer - time.ElapsedGameTime.Milliseconds;
            // Don't work on farmEvent days
            if (Game1.farmEvent != null)
                return;
            // Winter
            if (Game1.IsWinter && !Util.Config.JunimoImprovements.CanWorkInWinter)
                return;
            // Rain
            if (Game1.isRaining && !Util.Config.JunimoImprovements.CanWorkInRain) 
                return;
            // Currently sending out a junimo
            if (___junimoSendOutTimer > 0) 
                return;
            // Already enough junimos
            if (__instance.myJunimos.Count() >= Util.Config.JunimoHuts.MaxJunimos)
                return;
            // Nothing to do
            if (!__instance.areThereMatureCropsWithinRadius())
                return;

            Util.SpawnJunimoAtHut(__instance);
            ___junimoSendOutTimer = 1000;
        }
    }

    /*
     * performTenMinuteAction
     * 
     * Add the end to trigger more than 3 junimos to spawn
     */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoTimerNumber {
        public static void Postfix(JunimoHut __instance, ref int ___junimoSendOutTimer) {
            int time = Util.Config.JunimoImprovements.CanWorkInEvenings ? 2400 : 1900;
            if (__instance.myJunimos.Count() < Util.Config.JunimoHuts.MaxJunimos && 
                Game1.timeOfDay < time) {
                ___junimoSendOutTimer = 1;
            }
        }
    }

    /* getUnusedJunimoNumber
     * 
     * Completely rewrite method to support more than 3 junimos
     * The only difference is the use of MaxJunimos
    */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoHutNumber {
        public static bool Prefix(JunimoHut __instance, ref int __result) {
            for (int index = 0; index < Util.Config.JunimoHuts.MaxJunimos; ++index) {
                if (index >= __instance.myJunimos.Count()) {
                    __result = index;
                    return false;
                }
                bool flag = false;
                foreach (JunimoHarvester junimo in __instance.myJunimos) {
                    if (junimo.whichJunimoFromThisHut == index) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    __result = index;
                    return false;
                }
            }
            __result = 2;
            return false;
        }
    }
}