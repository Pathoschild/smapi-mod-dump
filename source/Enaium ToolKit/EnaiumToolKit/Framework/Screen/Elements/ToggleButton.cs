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

namespace EnaiumToolKit.Framework.Screen.Elements;

public class ToggleButton : BaseButton
{
    [Obsolete] public bool Toggled;

    public bool Current
    {
        get => Toggled;
        set => Toggled = value;
    }

    public Action<bool>? OnCurrentChanged = null;

    public ToggleButton(string title, string? description = null) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        var color = Toggled ? Color.Green : Color.Red;
        b.DrawButtonTexture(x, y, Width, Height, color);
        b.DrawStringCenter(Title!, x, y, Width, Height);
        base.Render(b, x, y);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        Toggled = !Toggled;
        OnCurrentChanged?.Invoke(Toggled);
        if (!Toggled)
        {
            Game1.playSound("drumkit5");
        }

        base.MouseLeftClicked(x, y);
    }
}