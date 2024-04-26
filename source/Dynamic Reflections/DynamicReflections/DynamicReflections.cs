/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using DynamicReflections.Framework.Models;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using DynamicReflections.Framework.Patches.SMAPI;
using DynamicReflections.Framework.Patches.Tiles;
using DynamicReflections.Framework.Patches.Tools;
using System.Linq;
using DynamicReflections.Framework.Patches.Objects;
using DynamicReflections.Framework.Utilities;
using DynamicReflections.Framework.Managers;
using DynamicReflections.Framework.Models.Settings;
using System.Text.Json;
using DynamicReflections.Framework.External.GenericModConfigMenu;
using StardewValley.Locations;
using StardewValley.Menus;
using DynamicReflections.Framework.Interfaces.Internal;

namespace DynamicReflections
{
    public class DynamicReflections : Mod
    {
        // Shared static helpers
        internal static Api api;
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static Multiplayer multiplayer;

        // Managers
        internal static ApiManager apiManager;
        internal static AssetManager assetManager;
        internal static MirrorsManager mirrorsManager;
        internal static PuddleManager puddleManager;
        internal static SkyManager skyManager;

        // Config options
        internal static ModConfig modConfig;
        internal static string[] activeLocationNames = new string[2];
        internal static WaterSettings currentWaterSettings = new WaterSettings();
        internal static PuddleSettings currentPuddleSettings = new PuddleSettings();
        internal static SkySettings currentSkySettings = new SkySettings();

        // Water reflection variables
        internal static Dictionary<NPC, Vector2> npcToWaterReflectionPosition = new Dictionary<NPC, Vector2>();
        internal static Vector2? waterReflectionPosition;
        internal static Vector2? waterReflectionTilePosition;
        internal static bool shouldDrawWaterReflection;
        internal static bool isDrawingWaterReflection;
        internal static bool isFilteringWater;
        internal static bool shouldSkipWaterOverlay;

        // Puddle reflection variables
        internal static bool shouldDrawPuddlesReflection;
        internal static bool isFilteringPuddles;
        internal static bool isDrawingPuddles;

        // Mirror reflection variables
        internal static FarmerSprite mirrorReflectionSprite;
        internal static Dictionary<Point, Mirror> mirrors = new Dictionary<Point, Mirror>();
        internal static List<Point> activeMirrorPositions = new List<Point>();
        internal static bool shouldDrawMirrorReflection;
        internal static bool isDrawingMirrorReflection;
        internal static bool isFilteringMirror;

        // Sky related reflection variables
        internal static float skyAlpha = 0.5f;
        internal static float waterAlpha = 0.5f;
        internal static bool isMeteorShower = false;
        internal static bool shouldDrawNightSky;
        internal static bool isFilteringSky;
        internal static bool isFilteringStar;

        // Effects and RenderTarget2Ds
        internal static Effect waterReflectionEffect;
        internal static Effect mirrorReflectionEffect;
        internal static RenderTarget2D nightSkyRenderTarget;
        internal static RenderTarget2D cloudRenderTarget;
        internal static RenderTarget2D playerWaterReflectionRender;
        internal static RenderTarget2D playerPuddleReflectionRender;
        internal static RenderTarget2D[] composedPlayerMirrorReflectionRenders;
        internal static RenderTarget2D[] maskedPlayerMirrorReflectionRenders;
        internal static RenderTarget2D npcWaterReflectionRender;
        internal static RenderTarget2D npcPuddleReflectionRender;
        internal static RenderTarget2D inBetweenRenderTarget;
        internal static RenderTarget2D mirrorsLayerRenderTarget;
        internal static RenderTarget2D mirrorsFurnitureRenderTarget;
        internal static RenderTarget2D puddlesRenderTarget;
        internal static RasterizerState rasterizer;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and multiplayer
            api = new Api();
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            modConfig = new ModConfig();

            // Load the managers
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(modHelper);
            mirrorsManager = new MirrorsManager();
            puddleManager = new PuddleManager();
            skyManager = new SkyManager();

            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply patches
                new LayerPatch(monitor, modHelper).Apply(harmony);
                new DisplayDevicePatch(monitor, modHelper).Apply(harmony);
                new ToolPatch(monitor, modHelper).Apply(harmony);
                new FishingRodPatch(monitor, modHelper).Apply(harmony);
                new FurniturePatch(monitor, modHelper).Apply(harmony);
                new GameLocationPatch(monitor, modHelper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in the debug commands
            helper.ConsoleCommands.Add("dr_reload", "Reloads all Dynamic Reflections content packs.\n\nUsage: dr_reload", delegate { this.LoadContentPacks(); this.DetectMirrorsForActiveLocation(); });

            // Hook into the required events
            helper.Events.Display.WindowResized += OnWindowResized;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.World.FurnitureListChanged += OnFurnitureListChanged;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        public override object GetApi()
        {
            return api;
        }

        private void OnWindowResized(object sender, StardewModdingAPI.Events.WindowResizedEventArgs e)
        {
            LoadRenderers();
        }

        private void OnButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady is false || Game1.activeClickableMenu is not null)
            {
                return;
            }

            if (e.Button == modConfig.QuickMenuKey && Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.GetGenericModConfigMenuApi() is not null)
            {
                apiManager.GetGenericModConfigMenuApi().OpenModMenu(ModManifest);
            }
        }

