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

public class ModeButton : BaseButton
{
    public List<string> modes;
    public string Current;

    public ModeButton(string title, string description) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Render2DUtils.DrawButton(b, x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
        FontUtils.DrawHvCentered(b, $"{Title}:({GetCurrentIndex() + 1}/{modes.Count}){modes[GetCurrentIndex()]}", x, y,
            Width, Height);
        base.Render(b, x, y);
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

        Game1.playSound("drumkit6");
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

        Game1.playSound("drumkit5");
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