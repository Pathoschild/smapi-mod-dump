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

namespace HDSprites
{
    public class HDSpritesMod : Mod, IAssetEditor
    {
        public static GraphicsDevice GraphicsDevice = null;
        public static IModHelper ModHelper;
        public static bool EnableMod = true;
        public static Dictionary<string, AssetTexture> AssetTextures = new Dictionary<string, AssetTexture>();

        public static List<string> ScaleFixAssets = new List<string>()
        {
            "TileSheets\\Craftables",
            "Maps\\MenuTiles",
            "LooseSprites\\LanguageButtons",
            "LooseSprites\\chatBox",
            "LooseSprites\\textBox",
            "LooseSprites\\yellowLettersLogo",
            "LooseSprites\\JunimoNote"
        };

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

            ModHelper = help;
            EnableMod = this.Config.EnableMod;
            
            this.HDAssetManager = new HDAssetManager(help);
            this.ContentPackManager = new ContentPackManager(this.HDAssetManager);
            
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            this.Helper.Events.Player.Warped += OnWarped;
            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;

            foreach (var asset in this.Config.LoadAssets)
            {
                if (asset.Value)
                {
                    string loadSection = asset.Key.Substring(0, asset.Key.LastIndexOf("/"));
                    if (this.Config.LoadSections.GetValueSafe(loadSection))
                    {
                        string assetFile = Path.Combine(this.Config.AssetsPath, asset.Key) + ".png";
                        if (File.Exists(Path.Combine(help.DirectoryPath, assetFile)))
                        {
                            this.HDAssetManager.AddAssetFile(asset.Key, assetFile);
                        }
                    }
                }
            }
            
            string[] contentPackDirs = Directory.GetDirectories(Path.Combine(help.DirectoryPath, ".."));
            foreach (string dir in contentPackDirs)
            {
                string manifestFile = Path.Combine(dir, "manifest.json");
                ContentPackManifest manifest = this.Helper.ReadJsonFile<ContentPackManifest>(manifestFile);
                if (manifest != null && this.Config.LoadContentPacks.TryGetValue(manifest.UniqueID, out bool load) && load)
                {
                    this.Monitor.Log($"Reading content pack: {manifest.Name} {manifest.Version} from {manifestFile}");
                    
                    WhenDictionary configChoices = this.Helper.ReadJsonFile<WhenDictionary>(Path.Combine(dir, "config.json"));
                    if (configChoices == null) configChoices = new WhenDictionary();

                    ContentConfig contentConfig = this.Helper.ReadJsonFile<ContentConfig>(Path.Combine(dir, "content.json"));
                    if (contentConfig == null) continue;

                    string contentPath = Path.Combine(help.DirectoryPath, this.Config.ContentPacksPath, manifest.UniqueID);
                    Directory.CreateDirectory(contentPath);
                                        
                    ContentPackObject contentPack = new ContentPackObject(dir, contentPath, manifest, contentConfig, configChoices);
                    this.ContentPackManager.AddContentPack(contentPack);
                }
            }

            HarmonyInstance instance = HarmonyInstance.Create("NinthWorld.HDSprites");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public bool CanEdit<T>(IAssetInfo assetInfo)
        {
            return this.HDAssetManager.ContainsAsset(assetInfo.AssetName);
        }

        public void Edit<T>(IAssetData assetData)
        {
            AssetTexture assetTexture;
            if (!AssetTextures.ContainsKey(assetData.AssetName))
            {
                Texture2D texture = this.HDAssetManager.LoadAsset(assetData.AssetName);
                assetTexture = new AssetTexture(assetData.AssetName, assetData.AsImage().Data, texture, this.Config.AssetScale, WhiteBoxFixAssets.Contains(assetData.AssetName));
                AssetTextures.Add(assetData.AssetName, assetTexture);
            }
            else
            {
                assetTexture = AssetTextures.GetValueSafe(assetData.AssetName);
                assetTexture.setOriginalTexture(assetData.AsImage().Data);
            }

            this.ContentPackManager.EditAsset(assetData.AssetName);

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
    }
}