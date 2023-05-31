/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities.Extensions
{
    public static class Texture2DExtensions
    {
        public static Color GetMiddlePixelFromParentSheetIndex(Texture2D texture, int tilePosition, int spriteWidth, int spriteHeight)
        {
            return GetPixelFromParentSheetIndex(texture, tilePosition, spriteWidth / 2, spriteHeight / 2,spriteWidth,spriteHeight);
        }

        public static Color GetPixelFromParentSheetIndex(Texture2D texture, int tilePosition, int xPixelOffset, int yPixelOffset ,int spriteWidth, int spriteHeight)
        {
            Color[] colors = texture.GetPixels();
            Rectangle pos= Game1.getSourceRectForStandardTileSheet(texture, tilePosition, spriteWidth, spriteHeight);
            return GetPixel(colors, pos.X + xPixelOffset, pos.Y + yPixelOffset, texture.Width);
        }

        public static Color GetPixel(this Color[] colors, int xPixel, int yPixel, int textureWidth)
        {
            return colors[xPixel + (yPixel * textureWidth)];
        }
        public static Color[] GetPixels(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors1D);
            return colors1D;
        }
    }
}
