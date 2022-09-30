/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DecidedlyShared.APIs;
using DecidedlyShared.Logging;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBuilding.UI;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using IGenericModConfigMenuApi = DecidedlyShared.APIs.IGenericModConfigMenuApi;
using Patches = SmartBuilding.HarmonyPatches.Patches;
using SObject = StardewValley.Object;

namespace SmartBuilding
{
    public class ModEntry : Mod
    {
        // SMAPI gubbins.
        private static IModHelper helper = null!;
        private static IMonitor monitor = null!;
        private static Logger logger = null!;
        private static ModConfig config = null!;
        private ButtonActions buttonActions;

        // Debug stuff to make my life less painful when going through my pre-release checklist.
        private ConsoleCommand commands = null!;
        private int currentMouseX;
        private int currentMouseY;
        private IDynamicGameAssetsApi? dgaApi;

        // Helper utilities
        private DrawingUtils drawingUtils;
        private Toolbar gameToolbar;
        private ITapGiantCropsAPI? giantCropTapApi;
        private IdentificationUtils identificationUtils;

        // UI gubbins
        private Texture2D itemBox = null!;

        // Mod state
        private ModState modState;

        // Mod integrations.
        private IMoreFertilizersAPI? moreFertilizersApi;
        private PlacementUtils placementUtils;
        private PlayerUtils playerUtils;
        private Options.ItemStowingModes previousStowingMode;
        private IToolbarIconsApi? toolbarIconsApi;
        private Texture2D toolButtonsTexture;
        private ToolMenu toolMenuUi;
        private WorldUtils worldUtils;

        #region Asset Loading Gubbins

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Mods/SmartBuilding/ToolButtons"))
                e.LoadFromModFile<Texture2D>("assets/Buttons.png", AssetLoadPriority.Low);

            // if (e.Name.IsEquivalentTo("Mods/SmartBuilding/WindowSkin"))
            //     e.LoadFromModFile<Texture2D>("assets/WindowSkin.png", AssetLoadPriority.Medium);
        }

        #endregion

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            ModEntry.helper = helper;
            monitor = this.Monitor;
            logger = new Logger(monitor, helper.Translation);
            config = ModEntry.helper.ReadConfig<ModConfig>();

            // This is where we'll register with GMCM.
            ModEntry.helper.Events.GameLoop.GameLaunched += this.GameLaunched;

            // This is fired whenever input is changed, so we check for input here.
            ModEntry.helper.Events.Input.ButtonsChanged += this.OnInput;

            // This is used to have the queued builds draw themselves in the world.
            ModEntry.helper.Events.Display.RenderedWorld += this.RenderedWorld;

            // This is a huge mess, and is used to draw the building mode HUD, and build queue if enabled.
            ModEntry.helper.Events.Display.RenderedHud += this.RenderedHud;

            // This is purely for our rectangle quantity drawing.
            ModEntry.helper.Events.Display.Rendered += this.Rendered;

            // We need this to handle our custom UI events. The alternative is a Harmony patch, but that feels excessive.
            ModEntry.helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;

            // If the screen is changed, clear our painted tiles, because currently, placing objects is done on the current screen.
            ModEntry.helper.Events.Player.Warped += (sender, args) => { this.LeaveBuildMode(); };

            ModEntry.helper.Events.GameLoop.SaveLoaded += (sender, args) => { this.LeaveBuildMode(); };

            ModEntry.helper.Events.GameLoop.ReturnedToTitle += (sender, args) => { this.LeaveBuildMode(); };

            ModEntry.helper.Events.Content.AssetRequested += this.OnAssetRequested;

