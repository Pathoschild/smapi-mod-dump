/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using StardewValley;

namespace NoIndoorDismount {
    internal class ModEntry : Mod {

        public static ModConfig? Config;
        public static IMonitor? SMonitor;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            SMonitor = Monitor;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.ShouldDismountOnWarp)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Game1_ShouldDismountOnWarp_Postfix))
            );
        }

        static void Game1_ShouldDismountOnWarp_Postfix(GameLocation __1, ref bool __result) { // __1 is the index equivalent of new_location from original method
            bool dismount = __1.IsOutdoors || __1.treatAsOutdoors.Value; // check if new_location is outdoors or treated as outdoors
            __result = !dismount; // set the result that's passed back to the original call
        }                         // true causes dismount and we don't want to dismount in places treated as outdoors

    }
}
