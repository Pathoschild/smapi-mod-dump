/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterJunimos.Patches;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using BetterJunimos.Utils;

namespace BetterJunimos {
    public class BetterJunimos : Mod {
        internal ModConfig Config;
        internal ProgressionData ProgData;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            // Only run if the master game
            if (!Game1.IsMasterGame) {
                return;
            }

            Config = helper.ReadConfig<ModConfig>();

            Util.Config = Config;
            Util.Reflection = helper.Reflection;

            Util.Abilities = new JunimoAbilities(Config.JunimoAbilites);
            helper.WriteConfig(Config);

            Util.Payments = new JunimoPayments(Config.JunimoPayment);
            Util.MaxRadius = Config.JunimoPayment.WorkForWages ? Util.UnpaidRadius : Config.JunimoHuts.MaxRadius;

            Util.Progression = new JunimoProgression(Config, Monitor, Helper);

            helper.Content.AssetEditors.Add(new JunimoEditor(helper.Content));
            helper.Content.AssetEditors.Add(new BlueprintEditor());

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            Helper.Events.GameLoop.GameLaunched += OnLaunched;

            DoHarmonyRegistration();
        }

        private void DoHarmonyRegistration() {
            HarmonyInstance harmony = HarmonyInstance.Create("com.hawkfalcon.BetterJunimos");
            // Thank you to Cat (danvolchek) for this harmony setup implementation
            // https://github.com/danvolchek/StardewMods/blob/master/BetterGardenPots/BetterGardenPots/BetterGardenPotsMod.cs#L29
            IList<Tuple<string, Type, Type>> replacements = new List<Tuple<string, Type, Type>>();

            // Junimo Harvester patches
            Type junimoType = typeof(JunimoHarvester);
            replacements.Add("foundCropEndFunction", junimoType, typeof(PatchFindingCropEnd));
            replacements.Add("tryToHarvestHere", junimoType, typeof(PatchHarvestAttemptToCustom));
            replacements.Add("update", junimoType, typeof(PatchJunimoShake));

            // improve pathfinding
            replacements.Add("pathfindToRandomSpotAroundHut", junimoType, typeof(PatchPathfind));
            replacements.Add("pathFindToNewCrop_doWork", junimoType, typeof(PatchPathfindDoWork));

            // Junimo Hut patches
            Type junimoHutType = typeof(JunimoHut);
            replacements.Add("areThereMatureCropsWithinRadius", junimoHutType, typeof(PatchSearchAroundHut));

            // replacements for hardcoded max junimos
            replacements.Add("Update", junimoHutType, typeof(ReplaceJunimoHutUpdate));
            replacements.Add("getUnusedJunimoNumber", junimoHutType, typeof(ReplaceJunimoHutNumber));
            replacements.Add("performTenMinuteAction", junimoHutType, typeof(ReplaceJunimoTimerNumber));

            foreach (Tuple<string, Type, Type> replacement in replacements) {
                MethodInfo original = replacement.Item2.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList().Find(m => m.Name == replacement.Item1);

                MethodInfo prefix = replacement.Item3.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Prefix");
                MethodInfo postfix = replacement.Item3.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Postfix");

                harmony.Patch(original, prefix == null ? null : new HarmonyMethod(prefix), postfix == null ? null : new HarmonyMethod(postfix));
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (!Context.IsWorldReady) { return; }

            if (e.Button == Config.Other.SpawnJunimoKeybind) {
                SpawnJunimoCommand();
            }
        }

        /// <summary>Raised after a the game is saved</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnSaving(object sender, SavingEventArgs e) {
            this.Helper.Data.WriteSaveData("hawkfalcon.BetterJunimos.ProgressionData", ProgData);
            Helper.WriteConfig(Config);
        }

