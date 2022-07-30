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
using System.Reflection.Emit;
using DecidedlyShared.APIs;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SmartBuilding.APIs;
using DecidedlyShared.Logging;
using SmartBuilding.UI;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;
using IGenericModConfigMenuApi = SmartBuilding.APIs.IGenericModConfigMenuApi;
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

        // Mod state
        private ModState modState;
        private Options.ItemStowingModes previousStowingMode;

        // Helper utilities
        private DrawingUtils drawingUtils;
        private IdentificationUtils identificationUtils;
        private PlacementUtils placementUtils;
        private PlayerUtils playerUtils;
        private WorldUtils worldUtils;

        // UI gubbins
        private Texture2D itemBox = null!;
        private Texture2D toolButtonsTexture;
        private ButtonActions buttonActions;
        private Toolbar gameToolbar;
        private int currentMouseX;
        private int currentMouseY;
        private ToolMenu toolMenuUi;

        // Debug stuff to make my life less painful when going through my pre-release checklist.
        private ConsoleCommand commands = null!;

        // Mod integrations.
        private IMoreFertilizersAPI? moreFertilizersApi;
        private IDynamicGameAssetsApi? dgaApi;
        private ITapGiantCropsAPI? giantCropTapApi;

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
            monitor = Monitor;
            logger = new Logger(monitor, helper.Translation);
            config = ModEntry.helper.ReadConfig<ModConfig>();

            // This is where we'll register with GMCM.
            ModEntry.helper.Events.GameLoop.GameLaunched += GameLaunched;

            // This is fired whenever input is changed, so we check for input here.
            ModEntry.helper.Events.Input.ButtonsChanged += OnInput;

            // This is used to have the queued builds draw themselves in the world.
            ModEntry.helper.Events.Display.RenderedWorld += RenderedWorld;

            // This is a huge mess, and is used to draw the building mode HUD, and build queue if enabled.
            ModEntry.helper.Events.Display.RenderedHud += RenderedHud;

            // This is purely for our rectangle quantity drawing.
            ModEntry.helper.Events.Display.Rendered += Rendered;

            // We need this to handle our custom UI events. The alternative is a Harmony patch, but that feels excessive.
            ModEntry.helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;

            // If the screen is changed, clear our painted tiles, because currently, placing objects is done on the current screen.
            ModEntry.helper.Events.Player.Warped += (sender, args) =>
            {
                LeaveBuildMode();
            };

            ModEntry.helper.Events.GameLoop.SaveLoaded += (sender, args) =>
            {
                LeaveBuildMode();
            };

            ModEntry.helper.Events.GameLoop.ReturnedToTitle += (sender, args) =>
            {
                LeaveBuildMode();
            };

                ModEntry.helper.Events.Content.AssetRequested += OnAssetRequested;
            
            // Load up our textures.
            toolButtonsTexture = helper.GameContent.Load<Texture2D>("Mods/SmartBuilding/ToolButtons");
            itemBox = helper.GameContent.Load<Texture2D>("LooseSprites/tailoring");

            Harmony harmony = new Harmony(ModManifest.UniqueID);

            // All of our Harmony patches to disable interactions while in build mode.
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
            
            harmony.Patch(
                original: AccessTools.Method(typeof(StorageFurniture), nameof(StorageFurniture.checkForAction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.Patches), nameof(HarmonyPatches.Patches.StorageFurniture_DoAction_Prefix)));
        }
        
        #region SMAPI Events
        
        private void GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // We need a reference to the toolbar in order to detect if the cursor is over it or not.
            foreach (IClickableMenu menu in Game1.onScreenMenus)
            {
                if (menu is Toolbar)
                    gameToolbar = (Toolbar)menu;
            }

            // Then we register with GMCM.
            RegisterWithGmcm();
            
            // Set up our mod integrations.
            SetupModIntegrations();
            
            // Set up our helpers.
            drawingUtils = new DrawingUtils();
            identificationUtils = new IdentificationUtils(ModEntry.helper, logger, config, dgaApi, moreFertilizersApi, placementUtils);
            placementUtils = new PlacementUtils(config, identificationUtils, moreFertilizersApi, giantCropTapApi, logger, helper);
            playerUtils = new PlayerUtils(logger);
            worldUtils = new WorldUtils(identificationUtils, placementUtils,  playerUtils, giantCropTapApi, config, logger, moreFertilizersApi);
            modState = new ModState(logger, playerUtils, identificationUtils, worldUtils, placementUtils);
            buttonActions = new ButtonActions(this, modState); // Ew, no. Fix this ugly nonsense later.
            
            // Set up our console commands.
            commands = new ConsoleCommand(logger, this, dgaApi, identificationUtils);
            this.Helper.ConsoleCommands.Add("sb_test", I18n.SmartBuilding_Commands_Debug_SbTest(), commands.TestCommand);
            this.Helper.ConsoleCommands.Add("sb_identify_all_items", I18n.SmartBuilding_Commands_Debug_SbIdentifyItems(), commands.IdentifyItemsCommand);
            this.Helper.ConsoleCommands.Add("sb_identify_cursor_target", "Identify targets under the cursor.", commands.IdentifyCursorTarget);
            
            // Then get the initial state of the item stowing mode setting.
            previousStowingMode = Game1.options.stowingMode;
        }

        /// <summary>
        /// SMAPI's <see cref="IGameLoopEvents.UpdateTicking"/> event.
        /// </summary>
        private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            if (toolMenuUi != null)
            {
                // If our tool menu is enabled and there's no menu up, we go forward with processing its events.
                if (toolMenuUi.Enabled && Game1.activeClickableMenu == null)
                {
                    MouseState mouseState = Game1.input.GetMouseState();
                    currentMouseX = mouseState.X;
                    currentMouseY = mouseState.Y;

                    currentMouseX = (int)MathF.Floor(currentMouseX / Game1.options.uiScale);
                    currentMouseY = (int)MathF.Floor(currentMouseY / Game1.options.uiScale);

                    // We need to process our custom middle click held event.
                    // if (mouseState.MiddleButton == ButtonState.Pressed && Game1.oldMouseState.MiddleButton == ButtonState.Pressed)
                    if (config.HoldToMoveMenu.IsDown())
                        toolMenuUi.MiddleMouseHeld(currentMouseX, currentMouseY);
                    // if (mouseState.MiddleButton == ButtonState.Released)
                    if (!config.HoldToMoveMenu.IsDown())
                        toolMenuUi.MiddleMouseReleased(currentMouseX, currentMouseY);

                    // Do our hover event.
                    toolMenuUi.DoHover(currentMouseX, currentMouseY);

                    toolMenuUi.SetCursorHoverState(currentMouseX, currentMouseY);

                    // We also need to manually call a click method, because by default, it'll only work if the bounds of the IClickableMenu contain the cursor.
                    // We specifically do not want the bounds to be expanded to include the side layer buttons, however, because that will be far too large a boundary.
                    if ((mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released) || config.HoldToDraw.JustPressed())
                    {
                        toolMenuUi.ReceiveLeftClick(currentMouseX, currentMouseY);
                    }
                }
            }
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
                            ItemType type = identificationUtils.IdentifyItemType((SObject)player.CurrentItem);
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
                        ProducerType type = identificationUtils.IdentifyProducer(producer);

                        logger.Log($"Identified producer {producer.Name} as {type}.");
                        logger.Log($"{I18n.SmartBuilding_Message_ProducerBeingIdentified()} {producer.Name}");
                        logger.Log($"{I18n.SmartBuilding_Message_IdentifiedProducerType()}: {type}");
                    }
                }
            }

            // If the player presses to engage build mode, we flip the bool.
            if (config.EngageBuildMode.JustPressed())
            {
                if (!modState.BuildingMode)
                {
                    EnterBuildMode();
                }
                else
                {
                    LeaveBuildMode();
                }
            }
            
            // Handle our tool hotkeys.
            if (modState.BuildingMode)
            {
                // We're in building mode, but we need to check to see if our UI has been instantiated.
                if (toolMenuUi != null)
                {
                    // It's not null, so we check to see if it's enabled.
                    if (toolMenuUi.Enabled)
                    {
                        // It's enabled, so now we go through our tool hotkeys.
                        if (config.DrawTool.JustPressed())
                        {
                            modState.ActiveTool = ButtonId.Draw;
                        }
                        
                        if (config.EraseTool.JustPressed())
                        {
                            modState.ActiveTool = ButtonId.Erase;
                        }
                        
                        if (config.FilledRectangleTool.JustPressed())
                        {
                            modState.ActiveTool = ButtonId.FilledRectangle;
                        }
                        
                        if (config.InsertTool.JustPressed())
                        {
                            modState.ActiveTool = ButtonId.Insert;
                        }

                        if (config.CommitBuild.JustPressed())
                        {
                            ConfirmBuild();
                        }

                        if (config.CancelBuild.JustPressed())
                        {
                            ClearBuild();
                        }
                        
                        // Now we only want and need to go through our layer hotkeys if the active tool is the eraser.
                        if (modState.ActiveTool == ButtonId.Erase)
                        {
                            // It is, so we go through them.
                            if (config.DrawnLayer.JustPressed())
                            {
                                modState.SelectedLayer = TileFeature.Drawn;
                            }
                            
                            if (config.ObjectLayer.JustPressed())
                            {
                                modState.SelectedLayer = TileFeature.Object;
                            }
                            
                            if (config.FloorLayer.JustPressed())
                            {
                                modState.SelectedLayer = TileFeature.TerrainFeature;
                            }
                            
                            if (config.FurnitureLayer.JustPressed())
                            {
                                modState.SelectedLayer = TileFeature.Furniture;
                            }
                        }
                    }
                }
            }

            // If the player is attempting to draw placeables in the world. 
            if (config.HoldToDraw.IsDown())
            {
                if (modState.BuildingMode)
                {
                    // We don't want to do anything here if we're hovering over the menu, or the toolbar.
                    if (!modState.BlockMouseInteractions && !gameToolbar.isWithinBounds(currentMouseX, currentMouseY))
                    {
                        // First, we need to make sure there even is a tool active.
                        if (modState.ActiveTool != ButtonId.None)
                        {
                            // There is, so we want to determine exactly which tool we're working with.
                            switch (modState.ActiveTool)
                            {
                                case ButtonId.Draw:
                                    // We don't want to draw if the cursor is in the negative.
                                    if (Game1.currentCursorTile.X < 0 || Game1.currentCursorTile.Y < 0)
                                        return;

                                    modState.AddTile(Game1.player.CurrentItem, Game1.currentCursorTile, this);
                                    if (config.InstantlyBuild)
                                        buttonActions.ConfirmBuildClicked();
                                    break;
                                case ButtonId.Erase:
                                    // if (modState.SelectedLayer.HasValue)
                                    // {
                                        worldUtils.DemolishOnTile(Game1.currentCursorTile, modState.SelectedLayer);
                                        modState.EraseTile(Game1.currentCursorTile, this);
                                    // }
                                    break;
                                case ButtonId.FilledRectangle:
                                    // This is a split method and is hideous, but this is the best I can think of for now.

                                    modState.RectangleItem = Game1.player.CurrentItem;

                                    if (modState.StartTile == null)
                                    {
                                        // If the start tile hasn't yet been set, then we want to set that.
                                        modState.StartTile = Game1.currentCursorTile;
                                    }

                                    modState.EndTile = Game1.currentCursorTile;

                                    modState.RectTiles = CalculateRectangle(modState.StartTile.Value, modState.EndTile.Value, modState.RectangleItem);

                                    break;
                                case ButtonId.Insert:
                                    this.AddItem(Game1.player.CurrentItem, Game1.currentCursorTile);
                                    break;
                            }
                        }
                    }
                }
            }
            else if (config.HoldToDraw.GetState() == SButtonState.Released)
            {
                // We don't care to do this if there's no tool active.
                if (modState.ActiveTool != ButtonId.None)
                {
                    if (modState.ActiveTool == ButtonId.FilledRectangle)
                    {
                        // We need to process the key up stuff for the filled rectangle.

                        // The rectangle drawing key was released, so we want to calculate the tiles within, and set CurrentlyDrawing to false.

                        if (modState.StartTile.HasValue && modState.EndTile.HasValue)
                        {
                            List<Vector2> tiles = CalculateRectangle(modState.StartTile.Value, modState.EndTile.Value, modState.RectangleItem);

                            foreach (Vector2 tile in tiles)
                            {
                                modState.AddTile(modState.RectangleItem, tile, this);
                            }

                            modState.StartTile = null;
                            modState.EndTile = null;
                            modState.RectTiles.Clear();
                        }
                    }
                }

                // Otherwise, the key is up, meaning we want to indicate we're not currently drawing.
                //CurrentlyDrawing = false;
            }
        }

        private void EnterBuildMode()
        {
            // If the world isn't ready, we return.
            if (!Context.IsWorldReady)
                return;

            // If it's a festival, we return.
            if (Game1.isFestival())
                return;
            
            modState.BuildingMode = true;
            // We're entering building mode, so we create our UI.
            CreateToolUi();

            // First we save the current item stowing mode
            previousStowingMode = Game1.options.stowingMode;

            // Then we set it to off to avoid a strange stuttery drawing issue.
            Game1.options.stowingMode = Options.ItemStowingModes.Off;
        }

        private void LeaveBuildMode()
        {
            modState.BuildingMode = false;

            // Kill our UI.
            KillToolUi();

            // And set our active tool and layer to none.
            modState.ActiveTool = ButtonId.None;
            modState.SelectedLayer = TileFeature.None;

            // Reset the state of the mod.
            modState.ResetState();

            // Then, finally, set the stowing mode back to what it used to be.
            Game1.options.stowingMode = previousStowingMode;
        }

        /// <summary>
        /// SMAPI's <see cref="IDisplayEvents.RenderedWorld"/> event.
        /// </summary>
        private void Rendered(object? sender, RenderedEventArgs e)
        {
            if (modState.BuildingMode)
            {
                // Now, we render our rectangle quantity amount.
                if (modState.RectTiles != null)
                {
                    foreach (Vector2 tile in modState.RectTiles)
                    {
                        Utility.drawTinyDigits(
                            modState.RectTiles.Count,
                            e.SpriteBatch,
                            // new Vector2(100, 100),
                            new Vector2(Game1.getMouseX() + 38, Game1.getMouseY() + 86),
                            3f,
                            -10f,
                            Color.White);
                    }
                }
            }
        }
        
        /// <summary>
        /// Render the drawn queue in the world.
        /// </summary>
        private void RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            SpriteBatch b = e.SpriteBatch;

            foreach (KeyValuePair<Vector2, ItemInfo> item in modState.TilesSelected)
            {
                // Here, we simply have the Item draw itself in the world.
                item.Value.Item.drawInMenu
                (e.SpriteBatch,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        item.Key * Game1.tileSize),
                    1f, 1f, 4f, StackDrawType.Hide);
            }

            if (modState.RectTiles != null)
            {
                foreach (Vector2 tile in modState.RectTiles)
                {
                    // Here, we simply have the Item draw itself in the world.
                    modState.RectangleItem.drawInMenu
                    (e.SpriteBatch,
                        Game1.GlobalToLocal(
                            Game1.viewport,
                            tile * Game1.tileSize),
                        1f, 1f, 4f, StackDrawType.Hide);
                }
            }
        }

        /// <summary>
        /// SMAPI's <see cref="IDisplayEvents.RenderedHud"/> event. 
        /// </summary>
        private void RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            // There's absolutely no need to run this while we're not in building mode.
            if (modState.BuildingMode)
            {
                if (config.ShowBuildQueue)
                {
                    Dictionary<Item, int> itemAmounts = new Dictionary<Item, int>();

                    foreach (var item in modState.TilesSelected.Values.GroupBy(x => x))
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

                        drawingUtils.DrawStringWithShadow(
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

                // Now, we render our rectangle quantity amount.
                if (modState.RectTiles != null)
                {
                    foreach (Vector2 tile in modState.RectTiles)
                    {
                    }
                }
            }
            
            // TODO: DEBUG STUFF.
            foreach (ResourceClump clump in Game1.currentLocation.resourceClumps)
            {
                Vector2 v = Game1.currentCursorTile;
                Item tapper = Utility.fuzzyItemSearch("Tapper");
                
                if (clump is GiantCrop && clump.occupiesTile((int)v.X, (int)v.Y))
                {
                    // It's a giant crop, so we defer to Tap Giant Crop's API for placement validity.

                    if (giantCropTapApi != null)
                    {
                        bool canPlace = giantCropTapApi.CanPlaceTapper(Game1.currentLocation, v, (SObject)tapper);

                        if (canPlace)
                        {
                            e.SpriteBatch.Draw(Game1.mouseCursors, 
                                new Vector2(Game1.getMouseX(), Game1.getMouseY()), 
                                new Rectangle(194, 388, 16, 16), 
                                Color.White, 
                                0f, 
                                Vector2.Zero, 
                                4f, 
                                SpriteEffects.None, 
                                1f);
                        }
                        else
                        {
                            e.SpriteBatch.Draw(Game1.mouseCursors, 
                                new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                                new Rectangle(194, 388, 16, 16), 
                                Color.Red, 
                                0f, 
                                Vector2.Zero, 
                                4f, 
                                SpriteEffects.None, 
                                1f);
                        }
                            
                    }
                }
            }
        }

        #endregion

        private void KillToolUi()
        {
            // If our tool UI is not null, we set it to disabled.
            if (toolMenuUi != null)
                toolMenuUi.Enabled = false;

            modState.ClearPaintedTiles();

            // And, if our UI exists, we kill it.
            if (Game1.onScreenMenus.Contains(toolMenuUi))
                Game1.onScreenMenus.Remove(toolMenuUi);
        }

        private void CreateToolUi()
        {
            // First, we create our list of buttons.
            List<ToolButton> toolButtons = new List<ToolButton>()
            {
                new ToolButton(ButtonId.Draw, ButtonType.Tool, buttonActions.DrawClicked,
                    I18n.SmartBuilding_Buttons_Draw_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.Erase, ButtonType.Tool, buttonActions.EraseClicked,
                    I18n.SmartBuilding_Buttons_Erase_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.FilledRectangle, ButtonType.Tool, buttonActions.FilledRectangleClicked,
                    I18n.SmartBuilding_Buttons_FilledRectangle_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.Insert, ButtonType.Tool, buttonActions.InsertClicked,
                    I18n.SmartBuilding_Buttons_Insert_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.ConfirmBuild, ButtonType.Function, buttonActions.ConfirmBuildClicked,
                    I18n.SmartBuilding_Buttons_ConfirmBuild_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.ClearBuild, ButtonType.Function, buttonActions.ClearBuildClicked,
                    I18n.SmartBuilding_Buttons_ClearBuild_Tooltip(), toolButtonsTexture, modState),
                new ToolButton(ButtonId.DrawnLayer, ButtonType.Layer, buttonActions.DrawnLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerDrawn_Tooltip(), toolButtonsTexture, modState, TileFeature.Drawn),
                new ToolButton(ButtonId.ObjectLayer, ButtonType.Layer, buttonActions.ObjectLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerObject_Tooltip(), toolButtonsTexture, modState, TileFeature.Object),
                new ToolButton(ButtonId.TerrainFeatureLayer, ButtonType.Layer, buttonActions.TerrainFeatureLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerTerrainfeature_Tooltip(), toolButtonsTexture, modState, TileFeature.TerrainFeature),
                new ToolButton(ButtonId.FurnitureLayer, ButtonType.Layer, buttonActions.FurnitureLayerClicked,
                    I18n.SmartBuilding_Buttons_LayerFurniture_Tooltip(), toolButtonsTexture, modState, TileFeature.Furniture),
            };

            // If we're enabling building mode, we create our UI, and set it to enabled.
            toolMenuUi = new ToolMenu(logger, toolButtonsTexture, toolButtons, modState);
            toolMenuUi.Enabled = true;

            // Then, if it isn't already in onScreenMenus, we add it.
            if (!Game1.onScreenMenus.Contains(toolMenuUi))
            {
                Game1.onScreenMenus.Add(toolMenuUi);
            }
        }
        
        private void SetupModIntegrations()
        {
            // First, check whether More Fertilizers is installed.
            if (this.Helper.ModRegistry.IsLoaded("atravita.MoreFertilizers"))
            {
                // I don't feel like I need a version check here, because this is v1.0 API stuff.
                
                // And then grab the API for atravita's More Fertilizers mod.
                try
                {
                    this.moreFertilizersApi = this.Helper.ModRegistry.GetApi<IMoreFertilizersAPI>("atravita.MoreFertilizers");
                }
                catch (Exception e)
                {
                    logger.Exception(e);
                }
            }
            
            // First, check whether DGA is installed.
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                if (this.Helper.ModRegistry.Get("spacechase0.DynamicGameAssets") is IModInfo modInfo)
                {
                    if (modInfo.Manifest.Version.IsOlderThan("1.4.3"))
                    {
                        logger.Log("Installed version of DGA is too low. Please update to DGA v1.4.3 or higher.");
                        dgaApi = null;
                    }
                    
                    // And then grab the API for Casey's DGA mod.
                    try
                    {
                        this.dgaApi = this.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e);
                    }
                }
            }
            
            // Check whether Tap Giant Crops is even installed
            if (helper.ModRegistry.IsLoaded("atravita.TapGiantCrops"))
            {
                // No need to check against the version here, so we just try to get the API.
                try
                {
                    this.giantCropTapApi = this.Helper.ModRegistry.GetApi<ITapGiantCropsAPI>("atravita.TapGiantCrops");
                }
                catch (Exception e)
                {
                    logger.Exception(e);
                }
            }
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

            RegisterMandatoryKeybinds(configMenuApi);

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );
            
            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_OptionalToggles_Title()
            );

            RegisterToggleSettings(configMenuApi);
            
            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_OptionalKeybinds_Title()
            );
            
            RegisterOptionalKeybinds(configMenuApi);

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_CheatyOptions_Title()
            );

            RegisterCheatyToggleOptions(configMenuApi);

            configMenuApi.AddParagraph(
                mod: ModManifest,
                text: () => "" // This is purely for spacing.
            );

            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.SmartBuilding_Settings_Debug_Title()
            );

            RegisterDebugSettings(configMenuApi);

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

            RegisterDangerousSettings(configMenuApi);

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

        private void RegisterDangerousSettings(IGenericModConfigMenuApi configMenuApi)
        {

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture(),
                tooltip: () => I18n.SmartBuilding_Settings_PotentiallyDangerous_EnablePlacingStorageFurniture_Tooltip(),
                getValue: () => config.EnablePlacingStorageFurniture,
                setValue: value => config.EnablePlacingStorageFurniture = value
            );
        }

        private void RegisterDebugSettings(IGenericModConfigMenuApi configMenuApi)
        {

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
        }

        private void RegisterCheatyToggleOptions(IGenericModConfigMenuApi configMenuApi)
        {

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
                name: () => I18n.SmartBuilding_Settings_CheatyOptions_EnableFertilisers(),
                getValue: () => config.EnableFertilizers,
                setValue: value => config.EnableFertilizers = value
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
        }

        private void RegisterToggleSettings(IGenericModConfigMenuApi configMenuApi)
        {

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_ShowBuildQueue(),
                getValue: () => config.ShowBuildQueue,
                setValue: value => config.ShowBuildQueue = value
            );

            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalToggles_InstantlyBuild(),
                getValue: () => config.InstantlyBuild,
                setValue: value => config.InstantlyBuild = value
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
                setValue: value => config.EnableReplacingFences = value);
        }

        private void RegisterOptionalKeybinds(IGenericModConfigMenuApi configMenuApi)
        {

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_DrawTool(),
                getValue: () => config.DrawTool,
                setValue: value => config.DrawTool = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_EraseTool(),
                getValue: () => config.EraseTool,
                setValue: value => config.EraseTool = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FilledRectangleTool(),
                getValue: () => config.FilledRectangleTool,
                setValue: value => config.FilledRectangleTool = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_InsertTool(),
                getValue: () => config.InsertTool,
                setValue: value => config.InsertTool = value
            );
            
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_CommitBuild(),
                getValue: () => config.CommitBuild,
                setValue: value => config.CommitBuild = value
            );
            
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_ClearBuild(),
                getValue: () => config.CancelBuild,
                setValue: value => config.CancelBuild = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_DrawnLayer(),
                getValue: () => config.DrawnLayer,
                setValue: value => config.DrawnLayer = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_ObjectLayer(),
                getValue: () => config.ObjectLayer,
                setValue: value => config.ObjectLayer = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FloorLayer(),
                getValue: () => config.FloorLayer,
                setValue: value => config.FloorLayer = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_OptionalKeybinds_FurnitureLayer(),
                getValue: () => config.FurnitureLayer,
                setValue: value => config.FurnitureLayer = value
            );
        }

        private void RegisterMandatoryKeybinds(IGenericModConfigMenuApi configMenuApi)
        {

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_EnterBuildMode(),
                getValue: () => config.EngageBuildMode,
                setValue: value => config.EngageBuildMode = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToDraw(),
                getValue: () => config.HoldToDraw,
                setValue: value => config.HoldToDraw = value
            );

            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => I18n.SmartBuilding_Settings_Keybinds_Binds_HoldToMoveUi(),
                getValue: () => config.HoldToMoveMenu,
                setValue: value => config.HoldToMoveMenu = value
            );
        }

        private List<Vector2> CalculateRectangle(Vector2 cornerOne, Vector2 cornerTwo, Item item)
        {
            Vector2 topLeft;
            Vector2 bottomRight;
            List<Vector2> tiles = new List<Vector2>();
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
            {
                for (int y = (int)topLeft.Y; y < rectHeight + topLeft.Y; y++)
                {
                    if (itemsRemainingInStack > 0)
                    {
                        if (placementUtils.CanBePlacedHere(new Vector2(x, y), item) && !modState.TilesSelected.Keys.Contains(new Vector2(x, y)))
                        {
                            tiles.Add(new Vector2(x, y));
                            itemsRemainingInStack--;
                        }
                    }
                }
            }

            return tiles;
        }

        /// <summary>
        /// Confirm the drawn build, and pass tiles and items into <see cref="WorldUtils.PlaceObject"/>.
        /// </summary>
        public void ConfirmBuild()
        {
            // The build has been confirmed, so we iterate through our Dictionary, and pass each tile into PlaceObject.
            foreach (KeyValuePair<Vector2, ItemInfo> v in modState.TilesSelected)
            {
                // We want to allow placement for the duration of this method.
                HarmonyPatches.Patches.AllowPlacement = true;

                worldUtils.PlaceObject(v);

                // And disallow it afterwards.
                HarmonyPatches.Patches.AllowPlacement = false;
            }

            // Then, we clear the list, because building is done, and all errors are handled internally.
            modState.TilesSelected.Clear();
        }

        /// <summary>
        /// Clear all painted tiles.
        /// </summary>
        public void ClearBuild()
        {
            modState.ClearPaintedTiles();
        }

        public void AddItem(Item item, Vector2 v)
        {
            // If we're not in building mode, we do nothing.
            if (!modState.BuildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;
            
            // If inserting items is disabled, we do nothing.
            if (!config.EnableInsertingItemsIntoMachines)
            {
                logger.Log(I18n.SmartBuilding_Message_CheatyOptions_EnableInsertingItemsIntoMachines_Disabled(), LogLevel.Trace, true);
                return;
            }

            // There is no queue for item insertion, so we simply try to insert.
            modState.TryToInsertHere(v, item, this);
        }
    }
}