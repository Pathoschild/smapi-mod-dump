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
using HDSprites.Token;
using Microsoft.Xna.Framework;

namespace HDSprites
{
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
            "TileSheets\\tools"
        };

        private ModConfig Config;
        private HDAssetManager HDAssetManager;
        private ContentPackManager ContentPackManager;

        public override void Entry(IModHelper help)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            Logger = this.Monitor;
            EnabledAssets = new Dictionary<string, bool>();
            ModHelper = help;
            EnableMod = this.Config.EnableMod;
            
            this.HDAssetManager = new HDAssetManager(help);
            this.ContentPackManager = new ContentPackManager(this.HDAssetManager);
            
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            this.Helper.Events.Player.Warped += OnWarped;
            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;

            foreach (var asset in this.Config.LoadAssets)
            {
                string loadSection = asset.Key.Substring(0, asset.Key.LastIndexOf("/"));
                bool enabled = asset.Value && this.Config.LoadSections.GetValueSafe(loadSection);
                AddEnabledAsset(asset.Key, enabled);
                if (enabled) { 
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
                    manifest = this.Helper.ReadJsonFile<ContentPackManifest>(manifestFile);
                }
                catch (Exception e)
                {
                    continue;
                }

                if (manifest != null && this.Config.LoadContentPacks.TryGetValue(manifest.UniqueID, out bool load) && load)
                {
                    this.Monitor.Log($"Reading content pack: {manifest.Name} {manifest.Version}");
                    
                    WhenDictionary configChoices = null;
                    try
                    {
                        configChoices = this.Helper.ReadJsonFile<WhenDictionary>(Path.Combine(dir, "config.json"));
                    }
                    catch (Exception e)
                    {
                        this.Monitor.Log($"Failed to read config.json for {manifest.Name} {manifest.Version}");
                        continue;
                    }
                        
                    if (configChoices == null) configChoices = new WhenDictionary();

                    ContentConfig contentConfig = null;

                    try
                    {
                        contentConfig = this.Helper.ReadJsonFile<ContentConfig>(Path.Combine(dir, "content.json"));
                    }
                    catch (Exception e)
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

        public bool CanEdit<T>(IAssetInfo assetInfo)
        {
            string assetName = assetInfo.AssetName.Replace("/", $"\\");
            if (this.ContentPackManager != null) this.ContentPackManager.UpdateAssetEditable(assetName);
            return IsAssetEnabled(assetName);
        }

        public void Edit<T>(IAssetData assetData)
        {
            AssetTexture assetTexture;
            string assetName = assetData.AssetName.Replace("/", $"\\");
            
            if (!AssetTextures.ContainsKey(assetName))
            {
                Texture2D texture = this.HDAssetManager.LoadAsset(assetName);

                assetTexture = new AssetTexture(assetName, assetData.AsImage().Data, texture, this.Config.AssetScale, WhiteBoxFixAssets.Contains(assetName));
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
                    AssetTextures[assetName] = assetTexture = new AssetTexture(assetName, assetData.AsImage().Data, assetTexture.HDTexture, this.Config.AssetScale, WhiteBoxFixAssets.Contains(assetName));
                }
            }

            this.ContentPackManager.EditAsset(assetName);

            if (GraphicsDevice == null) GraphicsDevice = assetTexture.GraphicsDevice;

            assetData.AsImage().ReplaceWith(assetTexture);
        }
        
        private void OnButtonPressed(object s, ButtonPressedEventArgs e)
        {
            if (e.Button.Equals(this.Config.ToggleEnableButton))
            {
                EnableMod = !EnableMod;
                this.Config.EnableMod = EnableMod;
                this.Helper.WriteConfig(this.Config);
            }
        }

        private void OnDayStarted(object s, DayStartedEventArgs e)
        {
            this.ContentPackManager.DynamicTokenManager.CheckTokens();
        }

        private void OnWarped(object s, WarpedEventArgs e)
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
    }
}