/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework;
using AnythingAnywhere.Framework.External.CustomBush;
using AnythingAnywhere.Framework.Patches;
using AnythingAnywhere.Framework.Utilities;
using Common.Managers;
using Common.Utilities;
using Common.Utilities.Options;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AnythingAnywhere;
public class ModEntry : Mod
{
    public static IModHelper ModHelper { get; private set; } = null!;
    public static IMonitor ModMonitor { get; private set; } = null!;
    public static ModConfig Config { get; private set; } = null!;
    public static Multiplayer? Multiplayer { get; private set; }
    public static ICustomBushApi? CustomBushApi { get; private set; }

    public override void Entry(IModHelper helper)
    {
        // Setup the monitor, helper, config and multiplayer
        ModMonitor = Monitor;
        ModHelper = helper;
        Config = Helper.ReadConfig<ModConfig>();
        Multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

        // Initialize ConfigManager
        ConfigManager.Init(ModManifest, Config, ModHelper, ModMonitor, true);

        // Harmony Patches
        new BuildingPatches().Apply();
        new FarmingPatches().Apply();
        new PlacementPatches().Apply();
        new CabinAndHousePatches().Apply();
        new MiscPatches().Apply();

        // Add debug commands
        ConsoleCommands.RegisterCommands();

        // Hook into Game events
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += EventHandlers.OnSaveLoaded;
        helper.Events.World.BuildingListChanged += EventHandlers.OnBuildingListChanged;
        helper.Events.Input.ButtonsChanged += EventHandlers.OnButtonsChanged;
        helper.Events.Content.AssetRequested += EventHandlers.OnAssetRequested;
        helper.Events.GameLoop.UpdateTicked += EventHandlers.OnUpdateTicked;
        helper.Events.Player.Warped += EventHandlers.OnWarped;
        helper.Events.GameLoop.DayEnding += EventHandlers.OnDayEnding;

        // Hook into Custom events
        ButtonOptions.Click += EventHandlers.OnClick;
        ConfigUtility.ConfigChanged += EventHandlers.OnConfigChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (Helper.ModRegistry.IsLoaded("furyx639.CustomBush"))
        {
            CustomBushApi = ApiRegistry.GetApi<ICustomBushApi>("furyx639.CustomBush");
        }

        if (Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks"))
        {
            Config.MultipleMiniObelisks = true;
            ConfigUtility.SkipConfig(nameof(ModConfig.MultipleMiniObelisks));
        }

        if (Helper.ModRegistry.IsLoaded("Pathoschild.CropsAnytimeAnywhere"))
        {
            Config.EnableFarmingAnywhere = false;
            ConfigUtility.SkipConfig(nameof(ModConfig.EnableFarmingAnywhere));
        }

        if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu")) return;

        // Register the main page
        ConfigManager.AddPageLink("Placing");
        ConfigManager.AddPageLink("Building");
        ConfigManager.AddPageLink("Farming");
        ConfigManager.AddPageLink("House");
        ConfigManager.AddPageLink("Misc");

        // Register the placing settings
        ConfigManager.AddPage("Placing");
        ConfigManager.AddButtonOption("Placing", "ResetPage", fieldId: "Placing");
        ConfigManager.AddHorizontalSeparator();
        ConfigManager.AddOption(nameof(ModConfig.EnablePlacing));
        ConfigManager.AddOption(nameof(ModConfig.EnablePlaceAnywhere));
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
        ConfigManager.AddButtonOption("BlacklistedLocations", renderLeft: true, fieldId: "BlacklistCurrentLocation", afterReset: () => EventHandlers.ResetBlacklist());

        // Register the farming settings
        ConfigManager.AddPage("Farming");
        ConfigManager.AddButtonOption("Farming", "ResetPage", fieldId: "Farming");
        ConfigManager.AddHorizontalSeparator();
        ConfigManager.AddOption(nameof(ModConfig.EnableFarmingAnywhere));
        ConfigManager.AddOption(nameof(ModConfig.EnableCropsAnytime));
        ConfigManager.AddOption(nameof(ModConfig.EnableTreesAnytime));
        ConfigManager.AddOption(nameof(ModConfig.EnableBushesAnytime));
        ConfigManager.AddOption(nameof(ModConfig.EnableDiggingAll));
        ConfigManager.AddOption(nameof(ModConfig.EnableFruitTreeTweaks));
        ConfigManager.AddOption(nameof(ModConfig.EnableWildTreeTweaks));
        ConfigManager.AddOption(nameof(ModConfig.ForceGreenhouseTreeSprite));

        // Register the Cabin and Farmhouse settings
        ConfigManager.AddPage("House");
        ConfigManager.AddButtonOption("House", "ResetPage", fieldId: "House");
        ConfigManager.AddHorizontalSeparator();
        ConfigManager.AddOption(nameof(ModConfig.DisableHardCodedWarp));
        ConfigManager.AddOption(nameof(ModConfig.InstantHomeUpgrade));
        ConfigManager.AddOption(nameof(ModConfig.UpgradeCabins));
        ConfigManager.AddOption(nameof(ModConfig.RenovateCabins));
        ConfigManager.AddOption(nameof(ModConfig.CabinMenuButton));
        ConfigManager.AddOption(nameof(ModConfig.EnableFreeHouseUpgrade));
        ConfigManager.AddOption(nameof(ModConfig.EnableFreeRenovations));

        // Register the Misc settings
        ConfigManager.AddPage("Misc");
        ConfigManager.AddButtonOption("Misc", "ResetPage", fieldId: "Misc");
        ConfigManager.AddHorizontalSeparator();
        ConfigManager.AddOption(nameof(ModConfig.EnableCaskFunctionality));
        ConfigManager.AddOption(nameof(ModConfig.EnableFreeCommunityUpgrade));
        ConfigManager.AddOption(nameof(ModConfig.EnableJukeboxFunctionality));
        ConfigManager.AddOption(nameof(ModConfig.EnableGoldClockAnywhere));
        ConfigManager.AddOption(nameof(ModConfig.MultipleMiniObelisks));
    }
}