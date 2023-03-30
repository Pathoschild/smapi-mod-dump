/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CopperStill.ModPatches {
    internal static class TipsyBuff {
        private const int Tipsy = 17;

        public static void Register(IModHelper helper) {
            var harmony = new Harmony(helper.ModContent.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(BuffsDisplay), "tryToAddDrinkBuff"),
                prefix: new HarmonyMethod(typeof(TipsyBuff), nameof(Prefix_TryToAddDrinkBuff))
            );
        }

        private static bool Prefix_TryToAddDrinkBuff(BuffsDisplay __instance, Buff b) {
            if (b.source.Contains("Brandy") || b.source.Contains("Vodka") || b.source.Contains("Gin")
                || b.source.Contains("Tequila") || b.source.Contains("Moonshine") || b.source.Contains("Whiskey")
            ) {
                __instance.addOtherBuff(new Buff(Tipsy));
                return false;
            }
            return true;
        }
    }
}
