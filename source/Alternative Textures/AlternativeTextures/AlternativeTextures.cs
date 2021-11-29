/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.External.GenericModConfigMenu;
using AlternativeTextures.Framework.External.ContentPatcher;
using AlternativeTextures.Framework.Interfaces.API;
using AlternativeTextures.Framework.Managers;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Patches;
using AlternativeTextures.Framework.Patches.SpecialObjects;
using AlternativeTextures.Framework.Patches.Buildings;
using AlternativeTextures.Framework.Patches.Entities;
using AlternativeTextures.Framework.Patches.GameLocations;
using AlternativeTextures.Framework.Patches.StandardObjects;
using AlternativeTextures.Framework.Patches.Tools;
using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xTile.Tiles;
using Microsoft.Xna.Framework.Input;

namespace AlternativeTextures
{
    public class AlternativeTextures : Mod
    {
        // Core modData keys
        internal const string TEXTURE_TOKEN_HEADER = "AlternativeTextures/Textures/";
        internal const string TOOL_TOKEN_HEADER = "AlternativeTextures/Tools/";
        internal const string DEFAULT_OWNER = "Stardew.Default";

        // Compatibility keys
        internal const string TOOL_CONVERSION_COMPATIBILITY = "AlternativeTextures.HasConvertedMilkPails";
        internal const string TYPE_FIX_COMPATIBILITY = "AlternativeTextures.HasFixedBadObjectTyping";

        // Tool related keys
        internal const string PAINT_BUCKET_FLAG = "AlternativeTextures.PaintBucketFlag";
        internal const string OLD_PAINT_BUCKET_FLAG = "AlternativeTexturesPaintBucketFlag";
        internal const string PAINT_BRUSH_FLAG = "AlternativeTextures.PaintBrushFlag";
        internal const string PAINT_BRUSH_SCALE = "AlternativeTextures.PaintBrushScale";
        internal const string SCISSORS_FLAG = "AlternativeTextures.ScissorsFlag";

        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static Multiplayer multiplayer;
        internal static ModConfig modConfig;

        // Managers
        internal static TextureManager textureManager;
        internal static ApiManager apiManager;
        internal static AssetManager assetManager;

        // Utilities
        internal static FpsCounter fpsCounter;

        // Debugging flags
        private bool _displayFPS = false;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Setup our managers
            textureManager = new TextureManager(monitor);
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(helper);

            // Setup our utilities
            fpsCounter = new FpsCounter();

            // Load the asset manager
            helper.Content.AssetLoaders.Add(assetManager);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply texture override related patches
                new GameLocationPatch(monitor, helper).Apply(harmony);
                new ObjectPatch(monitor, helper).Apply(harmony);
                new FencePatch(monitor, helper).Apply(harmony);
                new HoeDirtPatch(monitor, helper).Apply(harmony);
                new CropPatch(monitor, helper).Apply(harmony);
                new GiantCropPatch(monitor, helper).Apply(harmony);
                new GrassPatch(monitor, helper).Apply(harmony);
                new TreePatch(monitor, helper).Apply(harmony);
                new FruitTreePatch(monitor, helper).Apply(harmony);
                new ResourceClumpPatch(monitor, helper).Apply(harmony);
                new BushPatch(monitor, helper).Apply(harmony);
                new FlooringPatch(monitor, helper).Apply(harmony);
                new FurniturePatch(monitor, helper).Apply(harmony);
                new BedFurniturePatch(monitor, helper).Apply(harmony);
                new FishTankFurniturePatch(monitor, helper).Apply(harmony);

                // Start of special objects
                new ChestPatch(monitor, helper).Apply(harmony);
                new CrabPotPatch(monitor, helper).Apply(harmony);
                new IndoorPotPatch(monitor, helper).Apply(harmony);
                new PhonePatch(monitor, helper).Apply(harmony);
                new TorchPatch(monitor, helper).Apply(harmony);
                new WallpaperPatch(monitor, helper).Apply(harmony);
                /*
                 * Not supported:
                 * - Wood Chipper
                 */

                // Start of entity patches
                new CharacterPatch(monitor, helper).Apply(harmony);
                new ChildPatch(monitor, helper).Apply(harmony);
                new FarmAnimalPatch(monitor, helper).Apply(harmony);
                new HorsePatch(monitor, helper).Apply(harmony);
                new PetPatch(monitor, helper).Apply(harmony);
                new MonsterPatch(monitor, helper).Apply(harmony);

                // Start of building patches
                new BuildingPatch(monitor, helper).Apply(harmony);
                new StablePatch(monitor, helper).Apply(harmony); // Specifically for Tractor Mod, to allow texture variations
                new ShippingBinPatch(monitor, helper).Apply(harmony);

