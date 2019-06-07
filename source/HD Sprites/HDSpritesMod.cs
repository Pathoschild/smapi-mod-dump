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

namespace HDSprites
{
    public class HDSpritesMod : Mod, IAssetEditor
    {
        public static Dictionary<string, AssetTexture> AssetTextures = new Dictionary<string, AssetTexture>();        
        public static List<string> ScaleFixAssets = new List<string>()
        {
            "TileSheets\\Craftables",
            "Maps\\MenuTiles",
            "LooseSprites\\LanguageButtons",
            "LooseSprites\\chatBox",
            "LooseSprites\\textBox",
            "LooseSprites\\yellowLettersLogo"
        };
        public static List<string> WhiteBoxFixAssets = new List<string>()
        {
            "TileSheets\\tools"
        };
        public static bool EnableMod = true;

        private ModConfig Config;
        private Dictionary<string, string> AssetFiles;

        public override void Entry(IModHelper help)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.AssetFiles = new Dictionary<string, string>();

            EnableMod = this.Config.EnableMod;

            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;

            foreach (var asset in this.Config.LoadAssets)
            {
                foreach (var section in this.Config.LoadSections)
                {
                    if (asset.StartsWith(section))
                    {
                        string assetFileRel = Path.Combine(this.Config.AssetsPath, asset) + ".png";
                        string assetFileAbs = Path.Combine(help.DirectoryPath, assetFileRel);
                        if (File.Exists(assetFileAbs)) this.AssetFiles.Add(asset.Replace("/", $"\\"), assetFileRel);
                        break;
                    }
                }
            }

            HarmonyInstance instance = HarmonyInstance.Create("NinthWorld.HDSprites");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public bool CanEdit<T>(IAssetInfo assetInfo)
        {
            return this.AssetFiles.ContainsKey(assetInfo.AssetName);
        }

        public void Edit<T>(IAssetData assetData)
        {
            AssetTexture assetTexture;
            if (!AssetTextures.ContainsKey(assetData.AssetName))
            {
                Texture2D newTexture = this.Helper.Content.Load<Texture2D>(this.AssetFiles.GetValueSafe(assetData.AssetName), ContentSource.ModFolder);                
                assetTexture = new AssetTexture(assetData.AssetName, assetData.AsImage().Data, newTexture, this.Config.AssetScale, WhiteBoxFixAssets.Contains(assetData.AssetName));
                AssetTextures.Add(assetData.AssetName, assetTexture);
            }
            else
            {
                assetTexture = AssetTextures.GetValueSafe(assetData.AssetName);
            }

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
    }
}