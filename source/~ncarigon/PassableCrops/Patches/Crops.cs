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
using StardewValley.TerrainFeatures;
using StardewValley;

namespace PassableCrops.Patches {
    internal static class Crops {
        private static ModEntry? Mod;

        public static void Register(ModEntry mod) {
            Mod = mod;

            var harmony = new Harmony(Mod?.ModManifest?.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), "isPassable", new Type[] { typeof(Character) }),
                postfix: new HarmonyMethod(typeof(Crops), nameof(Postfix_HoeDirt_isPassable))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), "doCollisionAction"),
                prefix: new HarmonyMethod(typeof(Crops), nameof(HoeDirt_doCollisionAction)),
                postfix: new HarmonyMethod(typeof(Crops), nameof(HoeDirt_doCollisionAction))
            );
        }

        private static void Postfix_HoeDirt_isPassable(
            HoeDirt __instance, ref bool __result,
            Character c
        ) {
            try {
                if (Mod?.Config?.PassableCrops ?? false) {
                    var farmer = c as Farmer;
                    if (farmer is not null || (Mod?.Config?.PassableByAll ?? false)) {
                        __result |= __instance?.crop is not null;
                    }
                }
            } catch { }
        }

        private static void HoeDirt_doCollisionAction(
            HoeDirt __instance
        ) {
            try {
                if (Mod?.Config?.PassableCrops ?? false) {
                    // only handle raised seed crops, which should still shake when newly planted
                    if (__instance?.crop?.raisedSeeds?.Value ?? false) {
                        switch (__instance.crop.currentPhase.Value) {
                            case 0: // if newly planted, set to invalid phase to allow shake
                                __instance.crop.currentPhase.Value = -1;
                                break;
                            case -1: // if invalid phase, set back to 0
                                __instance.crop.currentPhase.Value = 0;
                                break;
                        }
                    }
                }
            } catch { }
        }
    }
}
