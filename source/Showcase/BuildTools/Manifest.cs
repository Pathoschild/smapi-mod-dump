namespace Igorious.BuildManifest
{
    public class Manifest
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public ModVersion Version { get; set; }
        public string Description { get; set; }
        public string UniqueID { get; set; }
        public string EntryDll { get; set; }
    }
}