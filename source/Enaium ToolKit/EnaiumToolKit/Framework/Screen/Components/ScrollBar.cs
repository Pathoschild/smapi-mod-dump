/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Extensions;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Components;

public class ScrollBar : BaseButton
{
    private bool _dragging;

    public int Min { get; set; }

    public int Max { get; set; }

    public int Current { get; set; }

    [Obsolete] public Action? OnValueChanged = null;

    public Action<int>? OnCurrentChanged = null;

    public ScrollBar(int x, int y, int width, int height) : base(null, null, x, y, width, height)
    {
    }

    public override void Render(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsSlider.sliderBGSource, X, Y, Width, Height,
            Color.White, 4f, false);
        var previous = Current;

        var blockHeight = 20;

        if (_dragging)
        {
            if (Max - Min != 0)
            {
                Current = MathHelper.Clamp((Game1.getMouseY() - Y) * (Max - Min) / (Height - blockHeight) + Min,
                    Min, Max);
                OnValueChanged?.Invoke();
                OnCurrentChanged?.Invoke(Current);
                if (previous != Current)
                {
                    Game1.playSound("shiny4");
                }
            }
        }

        var sliderOffset = Height - blockHeight;

        if (Max - Min != 0)
        {
            sliderOffset = sliderOffset * (Current - Min) / (Max - Min);
        }
        else
        {
            sliderOffset = 0;
        }

        b.DrawButtonTexture(X, Y + sliderOffset, Width, blockHeight);

        base.Render(b);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        _dragging = true;
    }

    public override void MouseLeftReleased(int x, int y)
    {
        _dragging = false;
    }

    public override void LostFocus(int x, int y)
    {
        _dragging = false;
    }
}