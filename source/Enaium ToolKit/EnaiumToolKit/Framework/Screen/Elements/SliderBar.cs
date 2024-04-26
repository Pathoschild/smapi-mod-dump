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

public class SliderBar : BaseButton
{
    public int Current;

    private int _min;
    private int _max;

    public bool Dragging;

    public SliderBar(string title, string description, int min, int max) : base(title, description)
    {
        _min = min;
        _max = max;
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        var blockSize = 20;

        if (Hovered)
        {
            if (Dragging)
            {
                Current = (int)(_min + MathHelper.Clamp((Game1.getMouseX() - x) / (float)(Width - blockSize), 0, 1) *
                    (_max - _min));
            }
        }
        else
        {
            Dragging = false;
        }

        var sliderOffset = (Width - blockSize) * (Current - _min) / (_max - _min);
        Render2DUtils.DrawButton(b, x + sliderOffset, y, blockSize, Height, Color.Wheat);

        FontUtils.DrawHvCentered(b, $"{Title}:{Current}", x, y, Width, Height);
        base.Render(b, x, y);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        Dragging = true;
        base.MouseLeftClicked(x, y);
    }

    public override void MouseLeftReleased(int x, int y)
    {
        Dragging = false;
        base.MouseLeftReleased(x, y);
    }
}