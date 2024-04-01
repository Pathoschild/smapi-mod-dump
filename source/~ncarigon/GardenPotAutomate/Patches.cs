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
using StardewValley;

namespace GardenPotAutomate {
    internal static class Patches {
        private static IMonitor Monitor = null!;
        private static Config Config = null!;

        public static void Register(IModHelper helper, IMonitor monitor, Config config) {
            Monitor = monitor;
            Config = config;
            var harmony = new Harmony(helper.ModContent.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "gainExperience"),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Farmer_gainExperience))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "addItemToInventoryBool"),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Farmer_addItemToInventoryBool))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "createItemDebris"),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Prefix_Game1_createItemDebris))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "get_player"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Postfix_Game1_get_player))
            );
        }

        // track fake player
        internal static Farmer Harvester = null!;

        // swaps in the fake player when harvesting from garden pot routines
        private static void Postfix_Game1_get_player(ref Farmer __result) {
            if (Harvester is null)
                return;
            __result = Harvester;
        }

        // track owner
        internal static Farmer Owner = null!;

        private static void Prefix_Farmer_gainExperience(Farmer __instance, int which, int howMuch) {
            try {
                // in a harvest routine, so the fake player ran the normal logic already
                if (Config.GainExperience && Owner is not null) {
                    if (__instance.UniqueMultiplayerID != Owner.UniqueMultiplayerID) {
                        Owner.gainExperience(which, howMuch);
                    }
                }
            } catch (Exception ex) { Monitor.Log($"Failed to gainExperience. Message: {ex.Message}", LogLevel.Error); }
        }

        // gather harvested items
        internal static Action<Item> Items = null!;

        private static bool Prefix_Farmer_addItemToInventoryBool(ref bool __result, Item item) {
            try {
                if (Items is not null) {
                    Items(item);
                    __result = true;
                    return false;
                }
            } catch (Exception ex) { Monitor.Log($"Failed to addItemToInventoryBool. Message: {ex.Message}", LogLevel.Error); }
            return true;
        }

        private static bool Prefix_Game1_createItemDebris(ref Debris __result, Item item) {
            try {
                if (Items is not null) {
                    Items(item);
                    __result = null!;
                    return false;
                }
            } catch (Exception ex) { Monitor.Log($"Failed to createItemDebris. Message: {ex.Message}", LogLevel.Error); }
            return true;
        }
    }
}
