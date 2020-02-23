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

            helper.Content.AssetEditors.Add(new JunimoEditor(helper.Content));
            helper.Content.AssetEditors.Add(new BlueprintEditor());

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;

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
        void OnSaved(object sender, SavedEventArgs e) {
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
                Util.Abilities.UpdateHutItems(Util.GetHutIdFromHut(hut));
            }

            if (Config.JunimoPayment.WorkForWages && !Util.Payments.WereJunimosPaidToday && huts.Any()) {
                Util.SendMessage("Junimos will not work until they are paid");
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
            AllowJunimoHutPurchasing();
        }

        public void AllowJunimoHutPurchasing() {
            if (Config.JunimoHuts.AvailibleImmediately ||
                (Config.JunimoHuts.AvailibleAfterCommunityCenterComplete &&
                Game1.MasterPlayer.mailReceived.Contains("ccIsComplete"))) {
                Game1.player.hasMagicInk = true;
            }
        }

        public static void SpawnJunimoCommand() {
            if (Game1.player.currentLocation.IsFarm) {
                Farm farm = Game1.getFarm();
                Random rand = new Random();

                IEnumerable<JunimoHut> huts = farm.buildings.OfType<JunimoHut>();
                if (huts.Count() <= 0) {
                    Util.SendMessage("There must be a Junimo Hut to spawn a Junimo");
                    return;
                }
                JunimoHut hut = huts.ElementAt(rand.Next(0, huts.Count()));
                Util.SpawnJunimoAtPosition(Game1.player.Position, hut, rand.Next(4, 100));
            }
            else {
                Util.SendMessage("Can only spawn Junimos on Farm");
            }
        }

        private void CheckForWages(JunimoHut hut) {
            if (!Util.Payments.WereJunimosPaidToday && Util.Payments.ReceivePaymentItems(hut)) {
                Util.Payments.WereJunimosPaidToday = true;
                Util.MaxRadius = Config.JunimoHuts.MaxRadius;
                Util.SendMessage("Junimos are happy with their payment!");
            }
        }

        public override object GetApi() {
            return new BetterJunimosApi();
        }
    }
}
