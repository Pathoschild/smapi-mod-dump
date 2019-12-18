using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace ChildToNPC.Patches
{
    /* Postfix for prepareToDisembarkOnNewSchedulePath
     * When the postfix comes around, the original will have executed these lines:
     * -->__instance.finishEndOfRouteAnimation()
     * -->__instance.doingEndOfRouteAnimation.Value = false;
     * -->__instance.currentDoingEndOfRouteAnimation = false;
     * then it returns because the NPC isn't married.
     * This postfix executes the rest of the code that was skipped.
     */
    class NPCPrepareToDisembarkOnNewSchedulePathPatch
    {
        public static void Postfix(NPC __instance)
        {
            if (!ModEntry.IsChildNPC(__instance))
                return;
            
            if(Utility.getGameLocationOfCharacter(__instance) is FarmHouse)
            {
                __instance.temporaryController = new PathFindController(__instance, __instance.getHome(), new Point(__instance.getHome().warps[0].X, __instance.getHome().warps[0].Y), 2, true)
                {
                    NPCSchedule = true
                };
                if (__instance.temporaryController.pathToEndPoint == null || __instance.temporaryController.pathToEndPoint.Count <= 0)
                {
                    __instance.temporaryController = null;
                    __instance.Schedule = null;
                }
                else
                    __instance.followSchedule = true;
            }
            else if(Utility.getGameLocationOfCharacter(__instance) is Farm)
            {
                __instance.temporaryController = null;
                __instance.Schedule = null;
            }
        }
    }
}