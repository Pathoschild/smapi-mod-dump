/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Linq;
using BetterJunimos.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;

// ReSharper disable InconsistentNaming

namespace BetterJunimos.Patches {
    /* areThereMatureCropsWithinRadius **OVERWRITES PREFIX**
     *
     * Search for actionable tiles
     * Completely rewrite original function.
    */
    internal class PatchSearchAroundHut {
        public static bool Prefix(JunimoHut __instance, ref bool __result) {
            if (!Context.IsMainPlayer) return true;

            // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: prefix starts");

            // Prevent unnecessary searching when unpaid
            if (BetterJunimos.Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday) {
                __instance.lastKnownCropLocation = Point.Zero;
                // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: prefix ends (unpaid)");
                return false;
            }

            __result = SearchAroundHut(__instance);
            
            // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: prefix ends");

            return false;
        }

        // search for crops + open plantable spots
        private static bool SearchAroundHut(JunimoHut hut) {
            var id = Util.GetHutIdFromHut(hut);
            var radius = Util.CurrentWorkingRadius;
            GameLocation farm = Game1.currentLocation;

            // SearchHutGrid manages hut.lastKnownCropLocation and Util.Abilities.lastKnownCropLocations
            var foundWork = SearchHutGrid(hut, radius, farm, id);

            if (BetterJunimos.Config.JunimoImprovements.CanWorkInGreenhouse) {
                var ghb = Util.Greenhouse.GreenhouseBuildingNearHut(id);
                var gh = Game1.getLocationFromName("Greenhouse");

                if (!Util.Greenhouse.HutHasGreenhouse(id)) {
                    // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: hut has no greenhouse", LogLevel.Debug);
                    return foundWork;
                }
                // SearchGreenhouseGrid manages hut.lastKnownCropLocation (a hack!) and Util.Abilities.lastKnownCropLocations
                foundWork |= SearchGreenhouseGrid(hut, id);
                Util.Abilities.lastKnownCropLocations.TryGetValue((hut, gh), out var lkc);

                // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: greenhouse lkc {lkc.X} {lkc.Y}", LogLevel.Trace);
            }
            
            // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: foundWork {foundWork}", LogLevel.Trace);
            return foundWork;
        }

        /// <summary>
        /// Search the Greenhouse for work to do, and update
        /// hut.lastKnownCropLocation and
        /// Util.Abilities.lastKnownCropLocations
        /// with the location of any work found
        /// </summary>
        /// <param name="hut">JunimoHut to search</param>
        /// <param name="hut_guid">GUID of hut to search</param>
        /// <returns>True if there's any work to do</returns>
        internal static bool SearchGreenhouseGrid(JunimoHut hut, Guid hut_guid)
        {
            var gh = Game1.getLocationFromName("Greenhouse");

            // BetterJunimos.SMonitor.Log($"SearchAroundHut: searching {gh.Name}", LogLevel.Trace);
            for (var x = 0; x < gh.map.Layers[0].LayerWidth; x++)
            {
                for (var y = 0; y < gh.map.Layers[0].LayerHeight; y++)
                {
                    var pos = new Vector2(x, y);
                    var ability = Util.Abilities.IdentifyJunimoAbility(gh, pos, hut_guid);
                    if (ability == null) continue;
                    hut.lastKnownCropLocation = new Point(x, y);
                    Util.Abilities.lastKnownCropLocations[(hut, gh)] = new Point(x, y);
                    // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: work at gh ({x}, {y})", LogLevel.Trace);
                    return true;
                }
            }

            // BetterJunimos.SMonitor.Log($"PatchSearchAroundHut: no work in gh", LogLevel.Trace);
            Util.Abilities.lastKnownCropLocations[(hut, gh)] = Point.Zero;
            return false;
        }

        private static bool SearchHutGrid(JunimoHut hut, int radius, GameLocation farm, Guid id)
        {
            // BetterJunimos.SMonitor.Log($"SearchAroundHut starts");

            for (var x = hut.tileX.Value + 1 - radius; x < hut.tileX.Value + 2 + radius; ++x)
            {
                for (var y = hut.tileY.Value + 1 - radius; y < hut.tileY.Value + 2 + radius; ++y)
                {
                    var pos = new Vector2(x, y);
                    var ability = Util.Abilities.IdentifyJunimoAbility(farm, pos, id);
                    if (ability == null) continue;
                    hut.lastKnownCropLocation = new Point(x, y);
                    Util.Abilities.lastKnownCropLocations[(hut, farm)] = new Point(x, y);
                    // BetterJunimos.SMonitor.Log($"SearchAroundHut: work at ({x}, {y})");

                    return true;
                }
            }
            
            hut.lastKnownCropLocation = Point.Zero;
            Util.Abilities.lastKnownCropLocations[(hut, farm)] = Point.Zero;
            return false;
        }
    }

