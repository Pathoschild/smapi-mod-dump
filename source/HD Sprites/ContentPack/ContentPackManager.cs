using HDSprites.Token;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace HDSprites.ContentPack
{
    public class ContentPackManager
    {
        private List<ContentPackAsset> ContentPackAssets { get; set; }
        private Dictionary<string, Texture2D> ContentPackTextures { get; set; }
        private HDAssetManager HDAssetManager { get; set; }
        public DynamicTokenManager DynamicTokenManager { get; set; }

        public ContentPackManager(HDAssetManager hdAssetManager)
        {
            this.ContentPackAssets = new List<ContentPackAsset>();
            this.ContentPackTextures = new Dictionary<string, Texture2D>();
            this.HDAssetManager = hdAssetManager;
            this.DynamicTokenManager = new DynamicTokenManager();
        }

        public void AddContentPack(ContentPackObject contentPack)
        {
            Dictionary<string, DynamicToken> configTokens = new Dictionary<string, DynamicToken>();
            foreach (var entry in contentPack.Config)
            {
                DynamicToken token = new DynamicToken(entry.Key);
                token.AddValue(new TokenValue(entry.Value, true));
                configTokens.Add(entry.Key, token);
            }
            
            if (contentPack.Config.Count < 1)
            {
                foreach (var token in contentPack.Content.ConfigSchema)
                {
                    if (token.Value.Default != null)
                    {
                        contentPack.Config.Add(token.Key, token.Value.Default);
                    }
                    else if (token.Value.AllowValues != null)
                    {
                        contentPack.Config.Add(token.Key, token.Value.AllowValues.Split(',')[0]);                        
                    }
                }
            }

            foreach (var dynamicToken in contentPack.Content.DynamicTokens)
            {
                if (!this.DynamicTokenManager.DynamicTokens.TryGetValue(dynamicToken.Name, out var token)) {
                    token = new DynamicToken(dynamicToken.Name);
                    this.DynamicTokenManager.AddToken(token);
                }

                List<TokenEntry> parsedWhen = new List<TokenEntry>();
                foreach (var entry in dynamicToken.When)
                {
                    parsedWhen.Add(new TokenEntry(entry.Key, entry.Value));
                }

                List<TokenEntry> when = new List<TokenEntry>();

                bool enabled = true;
                foreach (var entry in parsedWhen)
                {
                    if (contentPack.Config.TryGetValue(entry.Name, out var value))
                    {
                        if ((entry.IsConditional && entry.Condition.RawString.Equals(value) != value.ToLower().Equals("true"))
                            || !entry.Values.Contains(value))
                        {
                            enabled = false;
                            break;
                        }
                    }
                    else
                    {
                        when.Add(entry);
                    }
                }

                token.AddValue(new DynamicTokenValue(dynamicToken.Value, enabled, this.DynamicTokenManager, when));
            }

            foreach (var change in contentPack.Content.Changes)
            {
                if (!(change.Action.Equals("Load") || change.Action.Equals("EditImage")) 
                    || change.Enabled.ToLower().Equals("false")) continue;

                List<TokenEntry> parsedWhen = new List<TokenEntry>();
                foreach (var entry in change.When)
                {
                    parsedWhen.Add(new TokenEntry(entry.Key, entry.Value));
                }

                List<TokenEntry> when = new List<TokenEntry>();

                bool enabled = true;
                foreach (var entry in parsedWhen)
                {
                    if (contentPack.Config.TryGetValue(entry.Name, out var value))
                    {
                        if ((entry.IsConditional && entry.Condition.Parse(this.DynamicTokenManager.DynamicTokens).Equals(value) != value.ToLower().Equals("true")) 
                            || !entry.Values.Contains(value))
                        {
                            enabled = false;
                            break;
                        }
                    }
                    else
                    {
                        when.Add(entry);
                    }
                }
                if (!enabled) continue;

                string[] targetSplit = change.Target.Split(',');
                foreach (string targetStr in targetSplit)
                {
                    string targetStrFix = targetStr;
                    if (targetStr.StartsWith(" ")) targetStrFix = targetStr.Substring(1);

                    StringWithTokens target = new StringWithTokens(targetStrFix.Replace("/", $"\\")).Parse(configTokens);
                    StringWithTokens file = new StringWithTokens(change.FromFile).Parse(configTokens);
                    bool overlay = change.Patchmode.ToLower().Equals("overlay");

                    Rectangle fromArea = new Rectangle(change.FromArea.X * 2, change.FromArea.Y * 2, change.FromArea.Width * 2, change.FromArea.Height * 2);
                    Rectangle toArea = new Rectangle(change.ToArea.X * 2, change.ToArea.Y * 2, change.ToArea.Width * 2, change.ToArea.Height * 2);

                    ContentPackAsset asset = new ContentPackAsset(this, contentPack, target, file, when, fromArea, toArea, overlay);

                    this.ContentPackAssets.Add(asset);
                }
            }
        }

        public void EditAsset(string assetName)
        {
            foreach (ContentPackAsset asset in this.ContentPackAssets)
            {
                if (asset.GetTarget().Equals(assetName))
                {
                    this.UpdateAsset(asset);
                }
            }
        }

        public void UpdateAsset(ContentPackAsset asset)
        {
            string assetName = asset.GetTarget();

            if (assetName.Equals("Characters\\Haley"))
            {

            }

            if (HDSpritesMod.AssetTextures.TryGetValue(assetName, out var assetTexture))
            {
                bool enabled = asset.IsEnabled();

                /*
                if (!enabled && !asset.IsPartial())
                {
                    assetTexture.HDTexture = this.HDAssetManager.LoadAsset(assetName);
                }
                */

                if (enabled)
                {
                    Texture2D texture = this.LoadTexture(asset.GetFile(), asset.ContentPack);

                    if (!asset.IsPartial() && texture != null)
                    {
                        assetTexture.HDTexture = texture;
                    }
                    else
                    {
                        assetTexture.setSubTexture(texture, asset.FromArea, asset.ToArea, asset.Overlay);
                    }
                }

                if (!asset.IsPartial())
                {
                    foreach (var contentPack in this.ContentPackAssets)
                    {
                        if (contentPack.GetTarget().Equals(assetName)
                            && contentPack.ContentPack.Equals(asset.ContentPack)
                            && contentPack.IsPartial()
                            && contentPack.IsEnabled())
                        {
                            Texture2D partialTexture = this.LoadTexture(contentPack.GetFile(), contentPack.ContentPack);
                            if (partialTexture != null)
                            {
                                assetTexture.setSubTexture(partialTexture, contentPack.FromArea, contentPack.ToArea, contentPack.Overlay);
                            }
                        }
                    }
                }
            }
        }

        public Texture2D LoadTexture(string file, ContentPackObject contentPack)
        {
            if (!this.ContentPackTextures.TryGetValue(file, out Texture2D texture))
            {
                GraphicsDevice device = HDSpritesMod.GraphicsDevice;
                if (device == null) return null;

                string modFile = Path.Combine(contentPack.ModPath, file);
                string contentFile = Path.Combine(contentPack.ContentPath, file);
                
                if (File.Exists(contentFile))
                {
                    FileStream inStream = new FileStream(contentFile, FileMode.Open);
                    texture = Texture2D.FromStream(device, inStream);
                }
                else
                {
                    if (!File.Exists(modFile)) return null;

                    FileStream inStream = new FileStream(modFile, FileMode.Open);
                    Texture2D modTexture = Texture2D.FromStream(device, inStream);

                    texture = Upscaler.Upscale(modTexture);

                    Directory.CreateDirectory(contentFile.Substring(0, contentFile.Replace("/", "\\").LastIndexOf("\\")));

                    FileStream outStream = new FileStream(contentFile, FileMode.Create);
                    texture.SaveAsPng(outStream, texture.Width, texture.Height);
                }

                this.ContentPackTextures.Add(file, texture);
            }
            return texture;
        }
    }
}
