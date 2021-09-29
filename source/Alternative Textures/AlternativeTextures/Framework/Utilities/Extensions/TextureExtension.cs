/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeTextures.Framework.Utilities.Extensions
{
    public static class TextureExtension
    {
        public static Texture2D CreateSelectiveCopy(this Texture2D sourceTexture, GraphicsDevice device, Rectangle selectionRect)
        {
            Texture2D selectiveTexture = new Texture2D(device, selectionRect.Width, selectionRect.Height);
            try
            {

                int dimensions = selectionRect.Width * selectionRect.Height;
                Color[] data = new Color[dimensions];

                sourceTexture.GetData(0, selectionRect, data, 0, dimensions);
                selectiveTexture.SetData(data);
            }
            catch
            {
                AlternativeTextures.monitor.Log("Issue creating a selective texture copy, likely out of bounds for dimensions.", StardewModdingAPI.LogLevel.Trace);
            }

            return selectiveTexture;
        }
    }
}
