using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using Harmony;

namespace ShadowFestival
{
    internal class HarmonyPatcher
    {
        private static IMonitor Monitor;
        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            HarmonyPatcher.Monitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
                prefix: new HarmonyMethod(typeof(HarmonyPatcher), nameof(HarmonyPatcher.Prefix_takeDamage))
                );
        }
 
        private static bool Prefix_takeDamage(Farmer __instance, int damage, bool overrideParry, Monster damager)
        {
            Monitor.VerboseLog($"Farmer taking damge of {damage} from {damager.displayName}");

            if (__instance.hat.Value != null &&
                ModEntry.Data.CalmingHats.Contains(__instance.hat.Value.Name) &&
                (damager is ShadowBrute || damager is ShadowShaman || damager is ShadowGuy || damager is ShadowGirl))
            {
                Monitor.VerboseLog($"Farmer wearing hat {__instance.hat.Value.Name} and damage will be nullified.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
