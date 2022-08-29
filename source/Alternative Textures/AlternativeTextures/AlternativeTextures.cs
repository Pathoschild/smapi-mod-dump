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
using StardewValley.GameData;
using Newtonsoft.Json;
using StardewValley.Buildings;
using StardewValley.Monsters;

namespace AlternativeTextures
{
    public class AlternativeTextures : Mod
    {
        // Core modData keys
        internal const string TEXTURE_TOKEN_HEADER = "AlternativeTextures/Textures/";
        internal const string TOOL_TOKEN_HEADER = "AlternativeTextures/Tools/";
        internal const string DEFAULT_OWNER = "Stardew.Default";
        internal const string ENABLED_SPRAY_CAN_TEXTURES = "Stardew.Default";

        // Compatibility keys
        internal const string TOOL_CONVERSION_COMPATIBILITY = "AlternativeTextures.HasConvertedMilkPails";
        internal const string TYPE_FIX_COMPATIBILITY = "AlternativeTextures.HasFixedBadObjectTyping";

        // Tool related keys
        internal const string PAINT_BUCKET_FLAG = "AlternativeTextures.PaintBucketFlag";
        internal const string OLD_PAINT_BUCKET_FLAG = "AlternativeTexturesPaintBucketFlag";
        internal const string PAINT_BRUSH_FLAG = "AlternativeTextures.PaintBrushFlag";
        internal const string PAINT_BRUSH_SCALE = "AlternativeTextures.PaintBrushScale";
        internal const string SCISSORS_FLAG = "AlternativeTextures.ScissorsFlag";
        internal const string SPRAY_CAN_FLAG = "AlternativeTextures.SprayCanFlag";
        internal const string SPRAY_CAN_RARE = "AlternativeTextures.SprayCanRare";
        internal const string SPRAY_CAN_RADIUS = "AlternativeTextures.SprayCanRadius";

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

        // Tool related variables
        private Point _lastSprayCanTile = new Point();

        // Debugging flags
        private bool _displayFPS = false;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Setup our managers
            textureManager = new TextureManager(monitor, helper);
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(helper, textureManager);

            // Setup our utilities
            fpsCounter = new FpsCounter();

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
            helper.ConsoleCommands.Add("at_spawn_monsters", "Spawns monster(s) of a specified type and quantity at the current location.\n\nUsage: at_spawn_monsters [MONSTER_ID] (QUANTITY)", this.DebugSpawnMonsters);
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

            // Hook into Input events
            helper.Events.Input.ButtonsChanged += OnButtonChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // Hook into Display events
            helper.Events.Display.Rendered += OnDisplayRendered;

            // Hook into the Content events
            helper.Events.Content.AssetRequested += OnContentAssetRequested;
            helper.Events.Content.AssetsInvalidated += OnContentInvalidated;
        }

