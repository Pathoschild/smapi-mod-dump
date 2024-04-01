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
using StardewValley.Buffs;

namespace CopperStill.ModPatches {
    internal static class TipsyBuff {
        public static void Register(IModHelper helper) {
            var harmony = new Harmony(helper.ModContent.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(BuffManager), "Apply"),
                prefix: new HarmonyMethod(typeof(TipsyBuff), nameof(Prefix_BuffManager_Apply))
            );
        }

        private static bool Prefix_BuffManager_Apply(
            BuffManager __instance, Buff buff
        ) {
            if (buff.id != Buff.tipsy &&
                (buff.source.Contains("Brandy") || buff.source.Contains("Vodka") || buff.source.Contains("Gin")
                || buff.source.Contains("Tequila") || buff.source.Contains("Moonshine") || buff.source.Contains("Whiskey")
                || buff.source.Contains("Rum") || buff.source.Contains("Soju") || buff.source.Contains("Sake"))
            ) {
                __instance.Apply(new Buff(Buff.tipsy));
                return false;
            }
            return true;
        }
    }
}
