/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using HarmonyLib;
using BetterJunimos.Utils;
using System;
using System.Linq;
using Netcode;
using BetterJunimos.Abilities;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Pathfinding;

namespace BetterJunimos.Patches {
    /* foundCropEndFunction
     * 
     * Is there an action to perform at the end of this pathfind?
     * Completely replace
     */
    public class PatchFindingCropEnd {
        public static bool Prefix(JunimoHarvester __instance, ref PathNode currentNode, ref NetGuid ___netHome,
            ref bool __result) {
            __result = Util.Abilities.IsActionable(__instance.currentLocation,
                new Vector2(currentNode.x, currentNode.y), ___netHome.Value);

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
    public class PatchTryToHarvestHere {
        public static bool Prefix(JunimoHarvester __instance, ref int ___harvestTimer, ref NetGuid ___netHome) {
            if (!Context.IsMainPlayer) return true;
            if (__instance.home is null) {
                //BetterJunimos.SMonitor.Log($"No hut assigned");
                return false;
            }
            var id = ___netHome.Value;
            var pos = __instance.Tile;

            // if (__instance.currentLocation.IsGreenhouse) {
            //     BetterJunimos.SMonitor.Log($"PatchTryToHarvestHere: #{__instance.whichJunimoFromThisHut} looking in {__instance.currentLocation.Name} at [{pos.X} {pos.Y}]", LogLevel.Trace);
            // }

            int time;
            var junimoAbility = Util.Abilities.IdentifyJunimoAbility(__instance.currentLocation, pos, id);
            if (junimoAbility != null) {
                // if (__instance.currentLocation.IsGreenhouse) {
                //     BetterJunimos.SMonitor.Log(
                //         $"PatchTryToHarvestHere: #{__instance.whichJunimoFromThisHut} performing {junimoAbility.AbilityName()} at {__instance.currentLocation.Name} [{pos.X} {pos.Y}]",
                //         LogLevel.Trace);
                // }

                if (junimoAbility is HarvestBushesAbility) {
                    // Use the update() harvesting
                    time = 2000;
                }
                else if (!Util.Abilities.PerformAction(junimoAbility, id, __instance.currentLocation, pos,
                    __instance)) {
                    // didn't succeed, move on
                    time = 0;

                    // add failed action to ability cooldowns
                    Util.Abilities.ActionFailed(__instance.currentLocation, junimoAbility, pos);
                }
                else {
                    // succeeded, shake
                    if (junimoAbility is HarvestCropsAbility) time = 2000;
                    else if (BetterJunimos.Config.JunimoImprovements.WorkRidiculouslyFast) time = 20;
                    else time = Util.Progression.WorkFaster ? 300 : 998;

                    // BetterJunimos.SMonitor.Log($"PatchTryToHarvestHere performing {junimoAbility.AbilityName()} time {time}]", LogLevel.Trace);
                }
            }
            else {
                // nothing to do, wait a moment
                time = Util.Progression.WorkFaster ? 5 : 200;
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
            if (!Context.IsMainPlayer) return;

            if (Util.Progression.WorkFaster && ___harvestTimer == 999) {
                // skip last second of harvesting if faster
                ___harvestTimer = 0;
            }
            else if (___harvestTimer is > 500 and < 1000 || Util.Progression.WorkFaster && ___harvestTimer > 5) {
                __instance.shake(50);
            }
        }
    }

    // pathfindToRandomSpotAroundHut
    // Expand radius of random pathfinding
    public class PatchPathfindToRandomSpotAroundHut {
        public static void Postfix(JunimoHarvester __instance) {
            var hut = __instance.home;
            if (hut is null) return;

            var radius = Util.CurrentWorkingRadius;
            var retry = 0;
            do {
                var endPoint = __instance.currentLocation.IsGreenhouse
                    ? EndPointInGreenhouse(__instance)
                    : EndPointInFarm(hut, radius);

                // BetterJunimos.SMonitor.Log($"PatchPathfindToRandomSpotAroundHut: " +
                //                            $"#{__instance.whichJunimoFromThisHut} " +
                //                            $"in {__instance.currentLocation.Name} " +
                //                            $"from [{__instance.getTileX()} {__instance.getTileX()}] " +
                //                            $"to [{endPoint.X} {endPoint.Y}]",
                //     LogLevel.Debug);

                // BetterJunimos.SMonitor.Log(
                //     $"PatchPathfindToRandomSpotAroundHut: #{__instance.whichJunimoFromThisHut} current location is {__instance.currentLocation.Name}",
                //     LogLevel.Trace);
                // BetterJunimos.SMonitor.Log(
                //     $"PatchPathfindToRandomSpotAroundHut: #{__instance.whichJunimoFromThisHut} want to path to [{endPoint.X} {endPoint.Y}]",
                //     LogLevel.Trace);

                __instance.controller = new PathFindController(
                    __instance,
                    __instance.currentLocation,
                    endPoint,
                    -1,
                    __instance.reachFirstDestinationFromHut,
                    100);
                retry++;
            } while (retry <= 5 && __instance.controller?.pathToEndPoint == null);

            if (__instance.controller == null) {
                // BetterJunimos.SMonitor.Log(
                //     $"PatchPathfindToRandomSpotAroundHut: #{__instance.whichJunimoFromThisHut} controller is null, will be despawned",
                //     LogLevel.Trace);
            }

            else if (__instance.controller?.pathToEndPoint == null) {
                // BetterJunimos.SMonitor.Log(
                //     $"PatchPathfindToRandomSpotAroundHut: #{__instance.whichJunimoFromThisHut} controller.pathToEndPoint is null, will be despawned",
                //     LogLevel.Trace);
            }

            else {
                // BetterJunimos.SMonitor.Log(
                //     $"PatchPathfindToRandomSpotAroundHut: #{__instance.whichJunimoFromThisHut} has a path",
                //     LogLevel.Trace);
            }
        }

        private static Point EndPointInGreenhouse(JunimoHarvester jh) {
            var gw = jh.currentLocation.map.Layers[0].LayerWidth;
            var gh = jh.currentLocation.map.Layers[0].LayerHeight;
            return new Vector2(
                1 + Game1.random.Next(gw - 2),
                1 + Game1.random.Next(gh - 2)
                ).ToPoint();
        }

        private static Point EndPointInFarm(JunimoHut hut, int radius) {
            return Utility.Vector2ToPoint(
                new Vector2(
                    hut.tileX.Value + 1 + Game1.random.Next(-radius, radius + 1),
                    hut.tileY.Value + 1 + Game1.random.Next(-radius, radius + 1)));
        }
    }

    // pathfindToNewCrop - completely replace 
    // Remove the max distance boundary
    [HarmonyPriority(Priority.Low)]
    public class PatchPathfindDoWork {
        public static bool Prefix(JunimoHarvester __instance,
            ref NetEvent1Field<int, NetInt> ___netAnimationEvent) {
            if (!Context.IsMainPlayer) return true;

            var hut = __instance.home;
            if (hut is null) return true;

            var quittingTime = Util.Progression.CanWorkInEvenings ? 2400 : 1900;

            // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: Junimo {__instance.whichJunimoFromThisHut} in {__instance.currentLocation.Name} looking for work", LogLevel.Debug);

            if (Game1.timeOfDay > quittingTime) {
                // bedtime, all Junimos return to huts and/or despawn

                Util.Progression.PromptForCanWorkInEvenings();
                if (__instance.controller != null)
                    return false;

                if (__instance.currentLocation is Farm)
                    __instance.returnToJunimoHut(__instance.currentLocation);
                else {
                    // can't walk back to the hut from here, just despawn
                    __instance.junimoReachedHut(__instance, __instance.currentLocation);
                    // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: despawning due end of day", LogLevel.Trace);
                }
            }

            // Prevent working when not paid
            else if (BetterJunimos.Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday) {
                if (Game1.random.NextDouble() < 0.02 && __instance.currentLocation.IsFarm) {
                    __instance.pathfindToRandomSpotAroundHut();
                }
                else {
                    // go on strike
                    ___netAnimationEvent.Fire(7);
                }
            }

            else if (hut.noHarvest.Value || (Game1.random.NextDouble() < 0.035 && ! BetterJunimos.Config.JunimoImprovements.WorkRidiculouslyFast)) {
                // Hut has nothing to harvest
                // TODO: fix for greenhouse
                
                // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} hut noHarvest {hut.noHarvest.Value}", LogLevel.Debug);

                __instance.pathfindToRandomSpotAroundHut();
            }

            else {
                // walk to work?

                __instance.controller = new PathFindController(
                    __instance,
                    __instance.currentLocation,
                    __instance.foundCropEndFunction,
                    -1,
                    __instance.reachFirstDestinationFromHut,
                    100,
                    Point.Zero);

                var radius = Util.CurrentWorkingRadius;
                var outsideRadius =
                    __instance.controller.pathToEndPoint is not null &&
                    hut.tileX is not null &&
                    hut.tileY is not null &&
                    __instance.currentLocation is not null &&
                    __instance.currentLocation.NameOrUniqueName == hut.GetParentLocation().NameOrUniqueName && (
                        Math.Abs(__instance.controller.pathToEndPoint.Last().X - hut.tileX.Value - 1) > radius ||
                        Math.Abs(__instance.controller.pathToEndPoint.Last().Y - hut.tileY.Value - 1) > radius
                    );

                if (__instance.controller.pathToEndPoint != null && !outsideRadius) {
                    // Junimo has somewhere to be, let it happen
                    
                    // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} has more work", LogLevel.Debug);
                    ___netAnimationEvent.Fire(0);
                }
                else {
                    // Junimo has no path, or path endpoint is outside the hut radius

                    Util.Abilities.lastKnownCropLocations.TryGetValue((hut, __instance.currentLocation), out var lkc);

                    // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} needs work", LogLevel.Debug);

                    // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: #{__instance.whichJunimoFromThisHut} " +
                    //                            $"has no path, trying to find more work to do, " +
                    //                            $"lkc: {__instance.currentLocation.Name} [{lkc.X} {lkc.Y}]",
                    //     LogLevel.Trace);

                    if (Game1.random.NextDouble() < 0.5 && !lkc.Equals(Point.Zero)) {
                        // hut has some work to do, send Junimo there
                        __instance.controller = new PathFindController(
                            __instance,
                            __instance.currentLocation,
                            lkc,
                            -1,
                            __instance.reachFirstDestinationFromHut,
                            100);

                        if (__instance.controller.pathToEndPoint is null) {
                            // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} can't get to work at {lkc}", LogLevel.Debug);

                            // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: #{__instance.whichJunimoFromThisHut} " +
                            //                            $"attempted to path " +
                            //                            $"from {__instance.getTileX()} {__instance.getTileY()} " +
                            //                            $"to {__instance.currentLocation} {lkc.X} {lkc.Y}, " +
                            //                            $"no path to endpoint",
                            //     LogLevel.Trace);
                        }
                    }
                    else if (Game1.random.NextDouble() < 0.25) {
                        
                        // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} being sent home", LogLevel.Debug);

                        // unlucky, send Junimo home
                        ___netAnimationEvent.Fire(0);

                        if (__instance.currentLocation is Farm) {
                            __instance.returnToJunimoHut(__instance.currentLocation);
                        }
                        else if (__instance.currentLocation.IsGreenhouse) {
                            returnToGreenhouseDoor(__instance, __instance.currentLocation);
                        }
                        else {
                            // can't walk back to the hut from here, just despawn
                            __instance.junimoReachedHut(__instance, __instance.currentLocation);
                        }
                    }
                    else {
                        // move Junimo randomly
                        // BetterJunimos.SMonitor.Log($"PatchPathfindDoWork: {__instance.whichJunimoFromThisHut} going for a walk", LogLevel.Debug);

                        __instance.pathfindToRandomSpotAroundHut();
                    }
                }
            }

            return false;
        }

        private static void returnToGreenhouseDoor(JunimoHarvester junimo, GameLocation location) {
            if (Utility.isOnScreen(Utility.Vector2ToPoint(junimo.position.Value / 64f), 64, junimo.currentLocation))
                junimo.jump();

            // junimo.pathfindToNextScheduleLocation;
            junimo.collidesWithOtherCharacters.Value = false;

            if (Game1.IsMasterGame) {
                junimo.controller = new PathFindController(junimo, location, GreenhouseDoor(location), 1,
                    junimo.junimoReachedHut);
                if (junimo.controller.pathToEndPoint == null || junimo.controller.pathToEndPoint.Count == 0) {
                    // BetterJunimos.SMonitor.Log(
                    //     $"returnToGreenhouseDoor: #{junimo.whichJunimoFromThisHut} could not pathfind", LogLevel.Debug);
                    junimo.junimoReachedHut(junimo, junimo.currentLocation);
                    return;
                }
            }

            // var path = string.Join(", ", junimo.controller.pathToEndPoint);
            // BetterJunimos.SMonitor.Log(
            //     $"returnToGreenhouseDoor: #{junimo.whichJunimoFromThisHut} pathfind {junimo.controller.pathToEndPoint.Count} path {path}",
            //     LogLevel.Debug);

            if (!Utility.isOnScreen(Utility.Vector2ToPoint(junimo.position.Value / 64f), 64, junimo.currentLocation))
                return;
            location.playSound("junimoMeep1");
        }

        private static Point GreenhouseDoor(GameLocation location) {
            foreach (var warp in location.warps.Where(warp => warp.TargetName == "Farm"))
            {
                return new Point(warp.X, warp.Y);
            }

            return new Point(10, 23);
        }
    }
    
    
    // pokeToHarvest
    //public void pokeToHarvest()
    public class PatchPokeToHarvest {
        public static void Postfix(JunimoHarvester __instance, bool ___destroy) {
            if (___destroy) return; 
            if (__instance.controller != null) return;
            if (!BetterJunimos.Config.JunimoImprovements.WorkRidiculouslyFast) return;
            // BetterJunimos.SMonitor.Log($"PatchPokeToHarvest: Junimo {__instance.whichJunimoFromThisHut} being directed to new crop", LogLevel.Debug);
            __instance.pathfindToNewCrop();
        }
    }
}