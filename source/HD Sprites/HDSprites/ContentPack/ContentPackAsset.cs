using HDSprites.Token;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace HDSprites.ContentPack
{
    public class ContentPackAsset
    {
        private ContentPackManager Manager { get; set; }
        public ContentPackObject ContentPack { get; set; }
        public StringWithTokens Target { get; set; }
        public StringWithTokens File { get; set; }
        public List<TokenEntry> When { get; set; }
        public Rectangle FromArea { get; set; }
        public Rectangle ToArea { get; set; }
        public bool Overlay { get; set; }

        public ContentPackAsset(ContentPackManager manager, ContentPackObject contentPack, StringWithTokens target, StringWithTokens file, List<TokenEntry> when, Rectangle fromArea, Rectangle toArea, bool overlay)
        {
            this.Manager = manager;
            this.ContentPack = contentPack;
            this.Target = target;
            this.File = file;
            this.When = when;
            this.Overlay = overlay;
            this.FromArea = fromArea;
            this.ToArea = toArea;

            foreach (string token in this.Target.GetTokenNames())
            {
                this.Manager.DynamicTokenManager.RegisterAsset(token, this);
            }

            foreach (string token in this.File.GetTokenNames())
            {
                this.Manager.DynamicTokenManager.RegisterAsset(token, this);
            }

            foreach (var entry in this.When)
            {
                this.Manager.DynamicTokenManager.RegisterAsset(entry.Name, this);
            }
        }

        public string GetTarget()
        {
            return this.Target.Parse(this.Manager.DynamicTokenManager.DynamicTokens).ToCleanString();
        }

        public string GetFile()
        {
            return this.File.Parse(this.Manager.DynamicTokenManager.DynamicTokens, this.GetTarget()).ToCleanString();
        }

        public bool IsPartial()
        {
            return !this.FromArea.IsEmpty || !this.ToArea.IsEmpty || this.Overlay;
        }

        public bool IsEnabled()
        {
            foreach (var entry in this.When)
            {
                if (!entry.IsEnabled(this.Manager.DynamicTokenManager))
                {
                    return false;
                }
            }
            return true;
        }

        public void Update()
        {
            this.Manager.UpdateAsset(this);
        }
    }
}
