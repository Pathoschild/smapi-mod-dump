/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Diagnostics;

namespace ColorfulFishPonds {
    internal class ModEntry : Mod {

        public static ModConfig Config = new();
        public static IMonitor? SMonitor;
        public static IModHelper? SHelper;

        static ModData? ColorOverrideModel;

        static List<FishPond> PrismaticPonds = new List<FishPond>();

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            SMonitor = Monitor;
            SHelper = Helper;

            helper.ConsoleCommands.Add("rokugin.fishpack", "\n\nCommands:\n" +
                "reload\n    Reloads related content packs and refreshes pond colors.\n", PackCommands);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), "doFishSpecificWaterColoring"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(FishPond_DoFishSpecificWaterColoring_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), "addFishToPond"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(FishPond_RefreshColor_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), nameof(FishPond.CatchFish)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(FishPond_RefreshColor_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), nameof(FishPond.ClearPond)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(FishPond_ClearPond_Postfix))
            );
        }

        private void OnPeerConnected(object? sender, StardewModdingAPI.Events.PeerConnectedEventArgs e) {
            RefreshPondColors();
        }

        private void OnUpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e) {
            if (!Context.IsWorldReady || Context.IsMultiplayer) return;

            if (e.IsMultipleOf(10)) {
                foreach (var pond in PrismaticPonds) {
                    if (!pond.modData.ContainsKey("rokuginRandomInt")) {
                        int r = new Random().Next(0, 100);
                        pond.modData.Add("rokuginRandomInt", r.ToString());
                    }

                    pond.overrideWaterColor.Value = Utility.GetPrismaticColor(int.Parse(pond.modData["rokuginRandomInt"]), Config.PrismaticSpeed);
                }
            }
        }

        static void FishPond_RefreshColor_Postfix(FishPond __instance) {
            PrismaticPonds.Remove(__instance);
            SHelper!.Reflection.GetMethod(__instance, "doFishSpecificWaterColoring").Invoke();
        }

        static void FishPond_ClearPond_Postfix(FishPond __instance) {
            PrismaticPonds.Remove(__instance);
            __instance.modData.Remove("rokuginRandomInt");
            SHelper!.Reflection.GetMethod(__instance, "doFishSpecificWaterColoring").Invoke();

        }

        static void FishPond_DoFishSpecificWaterColoring_Postfix(FishPond __instance) {
            try {
                if (!Config.ModEnabled) return;

                if (Context.IsMainPlayer) {
                    Color? color = Color.White;
                    bool enoughFish = __instance.currentOccupants.Value >= Config.RequiredPopulation;

                    if (Config.DyeColors && enoughFish) {
                        color = ItemContextTagManager.GetColorFromTags(ItemRegistry.Create(ItemRegistry.type_object + __instance.fishType.Value));
                    }

                    if (ColorOverrideModel != null) {
                        foreach (var group in ColorOverrideModel.GroupOverrides) {
                            if (Game1.objectData.TryGetValue(__instance.fishType.Value, out var objectData) &&
                                objectData.ContextTags.Contains(group.Value.GroupTag) && enoughFish) {
                                Color groupOverrideColor = new Color(group.Value.PondColor.GetValueOrDefault("R"),
                                                                 group.Value.PondColor.GetValueOrDefault("G"),
                                                                 group.Value.PondColor.GetValueOrDefault("B"));
                                color = groupOverrideColor;
                                break;
                            };
                        }

                        if (!Config.DisableSingleRecolors) {
                            foreach (var fish in ColorOverrideModel.SingleOverrides) {
                                if (__instance.fishType.Value == fish.Value.FishID && enoughFish) {
                                    Color singleOverrideColor = new Color(fish.Value.PondColor.GetValueOrDefault("R"),
                                                                     fish.Value.PondColor.GetValueOrDefault("G"),
                                                                     fish.Value.PondColor.GetValueOrDefault("B"));

                                    if (!Context.IsMultiplayer && fish.Value.IsPrismatic && !PrismaticPonds.Contains(__instance)) {
                                        PrismaticPonds.Add(__instance);
                                    } else {
                                        color = singleOverrideColor;
                                    }

                                    break;
                                }
                            }
                        }

                        if (color == null) {
                            SMonitor!.Log("Override color is null, aborting\n", Config.Debugging ? LogLevel.Debug : LogLevel.Trace);
                            return;
                        }

                        __instance.overrideWaterColor.Value = color.Value;
                    }
                }
            }
            catch (Exception ex) {
                SMonitor!.LogOnce(
                    $"Harmony patch {nameof(FishPond_DoFishSpecificWaterColoring_Postfix)} encountered an error. Custom fish pond colors might not be applied.\n {ex}",
                    LogLevel.Error);
                return;
            }
        }

        void PackCommands(string command, string[] args) {
            if (args[0] == "reload") {
                ReadContentPacks();
                RefreshPondColors();
            } else {
                Monitor.Log($"Unrecognized command: \"{args[0]}\". Use help rokugin_pack to see a list of commands.", LogLevel.Error);
            }
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            SetUpGMCM();
            ReadContentPacks();
        }

        private void OnDayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            RefreshPondColors();
        }

        void RefreshPondColors() {
            PrismaticPonds.Clear();

            foreach (var location in Game1.locations) {
                foreach (var building in location.buildings) {
                    if (building.buildingType.Value == "Fish Pond") {
                        Helper.Reflection.GetMethod((FishPond)building, "doFishSpecificWaterColoring").Invoke();
                    }
                }
            }
        }

        void ReadContentPacks() {
            Monitor.Log("Loading content packs started.\n", LogLevel.Debug);
            Stopwatch sw = Stopwatch.StartNew();
            Helper.Data.WriteJsonFile("fishPondColorData.json", new ModData());
            ColorOverrideModel = Helper.Data.ReadJsonFile<ModData>("fishPondColorData.json");

            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned()) {
                Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", 
                    Config.Debugging ? LogLevel.Debug : LogLevel.Trace);

                if (!contentPack.HasFile("content.json")) {
                    Monitor.Log($"Content pack: {contentPack.Manifest.Name} missing required file: content.json, Skipping content pack.", LogLevel.Error);
                    continue;
                }

                ModData? data = contentPack.ReadJsonFile<ModData>("content.json");

                if (data == null) continue;

                Monitor.Log($"{data.SingleOverrides.Count} single overrides.", Config.Debugging ? LogLevel.Debug : LogLevel.Trace);

                if (ColorOverrideModel != null) {
                    foreach (var fish in data.SingleOverrides) {
                        if (ColorOverrideModel.SingleOverrides.ContainsKey(fish.Key)) {
                            Monitor.Log($"Content pack: {fish.Key} already present, skipping.\n", LogLevel.Warn);
                            continue;
                        }
                        Monitor.Log($"{fish.Key} color override found, applying.\n", Config.Debugging ? LogLevel.Debug : LogLevel.Trace);
                        ColorOverrideModel.SingleOverrides.Add(fish.Key, fish.Value);
                        Helper.Data.WriteJsonFile("fishPondColorData.json", ColorOverrideModel);
                    }

                    Monitor.Log($"{data.GroupOverrides.Count} group overrides.", Config.Debugging ? LogLevel.Debug : LogLevel.Trace);

                    foreach (var group in data.GroupOverrides) {
                        if (ColorOverrideModel.SingleOverrides.ContainsKey(group.Key)) {
                            Monitor.Log($"Content pack: {group.Key} already present, skipping.\n", LogLevel.Warn);
                            continue;
                        }
                        Monitor.Log($"{group.Key} color override found, applying.\n", Config.Debugging ? LogLevel.Debug : LogLevel.Trace);
                        ColorOverrideModel.GroupOverrides.Add(group.Key, group.Value);
                        Helper.Data.WriteJsonFile("fishPondColorData.json", ColorOverrideModel);
                    }
                }
            }

            sw.Stop();
            Monitor.Log($"Loading content packs finished. [{sw.ElapsedMilliseconds} ms]\n", LogLevel.Debug);
        }

        private void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("mod-enabled.label"),
                tooltip: () => Helper.Translation.Get("mod-enabled.tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("default-colors.label"),
                tooltip: () => Helper.Translation.Get("default-colors.tooltip"),
                getValue: () => Config.DyeColors,
                setValue: value => Config.DyeColors = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("required-population.label"),
                tooltip: () => Helper.Translation.Get("required-population.tooltip"),
                getValue: () => Config.RequiredPopulation,
                setValue: value => Config.RequiredPopulation = value,
                min: 1,
                max: 10
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("single-overrides.label"),
                tooltip: () => Helper.Translation.Get("single-overrides.tooltip"),
                getValue: () => Config.DisableSingleRecolors,
                setValue: value => Config.DisableSingleRecolors = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("debugging.label"),
                tooltip: () => Helper.Translation.Get("debugging.tooltip"),
                getValue: () => Config.Debugging,
                setValue: value => Config.Debugging = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("prismatic-speed.label"),
                tooltip: () => Helper.Translation.Get("prismatic-speed.tooltip"),
                getValue: () => Config.PrismaticSpeed,
                setValue: value => Config.PrismaticSpeed = value,
                min: 0f,
                max: 1f,
                interval: 0.05f
            );
        }

    }
}