        // BUG: player warps back to wizard hut after use
        private void OpenJunimoHutMenu() {
            CarpenterMenu menu = new CarpenterMenu(true);
            var blueprints = Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints");
            List<BluePrint> newBluePrints = new List<BluePrint>();
            newBluePrints.Add(new BluePrint("Junimo Hut"));
            blueprints.SetValue(newBluePrints);
            Game1.activeClickableMenu = (IClickableMenu)menu;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            // closed Junimo Hut menu
            if (e.OldMenu is ItemGrabMenu menu && menu.context is JunimoHut hut) {
                Util.Abilities.UpdateHutItems(Util.GetHutIdFromHut(hut));
                if (Config.JunimoPayment.WorkForWages) {
                    CheckForWages(hut);
                }
                if (Config.Progression.Enabled) {
                    CheckForProgressionItems(hut);
                }
            }

            // opened menu
            else if (e.OldMenu == null && e.NewMenu is CarpenterMenu) {
                if (Helper.Reflection.GetField<bool>(e.NewMenu, "magicalConstruction").GetValue())
                {
                    // limit to only junimo hut
                    if (!Game1.MasterPlayer.mailReceived.Contains("hasPickedUpMagicInk"))
                    {
                        OpenJunimoHutMenu();
                    }
                }
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnDayStarted(object sender, DayStartedEventArgs e) {
            if (Config.JunimoPayment.WorkForWages) {
                Util.Payments.JunimoPaymentsToday.Clear();
                Util.Payments.WereJunimosPaidToday = false;
                Util.MaxRadius = Util.UnpaidRadius;
            }
            var huts = Game1.getFarm().buildings.OfType<JunimoHut>();
            foreach (JunimoHut hut in huts) {
                CheckForWages(hut);
                CheckForProgressionItems(hut);
                Util.Abilities.UpdateHutItems(Util.GetHutIdFromHut(hut));
            }

            if (huts.Count() > 0) {
                Util.Progression.DayStartedProgressionPrompt(Game1.IsWinter, Game1.isRaining);
            }

            if (Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday && huts.Any()) {
                Util.SendMessage(Helper.Translation.Get("junimosWillNotWorkText"));
            }

            if (!Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas) {
                // reset for rainy days
                Helper.Content.InvalidateCache(@"Characters\Junimo");
            }
        }

        /// <summary>Raised after a building is added
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e) {
            foreach (Building building in e.Added) {
                if (building is JunimoHut hut) {
                    Util.Abilities.UpdateHutItems(Util.GetHutIdFromHut(hut));
                }
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnSaveLoaded(object sender, EventArgs e) {
            ProgData = this.Helper.Data.ReadSaveData<ProgressionData>("hawkfalcon.BetterJunimos.ProgressionData");
            if (ProgData is null) ProgData = new ProgressionData();
            Util.Progression.Initialize(ProgData);
            AllowJunimoHutPurchasing();
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e) {
            Config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api is null) return;
            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.SetDefaultIngameOptinValue(ModManifest, true);

            api.RegisterLabel(ModManifest, "Hut Settings", "");
            api.RegisterClampedOption(ModManifest, "Max Junimos", "", () => Config.JunimoHuts.MaxJunimos, (int val) => Config.JunimoHuts.MaxJunimos = val, 0, 25);
            api.RegisterClampedOption(ModManifest, "Max Radius", "", () => Config.JunimoHuts.MaxRadius, (int val) => Config.JunimoHuts.MaxRadius = val, 0, 25);
            api.RegisterSimpleOption(ModManifest, "Availible After CC Complete", "Availible After Community Center Complete", () => Config.JunimoHuts.AvailibleAfterCommunityCenterComplete, (bool val) => Config.JunimoHuts.AvailibleAfterCommunityCenterComplete = val);
            api.RegisterSimpleOption(ModManifest, "Availible Immediately", "", () => Config.JunimoHuts.AvailibleImmediately, (bool val) => Config.JunimoHuts.AvailibleImmediately = val);
            api.RegisterSimpleOption(ModManifest, "Reduced Cost To Construct", "", () => Config.JunimoHuts.ReducedCostToConstruct, (bool val) => Config.JunimoHuts.ReducedCostToConstruct = val);
            api.RegisterSimpleOption(ModManifest, "Free To Construct", "", () => Config.JunimoHuts.FreeToConstruct, (bool val) => Config.JunimoHuts.FreeToConstruct = val);

            api.RegisterLabel(ModManifest, "Improvements", "");
            api.RegisterSimpleOption(ModManifest, "Skills Progression", "Require each skill to be unlocked", () => Config.Progression.Enabled, (bool val) => Config.Progression.Enabled = val);
            api.RegisterSimpleOption(ModManifest, "Can Work In Rain", "", () => Config.JunimoImprovements.CanWorkInRain, (bool val) => Config.JunimoImprovements.CanWorkInRain = val);
            api.RegisterSimpleOption(ModManifest, "Can Work In Winter", "", () => Config.JunimoImprovements.CanWorkInWinter, (bool val) => Config.JunimoImprovements.CanWorkInWinter = val);
            api.RegisterSimpleOption(ModManifest, "Can Work In Evenings", "", () => Config.JunimoImprovements.CanWorkInEvenings, (bool val) => Config.JunimoImprovements.CanWorkInEvenings = val);
            api.RegisterSimpleOption(ModManifest, "Work Faster", "", () => Config.JunimoImprovements.WorkFaster, (bool val) => Config.JunimoImprovements.WorkFaster = val);
            api.RegisterSimpleOption(ModManifest, "Avoid Harvesting Flowers", "", () => Config.JunimoImprovements.AvoidHarvestingFlowers, (bool val) => Config.JunimoImprovements.AvoidHarvestingFlowers = val);
            api.RegisterSimpleOption(ModManifest, "Avoid Planting Coffee", "", () => Config.JunimoImprovements.AvoidPlantingCoffee, (bool val) => Config.JunimoImprovements.AvoidPlantingCoffee = val);

            api.RegisterLabel(ModManifest, "Payment", "");
            api.RegisterSimpleOption(ModManifest, "Work For Wages", "", () => Config.JunimoPayment.WorkForWages, (bool val) => Config.JunimoPayment.WorkForWages = val);
            api.RegisterClampedOption(ModManifest, "Foraged items", "", () => Config.JunimoPayment.DailyWage.ForagedItems, (int val) => Config.JunimoPayment.DailyWage.ForagedItems = val, 0, 20);
            api.RegisterClampedOption(ModManifest, "Flowers", "", () => Config.JunimoPayment.DailyWage.Flowers, (int val) => Config.JunimoPayment.DailyWage.Flowers = val, 0, 20);
            api.RegisterClampedOption(ModManifest, "Fruit", "", () => Config.JunimoPayment.DailyWage.Fruit, (int val) => Config.JunimoPayment.DailyWage.Fruit = val, 0, 20);
            api.RegisterClampedOption(ModManifest, "Wine", "", () => Config.JunimoPayment.DailyWage.Wine, (int val) => Config.JunimoPayment.DailyWage.Wine = val, 0, 20);

            api.RegisterLabel(ModManifest, "Other", "");
            api.RegisterClampedOption(ModManifest, "Rainy Spirit Factor", "Rainy Junimo Spirit Factor", () => Config.FunChanges.RainyJunimoSpiritFactor, (float val) => Config.FunChanges.RainyJunimoSpiritFactor = val, 0.0f, 1.0f, 0.05f);
            api.RegisterSimpleOption(ModManifest, "Always Have Umbrellas", "Junimos Always Have Leaf Umbrellas", () => Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas, (bool val) => Config.FunChanges.JunimosAlwaysHaveLeafUmbrellas = val);
            api.RegisterSimpleOption(ModManifest, "More Colorful Umbrellas", "More Colorful Leaf Umbrellas", () => Config.FunChanges.MoreColorfulLeafUmbrellas, (bool val) => Config.FunChanges.MoreColorfulLeafUmbrellas = val);
            api.RegisterSimpleOption(ModManifest, "Infinite Inventory", "Infinite Junimo Inventory", () => Config.FunChanges.InfiniteJunimoInventory, (bool val) => Config.FunChanges.InfiniteJunimoInventory = val);
            api.RegisterSimpleOption(ModManifest, "Spawn Junimo Keybind", "Spawn Junimo Keybind", () => Config.Other.SpawnJunimoKeybind, (SButton val) => Config.Other.SpawnJunimoKeybind = val);
            api.RegisterSimpleOption(ModManifest, "Receive Messages", "", () => Config.Other.ReceiveMessages, (bool val) => Config.Other.ReceiveMessages = val);
        }

        public void AllowJunimoHutPurchasing() {
            if (Config.JunimoHuts.AvailibleImmediately ||
                (Config.JunimoHuts.AvailibleAfterCommunityCenterComplete &&
                Game1.MasterPlayer.mailReceived.Contains("ccIsComplete"))) {
                Game1.player.hasMagicInk = true;
            }
        }

        public void SpawnJunimoCommand() {
            if (Game1.player.currentLocation.IsFarm) {
                Farm farm = Game1.getFarm();
                Random rand = new Random();

                IEnumerable<JunimoHut> huts = farm.buildings.OfType<JunimoHut>();
                if (huts.Count() <= 0) {
                    Util.SendMessage(Helper.Translation.Get("spawnJunimoCommandText"));
                    return;
                }
                JunimoHut hut = huts.ElementAt(rand.Next(0, huts.Count()));
                Util.SpawnJunimoAtPosition(Game1.player.Position, hut, rand.Next(4, 100));
            }
            else {
                Util.SendMessage(Helper.Translation.Get("spawnJunimoCommandText"));
            }
        }

        private void CheckForWages(JunimoHut hut) {
            if (!Util.Payments.WereJunimosPaidToday && Util.Payments.ReceivePaymentItems(hut)) {
                Util.Payments.WereJunimosPaidToday = true;
                Util.MaxRadius = Config.JunimoHuts.MaxRadius;
                Util.SendMessage(Helper.Translation.Get("checkForWagesText"));
            }
        }

        private void CheckForProgressionItems(JunimoHut hut) {
            if (!Config.Progression.Enabled) return;
            Util.Progression.ReceiveProgressionItems(hut);
        }

        public override object GetApi() {
            return new BetterJunimosApi();
        }
    }
}
