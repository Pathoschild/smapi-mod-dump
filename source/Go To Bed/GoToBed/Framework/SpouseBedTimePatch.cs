/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/GoToBed
**
*************************************************/

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using HarmonyLib;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Pathfinding;


namespace GoToBed.Framework {
    /// <summary>
    /// </summary>
    internal static class SpouseBedTimePatch {
        private static IMonitor monitor_;

        private static SpouseBedTimeVerifier spouseBedTime_;

        private static Point initialSpousePosition_;

        private static int initialSpouseFacingDirection_;

        /// <summary>
        /// Creates Harmony patch.
        /// </summary>
        /// <param name="uniqueID">Unique identifier.</param>
        public static void Create(string uniqueID, IMonitor monitor, SpouseBedTimeVerifier spouseBedTime) {
            monitor_ = monitor;
            spouseBedTime_ = spouseBedTime;

            monitor_.Log("Create Harmony patch");
            monitor_.Log(string.Join(" ", $"Set SpouseGetUpTime to {SpouseBedTimeVerifier.DefaultGetUpTime}",
                                          $"and SpouseGoToBedTime to {SpouseBedTimeVerifier.DefaultGoToBedTime}",
                                          "in config.json to disable it if you experience problems"));

            Harmony harmony = new Harmony(uniqueID);

            // If get up time was not modified there's no need to patch NPC.marriageDuties .
            if (spouseBedTime.GetUpTime > SpouseBedTimeVerifier.DefaultGetUpTime) {
                MethodInfo    original1 = typeof(NPC).GetMethod(nameof(NPC.marriageDuties));
                HarmonyMethod postfix1  = new HarmonyMethod(typeof(SpouseBedTimePatch),
                                                            nameof(SpouseBedTimePatch.Postfix_NPC_marriageDuties));
                harmony.Patch(original1, null, postfix1);
            }

            MethodInfo    original2 = typeof(FarmHouse).GetMethod(nameof(FarmHouse.performTenMinuteUpdate));
            HarmonyMethod prefix2   = new HarmonyMethod(typeof(SpouseBedTimePatch),
                                                        nameof(SpouseBedTimePatch.Prefix_FarmHouse_performTenMinuteUpdate));
            harmony.Patch(original2, prefix2, null);

            MethodInfo    original3 = typeof(FarmHouse).GetMethod("resetSharedState", (BindingFlags.NonPublic | BindingFlags.Instance));
            HarmonyMethod transpiler3 = new HarmonyMethod(typeof(SpouseBedTimePatch),
                                                                 nameof(SpouseBedTimePatch.Transpiler_FarmHouse_resetSharedState));
            harmony.Patch(original3, null, null, transpiler3);
            
        }

        /// <summary>
        /// Patch method that runs after NPC.marriageDuties() .
        /// Puts your spouse back into bed.
        /// </summary>
        private static void Postfix_NPC_marriageDuties(NPC __instance) {
            Farmer spouse = __instance.getSpouse();
            if (spouse == null) {
                return;
            }
            
            FarmHouse farmHouse = Game1.getLocationFromName(spouse.homeLocation.Value) as FarmHouse;
            if (!__instance.currentLocation.Equals(farmHouse)) {
                monitor_.Log($"{__instance.Name} is not in the farm house, get up time not modified", LogLevel.Info);

                return;
            }

            // New day has started, everything is set up, spouse already left bed.
            // Before we put him/her back into bed we have to remember current position.
            initialSpousePosition_        = __instance.TilePoint;
            initialSpouseFacingDirection_ = __instance.FacingDirection;
            // Every day starts in bed.
            __instance.setTilePosition(farmHouse.getSpouseBedSpot(__instance.Name));
            __instance.doEmote(Character.sleepEmote);
            monitor_.Log($"{__instance.Name} is in bed now");
        }

