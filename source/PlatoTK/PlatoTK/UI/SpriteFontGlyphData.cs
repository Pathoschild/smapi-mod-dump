/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;

namespace PlatoTK.UI
{
    internal class SpriteFontGlyphData
    {
        public Rectangle BoundsInTexture { get; set; }
        public Rectangle Cropping { get; set; }
        public char Character { get; set; }

        public float LeftSideBearing { get; set; }
        public float Width { get; set; }
        public float RightSideBearing { get; set; }
    }
}
