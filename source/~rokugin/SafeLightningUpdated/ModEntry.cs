/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System.Threading;

namespace SafeLightningUpdated {
    internal class ModEntry : Mod {

        static ModConfig Config;
        static IMonitor StaticMonitor;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            StaticMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.performLightningUpdate)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(Utility_PerformLightningUpdate_Prefix))
            );
        }

        static bool Utility_PerformLightningUpdate_Prefix(ref int time_of_day) {
            if (!Config.ModEnabled) return true;

            try {
                Random random = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, time_of_day);
                if (random.NextDouble() < 0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0) {
                    Farm.LightningStrikeEvent lightningEvent = new Farm.LightningStrikeEvent();
                    lightningEvent.bigFlash = true;
                    Farm farm = Game1.getFarm();
                    List<Vector2> lightningRods = new List<Vector2>();
                    foreach (KeyValuePair<Vector2, StardewValley.Object> v2 in farm.objects.Pairs) {
                        if (v2.Value.QualifiedItemId == "(BC)9") {
                            lightningRods.Add(v2.Key);
                        }
                    }
                    if (lightningRods.Count > 0) {
                        for (int i = 0; i < 2; i++) {
                            Vector2 v = random.ChooseFrom(lightningRods);
                            if (farm.objects[v].heldObject.Value == null) {
                                farm.objects[v].heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)787");
                                farm.objects[v].MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
                                farm.objects[v].shakeTimer = 1000;
                                lightningEvent.createBolt = true;
                                lightningEvent.boltPosition = v * 64f + new Vector2(32f, 0f);
                                if (!Config.DisableStrikes) farm.lightningStrikeEvent.Fire(lightningEvent);
                                break;
                            }
                        }
                    }
                    if (random.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0) {
                        try {
                            if (!Config.DisableStrikes) {
                                if (Utility.TryGetRandom(farm.terrainFeatures, out var tile, out var feature)) {
                                    if (feature is FruitTree fruitTree) {
                                        lightningEvent.createBolt = true;
                                        lightningEvent.boltPosition = tile * 64f + new Vector2(32f, -128f);
                                    } else {
                                        Crop crop = (feature as HoeDirt)?.crop;
                                        bool num = crop != null && !crop.dead.Value;
                                        if (feature.performToolAction(null, 0, tile)) {
                                            lightningEvent.createBolt = true;
                                            lightningEvent.boltPosition = tile * 64f + new Vector2(32f, -128f);
                                        }
                                        if (num && crop.dead.Value) {
                                            lightningEvent.createBolt = true;
                                            lightningEvent.boltPosition = tile * 64f + new Vector2(32f, 0f);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) {
                        }
                    }
                    if (!Config.DisableStrikes) farm.lightningStrikeEvent.Fire(lightningEvent);
                } else if (random.NextDouble() < 0.1) {
                    if (!Config.DisableStrikes) {
                        Farm.LightningStrikeEvent lightningEvent2 = new Farm.LightningStrikeEvent();
                        lightningEvent2.smallFlash = true;
                        Farm farm = Game1.getFarm();
                        farm.lightningStrikeEvent.Fire(lightningEvent2);
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex) {
                StaticMonitor.Log($"Failed in {nameof(Utility_PerformLightningUpdate_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("enable-mod.label"),
                tooltip: () => Helper.Translation.Get("enable-mod.tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("disable-strikes.label"),
                tooltip: () => Helper.Translation.Get("disable-strikes.tooltip"),
                getValue: () => Config.DisableStrikes,
                setValue: value => Config.DisableStrikes = value
            );
        }

    }
}
