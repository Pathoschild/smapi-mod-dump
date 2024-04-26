/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using AnythingAnywhere.Framework.UI;
using AnythingAnywhere.Framework.Managers;
using AnythingAnywhere.Framework.Interfaces;
using AnythingAnywhere.Framework.Patches.Menus;
using AnythingAnywhere.Framework.Patches.Locations;
using AnythingAnywhere.Framework.Patches.GameLocations;
using AnythingAnywhere.Framework.Patches.StandardObjects;
using AnythingAnywhere.Framework.Patches.TerrainFeatures;
using System.Linq;
using System;

namespace AnythingAnywhere
{
    public class ModEntry : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ICustomBushApi customBushApi;
        internal static Multiplayer multiplayer;
        internal static ModConfig modConfig;

        // Managers
        internal static ApiManager apiManager;

        public override void Entry(IModHelper helper)
        {
            // Setup i18n
            I18n.Init(helper.Translation);

            // Setup the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Setup the manager
            apiManager = new ApiManager(monitor);

            // Load the Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply GameLocation patches
                new GameLocationPatch(monitor, helper).Apply(harmony);

                // Apply Location patches
                new FarmHousePatch(monitor, helper).Apply(harmony);

                // Apply Menu patches
                new AnimalQueryMenuPatch(monitor, helper).Apply(harmony);

                // Apply StandardObject patches
                new CaskPatch(monitor, helper).Apply(harmony);
                new FurniturePatch(monitor, helper).Apply(harmony);
                new MiniJukeboxPatch(monitor, helper).Apply(harmony);
                new ObjectPatch(monitor, helper).Apply(harmony);

                // Apply TerrainFeature patches
                new FruitTreePatch(monitor, helper).Apply(harmony);
                new TreePatch(monitor, helper).Apply(harmony);
                new HoeDirtPatch(monitor, helper).Apply(harmony);

            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add debug commands
            helper.ConsoleCommands.Add("aa_remove_objects", "Removes all objects of a specified ID at a specified location.\n\nUsage: aa_remove_objects [LOCATION] [OBJECT_ID]", this.DebugRemoveObjects);
            helper.ConsoleCommands.Add("aa_remove_furniture", "Removes all furniture of a specified ID at a specified location.\n\nUsage: aa_remove_objects [LOCATION] [FURNITURE_ID]", this.DebugRemoveFurniture);

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Hook into Input events
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            modConfig = Helper.ReadConfig<ModConfig>();

            if (Helper.ModRegistry.IsLoaded("furyx639.CustomBush") && apiManager.HookIntoCustomBush(Helper))
            {
                customBushApi = apiManager.GetCustomBushApi();
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGenericModConfigMenu(Helper))
            {
                var configApi = apiManager.GetGenericModConfigMenuApi();
                configApi.Register(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));

                // Register the main page
                configApi.AddPageLink(ModManifest, "PlacingPage", () => String.Concat("> ", I18n.Config_AnythingAnywhere_Placing_Title()));
                configApi.AddPageLink(ModManifest, "BuildingsPage", () => String.Concat("> ", I18n.Config_AnythingAnywhere_Building_Title()));
                configApi.AddPageLink(ModManifest, "FarmingPage", () => String.Concat("> ", I18n.Config_AnythingAnywhere_Farming_Title()));
                configApi.AddPageLink(ModManifest, "OtherPage", () => String.Concat("> ", I18n.Config_AnythingAnywhere_Other_Title()));

                // Register the placing settings
                configApi.AddPage(ModManifest, "PlacingPage", I18n.Config_AnythingAnywhere_Placing_Title);
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Placing_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnablePlacing, value => modConfig.EnablePlacing = value, I18n.Config_AnythingAnywhere_EnablePlacing_Name, I18n.Config_AnythingAnywhere_EnablePlacing_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableWallFurnitureIndoors, value => modConfig.EnableWallFurnitureIndoors = value, I18n.Config_AnythingAnywhere_EnableWallFurnitureIndoors_Name, I18n.Config_AnythingAnywhere_EnableWallFurnitureIndoors_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableRugRemovalBypass, value => modConfig.EnableRugRemovalBypass = value, I18n.Config_AnythingAnywhere_EnableRugRemovalBypass_Name, I18n.Config_AnythingAnywhere_EnableRugRemovalBypass_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableFreePlace, value => modConfig.EnableFreePlace = value, I18n.Config_AnythingAnywhere_EnableFreePlace_Name, I18n.Config_AnythingAnywhere_EnableFreePlace_Description);

