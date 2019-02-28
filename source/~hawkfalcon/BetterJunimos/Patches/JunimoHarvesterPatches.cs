using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Characters;
using Harmony;
using BetterJunimos.Utils;
using System;
using System.Linq;
using Netcode;
using BetterJunimos.Abilities;

namespace BetterJunimos.Patches {
    /* foundCropEndFunction
     * 
     * Is there an action to perform at the end of this pathfind?
     * Completely replace
     */
    public class PatchFindingCropEnd {
        public static bool Prefix(JunimoHarvester __instance, ref PathNode currentNode, ref NetGuid ___netHome, ref bool __result) {
            __result = Util.Abilities.IsActionable(new Vector2(currentNode.x, currentNode.y), ___netHome.Value);

            return false;
        }
    }

    /* tryToHarvestHere
     * 
     * Try to perform ability
     * Except harvest
     * Completely replace
     *
     */
    public class PatchHarvestAttemptToCustom {
        public static bool Prefix(JunimoHarvester __instance, ref int ___harvestTimer, ref NetGuid ___netHome) {
            Guid id = ___netHome.Value;
            Vector2 pos = __instance.getTileLocation();

            int time;
            IJunimoAbility junimoAbility = Util.Abilities.IdentifyJunimoAbility(pos, id);
            if (junimoAbility != null) {
                // Use the update() harvesting
                if (junimoAbility.AbilityName() == "HarvestCrops") {
                    time = 2000;
                } else if (!Util.Abilities.PerformAction(junimoAbility, id, pos, __instance)) {
                    // didn't succeed, move on
                    time = 0;
                } else {
                    // succeeded, shake
                    time = Util.Config.JunimoImprovements.WorkFaster ? 300 : 998;
                }
            }
            else {
                // nothing to do, wait a moment
                time = Util.Config.JunimoImprovements.WorkFaster ? 5 : 200;
                __instance.pokeToHarvest();
            }
            ___harvestTimer = time;

            return false;
        }
    }

    // update
    // Animate & handle action timer 
    public class PatchJunimoShake {
        public static void Postfix(JunimoHarvester __instance, ref int ___harvestTimer) {
            if (Util.Config.JunimoImprovements.WorkFaster && ___harvestTimer == 999) {
                // skip last second of harvesting if faster
                ___harvestTimer = 0;
            }
            else if (___harvestTimer > 500 && ___harvestTimer < 1000 || (Util.Config.JunimoImprovements.WorkFaster && ___harvestTimer > 5)) {
                __instance.shake(50);
            }
        }
    }

    // pathfindToRandomSpotAroundHut
    // Expand radius of random pathfinding
    public class PatchPathfind {
        public static void Postfix(JunimoHarvester __instance, ref NetGuid ___netHome) {
            JunimoHut hut = Util.GetHutFromId(___netHome.Value);
            int radius = Util.MaxRadius;
            int retry = 0;
            do {
                __instance.controller = new PathFindController(__instance, __instance.currentLocation, Utility.Vector2ToPoint(
                    new Vector2((float)(hut.tileX.Value + 1 + Game1.random.Next(-radius, radius + 1)), (float)(hut.tileY.Value + 1 + Game1.random.Next(-radius, radius + 1)))),
                    -1, new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100);
                retry++;
            } while (retry <= 5 && (__instance.controller == null || __instance.controller.pathToEndPoint == null));
        }
    }

    // pathFindToNewCrop_doWork - completely replace 
    // Remove the max distance boundary
    [HarmonyPriority(Priority.Low)]
    public class PatchPathfindDoWork {
        
        public static bool Prefix(JunimoHarvester __instance, ref NetGuid ___netHome, ref NetEvent1Field<int, NetInt> ___netAnimationEvent) {
            JunimoHut hut = Util.GetHutFromId(___netHome.Value);
            int time = Util.Config.JunimoImprovements.CanWorkInEvenings ? 2400 : 1900;
            if (Game1.timeOfDay > time) {
                if (__instance.controller != null)
                    return false;
                __instance.returnToJunimoHut(__instance.currentLocation);
            }
            // Prevent working when not paid
            else if (Util.Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday) {
                if (Game1.random.NextDouble() < 0.02) {
                    __instance.pathfindToRandomSpotAroundHut();
                }
                else {
                    // go on strike
                    ___netAnimationEvent.Fire(7);
                }
            }
            else if (Game1.random.NextDouble() < 0.035 || hut.noHarvest.Value) {
                __instance.pathfindToRandomSpotAroundHut();
            }
            else {
                __instance.controller = new PathFindController(__instance, __instance.currentLocation, 
                    new PathFindController.isAtEnd(__instance.foundCropEndFunction), -1, false, 
                    new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100, Point.Zero);
            
                int radius = Util.MaxRadius;
                if (__instance.controller.pathToEndPoint == null ||
                    Math.Abs(__instance.controller.pathToEndPoint.Last().X - hut.tileX.Value + 1) > radius ||
                    Math.Abs(__instance.controller.pathToEndPoint.Last().Y - hut.tileY.Value + 1) > radius) {
                    if (Game1.random.NextDouble() < 0.5 && !hut.lastKnownCropLocation.Equals(Point.Zero)) {
                        __instance.controller = new PathFindController(__instance, __instance.currentLocation, hut.lastKnownCropLocation, -1,
                            new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100);
                        // refresh lastKnownCropLocation
                        hut.areThereMatureCropsWithinRadius();
                    }
                    else if (Game1.random.NextDouble() < 0.25) {
                        ___netAnimationEvent.Fire(0);
                        __instance.returnToJunimoHut(__instance.currentLocation);
                    }
                    else {
                        __instance.pathfindToRandomSpotAroundHut();
                    }
                }
                else {
                    ___netAnimationEvent.Fire(0);
                }
            }
            return false;
        }
    }
}