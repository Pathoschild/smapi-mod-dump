/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

/// **********
/// HDSpritesMod is a mod for Stardew Valley using SMAPI and Harmony.
/// It loads all *.png/*.xnb from the mod's local assets folder into 
/// ScaledTexture2D objects and replaces their game loaded counterparts.
/// 
/// Harmony is used to patch the XNA drawMethod (which the game uses to render its
/// textures) to check if the texture being drawn is of the replaced type ScaledTexture2D, 
/// and if it is, then draw the larger version using its scale adjusted parameters.
/// 
/// Credit goes to Platonymous for the ScaledTexture2D and SpriteBatchFix Harmony 
/// patch classes from his Portraiture mod that makes this whole mod possible.
/// 
/// Author: NinthWorld
/// Date: 5/31/19
/// **********

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using HDSprites.ContentPack;
using System;
using Newtonsoft.Json;

namespace HDSprites
{
    /// <summary>The mod entry class.</summary>
    public class HDSpritesMod : Mod, IAssetEditor
    {
        public static IMonitor Logger;
        public static Dictionary<string, bool> EnabledAssets { get; set; }
        public static GraphicsDevice GraphicsDevice = null;
        public static IModHelper ModHelper;
        public static bool EnableMod = true;
        public static Dictionary<string, AssetTexture> AssetTextures = new Dictionary<string, AssetTexture>();

        public static List<string> WhiteBoxFixAssets = new List<string>()
        {
            "TileSheets\\tools",
            "Characters\\Farmer\\farmer_base",
            "Characters\\Farmer\\farmer_base_bald",
            "Characters\\Farmer\\farmer_girl_base",
            "Characters\\Farmer\\farmer_girl_base_bald",
        };

        private ModConfig Config;
        private HDAssetManager HDAssetManager;
        private ContentPackManager ContentPackManager;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="help">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper help)
        {            
            this.Config = this.Helper.ReadConfig<ModConfig>();

            Logger = this.Monitor;
            EnabledAssets = new Dictionary<string, bool>();
            ModHelper = help;
            EnableMod = this.Config.EnableMod;

            this.HDAssetManager = new HDAssetManager(help);
            this.ContentPackManager = new ContentPackManager(this.HDAssetManager);

            help.Events.GameLoop.GameLaunched += OnGameLaunched;
            help.Events.Input.ButtonPressed += OnButtonPressed;
            help.Events.Player.Warped += OnWarped;
            help.Events.GameLoop.DayStarted += OnDayStarted;

            foreach (var asset in this.Config.LoadAssets)
            {
                string loadSection = asset.Key.Substring(0, asset.Key.LastIndexOf("/"));
                bool enabled = asset.Value && this.Config.LoadSections.GetValueSafe(loadSection);
                AddEnabledAsset(asset.Key, enabled);
                if (enabled)
                {
                    string assetFile = Path.Combine(this.Config.AssetsPath, asset.Key) + ".png";
                    if (File.Exists(Path.Combine(help.DirectoryPath, assetFile)))
                    {
                        this.HDAssetManager.AddAssetFile(asset.Key, assetFile);
                    }
                }
            }

            string[] contentPackDirs = Directory.GetDirectories(Path.Combine(help.DirectoryPath, ".."));

            foreach (string dir in contentPackDirs)
            {
                string manifestFile = Path.Combine(dir, "manifest.json");
                if (Directory.GetParent(manifestFile).Name.StartsWith(".")) continue;

                ContentPackManifest manifest = null;
                try
                {
                    manifest = ReadExternalJson<ContentPackManifest>(manifestFile);
                }
                catch (Exception)
                {
                    this.Monitor.Log($"Failed to read manifest.json from {dir}");
                    continue;
                }

                if (manifest != null && this.Config.LoadContentPacks.TryGetValue(manifest.UniqueID, out bool load) && load)
                {
                    this.Monitor.Log($"Reading content pack: {manifest.Name} {manifest.Version}");

                    WhenDictionary configChoices = null;
                    try
                    {
                        configChoices = ReadExternalJson<WhenDictionary>(Path.Combine(dir, "config.json"));
                    }
                    catch (Exception)
                    {
                        this.Monitor.Log($"Failed to read config.json for {manifest.Name} {manifest.Version}");
                        continue;
                    }

                    if (configChoices == null) configChoices = new WhenDictionary();

                    ContentConfig contentConfig = null;

                    try
                    {
                        contentConfig = ReadExternalJson<ContentConfig>(Path.Combine(dir, "content.json"));
                    }
                    catch (Exception)
                    {
                        this.Monitor.Log($"Failed to read content.json for {manifest.Name} {manifest.Version}");
                        continue;
                    }

                    if (contentConfig == null) continue;

                    string contentPath = Path.Combine(help.DirectoryPath, this.Config.ContentPacksPath, manifest.UniqueID);
                    Directory.CreateDirectory(contentPath);

                    ContentPackObject contentPack = new ContentPackObject(dir, contentPath, manifest, contentConfig, configChoices);
                    this.ContentPackManager.AddContentPack(contentPack);
                }
            }

            HarmonyInstance instance = HarmonyInstance.Create("NinthWorld.HDSprites");
            DrawFix.InitializePatch(instance);
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.ContentPackManager.DynamicTokenManager.CheckTokens();
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="assetInfo">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo assetInfo)
        {
            string assetName = assetInfo.AssetName.Replace("/", $"\\");
            ContentPackManager?.UpdateAssetEditable(assetName);
            return IsAssetEnabled(assetName);
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="assetData">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData assetData)
        {
            AssetTexture assetTexture;
            string assetName = assetData.AssetName.Replace("/", $"\\");

            if (!AssetTextures.ContainsKey(assetName))
            {
                Texture2D texture = this.HDAssetManager.LoadAsset(assetName);

                assetTexture = new AssetTexture(assetName, assetData.AsImage().Data, texture, 2, WhiteBoxFixAssets.Contains(assetName));
                AssetTextures.Add(assetName, assetTexture);
            }
            else
            {
                assetTexture = AssetTextures.GetValueSafe(assetName);

                if (assetData.AsImage().Data.Bounds.Equals(assetTexture.Bounds))
                {
                    assetTexture.SetOriginalTexture(assetData.AsImage().Data);
                }
                else
                {
                    AssetTextures[assetName] = assetTexture = new AssetTexture(assetName, assetData.AsImage().Data, assetTexture.HDTexture, 2, WhiteBoxFixAssets.Contains(assetName));
                }
            }

            this.ContentPackManager.EditAsset(assetName);

            if (GraphicsDevice == null) GraphicsDevice = assetTexture.GraphicsDevice;

            assetData.AsImage().ReplaceWith(assetTexture);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.ToggleEnableButton)
            {
                EnableMod = !EnableMod;
                this.Config.EnableMod = EnableMod;
                this.Helper.WriteConfig(this.Config);
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.ContentPackManager.DynamicTokenManager.CheckTokens();
        }

        /// <summary>Raised after a player warps to a new location. NOTE: this event is currently only raised for the current player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            this.ContentPackManager.DynamicTokenManager.CheckTokens();
        }

        public static void AddEnabledAsset(string assetName, bool enabled)
        {
            assetName = assetName.Replace("/", $"\\");
            if (!EnabledAssets.ContainsKey(assetName))
            {
                EnabledAssets.Add(assetName, enabled);
            }
        }

        public static bool IsAssetEnabled(string assetName)
        {
            return EnabledAssets.TryGetValue(assetName, out bool enabled) && enabled;
        }

        public static T ReadExternalJson<T>(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
