namespace HDSprites.ContentPack
{
    public class ContentPackObject
    {
        public string ModPath { get; set; }
        public string ContentPath { get; set; }
        public ContentPackManifest Manifest { get; set; }
        public ContentConfig Content { get; set; }
        public WhenDictionary Config { get; set; }

        public ContentPackObject(string modPath, string contentPath, ContentPackManifest manifest, ContentConfig content, WhenDictionary config)
        {
            this.ModPath = modPath;
            this.ContentPath = contentPath;
            this.Manifest = manifest;
            this.Content = content;
            this.Config = config;
        }
    }
}