        private void OnContentInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var asset in e.Names)
            {
                if (assetManager.toolNames.ContainsKey(asset.Name))
                {
                    assetManager.toolNames[asset.Name] = Helper.GameContent.Load<Texture2D>(asset);
                }
                else if (AlternativeTextures.textureManager.GetTextureByToken(asset.Name) is Texture2D texture && texture is not null)
                {
                    var loadedTexture = Helper.GameContent.Load<Texture2D>(asset.Name);

                    textureManager.UpdateTexture(asset.Name, loadedTexture);
                }
            }
        }

        private void OnContentAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.DataType == typeof(Texture2D))
            {
                var asset = e.Name;
                if (textureManager.GetModelByToken(asset.Name) is TokenModel tokenModel && tokenModel is not null)
                {
                    e.LoadFrom(() => tokenModel.AlternativeTexture.GetTexture(tokenModel.Variation), AssetLoadPriority.Exclusive);
                }
                else if (assetManager.toolNames.ContainsKey(asset.Name))
                {
                    e.LoadFrom(() => assetManager.toolNames[asset.Name], AssetLoadPriority.Exclusive);
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/AdditionalWallpaperFlooring") && textureManager.GetValidTextureNamesWithSeason().Count > 0)
            {
                e.Edit(asset =>
                {
                    List<ModWallpaperOrFlooring> moddedDecorations = asset.GetData<List<ModWallpaperOrFlooring>>();

                    foreach (var textureModel in textureManager.GetAllTextures().Where(t => t.IsDecoration() && !moddedDecorations.Any(d => d.ID == t.GetId())))
                    {
                        var decoration = new ModWallpaperOrFlooring()
                        {
                            ID = textureModel.GetId(),
                            Texture = $"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{textureModel.GetTokenId()}",
                            IsFlooring = String.Equals(textureModel.ItemName, "Floor", StringComparison.OrdinalIgnoreCase),
                            Count = textureModel.GetVariations()
                        };

                        moddedDecorations.Add(decoration);
                    }
                });
            }
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
            if (Game1.activeClickableMenu is null && Game1.player.CurrentTool is GenericTool tool && e.Button is SButton.MouseRight)
            {
                var xTile = (int)e.Cursor.Tile.X * 64;
                var yTile = (int)e.Cursor.Tile.Y * 64;

                if (tool.modData.ContainsKey(PAINT_BRUSH_FLAG))
                {
                    Helper.Input.Suppress(e.Button);

                    RightClickPaintBrush(tool, xTile, yTile);
                }
                else if (tool.modData.ContainsKey(SPRAY_CAN_FLAG))
                {
                    Helper.Input.Suppress(e.Button);

                    if (RightClickSprayCan(tool, xTile, yTile))
                    {
                        ToolPatch.UsePaintBucket(Game1.player.currentLocation, xTile, yTile, Game1.player, true);
                    }
                }
            }
        }

        private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is null && Game1.player.CurrentTool is GenericTool tool && e.Held.Contains(SButton.MouseLeft))
            {
                var xTile = (int)e.Cursor.Tile.X * 64;
                var yTile = (int)e.Cursor.Tile.Y * 64;

                if (tool.modData.ContainsKey(PAINT_BRUSH_FLAG))
                {
                    LeftClickPaintBrush(tool, xTile, yTile);
                }
                else if (tool.modData.ContainsKey(SPRAY_CAN_FLAG))
                {
                    LeftClickSprayCan(tool, xTile, yTile);
                }
            }
        }

        private void RightClickPaintBrush(GenericTool tool, int xTile, int yTile)
        {
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
                else if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop is not null)
                {
                    var modelType = AlternativeTextureModel.TextureType.Crop;
                    var instanceName = Game1.objectInformation.ContainsKey(hoeDirt.crop.netSeedIndex.Value) ? Game1.objectInformation[hoeDirt.crop.netSeedIndex.Value].Split('/')[0] : String.Empty;
                    if (!hoeDirt.modData.ContainsKey("AlternativeTextureName") || !hoeDirt.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{modelType}_{instanceName}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(hoeDirt, instanceSeasonName, true);
                    }

                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.texture_copied"), 2) { timeLeft = 1000 });
                    tool.modData[PAINT_BRUSH_FLAG] = $"{modelType}_{instanceName}";
                    tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                    tool.modData["AlternativeTextureOwner"] = hoeDirt.modData["AlternativeTextureOwner"];
                    tool.modData["AlternativeTextureName"] = hoeDirt.modData["AlternativeTextureName"];
                    tool.modData["AlternativeTextureVariation"] = hoeDirt.modData["AlternativeTextureVariation"];
                }
                else if (terrainFeature is Grass grass)
                {
                    var modelType = AlternativeTextureModel.TextureType.Grass;
                    if (!grass.modData.ContainsKey("AlternativeTextureName") || !grass.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{modelType}_Grass_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(grass, instanceSeasonName, true);
                    }

                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.texture_copied"), 2) { timeLeft = 1000 });
                    tool.modData[PAINT_BRUSH_FLAG] = $"{modelType}_Grass";
                    tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                    tool.modData["AlternativeTextureOwner"] = grass.modData["AlternativeTextureOwner"];
                    tool.modData["AlternativeTextureName"] = grass.modData["AlternativeTextureName"];
                    tool.modData["AlternativeTextureVariation"] = grass.modData["AlternativeTextureVariation"];
                }
                else if (terrainFeature is Bush bush)
                {
                    var modelType = AlternativeTextureModel.TextureType.Bush;
                    if (!bush.modData.ContainsKey("AlternativeTextureName") || !bush.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{modelType}_{PatchTemplate.GetBushTypeString(bush)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(bush, instanceSeasonName, true);
                    }

                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.info.texture_copied"), 2) { timeLeft = 1000 });
                    tool.modData[PAINT_BRUSH_FLAG] = $"{modelType}_{PatchTemplate.GetBushTypeString(bush)}";
                    tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                    tool.modData["AlternativeTextureOwner"] = bush.modData["AlternativeTextureOwner"];
                    tool.modData["AlternativeTextureName"] = bush.modData["AlternativeTextureName"];
                    tool.modData["AlternativeTextureVariation"] = bush.modData["AlternativeTextureVariation"];
                }
                else
                {
                    tool.modData[PAINT_BRUSH_FLAG] = String.Empty;
                    tool.modData[PAINT_BRUSH_SCALE] = 0.5f.ToString();
                    if (terrainFeature != null)
                    {
                        Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.brush_not_supported"), 3) { timeLeft = 2000 });
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

        private void LeftClickPaintBrush(GenericTool tool, int xTile, int yTile)
        {
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
                    else if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop is not null)
                    {
                        var modelType = AlternativeTextureModel.TextureType.Crop;
                        var instanceName = Game1.objectInformation.ContainsKey(hoeDirt.crop.netSeedIndex.Value) ? Game1.objectInformation[hoeDirt.crop.netSeedIndex.Value].Split('/')[0] : String.Empty;
                        if (tool.modData[PAINT_BRUSH_FLAG] == $"{modelType}_{instanceName}")
                        {
                            hoeDirt.modData["AlternativeTextureOwner"] = tool.modData["AlternativeTextureOwner"];
                            hoeDirt.modData["AlternativeTextureName"] = tool.modData["AlternativeTextureName"];
                            hoeDirt.modData["AlternativeTextureVariation"] = tool.modData["AlternativeTextureVariation"];
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[PAINT_BRUSH_FLAG] }), 3) { timeLeft = 2000 });
                        }
                    }
                    else if (terrainFeature is Grass grass)
                    {
                        var modelType = AlternativeTextureModel.TextureType.Grass;
                        if (tool.modData[PAINT_BRUSH_FLAG] == $"{modelType}_Grass")
                        {
                            grass.modData["AlternativeTextureOwner"] = tool.modData["AlternativeTextureOwner"];
                            grass.modData["AlternativeTextureName"] = tool.modData["AlternativeTextureName"];
                            grass.modData["AlternativeTextureVariation"] = tool.modData["AlternativeTextureVariation"];
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[PAINT_BRUSH_FLAG] }), 3) { timeLeft = 2000 });
                        }
                    }
                    else if (terrainFeature is Bush bush)
                    {
                        var modelType = AlternativeTextureModel.TextureType.Bush;
                        if (tool.modData[PAINT_BRUSH_FLAG] == $"{modelType}_{PatchTemplate.GetBushTypeString(bush)}")
                        {
                            bush.modData["AlternativeTextureOwner"] = tool.modData["AlternativeTextureOwner"];
                            bush.modData["AlternativeTextureName"] = tool.modData["AlternativeTextureName"];
                            bush.modData["AlternativeTextureVariation"] = tool.modData["AlternativeTextureVariation"];
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

        private bool RightClickSprayCan(GenericTool tool, int xTile, int yTile)
        {
            // Verify that a supported object exists at the tile
            var cachedFlag = String.Empty;
            if (tool.modData.ContainsKey(SPRAY_CAN_FLAG))
            {
                cachedFlag = tool.modData[SPRAY_CAN_FLAG];
            }

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

                    tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_{PatchTemplate.GetFlooringName(flooring)}";
                }
                else if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop is not null)
                {
                    var modelType = AlternativeTextureModel.TextureType.Crop;
                    var instanceName = Game1.objectInformation.ContainsKey(hoeDirt.crop.netSeedIndex.Value) ? Game1.objectInformation[hoeDirt.crop.netSeedIndex.Value].Split('/')[0] : String.Empty;
                    if (!hoeDirt.modData.ContainsKey("AlternativeTextureName") || !hoeDirt.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{modelType}_{instanceName}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(hoeDirt, instanceSeasonName, true);
                    }

                    tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_{instanceName}";
                }
                else if (terrainFeature is Grass grass)
                {
                    var modelType = AlternativeTextureModel.TextureType.Grass;
                    if (!grass.modData.ContainsKey("AlternativeTextureName") || !grass.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{modelType}_Grass_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(grass, instanceSeasonName, true);
                    }

                    tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_Grass";
                }
                else if (terrainFeature is Tree tree)
                {
                    var modelType = AlternativeTextureModel.TextureType.Tree;
                    if (!tree.modData.ContainsKey("AlternativeTextureName") || !tree.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Tree}_{PatchTemplate.GetTreeTypeString(tree)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(tree, instanceSeasonName, true);
                    }

                    tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_{PatchTemplate.GetTreeTypeString(tree)}";
                }
                else if (terrainFeature is FruitTree fruitTree)
                {
                    var modelType = AlternativeTextureModel.TextureType.FruitTree;
                    Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
                    var saplingIndex = data.FirstOrDefault(d => int.Parse(d.Value.Split('/')[0]) == fruitTree.treeType).Key;
                    var saplingName = Game1.objectInformation.ContainsKey(saplingIndex) ? Game1.objectInformation[saplingIndex].Split('/')[0] : String.Empty;
                    if (!fruitTree.modData.ContainsKey("AlternativeTextureName") || !fruitTree.modData.ContainsKey("AlternativeTextureVariation"))
                    {
                        // Assign default modData
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.FruitTree}_{saplingName}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        PatchTemplate.AssignDefaultModData(fruitTree, instanceSeasonName, true);
                    }

                    tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_{saplingName}";
                }
                else
                {
                    if (Game1.currentLocation is Farm farm)
                    {
                        var targetedBuilding = farm.getBuildingAt(new Vector2(xTile / 64, yTile / 64));
                        if (targetedBuilding != null)
                        {
                            Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.spray_can_not_supported"), 3) { timeLeft = 2000 });
                            return false;
                        }
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.spray_can_not_supported"), 3) { timeLeft = 2000 });
                        return false;
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

                tool.modData[SPRAY_CAN_FLAG] = $"{modelType}_{PatchTemplate.GetObjectName(placedObject)}";
            }

            if (cachedFlag != tool.modData[SPRAY_CAN_FLAG])
            {
                Game1.player.modData[ENABLED_SPRAY_CAN_TEXTURES] = null;
            }

            return true;
        }

        private void LeftClickSprayCan(GenericTool tool, int xTile, int yTile)
        {
            if (_lastSprayCanTile.X == xTile && _lastSprayCanTile.Y == yTile)
            {
                return;
            }
            _lastSprayCanTile = new Point(xTile, yTile);

            if (Game1.player.modData.ContainsKey(ENABLED_SPRAY_CAN_TEXTURES) is false || String.IsNullOrEmpty(Game1.player.modData[ENABLED_SPRAY_CAN_TEXTURES]))
            {
                Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.spray_can_is_empty"), 3) { timeLeft = 2000 });
            }
            else
            {
                var selectedModelsToVariations = JsonConvert.DeserializeObject<Dictionary<string, SelectedTextureModel>>(Game1.player.modData[ENABLED_SPRAY_CAN_TEXTURES]);

                if (selectedModelsToVariations.Count == 0)
                {
                    Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.spray_can_is_empty"), 3) { timeLeft = 2000 });
                    return;
                }

                int tileRadius = 1;
                if (Game1.player.modData.ContainsKey(SPRAY_CAN_RADIUS) is false || int.TryParse(Game1.player.modData[SPRAY_CAN_RADIUS], out tileRadius) is false)
                {
                    Game1.player.modData[SPRAY_CAN_RADIUS] = "1";
                }
                tileRadius = tileRadius > 0 ? tileRadius - 1 : tileRadius;

                // Convert to standard game tiles
                xTile /= 64;
                yTile /= 64;
                for (int x = xTile - tileRadius; x <= xTile + tileRadius; x++)
                {
                    for (int y = yTile - tileRadius; y <= yTile + tileRadius; y++)
                    {
                        var actualX = x * 64;
                        var actualY = y * 64;

                        // Select random texture
                        Random random = new Random(Guid.NewGuid().GetHashCode());
                        var selectedModelIndex = random.Next(0, selectedModelsToVariations.Count);
                        var actualSelectedModel = selectedModelsToVariations.ElementAt(selectedModelIndex).Value;
                        var selectedVariationIndex = random.Next(0, actualSelectedModel.Variations.Count);
                        var actualSelectedVariation = actualSelectedModel.Variations[selectedVariationIndex].ToString();

                        // Verify that a supported object exists at the tile
                        var terrainFeature = PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, actualX, actualY);
                        if (terrainFeature is Flooring flooring)
                        {
                            var modelType = AlternativeTextureModel.TextureType.Flooring;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_{PatchTemplate.GetFlooringName(flooring)}")
                            {
                                flooring.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                flooring.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                flooring.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
                        }
                        if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop is not null)
                        {
                            var modelType = AlternativeTextureModel.TextureType.Crop;
                            var instanceName = Game1.objectInformation.ContainsKey(hoeDirt.crop.netSeedIndex.Value) ? Game1.objectInformation[hoeDirt.crop.netSeedIndex.Value].Split('/')[0] : String.Empty;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_{instanceName}")
                            {
                                hoeDirt.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                hoeDirt.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                hoeDirt.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
                        }
                        if (terrainFeature is Grass grass)
                        {
                            var modelType = AlternativeTextureModel.TextureType.Grass;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_Grass")
                            {
                                grass.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                grass.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                grass.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
                        }
                        if (terrainFeature is Tree tree)
                        {
                            var modelType = AlternativeTextureModel.TextureType.Tree;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_{PatchTemplate.GetTreeTypeString(tree)}")
                            {
                                tree.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                tree.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                tree.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
                            else
                            {
                                Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[SPRAY_CAN_FLAG] }), 3) { timeLeft = 2000 });
                            }
                        }
                        if (terrainFeature is FruitTree fruitTree)
                        {
                            var modelType = AlternativeTextureModel.TextureType.FruitTree;
                            Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
                            var saplingIndex = data.FirstOrDefault(d => int.Parse(d.Value.Split('/')[0]) == fruitTree.treeType).Key;
                            var saplingName = Game1.objectInformation.ContainsKey(saplingIndex) ? Game1.objectInformation[saplingIndex].Split('/')[0] : String.Empty;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_{saplingName}")
                            {
                                fruitTree.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                fruitTree.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                fruitTree.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
                            else
                            {
                                Game1.addHUDMessage(new HUDMessage(modHelper.Translation.Get("messages.warning.invalid_copied_texture", new { textureName = tool.modData[SPRAY_CAN_FLAG] }), 3) { timeLeft = 2000 });
                            }
                        }

                        var placedObject = PatchTemplate.GetObjectAt(Game1.currentLocation, actualX, actualY);
                        if (placedObject is not null)
                        {
                            var modelType = placedObject is Furniture ? AlternativeTextureModel.TextureType.Furniture : AlternativeTextureModel.TextureType.Craftable;
                            if (tool.modData[SPRAY_CAN_FLAG] == $"{modelType}_{PatchTemplate.GetObjectName(placedObject)}")
                            {
                                placedObject.modData["AlternativeTextureOwner"] = actualSelectedModel.Owner;
                                placedObject.modData["AlternativeTextureName"] = actualSelectedModel.TextureName;
                                placedObject.modData["AlternativeTextureVariation"] = actualSelectedVariation;
                                continue;
                            }
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
            // Set our default configuration file
            modConfig = Helper.ReadConfig<ModConfig>();

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

            Monitor.Log($"Finished loading Alternative Textures content packs", LogLevel.Debug);

            // Register tools
            foreach (var tool in assetManager.toolNames.ToList())
            {
                var loadedTexture = Helper.GameContent.Load<Texture2D>(tool.Key);
                assetManager.toolNames[tool.Key] = loadedTexture;
            }

            // Hook into GMCM, if applicable
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGenericModConfigMenu(Helper))
            {
                var configApi = apiManager.GetGenericModConfigMenuApi();
                configApi.Register(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));

                // Register the standard settings
                configApi.RegisterLabel(ModManifest, $"Use Random Textures When Placing...", String.Empty);
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingFlooring, value => modConfig.UseRandomTexturesWhenPlacingFlooring = value, () => "Flooring");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingFruitTree, value => modConfig.UseRandomTexturesWhenPlacingFruitTree = value, () => "Fruit Tree");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingTree, value => modConfig.UseRandomTexturesWhenPlacingTree = value, () => "Tree");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingHoeDirt, value => modConfig.UseRandomTexturesWhenPlacingHoeDirt = value, () => "Hoe Dirt");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingGrass, value => modConfig.UseRandomTexturesWhenPlacingGrass = value, () => "Grass");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingFurniture, value => modConfig.UseRandomTexturesWhenPlacingFurniture = value, () => "Furniture");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingObject, value => modConfig.UseRandomTexturesWhenPlacingObject = value, () => "Object");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingFarmAnimal, value => modConfig.UseRandomTexturesWhenPlacingFarmAnimal = value, () => "Farm Animal");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingMonster, value => modConfig.UseRandomTexturesWhenPlacingMonster = value, () => "Monster");
                configApi.AddBoolOption(ModManifest, () => modConfig.UseRandomTexturesWhenPlacingBuilding, value => modConfig.UseRandomTexturesWhenPlacingBuilding = value, () => "Building");

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
                            configApi.RegisterImage(ModManifest, $"{AlternativeTextures.TEXTURE_TOKEN_HEADER}{model.GetTokenId(variation)}", sourceRect, scale);

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
                                else if (textureModel.IsDecoration())
                                {
                                    Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: Split textures (texture_1.png, texture_2.png, etc.) are not allowed for Decoration types (wallpapers / floors)!", LogLevel.Warn);
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

                                textureModel.TileSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, textureFolder.Name, textureFilePaths.First())).Name;
                            }
                            else
                            {
                                // Load in the single vertical texture
                                textureModel.TileSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, textureFolder.Name, "texture.png")).Name;
                                Texture2D singularTexture = contentPack.ModContent.Load<Texture2D>(textureModel.TileSheetPath);
                                if (singularTexture.Height >= AlternativeTextureModel.MAX_TEXTURE_HEIGHT)
                                {
                                    Monitor.Log($"Unable to add alternative texture for {textureModel.Owner}: The texture {textureModel.TextureId} has a height larger than 16384!\nPlease split it into individual textures (e.g. texture_0.png, texture_1.png, etc.) to resolve this issue.", LogLevel.Warn);
                                    continue;
                                }
                                else if (textureModel.IsDecoration())
                                {
                                    if (singularTexture.Width < 256)
                                    {
                                        Monitor.Log($"Unable to add alternative texture for {textureModel.ItemName} from {contentPack.Manifest.Name}: The required image width is 256 for Decoration types (wallpapers / floors). Please correct the image's width manually.", LogLevel.Warn);
                                        continue;
                                    }

                                    textureModel.Textures[0] = singularTexture;
                                }
                                else if (!SplitVerticalTexturesToModel(textureModel, contentPack.Manifest.Name, singularTexture))
                                {
                                    continue;
                                }
                            }

                            // Track the texture model
                            textureManager.AddAlternativeTexture(textureModel);

                            // Log it
                            if (modConfig.OutputTextureDataToLog)
                            {
                                Monitor.Log(textureModel.ToString(), LogLevel.Trace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error loading content pack {contentPack.Manifest.Name}: {ex}", LogLevel.Error);
                }
            }
        }

        internal bool SplitVerticalTexturesToModel(AlternativeTextureModel textureModel, string contentPackName, Texture2D verticalTexture)
        {
            try
            {
                for (int v = 0; v < textureModel.GetVariations(); v++)
                {
                    var extractRectangle = new Rectangle(0, textureModel.TextureHeight * v, verticalTexture.Width, textureModel.TextureHeight);
                    Color[] extractPixels = new Color[extractRectangle.Width * extractRectangle.Height];

                    if (verticalTexture.Bounds.Contains(extractRectangle) is false)
                    {
                        int maxVariationsPossible = verticalTexture.Height / textureModel.TextureHeight;

                        Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPackName}: More variations specified ({textureModel.GetVariations()}) than given ({maxVariationsPossible})", LogLevel.Warn);
                        return false;
                    }

                    // Get the required pixels
                    verticalTexture.GetData(0, extractRectangle, extractPixels, 0, extractPixels.Length);

                    // Set the required pixels
                    var extractedTexture = new Texture2D(Game1.graphics.GraphicsDevice, extractRectangle.Width, extractRectangle.Height);
                    extractedTexture.SetData(extractPixels);

                    textureModel.Textures[v] = (extractedTexture);
                }
            }
            catch (Exception exception)
            {
                Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPackName}: Unhandled framework error: {exception}", LogLevel.Warn);
                return false;
            }

            return true;
        }

        private bool StitchTexturesToModel(AlternativeTextureModel textureModel, IContentPack contentPack, string rootPath, IEnumerable<string> textureFilePaths)
        {
            Texture2D baseTexture = contentPack.ModContent.Load<Texture2D>(Path.Combine(rootPath, textureFilePaths.First()));

            // If there is only one split texture file, skip the rest of the logic to avoid issues
            if (textureFilePaths.Count() == 1 || textureModel.GetVariations() == 1)
            {
                if (textureModel.GetVariations() == 1 && textureFilePaths.Count() > 1)
                {
                    Monitor.Log($"Detected more split textures ({textureFilePaths.Count()}) than specified variations ({textureModel.GetVariations()}) for {textureModel.TextureId} from {contentPack.Manifest.Name}", LogLevel.Warn);
                }

                textureModel.Textures[0] = baseTexture;
                return true;
            }

            try
            {
                int variation = 0;
                foreach (var textureFilePath in textureFilePaths)
                {
                    var splitTexture = contentPack.ModContent.Load<Texture2D>(Path.Combine(rootPath, textureFilePath));
                    textureModel.Textures[variation] = splitTexture;

                    variation++;
                }
            }
            catch (Exception exception)
            {
                Monitor.Log($"Unable to add alternative texture for item {textureModel.ItemName} from {contentPack.Manifest.Name}: Unhandled framework error: {exception}", LogLevel.Warn);
                return false;
            }

            return true;
        }

        private void DebugSpawnMonsters(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log($"Missing required arguments: [MONSTER_ID]", LogLevel.Warn);
                return;
            }

            int amountToSpawn = 1;
            if (args.Length > 1 && Int32.TryParse(args[1], out amountToSpawn) is false)
            {
                Monitor.Log($"Invalid count given for (QUANTITY)", LogLevel.Warn);
                return;
            }
            Type monsterType = Type.GetType("StardewValley.Monsters." + args[0] + ",Stardew Valley");

            Monitor.Log(Game1.player.getTileLocation().ToString(), LogLevel.Debug);
            for (int i = 0; i < amountToSpawn; i++)
            {
                var monster = Activator.CreateInstance(monsterType, new object[] { Game1.player.getTileLocation() }) as Monster;
                monster.Position = Game1.player.Position;
                Game1.currentLocation.characters.Add(monster);
            }
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
            var items = new Dictionary<ISalable, int[]>()
            {
                { PatchTemplate.GetPaintBucketTool(), new int[2] { 500, 1 } },
                { PatchTemplate.GetScissorsTool(), new int[2] { 500, 1 } },
                { PatchTemplate.GetPaintBrushTool(), new int[2] { 500, 1 } },
                { PatchTemplate.GetSprayCanTool(true), new int[2] { 500, 1 } }
            };
            Game1.activeClickableMenu = new ShopMenu(items);
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
