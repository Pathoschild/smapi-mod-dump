/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using StardewValley;

namespace FamilyPlanning.Patches
{
    /* IsGaySpouse triggers in a few places to determine parenting dialogue.
     * Roommates will always adopt children, so I'm patching them as a gay spouse for the sake of adoption dialogue.
     * (By triggering this based on roommate status, and not by being Krobus, this should future-proof this mod.)
     */
     
    class IsGaySpousePatch
    {
        public static void Postfix(NPC __instance, ref bool __result)
        {
            if(ModEntry.RoommateConfig() && __instance.isRoommate())
            {
                __result = true;
                return;
            }
        }
    }
}
