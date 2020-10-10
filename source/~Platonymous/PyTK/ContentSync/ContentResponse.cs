/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace PyTK.ContentSync
{
    public class ContentResponse
    {
        public string assetName { get; set; }
        public string content { get; set; }
        public bool toGameContent { get; set; }
        public int type { get; set; }

        public ContentResponse()
        {

        }

        public ContentResponse(string assetName, int type, string content, bool toGameContent)
        {
            this.assetName = assetName;
            this.content = content;
            this.toGameContent = toGameContent;
            this.type = type;
        }
    }
}
