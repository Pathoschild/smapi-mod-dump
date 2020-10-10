/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

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
