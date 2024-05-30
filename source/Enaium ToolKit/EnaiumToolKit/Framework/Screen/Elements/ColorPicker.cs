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

namespace EnaiumToolKit.Framework.Screen.Elements;

public class ColorPicker : Element
{
    [Obsolete] public Color Color;

    public Color Current
    {
        get => Color;
        set => Color = value;
    }

    private readonly SliderBar _red = new("Red", null, 0, byte.MaxValue);

    private readonly SliderBar _green = new("Green", null, 0, byte.MaxValue);

    private readonly SliderBar _blue = new("Blue", null, 0, byte.MaxValue);

    private readonly SliderBar _alpha = new("Alpha", null, 0, byte.MaxValue);

    [Obsolete] public Action? OnColorChanged = null;

    public Action<Color>? OnCurrentChanged = null;

    public ColorPicker(string title, string? description, Color color) : base(title, description)
    {
        Color = color;
        _red.Current = color.R;
        _green.Current = color.G;
        _blue.Current = color.B;
        _alpha.Current = color.A;
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        b.Draw(Game1.staminaRect, new Rectangle(x, y, Width, Height),
            Color = new Color(_red.Current, _green.Current, _blue.Current, _alpha.Current));
        _red.Width = Width / 2;
        _red.Height = Height / 2;
        _red.Render(b, x, y);
        _red.Hovered = _red.Hovered && Hovered;
        _green.Width = Width / 2;
        _green.Height = Height / 2;
        _green.Render(b, x + Width / 2, y);
        _green.Hovered = _green.Hovered && Hovered;
        _blue.Width = Width / 2;
        _blue.Height = Height / 2;
        _blue.Render(b, x, y + Height / 2);
        _blue.Hovered = _blue.Hovered && Hovered;
        _alpha.Width = Width / 2;
        _alpha.Height = Height / 2;
        _alpha.Render(b, x + Width / 2, y + Height / 2);
        _alpha.Hovered = _alpha.Hovered && Hovered;

        var change = (int i) =>
        {
            OnColorChanged?.Invoke();
            OnCurrentChanged?.Invoke(Current);
        };

        _red.OnCurrentChanged = change;
        _green.OnCurrentChanged = change;
        _blue.OnCurrentChanged = change;
        _alpha.OnCurrentChanged = change;
        base.Render(b, x, y);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        if (_red.Hovered)
        {
            _red.MouseLeftClicked(x, y);
            _green.LostFocus(x, y);
            _blue.LostFocus(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_green.Hovered)
        {
            _red.LostFocus(x, y);
            _green.MouseLeftClicked(x, y);
            _blue.LostFocus(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_blue.Hovered)
        {
            _red.LostFocus(x, y);
            _green.LostFocus(x, y);
            _blue.MouseLeftClicked(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_alpha.Hovered)
        {
            _red.LostFocus(x, y);
            _green.LostFocus(x, y);
            _blue.LostFocus(x, y);
            _alpha.MouseLeftClicked(x, y);
        }

        base.MouseLeftClicked(x, y);
    }

    public override void MouseLeftReleased(int x, int y)
    {
        if (_red.Hovered)
        {
            _red.MouseLeftReleased(x, y);
            _green.LostFocus(x, y);
            _blue.LostFocus(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_green.Hovered)
        {
            _red.LostFocus(x, y);
            _green.MouseLeftReleased(x, y);
            _blue.LostFocus(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_blue.Hovered)
        {
            _red.LostFocus(x, y);
            _green.LostFocus(x, y);
            _blue.MouseLeftReleased(x, y);
            _alpha.LostFocus(x, y);
        }

        if (_alpha.Hovered)
        {
            _red.LostFocus(x, y);
            _green.LostFocus(x, y);
            _blue.LostFocus(x, y);
            _alpha.MouseLeftReleased(x, y);
        }

        base.MouseLeftReleased(x, y);
    }

    public override void LostFocus(int x, int y)
    {
        _red.LostFocus(x, y);
        _green.LostFocus(x, y);
        _blue.LostFocus(x, y);
        _alpha.LostFocus(x, y);
        base.LostFocus(x, y);
    }
}