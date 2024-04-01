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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using System;
using StardewValley.Menus;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;
using AnythingAnywhere.Framework.UI;
using AnythingAnywhere.Framework.Managers;
using AnythingAnywhere.Framework.Patches.GameLocations;
using AnythingAnywhere.Framework.Patches.StandardObjects;
using AnythingAnywhere.Framework.Patches.Menus;
using StardewValley.TerrainFeatures;
using AnythingAnywhere.Framework.Patches.TerrainFeatures;

namespace AnythingAnywhere
{
    public class ModEntry : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
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

                // Apply the StandardObject patches
                new ObjectPatch(monitor, helper).Apply(harmony);
                new FurniturePatch(monitor, helper).Apply(harmony);
                new MiniJukeboxPatch(monitor, helper).Apply(harmony);

                // Apply the TerrainFeature patches
                new FruitTreePatch(monitor, helper).Apply(harmony);
                new TreePatch(monitor, helper).Apply(harmony);
                new HoeDirtPatch(monitor, helper).Apply(harmony);

                // Apply the Menu patches OLD
                // new CarpenterMenuPatch(monitor, helper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add debug commands
            helper.ConsoleCommands.Add("aa_remove_objects", "Removes all objects of a specified ID at a specified location.\n\nUsage: aa_remove_objects [LOCATION] [OBJECT_ID]", this.DebugRemoveObjects);

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Hook into Input events
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            modConfig = Helper.ReadConfig<ModConfig>();

            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGenericModConfigMenu(Helper))
            {
                var configApi = apiManager.GetGenericModConfigMenuApi();
                configApi.Register(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));

                // Register the furniture settings
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Placing_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnablePlacing, value => modConfig.EnablePlacing = value, I18n.Config_AnythingAnywhere_EnablePlacing_Name, I18n.Config_AnythingAnywhere_EnablePlacing_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableWallFurnitureIndoors, value => modConfig.EnableWallFurnitureIndoors = value, I18n.Config_AnythingAnywhere_EnableWallFurnitureIndoors_Name, I18n.Config_AnythingAnywhere_EnableWallFurnitureIndoors_Description);
                //configApi.AddBoolOption(ModManifest, () => modConfig.EnableRugTweaks, value => modConfig.EnableRugTweaks = value, I18n.Config_AnythingAnywhere_EnableRugTweaks_Name, I18n.Config_AnythingAnywhere_EnableRugTweaks_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableFreePlace, value => modConfig.EnableFreePlace = value, I18n.Config_AnythingAnywhere_EnableFreePlace_Name, I18n.Config_AnythingAnywhere_EnableFreePlace_Description);

                // Register the build settings
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Building_Title);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableBuilding, value => modConfig.EnableBuilding = value, I18n.Config_AnythingAnywhere_EnableBuilding_Name, I18n.Config_AnythingAnywhere_EnableBuilding_Description);
                configApi.AddKeybindList(ModManifest, () => modConfig.BuildMenu, value => modConfig.BuildMenu = value, I18n.Config_AnythingAnywhere_BuildMenu_Name, I18n.Config_AnythingAnywhere_BuildMenu_Description);
                configApi.AddKeybindList(ModManifest, () => modConfig.WizardBuildMenu, value => modConfig.WizardBuildMenu = value, I18n.Config_AnythingAnywhere_WizardBuildMenu_Name, I18n.Config_AnythingAnywhere_WizardBuildMenu_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableFreeBuild, value => modConfig.EnableFreeBuild = value, I18n.Config_AnythingAnywhere_EnableFreeBuild_Name, I18n.Config_AnythingAnywhere_EnableFreeBuild_Description);

                // Register the other settings
                configApi.AddSectionTitle(ModManifest, I18n.Config_AnythingAnywhere_Other_Title);
                //configApi.AddKeybindList(ModManifest, () => modConfig.TableTweakBind, value => modConfig.TableTweakBind = value, I18n.Config_AnythingAnywhere_TableTweakKeybind_Name, I18n.Config_AnythingAnywhere_TableTweakKeybind_Description);
                //configApi.AddBoolOption(ModManifest, () => modConfig.EnableTableTweak, value => modConfig.EnableTableTweak = value, I18n.Config_AnythingAnywhere_EnableTableTweak_Name, I18n.Config_AnythingAnywhere_EnableTableTweak_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnablePlanting, value => modConfig.EnablePlanting = value, I18n.Config_AnythingAnywhere_EnablePlanting_Name, I18n.Config_AnythingAnywhere_EnablePlanting_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableFruitTreeTweaks, value => modConfig.EnableFruitTreeTweaks = value, I18n.Config_AnythingAnywhere_EnableFruitTreeTweaks_Name, I18n.Config_AnythingAnywhere_EnableFruitTreeTweaks_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableWildTreeTweaks, value => modConfig.EnableWildTreeTweaks = value, I18n.Config_AnythingAnywhere_EnableWildTreeTweaks_Name, I18n.Config_AnythingAnywhere_EnableWildTreeTweaks_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.AllowMiniObelisksAnywhere, value => modConfig.AllowMiniObelisksAnywhere = value, I18n.Config_AnythingAnywhere_EnableMiniObilisk_Name, I18n.Config_AnythingAnywhere_EnableMiniObilisk_Description);
                configApi.AddBoolOption(ModManifest, () => modConfig.EnableJukeboxFunctionality, value => modConfig.EnableJukeboxFunctionality = value, I18n.Config_AnythingAnywhere_UseJukeboxAnywhere_Name, I18n.Config_AnythingAnywhere_UseJukeboxAnywhere_Description);
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
                activateBuildAnywhereMenu(builder);
            }
            else if (Game1.activeClickableMenu is BuildAnywhereMenu)
            {
                Game1.displayFarmer = true;
                ((BuildAnywhereMenu)Game1.activeClickableMenu).returnToCarpentryMenu();
                ((BuildAnywhereMenu)Game1.activeClickableMenu).exitThisMenu();
            }
        }

        private void activateBuildAnywhereMenu(string builder)
        {
            if (builder == "Wizard" && !Game1.getFarmer(Game1.player.UniqueMultiplayerID).hasMagicInk && !modConfig.EnableFreeBuild)
            {
                string message = I18n.Message_AnythingAnywhere_NoMagicInk();
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                return;
            }
            else if (!modConfig.EnableBuilding)
            {
                return;
            }
            else
            {
                Game1.activeClickableMenu = (IClickableMenu)new BuildAnywhereMenu(builder, modConfig, this.Monitor);
            }
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