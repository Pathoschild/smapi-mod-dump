/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

#nullable disable
global using SObject = StardewValley.Object;
using AnythingAnywhere.Framework;
using AnythingAnywhere.Framework.External.CustomBush;
using AnythingAnywhere.Framework.Patches.GameLocations;
using AnythingAnywhere.Framework.Patches.Locations;
using AnythingAnywhere.Framework.Patches.Menus;
using AnythingAnywhere.Framework.Patches.StandardObjects;
using AnythingAnywhere.Framework.Patches.TerrainFeatures;
using Common.Helpers;
using Common.Managers;
using Common.Utilities;
using Common.Utilities.Options;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnythingAnywhere
{
    public class ModEntry : Mod
    {
        public static IModHelper ModHelper { get; private set; }
        public static IMonitor ModMonitor { get; private set; }
        public static ModConfig Config { get; private set; }
        public static Multiplayer Multiplayer { get; private set; }
        private static ApiRegistry ApiManager { get; set; }
        public static ICustomBushApi CustomBushApi { get; private set; }
        public static bool IsRelocateFarmAnimalsLoaded { get; private set; }

        private static Harmony harmony;

        public override void Entry(IModHelper helper)
        {
            // Setup the monitor, helper, config and multiplayer
            ModMonitor = Monitor;
            ModHelper = helper;
            Config = Helper.ReadConfig<ModConfig>();
            Multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Setup the managers/handlers
            ApiManager = new ApiRegistry(helper, ModMonitor);

            // Load the Harmony patches
            harmony = new Harmony(ModManifest.UniqueID);
            PatchHelper.Init(harmony);

            // GameLocation
            new GameLocationPatch().Apply();

            // Location
            new FarmHousePatch().Apply();

            // Menu
            new CarpenterMenuPatch().Apply();
            new AnimalQueryMenuPatch().Apply();

            // StandardObject
            new CaskPatch().Apply();
            new FurniturePatch().Apply();
            new BedFurniturePatch().Apply();
            new MiniJukeboxPatch().Apply();
            new ObjectPatch().Apply();

            // TerrainFeature
            new FruitTreePatch().Apply();
            new TreePatch().Apply();

            // Add debug commands
            helper.ConsoleCommands.Add("aa_remove_objects", "Removes all objects of a specified ID at a specified location.\n\nUsage: aa_remove_objects [LOCATION] [OBJECT_ID]", this.DebugRemoveObjects);
            helper.ConsoleCommands.Add("aa_remove_furniture", "Removes all furniture of a specified ID at a specified location.\n\nUsage: aa_remove_furniture [LOCATION] [FURNITURE_ID]", this.DebugRemoveFurniture);
            helper.ConsoleCommands.Add("aa_active", "Lists all active locations.\n\nUsage: aa_active", this.DebugListActiveLocations);

            // Hook into Game events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += EventHandlers.OnSaveLoaded;
            helper.Events.World.BuildingListChanged += EventHandlers.OnBuildingListChanged;
            helper.Events.Input.ButtonsChanged += EventHandlers.OnButtonsChanged;
            helper.Events.Content.AssetRequested += EventHandlers.OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += EventHandlers.OnUpdateTicked;
            helper.Events.Player.Warped += EventHandlers.OnWarped;

            // Hook into Custom events
            ButtonOptions.Click += EventHandlers.OnClick;
            ConfigUtility.ConfigChanged += EventHandlers.OnConfigChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("furyx639.CustomBush"))
            {
                CustomBushApi = ApiManager.GetApi<ICustomBushApi>("furyx639.CustomBush");
            }

            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks"))
            {
                Config.MultipleMiniObelisks = true;
            }

            IsRelocateFarmAnimalsLoaded = Helper.ModRegistry.IsLoaded("mouahrara.RelocateFarmAnimals");
            if (IsRelocateFarmAnimalsLoaded)
            {
                Config.EnableAnimalRelocate = false;
            }

            ConfigManager.Initialize(ModManifest, Config, ModHelper, ModMonitor, true);
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                // Register the main page
                ConfigManager.AddPageLink("Placing");
                ConfigManager.AddPageLink("Building");
                ConfigManager.AddPageLink("Farming");
                ConfigManager.AddPageLink("Other");

                // Register the placing settings
                ConfigManager.AddPage("Placing");
                ConfigManager.AddButtonOption("Placing", "ResetPage", fieldId: "Placing");
                ConfigManager.AddHorizontalSeparator();
                ConfigManager.AddOption(nameof(ModConfig.EnablePlacing));
                ConfigManager.AddOption(nameof(ModConfig.EnableFreePlace));
                ConfigManager.AddOption(nameof(ModConfig.EnableWallFurnitureIndoors));
                ConfigManager.AddOption(nameof(ModConfig.EnableRugRemovalBypass));

                // Register the build settings
                ConfigManager.AddPage("Building");
                ConfigManager.AddButtonOption("Building", "ResetPage", fieldId: "Building");
                ConfigManager.AddHorizontalSeparator();
                ConfigManager.AddOption(nameof(ModConfig.EnableBuilding));
                ConfigManager.AddOption(nameof(ModConfig.EnableBuildAnywhere));
                ConfigManager.AddOption(nameof(ModConfig.EnableInstantBuild));
                ConfigManager.AddOption(nameof(ModConfig.EnableFreeBuild));
                ConfigManager.AddOption(nameof(ModConfig.BuildMenu));
                ConfigManager.AddOption(nameof(ModConfig.WizardBuildMenu));
                ConfigManager.AddOption(nameof(ModConfig.BuildModifier));
                ConfigManager.AddOption(nameof(ModConfig.EnableGreenhouse));
                ConfigManager.AddOption(nameof(ModConfig.RemoveBuildConditions));
                ConfigManager.AddOption(nameof(ModConfig.EnableBuildingIndoors));
                ConfigManager.AddOption(nameof(ModConfig.BypassMagicInk));
                ConfigManager.AddHorizontalSeparator();
                ConfigManager.AddButtonOption("BlacklistedLocations", renderLeft: true, fieldId: "BlacklistCurrentLocation", afterReset: afterReset);

                // Register the farming settings
                ConfigManager.AddPage("Farming");
                ConfigManager.AddButtonOption("Farming", "ResetPage", fieldId: "Farming");
                ConfigManager.AddHorizontalSeparator();
                ConfigManager.AddOption(nameof(ModConfig.EnablePlanting));
                ConfigManager.AddOption(nameof(ModConfig.EnableDiggingAll));
                ConfigManager.AddOption(nameof(ModConfig.EnableFruitTreeTweaks));
                ConfigManager.AddOption(nameof(ModConfig.EnableWildTreeTweaks));

                // Register the other settings
                ConfigManager.AddPage("Other");
                ConfigManager.AddButtonOption("Other", "ResetPage", fieldId: "Other");
                ConfigManager.AddHorizontalSeparator();
                if (!IsRelocateFarmAnimalsLoaded) ConfigManager.AddOption(nameof(ModConfig.EnableAnimalRelocate));
                ConfigManager.AddOption(nameof(ModConfig.EnableCaskFunctionality));
                ConfigManager.AddOption(nameof(ModConfig.EnableJukeboxFunctionality));
                ConfigManager.AddOption(nameof(ModConfig.EnableGoldClockAnywhere));
                ConfigManager.AddOption(nameof(ModConfig.MultipleMiniObelisks));
            }
        }

        private static readonly Action afterReset = () => EventHandlers.ResetBlacklist();

        private void DebugRemoveFurniture(string command, string[] args)
        {
            if (args.Length <= 1)
            {
                Monitor.Log("Missing required arguments: [LOCATION] [FURNITURE_ID]", LogLevel.Warn);
                return;
            }

            // check context
            if (!Context.IsWorldReady)
            {
                ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
                return;
            }

            // get target location
            var location = Game1.locations.FirstOrDefault(p => p.Name?.Equals(args[0], StringComparison.OrdinalIgnoreCase) == true);
            if (location == null && args[0] == "current")
            {
                location = Game1.currentLocation;
            }
            if (location == null)
            {
                string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
                ModMonitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
                return;
            }

            // remove objects
            int removed = 0;
            foreach (var pair in location.furniture.ToArray())
            {
                if (pair.QualifiedItemId != args[1]) continue;
                location.furniture.Remove(pair);
                removed++;
            }

            ModMonitor.Log($"Command removed {removed} furniture objects at {location.NameOrUniqueName}", LogLevel.Info);
        }

        private void DebugRemoveObjects(string command, string[] args)
        {
            if (args.Length <= 1)
            {
                Monitor.Log("Missing required arguments: [LOCATION] [OBJECT_ID]", LogLevel.Warn);
                return;
            }

            // check context
            if (!Context.IsWorldReady)
            {
                ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
                return;
            }

            // get target location
            var location = Game1.locations.FirstOrDefault(p => p.Name?.Equals(args[0], StringComparison.OrdinalIgnoreCase) == true);
            if (location == null && args[0] == "current")
            {
                location = Game1.currentLocation;
            }
            if (location == null)
            {
                string[] locationNames = (from loc in Game1.locations where !string.IsNullOrWhiteSpace(loc.Name) orderby loc.Name select loc.Name).ToArray();
                ModMonitor.Log($"Could not find a location with that name. Must be one of [{string.Join(", ", locationNames)}].", LogLevel.Error);
                return;
            }

            // remove objects
            int removed = 0;
            foreach ((Vector2 tile, var obj) in location.Objects.Pairs.ToArray())
            {
                if (obj.QualifiedItemId != args[1]) continue;
                location.Objects.Remove(tile);
                removed++;
            }

            ModMonitor.Log($"Command removed {removed} objects at {location.NameOrUniqueName}", LogLevel.Info);
        }

        private void DebugListActiveLocations(string command, string[] args)
        {
            if (args.Length > 0)
            {
                Monitor.Log("This command does not take any arguments", LogLevel.Warn);
                return;
            }

            if (!Context.IsWorldReady)
            {
                ModMonitor.Log("You need to load a save to use this command.", LogLevel.Error);
                return;
            }

            List<string> activeLocations = [];
            activeLocations.AddRange(from location in Game1.locations where location.isAlwaysActive.Value select location.Name);

            // Print out the comma-separated list of active locations
            string activeLocationsStr = string.Join(", ", activeLocations);
            ModMonitor.Log($"Active locations: {activeLocationsStr}", LogLevel.Info);
        }
    }
}