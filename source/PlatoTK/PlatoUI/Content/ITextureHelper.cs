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
using Microsoft.Xna.Framework.Graphics;

namespace PlatoUI.Content
{
    public interface ITextureHelper
    {
        Texture2D WhitePixel { get; }

        Texture2D WhiteCircle { get; }

        Texture2D GetCircle(int radius, Color color);

        Texture2D GetRectangle(int width, int height, Color color);

        Texture2D ExtractArea(Texture2D texture, Rectangle targetArea);

        Texture2D ExtractTile(Texture2D texture, int index, int tileWidth = 16, int tileHeight = 16);

        Texture2D GetPatched(Texture2D texture, Point target, Texture2D patch, Rectangle? sourceArea = null);

        Texture2D GetTrimed(Texture2D texture);

        Texture2D ResizeTexture(Texture2D texture, int width, int height);

        bool TryParseColorFromString(string value, out Color color);

        string ColorToString(Color color);
    }
}