                // Start of location patches
                new GameLocationPatch(monitor, helper).Apply(harmony);
                new DecoratableLocationPatch(monitor, helper).Apply(harmony);
                new FarmPatch(monitor, helper).Apply(harmony);

                // Paint tool related patches
                new UtilityPatch(monitor, helper).Apply(harmony);
                new ToolPatch(monitor, helper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in our debug commands
            helper.ConsoleCommands.Add("at_spawn_gc", "Spawns a giant crop based given harvest product id (e.g. Melon == 254).\n\nUsage: at_spawn_gc [HARVEST_ID]", this.DebugSpawnGiantCrop);
            helper.ConsoleCommands.Add("at_spawn_rc", "Spawns a resource clump based given resource name (e.g. Stump).\n\nUsage: at_spawn_rc [RESOURCE_NAME]", this.DebugSpawnResourceClump);
            helper.ConsoleCommands.Add("at_spawn_child", "Spawns a child. Potentially buggy / gamebreaking, do not use. \n\nUsage: at_spawn_child [AGE] [IS_MALE] [SKIN_TONE]", this.DebugSpawnChild);
            helper.ConsoleCommands.Add("at_set_age", "Sets age for all children in location. Potentially buggy / gamebreaking, do not use. \n\nUsage: at_set_age [AGE]", this.DebugSetAge);
            helper.ConsoleCommands.Add("at_display_fps", "Displays FPS counter. Use again to disable. \n\nUsage: at_display_fps", delegate { _displayFPS = !_displayFPS; });
            helper.ConsoleCommands.Add("at_paint_shop", "Shows the carpenter shop with the paint bucket for sale.\n\nUsage: at_paint_shop", this.DebugShowPaintShop);
            helper.ConsoleCommands.Add("at_reload", "Reloads all Alternative Texture content packs.\n\nUsage: at_reload", delegate { this.LoadContentPacks(); });

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;

            // Hook into Player events
            helper.Events.Player.Warped += this.OnWarped;

            // Hook into Input events
            helper.Events.Input.ButtonsChanged += OnButtonChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // Hook into Display events
            helper.Events.Display.Rendered += OnDisplayRendered;
        }