            // Load up our textures.
            this.toolButtonsTexture = helper.GameContent.Load<Texture2D>("Mods/SmartBuilding/ToolButtons");
            this.itemBox = helper.GameContent.Load<Texture2D>("LooseSprites/tailoring");

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // All of our Harmony patches to disable interactions while in build mode.
            harmony.Patch(
                AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
                new HarmonyMethod(typeof(Patches), nameof(Patches.PlacementAction_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                new HarmonyMethod(typeof(Patches), nameof(Patches.Chest_CheckForAction_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(FishPond), nameof(FishPond.doAction)),
                new HarmonyMethod(typeof(Patches), nameof(Patches.FishPond_DoAction_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(StorageFurniture), nameof(StorageFurniture.checkForAction)),
                new HarmonyMethod(typeof(Patches), nameof(Patches.StorageFurniture_DoAction_Prefix)));

            harmony.Patch(
                AccessTools.Method(typeof(StorageFurniture), nameof(StorageFurniture.checkForAction)),
                new HarmonyMethod(typeof(Patches), nameof(Patches.StorageFurniture_DoAction_Prefix)));
        }

        private void KillToolUi()
        {
            // If our tool UI is not null, we set it to disabled.
            if (this.toolMenuUi != null) this.toolMenuUi.Enabled = false;

            this.modState.ClearPaintedTiles();

            // And, if our UI exists, we kill it.
            if (Game1.onScreenMenus.Contains(this.toolMenuUi))
                Game1.onScreenMenus.Remove(this.toolMenuUi);
        }

        private void CreateToolUi()
        {
            // First, we create our list of buttons.
            var toolButtons = new List<ToolButton>
            {
                new(ButtonId.Draw, ButtonType.Tool, this.buttonActions.DrawClicked,
                    I18n.SmartBuilding_Buttons_Draw_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.Erase, ButtonType.Tool, this.buttonActions.EraseClicked,
                    I18n.SmartBuilding_Buttons_Erase_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.FilledRectangle, ButtonType.Tool, this.buttonActions.FilledRectangleClicked,
                    I18n.SmartBuilding_Buttons_FilledRectangle_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.Insert, ButtonType.Tool, this.buttonActions.InsertClicked,
                    I18n.SmartBuilding_Buttons_Insert_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.ConfirmBuild, ButtonType.Function, this.buttonActions.ConfirmBuildClicked,
                    I18n.SmartBuilding_Buttons_ConfirmBuild_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.ClearBuild, ButtonType.Function, this.buttonActions.ClearBuildClicked,
                    I18n.SmartBuilding_Buttons_ClearBuild_Tooltip(), this.toolButtonsTexture, this.modState),
                new(ButtonId.DrawnLayer, ButtonType.Layer, this.buttonActions.DrawnLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerDrawn_Tooltip(), this.toolButtonsTexture, this.modState,
                    TileFeature.Drawn),
                new(ButtonId.ObjectLayer, ButtonType.Layer, this.buttonActions.ObjectLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerObject_Tooltip(), this.toolButtonsTexture, this.modState,
                    TileFeature.Object),
                new(ButtonId.TerrainFeatureLayer, ButtonType.Layer, this.buttonActions.TerrainFeatureLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerTerrainfeature_Tooltip(), this.toolButtonsTexture, this.modState,
                    TileFeature.TerrainFeature),
                new(ButtonId.FurnitureLayer, ButtonType.Layer, this.buttonActions.FurnitureLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerFurniture_Tooltip(), this.toolButtonsTexture, this.modState,
                    TileFeature.Furniture)
            };

            // If we're enabling building mode, we create our UI, and set it to enabled.
            this.toolMenuUi = new ToolMenu(logger, this.toolButtonsTexture, toolButtons, this.modState);
            this.toolMenuUi.Enabled = true;

            // Then, if it isn't already in onScreenMenus, we add it.
            if (!Game1.onScreenMenus.Contains(this.toolMenuUi)) Game1.onScreenMenus.Add(this.toolMenuUi);
        }

        private void SetupModIntegrations()
        {
            // First, check whether More Fertilizers is installed.
            if (this.Helper.ModRegistry.IsLoaded("atravita.MoreFertilizers"))
                // I don't feel like I need a version check here, because this is v1.0 API stuff.
                // And then grab the API for atravita's More Fertilizers mod.
                try
                {
                    this.moreFertilizersApi =
                        this.Helper.ModRegistry.GetApi<IMoreFertilizersAPI>("atravita.MoreFertilizers");
                }
                catch (Exception e)
                {
                    logger.Exception(e);
                }

            // First, check whether DGA is installed.
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
                if (this.Helper.ModRegistry.Get("spacechase0.DynamicGameAssets") is IModInfo modInfo)
                {
                    if (modInfo.Manifest.Version.IsOlderThan("1.4.3"))
                    {
                        logger.Log("Installed version of DGA is too low. Please update to DGA v1.4.3 or higher.");
                        this.dgaApi = null;
                    }

                    // And then grab the API for Casey's DGA mod.
                    try
                    {
                        this.dgaApi =
                            this.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e);
                    }
                }

            // Check whether Tap Giant Crops is even installed
            if (helper.ModRegistry.IsLoaded("atravita.TapGiantCrops"))
                // No need to check against the version here, so we just try to get the API.
                try
                {
                    this.giantCropTapApi = this.Helper.ModRegistry.GetApi<ITapGiantCropsAPI>("atravita.TapGiantCrops");
                }
                catch (Exception e)
                {
                    logger.Exception(e);
                }

            // Check if Toolbar Icons is installed.
            if (helper.ModRegistry.IsLoaded("furyx639.ToolbarIcons"))
                if (this.Helper.ModRegistry.Get("furyx639.ToolbarIcons") is IModInfo modInfo)
                {
                    if (modInfo.Manifest.Version.IsOlderThan("2.3.0"))
                    {
                        logger.Log(
                            "Installed version of Toolbar Icons is too old. Please update it to 2.3.0 or higher.");
                        this.toolbarIconsApi = null;
                    }
                    else
                        // Try to get the API.
                        try
                        {
                            this.toolbarIconsApi =
                                this.Helper.ModRegistry.GetApi<IToolbarIconsApi>("furyx639.ToolbarIcons");
                        }
                        catch (Exception e)
                        {
                            logger.Exception(e);
                        }
                }
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi == null)
            {
                logger.Log(I18n.SmartBuilding_Warning_GmcmNotInstalled());

                return;
            }

            configMenuApi.Register(this.ModManifest,
                () => config = new ModConfig(),
                () => this.Helper.WriteConfig(config));

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_Keybinds_Title()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_Keybinds_Paragraph_GmcmWarning()
            );

            this.RegisterMandatoryKeybinds(configMenuApi);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_OptionalToggles_Title()
            );

