/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/foxwhite25/Stardew-Ultimate-Fertilizer
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace UltimateFertilizer {
    /// <summary>The mod entry point.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named For Harmony")]
    [SuppressMessage("ReSharper", "RedundantAssignment", Justification = "Named For Harmony")]
    internal sealed class ModEntry : Mod {
        private Harmony? _harmony;
        private static IMonitor? _logger;

        private class Config {
            public string FertilizerMode = "multi-fertilizer-stack";

            public bool EnableAlwaysFertilizer = true;
            public bool EnableKeepFertilizerAcrossSeason = true;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public List<float> FertilizerSpeedBoost = new() {0.1f, 0.25f, 0.33f};
            public List<int> FertilizerSpeedAmount = new() {5, 5, 1};
            public bool SpeedRemainAfterHarvest = false;

            public List<int> FertilizerQualityBoost = new() {1, 2, 3};
            public List<int> FertilizerQualityAmount = new() {1, 2, 5};

            public List<float> FertilizerWaterRetentionBoost = new() {0.33f, 0.66f, 1.0f};
            public List<int> FertilizerWaterRetentionAmount = new() {1, 2, 1};
        }

        private static Config _config = null!;
        private const bool DebugMode = false;
        private static IModHelper _helper = null!;

        public override void Entry(IModHelper helper) {
            _config = Helper.ReadConfig<Config>();
            _logger = Monitor;
            _helper = helper;
            _harmony = new Harmony(ModManifest.UniqueID);
            _harmony.PatchAll();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched!;
            helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) => { InitShared.Postfix(); };

            Monitor.Log("Plugin is now working.", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) {
                return;
            }

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => _config = new Config(),
                save: () => {
                    Helper.WriteConfig(_config);
                    InitShared.Postfix();
                }
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => _helper.Translation.Get("config.section.toggles"));
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.fertilizer_mode.title"),
                tooltip: () =>
                    $"{_helper.Translation.Get("config.fertilizer_mode.tooltip.choose")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.option.multi-fertilizer-stack")}: {_helper.Translation.Get("config.fertilizer_mode.tooltip.multi-fertilizer-stack")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.option.multi-fertilizer-single-level")}: {_helper.Translation.Get("config.fertilizer_mode.tooltip.multi-fertilizer-single-level")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.option.single-fertilizer-replace")}: {_helper.Translation.Get("config.fertilizer_mode.tooltip.single-fertilizer-replace")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.option.single-fertilizer-stack")}: {_helper.Translation.Get("config.fertilizer_mode.tooltip.single-fertilizer-stack")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.option.vanilla")}: {_helper.Translation.Get("config.fertilizer_mode.tooltip.vanilla")}" +
                    $"{_helper.Translation.Get("config.fertilizer_mode.tooltip.note")}",
                getValue: () => _config.FertilizerMode,
                setValue: value => _config.FertilizerMode = value,
                allowedValues: new[] {
                    "multi-fertilizer-stack", "multi-fertilizer-single-level", "single-fertilizer-replace",
                    "single-fertilizer-stack", "Vanilla"
                },
                formatAllowedValue: s => {
                    return s switch {
                        "multi-fertilizer-stack" => _helper.Translation.Get(
                            "config.fertilizer_mode.option.multi-fertilizer-stack"),
                        "multi-fertilizer-single-level" => _helper.Translation.Get(
                            "config.fertilizer_mode.option.multi-fertilizer-single-level"),
                        "single-fertilizer-replace" => _helper.Translation.Get(
                            "config.fertilizer_mode.option.single-fertilizer-replace"),
                        "single-fertilizer-stack" => _helper.Translation.Get(
                            "config.fertilizer_mode.option.single-fertilizer-stack"),
                        "Vanilla" => _helper.Translation.Get("config.fertilizer_mode.option.vanilla"),
                        _ => s
                    };
                }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.enable_fertilizer_anytime.title"),
                tooltip: () => _helper.Translation.Get("config.enable_fertilizer_anytime.tooltip"),
                getValue: () => _config.EnableAlwaysFertilizer,
                setValue: value => _config.EnableAlwaysFertilizer = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.keep_fertilizer_across_season.title"),
                tooltip: () => _helper.Translation.Get("config.keep_fertilizer_across_season.tooltip"),
                getValue: () => _config.EnableKeepFertilizerAcrossSeason,
                setValue: value => _config.EnableKeepFertilizerAcrossSeason = value
            );

            configMenu.AddSectionTitle(mod: ModManifest,
                text: () => _helper.Translation.Get("config.section.speed_fertilizer"));
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.affect_multi_harvest.title"),
                tooltip: () => _helper.Translation.Get("config.affect_multi_harvest.tooltip"),
                getValue: () => _config.SpeedRemainAfterHarvest,
                setValue: value => _config.SpeedRemainAfterHarvest = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.speed_gro_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.speed_gro_bonus.tooltip"),
                getValue: () => _config.FertilizerSpeedBoost[0],
                setValue: value => _config.FertilizerSpeedBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_speed_gro_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_speed_gro_bonus.tooltip"),
                getValue: () => _config.FertilizerSpeedBoost[1],
                setValue: value => _config.FertilizerSpeedBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.hyper_speed_gro_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.hyper_speed_gro_bonus.tooltip"),
                getValue: () => _config.FertilizerSpeedBoost[2],
                setValue: value => _config.FertilizerSpeedBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.speed_gro_amount.title"),
                tooltip: () => _helper.Translation.Get("config.speed_gro_amount.tooltip"),
                getValue: () => _config.FertilizerSpeedAmount[0],
                setValue: value => _config.FertilizerSpeedAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_speed_gro_amount.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_speed_gro_amount.tooltip"),
                getValue: () => _config.FertilizerSpeedAmount[1],
                setValue: value => _config.FertilizerSpeedAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.hyper_speed_gro_amount.title"),
                tooltip: () => _helper.Translation.Get("config.hyper_speed_gro_amount.tooltip"),
                getValue: () => _config.FertilizerSpeedAmount[2],
                setValue: value => _config.FertilizerSpeedAmount[2] = value
            );

            configMenu.AddSectionTitle(mod: ModManifest,
                text: () => _helper.Translation.Get("config.section.quality_fertilizer"));
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.basic_fertilizer_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.basic_fertilizer_bonus.tooltip"),
                getValue: () => _config.FertilizerQualityBoost[0],
                setValue: value => _config.FertilizerQualityBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.quality_fertilizer_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.quality_fertilizer_bonus.tooltip"),
                getValue: () => _config.FertilizerQualityBoost[1],
                setValue: value => _config.FertilizerQualityBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_fertilizer_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_fertilizer_bonus.tooltip"),
                getValue: () => _config.FertilizerQualityBoost[2],
                setValue: value => _config.FertilizerQualityBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.basic_fertilizer_amount.title"),
                tooltip: () => _helper.Translation.Get("config.basic_fertilizer_amount.tooltip"),
                getValue: () => _config.FertilizerQualityAmount[0],
                setValue: value => _config.FertilizerQualityAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.quality_fertilizer_amount.title"),
                tooltip: () => _helper.Translation.Get("config.quality_fertilizer_amount.tooltip"),
                getValue: () => _config.FertilizerQualityAmount[1],
                setValue: value => _config.FertilizerQualityAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_fertilizer_amount.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_fertilizer_amount.tooltip"),
                getValue: () => _config.FertilizerQualityAmount[2],
                setValue: value => _config.FertilizerQualityAmount[2] = value
            );

            configMenu.AddSectionTitle(mod: ModManifest,
                text: () => _helper.Translation.Get("config.section.water_fertilizer"));
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.basic_retaining_soil_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.basic_retaining_soil_bonus.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionBoost[0],
                setValue: value => _config.FertilizerWaterRetentionBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.quality_retaining_soil_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.quality_retaining_soil_bonus.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionBoost[1],
                setValue: value => _config.FertilizerWaterRetentionBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_retaining_soil_bonus.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_retaining_soil_bonus.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionBoost[2],
                setValue: value => _config.FertilizerWaterRetentionBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.basic_retaining_soil_amount.title"),
                tooltip: () => _helper.Translation.Get("config.basic_retaining_soil_amount.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionAmount[0],
                setValue: value => _config.FertilizerWaterRetentionAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.quality_retaining_soil_amount.title"),
                tooltip: () => _helper.Translation.Get("config.quality_retaining_soil_amount.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionAmount[1],
                setValue: value => _config.FertilizerWaterRetentionAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => _helper.Translation.Get("config.deluxe_retaining_soil_amount.title"),
                tooltip: () => _helper.Translation.Get("config.deluxe_retaining_soil_amount.tooltip"),
                getValue: () => _config.FertilizerWaterRetentionAmount[2],
                setValue: value => _config.FertilizerWaterRetentionAmount[2] = value
            );
        }

        public static void Print(string msg) {
            if (DebugMode) {
                _logger?.Log(msg, LogLevel.Info);
            }
        }

        [HarmonyPatch(typeof(Crop), nameof(Crop.harvest))]
        public static class Harvest {
            public static void Postfix(Crop __instance, HoeDirt soil) {
                if (!_config.SpeedRemainAfterHarvest) {
                    return;
                }

                var data = __instance.GetData();
                var regrow_day = data?.RegrowDays ?? -1;
                if (regrow_day <= 0)
                    return;
                if (__instance.dayOfCurrentPhase.Value != regrow_day) {
                    return;
                }

                var speed = soil.GetFertilizerSpeedBoost();
                __instance.dayOfCurrentPhase.Value = (int) Math.Ceiling(regrow_day * (1.0 - speed));
            }
        }


        [HarmonyPatch(typeof(CraftingRecipe), nameof(CraftingRecipe.InitShared))]
        public static class InitShared {
            public static void Postfix() {
                foreach (var (key, value) in CraftingRecipe.craftingRecipes.ToArray()) {
                    Print("Found recipe for " + key);
                    var amount = key switch {
                        "Speed-Gro" => _config.FertilizerSpeedAmount[0],
                        "Deluxe Speed-Gro" => _config.FertilizerSpeedAmount[1],
                        "Hyper Speed-Gro" => _config.FertilizerSpeedAmount[2],
                        "Basic Fertilizer" => _config.FertilizerQualityAmount[0],
                        "Quality Fertilizer" => _config.FertilizerQualityAmount[1],
                        "Deluxe Fertilizer" => _config.FertilizerQualityAmount[2],
                        "Basic Retaining Soil" => _config.FertilizerQualityAmount[0],
                        "Quality Retaining Soil" => _config.FertilizerWaterRetentionAmount[1],
                        "Deluxe Retaining Soil" => _config.FertilizerWaterRetentionAmount[2],
                        _ => -1
                    };
                    if (amount == -1) {
                        continue;
                    }

                    Print(key + " Original: " + value);
                    var segment = value.Split("/");
                    var output = segment[2].Split(" ");
                    if (output.Length < 2) {
                        output = output.AddToArray(amount.ToString());
                    }
                    else {
                        output[1] = amount.ToString();
                    }

                    segment[2] = amount == 1 ? output[0] : output.Join(delimiter: " ");
                    CraftingRecipe.craftingRecipes[key] = segment.Join(delimiter: "/");
                    Print(key + " Fixed: " + CraftingRecipe.craftingRecipes[key]);
                }
            }
        }


        [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.CheckApplyFertilizerRules))]
        public static class CheckApplyFertilizerRules {
            private static bool ContainSameType(
                ICollection<string> fertilizers,
                string newFertilizerId,
                string currentFertilizerId
            ) {
                return fertilizers.Contains(newFertilizerId) &&
                       fertilizers.Any(currentFertilizerId.Contains);
            }

            private static bool ContainSameTypes(HoeDirt dirt, string fertilizerId) {
                return Fertilizers.Any(
                    fertilizer =>
                        ContainSameType(fertilizer, fertilizerId, dirt.fertilizer.Value)
                );
            }

            public static bool Prefix(
                HoeDirt __instance,
                ref HoeDirtFertilizerApplyStatus __result,
                string fertilizerId
            ) {
                __result = HoeDirtFertilizerApplyStatus.Okay;
                if (!_config.EnableAlwaysFertilizer && __instance.crop != null &&
                    __instance.crop.currentPhase.Value != 0 && fertilizerId is "(O)368" or "(O)369") {
                    __result = HoeDirtFertilizerApplyStatus.CropAlreadySprouted;
                    return false;
                }

                if (!__instance.HasFertilizer()) return false;
                fertilizerId = ItemRegistry.QualifyItemId(fertilizerId);

                if (__instance.fertilizer.Value.Contains(fertilizerId)) {
                    __result = HoeDirtFertilizerApplyStatus.HasThisFertilizer;
                    return false;
                }

                switch (_config.FertilizerMode) {
                    case "single-fertilizer-stack":
                        if (!ContainSameTypes(__instance, fertilizerId)) {
                            __result = HoeDirtFertilizerApplyStatus.HasAnotherFertilizer;
                        }

                        break;
                    case "Vanilla":
                        __result = __instance.fertilizer.Value.Contains(fertilizerId)
                            ? HoeDirtFertilizerApplyStatus.HasAnotherFertilizer
                            : HoeDirtFertilizerApplyStatus.HasThisFertilizer;
                        break;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Object), nameof(Object.placementAction))]
        public static class ObjectTranspiler {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
                var codes = new List<CodeInstruction>(instructions);
                var start = -1;
                var end = -1;
                var found = false;
                for (var i = 0; i < codes.Count; i++) {
                    if (codes[i].opcode != OpCodes.Ret) continue;
                    if (found) {
                        Print("Transpiler found end " + i);

                        end = i; // include current 'ret'
                        break;
                    }

                    Print("Transpiler potential start  " + (i + 1));
                    start = i + 1; // exclude current 'ret'

                    for (var j = start; j < codes.Count; j++) {
                        if (codes[j].opcode == OpCodes.Ret)
                            break;
                        var strOperand = codes[j].operand as string;
                        if (strOperand != "Strings\\StringsFromCSFiles:HoeDirt.cs.13916") continue;
                        found = true;
                        break;
                    }
                }

                if (start <= -1 || end <= -1) return codes.AsEnumerable();
                // we cannot remove the first code of our range since some jump actually jumps to
                // it, so we replace it with a no-op instead of fixing that jump (easier).
                for (var i = start; i <= end; i++) {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i].operand = null;
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.plant))]
        public static class Plant {
            public static bool Prefix(
                HoeDirt __instance,
                string itemId, Farmer who,
                bool isFertilizer,
                ref bool __result
            ) {
                if (!isFertilizer) {
                    return true;
                }

                if (!__instance.CanApplyFertilizer(itemId)) {
                    __result = false;
                    return false;
                }

                itemId = ItemRegistry.QualifyItemId(itemId) ?? itemId;
                __result = true;

                if (__instance.fertilizer.Value is {Length: > 0}) {
                    switch (_config.FertilizerMode) {
                        case "multi-fertilizer-stack":
                            __instance.fertilizer.Value += "|";
                            __instance.fertilizer.Value += itemId;
                            break;
                        case "multi-fertilizer-single-level":
                            var fertilizerList = Fertilizers.Find(list => list.Contains(itemId));
                            if (fertilizerList != null) {
                                var found = false;
                                foreach (var s in fertilizerList.Where(s => __instance.fertilizer.Value.Contains(s))) {
                                    __instance.fertilizer.Value = __instance.fertilizer.Value.Replace(s, itemId);
                                    found = true;
                                }

                                if (!found) {
                                    __instance.fertilizer.Value += "|";
                                    __instance.fertilizer.Value += itemId;
                                }
                            }

                            break;
                        case "single-fertilizer-stack":
                            __instance.fertilizer.Value += "|";
                            __instance.fertilizer.Value += itemId;
                            break;
                        case "single-fertilizer-replace":
                            __instance.fertilizer.Value = itemId;
                            break;
                        case "Vanilla":
                            break;
                    }
                }
                else {
                    __instance.fertilizer.Value = itemId;
                }

                Print("Fertilizer value: " + __instance.fertilizer.Value);
                if (_config.SpeedRemainAfterHarvest && __instance.crop != null &&
                    __instance.crop.dayOfCurrentPhase.Value != 0) {
                    var data = __instance.crop.GetData();
                    var regrow_day = data?.RegrowDays ?? -1;
                    if (regrow_day > 0) {
                        var speed = __instance.GetFertilizerSpeedBoost();
                        __instance.crop.dayOfCurrentPhase.Value = (int) Math.Ceiling(regrow_day * (1.0 - speed));
                    }
                }

                __instance.applySpeedIncreases(who);
                __instance.Location.playSound("dirtyHit");
                return false;
            }
        }

        [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.seasonUpdate))]
        public static class SeasonUpdate {
            public static void Prefix(
                ref bool onLoad
            ) {
                if (_config.EnableKeepFertilizerAcrossSeason && !onLoad) {
                    onLoad = true;
                }
            }
        }

        private static readonly List<string> FertilizerSpeed = new() {
            HoeDirt.speedGroQID, HoeDirt.superSpeedGroQID, HoeDirt.hyperSpeedGroQID
        };

        private static readonly List<string> FertilizerQuality = new() {
            HoeDirt.fertilizerLowQualityQID, HoeDirt.fertilizerHighQualityQID, HoeDirt.fertilizerDeluxeQualityQID
        };

        private static readonly List<string> FertilizerWaterRetention = new() {
            HoeDirt.waterRetentionSoilQID, HoeDirt.waterRetentionSoilQualityQID, HoeDirt.waterRetentionSoilDeluxeQID
        };

        private static readonly List<List<string>> Fertilizers = new()
            {FertilizerSpeed, FertilizerQuality, FertilizerWaterRetention};

        [HarmonyPatch(typeof(HoeDirt), "GetFertilizerSpeedBoost")]
        public static class GetFertilizerSpeedBoost {
            public static bool Prefix(HoeDirt __instance, ref float __result) {
                var str = __instance.fertilizer.Value;
                __result = 0.0f;
                if (str == null) {
                    return false;
                }

                for (var i = 0; i < FertilizerSpeed.Count; i++) {
                    if (!str.Contains(FertilizerSpeed[i])) continue;
                    if (_config != null) __result += _config.FertilizerSpeedBoost[i];
                }

                return false;
            }
        }


        [HarmonyPatch(typeof(HoeDirt), "GetFertilizerQualityBoostLevel")]
        public static class GetFertilizerQualityBoostLevel {
            public static bool Prefix(HoeDirt __instance, ref int __result) {
                var str = __instance.fertilizer.Value;
                __result = 0;
                if (str == null) {
                    return false;
                }

                for (var i = 0; i < FertilizerQuality.Count; i++) {
                    if (!str.Contains(FertilizerQuality[i])) continue;
                    if (_config != null) __result += _config.FertilizerQualityBoost[i];
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(HoeDirt), "GetFertilizerWaterRetentionChance")]
        public static class GetFertilizerWaterRetentionChance {
            public static bool Prefix(HoeDirt __instance, ref float __result) {
                var str = __instance.fertilizer.Value;
                __result = 0.0f;
                if (str == null) {
                    return false;
                }

                for (var i = 0; i < FertilizerWaterRetention.Count; i++) {
                    if (!str.Contains(FertilizerWaterRetention[i])) continue;
                    if (_config != null) __result += _config.FertilizerWaterRetentionBoost[i];
                }

                return false;
            }
        }


        [HarmonyPatch(typeof(HoeDirt), "DrawOptimized")]
        public static class DrawOptimized {
            private static Rectangle GetFertilizerSourceRect(string fertilizer) {
                int num;
                switch (fertilizer) {
                    case "(O)369":
                    case "369":
                        num = 1;
                        break;
                    case "(O)370":
                    case "370":
                        num = 3;
                        break;
                    case "(O)371":
                    case "371":
                        num = 4;
                        break;
                    case "(O)465":
                    case "465":
                        num = 6;
                        break;
                    case "(O)466":
                    case "466":
                        num = 7;
                        break;
                    case "(O)918":
                    case "918":
                        num = 8;
                        break;
                    case "(O)919":
                    case "919":
                        num = 2;
                        break;
                    case "(O)920":
                    case "920":
                        num = 5;
                        break;
                    default:
                        num = 0;
                        break;
                }

                return new Rectangle(173 + num / 3 * 16, 462 + num % 3 * 16, 16, 16);
            }

            public static void Prefix(HoeDirt __instance, SpriteBatch? fert_batch) {
                if (fert_batch == null || !__instance.HasFertilizer()) return;
                var local = Game1.GlobalToLocal(Game1.viewport, __instance.Tile * 64f);
                var layer = 1.9E-08f;
                foreach (var id in __instance.fertilizer.Value.Split("|")) {
                    fert_batch.Draw(Game1.mouseCursors, local, GetFertilizerSourceRect(id), Color.White,
                        0.0f, Vector2.Zero, 4f, SpriteEffects.None, layer);
                    layer += 1E-09f;
                }
            }
        }
    }
}