        private void OnDisplayRendered(object sender, RenderedEventArgs e)
        {
            if (!_displayFPS)
            {
                return;
            }

            fpsCounter.OnRendered(sender, e);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu is null && Game1.player.CurrentTool is GenericTool tool && tool.modData.ContainsKey(PAINT_BRUSH_FLAG) && e.Button is SButton.MouseRight)
            {
                Helper.Input.Suppress(e.Button);

                var xTile = (int)e.Cursor.Tile.X * 64;
                var yTile = (int)e.Cursor.Tile.Y * 64;

                // Verify that a supported object exists at the tile
                var placedObject = PatchTemplate.GetObjectAt(Game1.currentLocation, xTile, yTile);
                if (placedObject is null)
                {
                    var terrainFeature = PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, xTile, yTile);
                    if (terrainFeature is Flooring flooring)
                    {
                        var modelType = AlternativeTextureModel.TextureType.Flooring;
                        if (!flooring.modData.ContainsKey("AlternativeTextureName") || !flooring.modData.ContainsKey("AlternativeTextureVariation"))
                        {
                            // Assign default modData
                            var instanceSeasonName = $"{modelType}_{PatchTemplate.GetFlooringName(flooring)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                            PatchTemplate.AssignDefaultModData(flooring, instanceSeasonName, true);
                        }

                        Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.texture_copied"), 2) { timeLeft = 1000 });
                        tool.modData[PAINT_BRUSH_FLAG] = $"{modelType}_{PatchTemplate.GetFlooringName(flooring)}";
                        tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                        tool.modData["AlternativeTextureOwner"] = flooring.modData["AlternativeTextureOwner"];
                        tool.modData["AlternativeTextureName"] = flooring.modData["AlternativeTextureName"];
                        tool.modData["AlternativeTextureVariation"] = flooring.modData["AlternativeTextureVariation"];
                    }
                    else
                    {
                        tool.modData[PAINT_BRUSH_FLAG] = String.Empty;
                        tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                        if (terrainFeature != null)
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.brush_not_supported"), 3) { timeLeft = 2000 });
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.cleared_brush"), 2) { timeLeft = 1000 });
                        }
                    }
                }
                else
                {
                    var modelType = placedObject is Furniture ? AlternativeTextureModel.TextureType.Furniture : AlternativeTextureModel.TextureType.Craftable;
                    if (!placedObject.modData.ContainsKey("AlternativeTextureName") || !placedObject.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        var instanceSeasonName = $"{modelType}_{PatchTemplate.GetObjectName(placedObject)}_{Game1.currentSeason}";
                        PatchTemplate.AssignDefaultModData(placedObject, instanceSeasonName, true);
                    }

                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.texture_copied"), 2) { timeLeft = 1000 });
                    tool.modData[PAINT_BRUSH_FLAG] = $"{modelType}_{PatchTemplate.GetObjectName(placedObject)}";
                    tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                    tool.modData["AlternativeTextureOwner"] = placedObject.modData["AlternativeTextureOwner"];
                    tool.modData["AlternativeTextureName"] = placedObject.modData["AlternativeTextureName"];
                    tool.modData["AlternativeTextureVariation"] = placedObject.modData["AlternativeTextureVariation"];
                }
            }
        }

        private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is null && Game1.player.CurrentTool is GenericTool tool && tool.modData.ContainsKey(PAINT_BRUSH_FLAG) && e.Held.Contains(SButton.MouseLeft))
            {
                var xTile = (int)e.Cursor.Tile.X * 64;
                var yTile = (int)e.Cursor.Tile.Y * 64;

                if (String.IsNullOrEmpty(tool.modData[PAINT_BRUSH_FLAG]))
                {
                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.brush_is_empty"), 3) { timeLeft = 2000 });
                }
                else
                {
                    // Verify that a supported object exists at the tile
                    var placedObject = PatchTemplate.GetObjectAt(Game1.currentLocation, xTile, yTile);
                    if (placedObject is null)
                    {
                        var terrainFeature = PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, xTile, yTile);
                        if (terrainFeature is Flooring flooring)
                        {
                            var modelType = AlternativeTextureModel.TextureType.Flooring;
                            if (tool.modData[PAINT_BRUSH_FLAG] == $"{modelType}_{PatchTemplate.GetFlooringName(flooring)}")
                            {
                                flooring.modData["AlternativeTextureOwner"] = tool.modData["AlternativeTextureOwner"];
                                flooring.modData["AlternativeTextureName"] = tool.modData["AlternativeTextureName"];
                                flooring.modData["AlternativeTextureVariation"] = tool.modData["AlternativeTextureVariation"];
                            }
                            else
                            {
                                Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[PAINT_BRUSH_FLAG] }), 3) { timeLeft = 2000 });
                            }
                        }
                        else if (terrainFeature != null)
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.paint_not_placeable"), 3) { timeLeft = 2000 });
                        }
                    }
                    else
                    {
                        var modelType = placedObject is Furniture ? AlternativeTextureModel.TextureType.Furniture : AlternativeTextureModel.TextureType.Craftable;
                        if (tool.modData[PAINT_BRUSH_FLAG] == $"{modelType}_{PatchTemplate.GetObjectName(placedObject)}")
                        {
                            placedObject.modData["AlternativeTextureOwner"] = tool.modData["AlternativeTextureOwner"];
                            placedObject.modData["AlternativeTextureName"] = tool.modData["AlternativeTextureName"];
                            placedObject.modData["AlternativeTextureVariation"] = tool.modData["AlternativeTextureVariation"];
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[PAINT_BRUSH_FLAG] }), 3) { timeLeft = 2000 });
                        }
                    }
                }
            }
        }

        public override object GetApi()
        {
            return new Api(this);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                apiManager.HookIntoJsonAssets(Helper);
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                apiManager.HookIntoDynamicGameAssets(Helper);
            }

            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher") && apiManager.HookIntoContentPatcher(Helper))
            {
                apiManager.GetContentPatcherApi().RegisterToken(ModManifest, "Textures", new TextureToken(textureManager, assetManager));
                apiManager.GetContentPatcherApi().RegisterToken(ModManifest, "Tools", new ToolToken(textureManager, assetManager));
            }

            // Load any owned content packs
            this.LoadContentPacks();

            // Set our default configuration file
            modConfig = Helper.ReadConfig<ModConfig>();

            // Hook into GMCM, if applicable
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGenericModConfigMenu(Helper))
            {
                var configApi = apiManager.GetGenericModConfigMenuApi();
                configApi.RegisterModConfig(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));

                var contentPacks = Helper.ContentPacks.GetOwned();
                // Create the page labels for each content pack's page
                configApi.RegisterLabel(ModManifest, $"Content Packs", String.Empty);
                foreach (var contentPack in contentPacks)
                {
                    configApi.RegisterPageLabel(ModManifest, String.Concat("> ", CleanContentPackNameForConfig(contentPack.Manifest.Name)), contentPack.Manifest.Description, contentPack.Manifest.UniqueID);
                }

                // Add the content pack owner pages
                foreach (var contentPack in contentPacks)
                {
                    configApi.StartNewPage(ModManifest, contentPack.Manifest.UniqueID);
                    configApi.OverridePageDisplayName(ModManifest, contentPack.Manifest.UniqueID, CleanContentPackNameForConfig(contentPack.Manifest.Name));

                    // Create a page label for each TextureType under this content pack
                    configApi.RegisterLabel(ModManifest, $"Catagories", String.Empty);
                    foreach (var textureType in textureManager.GetAllTextures().Where(t => t.Owner == contentPack.Manifest.UniqueID).Select(t => t.GetTextureType()).Distinct().OrderBy(t => t))
                    {
                        configApi.RegisterPageLabel(ModManifest, String.Concat("> ", textureType), String.Empty, String.Concat(contentPack.Manifest.UniqueID, ".", textureType));
                    }

                    // Create a page label for each model under this content pack
                    foreach (var model in textureManager.GetAllTextures().Where(t => t.Owner == contentPack.Manifest.UniqueID).OrderBy(t => t.GetTextureType()).ThenBy(t => t.ItemName))
                    {
                        configApi.StartNewPage(ModManifest, String.Concat(model.Owner, ".", model.GetTextureType()));

                        // Create page label for each model
                        var description = $"Type: {model.GetTextureType()}\nSeason(s): {(String.IsNullOrEmpty(model.Season) ? "All" : model.Season)}\nVariations: {model.GetVariations()}";
                        configApi.RegisterPageLabel(ModManifest, String.Concat("> ", model.ItemName), description, model.GetId());
                    }

                    // Add the AlternativeTextureModel pages
                    foreach (var model in textureManager.GetAllTextures().Where(t => t.Owner == contentPack.Manifest.UniqueID))
                    {
                        configApi.StartNewPage(ModManifest, model.GetId());

                        for (int variation = 0; variation < model.GetVariations(); variation++)
                        {
                            // Add general description label
                            var description = $"Type: {model.GetTextureType()}\nSeason(s): {(String.IsNullOrEmpty(model.Season) ? "All" : model.Season)}";
                            configApi.RegisterLabel(ModManifest, $"Variation: {variation}", description);

                            // Add the reference image for the alternative texture
                            var sourceRect = new Rectangle(0, model.GetTextureOffset(variation), model.TextureWidth, model.TextureHeight);
                            switch (model.GetTextureType())
                            {
                                case "Decoration":
                                    var isFloor = model.ItemName.Equals("Floor", StringComparison.OrdinalIgnoreCase);
                                    var decorationOffset = isFloor ? 8 : 16;
                                    sourceRect = new Rectangle((variation % decorationOffset) * model.TextureWidth, (variation / decorationOffset) * model.TextureHeight, model.TextureWidth, model.TextureHeight);
                                    break;
                            }

                            var scale = 4;
                            if (model.TextureHeight >= 64)
                            {
                                scale = 2;
                            }
                            if (model.TextureHeight >= 128)
                            {
                                scale = 1;
                            }
                            configApi.RegisterImage(ModManifest, $"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{model.GetTokenId()}", sourceRect, scale);

                            // Add our custom widget, which passes over the required data needed to flag the TextureId with the appropriate Variation 
                            bool wasClicking = false;
                            var textureWidget = new TextureWidget() { TextureId = model.GetId(), Variation = variation, Enabled = !modConfig.IsTextureVariationDisabled(model.GetId(), variation) };
                            Func<Vector2, object, object> widgetUpdate = (Vector2 pos, object state) =>
                            {
                                var widget = state as TextureWidget;
                                if (widget is null)
                                {
                                    widget = textureWidget;
                                }

                                var bounds = new Rectangle((int)pos.X, (int)pos.Y, OptionsCheckbox.sourceRectChecked.Width * 4, OptionsCheckbox.sourceRectChecked.Width * 4);
                                bool isHovering = bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY());

                                bool isClicking = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
                                if (isHovering && isClicking && !wasClicking)
                                {
                                    widget.Enabled = !widget.Enabled;
                                }
                                wasClicking = isClicking;

                                return widget;
                            };
                            Func<SpriteBatch, Vector2, object, object> widgetDraw = (SpriteBatch b, Vector2 pos, object state) =>
                            {
                                var widget = state as TextureWidget;
                                b.Draw(Game1.mouseCursors, pos, widget.Enabled ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);

                                return widget;
                            };
                            Action<object> widgetSave = (object state) =>
                            {
                                if (state is null || !(state is TextureWidget widget))
                                {
                                    return;
                                }

                                modConfig.SetTextureStatus(widget.TextureId, widget.Variation, widget.Enabled);
                            };
                            configApi.RegisterLabel(ModManifest, String.Empty, String.Empty);
                            configApi.RegisterComplexOption(ModManifest, $"Enabled", $"If checked, this alternative texture will be available.", widgetUpdate, widgetDraw, widgetSave);

                            configApi.RegisterLabel(ModManifest, String.Empty, String.Empty);
                        }
                    }
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Load all available textures to account for any Content Patcher's OnDayStart updates
            UpdateTextures();

            // Backwards compatibility logic
            if (!Game1.player.modData.ContainsKey(TOOL_CONVERSION_COMPATIBILITY))
            {
                Monitor.Log($"Converting old Paint Buckets into generic tools...", LogLevel.Debug);
                Game1.player.modData[TOOL_CONVERSION_COMPATIBILITY] = true.ToString();
                ConvertPaintBucketsToGenericTools(Game1.player);
            }
            if (!Game1.player.modData.ContainsKey(TYPE_FIX_COMPATIBILITY))
            {
                Monitor.Log($"Fixing bad object and bigcraftable typings...", LogLevel.Debug);
                Game1.player.modData[TYPE_FIX_COMPATIBILITY] = true.ToString();
                FixBadObjectTyping();
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Load all available textures to account for any Content Patcher's OnWarped updates
            UpdateTextures();
        }

        private void UpdateTextures()
        {
            foreach (var texture in textureManager.GetAllTextures().Where(t => t.EnableContentPatcherCheck))
            {
                var loadedTexture = Helper.Content.Load<Texture2D>($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{texture.GetTokenId()}", ContentSource.GameContent);
                textureManager.UpdateTexture(texture.GetId(), loadedTexture);
            }

            foreach (var texture in textureManager.GetAllTextures().Where(t => t.EnableContentPatcherCheck))
            {
                var loadedTexture = Helper.Content.Load<Texture2D>($"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{texture.GetTokenId()}", ContentSource.GameContent);
                textureManager.UpdateTexture(texture.GetId(), loadedTexture);
            }

            foreach (var tool in assetManager.toolNames.ToList())
            {
                var loadedTexture = Helper.Content.Load<Texture2D>($"{AlternativeTextures.TOOL_TOKEN_HEADER}{tool.Key}", ContentSource.GameContent);
                assetManager.toolNames[tool.Key] = loadedTexture;
            }
        }

        private void LoadContentPacks()
        {
            // Load owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading textures from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", LogLevel.Debug);

                try
                {
                    var textureFolders = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Textures")).GetDirectories("*", SearchOption.AllDirectories);
                    if (textureFolders.Count() == 0)
                    {
                        Monitor.Log($"No sub-folders found under Textures for the content pack {contentPack.Manifest.Name}!", LogLevel.Warn);
                        continue;
                    }

                    // Load in the alternative textures
                    foreach (var textureFolder in textureFolders)
                    {
                        if (!File.Exists(Path.Combine(textureFolder.FullName, "texture.json")))
                        {
                            if (textureFolder.GetDirectories().Count() == 0)
                            {
                                Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a texture.json under {textureFolder.Name}!", LogLevel.Warn);
                            }

                            continue;
                        }

                        var parentFolderName = textureFolder.Parent.FullName.Replace(contentPack.DirectoryPath + Path.DirectorySeparatorChar, String.Empty);
                        var modelPath = Path.Combine(parentFolderName, textureFolder.Name, "texture.json");
                        var seasons = contentPack.ReadJsonFile<AlternativeTextureModel>(modelPath).Seasons;
                        for (int s = 0; s < 4; s++)
                        {
                            if ((seasons.Count() == 0 && s > 0) || (seasons.Count() > 0 && s >= seasons.Count()))
                            {
                                continue;
                            }

                            // Parse the model and assign it the content pack's owner
                            AlternativeTextureModel textureModel = contentPack.ReadJsonFile<AlternativeTextureModel>(modelPath);
                            textureModel.Owner = contentPack.Manifest.UniqueID;
                            textureModel.Type = textureModel.GetTextureType();

                            // Override Grass Alternative Texture pack ItemNames to always be Grass, in order to be compatible with translations 
                            textureModel.ItemName = textureModel.Type == "Grass" ? "Grass" : textureModel.ItemName;
                            if (String.IsNullOrEmpty(textureModel.ItemName))
                            {
                                Monitor.Log($"Unable to add alternative texture for {textureModel.Owner}: Missing the ItemName property!", LogLevel.Warn);
                                continue;
                            }

                            // Add the UniqueId to the top-level Keywords
                            textureModel.Keywords.Add(contentPack.Manifest.UniqueID);

                            // Add the top-level Keywords to any ManualVariations.Keywords
                            foreach (var variation in textureModel.ManualVariations)
                            {
                                variation.Keywords.AddRange(textureModel.Keywords);
                            }

                            // Set the season (if any)
                            textureModel.Season = seasons.Count() == 0 ? String.Empty : seasons[s];

                            // Set the ModelName and TextureId
                            textureModel.ModelName = String.IsNullOrEmpty(textureModel.Season) ? String.Concat(textureModel.GetTextureType(), "_", textureModel.ItemName) : String.Concat(textureModel.GetTextureType(), "_", textureModel.ItemName, "_", textureModel.Season);
                            textureModel.TextureId = String.Concat(textureModel.Owner, ".", textureModel.ModelName);

                            // Verify we are given a texture and if so, track it
                            if (!File.Exists(Path.Combine(textureFolder.FullName, "texture.png")))
                            {
                                // No texture.png found, may be using split texture files (texture_1.png, texture_2.png, etc.)
                                var textureFilePaths = Directory.GetFiles(textureFolder.FullName, "texture_*.png")
                                    .Select(t => Path.GetFileName(t))
                                    .Where(t => t.Any(char.IsDigit))
                                    .OrderBy(t => Int32.Parse(Regex.Match(t, @"\d+").Value));

                                if (textureFilePaths.Count() == 0)
                                {
                                    Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: No associated texture.png or split textures (texture_1.png, texture_2.png, etc.) given", LogLevel.Warn);
                                    continue;
                                }
                                if (textureModel.GetVariations() > textureFilePaths.Count())
                                {
                                    Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: There are less split texture files compared to variations ({textureFilePaths.Count()} file(s) vs {textureModel.GetVariations()} variation(s))", LogLevel.Warn);
                                    continue;
                                }
                                if (textureModel.GetVariations() < textureFilePaths.Count())
                                {
                                    Monitor.Log($"Warning for alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: There are less variations specified in texture.json than split textures files", LogLevel.Warn);
                                }

                                // Load in the first texture_#.png to get its dimensions for creating stitchedTexture
                                if (!StitchTexturesToModel(textureModel, contentPack, Path.Combine(parentFolderName, textureFolder.Name), textureFilePaths.Take(textureModel.GetVariations())))
                                {
                                    continue;
                                }

                                textureModel.TileSheetPath = contentPack.GetActualAssetKey(Path.Combine(parentFolderName, textureFolder.Name, textureFilePaths.First()));
                            }
                            else
                            {
                                // Load in the single vertical texture
                                textureModel.TileSheetPath = contentPack.GetActualAssetKey(Path.Combine(parentFolderName, textureFolder.Name, "texture.png"));
                                Texture2D singularTexture = contentPack.LoadAsset<Texture2D>(textureModel.TileSheetPath);
                                if (singularTexture.Height >= AlternativeTextureModel.MAX_TEXTURE_HEIGHT)
                                {
                                    Monitor.Log($"Unable to add alternative texture for {textureModel.Owner}: The texture {textureModel.TextureId} has a height larger than 16384!\nPlease split it into individual textures (e.g. texture_0.png, texture_1.png, etc.) to resolve this issue.", LogLevel.Warn);
                                    continue;
                                }
                                else
                                {
                                    textureModel.Textures.Add(singularTexture);
                                }
                            }

                            // Track the texture model
                            textureManager.AddAlternativeTexture(textureModel);

                            // Log it
                            Monitor.Log(textureModel.ToString(), LogLevel.Trace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error loading content pack {contentPack.Manifest.Name}: {ex}", LogLevel.Error);
                }
            }
        }

        private bool StitchTexturesToModel(AlternativeTextureModel textureModel, IContentPack contentPack, string rootPath, IEnumerable<string> textureFilePaths)
        {
            int maxVariationsPerTexture = AlternativeTextureModel.MAX_TEXTURE_HEIGHT / textureModel.TextureHeight;
            Texture2D baseTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(rootPath, textureFilePaths.First()));

            // If there is only one split texture file, skip the rest of the logic to avoid issues
            if (textureFilePaths.Count() == 1 || textureModel.GetVariations() == 1)
            {
                if (textureModel.GetVariations() == 1 && textureFilePaths.Count() > 1)
                {
                    Monitor.Log($"Detected more split textures ({textureFilePaths.Count()}) than specified variations ({textureModel.GetVariations()}) for {textureModel.TextureId} from {contentPack.Manifest.Name}", LogLevel.Warn);
                }

                textureModel.Textures.Add(baseTexture);
                return true;
            }

            try
            {
                for (int t = 0; t <= (textureModel.GetVariations() * textureModel.TextureHeight) / AlternativeTextureModel.MAX_TEXTURE_HEIGHT; t++)
                {
                    int variationLimit = Math.Min(maxVariationsPerTexture, textureModel.GetVariations() - (maxVariationsPerTexture * t));
                    if (variationLimit < 0)
                    {
                        variationLimit = 0;
                    }
                    Texture2D stitchedTexture = new Texture2D(Game1.graphics.GraphicsDevice, baseTexture.Width, Math.Min(textureModel.TextureHeight * variationLimit, AlternativeTextureModel.MAX_TEXTURE_HEIGHT));

                    // Now stitch together the split textures into a single texture
                    Color[] pixels = new Color[stitchedTexture.Width * stitchedTexture.Height];
                    for (int x = 0; x < variationLimit; x++)
                    {
                        int textureIndex = x + (maxVariationsPerTexture * t);
                        if (textureFilePaths.ElementAtOrDefault(textureIndex) is null)
                        {
                            Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: Attempted to add variation {textureIndex} from split texture, but the texture image doesn't exist!", LogLevel.Warn);
                            return false;
                        }

                        var fileName = textureFilePaths.ElementAt(textureIndex);
                        Monitor.Log($"Stitching together {textureModel.TextureId}: {fileName}", LogLevel.Trace);

                        var offset = x * baseTexture.Width * baseTexture.Height;
                        var subTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(rootPath, fileName));

                        try
                        {
                            Color[] subPixels = new Color[subTexture.Width * subTexture.Height];
                            subTexture.GetData(subPixels);
                            for (int i = 0; i < subPixels.Length; i++)
                            {
                                pixels[i + offset] = subPixels[i];
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: Failed to add variation from split texture {fileName}, it has a different image size [{subTexture.Width}x{subTexture.Height}] compared to the first texture [{baseTexture.Width}x{baseTexture.Height}]!", LogLevel.Warn);
                            return false;
                        }
                    }

                    stitchedTexture.SetData(pixels);
                    textureModel.Textures.Add(stitchedTexture);
                }
            }
            catch (Exception exception)
            {
                Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: Unhandled framework error: {exception}", LogLevel.Warn);
                return false;
            }

            return true;
        }

        private void DebugSpawnGiantCrop(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log($"Missing required arguments: [HARVEST_ID]", LogLevel.Warn);
                return;
            }

            if (!(Game1.currentLocation is Farm))
            {
                Monitor.Log($"Command can only be used on player's farm.", LogLevel.Warn);
                return;
            }

            var environment = Game1.currentLocation;
            foreach (var tile in environment.terrainFeatures.Pairs.Where(t => t.Value is HoeDirt))
            {
                int xTile = 0;
                int yTile = 0;
                var hoeDirt = tile.Value as HoeDirt;

                if (hoeDirt.crop is null || hoeDirt.crop.indexOfHarvest != int.Parse(args[0]))
                {
                    continue;
                }

                xTile = (int)tile.Key.X;
                yTile = (int)tile.Key.Y;

                if ((int.Parse(args[0]) == 276 || int.Parse(args[0]) == 190 || int.Parse(args[0]) == 254) && xTile != 0 && yTile != 0)
                {
                    for (int x = xTile - 1; x <= xTile + 1; x++)
                    {
                        for (int y2 = yTile - 1; y2 <= yTile + 1; y2++)
                        {
                            Vector2 v3 = new Vector2(x, y2);
                            if (!environment.terrainFeatures.ContainsKey(v3) || !(environment.terrainFeatures[v3] is HoeDirt) || (environment.terrainFeatures[v3] as HoeDirt).crop == null)
                            {
                                continue;
                            }

                            (environment.terrainFeatures[v3] as HoeDirt).crop = null;
                        }
                    }

                    (environment as Farm).resourceClumps.Add(new GiantCrop(int.Parse(args[0]), new Vector2(xTile - 1, yTile - 1)));
                }
            }
        }

        private void DebugSpawnResourceClump(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log($"Missing required arguments: [RESOURCE_NAME]", LogLevel.Warn);
                return;
            }

            if (!(Game1.currentLocation is Farm))
            {
                Monitor.Log($"Command can only be used on player's farm.", LogLevel.Warn);
                return;
            }

            if (args[0].ToLower() != "stump")
            {
                Monitor.Log($"That resource isn't supported.", LogLevel.Warn);
                return;
            }

            (Game1.currentLocation as Farm).resourceClumps.Add(new ResourceClump(600, 2, 2, Game1.player.getTileLocation() + new Vector2(1, 1)));
        }

        private void DebugSpawnChild(string command, string[] args)
        {
            if (args.Length < 2)
            {
                Monitor.Log($"Missing required arguments: [AGE] [IS_MALE] [SKIN_TONE]", LogLevel.Warn);
                return;
            }

            var age = -1;
            if (!int.TryParse(args[0], out age) || age < 0)
            {
                Monitor.Log($"Invalid number given: {args[0]}", LogLevel.Warn);
                return;
            }

            var isMale = false;
            if (args[1].ToLower() == "true")
            {
                isMale = true;
            }

            var hasDarkSkin = false;
            if (args[2].ToLower() == "dark")
            {
                hasDarkSkin = true;
            }

            var child = new Child("Test", isMale, hasDarkSkin, Game1.player);
            child.Position = Game1.player.Position;
            child.Age = age;
            Game1.currentLocation.characters.Add(child);
        }

        private void DebugSetAge(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log($"Missing required arguments: [AGE]", LogLevel.Warn);
                return;
            }

            var age = -1;
            if (!int.TryParse(args[0], out age))
            {
                Monitor.Log($"Invalid number given: {args[0]}", LogLevel.Warn);
                return;
            }

            foreach (var child in Game1.currentLocation.characters.Where(c => c is Child))
            {
                child.Age = 3;
            }
        }

        private void DebugShowPaintShop(string command, string[] args)
        {
            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
        }

        private string CleanContentPackNameForConfig(string contentPackName)
        {
            return contentPackName.Replace("[", String.Empty).Replace("]", String.Empty);
        }

        private void ConvertPaintBucketsToGenericTools(Farmer who)
        {
            // Check player's inventory first
            for (int i = 0; i < (int)who.maxItems; i++)
            {
                if (who.items[i] is MilkPail milkPail && milkPail.modData.ContainsKey(OLD_PAINT_BUCKET_FLAG))
                {
                    who.items[i] = PatchTemplate.GetPaintBucketTool();
                }
            }

            foreach (var location in Game1.locations)
            {
                ConvertStoredPaintBucketsToGenericTools(who, location);

                if (location is BuildableGameLocation)
                {
                    foreach (var building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        ConvertStoredPaintBucketsToGenericTools(who, indoorLocation);
                    }
                }
            }
        }

        private void ConvertStoredPaintBucketsToGenericTools(Farmer who, GameLocation location)
        {
            foreach (var chest in location.Objects.Pairs.Where(p => p.Value is Chest).Select(p => p.Value as Chest).ToList())
            {
                if (chest.isEmpty())
                {
                    continue;
                }

                if (chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                {
                    NetObjectList<Item> actual_items = chest.GetItemsForPlayer(who.UniqueMultiplayerID);
                    for (int j = actual_items.Count - 1; j >= 0; j--)
                    {
                        if (actual_items[j] is MilkPail milkPail && milkPail.modData.ContainsKey(OLD_PAINT_BUCKET_FLAG))
                        {
                            actual_items[j] = PatchTemplate.GetPaintBucketTool();
                        }
                    }
                }
                else
                {
                    for (int i = chest.items.Count - 1; i >= 0; i--)
                    {
                        if (chest.items[i] is MilkPail milkPail && milkPail.modData.ContainsKey(OLD_PAINT_BUCKET_FLAG))
                        {
                            chest.items[i] = PatchTemplate.GetPaintBucketTool();
                        }
                    }
                }
            }
        }

        private void FixBadObjectTyping()
        {
            foreach (var location in Game1.locations)
            {
                ConvertBadTypedObjectToNormalType(location);

                if (location is BuildableGameLocation)
                {
                    foreach (var building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        ConvertBadTypedObjectToNormalType(indoorLocation);
                    }
                }
            }
        }

        private void ConvertBadTypedObjectToNormalType(GameLocation location)
        {
            foreach (var obj in location.objects.Values.Where(o => o.modData.ContainsKey("AlternativeTextureName")))
            {
                if (obj.Type == "Craftable" || obj.Type == "Unknown")
                {
                    if (obj.bigCraftable && Game1.bigCraftablesInformation.TryGetValue(obj.parentSheetIndex, out var bigObjectInfo))
                    {
                        string[] objectInfoArray = bigObjectInfo.Split('/');
                        string[] typeAndCategory = objectInfoArray[3].Split(' ');
                        obj.type.Value = typeAndCategory[0];

                        if (typeAndCategory.Length > 1)
                        {
                            obj.Category = Convert.ToInt32(typeAndCategory[1]);
                        }
                    }
                    else if (!obj.bigCraftable && Game1.objectInformation.TryGetValue(obj.parentSheetIndex, out var objectInfo))
                    {
                        string[] objectInfoArray = objectInfo.Split('/');
                        string[] typeAndCategory = objectInfoArray[3].Split(' ');
                        obj.type.Value = typeAndCategory[0];
                        if (typeAndCategory.Length > 1)
                        {
                            obj.Category = Convert.ToInt32(typeAndCategory[1]);
                        }
                    }
                }
            }
        }
    }
}
