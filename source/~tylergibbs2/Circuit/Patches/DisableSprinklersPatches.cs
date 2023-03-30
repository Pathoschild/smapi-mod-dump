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

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.ApplySprinkler))]
    internal class ApplySprinklerPatch
    {
        public static bool Prefix()
        {
            if (!ModEntry.ShouldPatch())
                return true;

            if (EventManager.EventIsActive(EventType.WaterShortage))
                return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(SObject), nameof(SObject.ApplySprinklerAnimation))]
    internal class ApplySprinklerAnimationPatch
    {
        public static bool Prefix()
        {
            if (!ModEntry.ShouldPatch())
                return true;

            if (EventManager.EventIsActive(EventType.WaterShortage))
                return false;

            return true;
        }
    }
}