        /// <summary>
        /// Patch method that controls bed time and corresponding spouse behavior.
        /// </summary>
        private static bool Prefix_FarmHouse_performTenMinuteUpdate(FarmHouse __instance, ref int timeOfDay) {
            NPC spouse = __instance.characters.FirstOrDefault(c => c.isMarried());
            if (!Game1.IsMasterGame || spouse == null) {
                // Enable original method.
                return true;
            }

            // Full and half hours after 2200 make spouse go to bed so we have to call the method with a modified time.
            if (timeOfDay >= SpouseBedTimeVerifier.DefaultGoToBedTime && IsFullOrHalfHour(timeOfDay) && timeOfDay != spouseBedTime_.GoToBedTime) {
                monitor_.Log($"Call FarmHouse.performTenMinuteUpdate({timeOfDay + 1}) to prevent sleeping");

                timeOfDay += 1;

                // Enable original method.
                return true;
            }

            if (timeOfDay == spouseBedTime_.GetUpTime) {
                // Get up.
                SpouseFindPath(
                    __instance,
                    spouse,
                    initialSpousePosition_,
                    initialSpouseFacingDirection_,
                    (c, location) => {
                        monitor_.Log($"{c.Name} got up");
                    },
                    false
                );
            }
            else if (timeOfDay == spouseBedTime_.GoToBedTime) {
                // Go to bed.
                SpouseFindPath(
                    __instance,
                    spouse,
                    __instance.getSpouseBedSpot(spouse.Name),
                    0,
                    (c, location) => {
                        c.doEmote(Character.sleepEmote);
                        FarmHouse.spouseSleepEndFunction(c, location);

                        monitor_.Log($"{c.Name} went to bed");
                    },
                    true
                );
            }

            // Enable original method.
            return true;
        }

        /// <summary>
        /// Patch method that controls bed time and corresponding spouse behavior.
        /// </summary>
        private static IEnumerable<CodeInstruction> Transpiler_FarmHouse_resetSharedState(IEnumerable<CodeInstruction> instructions) {
            bool found = false;

            // Patching resetSharedState() in the desired way would mean to use a prefix to disable the original
            // and replace the code by an almost exact copy just to be able to change a single constant in the code.
            // This would be further complicated by the necessity to call its base method.
            // That's why a transpiler is the best way to deal with the situation.
            // The time when spouse goes to bed is hardcoded to 2200 so we look for the instruction
            // ldc.i4 2200
            // and replace it by the instructions
            // ldsfld GoToBed.Framework.SpouseBedTimeVerifier spouseBedTime_
            // callvirt int get_GoToBedTime()
            // Fortunately there's only one constant with the value 2200 in the method.
            var baseInstruction = new CodeInstruction(OpCodes.Ldc_I4, 2200);
            foreach (var instruction in instructions) {
                if (instruction.opcode == baseInstruction.opcode && instruction.operand.Equals(baseInstruction.operand)) {
                    found = true;

                    var newInstruction1
                        = new CodeInstruction(OpCodes.Ldsfld, typeof(SpouseBedTimePatch).GetField("spouseBedTime_", BindingFlags.NonPublic | BindingFlags.Static));
                    var newInstruction2
                        = new CodeInstruction(OpCodes.Callvirt, typeof(SpouseBedTimeVerifier).GetProperty("GoToBedTime").GetGetMethod());

                    yield return newInstruction1;
                    yield return newInstruction2;

                    monitor_.Log($"Instruction {instruction} found and replaced");
                }
                else {
                    yield return instruction;
                }
            }

            if (!found) {
                monitor_.Log($"Instruction {baseInstruction} not found", LogLevel.Error);
            }
        }

        private static bool IsFullOrHalfHour(int timeOfDay) {
            return timeOfDay % 100 % 30 == 0;
        }

        private static void SpouseFindPath(FarmHouse farmHouse,
                                           NPC npc,
                                           Point endPoint,
                                           int finalFacingDirection,
                                           PathFindController.endBehavior endBehaviorFunction,
                                           bool clearMarriageDialogues) {
            Point currentPosition = npc.TilePoint;
            if (npc.currentLocation.Equals(farmHouse) && currentPosition != endPoint) {
                // To set clearMarriageDialogues we need the most complicated constructor...
                npc.controller = new PathFindController(
                                     npc,
                                     farmHouse,
                                     PathFindController.isAtEndPoint,
                                     finalFacingDirection,
                                     endBehaviorFunction,
                                     10000,
                                     endPoint,
                                     clearMarriageDialogues
                                 );
            }
        }
    }
}
