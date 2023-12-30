/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spajus/stardew-valley-fast-loads
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FastLoads {
    public class ModConfig {
        public bool DetectCollisions { get; set; } = false;
        public bool VerboseCollisionChecks { get; set; } = false;
        public bool AbortOnCollision { get; set; } = false;
    }

    internal sealed class ModEntry : Mod {
        private ModConfig cfg;
        public override void Entry(IModHelper helper) {
            cfg = Helper.ReadConfig<ModConfig>();
            StardewFastLoads.Mon = Monitor;
            StardewFastLoads.DetectCollisions = cfg.DetectCollisions;
            StardewFastLoads.AbortOnCollision = cfg.AbortOnCollision;
            StardewFastLoads.VerboseCollisionChecks = cfg.VerboseCollisionChecks;
            StardewFastLoads.RoutesField = GetPrivateField<NPC>("routesFromLocationToLocation");
            StardewFastLoads.ExploreMethod = GetPrivateMethod<NPC>("exploreWarpPoints");
            if (cfg.DetectCollisions) {
                StardewFastLoads.CollisionSB = new();
                StardewFastLoads.CollisionMap = new();
            }
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
            Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) {
                return;
            }
            configMenu.Register(
                mod: ModManifest,
                reset: () => cfg = new ModConfig(),
                save: () => Helper.WriteConfig(this.cfg)
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Detect Collisions",
                tooltip: () => "Will detect path collisions if they would exist. Save files will load a little slower.",
                getValue: () => cfg.DetectCollisions,
                setValue: value => {
                    cfg.DetectCollisions = value;
                    StardewFastLoads.DetectCollisions = value;
                    if (value) {
                        StardewFastLoads.CollisionSB = new();
                        StardewFastLoads.CollisionMap = new();
                    }
                }
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Abort On Collision",
                tooltip: () => "Will abort the load operation if a collision is detected. Requires \"Detect Collisions\" to be enabled.",
                getValue: () => cfg.AbortOnCollision,
                setValue: value => {
                    cfg.AbortOnCollision = value;
                    StardewFastLoads.AbortOnCollision = value;
                }
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Verbose Collision Checks",
                tooltip: () => "Will print hashes of each route. Save files will load extremely slowly! For debugging only.",
                getValue: () => cfg.VerboseCollisionChecks,
                setValue: value => {
                    cfg.VerboseCollisionChecks = value;
                    StardewFastLoads.VerboseCollisionChecks = value;
                }
            );

        }

        // public override object GetApi() {
        //     return cfg ?? new ModConfig();
        // }

        private FieldInfo GetPrivateField<T>(string fieldName) {
            var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static);
            return field;
        }
        private MethodInfo GetPrivateMethod<T>(string fieldName) {
            var meth = typeof(T).GetMethod(fieldName, BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static);
            return meth;
        }
    }

    public static class StardewFastLoads {
        public static readonly HashSet<long> RoutesFromLocToLocHash = new();
        public static IMonitor Mon;
        public static FieldInfo RoutesField;
        public static MethodInfo ExploreMethod;
        public static bool DetectCollisions;
        public static bool AbortOnCollision;
        public static bool VerboseCollisionChecks;
        public static int CollisionsFound;
        public static Dictionary<long, string> CollisionMap;
        public static StringBuilder CollisionSB;


        [HarmonyPatch(typeof(NPC), "populateRoutesFromLocationToLocationList")]
        public static class NPC_populateRoutesFromLocationToLocationList {
            [HarmonyPrefix, HarmonyPriority(Priority.First)]
            public static bool Prefix() {
                Mon.Log($"Clearing RoutesFromLocToLoc before populate: {RoutesFromLocToLocHash.Count}", LogLevel.Info);
                CollisionsFound = 0;
                RoutesFromLocToLocHash.Clear();
                if (DetectCollisions) {
                    Mon.Log("Collision detection enabled, the game will load a little slower", LogLevel.Warn);
                }
                return true;
            }
            [HarmonyPostfix, HarmonyPriority(Priority.Last)]
            public static void Postfix() {
                var routes = (List<List<string>>) RoutesField.GetValue(null);

                foreach (var r in routes) {
                    RoutesFromLocToLocHash.Add(HashOf(r));
                }
                Mon.Log($"RoutesFromLocToLoc after populate: {routes.Count}. Hashes: {RoutesFromLocToLocHash.Count}.", LogLevel.Info);
                if (CollisionsFound > 0) {
                    Mon.Log($"Found {CollisionsFound} route collisions!", LogLevel.Error);
                }
            }
        }

        [HarmonyPatch(typeof(NPC), "doesRoutesListContain")]
        public static class NPC_doesRoutesListContain {
            [HarmonyPrefix, HarmonyPriority(Priority.First)]
            public static bool Prefix(ref bool __result, List<string> route) {
                // ModEntry.Mon.Log($"RoutesFromLocToLoc during contains check hashes: {routesFromLocToLocHash.Count}", LogLevel.Warn);
                __result = RoutesFromLocToLocHash.Contains(HashOf(route));
                return false;
            }
        }

        [HarmonyPatch(typeof(NPC), "exploreWarpPoints")]
        public static class NPC_exploreWarpPoints {
            [HarmonyPrefix, HarmonyPriority(Priority.First)]
            public static bool Prefix(ref bool __result, GameLocation l, List<string> route) {
                // ModEntry.Mon.Log($"Recursive ExploreWarpPoints: {list.Count} / {RoutesFromLocToLocHash.Count}", LogLevel.Warn);
                bool added = false;
                if (l != null && !route.Contains(l.name, StringComparer.Ordinal))
                {
                    route.Add(l.name);
                    if (route.Count == 1 || !RoutesFromLocToLocHash.Contains(HashOf(route)))
                    {
                        if (route.Count > 1)
                        {
                            var r = route.ToList<string>();
                            var list = (List<List<string>>) RoutesField.GetValue(null);
                            list.Add(r);
                            RoutesFromLocToLocHash.Add(HashOf(r));
                            added = true;
                        }
                        foreach (Warp warp in l.warps)
                        {
                            string name = warp.TargetName;
                            if (name == "BoatTunnel")
                            {
                                name = "IslandSouth";
                            }
                            if (!name.Equals("Farm", StringComparison.Ordinal) && !name.Equals("Woods", StringComparison.Ordinal) && !name.Equals("Backwoods", StringComparison.Ordinal) && !name.Equals("Tunnel", StringComparison.Ordinal) && !name.Contains("Volcano"))
                            {
                                ExploreMethod.Invoke(null,
                                    new object[] { Game1.getLocationFromName(name), route });
                            }
                        }
                        foreach (Point p in l.doors.Keys)
                        {
                            string name2 = l.doors[p];
                            if (name2 == "BoatTunnel")
                            {
                                name2 = "IslandSouth";
                            }
                            ExploreMethod.Invoke(null,
                                new object[] { Game1.getLocationFromName(name2), route });
                        }
                    }
                    if (route.Count > 0)
                    {
                        route.RemoveAt(route.Count - 1);
                    }
                }
                __result = added;
                if (CollisionsFound > 0) {
                    Mon.Log($"Found {CollisionsFound} route collisions!", LogLevel.Error);
                }
                return false;
            }
        }

        private static long HashOf(List<string> strings) {
            long hash = 17;
            if (DetectCollisions) {
                CollisionSB.Clear();
            }
            for (int i = strings.Count - 1; i >= 0; i--) {
                string s = strings[i];
                hash = hash * 31 + s.GetHashCode();
                if (DetectCollisions) {
                    CollisionSB.Append(s);
                    CollisionSB.Append('|');
                }
            }
            if (DetectCollisions) {
                var hashStr = CollisionSB.ToString();
                if (CollisionMap.TryGetValue(hash, out var str)) {
                    if (!str.Equals(hashStr, StringComparison.Ordinal)) {
                        var msg = $"Collision detected! {str} and {hashStr} are both {hash}! Collision map size: {CollisionMap.Count}";
                        Mon.Log(msg, LogLevel.Error);
                        CollisionsFound++;
                        if (AbortOnCollision) {
                            throw new Exception(msg);
                        }
                    }
                } else {
                    CollisionMap.Add(hash, hashStr);
                    if (VerboseCollisionChecks) {
                        Mon.Log($"{hashStr} -> {hash}", LogLevel.Warn);
                    }
                }
            }
            return hash;
        }
    }
}
