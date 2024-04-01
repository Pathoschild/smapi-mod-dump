/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EnaiumToolKit.Framework.Screen.Elements;

public class ValueButton : Element
{
    public int Current;
    public int Min;
    public int Max;

    public ValueButton(string title, string description) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);

        Render2DUtils.DrawButton(b, x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
        FontUtils.DrawHvCentered(b, $"{Title}:({Min}-{Max}){Current}", x + Width / 2, y + Height / 2);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        if (Current < Max)
        {
            Current += 1;
        }
        else
        {
            Current = Min;
        }

        base.MouseLeftClicked(x, y);
    }

    public override void MouseRightClicked(int x, int y)
    {
        if (Current > Min)
        {
            Current -= 1;
        }
        else
        {
            Current = Max;
        }

        base.MouseRightClicked(x, y);
    }
}