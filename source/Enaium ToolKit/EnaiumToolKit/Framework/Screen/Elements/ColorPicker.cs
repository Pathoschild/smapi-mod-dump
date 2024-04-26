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

public class ColorPicker : Element
{
    public Color Color;

    private readonly SliderBar _red = new("Red", "", 0, byte.MaxValue);

    private readonly SliderBar _green = new("Green", "", 0, byte.MaxValue);

    private readonly SliderBar _blue = new("Blue", "", 0, byte.MaxValue);

    private readonly SliderBar _alpha = new("Alpha", "", 0, byte.MaxValue);
    
    public Action OnColorChanged = () => { };

    public ColorPicker(string title, string description, Color color) : base(title, description)
    {
        Color = color;
        _red.Current = color.R;
        _green.Current = color.G;
        _blue.Current = color.B;
        _alpha.Current = color.A;
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);
        b.Draw(Game1.staminaRect, new Rectangle(x, y, Width, Height),
            Color = new Color(_red.Current, _green.Current, _blue.Current, _alpha.Current));
        _red.Width = Width / 2;
        _red.Height = Height / 2;
        _red.Render(b, x, y);
        _green.Width = Width / 2;
        _green.Height = Height / 2;
        _green.Render(b, x + Width / 2, y);
        _blue.Width = Width / 2;
        _blue.Height = Height / 2;
        _blue.Render(b, x, y + Height / 2);
        _alpha.Width = Width / 2;
        _alpha.Height = Height / 2;
        _alpha.Render(b, x + Width / 2, y + Height / 2);
        
        if (_red.Dragging || _green.Dragging || _blue.Dragging || _alpha.Dragging)
        {
            OnColorChanged.Invoke();
        }
    }

    public override void MouseLeftClicked(int x, int y)
    {
        _red.MouseLeftClicked(x, y);
        _green.MouseLeftClicked(x, y);
        _blue.MouseLeftClicked(x, y);
        _alpha.MouseLeftClicked(x, y);
        base.MouseLeftClicked(x, y);
    }

    public override void MouseLeftReleased(int x, int y)
    {
        _red.MouseLeftReleased(x, y);
        _green.MouseLeftReleased(x, y);
        _blue.MouseLeftReleased(x, y);
        _alpha.MouseLeftReleased(x, y);
        base.MouseLeftReleased(x, y);
    }
}