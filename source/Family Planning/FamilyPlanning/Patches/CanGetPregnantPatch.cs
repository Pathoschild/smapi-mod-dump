/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewValley;
using StardewValley.Characters;

namespace FamilyPlanning.Patches
{
    class CanGetPregnantPatch
    {
        public static void Postfix(NPC __instance, ref bool __result)
        {
            if (!Game1.IsMasterGame)
                return;

            int totalChildren = ModEntry.GetFamilyData().TotalChildren;
            NPC spouse = __instance;
            Farmer farmer = spouse.getSpouse();

            //This is from the original method
            //canGetPregnant() is only ever run on a spouse NPC, so this shouldn't be necessary?
            if (spouse is Horse)
                return;

            //Original kicks out Krobus/Roommates
            if (spouse.isRoommate() && !ModEntry.RoommateConfig())
                return;

            if (farmer == null || farmer.divorceTonight.Equals(new NetBool(true)))
                return;
            
            int heartLevelForNPC = farmer.getFriendshipHeartLevelForNPC(spouse.Name);
            Friendship spouseFriendship = farmer.GetSpouseFriendship();
            List<Child> children = farmer.getChildren();
            
            //This is from the original method
            spouse.defaultMap.Value = farmer.homeLocation.Value;

            if (Utility.getHomeOfFarmer(farmer).upgradeLevel < 2 || spouseFriendship.DaysUntilBirthing >= 0 || (heartLevelForNPC < 10 || farmer.GetDaysMarried() < 7))
                return;

            /* Toddlers are 55 daysOld, and pregnancy lasts 14 days,
             * so requiring the previous sibling to be at least 41 days old
             * will ensure that they are out of the crib when the baby is born.
             */
            if (children.Count < totalChildren)
            {
                //If you have 0 children, skips straight to true
                foreach (Child child in children)
                { 
                    if (child.daysOld < 41)
                    {
                        __result = false;
                        return;
                    }
                }
                __result = true;
            }
        }
    }
}