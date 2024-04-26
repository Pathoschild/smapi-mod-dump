/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aeremyns/AdoptWithKrobus
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AdoptWithKrobus.Patches
{
    class CanGetPregnantPatch
    {
        public static void Postfix(NPC __instance, ref bool __result)
        {
            NPC spouse = __instance;
            Farmer farmer = spouse.getSpouse();
            

            
            if (spouse is Horse || spouse.IsInvisible)
            {
                return;
            }

            if (farmer == null || farmer.divorceTonight.Equals(new NetBool(true)))
            {
                return;
            }
            int heartsWithSpouse = farmer.getFriendshipHeartLevelForNPC(spouse.Name);
            Friendship friendship = farmer.GetSpouseFriendship();
            List<Child> kids = farmer.getChildren();
            //spouse.defaultMap.Value = farmer.homeLocation.Value;
            FarmHouse farmHouse = Utility.getHomeOfFarmer(farmer);
            if (farmHouse.cribStyle.Value <= 0)
            {
                return;
            }
            if (farmHouse.upgradeLevel >= 2 && friendship.DaysUntilBirthing < 0 && heartsWithSpouse >= 10 && farmer.GetDaysMarried() >= 7)
            {
                if (kids.Count != 0)
                {
                    if (kids.Count < 2)
                    {
                        __result = kids[0].Age > 2;
                    }
                    return;
                }
                __result = true;
            }
        }
    }
}
