/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MoreMultiplayerInfo
{
    public class TextureHelper
    {
        static TextureHelper()
        {
            TextureHelper.WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            TextureHelper.WhitePixel.SetData(new[] { Color.White });

        }

        public static Texture2D WhitePixel;
    }
}