/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/


namespace TMXLoader
{
    public class ContentRequest
    {
        public int type { get; set; }
        public string assetName { get; set; }
        public bool fromGameContent { get; set; }

        public ContentRequest()
        {

        }

        public ContentRequest(ContentType type, string assetName, bool fromGameContent)
        {
            this.type = (int) type;
            this.assetName = assetName;
            this.fromGameContent = fromGameContent;
        }
    }
}