        private void OnFurnitureListChanged(object sender, StardewModdingAPI.Events.FurnitureListChangedEventArgs e)
        {
            if (e.IsCurrentLocation is false)
            {
                return;
            }

            // Attempt to add any DGA mirrors
            foreach (var furniture in e.Added)
            {
                if (DynamicReflections.mirrorsManager.GetSettings(furniture.Name) is MirrorSettings baseSettings && baseSettings is not null)
                {
                    var point = new Point((int)furniture.TileLocation.X, (int)furniture.TileLocation.Y);
                    var settings = new MirrorSettings()
                    {
                        Dimensions = new Rectangle(baseSettings.Dimensions.X, baseSettings.Dimensions.Y, baseSettings.Dimensions.Width, baseSettings.Dimensions.Height),
                        ReflectionOffset = baseSettings.ReflectionOffset,
                        ReflectionOverlay = baseSettings.ReflectionOverlay,
                        ReflectionScale = baseSettings.ReflectionScale
                    };

                    DynamicReflections.mirrors[point] = new Mirror()
                    {
                        FurnitureLink = furniture,
                        TilePosition = point,
                        Settings = settings
                    };
                }
            }

            // Attempt to remove any DGA mirrors
            foreach (var furniture in e.Removed)
            {
                if (DynamicReflections.mirrorsManager.GetSettings(furniture.Name) is MirrorSettings baseSettings && baseSettings is not null)
                {
                    var point = new Point((int)furniture.TileLocation.X, (int)furniture.TileLocation.Y);
                    foreach (var mirrorPosition in DynamicReflections.mirrors.Keys.ToList())
                    {
                        if (DynamicReflections.mirrors[mirrorPosition].FurnitureLink is not null && mirrorPosition.X == point.X && mirrorPosition.Y == point.Y)
                        {
                            DynamicReflections.mirrors.Remove(mirrorPosition);
                        }
                    }
                }
            }
        }

        private void OnWarped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            SetSkyReflectionSettings();
            SetPuddleReflectionSettings();
            SetWaterReflectionSettings();
            DetectMirrorsForActiveLocation();

