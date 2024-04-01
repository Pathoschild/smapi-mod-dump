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
            public bool EnableMultiFertilizer = true;
            public bool EnableAlwaysFertilizer = true;
            public string SameFertilizerMode = "Disable";

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public List<float> FertilizerSpeedBoost = new() {0.1f, 0.25f, 0.33f};
            public List<int> FertilizerSpeedAmount = new() {5, 5, 1};

            public List<int> FertilizerQualityBoost = new() {1, 2, 3};
            public List<int> FertilizerQualityAmount = new() {1, 2, 5};

            public List<float> FertilizerWaterRetentionBoost = new() {0.33f, 0.66f, 1.0f};
            public List<int> FertilizerWaterRetentionAmount = new() {1, 2, 1};
        }

        private static Config _config = null!;

        public override void Entry(IModHelper helper) {
            _harmony = new Harmony(ModManifest.UniqueID);
            _logger = Monitor;
            _harmony.PatchAll();
            _config = Helper.ReadConfig<Config>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched!;

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
                save: () => Helper.WriteConfig(_config)
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Toggles");
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Multi Fertilizer",
                tooltip: () =>
                    "Allow you to apply multiple types of fertilizer to a crop space.\n" +
                    "Config only apply when you use fertilizer, this means if your map already have mixed fertilizer, they still works.",
                getValue: () => _config.EnableMultiFertilizer,
                setValue: value => _config.EnableMultiFertilizer = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Multi Same Type Fertilizer Mode",
                getValue: () => _config.SameFertilizerMode,
                tooltip: () =>
                    "Allow you to choice what happen when you apply multiple same type of fertilizer to a crop space.\n" +
                    "Replace: the fertilizer you use would replace the current one without refund, effectively a upgrade or downgrade.\n" +
                    "Stack: same type of fertilizer stack their bonus, applying a 10% and a 25% gives you a 35% bonus.\n" +
                    "Disable: You can not apply fertilizer if it's the same type.\n" +
                    "Config only apply when you use fertilizer, this means if your map already have mixed fertilizer, they still works.\n" +
                    "Requires Enable Multi Fertilizer to do anything.",
                setValue: value => _config.SameFertilizerMode = value,
                allowedValues: new[] {"Replace", "Stack", "Disable"}
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Fertilizer Anytime",
                tooltip: () => "Allow you to apply fertilizer to a crop space that have grown crops.",
                getValue: () => _config.EnableAlwaysFertilizer,
                setValue: value => _config.EnableAlwaysFertilizer = value
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => "(RES) means restart required");
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Speed Fertilizer");
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Speed-Gro Bonus",
                tooltip: () => "Modify the speed bonus from Speed-Gro, by default 10% (0.1)",
                getValue: () => _config.FertilizerSpeedBoost[0],
                setValue: value => _config.FertilizerSpeedBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Speed-Gro Bonus",
                tooltip: () => "Modify the speed bonus from Deluxe Speed-Gro, by default 25% (0.25)",
                getValue: () => _config.FertilizerSpeedBoost[1],
                setValue: value => _config.FertilizerSpeedBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Hyper Speed-Gro Bonus",
                tooltip: () => "Modify the speed bonus from Hyper Speed-Gro, by default 33% (0.33)",
                getValue: () => _config.FertilizerSpeedBoost[2],
                setValue: value => _config.FertilizerSpeedBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Speed-Gro Amount (RES)",
                tooltip: () => "Modify the amount of Speed-Gro you get per craft, by default 5",
                getValue: () => _config.FertilizerSpeedAmount[0],
                setValue: value => _config.FertilizerSpeedAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Speed-Gro Amount (RES)",
                tooltip: () => "Modify the amount of Deluxe Speed-Gro you get per craft, by default 5",
                getValue: () => _config.FertilizerSpeedAmount[1],
                setValue: value => _config.FertilizerSpeedAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Hyper Speed-Gro Amount (RES)",
                tooltip: () => "Modify the amount of Hyper Speed-Gro you get per craft, by default 1",
                getValue: () => _config.FertilizerSpeedAmount[2],
                setValue: value => _config.FertilizerSpeedAmount[2] = value
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Quality Fertilizer");
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Basic Fertilizer Bonus",
                tooltip: () => "Modify the quality bonus from Basic Fertilizer, by default 1",
                getValue: () => _config.FertilizerQualityBoost[0],
                setValue: value => _config.FertilizerQualityBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Quality Fertilizer Bonus",
                tooltip: () => "Modify the speed bonus from Quality Fertilizer, by default 2",
                getValue: () => _config.FertilizerQualityBoost[1],
                setValue: value => _config.FertilizerQualityBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Fertilizer Bonus",
                tooltip: () => "Modify the speed bonus from Deluxe Fertilizer, by default 3",
                getValue: () => _config.FertilizerQualityBoost[2],
                setValue: value => _config.FertilizerQualityBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Basic Fertilizer Amount (RES)",
                tooltip: () => "Modify the amount of Basic Fertilizer get per craft, by default 1",
                getValue: () => _config.FertilizerQualityAmount[0],
                setValue: value => _config.FertilizerQualityAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Quality Fertilizer Amount (RES)",
                tooltip: () => "Modify the amount of Quality Fertilizer you get per craft, by default 2",
                getValue: () => _config.FertilizerQualityAmount[1],
                setValue: value => _config.FertilizerQualityAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Fertilizer Amount (RES)",
                tooltip: () => "Modify the amount of Deluxe Fertilizer you get per craft, by default 5",
                getValue: () => _config.FertilizerQualityAmount[2],
                setValue: value => _config.FertilizerQualityAmount[2] = value
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Water Fertilizer");
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Basic Retaining Soil Bonus",
                tooltip: () => "Modify the chance of retaining water from Basic Retaining Soil, by default 33% (0.33)",
                getValue: () => _config.FertilizerWaterRetentionBoost[0],
                setValue: value => _config.FertilizerWaterRetentionBoost[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Quality Retaining Soil Bonus",
                tooltip: () =>
                    "Modify the chance of retaining water from Quality Retaining Soil, by default 66% (0.66)",
                getValue: () => _config.FertilizerWaterRetentionBoost[1],
                setValue: value => _config.FertilizerWaterRetentionBoost[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Retaining Soil Bonus",
                tooltip: () => "Modify the chance of retaining water from Deluxe Retaining Soil, by default 100% (1.0)",
                getValue: () => _config.FertilizerWaterRetentionBoost[2],
                setValue: value => _config.FertilizerWaterRetentionBoost[2] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Basic Retaining Soil Amount (RES)",
                tooltip: () => "Modify the amount of Basic Retaining Soil get per craft, by default 1",
                getValue: () => _config.FertilizerWaterRetentionAmount[0],
                setValue: value => _config.FertilizerWaterRetentionAmount[0] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Quality Retaining Soil Amount (RES)",
                tooltip: () => "Modify the amount of Quality Retaining Soil you get per craft, by default 2",
                getValue: () => _config.FertilizerWaterRetentionAmount[1],
                setValue: value => _config.FertilizerWaterRetentionAmount[1] = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Deluxe Retaining Soil Amount (RES)",
                tooltip: () => "Modify the amount of Deluxe Retaining Soil you get per craft, by default 1",
                getValue: () => _config.FertilizerWaterRetentionAmount[2],
                setValue: value => _config.FertilizerWaterRetentionAmount[2] = value
            );
        }

        public static void Print(string msg) {
            _logger?.Log(msg, LogLevel.Info);
        }

        [HarmonyPatch(typeof(CraftingRecipe), "InitShared")]
        public static class InitShared {
            public static void Postfix() {
                foreach (var (key, value) in CraftingRecipe.craftingRecipes.ToArray()) {
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
                }
            }
        }


        [HarmonyPatch(typeof(HoeDirt), "CheckApplyFertilizerRules")]
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


                if (!_config.EnableMultiFertilizer) {
                    __result = __instance.fertilizer.Value.Contains(fertilizerId)
                        ? HoeDirtFertilizerApplyStatus.HasAnotherFertilizer
                        : HoeDirtFertilizerApplyStatus.HasThisFertilizer;
                    return false;
                }

                if (_config.SameFertilizerMode == "Disable" && ContainSameTypes(__instance, fertilizerId)) {
                    __result = HoeDirtFertilizerApplyStatus.HasThisFertilizer;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Object), "placementAction")]
        public static class ObjectTranspiler {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
                var codes = new List<CodeInstruction>(instructions);
                var start = -1;
                var end = -1;
                var found = false;
                for (var i = 0; i < codes.Count; i++) {
                    if (codes[i].opcode != OpCodes.Ret) continue;
                    if (found)
                    {
                        Print("END " + i);

                        end = i; // include current 'ret'
                        break;
                    }
                    
                    Print("START " + (i + 1));
                    start = i + 1; // exclude current 'ret'

                    for (var j = start; j < codes.Count; j++)
                    {
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

        [HarmonyPatch(typeof(HoeDirt), "plant")]
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

                if (__instance.fertilizer.Value is {Length: > 0}) {
                    void DoApply() {
                        if (_config.SameFertilizerMode == "Replace") {
                            var fertilizerList = Fertilizers.Find(list => list.Contains(itemId));
                            if (fertilizerList != null) {
                                foreach (var s in fertilizerList) {
                                    __instance.fertilizer.Value = __instance.fertilizer.Value.Replace(s, itemId);
                                }

                                return;
                            }
                        }

                        __instance.fertilizer.Value += "|";
                        __instance.fertilizer.Value += itemId;
                    }

                    DoApply();
                }
                else {
                    __instance.fertilizer.Value = itemId;
                }

                __instance.applySpeedIncreases(who);
                __instance.Location.playSound("dirtyHit");
                __result = true;
                return false;
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