    /* Update
     * 
     * To allow more junimos, allow working in rain
     */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoHutUpdate {
        // This is to prevent the update function from running, other than base.Update()
        // Capture sendOutTimer and use to stop execution
        public static void Prefix(JunimoHut __instance, ref int ___junimoSendOutTimer, out int __state) {
            __state = ___junimoSendOutTimer;
            ___junimoSendOutTimer = 0;
        }

        public static void Postfix(JunimoHut __instance, GameTime time, ref int ___junimoSendOutTimer, int __state) {
            if (__state <= 0) return;
            if (!Context.IsMainPlayer) return;

            // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutUpdate: postfix starts");

            ___junimoSendOutTimer = __state - time.ElapsedGameTime.Milliseconds;
            
            // Don't work on farmEvent days
            if (Game1.farmEvent != null)
                return;
            // Winter
            if (Game1.IsWinter && !Util.Progression.CanWorkInWinter) {
                return;
            }
            // Rain
            if (Game1.isRaining && !Util.Progression.CanWorkInRain){
                BetterJunimos.SMonitor.Log($"ReplaceJunimoHutUpdate: rain");
                return;
            }
            // Currently sending out a junimo
            if (___junimoSendOutTimer > 0) {
                // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutUpdate: sending");
                return;
            }
            // Already enough junimos
            if (__instance.myJunimos.Count >= Util.Progression.MaxJunimosUnlocked){
                // BetterJunimos.SMonitor.Log($"Already {__instance.myJunimos.Count} Junimos, limit is {Util.Progression.MaxJunimosUnlocked}");
                return;
            }
            // Nothing to do
            if (!__instance.areThereMatureCropsWithinRadius()) {
                // BetterJunimos.SMonitor.Log("No work for Junimos to do, not spawning another");
                return;
            }
            // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutUpdate: spawning");
            Util.SpawnJunimoAtHut(__instance);
            // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutUpdate: postfix ends");
            ___junimoSendOutTimer = 1000;
        }
    }

    /*
     * performTenMinuteAction
     * 
     * Add the end to trigger more than 3 junimos to spawn
     */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoTimerNumber {
        public static void Postfix(JunimoHut __instance, ref int ___junimoSendOutTimer) {
            if (!Context.IsMainPlayer) return;

            foreach (var location in Game1.locations) {
                if (location.IsGreenhouse) {
                    foreach (var npc in location.characters) {
                        if (npc is JunimoHarvester) {
                            if (!__instance.myJunimos.Contains(npc)) {
                                __instance.myJunimos.Add(npc as JunimoHarvester);
                                ((JunimoHarvester) npc).pokeToHarvest();
                            }
                        }
                    }
                }
            }
            
            var time = Util.Progression.CanWorkInEvenings ? 2400 : 1900;
            if (Game1.timeOfDay > time) return;

            if (__instance.myJunimos.Count < Util.Progression.MaxJunimosUnlocked) {
                ___junimoSendOutTimer = 1;
            }
        }
    }
    
    /*
     * performTenMinuteAction
     *
     * Complete rewrite to allow Junimos in greenhouse
     */
    internal class ReplaceTenMinuteAction {
        public static bool Prefix(int timeElapsed, JunimoHut __instance, ref int ___junimoSendOutTimer) {
            if (!Context.IsMainPlayer) return true;
            
            // ((Building) __instance).performTenMinuteAction(timeElapsed);
            for (var index = __instance.myJunimos.Count - 1; index >= 0; --index)
            {
                // if (!Game1.getFarm().characters.Contains(__instance.myJunimos[index]))
                //     __instance.myJunimos.RemoveAt(index);
                // else
                __instance.myJunimos[index].pokeToHarvest();
            }
            
            if (Game1.timeOfDay is >= 2000 and < 2400 && (!Game1.IsWinter && Game1.random.NextDouble() < 0.2))
            {
                __instance.wasLit.Value = true;
            }
            else
            {
                if (Game1.timeOfDay != 2400 || Game1.IsWinter)
                    return false;
                __instance.wasLit.Value = false;
            }
            var time = Util.Progression.CanWorkInEvenings ? 2400 : 1900;
            if (Game1.timeOfDay > time) return false;

            if (__instance.myJunimos.Count < Util.Progression.MaxJunimosUnlocked) {
                ___junimoSendOutTimer = 1;
            }

            return false;
        }
    }
    
    /* getUnusedJunimoNumber
     * 
     * Completely rewrite method to support more than 3 junimos
     * The only difference is the use of MaxJunimos
    */
    [HarmonyPriority(Priority.Low)]
    internal class ReplaceJunimoHutNumber {
        public static bool Prefix(JunimoHut __instance, ref int __result) {
            if (!Context.IsMainPlayer) return true;
            // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutNumber: prefix starts");

            for (var index = 0; index < Util.Progression.MaxJunimosUnlocked; ++index) {
                if (index >= __instance.myJunimos.Count) {
                    __result = index;
                    // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutNumber: prefix ends {__result}  A");

                    return false;
                }

                var flag = __instance.myJunimos.Any(junimo => junimo.whichJunimoFromThisHut == index);

                if (flag) continue;
                __result = index;
                // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutNumber: prefix ends {__result}  B");
                return false;
            }

            __result = 2;
            // BetterJunimos.SMonitor.Log($"ReplaceJunimoHutNumber: prefix ends {__result}  C");
            return false;
        }
    }
}