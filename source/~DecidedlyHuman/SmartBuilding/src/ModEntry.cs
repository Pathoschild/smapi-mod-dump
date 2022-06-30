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
using System.IO;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartBuilding.Helpers;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

/*
TODO: Implement correct spacing restrictions for fruit trees, etc. Should be relatively simple with a change to our adjacent tile detection method.
TODO: Split things into separate classes where it would make things neater.
*/

namespace SmartBuilding
{
    public class ModEntry : Mod, IAssetLoader
    {
        // SMAPI gubbins.
        private static IModHelper helper = null!;
        private static IMonitor monitor = null!;
        private static Logger logger = null!;
        private static ModConfig config = null!;

        private Dictionary<Vector2, ItemInfo> tilesSelected = new Dictionary<Vector2, ItemInfo>();
        private Vector2 currentTile = Vector2.Zero;
        private Vector2 hudPosition;
        private Texture2D buildingHud = null!;
        private Texture2D itemBox = null!;
        private int itemBarWidth = 800; // This is the default.
        
        // Rectangle drawing.
        private Vector2? startTile = null;
        private Vector2? endTile = null;
        private List<Vector2> rectTiles = new List<Vector2>();
        
        private Item rectangleItem;

        private bool currentlyDrawing = false;
        private bool currentlyErasing = false;
        private bool currentlyPlacing = false;
        private bool buildingMode = false;

        // Debug stuff to make my life less painful when going through my pre-release checklist.
        private ConsoleCommand command = null!;

        // Integration for atravita's More Fertilizers mod.
        private IMoreFertilizersAPI? moreFertilizersAPI;

        private bool BuildingMode
        {
            get { return buildingMode; }
            set
            {
                buildingMode = value;
                HarmonyPatches.Patches.CurrentlyInBuildMode = value;

                if (!buildingMode) // If this is now false, we want to clear the tiles list, and refund everything.
                {
                    ClearPaintedTiles();
                }
            }
        }

        private bool CurrentlyDrawing
        {
            get { return currentlyDrawing; }
            set
            {
                currentlyDrawing = value;
            }
        }

        private bool CurrentlyErasing
        {
            get { return currentlyErasing; }
            set
            {
                currentlyErasing = value;
            }
        }

        private bool CurrentlyPlacing
        {
            get { return currentlyPlacing; }
            set
            {
                currentlyPlacing = value;
            }
        }

        #region Asset Loading Gubbins

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Mods/DecidedlyHuman/BuildingHUD");
        }

        public T Load<T>(IAssetInfo asset)
        { // We can just return this, because this mod can load only a single asset.
            return this.Helper.Content.Load<T>(Path.Combine("assets", "HUD.png"));
        }

        #endregion

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            ModEntry.helper = helper;
            monitor = Monitor;
            logger = new Logger(monitor);
            config = ModEntry.helper.ReadConfig<ModConfig>();
            hudPosition = new Vector2(50, 0);
            buildingHud = ModEntry.helper.Content.Load<Texture2D>("Mods/DecidedlyHuman/BuildingHUD", ContentSource.GameContent);
            itemBox = ModEntry.helper.Content.Load<Texture2D>("LooseSprites/tailoring", ContentSource.GameContent);
            command = new ConsoleCommand(logger, buildingHud, this);

            Harmony harmony = new Harmony(ModManifest.UniqueID);

            // I'll need more patches to ensure you can't interact with chests, etc., while building. Should be simple. 
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.PlacementAction_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.Chest_CheckForAction_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), nameof(FishPond.doAction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.FishPond_DoAction_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(StorageFurniture), nameof(StorageFurniture.checkForAction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.StorageFurniture_DoAction_Prefix)));

            // This is where we'll register with GMCM.
            ModEntry.helper.Events.GameLoop.GameLaunched += GameLaunched;

            // This is fired whenever input is changed, so we check for input here.
            ModEntry.helper.Events.Input.ButtonsChanged += OnInput;

            // This is used to have the queued builds draw themselves in the world.
            ModEntry.helper.Events.Display.RenderedWorld += RenderedWorld;

            // This is a huge mess, and is used to draw the building mode HUD, and build queue if enabled.
            ModEntry.helper.Events.Display.RenderedHud += RenderedHud;

            // If the screen is changed, clear our painted tiles, because currently, placing objects is done on the current screen.
            ModEntry.helper.Events.Player.Warped += (sender, args) =>
            {
                ClearPaintedTiles();
                buildingMode = false;
                currentlyDrawing = false;
                HarmonyPatches.Patches.CurrentlyInBuildMode = false;
                HarmonyPatches.Patches.AllowPlacement = false;
            };

            ModEntry.helper.ConsoleCommands.Add("sb_test", I18n.SmartBuilding_Commands_Debug_SbTest(), command.TestCommand);
            ModEntry.helper.ConsoleCommands.Add("sb_identify_all_items", I18n.SmartBuilding_Commands_Debug_SbIdentifyItems(), command.IdentifyItemsCommand);
#if !DEBUG
            ModEntry.helper.ConsoleCommands.Add("sb_binding_ui", "This will open up Smart Building's binding UI.", command.BindingUI);
