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

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(SObject), "performToolAction")]
    internal class ObjectPerformActionPatch
    {
        public static bool Prefix(SObject __instance, ref bool __result)
        {
            // Farm Computer, Deconstructor, Garden Pot, Sprinkler
            if (__instance.ParentSheetIndex == 239 || __instance.ParentSheetIndex == 265 || __instance.ParentSheetIndex == 62 || __instance.ParentSheetIndex == 599)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
