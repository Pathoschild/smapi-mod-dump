/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/ChildToNPC
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;

namespace ChildToNPC.Patches
{
    /* Postfix for arriveAtFarmHouse
     * This code is directly translated from the original method
     * because the original method would immediately kick out non-married NPCs.
     */
    class NPCArriveAtFarmHousePatch
    {
        public static void Postfix(NPC __instance, FarmHouse farmHouse)
        {
            if (!ModEntry.IsChildNPC(__instance))
                return;

            __instance.setTilePosition(farmHouse.getEntryLocation());
            __instance.ignoreScheduleToday = true;
            __instance.temporaryController = null;
            __instance.controller = null;
            
            if(ModEntry.Config.DoChildrenHaveCurfew && Game1.timeOfDay >= ModEntry.Config.CurfewTime)
            {
                Point bedPoint = new Point((int)__instance.DefaultPosition.X / 64, (int)__instance.DefaultPosition.Y / 64);
                __instance.controller = new PathFindController(__instance, farmHouse, bedPoint, 2);
                //__instance.controller = new PathFindController(__instance, farmHouse, bedPoint, 0, new PathFindController.endBehavior(FarmHouse.spouseSleepEndFunction));
            }
            else
            {
                __instance.controller = new PathFindController(__instance, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 2);
            }

            if(__instance.controller.pathToEndPoint == null)
            {
                __instance.willDestroyObjectsUnderfoot = true;
                __instance.controller = new PathFindController(__instance, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 0);
                //__instance.setNewDialogue(Game1.LoadStringByGender(__instance.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4500"), false, false);
            }

            if (Game1.currentLocation == farmHouse)
                Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
        }
    }
}