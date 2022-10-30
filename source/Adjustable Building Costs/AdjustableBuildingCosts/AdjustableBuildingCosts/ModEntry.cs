/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MiguelLucas/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using AdjustableBuildingCosts.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace AdjustableBuildingCosts
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        private int buildingDaysLeft = 0;
        private int upgradingDaysLeft = 0;

        private bool isBuilding = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }


        /*********
        ** Private methods
        *********/

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints")) {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;


                    foreach (string itemID in data.Keys) {

                        string[] fields = data[itemID].Split('/');

                        if (fields.Length > 8) {
                            string name = fields[8];
                            string formattedCost = "";

                            if (Config.Buildings.ContainsKey(name)) {
                                formattedCost = Config.Buildings[name].getFormattedBlueprintCost();
                                fields[0] = formattedCost;
                                data[itemID] = string.Join("/", fields);

                                this.Monitor.Log("Changed '" + name + "' costs to " + Config.Buildings[name].getFormattedBlueprintCost(), LogLevel.Debug);
                            }
                        }
                    }
                });
            }
        }

        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.NewMenu is CarpenterMenu) {

                // get field
                IList<BluePrint> blueprints = this.Helper.Reflection
                    .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                    .GetValue();

                
                foreach (BluePrint bluePrint in blueprints) {

                    if (Config.Buildings.ContainsKey(bluePrint.displayName)) {
                        bluePrint.moneyRequired = Config.Buildings[bluePrint.displayName].GoldCost;
                        bluePrint.daysToConstruct = Config.Buildings[bluePrint.displayName].DaysToBuild;

                        this.Monitor.Log("Changed blueprint '" + bluePrint.displayName + "' gold cost to " + bluePrint.moneyRequired + " and days to build to " + bluePrint.daysToConstruct, LogLevel.Debug);
                    }
                }

                ((CarpenterMenu) e.NewMenu).setNewActiveBlueprint();
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var buildings = Game1.getFarm().buildings;
            for (int i = 0; i < buildings.Count; i++) {
                if (buildings[i].daysOfConstructionLeft.Value > 0 || buildings[i].daysUntilUpgrade.Value > 0) {
                    //Monitor.Log("Setting isBuilding to true", LogLevel.Debug);
                    isBuilding = true;
                    break;
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var buildings = Game1.getFarm().buildings;

            for (int i = 0; i < buildings.Count; i++) {
                if (buildings[i].daysOfConstructionLeft.Value > 0) {
                    /*Monitor.Log("------------- Inside construction ------------", LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].nameOfIndoors, LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].daysUntilUpgrade.ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].daysOfConstructionLeft.ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].buildingType.Value, LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].getNameOfNextUpgrade(), LogLevel.Debug);*/

                    buildingDaysLeft = buildings[i].daysOfConstructionLeft.Value;

                    if (!isBuilding) {
                        buildingDaysLeft = Config.Buildings[buildings[i].buildingType.Value].DaysToBuild;
                        buildings[i].daysOfConstructionLeft.Value = buildingDaysLeft;
                        isBuilding = true;

                        Monitor.Log("Setting days to construct to " + buildings[i].daysOfConstructionLeft.Value + " for " + buildings[i].buildingType.Value, LogLevel.Debug);
                        break;
                    }

                }

                if (buildings[i].daysUntilUpgrade.Value > 0) {
                    /*Monitor.Log("------------ Inside upgrade -------------", LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].nameOfIndoors, LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].daysUntilUpgrade.ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].daysOfConstructionLeft.ToString(), LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].buildingType.Value, LogLevel.Debug);
                    Monitor.Log("Building -> " + buildings[i].getNameOfNextUpgrade(), LogLevel.Debug);*/

                    upgradingDaysLeft = buildings[i].daysUntilUpgrade.Value;
                    if (!isBuilding) {
                        upgradingDaysLeft = Config.Buildings[buildings[i].getNameOfNextUpgrade()].DaysToBuild;
                        buildings[i].daysUntilUpgrade.Value = upgradingDaysLeft;
                        isBuilding = true;

                        Monitor.Log("Setting days to upgrade to " + buildings[i].daysUntilUpgrade.Value + " for " + buildings[i].buildingType.Value, LogLevel.Debug);
                        break;
                    }
                }

                /*Monitor.Log("------------- General ------------", LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].ToString(), LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].nameOfIndoors, LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].daysUntilUpgrade.ToString(), LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].daysOfConstructionLeft.ToString(), LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].buildingType.Value, LogLevel.Debug);
                Monitor.Log("Building -> " + buildings[i].getNameOfNextUpgrade(), LogLevel.Debug);*/
               
            }

            if (upgradingDaysLeft <= 1 && buildingDaysLeft <= 1) {
                isBuilding = false;
                //Monitor.Log("Resetting daysLeft", LogLevel.Debug);
                buildingDaysLeft = 0;
                upgradingDaysLeft = 0;
            }

            /*Monitor.Log("isBuilding -> " + isBuilding, LogLevel.Debug);
            Monitor.Log("upgradingDaysLeft -> " + upgradingDaysLeft, LogLevel.Debug);
            Monitor.Log("buildingDaysLeft -> " + buildingDaysLeft, LogLevel.Debug);*/
        }
    }
}
