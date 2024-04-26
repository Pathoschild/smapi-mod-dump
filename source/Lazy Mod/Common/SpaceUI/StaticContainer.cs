/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Common.SpaceUI;

public class StaticContainer : Container
{
    public Vector2 Size { get; set; }

    public Color? OutlineColor { get; set; } = null;

    public override int Width => (int)Size.X;

    public override int Height => (int)Size.Y;

    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        if (OutlineColor.HasValue)
        {
            IClickableMenu.drawTextureBox(b, (int)Position.X - 12, (int)Position.Y - 12, Width + 24, Height + 24, OutlineColor.Value);
        }
        base.Draw(b);
    }
}