namespace HDSprites.ContentPack
{
    public class ContentPackFor
    {
        public string UniqueID { get; set; }
        public string MinimumVersion { get; set; }
    }

    public class ContentPackManifest
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string UniqueID { get; set; }
        public ContentPackFor ContentPackFor { get; set; }
    }
}