                // Register the build settings
                configApi.AddPage(ModManifest, "BuildingsPage", I18n.Config_AnythingAnywhere_Building_Title);
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Building_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableBuilding, value => modConfig.EnableBuilding = value, I18n.Config_AnythingAnywhere_EnableBuilding_Name, I18n.Config_AnythingAnywhere_EnableBuilding_Description);
                configApi.AddKeybindList(ModManifest, () => modConfig.BuildMenu, value => modConfig.BuildMenu = value, I18n.Config_AnythingAnywhere_BuildMenu_Name, I18n.Config_AnythingAnywhere_BuildMenu_Description);
                configApi.AddKeybindList(ModManifest, () => modConfig.WizardBuildMenu, value => modConfig.WizardBuildMenu = value, I18n.Config_AnythingAnywhere_WizardBuildMenu_Name, I18n.Config_AnythingAnywhere_WizardBuildMenu_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableAnimalRelocate, value => modConfig.EnableAnimalRelocate = value, I18n.Config_AnythingAnywhere_AnimalRelocate_Name, I18n.Config_AnythingAnywhere_AnimalRelocate_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableBuildingIndoors, value => modConfig.EnableBuildingIndoors = value, I18n.Config_AnythingAnywhere_EnableBuildingIndoors_Name, I18n.Config_AnythingAnywhere_EnableBuildingIndoors_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableInstantBuild, value => modConfig.EnableInstantBuild = value, I18n.Config_AnythingAnywhere_EnableInstantBuild_Name, I18n.Config_AnythingAnywhere_EnableInstantBuild_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableBuildAnywhere, value => modConfig.EnableBuildAnywhere = value, I18n.Config_AnythingAnywhere_EnableBuildAnywhere_Name, I18n.Config_AnythingAnywhere_EnableBuildAnywhere_Description);

                // Register the farming settings
                configApi.AddPage(ModManifest, "FarmingPage", I18n.Config_AnythingAnywhere_Farming_Title);
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Farming_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnablePlanting, value => modConfig.EnablePlanting = value, I18n.Config_AnythingAnywhere_EnablePlanting_Name, I18n.Config_AnythingAnywhere_EnablePlanting_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableDiggingAll, value => modConfig.EnableDiggingAll = value, I18n.Config_AnythingAnywhere_EnableDiggingAll_Name, I18n.Config_AnythingAnywhere_EnableDiggingAll_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableFruitTreeTweaks, value => modConfig.EnableFruitTreeTweaks = value, I18n.Config_AnythingAnywhere_EnableFruitTreeTweaks_Name, I18n.Config_AnythingAnywhere_EnableFruitTreeTweaks_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableWildTreeTweaks, value => modConfig.EnableWildTreeTweaks = value, I18n.Config_AnythingAnywhere_EnableWildTreeTweaks_Name, I18n.Config_AnythingAnywhere_EnableWildTreeTweaks_Description);

                // Register the other settings
                configApi.AddPage(ModManifest, "OtherPage", I18n.Config_AnythingAnywhere_Other_Title);
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Other_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.BypassMagicInk, value => modConfig.BypassMagicInk = value, I18n.Config_AnythingAnywhere_BypassMagicInk_Name, I18n.Config_AnythingAnywhere_BypassMagicInk_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableCaskFunctionality, value => modConfig.EnableCaskFunctionality = value, I18n.Config_AnythingAnywhere_EnableCaskFunctionality_Name, I18n.Config_AnythingAnywhere_EnableCaskFunctionality_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableJukeboxFunctionality, value => modConfig.EnableJukeboxFunctionality = value, I18n.Config_AnythingAnywhere_UseJukeboxFunctionality_Name, I18n.Config_AnythingAnywhere_UseJukeboxFunctionality_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableCabinsAnywhere, value => modConfig.EnableCabinsAnywhere = value, I18n.Config_AnythingAnywhere_EnableCabinsAnywhere_Name, I18n.Config_AnythingAnywhere_EnableCabinsAnywhere_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.MultipleMiniObelisks, value => modConfig.MultipleMiniObelisks = value, I18n.Config_AnythingAnywhere_EnableMiniObilisk_Name, I18n.Config_AnythingAnywhere_EnableMiniObilisk_Description);
            }
        }
        
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (modConfig.BuildMenu.JustPressed() && modConfig.EnableBuilding)
                HandleInstantBuildButtonClick("Robin");

            if (modConfig.WizardBuildMenu.JustPressed() && modConfig.EnableBuilding)
                HandleInstantBuildButtonClick("Wizard");
        }

        private void HandleInstantBuildButtonClick(string builder)
        {
            if (Context.IsPlayerFree && Game1.activeClickableMenu == null)
            {
                ActivateBuildAnywhereMenu(builder);
            }
            else if (Game1.activeClickableMenu is BuildAnywhereMenu)
            {
                Game1.displayFarmer = true;
                ((BuildAnywhereMenu)Game1.activeClickableMenu).returnToCarpentryMenu();
                ((BuildAnywhereMenu)Game1.activeClickableMenu).exitThisMenu();
            }
        }

        private void ActivateBuildAnywhereMenu(string builder)
        {
            // Check if building is disabled
            if (!modConfig.EnableBuilding)
                return;

            // Check if indoors and building indoors is disabled
            if (!Game1.currentLocation.IsOutdoors && !modConfig.EnableBuildingIndoors)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Message_AnythingAnywhere_NoBuildingIndoors(), HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                return;
            }
            // Check if the builder is the Wizard and either the player doesn't have magic ink or the magic ink bypass is disabled
            bool magicInkCheck = !(Game1.getFarmer(Game1.player.UniqueMultiplayerID).hasMagicInk || modConfig.BypassMagicInk);
            if (builder == "Wizard" && magicInkCheck && !modConfig.EnableInstantBuild)
            {
                Game1.addHUDMessage(new HUDMessage(I18n.Message_AnythingAnywhere_NoMagicInk(), HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                return;
            }

            // If none of the above conditions are met, activate the BuildAnywhereMenu
            Game1.activeClickableMenu = new BuildAnywhereMenu(builder);
        }

        private void DebugRemoveFurniture(string command, string[] args)
        {
            if (args.Length <= 1)
            {
                Monitor.Log($"Missing required arguments: [LOCATION] [FURNITURE_ID]", LogLevel.Warn);
                return;
            }

            // check context
            if (!Context.IsWorldReady)
            {
                monitor.Log("You need to load a save to use this command.", LogLevel.Error);
                return;
            }

            // get target location
            var location = Game1.locations.FirstOrDefault(p => p.Name != null && p.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (location == null && args[0] == "current")
            {
                location = Game1.currentLocation;
            }
            if (location == null)
            {
                string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
                monitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
                return;
            }

            // remove objects
            int removed = 0;
            foreach (var pair in location.furniture.ToArray())
            {
                if (pair.QualifiedItemId == args[1])
                {
                    location.furniture.Remove(pair);
                    removed++;
                }
            }

            monitor.Log($"Command removed {removed} furniture objects at {location.NameOrUniqueName}", LogLevel.Info);
            return;
        }



        private void DebugRemoveObjects(string command, string[] args)
        {
            if (args.Length <= 1)
            {
                Monitor.Log($"Missing required arguments: [LOCATION] [OBJECT_ID]", LogLevel.Warn);
                return;
            }

            // check context
            if (!Context.IsWorldReady)
            {
                monitor.Log("You need to load a save to use this command.", LogLevel.Error);
                return;
            }

            // get target location
            var location = Game1.locations.FirstOrDefault(p => p.Name != null && p.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));
            if (location == null && args[0] == "current")
            {
                location = Game1.currentLocation;
            }
            if (location == null)
            {
                string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
                monitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
                return;
            }

            // remove objects
            int removed = 0;
            foreach ((Vector2 tile, var obj) in location.Objects.Pairs.ToArray())
            {
                if (obj.QualifiedItemId == args[1])
                {
                    location.Objects.Remove(tile);
                    removed++;
                }
            }

            monitor.Log($"Command removed {removed} objects at {location.NameOrUniqueName}", LogLevel.Info);
            return;
        }
    }
}