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
