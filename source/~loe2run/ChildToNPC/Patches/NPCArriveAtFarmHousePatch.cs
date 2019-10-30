using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace ChildToNPC.Patches
{
    /* Postfix for arriveAtFarmHouse
     * This code is directly translated from the original method
     * because the original method would immediately kick out non-married NPCs.
     */
    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("arriveAtFarmHouse")]
    class NPCArriveAtFarmHousePatch
    {
        public static void Postfix(NPC __instance, FarmHouse farmHouse)
        {
            if (!ModEntry.IsChildNPC(__instance))
                return; 

            if (Game1.newDay || Game1.timeOfDay <= 630)
                return;
            
            __instance.setTilePosition(farmHouse.getEntryLocation());
            __instance.temporaryController = null;
            __instance.controller = null;

            //normally endPoint is Game1.timeOfDay >= 2130 ? farmHouse.getSpouseBedSpot() : farmHouse.getKitchenStandingSpot()
            //I want to come back and find a better solution for this
            if(Game1.timeOfDay >= 2030)//830 pm
            {
                int birthNumber = 1;
                foreach (NPC childCopy in ModEntry.copies.Values)
                {
                    if (!__instance.Equals(childCopy))
                        birthNumber++;
                    else
                        return;
                }
                string bedSpot = ModEntry.GetBedSpot(birthNumber);
                int bedX = int.Parse(bedSpot.Substring(0, bedSpot.IndexOf(" ")));
                int bedY = int.Parse(bedSpot.Substring(bedSpot.IndexOf(" ") + 1, bedSpot.Length));
                __instance.controller = new PathFindController(__instance, farmHouse, new Point(bedX, bedY), 2);
            }
            else
            {
                int pointX = (int) __instance.DefaultPosition.X / 64;
                int pointY = (int) __instance.DefaultPosition.Y / 64;
                __instance.controller = new PathFindController(__instance, farmHouse, new Point(pointX, pointY), 2);
            }

            if (Game1.currentLocation is FarmHouse)
                Game1.currentLocation.playSound("doorClose");
        }
    }
}