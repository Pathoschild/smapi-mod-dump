/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A resizable nine-slice background.
    /// </summary>
    class Background : Widget
    {
        public readonly NineSlice Graphic;

        public Background(NineSlice nineSlice)
        {
            Graphic = nineSlice;
        }

        public Background(NineSlice nineSlice, int width, int height)
        {
            Graphic = nineSlice;
            Width = width;
            Height = height;
        }

        public override void Draw(SpriteBatch batch)
        {
            Graphic.Draw(batch, new Rectangle(GlobalPosition.X, GlobalPosition.Y, Width, Height));
        }
    }
}