            this.RegisterToggleSettings(configMenuApi);

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_OptionalKeybinds_Title()
            );

            this.RegisterOptionalKeybinds(configMenuApi);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => "" // This is purely for spacing.
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_CheatyOptions_Title()
            );

            this.RegisterCheatyToggleOptions(configMenuApi);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_Debug_Title()
            );

            this.RegisterDebugSettings(configMenuApi);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_PotentiallyDangerous_Title()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_PotentiallyDangerous_Paragraph()
            );

            this.RegisterDangerousSettings(configMenuApi);

            configMenuApi.AddPageLink(
                this.ModManifest,
                "JsonGuide",
                () => I18n.SmartBuilding_Settings_JsonGuide_PageLink()
            );

            configMenuApi.AddPage(
                this.ModManifest,
                "JsonGuide",
                () => I18n.SmartBuilding_Settings_JsonGuide_PageTitle()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_JsonGuide_Guide1()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_JsonGuide_Guide2()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_JsonGuide_Guide3()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_JsonGuide_Guide4()
            );

            configMenuApi.AddParagraph(
                this.ModManifest,
                () => I18n.SmartBuilding_Settings_JsonGuide_Guide5()
            );
        }

        private void RegisterDangerousSettings(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture(),
                tooltip: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture_Tooltip(),
                getValue: () => config.EnablePlacingStorageFurniture,
                setValue: value => config.EnablePlacingStorageFurniture = value
            );
        }

        private void RegisterDebugSettings(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_EnableDebugCommand(),
                getValue: () => config.EnableDebugCommand,
                setValue: value => config.EnableDebugCommand = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_EnableDebugKeybinds(),
                getValue: () => config.EnableDebugControls,
                setValue: value => config.EnableDebugControls = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_IdentifyProducerToConsole(),
                getValue: () => config.IdentifyProducer,
                setValue: value => config.IdentifyProducer = value);

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_IdentifyHeldItemToConsole(),
                getValue: () => config.IdentifyItem,
                setValue: value => config.IdentifyItem = value);
        }

        private void RegisterCheatyToggleOptions(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_CrabPotsInAnyWaterTile(),
                getValue: () => config.CrabPotsInAnyWaterTile,
                setValue: value => config.CrabPotsInAnyWaterTile = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnablePlantingCrops(),
                getValue: () => config.EnablePlantingCrops,
                setValue: value => config.EnablePlantingCrops = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableFertilisers(),
                getValue: () => config.EnableFertilizers,
                setValue: value => config.EnableFertilizers = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableTreeTappers(),
                getValue: () => config.EnableTreeTappers,
                setValue: value => config.EnableTreeTappers = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableInsertingItemsIntoMachines(),
                getValue: () => config.EnableInsertingItemsIntoMachines,
                setValue: value => config.EnableInsertingItemsIntoMachines = value
            );
        }

        private void RegisterToggleSettings(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_ShowBuildQueue(),
                getValue: () => config.ShowBuildQueue,
                setValue: value => config.ShowBuildQueue = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_InstantlyBuild(),
                getValue: () => config.InstantlyBuild,
                setValue: value => config.InstantlyBuild = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_CanDestroyChests(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_CanDestroyChests_Tooltip(),
                getValue: () => config.CanDestroyChests,
                setValue: value => config.CanDestroyChests = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalTogglesMoreLaxObjectPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalTogglesMoreLaxObjectPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveObjectPlacement,
                setValue: value => config.LessRestrictiveObjectPlacement = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFloorPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFloorPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveFloorPlacement,
                setValue: value => config.LessRestrictiveFloorPlacement = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFurniturePlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFurniturePlacement_Tooltip(),
                getValue: () => config.LessRestrictiveFurniturePlacement,
                setValue: value => config.LessRestrictiveFurniturePlacement = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxBedPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxBedPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveBedPlacement,
                setValue: value => config.LessRestrictiveBedPlacement = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFloors(),
                getValue: () => config.EnableReplacingFloors,
                setValue: value => config.EnableReplacingFloors = value
            );

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFences(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFences_Tooltip(),
                getValue: () => config.EnableReplacingFences,
                setValue: value => config.EnableReplacingFences = value);
        }

        private void RegisterOptionalKeybinds(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_DrawTool(),
                getValue: () => config.DrawTool,
                setValue: value => config.DrawTool = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_EraseTool(),
                getValue: () => config.EraseTool,
                setValue: value => config.EraseTool = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FilledRectangleTool(),
                getValue: () => config.FilledRectangleTool,
                setValue: value => config.FilledRectangleTool = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_InsertTool(),
                getValue: () => config.InsertTool,
                setValue: value => config.InsertTool = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_CommitBuild(),
                getValue: () => config.CommitBuild,
                setValue: value => config.CommitBuild = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_ClearBuild(),
                getValue: () => config.CancelBuild,
                setValue: value => config.CancelBuild = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_DrawnLayer(),
                getValue: () => config.DrawnLayer,
                setValue: value => config.DrawnLayer = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_ObjectLayer(),
                getValue: () => config.ObjectLayer,
                setValue: value => config.ObjectLayer = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FloorLayer(),
                getValue: () => config.FloorLayer,
                setValue: value => config.FloorLayer = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FurnitureLayer(),
                getValue: () => config.FurnitureLayer,
                setValue: value => config.FurnitureLayer = value
            );
        }

        private void RegisterMandatoryKeybinds(IGenericModConfigMenuApi configMenuApi)
        {
            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_EnterBuildMode(),
                getValue: () => config.EngageBuildMode,
                setValue: value => config.EngageBuildMode = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToDraw(),
                getValue: () => config.HoldToDraw,
                setValue: value => config.HoldToDraw = value
            );

            configMenuApi.AddKeybindList(
                this.ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToMoveUi(),
                getValue: () => config.HoldToMoveMenu,
                setValue: value => config.HoldToMoveMenu = value
            );
        }

        private List<Vector2> CalculateRectangle(Vector2 cornerOne, Vector2 cornerTwo, Item item)
        {
            Vector2 topLeft;
            Vector2 bottomRight;
            var tiles = new List<Vector2>();
            int itemsRemainingInStack = 0;
            int originalAmountInStack = 0;

            if (item != null)
            {
                itemsRemainingInStack = item.Stack;
                originalAmountInStack = itemsRemainingInStack;
            }
            else
            {
                itemsRemainingInStack = 0;
                originalAmountInStack = itemsRemainingInStack;
            }

            topLeft = new Vector2(MathF.Min(cornerOne.X, cornerTwo.X), MathF.Min(cornerOne.Y, cornerTwo.Y));
            bottomRight = new Vector2(MathF.Max(cornerOne.X, cornerTwo.X), MathF.Max(cornerOne.Y, cornerTwo.Y));

            int rectWidth = (int)bottomRight.X - (int)topLeft.X + 1;
            int rectHeight = (int)bottomRight.Y - (int)topLeft.Y + 1;

            for (int x = (int)topLeft.X; x < rectWidth + topLeft.X; x++)
            for (int y = (int)topLeft.Y; y < rectHeight + topLeft.Y; y++)
                if (itemsRemainingInStack > 0)
                    if (this.placementUtils.CanBePlacedHere(new Vector2(x, y), item) &&
                        !this.modState.TilesSelected.Keys.Contains(new Vector2(x, y)))
                    {
                        tiles.Add(new Vector2(x, y));
                        itemsRemainingInStack--;
                    }

            return tiles;
        }

        /// <summary>
        ///     Confirm the drawn build, and pass tiles and items into <see cref="WorldUtils.PlaceObject" />.
        /// </summary>
        public void ConfirmBuild()
        {
            // The build has been confirmed, so we iterate through our Dictionary, and pass each tile into PlaceObject.
            foreach (var v in this.modState.TilesSelected)
            {
                // We want to allow placement for the duration of this method.
                Patches.AllowPlacement = true;

                this.worldUtils.PlaceObject(v);

                // And disallow it afterwards.
                Patches.AllowPlacement = false;
            }

            // Then, we clear the list, because building is done, and all errors are handled internally.
            this.modState.TilesSelected.Clear();
        }

        /// <summary>
        ///     Clear all painted tiles.
        /// </summary>
        public void ClearBuild()
        {
            this.modState.ClearPaintedTiles();
        }

        public void AddItem(Item item, Vector2 v)
        {
            // If we're not in building mode, we do nothing.
            if (!this.modState.BuildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;

            // If inserting items is disabled, we do nothing.
            if (!config.EnableInsertingItemsIntoMachines)
            {
                logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnableInsertingItemsIntoMachines_Disabled(),
                    LogLevel.Trace, true);
                return;
            }

            // There is no queue for item insertion, so we simply try to insert.
            this.modState.TryToInsertHere(v, item, this);
        }

        #region SMAPI Events

        private void GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // We need a reference to the toolbar in order to detect if the cursor is over it or not.
            foreach (var menu in Game1.onScreenMenus)
                if (menu is Toolbar)
                    this.gameToolbar = (Toolbar)menu;

            // Then we register with GMCM.
            this.RegisterWithGmcm();

            // Set up our mod integrations.
            this.SetupModIntegrations();

            // Set up our helpers.
            this.drawingUtils = new DrawingUtils();
            this.identificationUtils =
                new IdentificationUtils(helper, logger, config, this.dgaApi, this.moreFertilizersApi,
                    this.placementUtils);
            this.placementUtils = new PlacementUtils(config, this.identificationUtils, this.moreFertilizersApi,
                this.giantCropTapApi, logger, helper);
            this.playerUtils = new PlayerUtils(logger);
            this.worldUtils = new WorldUtils(this.identificationUtils, this.placementUtils, this.playerUtils,
                this.giantCropTapApi, config, logger, this.moreFertilizersApi);
            this.modState = new ModState(logger, this.playerUtils, this.identificationUtils, this.worldUtils,
                this.placementUtils);
            this.buttonActions = new ButtonActions(this, this.modState); // Ew, no. Fix this ugly nonsense later.

            // Set up our console commands.
            this.commands = new ConsoleCommand(logger, this, this.dgaApi, this.identificationUtils);
            this.Helper.ConsoleCommands.Add("sb_test", I18n.SmartBuilding_Commands_Debug_SbTest(),
                this.commands.TestCommand);
            this.Helper.ConsoleCommands.Add("sb_identify_all_items",
                I18n.SmartBuilding_Commands_Debug_SbIdentifyItems(),
                this.commands.IdentifyItemsCommand);
            this.Helper.ConsoleCommands.Add("sb_identify_cursor_target", "Identify targets under the cursor.",
                this.commands.IdentifyCursorTarget);

            // Then get the initial state of the item stowing mode setting.
            this.previousStowingMode = Game1.options.stowingMode;

            if (this.toolbarIconsApi != null)
            {
                this.toolbarIconsApi.AddToolbarIcon("smart-building.toggle-build-mode",
                    "Mods/SmartBuilding/ToolButtons",
                    Ui.GetButtonSourceRect(ButtonId.Draw),
                    I18n.SmartBuilding_Integrations_ToolbarIcons_Tooltip());
                this.toolbarIconsApi.ToolbarIconPressed += (o, s) =>
                {
                    if (s.Equals("smart-building.toggle-build-mode")) this.ToggleBuildMode();
                };
            }
        }

        /// <summary>
        ///     SMAPI's <see cref="IGameLoopEvents.UpdateTicking" /> event.
        /// </summary>
        private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            if (this.toolMenuUi != null)
                // If our tool menu is enabled and there's no menu up, we go forward with processing its events.
                if (this.toolMenuUi.Enabled && Game1.activeClickableMenu == null)
                {
                    var mouseState = Game1.input.GetMouseState();
                    this.currentMouseX = mouseState.X;
                    this.currentMouseY = mouseState.Y;

                    this.currentMouseX = (int)MathF.Floor(this.currentMouseX / Game1.options.uiScale);
                    this.currentMouseY = (int)MathF.Floor(this.currentMouseY / Game1.options.uiScale);

                    // We need to process our custom middle click held event.
                    // if (mouseState.MiddleButton == ButtonState.Pressed && Game1.oldMouseState.MiddleButton == ButtonState.Pressed)
                    if (config.HoldToMoveMenu.IsDown())
                        this.toolMenuUi.MiddleMouseHeld(this.currentMouseX, this.currentMouseY);
                    // if (mouseState.MiddleButton == ButtonState.Released)
                    if (!config.HoldToMoveMenu.IsDown())
                        this.toolMenuUi.MiddleMouseReleased(this.currentMouseX, this.currentMouseY);

                    // Do our hover event.
                    this.toolMenuUi.DoHover(this.currentMouseX, this.currentMouseY);

                    this.toolMenuUi.SetCursorHoverState(this.currentMouseX, this.currentMouseY);

                    // We also need to manually call a click method, because by default, it'll only work if the bounds of the IClickableMenu contain the cursor.
                    // We specifically do not want the bounds to be expanded to include the side layer buttons, however, because that will be far too large a boundary.
                    if ((mouseState.LeftButton == ButtonState.Pressed &&
                         Game1.oldMouseState.LeftButton == ButtonState.Released) ||
                        config.HoldToDraw.JustPressed())
                        this.toolMenuUi.ReceiveLeftClick(this.currentMouseX, this.currentMouseY);
                }
        }

        /// <summary>
        ///     SMAPI's <see cref="IInputEvents.ButtonsChanged"> event.
        /// </summary>
        private void OnInput(object? sender, ButtonsChangedEventArgs e)
        {
            // If the world isn't ready, we definitely don't want to do anything.
            if (!Context.IsWorldReady)
                return;

            // If a menu is up, we don't want any of our controls to do anything.
            if (Game1.activeClickableMenu != null)
                return;

            if (config.EnableDebugControls)
            {
                if (config.IdentifyItem.JustPressed())
                {
                    // We now want to identify the currently held item.
                    var player = Game1.player;

                    if (player.CurrentItem != null)
                        if (player.CurrentItem is not Tool)
                        {
                            var type = this.identificationUtils.IdentifyItemType((SObject)player.CurrentItem);
                            var item = player.CurrentItem;

                            logger.Log($"{I18n.SmartBuilding_Message_ItemName()}");
                            logger.Log($"\t{item.Name}");
                            logger.Log($"{I18n.SmartBuilding_Message_ItemParentSheetIndex()}");
                            logger.Log($"\t{item.ParentSheetIndex}");
                            logger.Log($"{I18n.SmartBuilding_Message_ItemCategory()}");
                            logger.Log($"\t{item.Category}");
                            logger.Log($"{I18n.SmartBuilding_Message_ItemType()}");
                            logger.Log($"\t{(item as SObject).Type}");
                            logger.Log($"{I18n.SmartBuilding_Message_ItemSmartBuildingType()}");
                            logger.Log($"\t{type}.");
                            logger.Log("");
                        }
                }

                if (config.IdentifyProducer.JustPressed())
                {
                    // We're trying to identify the type of producer under the cursor.
                    var here = Game1.currentLocation;
                    var targetTile = Game1.currentCursorTile;

                    if (here.objects.ContainsKey(targetTile))
                    {
                        var producer = here.objects[targetTile];
                        var type = this.identificationUtils.IdentifyProducer(producer);

                        logger.Log($"Identified producer {producer.Name} as {type}.");
                        logger.Log($"{I18n.SmartBuilding_Message_ProducerBeingIdentified()} {producer.Name}");
                        logger.Log($"{I18n.SmartBuilding_Message_IdentifiedProducerType()}: {type}");
                    }
                }
            }

            // If the player presses to engage build mode, we flip the bool.
            if (config.EngageBuildMode.JustPressed()) this.ToggleBuildMode();

            // Handle our tool hotkeys.
            if (this.modState.BuildingMode)
                // We're in building mode, but we need to check to see if our UI has been instantiated.
                if (this.toolMenuUi != null)
                    // It's not null, so we check to see if it's enabled.
                    if (this.toolMenuUi.Enabled)
                    {
                        // It's enabled, so now we go through our tool hotkeys.
                        if (config.DrawTool.JustPressed()) this.modState.ActiveTool = ButtonId.Draw;

                        if (config.EraseTool.JustPressed()) this.modState.ActiveTool = ButtonId.Erase;

                        if (config.FilledRectangleTool.JustPressed())
                            this.modState.ActiveTool = ButtonId.FilledRectangle;

                        if (config.InsertTool.JustPressed()) this.modState.ActiveTool = ButtonId.Insert;

                        if (config.CommitBuild.JustPressed()) this.ConfirmBuild();

                        if (config.CancelBuild.JustPressed()) this.ClearBuild();

                        // Now we only want and need to go through our layer hotkeys if the active tool is the eraser.
                        if (this.modState.ActiveTool == ButtonId.Erase)
                        {
                            // It is, so we go through them.
                            if (config.DrawnLayer.JustPressed()) this.modState.SelectedLayer = TileFeature.Drawn;

                            if (config.ObjectLayer.JustPressed()) this.modState.SelectedLayer = TileFeature.Object;

                            if (config.FloorLayer.JustPressed())
                                this.modState.SelectedLayer = TileFeature.TerrainFeature;

                            if (config.FurnitureLayer.JustPressed())
                                this.modState.SelectedLayer = TileFeature.Furniture;
                        }
                    }

            // If the player is attempting to draw placeables in the world.
            if (config.HoldToDraw.IsDown())
            {
                if (this.modState.BuildingMode)
                    // We don't want to do anything here if we're hovering over the menu, or the toolbar.
                    if (!this.modState.BlockMouseInteractions &&
                        !this.gameToolbar.isWithinBounds(this.currentMouseX, this.currentMouseY))
                        // First, we need to make sure there even is a tool active.
                        if (this.modState.ActiveTool != ButtonId.None)
                            // There is, so we want to determine exactly which tool we're working with.
                            switch (this.modState.ActiveTool)
                            {
                                case ButtonId.Draw:
                                    // We don't want to draw if the cursor is in the negative.
                                    if (Game1.currentCursorTile.X < 0 || Game1.currentCursorTile.Y < 0)
                                        return;

                                    this.modState.AddTile(Game1.player.CurrentItem, Game1.currentCursorTile, this);
                                    if (config.InstantlyBuild) this.buttonActions.ConfirmBuildClicked();
                                    break;
                                case ButtonId.Erase:
                                    // if (modState.SelectedLayer.HasValue)
                                    // {
                                    this.worldUtils.DemolishOnTile(Game1.currentCursorTile,
                                        this.modState.SelectedLayer);
                                    this.modState.EraseTile(Game1.currentCursorTile, this);
                                    // }
                                    break;
                                case ButtonId.FilledRectangle:

                                    // This is a split method and is hideous, but this is the best I can think of for now.
                                    this.modState.RectangleItem = Game1.player.CurrentItem;

                                    if (this.modState.StartTile == null)
                                        // If the start tile hasn't yet been set, then we want to set that.
                                        this.modState.StartTile = Game1.currentCursorTile;

                                    this.modState.EndTile = Game1.currentCursorTile;

                                    this.modState.RectTiles = this.CalculateRectangle(
                                        this.modState.StartTile.Value, this.modState.EndTile.Value,
                                        this.modState.RectangleItem);

                                    break;
                                case ButtonId.Insert:
                                    this.AddItem(Game1.player.CurrentItem, Game1.currentCursorTile);
                                    break;
                            }
            }
            else if (config.HoldToDraw.GetState() == SButtonState.Released)
                // We don't care to do this if there's no tool active.
                if (this.modState.ActiveTool != ButtonId.None)
                    if (this.modState.ActiveTool == ButtonId.FilledRectangle)
                        // We need to process the key up stuff for the filled rectangle.
                        // The rectangle drawing key was released, so we want to calculate the tiles within, and set CurrentlyDrawing to false.
                        if (this.modState.StartTile.HasValue && this.modState.EndTile.HasValue)
                        {
                            var tiles = this.CalculateRectangle(this.modState.StartTile.Value,
                                this.modState.EndTile.Value,
                                this.modState.RectangleItem);

                            foreach (var tile in tiles) this.modState.AddTile(this.modState.RectangleItem, tile, this);

                            this.modState.StartTile = null;
                            this.modState.EndTile = null;
                            this.modState.RectTiles.Clear();
                        }
            // Otherwise, the key is up, meaning we want to indicate we're not currently drawing.
            //CurrentlyDrawing = false;
        }

        private void ToggleBuildMode()
        {
            if (!this.modState.BuildingMode)
                this.EnterBuildMode();
            else
                this.LeaveBuildMode();
        }

        private void EnterBuildMode()
        {
            // If the world isn't ready, we return.
            if (!Context.IsWorldReady)
                return;

            // If it's a festival, we return.
            if (Game1.isFestival())
                return;

            this.modState.BuildingMode = true;
            // We're entering building mode, so we create our UI.
            this.CreateToolUi();

            // First we save the current item stowing mode
            this.previousStowingMode = Game1.options.stowingMode;

            // Then we set it to off to avoid a strange stuttery drawing issue.
            Game1.options.stowingMode = Options.ItemStowingModes.Off;
        }

        private void LeaveBuildMode()
        {
            this.modState.BuildingMode = false;

            // Kill our UI.
            this.KillToolUi();

            // And set our active tool and layer to none.
            this.modState.ActiveTool = ButtonId.None;
            this.modState.SelectedLayer = TileFeature.None;

            // Reset the state of the mod.
            this.modState.ResetState();

            // Then, finally, set the stowing mode back to what it used to be.
            Game1.options.stowingMode = this.previousStowingMode;
        }

        /// <summary>
        ///     SMAPI's <see cref="IDisplayEvents.RenderedWorld" /> event.
        /// </summary>
        private void Rendered(object? sender, RenderedEventArgs e)
        {
            if (this.modState.BuildingMode)
                // Now, we render our rectangle quantity amount.
                if (this.modState.RectTiles != null)
                    foreach (var tile in this.modState.RectTiles)
                        Utility.drawTinyDigits(this.modState.RectTiles.Count,
                            e.SpriteBatch,
                            // new Vector2(100, 100),
                            new Vector2(Game1.getMouseX() + 38, Game1.getMouseY() + 86),
                            3f,
                            -10f,
                            Color.White);
        }

        /// <summary>
        ///     Render the drawn queue in the world.
        /// </summary>
        private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            var b = e.SpriteBatch;

            foreach (var item in this.modState.TilesSelected)
                // Here, we simply have the Item draw itself in the world.
                item.Value.Item.drawInMenu
                (e.SpriteBatch,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        item.Key * Game1.tileSize),
                    1f, 1f, 4f, StackDrawType.Hide);

            if (this.modState.RectTiles != null)
                foreach (var tile in this.modState.RectTiles)
                    // Here, we simply have the Item draw itself in the world.
                    this.modState.RectangleItem.drawInMenu
                    (e.SpriteBatch,
                        Game1.GlobalToLocal(
                            Game1.viewport,
                            tile * Game1.tileSize),
                        1f, 1f, 4f, StackDrawType.Hide);
        }

        /// <summary>
        ///     SMAPI's <see cref="IDisplayEvents.RenderedHud" /> event.
        /// </summary>
        private void RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            // There's absolutely no need to run this while we're not in building mode.
            if (this.modState.BuildingMode)
            {
                if (config.ShowBuildQueue)
                {
                    var itemAmounts = new Dictionary<Item, int>();

                    foreach (var item in this.modState.TilesSelected.Values.GroupBy(x => x))
                        itemAmounts.Add(item.Key.Item, item.Count());

                    float screenWidth, screenHeight;

                    screenWidth = Game1.uiViewport.Width;
                    screenHeight = Game1.uiViewport.Height;
                    var startingPoint = new Vector2();

                    #region Shameless decompile copy

                    var playerGlobalPosition = Game1.player.GetBoundingBox().Center;
                    var playerLocalVector =
                        Game1.GlobalToLocal(globalPosition: new Vector2(playerGlobalPosition.X, playerGlobalPosition.Y),
                            viewport: Game1.viewport);
                    bool toolbarAtTop = playerLocalVector.Y > Game1.viewport.Height / 2 + 64 ? true : false;

                    #endregion


                    if (toolbarAtTop)
                        startingPoint = new Vector2(screenWidth / 2 - 398, 130);
                    else
                        startingPoint = new Vector2(screenWidth / 2 - 398, screenHeight - 230);

                    foreach (var item in itemAmounts)
                    {
                        e.SpriteBatch.Draw(
                            this.itemBox,
                            startingPoint,
                            new Rectangle(0, 128, 24, 24),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            Game1.pixelZoom,
                            SpriteEffects.None,
                            1f
                        );

                        item.Key.drawInMenu(
                            e.SpriteBatch,
                            startingPoint + new Vector2(17, 16),
                            0.75f, 1f, 4f, StackDrawType.Hide);

                        this.drawingUtils.DrawStringWithShadow(
                            e.SpriteBatch,
                            Game1.smallFont,
                            item.Value.ToString(),
                            startingPoint + new Vector2(10, 14) * Game1.pixelZoom,
                            Color.White,
                            Color.Black
                        );

                        startingPoint += new Vector2(24 * Game1.pixelZoom + 4, 0);
                    }
                }

                // Now, we render our rectangle quantity amount.
                if (this.modState.RectTiles != null)
                    foreach (var tile in this.modState.RectTiles)
                    {
                    }
            }
        }

        #endregion
    }
}
