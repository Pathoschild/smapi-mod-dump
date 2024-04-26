/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;

namespace OrnithologistsGuild
{
    public class GameLocationPatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.addBirdies)),
               prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(addBirdies_Prefix))
            );
        }

        public static bool addBirdies_Prefix(StardewValley.GameLocation __instance, double chance, bool onlyIfOnScreen = false)
        {
            try
            {
                Monitor.Log($"{nameof(addBirdies_Prefix)}: chance={chance.ToString()} onlyIfOnScreen={onlyIfOnScreen.ToString()}");

                BetterBirdieSpawner.AddBirdies(__instance, chance, !onlyIfOnScreen /* for some reason this is inverted in the original game code */);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(addBirdies_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}

