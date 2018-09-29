using System;
using System.Linq;
using BetterJunimos.Utils;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

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
            Util.Abilities.UpdateHutItems(id);
            int radius = Util.MaxRadius;
            for (int x = hut.tileX.Value + 1 - radius; x < hut.tileX.Value + 2 + radius; ++x) {
                for (int y = hut.tileY.Value + 1 - radius; y < hut.tileY.Value + 2 + radius; ++y) {
                    Vector2 pos = new Vector2((float)x, (float)y);
                    if (Util.Abilities.IsActionable(pos, id)) {
                        hut.lastKnownCropLocation = new Point(x, y);
                        return true;
                    }
                }
            }
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
        public static void Prefix(JunimoHut __instance, int __state) {
            var junimoSendOutTimer = Util.Reflection.GetField<int>(__instance, "junimoSendOutTimer");
            __state = junimoSendOutTimer.GetValue();
            junimoSendOutTimer.SetValue(0);
        }

        public static void Postfix(JunimoHut __instance, GameTime time, int __state) {
            var junimoSendOutTimer = Util.Reflection.GetField<int>(__instance, "junimoSendOutTimer");
            int sendOutTimer = __state;

            // from Update
            junimoSendOutTimer.SetValue(sendOutTimer - time.ElapsedGameTime.Milliseconds);
            if (sendOutTimer > 0 || __instance.myJunimos.Count() >= Util.Config.JunimoHuts.MaxJunimos ||
                !__instance.areThereMatureCropsWithinRadius() || Game1.farmEvent != null)
                return;
            // Winter
            if (Game1.IsWinter && !Util.Config.JunimoImprovements.CanWorkInWinter)
                return;
            // Rain
            if (Game1.isRaining && !Util.Config.JunimoImprovements.CanWorkInRain) 
                return;
            
            Util.SpawnJunimoAtHut(__instance);
            junimoSendOutTimer.SetValue(1000);
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