            if (e.NewLocation is not null && e.NewLocation.IsOutdoors is true)
            {
                bool canRainHere = e.NewLocation.GetLocationContext().WeatherConditions.Any(w => w.Weather == "Rain" || w.Weather == "Storm");
                if (canRainHere is true)
                {
                    int puddlesPercentage = 0;
                    if (Game1.IsRainingHere(e.NewLocation) || (Game1.player.modData.ContainsKey(ModDataKeys.DID_RAIN_YESTERDAY) && Game1.player.modData[ModDataKeys.DID_RAIN_YESTERDAY] == "True"))
                    {
                        puddlesPercentage = Game1.IsRainingHere(e.NewLocation) ? currentPuddleSettings.PuddlePercentageWhileRaining : currentPuddleSettings.PuddlePercentageAfterRaining;
                        puddlesPercentage = Math.Max(1, (100 - puddlesPercentage));
                    }

                    DynamicReflections.puddleManager.Generate(e.NewLocation, percentOfDiggableTiles: puddlesPercentage);
                }

                DynamicReflections.skyManager.Generate(e.NewLocation);
            }
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady is false || Game1.currentLocation is null)
            {
                return;
            }

            // Handle updating the GMCM menu
            var waterSettings = modConfig.GetCurrentWaterSettings(Game1.currentLocation);
            GMCMHelper.IsLocationOverridingWaterDefault = waterSettings.OverrideDefaultSettings && waterSettings != DynamicReflections.modConfig.WaterReflectionSettings;

            var puddleSettings = modConfig.GetCurrentPuddleSettings(Game1.currentLocation);
            GMCMHelper.IsLocationOverridingPuddleDefault = puddleSettings.OverrideDefaultSettings && puddleSettings != DynamicReflections.modConfig.PuddleReflectionSettings;

            var skySettings = modConfig.GetCurrentSkySettings(Game1.currentLocation);
            GMCMHelper.IsLocationOverridingSkyDefault = skySettings.OverrideDefaultSettings && skySettings != DynamicReflections.modConfig.SkyReflectionSettings;

            if (Game1.activeClickableMenu is null)
            {
                GMCMHelper.RefreshLocationListing();
            }

            // Handle the sky reflections
            var targetDarkTime = Game1.getTrulyDarkTime(Game1.currentLocation) + 100;
            DynamicReflections.shouldDrawNightSky = false;
            if (modConfig.AreSkyReflectionsEnabled is not false && currentSkySettings is not null && currentSkySettings.AreReflectionsEnabled && Game1.currentLocation.IsOutdoors && Game1.IsRainingHere(Game1.currentLocation) is false && Game1.timeOfDay >= targetDarkTime)
            {
                DynamicReflections.shouldDrawNightSky = true;
                if (Game1.timeOfDay < targetDarkTime + 100) // Less then 10 PM
                {
                    DynamicReflections.waterAlpha = currentSkySettings.GettingDarkWaterAlpha;

                    DynamicReflections.skyAlpha = 1f - ((targetDarkTime + 100) - Game1.timeOfDay) * 0.005f;
                }
                else if (Game1.timeOfDay < targetDarkTime + 200) // Less then 11 PM
                {
                    DynamicReflections.waterAlpha = currentSkySettings.HalfwayDarkWaterAlpha;
                    DynamicReflections.skyAlpha = Math.Min(0.95f, 1f - ((targetDarkTime + 125) - Game1.timeOfDay) * 0.005f);
                }
                else
                {
                    DynamicReflections.waterAlpha = currentSkySettings.FinishedDarkWaterAlpha;
                    DynamicReflections.skyAlpha = 1f;
                }

                var secondsIntervalForShootingStar = Math.Max(1, (DynamicReflections.currentSkySettings.MillisecondsBetweenShootingStarAttempt / 1000f) * 60);
                var secondsIntervalForMeteorShower = Math.Max(1, (DynamicReflections.currentSkySettings.MillisecondsBetweenShootingStarAttemptDuringMeteorShower / 1000f) * 60);
                if (Game1.game1.IsActive && DynamicReflections.currentSkySettings.AreShootingStarsEnabled && e.IsMultipleOf((uint)(isMeteorShower ? secondsIntervalForMeteorShower : secondsIntervalForShootingStar)))
                {
                    var maxShootingStars = Math.Max(2, DynamicReflections.currentSkySettings.MaxShootingStarAttemptsPerInterval);
                    for (int i = 0; i < Game1.random.Next(1, maxShootingStars); i++)
                    {
                        DynamicReflections.skyManager.AttemptEffects(Game1.currentLocation);
                    }
                }
            }

            // Handle the puddle reflection
            DynamicReflections.shouldDrawPuddlesReflection = false;
            if (modConfig.ArePuddleReflectionsEnabled is not false && currentPuddleSettings is not null && currentPuddleSettings.AreReflectionsEnabled)
            {
                DynamicReflections.shouldDrawPuddlesReflection = true;
            }

            // Handle the water reflection
            DynamicReflections.shouldDrawWaterReflection = false;
            if (modConfig.AreWaterReflectionsEnabled is not false && currentWaterSettings is not null && currentWaterSettings.AreReflectionsEnabled)
            {
                var positionInverter = currentWaterSettings.ReflectionDirection == Direction.North && currentWaterSettings.PlayerReflectionOffset.Y > 0 ? -1 : 1;
                var playerPosition = Game1.player.Position;
                playerPosition += currentWaterSettings.PlayerReflectionOffset * 64 * positionInverter;
                DynamicReflections.waterReflectionPosition = playerPosition;
                DynamicReflections.waterReflectionTilePosition = playerPosition / 64f;

                // Hide the reflection if it will show up out of bounds on the map or not drawn on water tile
                var waterReflectionPosition = DynamicReflections.waterReflectionTilePosition.Value;
                for (int yOffset = -1; yOffset <= Math.Ceiling(currentWaterSettings.PlayerReflectionOffset.Y); yOffset++)
                {
                    var tilePosition = waterReflectionPosition + new Vector2(0, yOffset);
                    if (IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X - 1, (int)tilePosition.Y) is true || IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X, (int)tilePosition.Y) is true || IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X + 1, (int)tilePosition.Y) is true)
                    {
                        DynamicReflections.shouldDrawWaterReflection = true;
                        break;
                    }
                }

                // Handle the wavy effect if enabled
                if (currentWaterSettings.IsReflectionWavy && DynamicReflections.waterReflectionEffect is not null)
                {
                    var phase = waterReflectionEffect.Parameters["Phase"].GetValueSingle();
                    phase += (float)Game1.currentGameTime.ElapsedGameTime.TotalSeconds * currentWaterSettings.WaveSpeed;

                    waterReflectionEffect.Parameters["Phase"].SetValue(phase);
                    waterReflectionEffect.Parameters["Frequency"].SetValue(currentWaterSettings.WaveFrequency);
                    waterReflectionEffect.Parameters["Amplitude"].SetValue(currentWaterSettings.WaveAmplitude);
                }
            }

            if (modConfig.AreNPCReflectionsEnabled is not false && currentWaterSettings is not null && currentWaterSettings.AreReflectionsEnabled)
            {
                npcToWaterReflectionPosition.Clear();
                if (Game1.currentLocation is not null && Game1.currentLocation.characters is not null)
                {
                    foreach (var npc in GetActiveNPCs(Game1.currentLocation))
                    {
                        var positionInverter = currentWaterSettings.ReflectionDirection == Direction.North && currentWaterSettings.NPCReflectionOffset.Y > 0 ? -1 : 1;
                        var npcPosition = npc.Position;
                        npcPosition += currentWaterSettings.NPCReflectionOffset * 64 * positionInverter;

                        // Hide the reflection if it will show up out of bounds on the map or not drawn on water tile
                        var waterReflectionPosition = npcPosition / 64f;
                        for (int yOffset = -1; yOffset <= Math.Ceiling(currentWaterSettings.NPCReflectionOffset.Y); yOffset++)
                        {
                            var tilePosition = waterReflectionPosition + new Vector2(0, yOffset);
                            if (IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X - 1, (int)tilePosition.Y) is true || IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X, (int)tilePosition.Y) is true || IsWaterReflectiveTile(Game1.currentLocation, (int)tilePosition.X + 1, (int)tilePosition.Y) is true)
                            {
                                npcToWaterReflectionPosition[npc] = npcPosition;
                                break;
                            }
                        }
                    }
                }
            }

            // Handle the mirror reflections
            DynamicReflections.shouldDrawMirrorReflection = false;
            if (DynamicReflections.modConfig.AreMirrorReflectionsEnabled)
            {
                var playerWorldPosition = Game1.player.Position;
                var playerTilePosition = Game1.player.TilePoint;

                DynamicReflections.activeMirrorPositions.Clear();
                foreach (var mirror in DynamicReflections.mirrors.Values.OrderByDescending(m => m.TilePosition.Y))
                {
                    mirror.IsEnabled = false;

                    // Limit the amount of active Mirrors to the amount of available reflection renders
                    if (activeMirrorPositions.Count >= DynamicReflections.maskedPlayerMirrorReflectionRenders.Length)
                    {
                        break;
                    }

                    var mirrorWidth = mirror.TilePosition.X + (mirror.FurnitureLink is not null ? (int)Math.Ceiling(mirror.Settings.Dimensions.Width / 16f) : mirror.Settings.Dimensions.Width);
                    if (mirror.TilePosition.X - 1 <= playerTilePosition.X && playerTilePosition.X <= mirrorWidth)
                    {
                        var mirrorRange = mirror.TilePosition.Y + (mirror.FurnitureLink is not null ? (int)Math.Ceiling(mirror.Settings.Dimensions.Height / 16f) : mirror.Settings.Dimensions.Height);
                        if (mirror.TilePosition.Y < playerTilePosition.Y && playerTilePosition.Y <= mirrorRange)
                        {
                            // Skip any mirrors that are within range of an already active mirror
                            if (IsTileWithinActiveMirror(mirrorRange))
                            {
                                continue;
                            }

                            mirror.IsEnabled = true;
                            mirror.ActiveIndex = DynamicReflections.activeMirrorPositions.Count;

                            var playerDistanceFromBase = mirror.WorldPosition.Y - playerWorldPosition.Y;
                            var adjustedPosition = new Vector2(playerWorldPosition.X, mirror.WorldPosition.Y + playerDistanceFromBase + 64f);
                            mirror.PlayerReflectionPosition = adjustedPosition;

                            DynamicReflections.shouldDrawMirrorReflection = true;
                            DynamicReflections.activeMirrorPositions.Add(mirror.TilePosition);
                        }
                    }
                }

                if (DynamicReflections.mirrorReflectionSprite is null)
                {
                    DynamicReflections.mirrorReflectionSprite = new FarmerSprite(Game1.player.FarmerSprite.textureName.Value);
                }

                if (Game1.player.FacingDirection == 0 && DynamicReflections.mirrorReflectionSprite.PauseForSingleAnimation is false && Game1.player.UsingTool is false)
                {
                    bool isCarrying = Game1.player.IsCarrying();
                    if (Game1.player.isMoving())
                    {
                        if (Game1.player.running && !isCarrying)
                        {
                            DynamicReflections.mirrorReflectionSprite.animate(32, Game1.currentGameTime);
                        }
                        else if (Game1.player.running)
                        {
                            DynamicReflections.mirrorReflectionSprite.animate(128, Game1.currentGameTime);
                        }
                        else if (isCarrying)
                        {
                            DynamicReflections.mirrorReflectionSprite.animate(96, Game1.currentGameTime);
                        }
                        else
                        {
                            DynamicReflections.mirrorReflectionSprite.animate(0, Game1.currentGameTime);
                        }
                    }
                    else if (Game1.player.IsCarrying())
                    {
                        DynamicReflections.mirrorReflectionSprite.setCurrentFrame(128);
                    }
                    else
                    {
                        DynamicReflections.mirrorReflectionSprite.setCurrentFrame(32);
                    }
                }
            }
        }

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            // Populate the location-based settings
            GMCMHelper.RefreshLocationListing();

            // Hook into GMCM, if applicable
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.GetGenericModConfigMenuApi() is not null)
            {
                GMCMHelper.Register(apiManager.GetGenericModConfigMenuApi(), this, unregisterOld: true, loadLocationNames: true);
            }

            SetSkyReflectionSettings();
            SetPuddleReflectionSettings();
            SetWaterReflectionSettings();
            DetectMirrorsForActiveLocation();

            DynamicReflections.puddleManager.Reset();
        }

        private void OnDayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            Game1.player.modData[ModDataKeys.DID_RAIN_YESTERDAY] = Game1.IsRainingHere(Game1.player.currentLocation).ToString();
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // Load any owned content packs
            LoadContentPacks();

            // Set our default configuration file
            modConfig = Helper.ReadConfig<ModConfig>();
            modConfig.LocalWaterReflectionSettings[GMCMHelper.DEFAULT_LOCATION] = modConfig.WaterReflectionSettings;
            modConfig.LocalPuddleReflectionSettings[GMCMHelper.DEFAULT_LOCATION] = modConfig.PuddleReflectionSettings;
            modConfig.LocalSkyReflectionSettings[GMCMHelper.DEFAULT_LOCATION] = modConfig.SkyReflectionSettings;

            // Hook into GMCM, if applicable
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGenericModConfigMenu(Helper))
            {
                GMCMHelper.Register(apiManager.GetGenericModConfigMenuApi(), this);
            }

            // Load in our shaders
            // Compile via the command: mgfxc wavy.fx wavy.mgfx
            // Unused: opacityEffect = new Effect(Game1.graphics.GraphicsDevice, File.ReadAllBytes(Path.Combine(modHelper.DirectoryPath, "Framework", "Assets", "Shaders", "opacity.mgfx")));
            mirrorReflectionEffect = new Effect(Game1.graphics.GraphicsDevice, File.ReadAllBytes(Path.Combine(modHelper.DirectoryPath, "Framework", "Assets", "Shaders", "mask.mgfx")));

            waterReflectionEffect = new Effect(Game1.graphics.GraphicsDevice, File.ReadAllBytes(Path.Combine(modHelper.DirectoryPath, "Framework", "Assets", "Shaders", "wavy.mgfx")));
            waterReflectionEffect.CurrentTechnique = waterReflectionEffect.Techniques["Wavy"];

            // Create the RenderTarget2D and RasterizerState for use by the water reflection
            LoadRenderers();
        }

        private void LoadContentPacks(bool silent = false)
        {
            // Clear the existing cache of custom buildings
            mirrorsManager.Reset();

            // Load owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                try
                {
                    Monitor.Log($"Loading data from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", silent ? LogLevel.Trace : LogLevel.Debug);

                    // Load mirrors
                    if (!File.Exists(Path.Combine(contentPack.DirectoryPath, "mirrors.json")))
                    {
                        Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a mirrors.json!", LogLevel.Warn);
                        continue;
                    }

                    var mirrorModels = contentPack.ReadJsonFile<List<ContentPackModel>>("mirrors.json");
                    if (mirrorModels is null || mirrorModels.Count == 0)
                    {
                        Monitor.Log($"Content pack {contentPack.Manifest.Name} has an empty or invalid mirrors.json!", LogLevel.Warn);
                        continue;
                    }

                    // Verify that the mask texture exists under the content pack's Masks folder
                    foreach (var mirrorModel in mirrorModels)
                    {
                        if (String.IsNullOrEmpty(mirrorModel.MaskTexture) || !File.Exists(Path.Combine(contentPack.DirectoryPath, "Masks", mirrorModel.MaskTexture)))
                        {
                            Monitor.Log($"The mirror for {mirrorModel.FurnitureId} within content pack {contentPack.Manifest.Name} is missing its mask texture!", LogLevel.Warn);
                            continue;
                        }

                        mirrorModel.Mask = contentPack.ModContent.Load<Texture2D>(Path.Combine("Masks", mirrorModel.MaskTexture));
                    }

                    mirrorsManager.Add(mirrorModels);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the content pack {contentPack.Manifest.UniqueID}: {ex}", LogLevel.Warn);
                }
            }
        }


        internal void SetSkyReflectionSettings(bool recalculate = false)
        {
            if (currentSkySettings is null)
            {
                currentSkySettings = new SkySettings();
            }

            if (Context.IsWorldReady is false || Game1.currentLocation is null || Game1.currentLocation.Map is null)
            {
                return;
            }
            currentSkySettings.Reset(modConfig.GetCurrentSkySettings(Game1.currentLocation));

            // Check if today should have a meteor shower
            isMeteorShower = false;
            if (Game1.random.NextDouble() <= modConfig.MeteorShowerNightChance / 100f)
            {
                isMeteorShower = true;
            }

            // Set the map specific puddle settings
            var map = Game1.currentLocation.Map;
            if (map.Properties.ContainsKey(SkySettings.MapProperty_IsEnabled))
            {
                currentSkySettings.AreReflectionsEnabled = map.Properties[SkySettings.MapProperty_IsEnabled].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_AreShootingStarsEnabled))
            {
                currentSkySettings.AreShootingStarsEnabled = map.Properties[SkySettings.MapProperty_AreShootingStarsEnabled].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_MillisecondsBetweenShootingStarAttempt))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_MillisecondsBetweenShootingStarAttempt], out var milliseconds))
                {
                    currentSkySettings.MillisecondsBetweenShootingStarAttempt = milliseconds;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_MaxShootingStarAttemptsPerInterval))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_MaxShootingStarAttemptsPerInterval], out var max))
                {
                    currentSkySettings.MaxShootingStarAttemptsPerInterval = max;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_CometChance))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_CometChance], out var chance))
                {
                    currentSkySettings.CometChance = chance;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_CometSegmentMin))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_CometSegmentMin], out var segment))
                {
                    currentSkySettings.CometSegmentMin = segment;
                }
            }
            if (map.Properties.ContainsKey(SkySettings.MapProperty_CometSegmentMax))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_CometSegmentMax], out var segment))
                {
                    currentSkySettings.CometSegmentMax = segment;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_ShootingStarMinSpeed))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_ShootingStarMinSpeed], out var speed))
                {
                    currentSkySettings.ShootingStarMinSpeed = speed;
                }
            }
            if (map.Properties.ContainsKey(SkySettings.MapProperty_ShootingStarMaxSpeed))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_ShootingStarMaxSpeed], out var speed))
                {
                    currentSkySettings.ShootingStarMaxSpeed = speed;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_CometMinSpeed))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_CometMinSpeed], out var speed))
                {
                    currentSkySettings.CometMinSpeed = speed;
                }
            }
            if (map.Properties.ContainsKey(SkySettings.MapProperty_CometMaxSpeed))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_CometMaxSpeed], out var speed))
                {
                    currentSkySettings.CometMaxSpeed = speed;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_GettingDarkWaterAlpha))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_GettingDarkWaterAlpha], out var darkness))
                {
                    currentSkySettings.GettingDarkWaterAlpha = darkness;
                }
            }
            if (map.Properties.ContainsKey(SkySettings.MapProperty_HalfwayDarkWaterAlpha))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_HalfwayDarkWaterAlpha], out var darkness))
                {
                    currentSkySettings.HalfwayDarkWaterAlpha = darkness;
                }
            }
            if (map.Properties.ContainsKey(SkySettings.MapProperty_FinishedDarkWaterAlpha))
            {
                if (float.TryParse(map.Properties[SkySettings.MapProperty_FinishedDarkWaterAlpha], out var darkness))
                {
                    currentSkySettings.FinishedDarkWaterAlpha = darkness;
                }
            }

            if (map.Properties.ContainsKey(SkySettings.MapProperty_MillisecondsBetweenShootingStarAttemptDuringMeteorShower))
            {
                if (Int32.TryParse(map.Properties[SkySettings.MapProperty_MillisecondsBetweenShootingStarAttemptDuringMeteorShower], out var milliseconds))
                {
                    currentSkySettings.MillisecondsBetweenShootingStarAttemptDuringMeteorShower = milliseconds;
                }
            }

            if (recalculate && DynamicReflections.skyManager is not null && Game1.currentLocation is not null && Game1.currentLocation.IsOutdoors is true)
            {
                DynamicReflections.skyManager.Generate(Game1.currentLocation, force: true);
            }
        }

        internal void SetPuddleReflectionSettings(bool recalculate = false)
        {
            if (currentPuddleSettings is null)
            {
                currentPuddleSettings = new PuddleSettings();
            }

            if (Context.IsWorldReady is false || Game1.currentLocation is null || Game1.currentLocation.Map is null)
            {
                return;
            }
            currentPuddleSettings.Reset(modConfig.GetCurrentPuddleSettings(Game1.currentLocation));

            // Set the map specific puddle settings
            var map = Game1.currentLocation.Map;
            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_IsEnabled))
            {
                currentPuddleSettings.AreReflectionsEnabled = map.Properties[PuddleSettings.MapProperty_IsEnabled].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_ShouldGeneratePuddles))
            {
                currentPuddleSettings.ShouldGeneratePuddles = map.Properties[PuddleSettings.MapProperty_ShouldGeneratePuddles].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_ShouldPlaySplashSound))
            {
                currentPuddleSettings.ShouldPlaySplashSound = map.Properties[PuddleSettings.MapProperty_ShouldPlaySplashSound].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_ShouldRainSplashPuddles))
            {
                currentPuddleSettings.ShouldRainSplashPuddles = map.Properties[PuddleSettings.MapProperty_ShouldRainSplashPuddles].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_PuddleReflectionOffset))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Vector2>(map.Properties[PuddleSettings.MapProperty_PuddleReflectionOffset]) is Vector2 offset)
                    {
                        currentPuddleSettings.ReflectionOffset = offset;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_PuddleReflectionOffset from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_PuddleReflectionOffset from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_NPCReflectionOffset))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Vector2>(map.Properties[PuddleSettings.MapProperty_NPCReflectionOffset]) is Vector2 offset)
                    {
                        currentPuddleSettings.NPCReflectionOffset = offset;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_NPCReflectionOffset from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_NPCReflectionOffset from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_PuddlePercentageWhileRaining))
            {
                if (Int32.TryParse(map.Properties[PuddleSettings.MapProperty_PuddlePercentageWhileRaining], out var percentage))
                {
                    currentPuddleSettings.PuddlePercentageWhileRaining = percentage;
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_PuddlePercentageAfterRaining))
            {
                if (Int32.TryParse(map.Properties[PuddleSettings.MapProperty_PuddlePercentageAfterRaining], out var percentage))
                {
                    currentPuddleSettings.PuddlePercentageAfterRaining = percentage;
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_BigPuddleChance))
            {
                if (Int32.TryParse(map.Properties[PuddleSettings.MapProperty_BigPuddleChance], out var percentage))
                {
                    currentPuddleSettings.BigPuddleChance = percentage;
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_MillisecondsBetweenRaindropSplashes))
            {
                if (Int32.TryParse(map.Properties[PuddleSettings.MapProperty_MillisecondsBetweenRaindropSplashes], out var percentage))
                {
                    currentPuddleSettings.MillisecondsBetweenRaindropSplashes = percentage;
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_ReflectionOverlay))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Color>(map.Properties[PuddleSettings.MapProperty_ReflectionOverlay]) is Color overlay)
                    {
                        currentPuddleSettings.ReflectionOverlay = overlay;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_ReflectionOverlay from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_ReflectionOverlay from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_PuddleColor))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Color>(map.Properties[PuddleSettings.MapProperty_PuddleColor]) is Color overlay)
                    {
                        currentPuddleSettings.PuddleColor = overlay;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_PuddleColor from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_PuddleColor from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(PuddleSettings.MapProperty_RippleColor))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Color>(map.Properties[PuddleSettings.MapProperty_RippleColor]) is Color overlay)
                    {
                        currentPuddleSettings.RippleColor = overlay;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_RippleColor from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get PuddleSettings.MapProperty_RippleColor from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            // TODO: Implement this as a button in the config
            if (recalculate && DynamicReflections.puddleManager is not null && Game1.currentLocation is not null)
            {
                DynamicReflections.puddleManager.Generate(Game1.currentLocation, force: true);
            }
        }

        internal void SetWaterReflectionSettings()
        {
            if (currentWaterSettings is null)
            {
                currentWaterSettings = new WaterSettings();
            }

            if (Context.IsWorldReady is false || Game1.currentLocation is null || Game1.currentLocation.Map is null)
            {
                return;
            }
            currentWaterSettings.Reset(modConfig.GetCurrentWaterSettings(Game1.currentLocation));

            // Set the map specific water settings
            var map = Game1.currentLocation.Map;
            if (map.Properties.ContainsKey(WaterSettings.MapProperty_IsEnabled))
            {
                currentWaterSettings.AreReflectionsEnabled = map.Properties[WaterSettings.MapProperty_IsEnabled].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_ReflectionDirection))
            {
                if (Enum.TryParse<Direction>(map.Properties[WaterSettings.MapProperty_ReflectionDirection], out var direction))
                {
                    currentWaterSettings.ReflectionDirection = direction;
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_ReflectionOverlay))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Color>(map.Properties[WaterSettings.MapProperty_ReflectionOverlay]) is Color overlay)
                    {
                        currentWaterSettings.ReflectionOverlay = overlay;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_ReflectionOverlay from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_ReflectionOverlay from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_ReflectionOffset))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Vector2>(map.Properties[WaterSettings.MapProperty_ReflectionOffset]) is Vector2 offset)
                    {
                        currentWaterSettings.PlayerReflectionOffset = offset;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_ReflectionOffset from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_ReflectionOffset from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_NPCReflectionOffset))
            {
                try
                {
                    if (JsonSerializer.Deserialize<Vector2>(map.Properties[WaterSettings.MapProperty_NPCReflectionOffset]) is Vector2 offset)
                    {
                        currentWaterSettings.NPCReflectionOffset = offset;
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_NPCReflectionOffset from the map {map.Id}!", LogLevel.Warn);
                    Monitor.Log($"Failed to get WaterSettings.MapProperty_NPCReflectionOffset from the map {map.Id}: {ex}", LogLevel.Trace);
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_IsReflectionWavy))
            {
                currentWaterSettings.IsReflectionWavy = map.Properties[WaterSettings.MapProperty_IsReflectionWavy].ToString().Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_WaveSpeed))
            {
                if (float.TryParse(map.Properties[WaterSettings.MapProperty_WaveSpeed], out var waveSpeed))
                {
                    currentWaterSettings.WaveSpeed = waveSpeed;
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_WaveAmplitude))
            {
                if (float.TryParse(map.Properties[WaterSettings.MapProperty_WaveAmplitude], out var waveAmplitude))
                {
                    currentWaterSettings.WaveAmplitude = waveAmplitude;
                }
            }

            if (map.Properties.ContainsKey(WaterSettings.MapProperty_WaveFrequency))
            {
                if (float.TryParse(map.Properties[WaterSettings.MapProperty_WaveFrequency], out var waveFrequency))
                {
                    currentWaterSettings.WaveFrequency = waveFrequency;
                }
            }
        }

        private void DetectMirrorsForActiveLocation()
        {
            if (Context.IsWorldReady is false || Game1.currentLocation is null)
            {
                return;
            }
            var currentLocation = Game1.currentLocation;

            // Clear the old base points out
            DynamicReflections.mirrors.Clear();

            // Check current map for tiles with IsMirrorBase
            var map = currentLocation.Map;
            if (map is not null && (map.GetLayer("Mirrors") is var mirrorLayer && mirrorLayer is not null))
            {
                for (int x = 0; x < mirrorLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < mirrorLayer.LayerHeight; y++)
                    {
                        if (IsMirrorBaseTile(currentLocation, x, y, true) is false)
                        {
                            continue;
                        }

                        var point = new Point(x, y);
                        if (DynamicReflections.mirrors.ContainsKey(point) is false)
                        {
                            var settings = new MirrorSettings()
                            {
                                Dimensions = new Rectangle(0, 0, GetMirrorWidth(currentLocation, x, y), GetMirrorHeight(currentLocation, x, y) - 1),
                                ReflectionOffset = GetMirrorOffset(currentLocation, x, y),
                                ReflectionOverlay = GetMirrorOverlay(currentLocation, x, y),
                                ReflectionScale = GetMirrorScale(currentLocation, x, y)
                            };

                            DynamicReflections.mirrors[point] = new Mirror()
                            {
                                TilePosition = point,
                                Settings = settings
                            };
                        }
                    }
                }
            }

            // Find all mirror furniture
            foreach (var furniture in currentLocation.furniture)
            {
                if (DynamicReflections.mirrorsManager.GetSettings(furniture.Name) is MirrorSettings baseSettings && baseSettings is not null)
                {
                    var point = new Point((int)furniture.TileLocation.X, (int)furniture.TileLocation.Y);
                    var settings = new MirrorSettings()
                    {
                        Dimensions = new Rectangle(baseSettings.Dimensions.X, baseSettings.Dimensions.Y, baseSettings.Dimensions.Width, baseSettings.Dimensions.Height),
                        ReflectionOffset = baseSettings.ReflectionOffset,
                        ReflectionOverlay = baseSettings.ReflectionOverlay,
                        ReflectionScale = baseSettings.ReflectionScale
                    };

                    DynamicReflections.mirrors.Add(point, new Mirror()
                    {
                        FurnitureLink = furniture,
                        TilePosition = point,
                        Settings = settings
                    });
                }
            }
        }

        internal void LoadRenderers()
        {
            bool shouldUseScreenDimensions = Game1.options.zoomLevel != 1f;

            // Handle the render targets
            RegenerateRenderer(ref nightSkyRenderTarget, shouldUseScreenDimensions);
            RegenerateRenderer(ref cloudRenderTarget, shouldUseScreenDimensions);
            RegenerateRenderer(ref playerWaterReflectionRender, shouldUseScreenDimensions);
            RegenerateRenderer(ref playerPuddleReflectionRender, shouldUseScreenDimensions);
            RegenerateRenderer(ref npcWaterReflectionRender, shouldUseScreenDimensions);
            RegenerateRenderer(ref npcPuddleReflectionRender, shouldUseScreenDimensions);
            RegenerateRenderer(ref puddlesRenderTarget, shouldUseScreenDimensions);

            RegenerateRenderer(ref mirrorsLayerRenderTarget, shouldUseScreenDimensions);
            RegenerateRenderer(ref mirrorsFurnitureRenderTarget, shouldUseScreenDimensions);

            if (maskedPlayerMirrorReflectionRenders is null)
            {
                maskedPlayerMirrorReflectionRenders = new RenderTarget2D[3];
            }
            for (int i = 0; i < 3; i++)
            {
                RegenerateRenderer(ref maskedPlayerMirrorReflectionRenders[i], shouldUseScreenDimensions);
            }

            if (composedPlayerMirrorReflectionRenders is null)
            {
                composedPlayerMirrorReflectionRenders = new RenderTarget2D[3];
            }
            for (int i = 0; i < 3; i++)
            {
                RegenerateRenderer(ref composedPlayerMirrorReflectionRenders[i], shouldUseScreenDimensions);
            }

            RegenerateRenderer(ref inBetweenRenderTarget, shouldUseScreenDimensions);

            // Handle the rasterizer
            if (rasterizer is not null)
            {
                rasterizer.Dispose();
            }

            rasterizer = new RasterizerState();
            rasterizer.CullMode = CullMode.CullClockwiseFace;
        }

        private void RegenerateRenderer(ref RenderTarget2D renderTarget2D, bool shouldUseScreenDimensions = false)
        {
            if (renderTarget2D is not null)
            {
                renderTarget2D.Dispose();
            }

            var height = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var width = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            if (shouldUseScreenDimensions is true && Game1.game1 is not null && Game1.game1.screen is not null)
            {
                height = Game1.game1.screen.Height;
                width = Game1.game1.screen.Width;
            }

            renderTarget2D = new RenderTarget2D(
                Game1.graphics.GraphicsDevice,
                width,
                height,
                false,
                Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);
        }

        private bool IsTileWithinActiveMirror(int tileY)
        {
            foreach (var activePosition in DynamicReflections.activeMirrorPositions)
            {
                if (tileY - 1 <= activePosition.Y && activePosition.Y <= tileY + 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMirrorBaseTile(GameLocation location, int x, int y, bool requireEnabled = false)
        {
            string isMirrorProperty = location.doesTileHavePropertyNoNull(x, y, "IsMirrorBase", "Mirrors");
            if (String.IsNullOrEmpty(isMirrorProperty))
            {
                return false;
            }

            if (requireEnabled is true && isMirrorProperty.Equals("T", StringComparison.OrdinalIgnoreCase) is false && isMirrorProperty.Equals("True", StringComparison.OrdinalIgnoreCase) is false)
            {
                return false;
            }

            return true;
        }

        private int GetMirrorHeight(GameLocation location, int x, int y)
        {
            string mirrorHeightProperty = location.doesTileHavePropertyNoNull(x, y, "MirrorHeight", "Mirrors");
            if (String.IsNullOrEmpty(mirrorHeightProperty))
            {
                return 1;
            }

            if (Int32.TryParse(mirrorHeightProperty, out int mirrorHeightValue) is false)
            {
                return 1;
            }

            return mirrorHeightValue;
        }

        private int GetMirrorWidth(GameLocation location, int x, int y)
        {
            string mirrorWidthProperty = location.doesTileHavePropertyNoNull(x, y, "MirrorWidth", "Mirrors");
            if (String.IsNullOrEmpty(mirrorWidthProperty))
            {
                return 1;
            }

            if (Int32.TryParse(mirrorWidthProperty, out int mirrorWidthValue) is false)
            {
                return 1;
            }

            return mirrorWidthValue;
        }

        private float GetMirrorScale(GameLocation location, int x, int y)
        {
            string mirrorScaleProperty = location.doesTileHavePropertyNoNull(x, y, "MirrorScale", "Mirrors");
            if (String.IsNullOrEmpty(mirrorScaleProperty))
            {
                return 1f;
            }

            if (float.TryParse(mirrorScaleProperty, out float mirrorScaleValue) is false)
            {
                return 1f;
            }

            return mirrorScaleValue;
        }

        private Color GetMirrorOverlay(GameLocation location, int x, int y)
        {
            string mirrorOverlayProperty = location.doesTileHavePropertyNoNull(x, y, "MirrorOverlay", "Mirrors");
            if (String.IsNullOrEmpty(mirrorOverlayProperty) || mirrorOverlayProperty.Split(' ').Length < 3)
            {
                return Color.White;
            }
            var splitColorValues = mirrorOverlayProperty.Split(' ');

            for (int i = 0; i < 3; i++)
            {
                if (Int32.TryParse(splitColorValues[i], out int _) is false)
                {
                    return Color.White;
                }
            }

            return new Color(Int32.Parse(splitColorValues[0]), Int32.Parse(splitColorValues[1]), Int32.Parse(splitColorValues[2]), Int32.Parse(splitColorValues[3]));
        }

        private Vector2 GetMirrorOffset(GameLocation location, int x, int y)
        {
            string mirrorOffsetProperty = location.doesTileHavePropertyNoNull(x, y, "MirrorOffset", "Mirrors");
            if (String.IsNullOrEmpty(mirrorOffsetProperty))
            {
                return Vector2.Zero;
            }

            if (mirrorOffsetProperty.Contains(" ") is false)
            {
                return Vector2.Zero;
            }

            var xOffsetText = mirrorOffsetProperty.Split(" ")[0];
            var yOffsetText = mirrorOffsetProperty.Split(" ")[1];
            if (float.TryParse(xOffsetText, out float xOffsetValue) is false || float.TryParse(yOffsetText, out float yOffsetValue) is false)
            {
                return Vector2.Zero;
            }

            return new Vector2(xOffsetValue, yOffsetValue);
        }

        private bool IsWaterReflectiveTile(GameLocation location, int x, int y)
        {
            if (location is null)
            {
                return false;
            }

            return location.isWaterTile(x, y);
        }

        internal static int GetReflectedDirection(int initialDirection, bool isMirror = false)
        {
            if (initialDirection == 0)
            {
                return isMirror is true ? 2 : 0;
            }
            else if (initialDirection == 2)
            {
                return isMirror is true ? 0 : 2;
            }

            return initialDirection;
        }

        internal static List<NPC> GetActiveNPCs(GameLocation location)
        {
            var npcs = new List<NPC>();
            if (location is null)
            {
                return npcs;
            }

            if (Game1.eventUp && location.currentEvent is not null && location.currentEvent.actors is not null)
            {
                npcs = location.currentEvent.actors.ToList();
            }
            else if (location.characters is not null)
            {
                npcs = location.characters.ToList();
            }

            return npcs;
        }
    }
}
