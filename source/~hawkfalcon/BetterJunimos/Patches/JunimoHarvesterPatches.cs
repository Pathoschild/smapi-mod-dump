using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Characters;
using Harmony;
using BetterJunimos.Utils;
using System;
using System.Linq;

namespace BetterJunimos.Patches {
    /* foundCropEndFunction
     * 
     * Is there an action to perform at the end of this pathfind?
     * Completely replace
     */
    public class PatchFindingCropEnd {
        public static bool Prefix(JunimoHarvester __instance, ref PathNode currentNode, ref bool __result) {
            __result = Util.Abilities.IsActionable(new Vector2(currentNode.x, currentNode.y), Util.GetHutIdFromJunimo(__instance));

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
        public static bool Prefix(JunimoHarvester __instance) {
            Vector2 pos = __instance.getTileLocation();
            var harvestTimer = Util.Reflection.GetField<int>(__instance, "harvestTimer");
            // avoid flowers, etc (todo: move)
            if (Util.ShouldAvoidHarvesting(pos)) {
                harvestTimer.SetValue(0);
                __instance.jumpWithoutSound();
                __instance.pathfindToNewCrop();
                return false;
            }
            int time = Util.Config.JunimoImprovements.WorkFaster ? 300 : 998;
            Guid id = Util.GetHutIdFromJunimo(__instance);

            JunimoAbility junimoAbility = Util.Abilities.IdentifyJunimoAbility(pos, id);
            // Use the update() harvesting
            if (junimoAbility == JunimoAbility.HarvestCrops) {
                time = 2000;
            }
            else if (junimoAbility != JunimoAbility.None) {
                if (!Util.Abilities.PerformAction(junimoAbility, id, pos, __instance)) {
                    // didn't succeed, move on
                    time = 0;
                }
            }
            else {
                __instance.pokeToHarvest();
            }
            harvestTimer.SetValue(time);

            return false;
        }
    }

    // pokeToHarvest
    // Reduce chance of doing nothing
    //public class PatchPokeToHarvest {
    //    public static void Postfix(JunimoHarvester __instance) {
    //        var harvestTimer = Util.Reflection.GetField<int>(__instance, "harvestTimer");
    //        if (harvestTimer.GetValue() <= 0 && (Util.Config.JunimoImprovements.WorkFaster || Game1.random.NextDouble() >= 0.3)) {
    //            __instance.pathfindToNewCrop();
    //        }
    //    }
    //}

    // update
    // Animate & handle action timer 
    public class PatchJunimoShake {
        public static void Postfix(JunimoHarvester __instance) {
            var harvestTimer = Util.Reflection.GetField<int>(__instance, "harvestTimer");
            int time = harvestTimer.GetValue();
            if (Util.Config.JunimoImprovements.WorkFaster && time == 999) {
                // skip last second of harvesting if faster
                harvestTimer.SetValue(0);
                //__instance.pokeToHarvest();
            }
            else if (time > 500 && time < 1000 || (Util.Config.JunimoImprovements.WorkFaster && time > 5)) {
                __instance.shake(50);
            }
        }
    }

    // pathfindToRandomSpotAroundHut
    // Expand radius of random pathfinding
    public class PatchPathfind {
        public static void Postfix(JunimoHarvester __instance) {
            JunimoHut hut = Util.GetHutFromId(Util.GetHutIdFromJunimo(__instance));
            int radius = Util.MaxRadius;
            __instance.controller = new PathFindController(__instance, __instance.currentLocation, Utility.Vector2ToPoint(
                new Vector2((float)(hut.tileX.Value + 1 + Game1.random.Next(-radius, radius + 1)), (float)(hut.tileY.Value + 1 + Game1.random.Next(-radius, radius + 1)))),
                -1, new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100);
        }
    }

    // pathFindToNewCrop_doWork - completely replace 
    // Remove the max distance boundary
    [HarmonyPriority(Priority.Low)]
    public class PatchPathfindDoWork {
        
        public static bool Prefix(JunimoHarvester __instance) {
            JunimoHut hut = Util.GetHutFromId(Util.GetHutIdFromJunimo(__instance));
            if (Game1.timeOfDay > 1900 && !Util.Config.JunimoImprovements.CanWorkInEvenings) {
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
                    Util.AnimateJunimo(7, __instance);
                }
            }
            else if (Game1.random.NextDouble() < 0.035 || hut.noHarvest) {
                __instance.pathfindToRandomSpotAroundHut();
            }
            else {
                __instance.controller = new PathFindController(__instance, __instance.currentLocation, 
                    new PathFindController.isAtEnd(__instance.foundCropEndFunction), -1, false, 
                    new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100, Point.Zero);
            
                int radius = Util.MaxRadius;
                if (__instance.controller.pathToEndPoint == null ||
                    Math.Abs(__instance.controller.pathToEndPoint.Last().X - hut.tileX) + 1 > radius ||
                    Math.Abs(__instance.controller.pathToEndPoint.Last().Y - hut.tileY) + 1 > radius) {
                    if (Game1.random.NextDouble() < 0.5 && !hut.lastKnownCropLocation.Equals(Point.Zero)) {
                        __instance.controller = new PathFindController(__instance, __instance.currentLocation, hut.lastKnownCropLocation, -1,
                            new PathFindController.endBehavior(__instance.reachFirstDestinationFromHut), 100);
                    }
                    else if (Game1.random.NextDouble() < 0.25) {
                        Util.AnimateJunimo(0, __instance);
                        __instance.returnToJunimoHut(__instance.currentLocation);
                    }
                    else {
                        __instance.pathfindToRandomSpotAroundHut();
                    }
                }
                else {
                    Util.AnimateJunimo(0, __instance);
                }
            }
            return false;
        }
    }
}
