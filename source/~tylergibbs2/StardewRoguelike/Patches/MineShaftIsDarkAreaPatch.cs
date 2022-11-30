/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "isDarkArea")]
    internal class MineShaftIsDarkAreaPatch
    {
        public static bool Prefix(MineShaft __instance, ref bool __result)
        {
            bool loadedDarkArea = (bool)__instance.GetType().GetField("loadedDarkArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(__instance)!;

            if (loadedDarkArea)
                __result = true;

            return false;
        }
    }
}
