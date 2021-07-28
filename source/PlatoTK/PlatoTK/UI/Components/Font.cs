/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using BmFont;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlatoTK.UI.Components
{
    public class Font
    {
        public virtual FontFile FontFile { get; }

        public virtual Dictionary<char, FontChar> CharacterMap { get; }

        public virtual List<Texture2D> FontPages { get;}

        public Font(IPlatoHelper helper, string assetName)
        {
            FontFile = helper.UI.LoadFontFile(assetName);
            CharacterMap = helper.UI.ParseCharacterMap(FontFile);
            FontPages = helper.UI.LoadFontPages(FontFile, assetName);
        }
    }
}
