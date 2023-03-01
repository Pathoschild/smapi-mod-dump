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

using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;


namespace LittleNPCs.Framework.Patches {
     /// <summary>
     /// Postfix for <code>NPC.arriveAtFarmHouse</code>.
     /// This code is directly translated from the original method
     /// because the original method would immediately kick out non-married NPCs.
     /// </summary>
    class NPCArriveAtFarmHousePatch {
        public static void Postfix(NPC __instance, FarmHouse farmHouse) {
            if (__instance is not LittleNPC) {
                return;
            }

            if (ModEntry.config_.DoChildrenHaveCurfew && Game1.timeOfDay >= ModEntry.config_.CurfewTime) {
                Point bedPoint = Utility.Vector2ToPoint(__instance.DefaultPosition / 64f);

                // If farmer is not here move to bed instantly because path finding will be cancelled when the farmer enters the house.
                // Note that we need PathFindController even in this case because setTilePosition(bedPoint) places the NPC next to bed.
                if (Game1.player.currentLocation != farmHouse) {
                    __instance.setTilePosition(bedPoint);
                }
                else {
                    __instance.setTilePosition(farmHouse.getEntryLocation());
                }

                __instance.ignoreScheduleToday = true;
                __instance.temporaryController = null;
                // In order to make path finding work we must assign null first.
                __instance.controller = null;

                __instance.controller = new PathFindController(__instance, farmHouse, bedPoint, 2);
            }
            else {
                __instance.setTilePosition(farmHouse.getEntryLocation());

                __instance.ignoreScheduleToday = true;
                __instance.temporaryController = null;
                __instance.controller = null;

                __instance.controller = new PathFindController(__instance, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 2);
            }

            if (__instance.controller.pathToEndPoint is null) {
                __instance.willDestroyObjectsUnderfoot = true;
                __instance.controller = new PathFindController(__instance, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 0);
            }

            if (Game1.currentLocation == farmHouse) {
                Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
            }
        }
    }
}
