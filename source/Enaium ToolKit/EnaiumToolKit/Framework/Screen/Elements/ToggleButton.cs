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
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Elements;

public class ToggleButton : BaseButton
{
    public bool Toggled;

    public ToggleButton(string title, string description) : base(title, description)
    {
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        var color = Toggled ? Color.Green : Color.Red;
        Render2DUtils.DrawButton(b, x, y, Width, Height, color);
        FontUtils.DrawHvCentered(b, Title, x, y, Width, Height);
        base.Render(b, x, y);
    }

    public override void MouseLeftClicked(int x, int y)
    {
        Toggled = !Toggled;
        Game1.playSound(Toggled ? "drumkit6" : "drumkit5");
        base.MouseLeftClicked(x, y);
    }
}