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
            
            __instance.setTilePosition(farmHouse.getEntryLocation());
            __instance.temporaryController = null;
            //__instance.controller = null;

            //normally endPoint is Game1.timeOfDay >= 2130 ? farmHouse.getSpouseBedSpot() : farmHouse.getKitchenStandingSpot()
            //this is normally a controller, not temporaryController (test?)
            if(ModEntry.Config.DoChildrenHaveCurfew && Game1.timeOfDay >= ModEntry.Config.CurfewTime)//700 pm by default
            {
                Point bedPoint = new Point((int)__instance.DefaultPosition.X / 64, (int)__instance.DefaultPosition.Y / 64);
                __instance.temporaryController = new PathFindController(__instance, farmHouse, bedPoint, 2);
            }
            else
            {
                __instance.temporaryController = new PathFindController(__instance, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 2);
            }

            if (Game1.currentLocation is FarmHouse)
                Game1.currentLocation.playSound("doorClose");
        }
    }
}