using Harmony;
using System.Collections.Generic;
using StardewValley;
using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;
using Netcode;
using StardewValley.Tools;

namespace PrismaticTools.Framework {

    [HarmonyPatch(typeof(Tree), "performToolAction")]
    internal class PrismaticPerformToolActionTree {
        static void Prefix(ref Tree __instance, Tool t, int explosion) {
            if (t is Axe && (t as Axe).UpgradeLevel == 5 && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f) {
                __instance.health.Value = 0.0f;
            }
        }
    }

    [HarmonyPatch(typeof(FruitTree), "performToolAction")]
    internal class PrismaticPerformToolActionFruitTree {
        static void Prefix(ref FruitTree __instance, Tool t, int explosion) {
            if (t is Axe && (t as Axe).UpgradeLevel == 5 && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f) {
                __instance.health.Value = 0.0f;
            }
        }
    }

    [HarmonyPatch(typeof(Pickaxe), "DoFunction")]
    internal class PrimaticDoFunction {
        static void Prefix(ref Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who) {
            if (__instance.UpgradeLevel == 5) {
                if (location.Objects.TryGetValue(new Vector2(x/64, y/64), out Object obj)) {
                    if (obj.Name == "Stone") {
                        obj.MinutesUntilReady = 0;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ResourceClump), "performToolAction")]
    internal class PrismaticPerformToolActionResourceClump {
        static void Prefix(ref ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location) {
            if (t is Axe && t.UpgradeLevel == 5 && (__instance.parentSheetIndex.Value == 600 || __instance.parentSheetIndex.Value == 602)) {
                __instance.health.Value = 0;
            }
        }
    }

    [HarmonyPatch(typeof(Tool), "tilesAffected")]
    internal class PrismaticTilesAffected {
        static void Postfix(ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who) {
            if (power >= 6) {
                __result.Clear();
                Vector2 direction;
                Vector2 orth;
                int radius = ModEntry.Config.PrismaticToolWidth;
                int length = ModEntry.Config.PrismaticToolLength;
                switch (who.FacingDirection) {
                    case 0: direction = new Vector2(0, -1); orth = new Vector2(1, 0); break;
                    case 1: direction = new Vector2(1, 0);  orth = new Vector2(0, 1); break;
                    case 2: direction = new Vector2(0, 1);  orth = new Vector2(-1, 0); break;
                    case 3: direction = new Vector2(-1, 0); orth = new Vector2(0, -1); break;
                    default: direction = new Vector2(0, 0); orth = new Vector2(0, 0); break;
                }
                for (int i = 0; i < length; i++) {
                    __result.Add(direction * i + tileLocation);
                    for (int j = 1; j <= radius; j++) {
                        __result.Add(direction * i + orth * j + tileLocation);
                        __result.Add(direction * i + orth * -j + tileLocation);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Tool), "get_Name")]
    internal class PrismaticGetName {
        public static bool Prefix(Tool __instance, ref string __result) {
            if (__instance.UpgradeLevel == 5) {

                switch (__instance.BaseName) {
                    case "Axe": __result = ModEntry.ModHelper.Translation.Get("prismaticAxe"); break;
                    case "Pickaxe": __result = ModEntry.ModHelper.Translation.Get("prismaticPickaxe"); break;
                    case "Watering Can": __result = ModEntry.ModHelper.Translation.Get("prismaticWatercan"); break;
                    case "Hoe": __result = ModEntry.ModHelper.Translation.Get("prismaticHoe"); break;
                }
                //__result = "Prismatic " + __instance.BaseName;
                //__result = ModEntry.ModHelper.Translation.Get("prismatic.prefix") + " " + Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.1");
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), "get_DisplayName")]
    internal static class CobaltDisplayNameHook {
        public static bool Prefix(Tool __instance, ref string __result) {
            if (__instance.UpgradeLevel == 5) {
                __result = __instance.Name;
                return false;
            }
            return true;
        }
    }
}
