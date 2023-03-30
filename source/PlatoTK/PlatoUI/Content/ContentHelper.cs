/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace PlatoUI.Content
{
    internal class ContentHelper : InnerHelper, IContentHelper
    {
        public ITextureHelper Textures { get; }

        public ContentHelper(IPlatoUIHelper helper)
            : base(helper)
        {
            Textures = new TextureHelper(helper);
        }

    }
}
