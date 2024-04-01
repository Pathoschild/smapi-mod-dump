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

public class ModeButton : Element
{
    public List<string> modes;
    public string Current;

    public ModeButton(string title, string description) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);

        Render2DUtils.DrawButton(b, x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
        FontUtils.DrawHvCentered(b, $"{Title}:({GetCurrentIndex() + 1}/{modes.Count}){modes[GetCurrentIndex()]}",
            x + Width / 2,
            y + Height / 2);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        try
        {
            Current = modes[GetCurrentIndex() + 1];
        }
        catch (Exception e)
        {
            Current = modes.First();
        }

        base.MouseLeftClicked(x, y);
    }

    public override void MouseRightClicked(int x, int y)
    {
        try
        {
            Current = modes[GetCurrentIndex() - 1];
        }
        catch (Exception e)
        {
            Current = modes.Last();
        }

        base.MouseRightClicked(x, y);
    }

    private int GetCurrentIndex()
    {
        var index = 0;
        foreach (var variable in modes)
        {
            if (variable.Equals(Current))
            {
                return index;
            }

            index++;
        }

        return index;
    }
}