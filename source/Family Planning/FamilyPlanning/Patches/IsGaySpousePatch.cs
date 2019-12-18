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
