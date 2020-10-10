/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public sealed class IconMenuCell : MenuCell
    {
        public IconMenuCell(int row, int column, Aligment aligment, Texture2D texture, Rectangle sourceRect, int width, int? height = null, bool shadow = false) : base(row, column, aligment)
        {
            Width = width;
            Height = height ?? width;
            Draw = r => Game1.spriteBatch.Draw(
                texture,
                r,
                sourceRect,
                shadow? (Color.Black * 0.3f) : Color.White);
        }
    }
}