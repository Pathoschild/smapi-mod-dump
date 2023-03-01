/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley;

namespace PassbleCrops {
    public class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), "isPassable", new Type[] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(PassableIfFarmer))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), "doCollisionAction"),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(DoCollisionAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(DoCollisionAction))
            );
        }

        private static void PassableIfFarmer(HoeDirt __instance, Character c, ref bool __result) {
            try {
                __result |= __instance?.crop is not null && c is Farmer;
            } catch { }
        }

        private static void DoCollisionAction(HoeDirt __instance) {
            try {
                // only handle raised seed crops, which should still shake when newly planted
                if (__instance?.crop?.raisedSeeds?.Value ?? false) {
                    var phase = __instance?.crop?.currentPhase?.Value ?? -2;
                    if (phase == 0) { // prefix: if newly planted, set to invalid phase to allow shake
                        __instance!.crop.currentPhase.Value = -1;
                    } else if (phase == -1) { // postfix: if invalid phase, set back to 0
                        __instance!.crop.currentPhase.Value = 0;
                    }
                }
            } catch { }
        }
    }
}
