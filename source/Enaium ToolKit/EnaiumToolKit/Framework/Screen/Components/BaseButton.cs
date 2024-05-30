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

public class BaseButton : Component
{
    protected BaseButton(string? title, string? description, int x, int y, int width, int height) : base(title,
        description,
        x, y, width, height)
    {
    }

    public override void Render(SpriteBatch b)
    {
        Hovered = Bounds.Contains(Game1.getMouseX(), Game1.getMouseY());
        if (Hovered)
        {
            b.DrawBoundsTexture(Bounds);
        }
    }

    public override void MouseLeftClicked(int x, int y)
    {
        Game1.playSound("drumkit6");
        base.MouseLeftClicked(x, y);
    }
}