#endif
        }

        /// <summary>
        /// SMAPI's <see cref="IInputEvents.ButtonsChanged"> event.
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
                    Farmer player = Game1.player;

                    if (player.CurrentItem != null)
                    {
                        if (player.CurrentItem is not Tool)
                        {
                            ItemType type = IdentifyItemType((SObject)player.CurrentItem);
                            Item item = player.CurrentItem;

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
                }

                if (config.IdentifyProducer.JustPressed())
                {
                    // We're trying to identify the type of producer under the cursor.
                    GameLocation here = Game1.currentLocation;
                    Vector2 targetTile = Game1.currentCursorTile;

                    if (here.objects.ContainsKey(targetTile))
                    {
                        SObject producer = here.objects[targetTile];
                        ProducerType type = IdentifyProducer(producer);

                        logger.Log($"Identified producer {producer.Name} as {type}.");
                        logger.Log($"{I18n.SmartBuilding_Message_ProducerBeingIdentified()} {producer.Name}");
                        logger.Log($"{I18n.SmartBuilding_Message_IdentifiedProducerType()}: {type}");
                    }
                    
                    foreach (var thing in here.resourceClumps)
                    {
                        if (thing.tile.Equals(targetTile))
                        {
                                
                        }
                    }
                }
            }

            // If the player presses to engage build mode, we flip the bool.
            if (config.EngageBuildMode.JustPressed())
            {
                // The BuildingMode property takes care of clearing the build queue.
                BuildingMode = !BuildingMode;
            }

            // If the player is drawing placeables in the world. 
            if (config.HoldToDraw.IsDown())
            {
                if (buildingMode)
                {
                    // We set our CurrentlyDrawing property to true, which will update the value in our patch.
                    CurrentlyDrawing = true;
                    
                    AddTile(Game1.player.CurrentItem, Game1.currentCursorTile);
                }
            }
            else
            {
                // Otherwise, the key is up, meaning we want to indicate we're not currently drawing.
                CurrentlyDrawing = false;
            }
            
            if (config.HoldToDrawRectangle.IsDown())
            {
                // If we're holding our rectangle modifier, we do things a little differently.

                if (buildingMode)
                {
                    CurrentlyDrawing = true;
                    rectangleItem = Game1.player.CurrentItem;

                    if (startTile == null)
                    {
                        // If the start tile hasn't yet been set, then we want to set that.
                        startTile = Game1.currentCursorTile;
                    }

                    endTile = Game1.currentCursorTile;

                    rectTiles = CalculateRectangle(startTile.Value, endTile.Value, rectangleItem);
                }
            }
            else
            {
                // The rectangle drawing key was released, so we want to calculate the tiles within, and set CurrentlyDrawing to false.

                if (startTile.HasValue && endTile.HasValue)
                {
                    List<Vector2> tiles = CalculateRectangle(startTile.Value, endTile.Value, rectangleItem);

                    foreach (Vector2 tile in tiles)
                    {
                        AddTile(rectangleItem, tile);
                    }

                    startTile = null;
                    endTile = null;
                    rectTiles.Clear();
                }

                CurrentlyDrawing = false;
            }

            if (config.HoldToErase.IsDown())
            {
                if (buildingMode)
                {
                    // We update this to set both our mod state, and patch bool.
                    CurrentlyErasing = true;

                    EraseTile(Game1.currentCursorTile);
                }
            }
            else
            {
                // The key is no longer held, so we set this to false.
                CurrentlyErasing = false;
            }

            if (config.HoldToInsert.IsDown())
            {
                if (buildingMode)
                {
                    // We're in building mode, but we also want to ensure the setting to enable this is on.
                    if (config.EnableInsertingItemsIntoMachines)
                    {
                        // If it is, we proceed to flag that we're placing items.
                        CurrentlyPlacing = true;

                        AddItem(Game1.player.CurrentItem, Game1.currentCursorTile);
                    }
                }
            }
            else
            {
                CurrentlyPlacing = false;
            }

            if (config.ConfirmBuild.JustPressed())
            {
                // The build has been confirmed, so we iterate through our Dictionary, and pass each tile into PlaceObject.
                foreach (KeyValuePair<Vector2, ItemInfo> v in tilesSelected)
                {
                    // We want to allow placement for the duration of this method.
                    HarmonyPatches.Patches.AllowPlacement = true;
                    
                    PlaceObject(v);
                    
                    // And disallow it afterwards.
                    HarmonyPatches.Patches.AllowPlacement = false;
                }

                // Then, we clear the list, because building is done, and all errors are handled internally.
                tilesSelected.Clear();
            }

            if (config.PickUpObject.IsDown())
            {
                if (buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with an SObject.
                    DemolishOnTile(Game1.currentCursorTile, TileFeature.Object);
            }

            if (config.PickUpFloor.IsDown())
            {
                if (buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with TerrainFeature.
                    DemolishOnTile(Game1.currentCursorTile, TileFeature.TerrainFeature);
            }

            if (config.PickUpFurniture.IsDown())
            {
                if (buildingMode) // If we're in building mode, we demolish the tile, indicating we're dealing with Furniture.
                    DemolishOnTile(Game1.currentCursorTile, TileFeature.Furniture);
            }
        }

        private List<Vector2> CalculateRectangle(Vector2 cornerOne, Vector2 cornerTwo, Item item)
        {
            Vector2 topLeft;
            Vector2 bottomRight;
            List<Vector2> tiles = new List<Vector2>();
            int itemsRemainingInStack = 0;

            if (item != null)
                itemsRemainingInStack = item.Stack;
            else
                itemsRemainingInStack = 0;
            
            topLeft = new Vector2(MathF.Min(cornerOne.X, cornerTwo.X), MathF.Min(cornerOne.Y, cornerTwo.Y));
            bottomRight = new Vector2(MathF.Max(cornerOne.X, cornerTwo.X), MathF.Max(cornerOne.Y, cornerTwo.Y));
            
            int rectWidth = (int)bottomRight.X - (int)topLeft.X + 1;
            int rectHeight = (int)bottomRight.Y - (int)topLeft.Y + 1;

            for (int x = (int)topLeft.X; x < rectWidth + topLeft.X; x++)
            {
                for (int y = (int)topLeft.Y; y < rectHeight + topLeft.Y; y++)
                {
                    if (itemsRemainingInStack > 0)
                    {
                        if (CanBePlacedHere(new Vector2(x, y), item))
                        {
                            tiles.Add(new Vector2(x, y));
                            itemsRemainingInStack--;
                        }
                    }
                }
            }

            return tiles;
        }

        private void GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            RegisterWithGmcm();

            this.moreFertilizersAPI = this.Helper.ModRegistry.GetApi<IMoreFertilizersAPI>("atravita.MoreFertilizers");
        }

        private void RegisterWithGmcm()
        {
            IGenericModConfigMenuApi configMenuApi =
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenuApi == null)
            {
                logger.Log(I18n.SmartBuilding_Warning_GmcmNotInstalled(), LogLevel.Info);

                return;
            }

            configMenuApi.Register(ModManifest,
                () => config = new ModConfig(),
                () => Helper.WriteConfig(config));

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_Keybinds_Title()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_Keybinds_Paragraph_GmcmWarning()
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_EnterBuildMode(),
                getValue: () => config.EngageBuildMode,
                setValue: value => config.EngageBuildMode = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToDraw(),
                getValue: () => config.HoldToDraw,
                setValue: value => config.HoldToDraw = value);
            
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToDrawRectangle(),
                getValue: () => config.HoldToDrawRectangle,
                setValue: value => config.HoldToDrawRectangle = value
                );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToErase(),
                getValue: () => config.HoldToErase,
                setValue: value => config.HoldToErase = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToInsert(),
                getValue: () => config.HoldToInsert,
                setValue: value => config.HoldToInsert = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_ConfirmBuild(),
                getValue: () => config.ConfirmBuild,
                setValue: value => config.ConfirmBuild = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_PickUpObject(),
                getValue: () => config.PickUpObject,
                setValue: value => config.PickUpObject = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_PickUpFloor(),
                getValue: () => config.PickUpFloor,
                setValue: value => config.PickUpFloor = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_PickUpFurniture(),
                getValue: () => config.PickUpFurniture,
                setValue: value => config.PickUpFurniture = value);

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_OptionalToggles_Title()
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_ShowBuildQueue(),
                getValue: () => config.ShowBuildQueue,
                setValue: value => config.ShowBuildQueue = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_CanDestroyChests(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_CanDestroyChests_Tooltip(),
                getValue: () => config.CanDestroyChests,
                setValue: value => config.CanDestroyChests = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalTogglesMoreLaxObjectPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalTogglesMoreLaxObjectPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveObjectPlacement,
                setValue: value => config.LessRestrictiveObjectPlacement = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFloorPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFloorPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveFloorPlacement,
                setValue: value => config.LessRestrictiveFloorPlacement = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFurniturePlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxFurniturePlacement_Tooltip(),
                getValue: () => config.LessRestrictiveFurniturePlacement,
                setValue: value => config.LessRestrictiveFurniturePlacement = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxBedPlacement(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_MoreLaxBedPlacement_Tooltip(),
                getValue: () => config.LessRestrictiveBedPlacement,
                setValue: value => config.LessRestrictiveBedPlacement = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFloors(),
                getValue: () => config.EnableReplacingFloors,
                setValue: value => config.EnableReplacingFloors = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFences(),
                tooltip: () => I18n.SmartBuilding_Settings_OptionalToggles_EnableReplacingFences_Tooltip(),
                getValue: () => config.EnableReplacingFences,
                setValue: value => config.EnableReplacingFences = value
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_CheatyOptions_Title()
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_CrabPotsInAnyWaterTile(),
                getValue: () => config.CrabPotsInAnyWaterTile,
                setValue: value => config.CrabPotsInAnyWaterTile = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnablePlantingCrops(),
                getValue: () => config.EnablePlantingCrops,
                setValue: value => config.EnablePlantingCrops = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableCropFertilisers(),
                getValue: () => config.EnableCropFertilizers,
                setValue: value => config.EnableCropFertilizers = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableTreeFertilisers(),
                getValue: () => config.EnableTreeFertilizers,
                setValue: value => config.EnableTreeFertilizers = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableTreeTappers(),
                getValue: () => config.EnableTreeTappers,
                setValue: value => config.EnableTreeTappers = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableInsertingItemsIntoMachines(),
                getValue: () => config.EnableInsertingItemsIntoMachines,
                setValue: value => config.EnableInsertingItemsIntoMachines = value
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_Debug_Title()
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_EnableDebugCommand(),
                getValue: () => config.EnableDebugCommand,
                setValue: value => config.EnableDebugCommand = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_EnableDebugKeybinds(),
                getValue: () => config.EnableDebugControls,
                setValue: value => config.EnableDebugControls = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_IdentifyProducerToConsole(),
                getValue: () => config.IdentifyProducer,
                setValue: value => config.IdentifyProducer = value);

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Debug_IdentifyHeldItemToConsole(),
                getValue: () => config.IdentifyItem,
                setValue: value => config.IdentifyItem = value);

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_Title()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_Paragraph()
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture(),
                tooltip: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture_Tooltip(),
                getValue: () => config.EnablePlacingStorageFurniture,
                setValue: value => config.EnablePlacingStorageFurniture = value
            );

            configMenuApi.AddPageLink(
                mod: ModManifest,
                pageId: "JsonGuide",
                text: () => I18n.SmartBuilding_Settings_JsonGuide_PageLink()
            );

            configMenuApi.AddPage(
                mod: ModManifest,
                pageId: "JsonGuide",
                pageTitle: () => I18n.SmartBuilding_Settings_JsonGuide_PageTitle()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_JsonGuide_Guide1()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_JsonGuide_Guide2()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_JsonGuide_Guide3()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_JsonGuide_Guide4()
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_JsonGuide_Guide5()
            );
        }

        // TODO: Actually comment things in this method.
        private void RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (buildingMode)
            { 
                // There's absolutely no need to run this while we're not in building mode.
                int windowWidth = Game1.game1.Window.ClientBounds.Width;

                // TODO: Use the newer logic I have to get the toolbar position for this.
                hudPosition = new Vector2(
                    windowWidth / 2 - itemBarWidth / 2 - buildingHud.Width * 4,
                    0);

                e.SpriteBatch.Draw(
                    texture: buildingHud,
                    position: hudPosition,
                    sourceRectangle: buildingHud.Bounds,
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: Game1.pixelZoom,
                    effects: SpriteEffects.None,
                    layerDepth: 1f
                );

                if (config.ShowBuildQueue)
                {
                    Dictionary<Item, int> itemAmounts = new Dictionary<Item, int>();

                    foreach (var item in tilesSelected.Values.GroupBy(x => x))
                    {
                        itemAmounts.Add(item.Key.Item, item.Count());
                    }

                    float screenWidth, screenHeight;

                    screenWidth = Game1.uiViewport.Width;
                    screenHeight = Game1.uiViewport.Height;
                    Vector2 startingPoint = new Vector2();

                    #region Shameless decompile copy

                    Point playerGlobalPosition = Game1.player.GetBoundingBox().Center;
                    Vector2 playerLocalVector = Game1.GlobalToLocal(globalPosition: new Vector2(playerGlobalPosition.X, playerGlobalPosition.Y), viewport: Game1.viewport);
                    bool toolbarAtTop = playerLocalVector.Y > (float)(Game1.viewport.Height / 2 + 64) ? true : false;

                    #endregion


                    if (toolbarAtTop)
                    {
                        startingPoint = new Vector2(screenWidth / 2 - 398, 130);
                    }
                    else
                        startingPoint = new Vector2(screenWidth / 2 - 398, screenHeight - 230);

                    foreach (var item in itemAmounts)
                    {
                        e.SpriteBatch.Draw(
                            texture: itemBox,
                            position: startingPoint,
                            sourceRectangle: new Rectangle(0, 128, 24, 24),
                            color: Color.White,
                            rotation: 0f,
                            origin: Vector2.Zero,
                            scale: Game1.pixelZoom,
                            effects: SpriteEffects.None,
                            layerDepth: 1f
                        );

                        item.Key.drawInMenu(
                            e.SpriteBatch,
                            startingPoint + new Vector2(17, 16),
                            0.75f, 1f, 4f, StackDrawType.Hide);

                        DrawStringWithShadow(
                            spriteBatch: e.SpriteBatch,
                            font: Game1.smallFont,
                            text: item.Value.ToString(),
                            position: startingPoint + new Vector2(10, 14) * Game1.pixelZoom,
                            textColour: Color.White,
                            shadowColour: Color.Black
                        );

                        startingPoint += new Vector2(24 * Game1.pixelZoom + 4, 0);
                    }
                }
            }
        }

        private void DrawStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color textColour, Color shadowColour)
        {
            spriteBatch.DrawString(
                spriteFont: font,
                text: text,
                position: position + new Vector2(2, 2),
                shadowColour
            );

            spriteBatch.DrawString(
                spriteFont: font,
                text: text,
                position: position,
                textColour
            );
        }

        private void EraseTile(Vector2 tile)
        {
            Vector2 flaggedForRemoval = new Vector2();

            foreach (var item in tilesSelected)
            {
                if (item.Key == tile)
                {
                    // If we're over a tile in _tilesSelected, remove it and refund the item to the player.
                    Game1.player.addItemToInventoryBool(item.Value.Item.getOne(), false);
                    monitor.Log($"{item.Value.Item.Name} {I18n.SmartBuilding_Info_RefundedIntoPlayerInventory()}");

                    // And flag it for removal from the queue, since we can't remove from within the foreach.
                    flaggedForRemoval = tile;
                }
            }

            tilesSelected.Remove(flaggedForRemoval);
        }

        private bool IsTypeOfObject(SObject o, ItemType type)
        {
            // We try to identify what kind of object we've been passed.
            ItemType oType = IdentifyItemType(o);

            return oType == type;
        }

        private ProducerType IdentifyProducer(SObject o)
        {
            ProducerType type = ProducerType.NotAProducer;

            if (o.Category == -9 && o.Type.Equals("Crafting"))
            {
                // We know this matches the two things all producers (both vanilla and PFM) have in common, so now we can move on to figuring out exactly what type of producer we're looking at.
                string producerName = o.Name;

                // Now, the most efficient thing to do will be to attempt to find only the vanilla machines which do not deduct automatically, as everything else, vanilla and PFM, deducts automatically.
                switch (producerName)
                {
                    case "Mayonnaise Machine":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Preserves Jar":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Cheese Press":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Loom":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Keg":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Cask":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Oil Maker":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Crystalarium":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Recycling Machine":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Seed Maker":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Slime Incubator":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Ostrich Incubator":
                        type = ProducerType.ManualRemoval;
                        break;
                    case "Deconstructor":
                        type = ProducerType.ManualRemoval;
                        break;
                    default:
                        // At this point, we've filtered out all vanilla producers which require manual removal, so we're left with only producers, vanilla and modded, that deduct automatically.
                        type = ProducerType.AutomaticRemoval;
                        break;
                }

                return type;
            }

            return type;
        }

        private bool CanBeInsertedHere(Vector2 targetTile, Item item)
        {
            // First, we need to ensure there's an SObject here.
            if (Game1.currentLocation.objects.ContainsKey(targetTile))
            {
                // There is one, so we grab a reference to it.
                SObject o = Game1.currentLocation.objects[targetTile];

                // We also need to know what type of producer we're looking at, if any.
                ProducerType type = IdentifyProducer(o);

                // Whether or not we need to manually deduct the item.
                bool needToDeduct = false;

                // If this isn't a producer, we return immediately.
                if (type == ProducerType.NotAProducer)
                {
                    return false;
                }
                else if (type == ProducerType.ManualRemoval)
                {
                    // If this requires manual removal, we mark that we do need to manually deduct the item.
                    needToDeduct = true;
                }
                else if (type == ProducerType.AutomaticRemoval)
                {
                    // The producer in question removes automatically, so we don't need to manually deduct the item.
                    needToDeduct = false;
                }

                InsertItem(item, o, needToDeduct);
            }

            // There was no object here, so we return false.
            return false;
        }

        private void InsertItem(Item item, SObject o, bool shouldManuallyDeduct)
        {
            // For some reason, apparently, we always need to deduct the held item by one, even if we're working with a producer which does it by itself.
            // TODO: Investigate this, but for now, this seems to work.

            //if (shouldManuallyDeduct)
            //{
            //    // This is marked as needing to be deducted manually, so we do just that.
            //    Game1.player.reduceActiveItemByOne();
            //}

            // First, we perform the drop in action.
            bool successfullyInserted = o.performObjectDropInAction(item, false, Game1.player);

            // Then, perplexingly, we still need to manually deduct the item by one, or we can end up with an item that has a stack size of zero.
            if (successfullyInserted)
            {
                Game1.player.reduceActiveItemByOne();
            }
        }

        /// <summary>
        /// Will return whether or not a tile can be placed 
        /// </summary>
        /// <param name="v">The world-space Tile in which the check is to be performed.</param>
        /// <param name="i">The placeable type.</param>
        /// <returns></returns>
        private bool CanBePlacedHere(Vector2 v, Item i)
        {
            // If the item is a tool, we want to return.
            if (i is Tool)
                return false;
            
            ItemType itemType = IdentifyItemType((SObject)i);
            GameLocation here = Game1.currentLocation;

            switch (itemType)
            {
                case ItemType.NotPlaceable:
                    return false;
                case ItemType.Torch:
                    // We need to figure out whether there's a fence in the placement tile.
                    if (here.objects.ContainsKey(v))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        SObject o = here.objects[v];

                        // Then we return true if it's a fence, because we want to place the torch on the fence.
                        if (IsTypeOfObject(o, ItemType.Fence))
                        {
                            // It's a type of fence, but we also want to ensure that it isn't a gate.

                            if (o.Name.Equals("Gate"))
                                return false;
                            
                            return true;
                        }
                    }
                    else
                        goto GenericPlaceable; // Please don't hate me too much. This is temporary until everything gets split out into separate methods eventually.

                    break;
                case ItemType.CrabPot: // We need to determine if the crab pot is being placed in an appropriate water tile.
                    return CrabPot.IsValidCrabPotLocationTile(here, (int)v.X, (int)v.Y) && HasAdjacentNonWaterTile(v);
                case ItemType.GrassStarter:
                    // If there's a terrain feature here, we can't possibly place a grass starter.
                    return !here.terrainFeatures.ContainsKey(v);
                case ItemType.Floor:
                    // In this case, we need to know whether there's a TerrainFeature in the tile.
                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // At this point, we know there's a terrain feature here, so we grab a reference to it.
                        TerrainFeature tf = Game1.currentLocation.terrainFeatures[v];

                        // Then we check to see if it is, indeed, Flooring.
                        if (tf != null && tf is Flooring)
                        {
                            // If it is, and if the setting to replace floors with floors is enabled, we return true.
                            if (config.EnableReplacingFloors)
                                return true;
                        }

                        return false;
                    }
                    else if (here.objects.ContainsKey(v))
                    {
                        // We know an object exists here now, so we grab it.
                        SObject o = here.objects[v];
                        ItemType type;
                        Item itemToDestroy;

                        itemToDestroy = Utility.fuzzyItemSearch(o.Name);
                        type = IdentifyItemType((SObject)itemToDestroy);

                        if (type == ItemType.Fence)
                        {
                            // This is a fence, so we return true.

                            return true;
                        }
                    }

                    // At this point, we return appropriately with vanilla logic, or true depending on the placement setting.
                    return config.LessRestrictiveFloorPlacement || here.isTileLocationTotallyClearAndPlaceable(v);
                case ItemType.Chest:
                    goto case ItemType.Generic;
                case ItemType.Fertilizer:
                    // If the setting to enable fertilizers is off, return false to ensure they can't be added to the queue.
                    if (!config.EnableCropFertilizers)
                        return false;

                    // If this is a More Fertilizers fertilizer, defer to More Fertilizer's placement logic.
                    if (i is SObject obj && moreFertilizersAPI?.CanPlaceFertilizer(obj, here, v) == true)
                        return true;

                    // If there's an object present, we don't want to place any fertilizer.
                    // It is technically valid, but there's no reason someone would want to.
                    if (here.Objects.ContainsKey(v))
                        return false;

                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
                        if (here.terrainFeatures[v] is HoeDirt)
                        {
                            // If it is, we want to grab the HoeDirt, and check for the possibility of planting.
                            HoeDirt hd = (HoeDirt)here.terrainFeatures[v];

                            if (hd.crop != null)
                            {
                                // If the HoeDirt has a crop, we want to grab it and check for growth phase and fertilization status.
                                Crop cropToCheck = hd.crop;

                                if (cropToCheck.currentPhase.Value != 0)
                                {
                                    // If the crop's current phase is not zero, we return false.

                                    return false;
                                }
                            }

                            // At this point, we fall to vanilla logic to determine placement validity.
                            return hd.canPlantThisSeedHere(i.ParentSheetIndex, (int)v.X, (int)v.Y, true);
                        }
                    }

                    return false;
                case ItemType.TreeFertilizer:
                    // If the setting to enable tree fertilizers is off, return false to ensure they can't be added to the queue.
                    if (!config.EnableTreeFertilizers)
                        return false;

                    // First, we determine if there's a TerrainFeature here.
                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // Then we check if it's a tree.
                        if (here.terrainFeatures[v] is Tree)
                        {
                            // It is a tree, so now we check to see if the tree is fertilised.
                            Tree tree = (Tree)here.terrainFeatures[v];

                            // If it's already fertilised, there's no need for us to want to place tree fertiliser on it, so we return false.
                            if (tree.fertilized.Value)
                                return false;
                            else
                                return true;
                        }
                    }

                    return false;
                case ItemType.Seed:
                    // If the setting to enable crops is off, return false to ensure they can't be added to the queue.
                    if (!config.EnablePlantingCrops)
                        return false;

                    // If there's an object present, we don't want to place a seed.
                    // It is technically valid, but there's no reason someone would want to.
                    if (here.Objects.ContainsKey(v))
                        return false;

                    // First, we check for a TerrainFeature.
                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // Then, we check to see if it's HoeDirt.
                        if (here.terrainFeatures[v] is HoeDirt)
                        {
                            // If it is, we grab a reference to the HoeDirt to use its canPlantThisSeedHere method.
                            HoeDirt hd = (HoeDirt)here.terrainFeatures[v];

                            return hd.canPlantThisSeedHere(i.ParentSheetIndex, (int)v.X, (int)v.Y);
                        }
                    }

                    return false;
                case ItemType.Tapper:
                    // If the setting to enable tree tappers is off, we return false here to ensure nothing further happens.
                    if (!config.EnableTreeTappers)
                        return false;

                    // First, we need to check if there's a TerrainFeature here.
                    if (here.terrainFeatures.ContainsKey(v))
                    {
                        // If there is, we check to see if it's a tree.
                        if (here.terrainFeatures[v] is Tree)
                        {
                            // If it is, we grab a reference to the tree to check its details.
                            Tree tree = (Tree)here.terrainFeatures[v];

                            // If the tree isn't tapped, we confirm that a tapper can be placed here.
                            if (!tree.tapped)
                            {
                                // If the tree is fully grown, we *can* place a tapper.
                                return tree.growthStage >= 5;
                            }
                        }
                    }

                    return false;
                case ItemType.Fence:
                    // We want to deal with fences specifically in order to handle fence replacements.
                    if (here.objects.ContainsKey(v))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        SObject o = here.objects[v];

                        // Then we return true if this is both a fence, and replacing fences is enabled.
                        return IsTypeOfObject(o, ItemType.Fence) && config.EnableReplacingFences;
                    }
                    else if (here.terrainFeatures.ContainsKey(v))
                    {
                        // There's a terrain feature here, so we want to check if it's a HoeDirt with a crop.
                        TerrainFeature feature = here.terrainFeatures[v];

                        if (feature != null && feature is HoeDirt)
                        {
                            if ((feature as HoeDirt).crop != null)
                            {
                                // There's a crop here, so we return false.
                                return false;
                            }

                            // At this point, we know it's a HoeDirt, but has no crop, so we can return true.
                            return true;
                        }
                    }

                    goto case ItemType.Generic;
                case ItemType.FishTankFurniture:
                    // TODO: Until I figure out how to successfully transplant fish, I'm hard blocking these.
                    return false;
                case ItemType.StorageFurniture:
                    // Since FishTankFurniture will sneak through here:
                    if (i is FishTankFurniture)
                        return false;

                    // If the setting for allowing storage furniture is off, we get the hell out.
                    if (!config.EnablePlacingStorageFurniture)
                        return false;

                    if (config.LessRestrictiveFurniturePlacement)
                        return true;
                    else
                        return (i as StorageFurniture).canBePlacedHere(here, v);
                case ItemType.TvFurniture:
                    if (config.LessRestrictiveFurniturePlacement)
                        return true;
                    else
                        return (i as TV).canBePlacedHere(here, v);
                case ItemType.BedFurniture:
                    if (config.LessRestrictiveBedPlacement)
                        return true;
                    else
                        return (i as BedFurniture).canBePlacedHere(here, v);
                case ItemType.GenericFurniture:
                    // In this place, we play fast and loose, and return true.
                    if (config.LessRestrictiveFurniturePlacement)
                        return true;
                    else
                        return (i as Furniture).canBePlacedHere(here, v);
                case ItemType.Generic:
                    GenericPlaceable: // A goto, I know, gross, but... it works, and is fine for now, until I split out detection logic into methods. TODO.
                    if (config.LessRestrictiveObjectPlacement)
                    {
                        // If the less restrictive object placement setting is enabled, we first want to check if vanilla logic dictates the object be placeable.
                        if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
                        {
                            // It dictates that it is, so we can simply return true.
                            return true;
                        } else
                        {
                            // Otherwise, we want to check for an object already present in this location.
                            if (!here.Objects.ContainsKey(v))
                            {
                                // There is no object here, so we return true, as we should be able to place the object here.
                                return true;
                            }

                            // We could just fall through to vanilla logic again at this point, but that would be vaguely pointless, so we just return false.
                            return false;
                        }
                    }
                    return Game1.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v);
            }

            // If the PlaceableType is somehow none of these, we want to be safe and return false.
            return false;
        }

        public ItemType IdentifyItemType(SObject item) // Making this public for access from the command is awful. TODO: Split this off into its own class.
        {
            // TODO: Make this detection more robust. If possible, don't depend upon it at all.
            string itemName = item.Name;

            // The whole point of this is to determine whether the object being placed requires special treatment.
            if (item is Tool)
                return ItemType.NotPlaceable;
            else if (item.Name.Equals("Torch") && item.Category.Equals(0) && item.Type.Equals("Crafting"))
                return ItemType.Torch;
            else if (!item.isPlaceable())
                return ItemType.NotPlaceable;
            else if (item is FishTankFurniture)
                return ItemType.FishTankFurniture;
            else if (item is StorageFurniture)
                return ItemType.StorageFurniture;
            else if (item is BedFurniture)
                return ItemType.BedFurniture;
            else if (item is TV)
                return ItemType.TvFurniture;
            else if (item is Furniture)
                return ItemType.GenericFurniture;
            else if (itemName.Contains("Floor") || itemName.Contains("Path") && item.Category == -24)
                return ItemType.Floor;
            else if (itemName.Contains("Chest") || item is Chest)
                return ItemType.Chest;
            else if (itemName.Contains("Fence"))
                return ItemType.Fence;
            else if (itemName.Equals("Gate") || item.ParentSheetIndex.Equals(325))
                return ItemType.Fence;
            else if (itemName.Equals("Grass Starter"))
                return ItemType.GrassStarter;
            else if (itemName.Equals("Crab Pot"))
                return ItemType.CrabPot;
            else if (item.Type == "Seeds" || item.Category == -74)
            {
                if (!item.Name.Contains("Sapling") && !item.Name.Equals("Acorn") && !item.Name.Equals("Maple Seed") && !item.Name.Equals("Pine Cone") && !item.Name.Equals("Mahogany Seed"))
                    return ItemType.Seed;
            }
            else if (item.Name.Equals("Tree Fertilizer"))
                return ItemType.TreeFertilizer;
            else if (item.Category == -19)
                return ItemType.Fertilizer;
            else if (item.Name.Equals("Tapper") || item.Name.Equals("Heavy Tapper"))
                return ItemType.Tapper;

            return ItemType.Generic;
        }

        private ItemInfo GetItemInfo(SObject item)
        {
            ItemType itemType = IdentifyItemType(item);

            return new ItemInfo()
            {
                Item = item,
                ItemType = itemType
            };
        }

        private void AddItem(Item item, Vector2 v)
        {
            // If we're not in building mode, we do nothing.
            if (!buildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;

            // There is no queue for item insertion, so we simply try to insert.
            CanBeInsertedHere(v, item);
        }

        private void AddTile(Item item, Vector2 v)
        {
            // If we're not in building mode, we do nothing.
            if (!buildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;

            // If the item cannot be placed here according to our own rules, we do nothing. This is to allow for slightly custom placement logic.
            if (!CanBePlacedHere(v, item))
                return;

            ItemInfo itemInfo = GetItemInfo((SObject)item);

            // We only want to add the tile if the Dictionary doesn't already contain it. 
            if (!tilesSelected.ContainsKey(v))
            {
                // We then want to check if the item can even be placed in this spot.
                if (CanBePlacedHere(v, item))
                {
                    tilesSelected.Add(v, itemInfo);
                    Game1.player.reduceActiveItemByOne();
                }
            }
        }

        private bool HasAdjacentNonWaterTile(Vector2 v)
        {
            // Although crab pots are the only currently tested object that
            // go in water, I do want to modularise this later.
            // TODO: Modularise for not only crab pots.

            if (config.CrabPotsInAnyWaterTile)
                return true;

            List<Vector2> directions = new List<Vector2>()
            {
                v + new Vector2(-1, 0), // Left
                v + new Vector2(1, 0), // Right
                v + new Vector2(0, -1), // Up
                v + new Vector2(0, 1), // Down
                v + new Vector2(-1, -1), // Up left
                v + new Vector2(1, -1), // Up right
                v + new Vector2(-1, 1), // Down left
                v + new Vector2(1, 1) // Down right
            };

            foreach (Vector2 vector in directions)
            {
                if (!Game1.currentLocation.isWaterTile((int)vector.X, (int)vector.Y))
                    return true;
            }

            return false;
        }

        private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            foreach (KeyValuePair<Vector2, ItemInfo> item in tilesSelected)
            {
                // Here, we simply have the Item draw itself in the world.
                item.Value.Item.drawInMenu
                (e.SpriteBatch,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        item.Key * Game1.tileSize),
                    1f, 1f, 4f, StackDrawType.Hide);
            }

            if (rectTiles != null)
            {
                foreach (Vector2 tile in rectTiles)
                {
                    // Here, we simply have the Item draw itself in the world.
                    rectangleItem.drawInMenu
                    (e.SpriteBatch,
                        Game1.GlobalToLocal(
                            Game1.viewport,
                            tile * Game1.tileSize),
                        1f, 1f, 4f, StackDrawType.Hide);
                }
            }
        }

        private void ClearPaintedTiles()
        {
            // To clear the painted tiles, we want to iterate through our Dictionary, and refund every item contained therein.
            foreach (var t in tilesSelected)
            {
                RefundItem(t.Value.Item, I18n.SmartBuilding_Info_BuildCancelled(), LogLevel.Trace, false);
            }

            // And, finally, clear it.
            tilesSelected.Clear();
            
            // We also want to clear the rect tiles. No refunding necessary here, however, as items are only deducted when added to tilesSelected.
            rectTiles.Clear();
        }

        // TODO: Modularise this method more. Right now, it just works. It is not well structured for future maintenance.
        private void DemolishOnTile(Vector2 tile, TileFeature feature)
        {
            GameLocation here = Game1.currentLocation;
            Vector2 playerTile = Game1.player.getTileLocation();
            Item itemToDestroy;
            ItemType type;

            // We're working with an SObject in this specific instance.
            if (feature == TileFeature.Object)
            {
                if (here.objects.ContainsKey(tile))
                {
                    // We have an object in this tile, so we want to try to figure out what it is.
                    SObject o = here.objects[tile];
                    itemToDestroy = Utility.fuzzyItemSearch(o.Name);

                    type = IdentifyItemType((SObject)itemToDestroy);

                    // Chests need special handling because they can store items.
                    if (type == ItemType.Chest)
                    {
                        // We're double checking at this point for safety. I want to be extra careful with chests.
                        if (here.objects.ContainsKey(tile))
                        {
                            // If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
                            if (config.CanDestroyChests)
                            {
                                // This is fairly fragile, but it's fine with vanilla chests, at least.
                                Chest chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

                                (o as Chest).destroyAndDropContents(tile * 64, here);
                                Game1.player.addItemByMenuIfNecessary(chest.getOne());
                                here.objects.Remove(tile);
                            }
                        }
                    }
                    else if (o is Chest)
                    {
                        // We're double checking at this point for safety. I want to be extra careful with chests.
                        if (here.objects.ContainsKey(tile))
                        {
                            // If the setting to disable chest pickup is enabled, we pick up the chest. If not, we do nothing.
                            if (config.CanDestroyChests)
                            {
                                // This is fairly fragile, but it's fine with vanilla chests, at least.
                                Chest chest = new Chest(o.ParentSheetIndex, tile, 0, 1);

                                (o as Chest).destroyAndDropContents(tile * 64, here);
                                Game1.player.addItemByMenuIfNecessary(chest.getOne());
                                here.objects.Remove(tile);
                            }
                        }
                    }
                    else if (type == ItemType.Fence)
                    {
                        // We need special handling for fences, since we don't want to pick them up if their health has deteriorated too much.
                        Fence fenceToRemove = (Fence)o;
                        
                        // We also need to check to see if the fence has a torch on it so we can remove the light source.
                        if (o.heldObject.Value != null)
                        {
                            // There's an item there, so we can relatively safely assume it's a torch.
                            
                            // We remove its light source from the location, and refund the torch.
                            here.removeLightSource(o.heldObject.Value.lightSource.identifier);
                            
                            RefundItem(o.heldObject, "No error. Do not log.", LogLevel.Trace, false);
                        }

                        fenceToRemove.performRemoveAction(tile * 64, here);
                        here.objects.Remove(tile);

                        // And, if the fence had enough health remaining, we refund it.
                        if (fenceToRemove.maxHealth.Value - fenceToRemove.health.Value < 0.5f)
                            Game1.player.addItemByMenuIfNecessary(fenceToRemove.getOne());
                    }
                    else if (type == ItemType.Tapper)
                    {
                        // Tappers need special handling to mark the tree as untapped, otherwise they can't be chopped down with an axe.
                        if (here.terrainFeatures.ContainsKey(tile))
                        {
                            // We've confirmed there's a TerrainFeature here, so next we grab a reference to it if it is a tree.
                            if (here.terrainFeatures[tile] is Tree treeToUntap)
                            { 
                                // After double checking there's a tree here, we grab a reference to it.
                                treeToUntap.tapped.Value = false;
                            }
                            
                            o.performRemoveAction(tile * 64, here);
                            Game1.player.addItemByMenuIfNecessary(o.getOne());

                            here.objects.Remove(tile);
                        }
                    }
                    else
                    {
                        if (o.Fragility == 2)
                        {
                            // A fragility of 2 means the item should not be broken, or able to be picked up, like incubators in coops, so we return.

                            return;
                        }

                        // Now we need to figure out whether the object has a heldItem within it.
                        if (o.heldObject != null)
                        {
                            // There's an item inside here, so we need to determine whether to refund the item, or discard it if it's a chest.
                            if (o.heldObject.Value is Chest)
                            {
                                // It's a chest, so we want to force it to drop all of its items.
                                if ((o.heldObject.Value as Chest).items.Count > 0)
                                {
                                    (o.heldObject.Value as Chest).destroyAndDropContents(tile * 64, here);
                                }
                            }
                        }

                        o.performRemoveAction(tile * 64, here);
                        Game1.player.addItemByMenuIfNecessary(o.getOne());

                        here.objects.Remove(tile);
                    }
                    // TODO: Temporary return!
                    return;
                }
            }

            // We're working with a TerrainFeature.
            if (feature == TileFeature.TerrainFeature)
            {
                if (here.terrainFeatures.ContainsKey(tile))
                {
                    TerrainFeature tf = here.terrainFeatures[tile];

                    // We only really want to be handling flooring when removing TerrainFeatures.
                    if (tf is Flooring)
                    {
                        Flooring floor = (Flooring)tf;

                        int? floorType = floor.whichFloor.Value;
                        string? floorName = GetFlooringNameFromId(floorType.Value);
                        SObject finalFloor;

                        if (floorType.HasValue)
                        {
                            floorName = GetFlooringNameFromId(floorType.Value);
                            finalFloor = (SObject)Utility.fuzzyItemSearch(floorName, 1);
                        }
                        else
                        {
                            finalFloor = null;
                        }

                        if (finalFloor != null)
                            Game1.player.addItemByMenuIfNecessary(finalFloor);
                        // Game1.createItemDebris(finalFloor, playerTile * 64, 1, here);

                        here.terrainFeatures.Remove(tile);
                    }
                }
            }

            if (feature == TileFeature.Furniture)
            {
                Furniture furnitureToGrab = null;

                foreach (Furniture f in here.furniture)
                {
                    if (f.boundingBox.Value.Intersects(new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 1, 1)))
                    {
                        furnitureToGrab = f;
                    }
                }

                if (furnitureToGrab != null)
                {
                    // If it's a StorageFurniture, and the setting to allow working with it is false, do nothing.
                    if (furnitureToGrab is StorageFurniture && !config.EnablePlacingStorageFurniture)
                        return;
                    
                    // Otherwise, we can continue.
                    logger.Log($"{I18n.SmartBuikding_Message_TryingToGrab()} {furnitureToGrab.Name}");
                    Game1.player.addItemToInventory(furnitureToGrab);
                    here.furniture.Remove(furnitureToGrab);
                }
            }
        }

        //private ProducerType IdentifyProducerType(SObject o)
        //{
        //  ProducerType producerType;


        //  return producerType;
        //}

        private void PlaceObject(KeyValuePair<Vector2, ItemInfo> item)
        {
            SObject itemToPlace = (SObject)item.Value.Item;
            Vector2 targetTile = item.Key;
            ItemInfo itemInfo = item.Value;
            GameLocation here = Game1.currentLocation;

            if (itemToPlace != null && CanBePlacedHere(targetTile, itemInfo.Item))
            { // The item can be placed here.
                if (itemInfo.ItemType == ItemType.Floor)
                {
                    // We're specifically dealing with a floor/path.

                    int? floorType = GetFlooringIdFromName(itemToPlace.Name);
                    Flooring floor;

                    if (floorType.HasValue)
                        floor = new Flooring(floorType.Value);
                    else
                    {
                        // At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_TerrainFeature_Flooring_CouldNotIdentifyFloorType(), LogLevel.Error, true);

                        return;
                    }

                    // At this point, we *need* there to be no TerrainFeature present.
                    if (!here.terrainFeatures.ContainsKey(targetTile))
                        here.terrainFeatures.Add(targetTile, floor);
                    else
                    {
                        // At this point, we know there's a terrain feature here.
                        if (config.EnableReplacingFloors)
                        {
                            TerrainFeature tf = here.terrainFeatures[targetTile];

                            if (tf != null && tf is Flooring)
                            {
                                // At this point, we know it's Flooring, so we remove the existing terrain feature, and add our new one.
                                DemolishOnTile(targetTile, TileFeature.TerrainFeature);
                                here.terrainFeatures.Add(targetTile, floor);
                            }
                            else
                            {
                                // At this point, there IS a terrain feature here, but it isn't flooring, so we want to refund the item, and return.
                                RefundItem(item.Value.Item, I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);

                                // We now want to jump straight out of this method, because this will flow through to the below if, and bad things will happen.
                                return;
                            }
                        }
                    }

                    // By this point, we'll have returned false if this could be anything but our freshly placed floor.
                    if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[item.Key] is Flooring))
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_TerrainFeature_Generic_UnknownError(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Chest)
                {
                    // We're dealing with a chest.
                    int? chestType = GetChestType(itemToPlace.Name);
                    Chest chest;

                    if (chestType.HasValue)
                    {
                        chest = new Chest(true, chestType.Value);
                    }
                    else
                    { // At this point, something is very wrong, so we want to refund the item to the player's inventory, and print an error.
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_Chest_CouldNotIdentifyChest(), LogLevel.Error, true);

                        return;
                    }

                    // We do our second placement possibility check, just in case something was placed in the meantime.
                    if (CanBePlacedHere(targetTile, itemToPlace))
                    {
                        bool placed = chest.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                        // Apparently, chests placed in the world are hardcoded with the name "Chest".
                        if (!here.objects.ContainsKey(targetTile) || !here.objects[targetTile].Name.Equals("Chest"))
                            RefundItem(itemToPlace, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                    }
                }
                else if (itemInfo.ItemType == ItemType.Fence)
                {
                    // We want to check to see if the target tile contains an object.
                    if (here.objects.ContainsKey(targetTile))
                    {
                        SObject o = here.objects[targetTile];

                        if (o != null)
                        {
                            // We try to identify what kind of object is placed here.
                            if (IsTypeOfObject(o, ItemType.Fence))
                            {
                                if (config.EnableReplacingFences)
                                {
                                    // We have a fence, so we want to remove it before placing our new one.
                                    DemolishOnTile(targetTile, TileFeature.Object);
                                }
                            }
                            else
                            {
                                // If it isn't a fence, we want to refund the item, and return to avoid placing the fence.
                                RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                                return;
                            }
                        }
                    }

                    if (!itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player))
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.GrassStarter)
                {
                    Grass grassStarter = new Grass(1, 4);

                    // At this point, we *need* there to be no TerrainFeature present.
                    if (!here.terrainFeatures.ContainsKey(targetTile))
                        here.terrainFeatures.Add(targetTile, grassStarter);
                    else
                    {
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);

                        // We now want to jump straight out of this method, because this will flow through to the below if, and bad things may happen.
                        return;
                    }

                    if (!(here.terrainFeatures.ContainsKey(item.Key) && here.terrainFeatures[targetTile] is Grass))
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_TerrainFeature_Generic_AlreadyPresent(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.CrabPot)
                {
                    CrabPot pot = new CrabPot(targetTile);

                    if (CanBePlacedHere(targetTile, itemToPlace))
                    {
                        itemToPlace.placementAction(Game1.currentLocation, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);
                    }
                }
                else if (itemInfo.ItemType == ItemType.Seed)
                {
                    // Here, we're dealing with a seed, so we need very special logic for this.
                    // Item.placementAction for seeds is semi-broken, unless the player is currently
                    // holding the specific seed being planted.

                    bool successfullyPlaced = false;

                    // First, we check for a TerrainFeature.
                    if (Game1.currentLocation.terrainFeatures.ContainsKey(targetTile))
                    {
                        // Then, we check to see if it's a HoeDirt.
                        if (Game1.currentLocation.terrainFeatures[targetTile] is HoeDirt)
                        {
                            // If it is, we grab a reference to it.
                            HoeDirt hd = (HoeDirt)Game1.currentLocation.terrainFeatures[targetTile];

                            // We check to see if it can be planted, and act appropriately.
                            if (hd.canPlantThisSeedHere(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y))
                            {
                                successfullyPlaced = hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, false, Game1.currentLocation);
                            }
                        }
                    }

                    // If the planting failed, we refund the seed.
                    if (!successfullyPlaced)
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Seeds_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Fertilizer)
                {
                    // First, we get whether or not More Fertilizers can place this fertiliser.
                    if (this.moreFertilizersAPI?.CanPlaceFertilizer(itemToPlace, here, targetTile) == true)
                    {
                        // If it can, we try to place it.
                        if (this.moreFertilizersAPI.TryPlaceFertilizer(itemToPlace, here, targetTile))
                        {
                            // If the placement is successful, we do the fancy animation thing.
                            this.moreFertilizersAPI.AnimateFertilizer(itemToPlace, here, targetTile);
                        }
                        else
                        {
                            // Otherwise, the fertiliser gets refunded.
                            RefundItem(itemToPlace, $"{I18n.SmartBuilding_Integrations_MoreFertilizers_InvalidFertiliserPosition()}: {itemToPlace.Name} @ {targetTile}", LogLevel.Debug);
                        }
                    }

                    if (here.terrainFeatures.ContainsKey(targetTile))
                    {
                        // We know there's a TerrainFeature here, so next we want to check if it's HoeDirt.
                        if (here.terrainFeatures[targetTile] is HoeDirt)
                        {
                            // If it is, we want to grab the HoeDirt, check if it's already got a fertiliser, and fertilise if not.
                            HoeDirt hd = (HoeDirt)here.terrainFeatures[targetTile];

                            // 0 here means no fertilizer. TODO: Known change in 1.6.
                            if (hd.fertilizer.Value == 0)
                            {
                                // Next, we want to check if there's already a crop here.
                                if (hd.crop != null)
                                {
                                    Crop cropToCheck = hd.crop;

                                    if (cropToCheck.currentPhase.Value == 0)
                                    {
                                        // If the current crop phase is zero, we can plant the fertilizer here.

                                        hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, true, Game1.currentLocation);
                                    }
                                }
                                else
                                {
                                    // If there is no crop here, we can plant the fertilizer with reckless abandon.
                                    hd.plant(itemToPlace.ParentSheetIndex, (int)targetTile.X, (int)targetTile.Y, Game1.player, true, Game1.currentLocation);
                                }
                            }
                            else
                            {
                                // If there is already a fertilizer here, we want to refund the item.
                                RefundItem(itemToPlace, I18n.SmartBuilding_Error_Fertiliser_AlreadyFertilised(), LogLevel.Warn);
                            }

                            // Now, we want to run the final check to see if the fertilization was successful.
                            if (hd.fertilizer.Value == 0)
                            {
                                // If there's still no fertilizer here, we need to refund the item.
                                RefundItem(itemToPlace, I18n.SmartBuilding_Error_Fertiliser_IneligibleForFertilisation(), LogLevel.Warn);
                            }
                        }
                    }

                }
                else if (itemInfo.ItemType == ItemType.TreeFertilizer)
                {
                    if (here.terrainFeatures.ContainsKey(targetTile))
                    {
                        // If there's a TerrainFeature here, we check if it's a tree.
                        if (here.terrainFeatures[targetTile] is Tree)
                        {
                            // It is a tree, so now we check to see if the tree is fertilised.
                            Tree tree = (Tree)here.terrainFeatures[targetTile];

                            // If it's already fertilised, there's no need for us to want to place tree fertiliser on it.
                            if (!tree.fertilized.Value)
                                tree.fertilize(here);
                        }
                    }
                }
                else if (itemInfo.ItemType == ItemType.Tapper)
                {
                    if (CanBePlacedHere(targetTile, itemToPlace))
                    {
                        // If there's a TerrainFeature here, we need to know if it's a tree.
                        if (here.terrainFeatures[targetTile] is Tree)
                        {
                            // If it is, we grab a reference, and check for a tapper on it already.
                            Tree tree = (Tree)here.terrainFeatures[targetTile];

                            if (!tree.tapped.Value)
                            {
                                if (!itemToPlace.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player))
                                {
                                    // If the placement action didn't succeed, we refund the item.
                                    RefundItem(itemToPlace, I18n.SmartBuilding_Error_TreeTapper_PlacementFailed(), LogLevel.Error);
                                }
                            }
                        }
                    }
                }
                else if (itemInfo.ItemType == ItemType.FishTankFurniture)
                {
                    // TODO: This cannot be reached, because placement of fish tanks is blocked for now.
                    // // We're dealing with a fish tank. This has dangerous consequences.
                    // if (_config.LessRestrictiveFurniturePlacement)
                    // {
                    //  FishTankFurniture tank = new FishTankFurniture(itemToPlace.ParentSheetIndex, targetTile);
                    //
                    //  foreach (var fish in (itemToPlace as FishTankFurniture).tankFish)
                    //  {
                    //      tank.tankFish.Add(fish);
                    //  }
                    //
                    //  foreach (var fish in tank.tankFish)
                    //  {
                    //      fish.ConstrainToTank();
                    //  }
                    //  
                    //  here.furniture.Add(tank);
                    // }
                    // else
                    // {
                    //  (itemToPlace as FishTankFurniture).placementAction(here, (int)targetTile.X, (int)targetTile.Y, Game1.player);
                    // }
                }
                else if (itemInfo.ItemType == ItemType.StorageFurniture)
                {
                    if (config.EnablePlacingStorageFurniture)
                    {
                        bool placedSuccessfully = false;

                        // We need to create a new instance of StorageFurniture.
                        StorageFurniture storage = new StorageFurniture(itemToPlace.ParentSheetIndex, targetTile);

                        // A quick bool to avoid an unnecessary log to console later.
                        bool anyItemsAdded = false;
                        
                        // Then, we iterate through all of the items in the existing StorageFurniture, and add them to the new one.
                        foreach (var itemInStorage in (itemToPlace as StorageFurniture).heldItems)
                        {
                            logger.Log($"{I18n.SmartBuilding_Message_StorageFurniture_AddingItem()} {itemInStorage.Name} ({itemInStorage.ParentSheetIndex}).", LogLevel.Info);
                            storage.AddItem(itemInStorage);

                            anyItemsAdded = true;
                        }
                        
                        // If any items were added, inform the user of the purpose of logging them.
                        if (anyItemsAdded)
                            logger.Log(I18n.SmartBuilding_Message_StorageFurniture_RetrievalTip(), LogLevel.Info);

                        // If we have less restrictive furniture placement enabled, we simply try to place it. Otherwise, we use the vanilla placementAction.
                        if (config.LessRestrictiveFurniturePlacement)
                            here.furniture.Add(storage as StorageFurniture);
                        else
                            placedSuccessfully = storage.placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                        // Here, we check to see if the placement was successful. If not, we refund the item.
                        if (!here.furniture.Contains(storage) && !placedSuccessfully)
                            RefundItem(storage, I18n.SmartBuilding_Error_StorageFurniture_PlacementFailed(), LogLevel.Info);
                    }
                    else
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_StorageFurniture_SettingIsOff(), LogLevel.Info, true);
                }
                else if (itemInfo.ItemType == ItemType.TvFurniture)
                {
                    bool placedSuccessfully = false;
                    TV tv = null;

                    // We need to determine which we we're placing this TV based upon the furniture placement restriction option.
                    if (config.LessRestrictiveFurniturePlacement)
                    {
                        tv = new TV(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(tv);
                    }
                    else
                    {
                        placedSuccessfully = (itemToPlace as TV).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);
                    }

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (tv != null && !here.furniture.Contains(tv as TV) && !placedSuccessfully)
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_TvFurniture_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.BedFurniture)
                {
                    bool placedSuccessfully = false;
                    BedFurniture bed = null;

                    // We decide exactly how we're placing the furniture based upon the less restrictive setting.
                    if (config.LessRestrictiveBedPlacement)
                    {
                        bed = new BedFurniture(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(bed);
                    }
                    else
                        placedSuccessfully = (itemToPlace as BedFurniture).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (bed != null && !here.furniture.Contains(bed as BedFurniture) && !placedSuccessfully)
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_BedFurniture_PlacementFailed(), LogLevel.Error);

                }
                else if (itemInfo.ItemType == ItemType.GenericFurniture)
                {
                    bool placedSuccessfully = false;
                    Furniture furniture = null;

                    // Determine exactly how we're placing this furniture.
                    if (config.LessRestrictiveFurniturePlacement)
                    {
                        furniture = new Furniture(itemToPlace.ParentSheetIndex, targetTile);
                        here.furniture.Add(furniture);
                    }
                    else
                        placedSuccessfully = (itemToPlace as Furniture).placementAction(here, (int)targetTile.X * 64, (int)targetTile.Y * 64, Game1.player);

                    // If both of these are false, the furniture was not successfully placed, so we need to refund the item.
                    if (furniture != null && !here.furniture.Contains(furniture as Furniture) && !placedSuccessfully)
                        RefundItem(itemToPlace, I18n.SmartBuilding_Error_Furniture_PlacementFailed(), LogLevel.Error);
                }
                else if (itemInfo.ItemType == ItemType.Torch)
                {
                    // We need to figure out whether there's a fence in the placement tile.
                    if (here.objects.ContainsKey(targetTile))
                    {
                        // We know there's an object at these coordinates, so we grab a reference.
                        SObject o = here.objects[targetTile];

                        if (IsTypeOfObject(o, ItemType.Fence))
                        {
                            // If the object in this tile is a fence, we add the torch to it.
                            //itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player);
                            
                            // We know it's a fence by type, but we need to make sure it isn't a gate, and to ensure it isn't already "holding" anything.
                            if (!o.Name.Equals("Gate") && o.heldObject != null)
                            {
                                // There's something in there, so we need to refund the torch.
                                RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Torch_PlacementInFenceFailed(), LogLevel.Error);
                            }
                            
                            o.performObjectDropInAction(itemToPlace, false, Game1.player);

                            if (IdentifyItemType(o.heldObject) != ItemType.Torch)
                            {
                                // If the fence isn't "holding" a torch, there was a problem, so we should refund.
                                RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Torch_PlacementInFenceFailed(), LogLevel.Error);
                            }

                            return;
                        }
                        else
                        {
                            // If it's not a fence, we want to refund the item.
                            RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                            
                            return;
                        }
                    }
                    
                    // There is no object here, so we treat it like a generic placeable.
                    if (!itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player))
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                }
                else
                { // We're dealing with a generic placeable.
                    bool successfullyPlaced = itemToPlace.placementAction(Game1.currentLocation, (int)item.Key.X * 64, (int)item.Key.Y * 64, Game1.player);

                    // if (Game1.currentLocation.objects.ContainsKey(item.Key) && Game1.currentLocation.objects[item.Key].Name.Equals(itemToPlace.Name))
                    if (!successfullyPlaced)
                        RefundItem(item.Value.Item, I18n.SmartBuilding_Error_Object_PlacementFailed(), LogLevel.Error);
                }
            }
            else
            {
                RefundItem(item.Value.Item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">The item to be refuneded to the player's inventory.</param>
        /// <param name="reason">The reason for the refund. This could be an error, or simply the player cancelling the build.</param>
        /// <param name="logLevel">The <see cref="StardewModdingAPI.LogLevel"/> to log with.</param>
        /// <param name="shouldLog">Whether or not to log. This is overridden by <see cref="StardewModdingAPI.LogLevel.Alert"/>, <see cref="StardewModdingAPI.LogLevel.Error"/>, and <see cref="StardewModdingAPI.LogLevel.Warn"/>.</param>
        private void RefundItem(Item item, string reason = "Something went wrong", LogLevel logLevel = LogLevel.Trace, bool shouldLog = false)
        {
            Game1.player.addItemByMenuIfNecessary(item.getOne());

            if (shouldLog || logLevel == LogLevel.Debug || logLevel == LogLevel.Error || logLevel == LogLevel.Warn || logLevel == LogLevel.Alert)
                monitor.Log($"{reason} {I18n.SmartBuilding_Error_Refunding_RefundingItemToPlayerInventory()} {item.Name}", logLevel);
        }

        private int? GetFlooringIdFromName(string itemName)
        {
            // TODO: Investigate whether or not there's a less terrible way to do this.
            switch (itemName)
            {
                case "Wood Floor":
                    return 0; // Correct.
                case "Rustic Plank Floor":
                    return 11; // Correct.
                case "Straw Floor":
                    return 4; // Correct
                case "Weathered Floor":
                    return 2; // Correct.
                case "Crystal Floor":
                    return 3; // Correct.
                case "Stone Floor":
                    return 1; // Correct.
                case "Stone Walkway Floor":
                    return 12; // Correct.
                case "Brick Floor":
                    return 10; // Correct
                case "Wood Path":
                    return 6; // Correct.
                case "Gravel Path":
                    return 5; // Correct.
                case "Cobblestone Path":
                    return 8; // Correct.
                case "Stepping Stone Path":
                    return 9; // Correct.
                case "Crystal Path":
                    return 7; // Correct.
                default:
                    return null;
            }
        }

        private string? GetFlooringNameFromId(int id)
        {
            // TODO: Investigate whether or not there's a less terrible way to do this.
            switch (id)
            {
                case 0:
                    return "Wood Floor"; // Correct.
                case 11:
                    return "Rustic Plank Floor"; // Correct.
                case 4:
                    return "Straw Floor"; // Correct
                case 2:
                    return "Weathered Floor"; // Correct.
                case 3:
                    return "Crystal Floor"; // Correct.
                case 1:
                    return "Stone Floor"; // Correct.
                case 12:
                    return "Stone Walkway Floor"; // Correct.
                case 10:
                    return "Brick Floor"; // Correct
                case 6:
                    return "Wood Path"; // Correct.
                case 5:
                    return "Gravel Path"; // Correct.
                case 8:
                    return "Cobblestone Path"; // Correct.
                case 9:
                    return "Stepping Stone Path"; // Correct.
                case 7:
                    return "Crystal Path"; // Correct.
                default:
                    return null;
            }
        }

        private int? GetChestType(string itemName)
        {
            // TODO: Investigate whether or not there's a less terrible way to do this.
            switch (itemName)
            {
                case "Chest":
                    return 130;
                case "Stone Chest":
                    return 232;
                case "Junimo Chest":
                    return 256;
                default:
                    return null;
            }
        }
    }
}