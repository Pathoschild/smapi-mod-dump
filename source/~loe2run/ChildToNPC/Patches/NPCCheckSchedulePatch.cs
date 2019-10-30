using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ChildToNPC.Patches
{
    /* Prefix for checkSchedule
     * Normally the previousEndPoint check should be skipped for residents of the FarmHouse,
     * but that's checked for with isMarried() to skip.
     * So I've manually set the previousEndPoint to Point.Zero here to avoid that issue.
     */

    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("checkSchedule")]
    class NPCCheckSchedulePatch
    {
        public static bool Prefix(NPC __instance, ref Point ___previousEndPoint)
        {
            if (ModEntry.IsChildNPC(__instance))
                ___previousEndPoint = new Point(0, 0);

            return true;
        }
    }
}
