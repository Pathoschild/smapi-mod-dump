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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EnaiumToolKit.Framework.Screen.Elements;

public class CheckBox : Element
{
    public bool Current;
    public Action<bool>? OnCurrentChanged = null;
    private bool _hovered;

    public CheckBox(string title, string? description = null) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        var textureX = x + Width - 36;
        var textureY = y + Height / 2 - 36 / 2;
        _hovered = new Rectangle(textureX, textureY, 36, 36).Contains(Game1.getMouseX(), Game1.getMouseY());
        b.DrawStringVCenter(Title!, x, y, Height);
        b.DrawCheckboxTexture(textureX, textureY, Current);
        base.Render(b, x, y);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        if (_hovered)
        {
            Current = !Current;
            Game1.playSound("drumkit6");
            OnCurrentChanged?.Invoke(Current);
            base.MouseLeftClicked(x, y);
        }
    }
}