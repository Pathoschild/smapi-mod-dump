/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Locations;


namespace LittleNPCs.Framework.Patches {
    /// <summary>
    /// Postfix for <code>PathFindController.prepareToDisembarkOnNewSchedulePath</code>.
    /// When the postfix comes around, the original will have executed these lines:
    /// __instance.finishEndOfRouteAnimation();
    /// __instance.doingEndOfRouteAnimation.Value = false;
    /// __instance.currentDoingEndOfRouteAnimation = false;
    /// then it returns because the NPC isn't married.
    /// This postfix executes the rest of the code that was skipped.
    /// </summary>
    class NPCPrepareToDisembarkOnNewSchedulePathPatch {
        public static void Postfix(NPC __instance) {
            if (__instance is not LittleNPC) {
                return;
            }
            
            if (Utility.getGameLocationOfCharacter(__instance) is FarmHouse) {
                __instance.temporaryController = new PathFindController(__instance, __instance.getHome(), new Point(__instance.getHome().warps[0].X, __instance.getHome().warps[0].Y), 2, true) {
                    NPCSchedule = true
                };
                if (__instance.temporaryController.pathToEndPoint is null || __instance.temporaryController.pathToEndPoint.Count <= 0) {
                    __instance.temporaryController = null;
                    __instance.Schedule = null;
                }
                else {
                    __instance.followSchedule = true;
                }
            }
            else if (Utility.getGameLocationOfCharacter(__instance) is Farm) {
                __instance.temporaryController = null;
                __instance.Schedule = null;
            }
        }
    }
}
